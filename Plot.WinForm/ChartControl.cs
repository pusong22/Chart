using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Kernel.View;
using SkiaSharpBackend;

namespace Plot.WinForm;
public abstract partial class ChartControl : UserControl, IChartView
{
    private IEnumerable<CoreAxis>? _xAxes;
    private IEnumerable<CoreAxis>? _yAxes;

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
            using var g = CreateGraphics();
            float scale = g.DpiX / 96f;
            return new(ClientSize.Width / scale, ClientSize.Height / scale);
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
