namespace Core.Kernel;

public interface IChartElement
{
    object? Tag { get; }

    void Invalidate(CartesianChart chart);
}
