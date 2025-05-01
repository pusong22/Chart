using Core.Kernel.Chart;
using Core.Kernel.Drawing;
using Core.Primitive;
using Core.View;

namespace Plot.WinForm;
public partial class CartesianChartControl : ChartControl, ICartesianChartView
{
    private CoreChart? _coreChart;


    private CoreDrawnDataArea? _drawnDataArea;

    public CartesianChartControl()
    {

    }

    protected override CoreChart CoreChart
    {
        get
        {
            _coreChart ??= new CartesianChart(this, _canvasControl.Canvas);

            return _coreChart;
        }
    }

    public ElementLayoutKind ElementLayoutKind { get; set; } = ElementLayoutKind.Stack;
    public CoreDrawnDataArea? CoreDrawnDataArea
    {
        get => _drawnDataArea;
        set => _drawnDataArea = value;
    }
}
