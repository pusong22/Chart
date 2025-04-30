using Core.Kernel.Drawing.Geometry;

namespace SkiaSharpBackend.Drawing.Geometry;
public class RectangleGeometry : BaseRectangleGeometry
{
    public override void Draw<TDrawnContext>(TDrawnContext context)
    {
        if (context is not SkiaSharpDrawnContext skContext) return;

        skContext.Canvas.DrawRect(X, Y, Width, Height, skContext.ActivateSkPaint);
    }
}
