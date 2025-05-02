using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.Kernel.View;
public interface IChartView
{
    Size ControlSize { get; }

    IEnumerable<CoreAxis>? XAxes { get; }
    IEnumerable<CoreAxis>? YAxes { get; }

    void InvokeUIThread(Action action);
}
