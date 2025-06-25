using Core.Kernel.Painting;

namespace Core.Kernel;
public interface ILineSeries : ICartesianSeries
{
    float LineSmoothness { get; set; }
    double SampleInterval { get; set; }
    Paint? SeriesPaint { get; set; }
    float VisualGeometrySize { get; set; }
}
