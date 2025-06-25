using Core.Primitive;

namespace Core.Kernel;
public interface ICartesianSeries : IChartElement
{
    int XIndex { get; set; }
    int YIndex { get; set; }

    SeriesBound GetBound();
    IEnumerable<Coordinate> Fetch();
}
