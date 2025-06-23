using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseLineGeometry : DrawnGeometry
{
    public float X1 { get; protected internal set; }

    public float Y1 { get; protected internal set; }

    public override Size Measure()
    {
        return new Size(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
    }
}
