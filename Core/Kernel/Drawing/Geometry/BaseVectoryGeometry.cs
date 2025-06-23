using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseVectoryGeometry<TSegment> : DrawnGeometry
    where TSegment : CubicBezierSegment
{
    public List<TSegment> Segments { get; } = [];


    public override Size Measure()
    {
        return new Size(0f, 0f);
    }
}
