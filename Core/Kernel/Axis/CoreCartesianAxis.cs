using Core.Kernel.Chart;
using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.TickGenerator;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreCartesianAxis : CoreAxis
{
    public AxisOrientation Orientation { get; set; }
    public AxisPosition Position { get; set; }

    public float X { get; set; }
    public float Y { get; set; }

    public Paint? TickPaint { get; set; }
    public Paint? SubTickPaint { get; set; }

    public void Reset(AxisOrientation orientation)
    {
        void ValidateAxisLimit()
        {
            bool Validate(double val)
            {
                return double.IsNaN(val) || double.IsInfinity(val);
            }

            if (Validate(Min)) Min = 0d;
            if (Validate(Max)) Max = 10d;
        }

        Orientation = orientation;

        ValidateAxisLimit();
    }

    public abstract void GenerateTick(float axisLength);
    public abstract void Invalidate(CoreChart chart);
}

public abstract class CoreCartesianAxis<TTLabelGeometry, TLineGeometry> : CoreCartesianAxis
    where TTLabelGeometry : BaseLabelGeometry, new()
    where TLineGeometry : BaseLineGeometry, new()
{
    private TTLabelGeometry? _nameGeometry;

    private BaseTickGenerator<TTLabelGeometry>? _generator;

    public override void Invalidate(CoreChart chart)
    {
        var controlSize = chart.ScaledControlSize;

        var scale = new Scaler(this, chart.DataRect);

        if (Name is not null && NamePaint is not null)
        {
            _nameGeometry ??= new TTLabelGeometry();
            _nameGeometry.Text = Name;
            _nameGeometry.TextSize = NameSize;
            _nameGeometry.Paint = NamePaint;
            _nameGeometry.RotateTransform = Orientation == AxisOrientation.X
                    ? 0
                    : -90;
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

            chart.Canvas.AddDrawnTask(NamePaint, _nameGeometry);
        }

        GenerateTick(Orientation == AxisOrientation.X
            ? chart.DataRect.Width : chart.DataRect.Height);

        foreach (var tick in _generator?.Ticks)
        {
            string label = tick.Label;
            double position = tick.Position;
            bool majorTick = tick.MajorPos;

            float tickLength = majorTick ? 5f : 3f;
            float x, y;

            if (Orientation == AxisOrientation.X)
            {
                x = scale.ToPixel(position);
                y = Position == AxisPosition.Start
                    ? controlSize.Height - Y
                    : Y;
            }
            else
            {
                x = Position == AxisPosition.Start
                    ? X
                    : controlSize.Width - X;
                y = scale.ToPixel(position);
            }

            if (TickPaint is not null)
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

                var tickGeometry = new TLineGeometry()
                {
                    Paint = TickPaint,
                    X = x1,
                    Y = y1,
                    X1 = x2,
                    Y1 = y2
                };

                chart.Canvas.AddDrawnTask(TickPaint, tickGeometry);
            }

            if (majorTick && !string.IsNullOrWhiteSpace(label) && LabelPaint is not null)
            {
                var labelGeometry = new TTLabelGeometry()
                {
                    Text = label,
                    TextSize = LabelSize,
                    Paint = LabelPaint,
                    Padding = LabelPadding,
                    X = x,
                    Y = y
                };

                chart.Canvas.AddDrawnTask(LabelPaint, labelGeometry);
            }
        }
    }

    public override Size MeasureNameLabelSize()
    {
        if (string.IsNullOrWhiteSpace(Name) || NamePaint is null)
            return new Size(0f, 0f);

        _nameGeometry ??= new TTLabelGeometry();
        _nameGeometry.Text = Name;
        _nameGeometry.TextSize = NameSize;
        _nameGeometry.Paint = NamePaint;
        _nameGeometry.RotateTransform = Orientation == AxisOrientation.X
                ? 0
                : -90;
        _nameGeometry.Padding = NamePadding;

        return _nameGeometry.Measure();
    }

    public override Size MeasureLabelSize()
    {
        if (string.IsNullOrWhiteSpace(_generator?.MaxLabel) || LabelPaint is null)
            return new Size(0f, 0f);

        var geometry = new TTLabelGeometry()
        {
            Text = _generator?.MaxLabel,
            TextSize = LabelSize,
            Paint = LabelPaint,
            Padding = LabelPadding,
        };

        return geometry.Measure();
    }


    public override void GenerateTick(float axisLength)
    {
        if (LabelPaint is null)
            return;

        _generator ??= new LinearGenerator<TTLabelGeometry>(this);

        _generator.Generate(axisLength);
    }
}
