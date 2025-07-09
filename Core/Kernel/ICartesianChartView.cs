namespace Core.Kernel;
public interface ICartesianChartView
{
    void RequestInvalidateVisual(ChartDrawingCommand command);
}
