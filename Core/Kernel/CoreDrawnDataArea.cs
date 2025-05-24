using Core.Helper;
using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;
public abstract class CoreDrawnDataArea : ChartElement
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
}

public abstract class CoreDrawnDataArea<TRectangleGeometry> : CoreDrawnDataArea
    where TRectangleGeometry : BaseRectangleGeometry, new()
{
    private TRectangleGeometry? _fillGeometry;
    private TRectangleGeometry? _strokeGeometry;

    public override void Invalidate(CoreChart chart)
    {
        var location = chart.DrawnLocation;
        var size = chart.DrawnSize;

        if (Fill is not null)
        {
            if (_fillGeometry is null)
            {
                _fillGeometry = new TRectangleGeometry();
                _fillGeometry.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                chart.CanvasContext.AddDrawnTask(Fill, _fillGeometry);
            }

            _fillGeometry.X = location.X;
            _fillGeometry.Y = location.Y;
            _fillGeometry.Width = size.Width;
            _fillGeometry.Height = size.Height;
        }

        if (Stroke is not null)
        {
            if (_strokeGeometry is null)
            {
                _strokeGeometry = new TRectangleGeometry();
                _strokeGeometry.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                chart.CanvasContext.AddDrawnTask(Stroke, _strokeGeometry);
            }

            _strokeGeometry.X = location.X;
            _strokeGeometry.Y = location.Y;
            _strokeGeometry.Width = size.Width;
            _strokeGeometry.Height = size.Height;
        }
    }
}
