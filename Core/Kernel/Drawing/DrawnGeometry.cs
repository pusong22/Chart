using Core.Primitive;

namespace Core.Kernel.Drawing;
public abstract class DrawnGeometry
{
    public float Xo { get; set; }
    public float Yo { get; set; }

    public abstract void Draw(DrawnContext context);
    public abstract Size Measure();
}
