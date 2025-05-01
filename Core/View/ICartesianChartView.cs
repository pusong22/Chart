using Core.Kernel.Drawing;
using Core.Primitive;

namespace Core.View;
public interface ICartesianChartView : IChartView
{
    CoreDrawnDataArea? CoreDrawnDataArea { get; set; }
    ElementLayoutKind ElementLayoutKind { get; set; }
}
