using Core.Kernel.Drawing;
using SkiaSharp;
using SkiaSharpBackend.Drawing;

namespace SkiaSharpBackend.Painting;
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
        _paint.Style = Style == Core.Primitive.PaintStyle.Fill
            ? SKPaintStyle.Fill : SKPaintStyle.Stroke;

        skContext.ActivateSkPaint = _paint;
    }


    public override void ApplyOpacity(DrawnContext context, float opacity)
    {
        var skContext = (SkiaSharpDrawnContext)context;

        var baseColor = color;
        _paint ??= new SKPaint();

        _paint.Color =
            new SKColor(
                baseColor.Red,
                baseColor.Green,
                baseColor.Blue,
                (byte)(baseColor.Alpha * opacity));

    }

    public override void RestoreOpacityMask(DrawnContext context)
    {
        _paint ??= new SKPaint();

        _paint.Color = color;
    }
}
