using Core.Kernel.Drawing.Geometry;

namespace SkiaSharpBackend.Drawing.Geometry;

public class LineGeometry : BaseLineGeometry
{
    public override void Draw<TDrawnGeometry>(TDrawnGeometry context)
    {
        if (context is not SkiaSharpDrawnContext skContext) return;

        skContext.Canvas.DrawLine(Xo, Yo, Xo1, Yo1, skContext.ActivateSkPaint);
    }
}
