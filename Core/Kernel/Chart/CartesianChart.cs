using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Layout;
using Core.Kernel.Series;
using Core.Kernel.View;
using Core.Kernel.Visual;
using Core.Primitive;

namespace Core.Kernel.Chart;

public class CartesianChart(ICartesianChartView view) : CoreChart(view)
{
    public CoreDrawnRect? CoreDrawnDataArea { get; private set; }
    public BaseLabelVisual? Title { get; private set; }

    public CoreCartesianAxis[]? XAxes { get; private set; }
    public CoreCartesianAxis[]? YAxes { get; private set; }
    public CoreCartesianSeries[]? Series { get; private set; }

    protected override void Initialize()
    {
        CoreDrawnDataArea = view.CoreDrawnDataArea;
        Title = view.Title as BaseLabelVisual;

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


        var s = view.Series;

        if (s == null || s.Count() == 0)
        {
            s = [];
        }

        Series = s.Cast<CoreCartesianSeries>().ToArray();

        foreach (var series in Series)
        {
            var xAxis = XAxes[series.XIndex];
            var yAxis = YAxes[series.YIndex];
            var seriesBound = series.GetBound();

            if (seriesBound.PrimaryBound is not null)
                xAxis.SetBound(seriesBound.PrimaryBound.Minimum, seriesBound.PrimaryBound.Maximum);

            if (seriesBound.SecondaryBound is not null)
                yAxis.SetBound(seriesBound.SecondaryBound.Minimum, seriesBound.SecondaryBound.Maximum);
        }

        foreach (var axis in XAxes)
        {
            axis.Reset(AxisOrientation.X);
        }

        foreach (var axis in YAxes)
        {
            axis.Reset(AxisOrientation.Y);
        }
    }

    protected override void Measure()
    {
        var margin = new Margin(0f);

        if (Title is not null)
        {
            var titleSize = Title.Measure();
            float xo = ControlSize.Width * .5f - titleSize.Width * .5f;
            float yo = 0f;

            Title.TextDesiredRect = new Rect(
                new Point(xo, yo),
                titleSize);

            margin.Top = titleSize.Height;
        }


        BaseLayoutStrategy layoutStrategy = view.LayoutKind switch
        {
            LayoutKind.Stack => new StackedLayoutStrategy(this),
            _ => throw new NotImplementedException(),
        };

        layoutStrategy.CalculateLayout(margin);
    }
    
    protected override void Invalidate()
    {
        CoreDrawnDataArea?.Invalidate(this);
        Title?.Invalidate(this);

        var axes = XAxes.Concat(YAxes);
        foreach (var axis in axes)
        {
            axis.Invalidate(this);
        }

        foreach (var s in Series!)
        {
            s.Invalidate(this);
        }
    }
}
