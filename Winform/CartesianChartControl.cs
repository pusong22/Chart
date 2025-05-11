using Core.Kernel;
using Core.Kernel.Chart;
using Core.Kernel.View;
using Core.Primitive;

namespace WinForm;
public partial class CartesianChartControl : ChartControl, ICartesianChartView
{
    private CoreChart? _coreChart;
    private CoreDrawnDataArea? _drawnDataArea;



    protected override CoreChart CoreChart
    {
        get
        {
            _coreChart ??= new CartesianChart(this, _canvasControl.Canvas);
            return _coreChart;
        }
    }

    public LayoutKind LayoutKind { get; set; } = LayoutKind.Stack;

    public CoreDrawnDataArea? CoreDrawnDataArea
    {
        get => _drawnDataArea;
        set => _drawnDataArea = value;
    }
}
