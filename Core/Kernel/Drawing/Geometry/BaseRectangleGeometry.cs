using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseRectangleGeometry : DrawnGeometry
{
    public float Width { get; set; }
    public float Height { get; set; }

    public override Size Measure()
    {
        return new Size(Width, Height);
    }
}
