using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Kernel.Series;
using Core.Kernel.Visual;
using Core.Primitive;

namespace Core.Kernel.View;
public interface IChartView
{
    CanvasContext CanvasContext { get; }
    Size ControlSize { get; }

    IEnumerable<CoreAxis>? XAxes { get; }
    IEnumerable<CoreAxis>? YAxes { get; }
    IEnumerable<CoreSeries>? Series { get; }

    void InvokeUIThread(Action action);

    VisualElement? Title { get; }
}
