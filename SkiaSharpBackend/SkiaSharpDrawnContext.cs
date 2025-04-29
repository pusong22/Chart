using Core.Kernel.Drawing;
using SkiaSharp;

namespace SkiaSharpBackend;
public class SkiaSharpDrawnContext(SKSurface surface, SKImageInfo info)
    : DrawnContext
{
    public SKCanvas Canvas { get; } = surface.Canvas;
    public SKImageInfo Info { get; } = info;
    public SKPaint? ActivateSkPaint { get; set; }

    public override void BeginDraw()
    {
        Canvas.Clear();
        using var p = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255)
        };

        Canvas.DrawRect(Info.Rect, p);
    }

    public override void Draw(DrawnGeometry drawable)
    {
        drawable.Draw(this);
    }

    public override void DisposePaint()
    {
        ActivatePaint?.Dispose();
        ActivatePaint = null;
        ActivateSkPaint = null;
    }

    public override void InitializePaint(Paint paint)
    {
        ActivatePaint = paint;
        paint.Initialize(this);
    }
}
