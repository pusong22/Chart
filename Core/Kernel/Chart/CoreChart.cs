using Core.Kernel.View;
using Core.Primitive;

namespace Core.Kernel.Chart;
public abstract class CoreChart(IChartView view, Canvas canvas)
{
    private readonly object _sync = new();

    public Canvas Canvas { get; } = canvas;
    public Rect DataRect { get; set; }
    public Point DrawnLocation { get; set; }
    public Size DrawnSize { get; set; }

    public Size ControlSize { get => view.ControlSize; }


    protected abstract void Measure();
    protected abstract void Invalidate();

    public void Load()
    {

        Update();
    }

    public void Update()
    {
        view.InvokeUIThread(() =>
        {
            lock (_sync)
            {
                Measure();
                Invalidate();
            }
        });
    }
}
