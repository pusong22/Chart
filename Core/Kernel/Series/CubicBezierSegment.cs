using Core.Primitive;

namespace Core.Kernel.Series;
public class CubicBezierSegment
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public Point Control1 { get; set; }
    public Point Control2 { get; set; }
}
