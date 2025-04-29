using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Primitive;
using Core.View;

namespace Plot.WinForm;
public partial class CartesianChartControl : ChartControl, ICartesianChartView
{
    private CoreChart? _coreChart;

    private IEnumerable<CoreAxis> _xAxes = [];
    private IEnumerable<CoreAxis> _yAxes = [];

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

    public IEnumerable<CoreAxis> XAxes
    {
        get => _xAxes;
        set
        {
            _xAxes = value;
        }
    }

    public IEnumerable<CoreAxis> YAxes
    {
        get => _yAxes;
        set
        {
            _yAxes = value;
        }
    }

    public ElementLayoutKind ElementLayoutKind { get; set; } = ElementLayoutKind.Stack;
}
