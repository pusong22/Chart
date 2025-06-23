using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;
public interface ILineSeries : IChartElement
{
    float LineSmoothness { get; set; }
    double SampleInterval { get; set; }
    Paint? SeriesPaint { get; set; }
    float VisualGeometrySize { get; set; }
    int XIndex { get; set; }
    int YIndex { get; set; }

    IEnumerable<Coordinate> Fetch();
    SeriesBound GetBound();
}
