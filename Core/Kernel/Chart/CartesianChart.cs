using Core.Default;
using Core.Kernel.Axis;
using Core.Kernel.Layout;
using Core.Primitive;
using Core.View;

namespace Core.Kernel.Chart;

public class CartesianChart(ICartesianChartView view, Canvas canvas) : CoreChart(view, canvas)
{
    public CoreCartesianAxis[] XAxes { get; private set; } = [];
    public CoreCartesianAxis[] YAxes { get; private set; } = [];
    public IEnumerable<CoreCartesianAxis> Axes => XAxes.Concat(YAxes);

    protected override void Measure()
    {
        var x = view.XAxes;
        var y = view.YAxes;

        if (x is null || x.Count() == 0 || y is null || y.Count() == 0)
        {
            var engine = DefaultSetting.GetEngine();
            x = [engine.GetDefaultCartesianAxis()];
            y = [engine.GetDefaultCartesianAxis()];
        }

        XAxes = x.Cast<CoreCartesianAxis>().ToArray();
        YAxes = y.Cast<CoreCartesianAxis>().ToArray();

        if (XAxes.Length == 0 || YAxes.Length == 0)
        {
            throw new Exception($"{nameof(XAxes)} and {nameof(YAxes)} must contain at least one element.");
        }

        // 初始化XAxes和YAxes的min/max，以及属性设置
        foreach (var axis in XAxes)
        {
            axis.Reset(AxisOrientation.X);
        }

        foreach (var axis in YAxes)
        {
            axis.Reset(AxisOrientation.Y);
        }


        BaseLayoutStrategy layoutStrategy = view.ElementLayoutKind switch
        {
            ElementLayoutKind.Stack => new StackedLayoutStrategy(this),
            ElementLayoutKind.Flow => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        DataRect = layoutStrategy.CalculateLayout();
    }

    protected override void Invalidate()
    {
        foreach (var axis in Axes)
        {
            axis.Invalidate(this);
        }


        Canvas.Invalidate();
    }
}
