using Core.Kernel.Axis;
using Core.Kernel.Series;
using Core.Primitive;

namespace Core.Kernel.View;
public interface IChartView
{
    Size ControlSize { get; }

    IEnumerable<CoreAxis>? XAxes { get; }
    IEnumerable<CoreAxis>? YAxes { get; }
    IEnumerable<CoreSeries>? Series { get; }

    void InvokeUIThread(Action action);
}
