using Core.Primitive;
using Core.View;

namespace Core.Kernel.Chart;
public abstract class CoreChart(IChartView view, Canvas canvas)
{
    private readonly object _sync = new();

    #region Configure
    public bool EnableLog { get; set; } = false;
    #endregion

    public Canvas Canvas { get; private set; } = canvas;
    public Rect DataRect { get; set; }

    public Size ScaledControlSize => new(
        view.ControlSize.Width / view.DisplayScale,
        view.ControlSize.Height / view.DisplayScale);

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
