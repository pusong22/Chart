using Core.Kernel.View;
using Core.Primitive;

namespace Core.Kernel.Chart;
public abstract class CoreChart(IChartView view)
{
    public CanvasContext CanvasContext { get; private set; } = view.CanvasContext;
    public Point DrawnLocation { get; protected internal set; }
    public Size DrawnSize { get; protected internal set; }
    public bool IsLoad { get; private set; }
    public Size ControlSize { get; private set; } = view.ControlSize;

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

        view.InvokeUIThread(() =>
        {
            CanvasContext = view.CanvasContext;
            ControlSize = view.ControlSize;

            Measure();
            Invalidate();

            CanvasContext.Invalidate();
        });
    }
}
