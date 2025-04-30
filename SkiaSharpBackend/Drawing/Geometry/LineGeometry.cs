using Core.Kernel.Drawing.Geometry;

namespace SkiaSharpBackend.Drawing.Geometry;

public class LineGeometry : BaseLineGeometry
{
    public override void Draw<TDrawnGeometry>(TDrawnGeometry context)
    {
        if (context is not SkiaSharpDrawnContext skContext) return;

        skContext.Canvas.DrawLine(X, Y, X1, Y1, skContext.ActivateSkPaint);
    }
}
