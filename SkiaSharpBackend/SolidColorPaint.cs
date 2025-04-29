using Core.Kernel.Drawing;
using SkiaSharp;

namespace SkiaSharpBackend;
public class SolidColorPaint(SKColor color) : SkiaSharpPaint
{
    private SKPaint? _paint;

    public SolidColorPaint() : this(new SKColor(0, 0, 0)) { }

    public override void Dispose()
    {
        _paint?.Dispose();
        _paint = null;
    }

    public override void Initialize(DrawnContext context)
    {
        var skContext = (SkiaSharpDrawnContext)context;
        _paint ??= new SKPaint();

        _paint.Color = color;

        _paint.IsAntialias = IsAntialias;

        skContext.ActivateSkPaint = _paint;
    }
}
