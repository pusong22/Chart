using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using SkiaSharp;

namespace SkiaSharpBackend.Drawing;
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
        if (drawable.HasTransform)
        {
            Canvas.Save();
            var p = new SKPoint(drawable.X, drawable.Y);

            if (drawable.HasRotation)
            {
                Canvas.Translate(p.X, p.Y);
                Canvas.RotateDegrees(drawable.RotateTransform);
                Canvas.Translate(-p.X, -p.Y);
            }
        }

        drawable.Draw(this);

        if (drawable.HasTransform) Canvas.Restore();
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
