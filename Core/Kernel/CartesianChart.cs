using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Layout;
using Core.Kernel.Painting;
using Core.Kernel.Visual;
using Core.Primitive;

namespace Core.Kernel;

public class CartesianChart(ICartesianChartView view)
{
    private readonly HashSet<Paint> _currentPaints = [];

    public Point DrawnLocation { get; protected internal set; }
    public Size DrawnSize { get; protected internal set; }
    public bool IsLoad { get; private set; }
    public Size ControlSize { get; private set; }

    public IBaseLabelVisual? Title { get; private set; }

    public ICartesianAxis[]? XAxes { get; private set; }
    public ICartesianAxis[]? YAxes { get; private set; }
    public ICartesianSeries[]? Series { get; private set; }

    public event EventHandler? RedrawHandler;

    public void Load()
    {
        IsLoad = true;
    }

    public void UnLoad()
    {
        IsLoad = false;
    }

    public async Task UpdateAsync(
      Size controlSize,
      IBaseLabelVisual? title,
      IEnumerable<ICartesianAxis>? xAxes,
      IEnumerable<ICartesianAxis>? yAxes,
      IEnumerable<ICartesianSeries>? series)
    {
        if (!IsLoad) return;

        ControlSize = controlSize;

        Title = title;
        XAxes = xAxes?.ToArray() ?? [];
        YAxes = yAxes?.ToArray() ?? [];
        Series = series?.ToArray() ?? [];

        await Task.Run(() =>
        {
            Return();
            InitializeInternal();
            MeasureInternal();
            CalculateGeometriesInternal();
        });

        if (IsLoad)
            view.RequestInvalidateVisual();
    }

    protected void InitializeInternal()
    {
        XAxes = [.. XAxes.Cast<ICartesianAxis>()];
        YAxes = [.. YAxes.Cast<ICartesianAxis>()];

        Series = [.. Series.Cast<ICartesianSeries>()];

        // Known Issue: When multiple series share a common X-axis but have different sampling rates or time spans,
        // the axis bounds may be dominated by the smaller-range series, leading to incomplete rendering of others.
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
            axis.Reset(AxisOrientation.X);

        foreach (var axis in YAxes)
            axis.Reset(AxisOrientation.Y);
    }

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

    public void DrawFrame<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext
    {
        if (!_currentPaints.Any()) return;

        context.BeginDraw();

        foreach (var paint in _currentPaints.Where(x => x is not null).OrderBy(x => x.ZIndex))
        {
            context.InitializePaint(paint);

            foreach (var geometry in paint.Geometries)
                context.Draw(geometry);

            context.DisposePaint();
        }

        context.EndDraw();
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
