using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Layout;
using Core.Kernel.Painting;
using Core.Kernel.Visual;
using Core.Primitive;

namespace Core.Kernel;

public class CartesianChart(ICartesianChartView view)
{
    private bool _drawing = false;
    private readonly HashSet<Paint> _currentPaints = [];

    public Point DrawnLocation { get; protected internal set; }
    public Size DrawnSize { get; protected internal set; }
    public bool IsLoad { get; private set; }
    public Size ControlSize { get; private set; }

    public IBaseLabelVisual? Title { get; private set; }

    public ICartesianAxis[]? XAxes { get; private set; }
    public ICartesianAxis[]? YAxes { get; private set; }
    public ICartesianSeries[]? Series { get; private set; }

    public void Load()
    {
        IsLoad = true;
    }

    public void UnLoad()
    {
        IsLoad = false;
        Return();
    }

    public async Task UpdateAsync(ChatModelSnapshot snapshot)
    {
        if (!IsLoad || _drawing) return;

        _drawing = true;

        ControlSize = snapshot.ControlSize;

        Title = snapshot.Title;

        XAxes = snapshot.XAxes?.ToArray() ?? [];
        YAxes = snapshot.YAxes?.ToArray() ?? [];
        Series = snapshot.Series?.ToArray() ?? [];

        // BUG: 释放时机
        await Task.Run(() =>
        {
            Return();
            InitializeInternal();
            MeasureInternal();
            CalculateGeometriesInternal();
        });

        ChartDrawingCommand command = new(_currentPaints);
        view.RequestInvalidateVisual(command);

        _drawing = false;
    }

    protected void InitializeInternal()
    {
        // Known Issue: When multiple series share a common X-axis but have different sampling rates or time spans,
        // the axis bounds may be dominated by the smaller-range series, leading to incomplete rendering of others.
        foreach (var series in Series!)
        {
            var xAxis = XAxes![series.XIndex];
            var yAxis = YAxes![series.YIndex];
            var seriesBound = series.GetBound();

            if (seriesBound.PrimaryBound is not null)
                xAxis.SetBound(seriesBound.PrimaryBound.Minimum, seriesBound.PrimaryBound.Maximum);

            if (seriesBound.SecondaryBound is not null)
                yAxis.SetBound(seriesBound.SecondaryBound.Minimum, seriesBound.SecondaryBound.Maximum);
        }

        foreach (var axis in XAxes!)
            axis.Reset(AxisOrientation.X);

        foreach (var axis in YAxes!)
            axis.Reset(AxisOrientation.Y);
    }

    //protected void InitializeInternal()
    //{
    //    var xAxisBounds = new Dictionary<ICartesianAxis, (float min, float max)>();
    //    var yAxisBounds = new Dictionary<ICartesianAxis, (float min, float max)>();

    //    foreach (var series in Series!)
    //    {
    //        var xAxis = XAxes![series.XIndex];
    //        var yAxis = YAxes![series.YIndex];
    //        var seriesBound = series.GetBound();

    //        if (seriesBound.PrimaryBound is not null)
    //        {
    //            if (!xAxisBounds.TryGetValue(xAxis, out var currentXBound))
    //            {
    //                xAxisBounds[xAxis] = (seriesBound.PrimaryBound.Minimum, seriesBound.PrimaryBound.Maximum);
    //            }
    //            else
    //            {
    //                xAxisBounds[xAxis] = (
    //                    Math.Min(currentXBound.min, seriesBound.PrimaryBound.Minimum),
    //                    Math.Max(currentXBound.max, seriesBound.PrimaryBound.Maximum)
    //                );
    //            }
    //        }

    //        if (seriesBound.SecondaryBound is not null)
    //        {
    //            if (!yAxisBounds.TryGetValue(yAxis, out var currentYBound))
    //            {
    //                yAxisBounds[yAxis] = (seriesBound.SecondaryBound.Minimum, seriesBound.SecondaryBound.Maximum);
    //            }
    //            else
    //            {
    //                yAxisBounds[yAxis] = (
    //                    Math.Min(currentYBound.min, seriesBound.SecondaryBound.Minimum),
    //                    Math.Max(currentYBound.max, seriesBound.SecondaryBound.Maximum)
    //                );
    //            }
    //        }
    //    }

    //    foreach (var entry in xAxisBounds)
    //    {
    //        entry.Key.SetBound(entry.Value.min, entry.Value.max);
    //        entry.Key.Reset(AxisOrientation.X);
    //    }

    //    foreach (var entry in yAxisBounds)
    //    {
    //        entry.Key.SetBound(entry.Value.min, entry.Value.max);
    //        entry.Key.Reset(AxisOrientation.Y);
    //    }
    //}

    protected void MeasureInternal()
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

        BaseLayoutStrategy layoutStrategy = new StackedLayoutStrategy(this);
        layoutStrategy.CalculateLayout(margin);
    }

    protected void CalculateGeometriesInternal()
    {
        Title?.CalculateGeometries(this);

        var axes = XAxes.Concat(YAxes);
        foreach (var axis in axes)
            axis.CalculateGeometries(this);

        foreach (var s in Series!)
            s.CalculateGeometries(this);
    }

    public void RequestGeometry(Paint paint, DrawnGeometry geometry)
    {
        _currentPaints.Add(paint);
        geometry.Paint = paint;
        geometry.Opacity = 1f;

        paint.Geometries.Add(geometry);
    }

    public void Return()
    {
        foreach (var paint in _currentPaints)
        {
            paint.Geometries.Clear();
        }

        _currentPaints.Clear();
    }
}
