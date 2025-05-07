using Core.Helper;
using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Series;

public abstract class CoreSeries : ChartElement
{

}

// Cartesian系列需要
public abstract class CoreCartesianSeries : CoreSeries
{
    private int _xIndex;
    private int _yIndex;

    public int XIndex
    {
        get => _xIndex;
        set => _xIndex = value;
    }

    public int YIndex
    {
        get => _yIndex;
        set => _yIndex = value;
    }

    public Paint? LinePaint { get; set; }

    public abstract IEnumerable<Coordinate> Fetch();

    public virtual SeriesBound GetBound()
    {
        var primaryaryBound = new Bound(0d, 0d);
        var secondaryBound = new Bound(0d, 0d);
        foreach (var item in Fetch())
        {
            primaryaryBound.AppendValue(item.X);
            secondaryBound.AppendValue(item.Y);
        }

        return new SeriesBound()
        {
            PrimaryaryBound = primaryaryBound,
            SecondaryBound = secondaryBound
        };
    }
}

public abstract class CoreLineSeries : CoreCartesianSeries
{
    private float _lineSmoothness = 0.65f;

    public float LineSmoothness
    {
        get => _lineSmoothness;
        set
        {
            if (value < 0) value = 0;
            if (value > 1) value = 1;
            _lineSmoothness = value;
        }
    }

    public float VisualGeometrySize { get; set; }

}

public abstract class CoreLineSeries<TValueType, TVisual, TPath>(IReadOnlyCollection<TValueType>? values) : CoreLineSeries
    where TVisual : BaseRectangleGeometry, new()
    where TPath : BaseVectoryGeometry<CubicBezierSegment>, new()
{
    public Paint? StrokeGeometryPaint { get; set; }
    public Paint? FillGeometryPaint { get; set; }

    bool first = true;
    public override void Invalidate(CoreChart chart)
    {
        var cartesianChart = (CartesianChart)chart;
        var primaryAxis = cartesianChart.XAxes![XIndex];
        var secondaryAxis = cartesianChart.YAxes![YIndex];

        var drawnLocation = cartesianChart.DrawnLocation;
        var drawnSize = cartesianChart.DrawnSize;

        // TODO: 根据x/y切出一个Rect
        var primaryScaler = new Scaler(primaryAxis, drawnLocation, drawnSize);
        var secondaryScaler = new Scaler(secondaryAxis, drawnLocation, drawnSize);

        var coordinates = Fetch();

#if DEBUG

        if (first)
        {
            TPath path = new();

            foreach (var bezierData in GetCubicBezierSegment(coordinates.ToList()))
            {
                var start = new Point(primaryScaler.ToPixel(bezierData.Start.X), secondaryScaler.ToPixel(bezierData.Start.Y));
                var end = new Point(primaryScaler.ToPixel(bezierData.End.X), secondaryScaler.ToPixel(bezierData.End.Y));
                var control1 = new Point(primaryScaler.ToPixel(bezierData.Control1.X), secondaryScaler.ToPixel(bezierData.Control1.Y));
                var control2 = new Point(primaryScaler.ToPixel(bezierData.Control2.X), secondaryScaler.ToPixel(bezierData.Control2.Y));

                var geometry = new TVisual()
                {
                    X = end.X,
                    Y = end.Y,
                    Width = VisualGeometrySize / 2f,
                    Height = VisualGeometrySize / 2f
                };

                if (StrokeGeometryPaint is not null)
                {
                    StrokeGeometryPaint.Style = PaintStyle.Stroke;
                    StrokeGeometryPaint.ZIndex = 1000;
                    chart.Canvas.AddDrawnTask(StrokeGeometryPaint, geometry);
                }

                if (FillGeometryPaint is not null)
                {
                    FillGeometryPaint.Style = PaintStyle.Fill;
                    FillGeometryPaint.ZIndex = 999;
                    chart.Canvas.AddDrawnTask(FillGeometryPaint, geometry);
                }

                path.Segments.AddLast(new CubicBezierSegment()
                {
                    Start = start,
                    End = end,
                    Control1 = control1,
                    Control2 = control2,
                });
            }

            if (LinePaint is not null)
            {
                LinePaint.Style = PaintStyle.Stroke;
                chart.Canvas.AddDrawnTask(LinePaint, path);
            }

            first = false;
        }

        return;
#endif
    }

    private IEnumerable<BezierData> GetCubicBezierSegment(List<Coordinate> coordinates)
    {
        if (coordinates == null || coordinates.Count < 2)
            yield break;

        for (int i = 0; i < coordinates.Count - 1; i++)
        {
            var p0 = i == 0 ? coordinates[i] : coordinates[i - 1];
            var p1 = coordinates[i];
            var p2 = coordinates[i + 1];
            var p3 = i + 2 < coordinates.Count ? coordinates[i + 2] : p2;

            // 计算p1-p2，需要用到p0-p3这4个点来
            // Catmull-Rom to Bezier conversion
            var cp1 = new Coordinate(
                p1.X + (p2.X - p0.X) / 6.0 * LineSmoothness,
                p1.Y + (p2.Y - p0.Y) / 6.0 * LineSmoothness);

            var cp2 = new Coordinate(
                p2.X - (p3.X - p1.X) / 6.0 * LineSmoothness,
                p2.Y - (p3.Y - p1.Y) / 6.0 * LineSmoothness);

            yield return new BezierData()
            {
                Start = p1,
                Control1 = cp1,
                Control2 = cp2,
                End = p2
            };
        }
    }


    public override IEnumerable<Coordinate> Fetch()
    {
        var parser = ChartConfig.Instance.GetParser<TValueType>();
        int index = 0;
        if (values is null) yield break;

        foreach (var value in values.Cast<TValueType>())
        {
            yield return parser(index++, value!);
        }
    }

    public class BezierData
    {
        public Coordinate Start { get; set; }
        public Coordinate Control1 { get; set; }
        public Coordinate Control2 { get; set; }
        public Coordinate End { get; set; }
    }
}
