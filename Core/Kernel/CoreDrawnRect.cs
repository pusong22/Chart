using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;

namespace Core.Kernel;
public abstract class CoreDrawnRect : ChartElement
{
    private Paint? _fill;
    private Paint? _stroke;

    public Paint? Fill
    {
        get => _fill;
        set
        {
            if (_fill != value)
            {
                _fill = value;
            }
        }
    }

    public Paint? Stroke
    {
        get => _stroke;
        set
        {
            if (_stroke != value)
            {
                _stroke = value;
            }
        }
    }
}

public abstract class CoreDrawnDataArea<TRect> : CoreDrawnRect
    where TRect : BaseRectangleGeometry, new()
{
    private TRect? _fillGeometry;
    private TRect? _strokeGeometry;

    public override void Invalidate(CoreChart chart)
    {
        var location = chart.DrawnLocation;
        var size = chart.DrawnSize;

        if (Fill is not null)
        {
            _fillGeometry ??= chart.CanvasContext.RequestGeometry<TRect>(Fill);

            _fillGeometry.X = location.X;
            _fillGeometry.Y = location.Y;
            _fillGeometry.Width = size.Width;
            _fillGeometry.Height = size.Height;
        }

        if (Stroke is not null)
        {
            _strokeGeometry ??= chart.CanvasContext.RequestGeometry<TRect>(Stroke);

            _strokeGeometry.X = location.X;
            _strokeGeometry.Y = location.Y;
            _strokeGeometry.Width = size.Width;
            _strokeGeometry.Height = size.Height;
        }
    }
}
