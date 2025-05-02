using Core.Primitive;

namespace Core.Kernel.View;
public interface ICartesianChartView : IChartView
{
    CoreDrawnDataArea? CoreDrawnDataArea { get; }
    LayoutKind LayoutKind { get; }
}
