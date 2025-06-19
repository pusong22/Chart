using Core.Primitive;

namespace Core.Kernel.View;
public interface ICartesianChartView : IChartView
{
    LayoutKind LayoutKind { get; }
}
