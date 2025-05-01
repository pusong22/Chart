using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.View;
using System.ComponentModel;

namespace Plot.WinForm;
public abstract partial class ChartControl : UserControl, IChartView
{
    private IEnumerable<CoreAxis>? _xAxes;
    private IEnumerable<CoreAxis>? _yAxes;

    protected ChartControl()
    {
        InitializeComponent();
    }

    protected abstract CoreChart? CoreChart { get; }

    public Core.Primitive.Size ControlSize => new(ClientSize.Width, ClientSize.Height);

    public bool IsDesignMode => DesignMode
        || LicenseManager.UsageMode == LicenseUsageMode.Designtime;


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


    public float DisplayScale
    {
        get
        {
            using Graphics g = CreateGraphics();
            return g.DpiX / 96f;
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
        //base.OnResize(e);

        _canvasControl.Size = Size;
        CoreChart?.Update();
    }
}
