using Core.Kernel.Chart;
using Core.View;
using System.ComponentModel;

namespace Plot.WinForm;
public abstract partial class ChartControl : UserControl, IChartView
{
    protected ChartControl()
    {
        InitializeComponent();
    }

    protected abstract CoreChart? CoreChart { get; }

    public Core.Primitive.Size ControlSize => new(ClientSize.Width, ClientSize.Height);

    public bool IsDesignMode => DesignMode
        || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

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
        base.OnResize(e);

        CoreChart?.Update();
    }
}
