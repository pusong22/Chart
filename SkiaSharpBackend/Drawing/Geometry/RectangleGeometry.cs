using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace SkiaSharpBackend.Drawing.Geometry;
public class RectangleGeometry : BaseRectangleGeometry
{
    public override void Draw<TDrawnContext>(TDrawnContext context)
    {
        context.DrawRect(new Rect(X, Y, Width, Height));
    }
}
