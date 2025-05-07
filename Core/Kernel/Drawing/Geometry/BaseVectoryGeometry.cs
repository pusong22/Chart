using Core.Kernel.Series;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseVectoryGeometry<TSegment> : DrawnGeometry
    where TSegment : CubicBezierSegment
{
    public LinkedList<TSegment> Segments { get; } = new();
}
