using Core.Kernel.View;
using Core.Primitive;

namespace Core.Kernel.Chart;
public abstract class CoreChart(IChartView view, Canvas canvas)
{
    private readonly object _sync = new();

    public Canvas Canvas { get; } = canvas;
    public Point DrawnLocation { get; protected internal set; }
    public Size DrawnSize { get; protected internal set; }
    public bool IsLoad { get; private set; }
    public Size ControlSize { get; private set; }


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
        view.InvokeUIThread(() =>
        {
            lock (_sync)
            {
                ControlSize = view.ControlSize;
                Measure();
                Invalidate();
                Canvas.Invalidate();
            }
        });
    }
}
