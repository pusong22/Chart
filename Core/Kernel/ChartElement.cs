using Core.Kernel.Chart;

namespace Core.Kernel;

public abstract class ChartElement
{
    public object? Tag { get; protected internal set; }
    public abstract void Invalidate(CoreChart chart);
}
