using Core.Kernel;
using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.View;
public interface ICartesianChartView : IChartView
{
    IEnumerable<CoreAxis>? XAxes { get; set; }
    IEnumerable<CoreAxis>? YAxes { get; set; }

    CoreDrawnDataArea? CoreDrawnDataArea { get; set; }
    ElementLayoutKind ElementLayoutKind { get; set; }
}
