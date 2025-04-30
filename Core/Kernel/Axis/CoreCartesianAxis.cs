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

    public float Size { get; internal set; }

    public Paint? TickPaint { get; set; }
    public Paint? SubTickPaint { get; set; }
    public Paint? SeperatorPaint { get; set; }
    public Paint? SubSeperatorPaint { get; set; }

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

public abstract class CartesianAxis<TTLabelGeometry, TLineGeometry> : CoreCartesianAxis
    where TTLabelGeometry : BaseLabelGeometry, new()
    where TLineGeometry : BaseLineGeometry, new()
{
    private TTLabelGeometry? _nameGeometry;
    private TLineGeometry? _tickGeometry;
    private TLineGeometry? _subTickGeometry;
    private TLineGeometry? _seperatorGeometry;
    private TLineGeometry? _subSeperatorGeometry;

    private BaseTickGenerator<TTLabelGeometry>? _generator;

    public override void Invalidate(CoreChart chart)
    {
        var cartesianChart = (CartesianChart)chart;

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



        if (TickPaint is not null)
        {
            _tickGeometry ??= new TLineGeometry();


            chart.Canvas.AddDrawnTask(TickPaint, _tickGeometry);
        }

        if (SubTickPaint is not null)
        {
            _subTickGeometry ??= new TLineGeometry();

            chart.Canvas.AddDrawnTask(SubTickPaint, _subTickGeometry);
        }

        if (SeperatorPaint is not null)
        {
            _seperatorGeometry ??= new TLineGeometry();

            chart.Canvas.AddDrawnTask(SeperatorPaint, _seperatorGeometry);
        }

        if (SubSeperatorPaint is not null)
        {
            _subSeperatorGeometry ??= new TLineGeometry();

            chart.Canvas.AddDrawnTask(SubSeperatorPaint, _subSeperatorGeometry);
        }


        // Generatick 
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
                y = Y;
            }
            else
            {
                x = X;
                y = scale.ToPixel(position);
            }

            float x1, y1, x2, y2;
            if (Orientation == AxisOrientation.X)
            {
               x1 = 
            }
            else
            {
               
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
