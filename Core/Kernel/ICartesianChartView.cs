using Core.Kernel.Visual;
using Core.Primitive;

namespace Core.Kernel;
public interface ICartesianChartView
{
    Size ControlSize { get; }

    IEnumerable<ICartesianAxis>? XAxes { get; }
    IEnumerable<ICartesianAxis>? YAxes { get; }
    IEnumerable<ILineSeries>? Series { get; }

    void InvokeUIThread(Action action);

    IBaseLabelVisual? Title { get; }
}
