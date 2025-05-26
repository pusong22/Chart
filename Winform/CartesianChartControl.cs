using Core.Kernel;
using Core.Kernel.Chart;
using Core.Kernel.View;
using Core.Primitive;

namespace WinForm;
public partial class CartesianChartControl : ChartControl, ICartesianChartView
{
    private CoreChart? _coreChart;
    private CoreDrawnRect? _drawnDataArea;

    protected override CoreChart CoreChart
    {
        get
        {
            _coreChart ??= new CartesianChart(this);
            return _coreChart;
        }
    }

    public LayoutKind LayoutKind { get; set; } = LayoutKind.Stack;

    public CoreDrawnRect? CoreDrawnDataArea
    {
        get => _drawnDataArea;
        set => _drawnDataArea = value;
    }
}
