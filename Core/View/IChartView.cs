using Core.Primitive;

namespace Core.View;
public interface IChartView
{
    bool IsDesignMode { get; }
    float DisplayScale { get; }

    Size ControlSize { get; }

    void InvokeUIThread(Action action);
}
