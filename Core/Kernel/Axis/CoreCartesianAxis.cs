using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.TickGenerator;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreCartesianAxis : CoreAxis
{
    public AxisOrientation Orientation { get; set; }
    public AxisPosition Position { get; set; }

    public float Xo { get; set; }
    public float Yo { get; set; }

    public Func<double, string>? Labeler { get; set; }
    public float Size { get; internal set; }

    public void Reset(AxisOrientation orientation)
    {
        Orientation = orientation;
        DataBound = new(0d, 10d);
    }

    public abstract void GenerateTick(float axisLength);
    public abstract Size MeasureTickLabelSize();
    public abstract void Invalidate(CoreChart chart);
}

public abstract class CartesianAxis<TTLabelGeometry, TLineGeometry> : CoreCartesianAxis
    where TTLabelGeometry : BaseLabelGeometry, new()
    where TLineGeometry : BaseLineGeometry, new()
{
    private TTLabelGeometry? _nameGeometry;
    private TLineGeometry? _lineGeometry;
    private BaseTickGenerator<TTLabelGeometry>? _generator;
    public IEnumerable<Tick>? Ticks { get; private set; }

    public override void Invalidate(CoreChart chart)
    {
        var cartesianChart = (CartesianChart)chart;

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
                _nameGeometry.Xo = NameDesiredRect.X + NameDesiredRect.Width * 0.5f;
                _nameGeometry.Yo = NameDesiredRect.Y + NameDesiredRect.Height * 0.5f;

            }
            else
            {
                _nameGeometry.Xo = NameDesiredRect.X + NameDesiredRect.Width * 0.5f;
                _nameGeometry.Yo = NameDesiredRect.Y + NameDesiredRect.Height * 0.5f;
            }

            chart.Canvas.AddDrawnTask(NamePaint, _nameGeometry);
        }
    }

    public override Size MeasureNameLabelSize()
    {
        if (Name is null || NamePaint is null)
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

    public override Size MeasureTickLabelSize()
    {
        return new Size();
        if (_generator?.MaxLabel is null || LabelPaint is null)
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
        if (axisLength == 0f)
        {
            throw new Exception("Size isn`t invalid");
        }

        if (LabelPaint is null)
            return;

        _generator ??= new LinearGenerator<TTLabelGeometry>(Labeler);
        _generator.LabelPaint = LabelPaint;

        bool vertical = Orientation == AxisOrientation.Y;

        DataBound ??= new(0d, 10d);
        Ticks = _generator.Generate(DataBound, vertical, axisLength);
    }
}
