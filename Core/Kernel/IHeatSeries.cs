using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;
public interface IHeatSeries : ICartesianSeries
{
    Color[] HeatMap { get; set; }
    Paint? HeatPaint { get; set; }
    float CellHeight { get; set; }
    float CellWidth { get; set; }
}
