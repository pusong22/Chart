using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Kernel.Series;
using Core.Kernel.View;
using Core.Kernel.Visual;
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

        Load += OnLoad;
        Resize += OnResize;
        HandleDestroyed += OnHandleDestroyed;
    }

    protected abstract CoreChart? CoreChart { get; }

    public Core.Primitive.Size ControlSize => new(_canvasControl.ClientSize.Width, _canvasControl.ClientSize.Height);

    public CanvasContext CanvasContext => _canvasControl.CanvasContext;

    public VisualElement? Title { get; set; }

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
        _ = BeginInvoke(action);
    }

    public void OnLoad(object s, EventArgs e)
    {
        CoreChart?.Load();
    }

    public void OnHandleDestroyed(object s, EventArgs e)
    {
        CoreChart?.UnLoad();
    }

    public void OnResize(object s, EventArgs e)
    {
        CoreChart?.Update();
    }
}
