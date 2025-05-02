using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace SkiaSharpBackend.Drawing.Geometry;

public class LineGeometry : BaseLineGeometry
{
    public override void Draw<TDrawnContext>(TDrawnContext context)
    {
        context.DrawLine(new Point(X, Y), new Point(X1, Y1));
    }
}
