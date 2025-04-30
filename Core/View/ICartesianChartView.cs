using Core.Kernel;
using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.View;
public interface ICartesianChartView : IChartView
{
    IEnumerable<CoreCartesianAxis>? XAxes { get; set; }
    IEnumerable<CoreCartesianAxis>? YAxes { get; set; }

    CoreDrawnDataArea? CoreDrawnDataArea { get; set; }
    ElementLayoutKind ElementLayoutKind { get; set; }
}
