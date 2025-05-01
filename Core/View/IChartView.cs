using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.View;
public interface IChartView
{
    bool IsDesignMode { get; }
    float DisplayScale { get; }

    Size ControlSize { get; }

    IEnumerable<CoreAxis>? XAxes { get; set; }
    IEnumerable<CoreAxis>? YAxes { get; set; }

    void InvokeUIThread(Action action);
}
