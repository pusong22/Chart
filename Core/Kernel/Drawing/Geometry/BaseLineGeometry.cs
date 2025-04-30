using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseLineGeometry : DrawnGeometry
{
    public float Xo1 { get; set; }
    public float Yo1 { get; set; }

    public override Size Measure()
    {
        return new Size(Math.Abs(Xo1 - Xo), Math.Abs(Yo1 - Yo));
    }
}
