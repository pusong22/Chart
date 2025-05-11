using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Kernel.Series;
using Core.Kernel.View;
using SkiaSharpBackend;

namespace WinForm;
public abstract partial class ChartControl : UserControl, IChartView
{
    private IEnumerable<CoreAxis>? _xAxes;
    private IEnumerable<CoreAxis>? _yAxes;
    private IEnumerable<CoreSeries>? _series;

    protected ChartControl()
    {
        InitializeComponent();

        ChartConfig.Configure(config => config.UseDefault());
    }

    protected abstract CoreChart? CoreChart { get; }

    public Core.Primitive.Size ControlSize
    {
        get
        {
            return new(_canvasControl.ClientSize.Width, _canvasControl.ClientSize.Height);
        }
    }

    public IEnumerable<CoreAxis>? XAxes
    {
        get => _xAxes;
        set
        {
            _xAxes = value;
        }
    }

    public IEnumerable<CoreAxis>? YAxes
    {
        get => _yAxes;
        set
        {
            _yAxes = value;
        }
    }


    public IEnumerable<CoreSeries>? Series
    {
        get => _series;
        set
        {
            _series = value;
        }
    }

    public void InvokeUIThread(Action action)
    {
        if (!IsHandleCreated) return;
        _ = BeginInvoke(action);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CoreChart?.Load();
    }

    protected override void OnResize(EventArgs e)
    {
        // 手动调用canvasControl的invalidate
        base.OnResize(e);

        //_canvasControl.Size = Size;
        CoreChart?.Update();
    }
}
