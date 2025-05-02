using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Layout;
using Core.Kernel.View;
using Core.Primitive;

namespace Core.Kernel.Chart;

public class CartesianChart(ICartesianChartView view, Canvas canvas)
    : CoreChart(view, canvas)
{
    public CoreDrawnDataArea? CoreDrawnDataArea => view.CoreDrawnDataArea;

    public CoreCartesianAxis[] XAxes { get; private set; } = [];
    public CoreCartesianAxis[] YAxes { get; private set; } = [];
    public IEnumerable<CoreCartesianAxis> Axes => XAxes.Concat(YAxes);

    protected override void Measure()
    {
#if DEBUG
        if (ChartConfig.EnableLog)
        {

        }
#endif
        var x = view.XAxes;
        var y = view.YAxes;

        var instance = ChartConfig.Instance;

        if (x is null || x.Count() == 0 || y is null || y.Count() == 0)
        {
            var provider = instance.GetProvider();
            x = [provider.GetAxis()];
            y = [provider.GetAxis()];
        }

        XAxes = x.Cast<CoreCartesianAxis>().ToArray();
        YAxes = y.Cast<CoreCartesianAxis>().ToArray();

        if (XAxes.Length == 0 || YAxes.Length == 0)
        {
            throw new Exception($"{nameof(XAxes)} and {nameof(YAxes)} must contain at least one element.");
        }

        foreach (var axis in XAxes)
        {
            if (instance != axis.Tag)
            {
                instance.ApplyStyleToAxis(axis);
                axis.Tag = instance;
            }

            axis.Reset(AxisOrientation.X);
        }

        foreach (var axis in YAxes)
        {
            if (instance != axis.Tag)
            {
                instance.ApplyStyleToAxis(axis);
                axis.Tag = instance;
            }

            axis.Reset(AxisOrientation.Y);
        }


        BaseLayoutStrategy layoutStrategy = view.LayoutKind switch
        {
            LayoutKind.Stack => new StackedLayoutStrategy(this),
            _ => throw new NotImplementedException(),
        };

        layoutStrategy.CalculateLayout();
    }

    protected override void Invalidate()
    {
        Canvas.IsCompleted = false;

        CoreDrawnDataArea?.Invalidate(this);

        foreach (var axis in Axes)
        {
            axis.Invalidate(this);
        }

        Canvas.Invalidate();
    }
}
