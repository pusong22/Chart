using Core.Primitive;

namespace Core.Kernel.View;
public interface ICartesianChartView : IChartView
{
    CoreDrawnRect? CoreDrawnDataArea { get; }
    LayoutKind LayoutKind { get; }
}
