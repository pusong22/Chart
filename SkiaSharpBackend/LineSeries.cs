using Core.Kernel.Series;
using SkiaSharpBackend.Drawing.Geometry;

namespace SkiaSharpBackend;
public class LineSeries<TValueType>
    : CoreLineSeries<TValueType, CircleGeometry, CubicBezierVectorGeometry>
{
    public LineSeries() : base(null) { }

    public LineSeries(IReadOnlyCollection<TValueType> values) : base(values) { }
}
