using Core.Helper;
using Core.Kernel.View;
using Core.Primitive;

namespace Core.Kernel.Chart;
public abstract class CoreChart(IChartView view)
{
    public CanvasContext CanvasContext { get; private set; } = view.CanvasContext;
    public Point DrawnLocation { get; protected internal set; }
    public Size DrawnSize { get; protected internal set; }
    public bool IsLoad { get; private set; }
    public Size ControlSize { get; private set; }

    protected abstract bool Initialize();

    protected abstract void Measure();

    protected abstract void Invalidate();

    public void Load()
    {
        IsLoad = true;
        Update();
    }

    public void UnLoad()
    {
        IsLoad = false;
    }

    public void Update()
    {
        if (!IsLoad) return;

#if DEBUG
        if (ChartConfig.EnableLog)
        {

        }
#endif

        view.InvokeUIThread(() =>
        {
            CanvasContext = view.CanvasContext;
            ControlSize = view.ControlSize;

            if (!Initialize()) return;
            Measure();
            Invalidate();

            CanvasContext.Invalidate();
        });
    }
}
