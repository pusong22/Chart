using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace SkiaSharpBackend.Drawing.Geometry;
public class CircleGeometry : BaseRectangleGeometry
{
    public override void Draw<TDrawnContext>(TDrawnContext context)
    {
        float radius = Width / 2f;
        context.DrawCircle(new Point(X, Y), radius);
        context.DrawCircle(new Point(X, Y), radius / 9);
    }
}
