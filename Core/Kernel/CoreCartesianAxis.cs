using Core.Helper;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;

public abstract partial class CoreCartesianAxis<TLabel, TLine> : ICartesianAxis
    where TLabel : BaseLabelGeometry, new()
    where TLine : BaseLineGeometry, new()
{
    private class AxisVisual
    {
        public double Position { get; set; }
        public TLine? Tick { get; set; }
        public TLine[]? SubTick { get; set; }
        public TLabel? TickLabel { get; set; }
        public TLine? Separator { get; set; }
        public TLine[]? SubSeparator { get; set; }
    }

    private readonly Dictionary<string, AxisVisual> _visuals = [];
    private HashSet<string> _previousFrameLabels = [];
    private TLabel? _nameGeometry;
    private TLine? _tickPath;

    private Paint? _namePaint;
    private Paint? _labelPaint;

    private Paint? _tickPaint;
    private Paint? _subTickPaint;
    private Paint? _axisLinePaint;
    private Paint? _separatorPaint;
    private Paint? _subSeparatorPaint;

    public object? Tag { get; protected internal set; }
    public double Min { get; protected internal set; } = double.PositiveInfinity;
    public double Max { get; protected internal set; } = double.NegativeInfinity;

    public string? Name { get; set; }

    public float NameRotation { get; set; }
    public float LabelRotation { get; set; }

    public Padding NamePadding { get; set; } = new(5f);
    public Padding LabelPadding { get; set; } = new(5f);

    public AxisOrientation Orientation { get; protected internal set; }
    public AxisPosition Position { get; set; }

    public float X { get; set; }
    public float Y { get; set; }

    public int SeparatorCount { get; set; } = 4;
    public float TickLength { get; set; } = 5f;
    public float LabelDensity { get; set; } = 0.85f;

    public Func<double, string>? Labeler { get; set; }

    public Paint? NamePaint
    {
        get => _namePaint;
        set
        {
            if (value != _namePaint)
            {
                _namePaint = value;
            }
        }
    }

    public Paint? LabelPaint
    {
        get => _labelPaint;
        set
        {
            if (value != _labelPaint)
            {
                _labelPaint = value;
            }
        }
    }

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

    // 保存测量后的坐标信息
    public Rect NameDesiredRect { get; set; }
    public Rect LabelDesiredRect { get; set; }

    public void Reset(AxisOrientation orientation)
    {
        Orientation = orientation;

        if (!Extensions.Validate(Min)) Min = -10d;
        if (!Extensions.Validate(Max)) Max = 10d;
    }

    public void SetBound(double min, double max)
    {
        Min = min;
        Max = max;
    }


    public void CalculateGeometries(CartesianChart chart)
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

        if (NamePaint is not null && Name is not null)
        {
            _nameGeometry ??= new TLabel();
            chart.RequestGeometry(NamePaint, _nameGeometry);

            _nameGeometry.Text = Name;
            _nameGeometry.RotateTransform = NameRotation;
            _nameGeometry.Padding = NamePadding;
            _nameGeometry.X = NameDesiredRect.X + NameDesiredRect.Width * 0.5f;
            _nameGeometry.Y = NameDesiredRect.Y + NameDesiredRect.Height * 0.5f;
        }

        if (AxisLinePaint is not null)
        {
            _tickPath ??= new TLine();
            chart.RequestGeometry(AxisLinePaint, _tickPath);

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

        double step = this.GetIdealStep(NameDesiredRect.Size);

        var startOffset = Math.Floor(Min / step) * step;
        var labeler = GetActualLabeler();

        HashSet<string> currentFrameLabels = [];

        foreach (var i in Extensions.EnumerateSeparators(startOffset, Max, step))
        {
            if (i < Min || i > Max) continue;

            string label = labeler(i);

            currentFrameLabels.Add(label);

            if (!_visuals.TryGetValue(label, out var visual))
            {
                visual = new AxisVisual
                {
                    Position = i
                };

                _visuals[label] = visual;
            }

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

            if (TickPaint is not null)
            {
                visual.Tick ??= new TLine();
                chart.RequestGeometry(TickPaint, visual.Tick);

                UpdateTick(TickLength, x, y, visual.Tick);

                UpdateOpacity(visual.Tick, true);
            }

            if (SubTickPaint is not null)
            {
                visual.SubTick ??= new TLine[SeparatorCount];

                for (var j = 0; j < SeparatorCount; j++)
                {
                    visual.SubTick[j] ??= new TLine();
                    chart.RequestGeometry(SubTickPaint, visual.SubTick[j]);
                }

                UpdateSubticks(visual.SubTick, scaler, step, x, y);

                for (int j = 0; j < SeparatorCount; j++)
                    UpdateOpacity(visual.SubTick[j], true);
            }

            if (LabelPaint is not null)
            {
                visual.TickLabel ??= new TLabel();
                chart.RequestGeometry(LabelPaint, visual.TickLabel);

                UpdateLabel(label, x, y, visual.TickLabel);

                UpdateOpacity(visual.TickLabel, true);
            }

            if (SeparatorPaint is not null)
            {
                visual.Separator ??= new TLine();
                chart.RequestGeometry(SeparatorPaint, visual.Separator);

                UpdateSeparator(lxi, lxj, lyi, lyj, x, y, visual.Separator);
                UpdateOpacity(visual.Separator, true);
            }


            if (SubSeparatorPaint is not null)
            {
                visual.SubSeparator ??= new TLine[SeparatorCount];

                for (var j = 0; j < SeparatorCount; j++)
                {
                    visual.SubSeparator[j] ??= new TLine();
                    chart.RequestGeometry(SubSeparatorPaint, visual.SubSeparator[j]);
                }

                UpdateSubSeparator(visual.SubSeparator, scaler, step, x, y,
                    lxi, lxj, lyi, lyj);

                for (int j = 0; j < SeparatorCount; j++)
                    UpdateOpacity(visual.SubSeparator[j], true);
            }

        }

        // remove unvisiblity
        List<string> removedLabels = [.. _previousFrameLabels.Except(currentFrameLabels)];
        foreach (string label in removedLabels)
        {
            AxisVisual visual = _visuals[label];

            float x, y;
            if (Orientation == AxisOrientation.X)
            {
                x = scaler.ToPixel(visual.Position);
                y = yo;
            }
            else
            {
                x = xo;
                y = scaler.ToPixel(visual.Position);
            }

            if (visual.Tick is not null)
            {
                UpdateTick(TickLength, x, y, visual.Tick);

                UpdateOpacity(visual.Tick, false);
            }

            if (visual.SubTick is not null)
            {
                UpdateSubticks(visual.SubTick, scaler, step, x, y);

                for (int j = 0; j < SeparatorCount; j++)
                    UpdateOpacity(visual.SubTick[j], false);
            }

            if (visual.TickLabel is not null)
            {
                UpdateLabel(label, x, y, visual.TickLabel);

                UpdateOpacity(visual.TickLabel, false);
            }

            if (visual.Separator is not null)
            {
                UpdateSeparator(lxi, lxj, lyi, lyj, x, y, visual.Separator);
                UpdateOpacity(visual.Separator, false);
            }

            if (visual.SubSeparator is not null)
            {
                UpdateSubSeparator(visual.SubSeparator, scaler, step, x, y,
                  lxi, lxj, lyi, lyj);

                for (int j = 0; j < SeparatorCount; j++)
                    UpdateOpacity(visual.SubSeparator[j], false);
            }

            _visuals.Remove(label);
        }

        _previousFrameLabels = currentFrameLabels;
    }

    public void UpdateOpacity(DrawnGeometry geometry, bool visiblity)
    {
        geometry.Opacity = visiblity ? 1 : 0;
        geometry.Remove = !visiblity;
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
        float lyj)
    {
        for (var j = 0; j < subSeparators.Length; j++)
        {
            var subSeparator = subSeparators[j];

            var kl = GetSubStepFactor(j);

            float xs = 0f, ys = 0f;
            if (Orientation == AxisOrientation.X)
                xs = scaler.MeasureInPixels(step * kl);
            else
                ys = scaler.MeasureInPixels(step * kl);

            UpdateSeparator(lxi, lxj, lyi, lyj, x + xs, y - ys, subSeparator);
        }
    }

    private void UpdateSeparator(
        float lxi,
        float lxj,
        float lyi,
        float lyj,
        float x,
        float y,
        BaseLineGeometry geometry)
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

        geometry.X = x1;
        geometry.Y = y1;
        geometry.X1 = x2;
        geometry.Y1 = y2;
    }

    private void UpdateLabel(
        string label,
        float x,
        float y,
        BaseLabelGeometry geometry)
    {
        geometry.Text = label;
        geometry.Padding = LabelPadding;
        geometry.RotateTransform = LabelRotation;
        geometry.X = x;
        geometry.Y = y;
    }

    private void UpdateSubticks(
        BaseLineGeometry[] subticks,
        Scaler scaler,
        double step,
        float x,
        float y)
    {
        for (var j = 0; j < subticks.Length; j++)
        {
            var subtick = subticks[j];
            double kl = GetSubStepFactor(j);

            float xs = 0f, ys = 0f;
            if (Orientation == AxisOrientation.X)
                xs = scaler.MeasureInPixels(step * kl);
            else
                ys = scaler.MeasureInPixels(step * kl);

            UpdateTick(2.5f, x + xs, y - ys, subtick);
        }
    }

    private void UpdateTick(
        float tickLength,
        float x,
        float y,
        BaseLineGeometry geometry)
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

        geometry.X = x1;
        geometry.Y = y1;
        geometry.X1 = x2;
        geometry.Y1 = y2;
    }

    #endregion

    public Size MeasureNameLabelSize()
    {
        if (string.IsNullOrWhiteSpace(Name) || NamePaint is null)
            return new Size(0f, 0f);

        var _nameGeometry = new TLabel
        {
            Text = Name,
            Paint = NamePaint,
            RotateTransform = NameRotation,
            Padding = NamePadding
        };

        // TODO: 性能
        return _nameGeometry.Measure();
    }

    public Size MeasureMaxLabelSize()
    {
        if (LabelPaint is null || Max == Min)
            return new Size(0f, 0f);

        const double testSeparators = 25;
        double step = (Max - Min) / testSeparators;

        return MeasureLabelInternal(Extensions.EnumerateSeparators(Min, Max, step));
    }


    public Size MeasureLabelSize(Size size)
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

        TLabel geometry = new();
        foreach (var i in separatorGenerator)
        {
            geometry.Text = labeler(i);
            geometry.RotateTransform = LabelRotation;
            geometry.Padding = LabelPadding;
            geometry.Paint = LabelPaint;

            var m = geometry.Measure();

            if (m.Width > w) w = m.Width;
            if (m.Height > h) h = m.Height;
        }

        return new Size(w, h);
    }

    private double GetSubStepFactor(int index)
    {
        return (index + 1) / (double)(SeparatorCount + 1);
    }


    private Func<double, string> GetActualLabeler()
    {
        return Labeler ??= t => t.ToString("N3");
    }
}
