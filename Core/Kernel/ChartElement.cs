using Core.Kernel.Chart;

namespace Core.Kernel;

public abstract class ChartElement
{
    public abstract void Invalidate(CoreChart chart);
}
