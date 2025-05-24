using Core.Helper;
using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreCartesianAxis : CoreAxis
{
    private Paint? _tickPaint;
    private Paint? _subTickPaint;
    private Paint? _axisLinePaint;
    private Paint? _separatorPaint;
    private Paint? _subSeparatorPaint;

    public AxisOrientation Orientation { get; protected internal set; }
    public AxisPosition Position { get; set; }

    public float X { get; protected internal set; }
    public float Y { get; protected internal set; }

    public int SeparatorCount { get; set; } = 4;
    public float TickLength { get; set; } = 5f;
    public float LabelDensity { get; set; } = 0.85f;

    public Paint? TickPaint
    {
        get => _tickPaint;
        set
        {
            if (value != _tickPaint)
            {
                _tickPaint = value;
            }
        }
    }

    public Paint? AxisLinePaint
    {
        get => _axisLinePaint;
        set
        {
            if (value != _axisLinePaint)
            {
                _axisLinePaint = value;
            }
        }
    }

    public Paint? SubTickPaint
    {
        get => _subTickPaint;
        set
        {
            if (value != _subTickPaint)
            {
                _subTickPaint = value;
            }
        }
    }

    public Paint? SeparatorPaint
    {
        get => _separatorPaint;
        set
        {
            if (value != _separatorPaint)
            {
                _separatorPaint = value;
            }
        }
    }

    public Paint? SubSeparatorPaint
    {
        get => _subSeparatorPaint;
        set
        {
            if (value != _subSeparatorPaint)
            {
                _subSeparatorPaint = value;
            }
        }
    }

    public void Reset(AxisOrientation orientation)
    {
        Orientation = orientation;

        if (!Extensions.Validate(Min)) Min = -10d;
        if (!Extensions.Validate(Max)) Max = 10d;
    }

    public void SetBound(double min, double max)
    {
        if (Min > min) Min = min;
        if (Max < max) Max = max;
    }
}

public abstract class CoreCartesianAxis<TLabelGeometry, TLineGeometry> : CoreCartesianAxis
    where TLabelGeometry : BaseLabelGeometry, new()
    where TLineGeometry : BaseLineGeometry, new()
{
    private TLabelGeometry? _nameGeometry;
    private TLineGeometry? _tickPath;

    private readonly Dictionary<string, AxisVisual> _cached = [];

    public override void Invalidate(CoreChart chart)
    {
        var controlSize = chart.ControlSize;

        var drawLocation = chart.DrawnLocation;
        var drawSize = chart.DrawnSize;

        bool flip = Orientation == AxisOrientation.Y;
        float start = flip ? LabelDesiredRect.Y : LabelDesiredRect.X;
        float end = flip
            ? LabelDesiredRect.Y + NameDesiredRect.Height
            : LabelDesiredRect.X + NameDesiredRect.Width;

        var scaler = new Scaler(flip, start, end, Min, Max);

        float lxi = drawLocation.X;
        float lxj = drawLocation.X + drawSize.Width;
        float lyi = drawLocation.Y;
        float lyj = drawLocation.Y + drawSize.Height;

        var g = new HashSet<AxisVisual>();

        float xo = 0f, yo = 0f;
        if (Orientation == AxisOrientation.X)
        {
            yo = Position == AxisPosition.Start
                ? controlSize.Height - Y
                : Y;
        }
        else
        {
            xo = Position == AxisPosition.Start
                ? X
                : controlSize.Width - X;
        }

        DrawAxisName(chart);

        DrawAxisLine(chart, xo, yo);

        double step = this.GetIdealStep(NameDesiredRect.Size);

        var startOffset = Math.Floor(Min / step) * step;
        var labeler = GetActualLabeler();

        foreach (var i in Extensions.EnumerateSeparators(startOffset, Max, step))
        {
            if (i < Min || i > Max) continue;

            string label = labeler(i);

            float x, y;

            if (Orientation == AxisOrientation.X)
            {
                x = scaler.ToPixel(i);
                y = yo;
            }
            else
            {
                x = xo;
                y = scaler.ToPixel(i);
            }

            if (!_cached.TryGetValue(label, out var axisVisual))
            {
                axisVisual = new AxisVisual() { Value = i };
                _cached[label] = axisVisual;
            }


            DrawAxisTick(chart, step, scaler, lxi, lxj, lyi, lyj, x, y, axisVisual);
            DrawAxisSubTick(chart, step, scaler, lxi, lxj, lyi, lyj, x, y, axisVisual);

            DrawAxisLabel(chart, label, x, y, axisVisual);

            DrawAxisSeparator(chart, lxi, lxj, lyi, lyj, x, y, axisVisual);
            DrawAxisSubSeparator(chart, step, scaler, lxi, lxj, lyi, lyj, x, y, axisVisual);

            _ = g.Add(axisVisual);
        }

        #region Remove old

        foreach (var item in _cached.ToArray())
        {
            var axisVisual = item.Value;
            if (g.Contains(axisVisual)) continue;

            float x, y;
            if (Orientation == AxisOrientation.X)
            {
                x = scaler.ToPixel(axisVisual.Value);
                y = yo;
            }
            else
            {
                x = xo;
                y = scaler.ToPixel(axisVisual.Value);
            }

            if (axisVisual.Separator is not null)
            {
                UpdateSeparator(lxi, lxj, lyi, lyj, x, y, axisVisual.Separator, VisualState.Remove);
            }

            if (axisVisual.SubSeparator is not null)
            {
                UpdateSubSeparator(axisVisual.SubSeparator, scaler, step, x, y,
                   lxi, lxj, lyi, lyj, VisualState.Remove);
            }

            if (axisVisual.Label is not null)
            {
                UpdateLabel(labeler(axisVisual.Value), x, y, axisVisual.Label, VisualState.Remove);
            }

            if (axisVisual.Tick is not null)
            {
                UpdateTick(TickLength, x, y, axisVisual.Tick, VisualState.Remove);
            }

            if (axisVisual.SubTick is not null)
            {
                UpdateSubticks(axisVisual.SubTick, scaler, step, x, y, lxi, lxj, lyi, lyj, VisualState.Remove);
            }

            _ = _cached.Remove(item.Key);
        }

        #endregion
    }

    private TLineGeometry CreateLineGeometry()
    {
        var g = new TLineGeometry();
        g.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
        return g;
    }

    private TLabelGeometry CreateLabelGeometry()
    {
        var g = new TLabelGeometry();
        g.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
        return g;
    }

    private void DrawAxisTick(CoreChart chart, double step, Scaler scaler, float lxi, float lxj, float lyi, float lyj, float x, float y, AxisVisual axisVisual)
    {
        if (TickPaint is null) return;

        axisVisual.Tick ??= CreateLineGeometry();

        UpdateTick(TickLength, x, y, axisVisual.Tick, VisualState.Display);

        chart.CanvasContext.AddDrawnTask(TickPaint, axisVisual.Tick);

    }

    private void DrawAxisSubTick(CoreChart chart, double step, Scaler scaler, float lxi, float lxj, float lyi, float lyj, float x, float y, AxisVisual axisVisual)
    {
        if (SubTickPaint is null) return;

        if (axisVisual.SubTick is null)
        {
            axisVisual.SubTick = new TLineGeometry[SeparatorCount];

            for (var j = 0; j < SeparatorCount; j++)
            {
                axisVisual.SubTick[j] = CreateLineGeometry();
            }
        }

        UpdateSubticks(axisVisual.SubTick, scaler, step, x, y, lxi, lxj, lyi, lyj, VisualState.Display);

        for (var j = 0; j < SeparatorCount; j++)
        {
            chart.CanvasContext.AddDrawnTask(SubTickPaint, axisVisual.SubTick[j]);
        }
    }

    private void DrawAxisSeparator(CoreChart chart, float lxi, float lxj, float lyi, float lyj, float x, float y, AxisVisual axisVisual)
    {
        if (SeparatorPaint is null) return;

        axisVisual.Separator ??= CreateLineGeometry();

        UpdateSeparator(lxi, lxj, lyi, lyj, x, y, axisVisual.Separator, VisualState.Display);

        chart.CanvasContext.AddDrawnTask(SeparatorPaint, axisVisual.Separator);
    }

    private void DrawAxisSubSeparator(CoreChart chart, double step, Scaler scaler, float lxi, float lxj, float lyi, float lyj, float x, float y, AxisVisual axisVisual)
    {
        if (SubSeparatorPaint is null) return;

        if (axisVisual.SubSeparator is null)
        {
            axisVisual.SubSeparator = new TLineGeometry[SeparatorCount];

            for (var j = 0; j < SeparatorCount; j++)
            {
                axisVisual.SubSeparator[j] = CreateLineGeometry();
            }
        }

        UpdateSubSeparator(axisVisual.SubSeparator, scaler, step, x, y,
            lxi, lxj, lyi, lyj, VisualState.Display);

        for (var j = 0; j < SeparatorCount; j++)
        {
            chart.CanvasContext.AddDrawnTask(SubSeparatorPaint, axisVisual.SubSeparator[j]);
        }
    }

    private void DrawAxisLabel(CoreChart chart, string label, float x, float y, AxisVisual axisVisual)
    {
        if (LabelPaint is null) return;

        axisVisual.Label ??= CreateLabelGeometry();

        chart.CanvasContext.AddDrawnTask(LabelPaint, axisVisual.Label);


        UpdateLabel(label, x, y, axisVisual.Label, VisualState.Display);
    }

    private void DrawAxisName(CoreChart chart)
    {
        if (NamePaint is null || Name is null) return;

        _nameGeometry ??= CreateLabelGeometry();

        chart.CanvasContext.AddDrawnTask(NamePaint, _nameGeometry);

        _nameGeometry.Text = Name;
        _nameGeometry.TextSize = NameSize;
        _nameGeometry.Paint = NamePaint;
        _nameGeometry.RotateTransform = NameRotation;
        _nameGeometry.Padding = NamePadding;
        _nameGeometry.X = NameDesiredRect.X + NameDesiredRect.Width * 0.5f;
        _nameGeometry.Y = NameDesiredRect.Y + NameDesiredRect.Height * 0.5f;
    }

    private void DrawAxisLine(CoreChart chart, float xo, float yo)
    {
        if (AxisLinePaint is null) return;

        _tickPath ??= CreateLineGeometry();

        chart.CanvasContext.AddDrawnTask(TickPaint!, _tickPath);

        if (Orientation == AxisOrientation.X)
        {
            var yp = yo + (LabelDesiredRect.Height * 0.5f + TickLength) * (Position == AxisPosition.Start ? -1 : 1);
            _tickPath.X = LabelDesiredRect.Location.X;
            _tickPath.X1 = LabelDesiredRect.Location.X + NameDesiredRect.Size.Width;
            _tickPath.Y = yp;
            _tickPath.Y1 = yp;
        }
        else
        {
            var xp = xo + (LabelDesiredRect.Width * 0.5f + TickLength) * (Position == AxisPosition.Start ? 1 : -1);
            _tickPath.X = xp;
            _tickPath.X1 = xp;
            _tickPath.Y = LabelDesiredRect.Location.Y;
            _tickPath.Y1 = LabelDesiredRect.Location.Y + NameDesiredRect.Size.Height;
        }
    }

    private Func<double, string> GetActualLabeler()
    {
        return Labeler ??= t => t.ToString("N10");
    }

    public override Size MeasureNameLabelSize()
    {
        if (string.IsNullOrWhiteSpace(Name) || NamePaint is null)
            return new Size(0f, 0f);

        var _nameGeometry = new TLabelGeometry
        {
            Text = Name,
            TextSize = NameSize,
            Paint = NamePaint,
            RotateTransform = NameRotation,
            Padding = NamePadding
        };

        // TODO: 性能
        return _nameGeometry.Measure();
    }

    public override Size MeasureMaxLabelSize()
    {
        if (LabelPaint is null || Max == Min)
            return new Size(0f, 0f);

        const double testSeparators = 25;
        double step = (Max - Min) / testSeparators;

        return MeasureLabelInternal(Extensions.EnumerateSeparators(Min, Max, step));
    }

    public override Size MeasureLabelSize(Size size)
    {
        if (LabelPaint is null)
            return new Size(0f, 0f);

        double step = this.GetIdealStep(size);

        var start = Math.Floor(Min / step) * step;

        return MeasureLabelInternal(Extensions.EnumerateSeparators(start, Max, step));
    }

    private Size MeasureLabelInternal(IEnumerable<double> separatorGenerator)
    {
        var labeler = GetActualLabeler();

        float w = 0f, h = 0f;

        TLabelGeometry geometry = new();
        foreach (var i in separatorGenerator)
        {
            geometry.Text = labeler(i);
            geometry.TextSize = LabelSize;
            geometry.RotateTransform = LabelRotation;
            geometry.Padding = LabelPadding;
            geometry.Paint = LabelPaint;

            var m = geometry.Measure();

            if (m.Width > w) w = m.Width;
            if (m.Height > h) h = m.Height;
        }

        return new Size(w, h);
    }


    #region Update Visual

    private void UpdateSubSeparator(
        BaseLineGeometry[] subSeparators,
        Scaler scaler,
        double step,
        float x,
        float y,
        float lxi,
        float lxj,
        float lyi,
        float lyj,
        VisualState visualState)
    {
        for (var j = 0; j < subSeparators.Length; j++)
        {
            var subSeparator = subSeparators[j];

            var kl = GetSubStepFactor(j);

            float xs = 0f, ys = 0f;
            if (Orientation == AxisOrientation.X)
            {
                xs = scaler.MeasureInPixels(step * kl);
                if (x + xs >= lxj || x + xs <= lxi)
                {
                    visualState = VisualState.Remove;
                }
            }
            else
            {
                ys = scaler.MeasureInPixels(step * kl);
                if (y - ys >= lyj || y - ys <= lyi)
                {
                    visualState = VisualState.Remove;
                }
            }

            UpdateSeparator(lxi, lxj, lyi, lyj, x + xs, y - ys, subSeparator, visualState);
        }
    }

    private void UpdateSeparator(
        float lxi,
        float lxj,
        float lyi,
        float lyj,
        float x,
        float y,
        BaseLineGeometry geometry,
        VisualState visualState)
    {
        float x1, x2, y1, y2;
        if (Orientation == AxisOrientation.X)
        {
            x1 = x;
            x2 = x;
            y1 = lyi;
            y2 = lyj;
        }
        else
        {
            y1 = y;
            y2 = y;
            x1 = lxi;
            x2 = lxj;
        }

        geometry.Paint = TickPaint;
        geometry.X = x1;
        geometry.Y = y1;
        geometry.X1 = x2;
        geometry.Y1 = y2;

        ChangeVisualState(geometry, visualState);
    }

    private void UpdateLabel(
        string label,
        float x,
        float y,
        BaseLabelGeometry geometry,
        VisualState visualState)
    {
        geometry.Text = label;
        geometry.TextSize = LabelSize;
        geometry.Paint = LabelPaint;
        geometry.Padding = LabelPadding;
        geometry.RotateTransform = LabelRotation;
        geometry.X = x;
        geometry.Y = y;

        ChangeVisualState(geometry, visualState);
    }

    private void UpdateSubticks(
        BaseLineGeometry[] subticks,
        Scaler scaler,
        double step,
        float x,
        float y,
        float lxi,
        float lxj,
        float lyi,
        float lyj,
        VisualState visualState)
    {
        for (var j = 0; j < subticks.Length; j++)
        {
            var subtick = subticks[j];
            double kl = GetSubStepFactor(j);

            float xs = 0f, ys = 0f;
            if (Orientation == AxisOrientation.X)
            {
                xs = scaler.MeasureInPixels(step * kl);
                if (x + xs >= lxj || x + xs <= lxi)
                {
                    visualState = VisualState.Remove;
                }
            }
            else
            {
                ys = scaler.MeasureInPixels(step * kl);
                if (y - ys >= lyj || y - ys <= lyi)
                {
                    visualState = VisualState.Remove;
                }
            }

            UpdateTick(2.5f, x + xs, y - ys, subtick, visualState);
        }
    }

    private double GetSubStepFactor(int index)
    {
        return (index + 1) / (double)(SeparatorCount + 1);
    }

    private void UpdateTick(
        float tickLength,
        float x,
        float y,
        BaseLineGeometry geometry,
        VisualState visualState)
    {

        float x1, x2, y1, y2;
        if (Orientation == AxisOrientation.X)
        {
            float yp = y + (LabelDesiredRect.Height * 0.5f + TickLength) * (Position == AxisPosition.Start ? -1 : 1);
            x1 = x;
            x2 = x;
            y1 = yp;
            y2 = yp + tickLength * (Position == AxisPosition.Start ? 1 : -1);
        }
        else
        {
            float xp = x + (LabelDesiredRect.Width * 0.5f + TickLength) * (Position == AxisPosition.Start ? 1 : -1);
            x1 = xp;
            x2 = xp + tickLength * (Position == AxisPosition.Start ? -1 : 1);
            y1 = y;
            y2 = y;
        }

        geometry.Paint = TickPaint;
        geometry.X = x1;
        geometry.Y = y1;
        geometry.X1 = x2;
        geometry.Y1 = y2;

        ChangeVisualState(geometry, visualState);
    }

    #endregion


    private void ChangeVisualState(DrawnGeometry geometry, VisualState visualState)
    {
        switch (visualState)
        {
            case VisualState.Display:
                geometry.Opacity = 1f;
                break;
            case VisualState.Remove:
                geometry.Opacity = 0f;
                geometry.Remove = true;
                break;
        }
    }

    private enum VisualState
    {
        Display, // 1
        Remove // 0
    }

    private class AxisVisual
    {
        public double Value { get; set; }
        public BaseLabelGeometry? Label { get; set; }
        public BaseLineGeometry? Tick { get; set; }
        public BaseLineGeometry[]? SubTick { get; set; }
        public BaseLineGeometry? Separator { get; set; }
        public BaseLineGeometry[]? SubSeparator { get; set; }
    }
}
