using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace Core.Kernel.Drawing;
public abstract class CoreDrawnDataArea
{
    private Paint? _fill;
    private Paint? _stroke;

    public Paint? Fill
    {
        get => _fill;
        set
        {
            _fill = value;
            if (_fill is null) return;

            _fill.Style = PaintStyle.Fill;
        }
    }

    public Paint? Stroke
    {
        get => _stroke;
        set
        {
            _stroke = value;
            if (_stroke is null) return;

            _stroke.Style = PaintStyle.Stroke;
        }
    }

    public abstract void Invalidate(CoreChart chart);
}

public abstract class CoreDrawnDataArea<TRectangleGeometry> : CoreDrawnDataArea
    where TRectangleGeometry : BaseRectangleGeometry, new()
{
    private TRectangleGeometry? _fillGeometry;
    private TRectangleGeometry? _strokeGeometry;

    public override void Invalidate(CoreChart chart)
    {
        var drawLocation = chart.DataRect.Location;
        var drawSize = chart.DataRect.Size;

        if (Fill is not null)
        {
            _fillGeometry ??= new TRectangleGeometry();

            _fillGeometry.X = drawLocation.X;
            _fillGeometry.Y = drawLocation.Y;
            _fillGeometry.Width = drawSize.Width;
            _fillGeometry.Height = drawSize.Height;

            chart.Canvas.AddDrawnTask(Fill, _fillGeometry);
        }

        if (Stroke is not null)
        {
            _strokeGeometry ??= new TRectangleGeometry();

            _strokeGeometry.X = drawLocation.X;
            _strokeGeometry.Y = drawLocation.Y;
            _strokeGeometry.Width = drawSize.Width;
            _strokeGeometry.Height = drawSize.Height;

            chart.Canvas.AddDrawnTask(Stroke, _strokeGeometry);
        }
    }
}
