using Core.Helper;
using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreCartesianAxis : CoreAxis
{
    public AxisOrientation Orientation { get; set; }
    public AxisPosition Position { get; set; }

    // 保存测量大小
    public float X { get; protected internal set; }
    public float Y { get; protected internal set; }

    public Paint? TickPaint { get; set; }
    public Paint? SubTickPaint { get; set; }
    public Paint? SeparatorPaint { get; set; }
    public Paint? SubSeparatorPaint { get; set; }

    public bool? ShowSeparatorLine { get; set; }

    public bool? DrawTickPath { get; set; }
    public int? SeparatorCount { get; set; }
    public float? TickLength { get; set; }
    public float? LabelDensity { get; set; }

    public void Reset(AxisOrientation orientation)
    {
        void ValidateAxisLimit()
        {
            bool Validate(double val)
            {
                return double.IsNaN(val) || double.IsInfinity(val);
            }

            if (Validate(Min)) Min = -10d;
            if (Validate(Max)) Max = 10d;
        }

        Orientation = orientation;

        ValidateAxisLimit();
    }

    public void SetBound(double min, double max)
    {
        Min = Max = 0d;

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

        double step = this.GetIdealStep(NameDesiredRect.Size);

        var start = Math.Floor(Min / step) * step;
        Min = start;

        var scaler = new Scaler(this, LabelDesiredRect.Location, NameDesiredRect.Size);
        var labeler = GetActualLabeler();

        float lxi = drawLocation.X;
        float lxj = drawLocation.X + drawSize.Width;
        float lyi = drawLocation.Y;
        float lyj = drawLocation.Y + drawSize.Height;

        var g = new HashSet<AxisVisual>();

        #region Axis Name

        if (Name is not null && NamePaint is not null)
        {
            if (_nameGeometry is null)
            {
                _nameGeometry = new TLabelGeometry();
                _nameGeometry.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                chart.Canvas.AddDrawnTask(NamePaint, _nameGeometry);
            }

            _nameGeometry.Text = Name;
            _nameGeometry.TextSize = NameSize!.Value;
            _nameGeometry.Paint = NamePaint;
            _nameGeometry.RotateTransform = NameRotation!.Value;
            _nameGeometry.Padding = NamePadding;

            if (Orientation == AxisOrientation.X)
            {
                _nameGeometry.X = NameDesiredRect.X + NameDesiredRect.Width * 0.5f;
                _nameGeometry.Y = NameDesiredRect.Y + NameDesiredRect.Height * 0.5f;

            }
            else
            {
                _nameGeometry.X = NameDesiredRect.X + NameDesiredRect.Width * 0.5f;
                _nameGeometry.Y = NameDesiredRect.Y + NameDesiredRect.Height * 0.5f;
            }

        }

        #endregion

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

        #region Axis Line

        if (TickPaint is not null && DrawTickPath!.Value)
        {
            if (_tickPath is null)
            {
                _tickPath = new TLineGeometry();
                _tickPath.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
            }

            if (Orientation == AxisOrientation.X)
            {
                var yp = yo + LabelDesiredRect.Height * 0.5f * (Position == AxisPosition.Start ? -1 : 1);
                _tickPath.X = LabelDesiredRect.Location.X;
                _tickPath.X1 = LabelDesiredRect.Location.X + LabelDesiredRect.Size.Width;
                _tickPath.Y = yp;
                _tickPath.Y1 = yp;
            }
            else
            {
                var xp = xo + LabelDesiredRect.Width * 0.5f * (Position == AxisPosition.Start ? 1 : -1);
                _tickPath.X = xp;
                _tickPath.X1 = xp;
                _tickPath.Y = LabelDesiredRect.Location.Y;
                _tickPath.Y1 = LabelDesiredRect.Location.Y + LabelDesiredRect.Size.Height;
            }

            chart.Canvas.AddDrawnTask(TickPaint, _tickPath);
        }

        #endregion



        foreach (var i in this.EnumerateSeparators(start, Max, step))
        {
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

            #region Initialize Visual

            if (TickPaint is not null)
            {
                if (axisVisual.Tick is null)
                {
                    axisVisual.Tick = new TLineGeometry();

                    axisVisual.Tick.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                }

                UpdateTick(TickLength!.Value, x, y, axisVisual.Tick, VisualState.Display);

                chart.Canvas.AddDrawnTask(TickPaint, axisVisual.Tick);
            }

            if (SubTickPaint is not null && (i >= Min && i < Max))
            {
                if (axisVisual.SubTick is null)
                {
                    axisVisual.SubTick = new TLineGeometry[SeparatorCount!.Value];

                    for (var j = 0; j < SeparatorCount; j++)
                    {
                        axisVisual.SubTick[j] = new TLineGeometry();
                        axisVisual.SubTick[j].Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                    }
                }

                UpdateSubticks(axisVisual.SubTick, scaler, step, x, y, VisualState.Display);

                for (var j = 0; j < SeparatorCount; j++)
                {
                    chart.Canvas.AddDrawnTask(SubTickPaint, axisVisual.SubTick[j]);
                }
            }

            if (LabelPaint is not null)
            {
                if (axisVisual.Label is null)
                {
                    axisVisual.Label = new TLabelGeometry();

                    axisVisual.Label.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                }

                UpdateLabel(label, x, y, axisVisual.Label, VisualState.Display);

                chart.Canvas.AddDrawnTask(LabelPaint, axisVisual.Label);
            }

            if (SeparatorPaint is not null && ShowSeparatorLine!.Value)
            {
                if (axisVisual.Separator is null)
                {
                    axisVisual.Separator = new TLineGeometry();

                    axisVisual.Separator.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                }

                UpdateSeparator(lxi, lxj, lyi, lyj, x, y, axisVisual.Separator, VisualState.Display);

                chart.Canvas.AddDrawnTask(SeparatorPaint, axisVisual.Separator);
            }

            if (SubSeparatorPaint is not null && ShowSeparatorLine!.Value && (i >= Min && i < Max))
            {
                if (axisVisual.SubSeparator is null)
                {
                    axisVisual.SubSeparator = new TLineGeometry[SeparatorCount!.Value];

                    for (var j = 0; j < SeparatorCount; j++)
                    {
                        axisVisual.SubSeparator[j] = new TLineGeometry();
                        axisVisual.SubSeparator[j].Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                    }
                }

                UpdateSubSeparator(axisVisual.SubSeparator, scaler, step, x, y,
                    lxi, lxj, lyi, lyj, VisualState.Display);

                for (var j = 0; j < SeparatorCount; j++)
                {
                    chart.Canvas.AddDrawnTask(SubSeparatorPaint, axisVisual.SubSeparator[j]);
                }
            }

            #endregion

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
                UpdateTick(TickLength!.Value, x, y, axisVisual.Tick, VisualState.Remove);
            }

            if (axisVisual.SubTick is not null)
            {
                UpdateSubticks(axisVisual.SubTick, scaler, step, x, y, VisualState.Remove);
            }

            _ = _cached.Remove(item.Key);
        }

        #endregion
    }



    private Func<double, string> GetActualLabeler()
    {
        return Labeler ??= t => t.ToString("N1");
    }

    public override Size MeasureNameLabelSize()
    {
        if (string.IsNullOrWhiteSpace(Name) || NamePaint is null)
            return new Size(0f, 0f);

        var _nameGeometry = new TLabelGeometry
        {
            Text = Name,
            TextSize = NameSize!.Value,
            Paint = NamePaint,
            RotateTransform = NameRotation!.Value,
            Padding = NamePadding
        };

        return _nameGeometry.Measure();
    }

    public override Size MeasureMaxLabelSize()
    {
        if (LabelPaint is null || Max == Min)
            return new Size(0f, 0f);

        var labeler = GetActualLabeler();

        const double testSeparators = 25;
        double step = (Max - Min) / testSeparators;

        float w = 0f, h = 0f;

        foreach (var i in this.EnumerateSeparators(Min, Max, step))
        {
            var textGeometry = new TLabelGeometry
            {
                Text = labeler(i),
                TextSize = LabelSize!.Value,
                RotateTransform = LabelRotation!.Value,
                Padding = LabelPadding,
                Paint = LabelPaint
            };

            var m = textGeometry.Measure();

            if (m.Width > w) w = m.Width;
            if (m.Height > h) h = m.Height;
        }

        return new Size(w, h);
    }

    public override Size MeasureLabelSize(Size size)
    {
        if (LabelPaint is null)
            return new Size(0f, 0f);

        var labeler = GetActualLabeler();

        double step = this.GetIdealStep(size);

        var start = Math.Floor(Min / step) * step;

        float w = 0f, h = 0f;

        foreach (var i in this.EnumerateSeparators(start, Max, step))
        {
            var textGeometry = new TLabelGeometry
            {
                Text = labeler(i),
                TextSize = LabelSize!.Value,
                RotateTransform = LabelRotation!.Value,
                Padding = LabelPadding,
                Paint = LabelPaint
            };

            var m = textGeometry.Measure();

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

            var kl = (j + 1) / (double)SeparatorCount!.Value;

            float xs = 0f, ys = 0f;
            if (Orientation == AxisOrientation.X)
            {
                xs = scaler.MeasureInPixels(step * kl);
            }
            else
            {
                ys = scaler.MeasureInPixels(step * kl);
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
        geometry.TextSize = LabelSize!.Value;
        geometry.Paint = LabelPaint;
        geometry.Padding = LabelPadding;
        geometry.RotateTransform = LabelRotation!.Value;
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
        VisualState visualState)
    {
        for (var j = 0; j < subticks.Length; j++)
        {
            var subtick = subticks[j];

            var kl = (j + 1) / (double)SeparatorCount!.Value;

            float xs = 0f, ys = 0f;
            if (Orientation == AxisOrientation.X)
            {
                xs = scaler.MeasureInPixels(step * kl);
            }
            else
            {
                ys = scaler.MeasureInPixels(step * kl);
            }

            UpdateTick(2.5f, x + xs, y - ys, subtick, visualState);
        }
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
            float a = y + LabelDesiredRect.Height * 0.5f; // =label height
            float b = y - LabelDesiredRect.Height * 0.5f;
            x1 = x;
            x2 = x;
            y1 = Position == AxisPosition.Start
                ? b : a - tickLength;
            y2 = Position == AxisPosition.Start
                ? b + tickLength : a;
        }
        else
        {
            float a = x + LabelDesiredRect.Width * 0.5f; // =label Width
            float b = x - LabelDesiredRect.Width * 0.5f;
            y1 = y;
            y2 = y;
            x1 = Position == AxisPosition.Start
                ? a : b;
            x2 = Position == AxisPosition.Start
                ? a - tickLength : b + tickLength;
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

}


public enum VisualState
{
    Display, // 1
    Remove // 0
}

public class AxisVisual
{
    public double Value { get; set; }
    public BaseLabelGeometry? Label { get; set; }
    public BaseLineGeometry? Tick { get; set; }
    public BaseLineGeometry[]? SubTick { get; set; }
    public BaseLineGeometry? Separator { get; set; }
    public BaseLineGeometry[]? SubSeparator { get; set; }
}

