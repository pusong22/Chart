namespace Core.Kernel;

public interface IChartElement
{
    object? Tag { get; }

    void CalculateGeometries(CartesianChart chart);
}
