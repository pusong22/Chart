using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseRectangleGeometry : DrawnGeometry
{
    public float Width { get; protected internal set; }
    public float Height { get; protected internal set; }

    public override Size Measure()
    {
        return new Size(Width, Height);
    }
}
