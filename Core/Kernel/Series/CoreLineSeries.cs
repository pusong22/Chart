using Core.Helper;
using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Series;

public abstract class CoreLineSeries : CoreCartesianSeries
{
    private float _lineSmoothness;

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
    public double SampleInterval { get; set; } = 1d;
}

public abstract class CoreLineSeries<TValueType, TVisual, TPath>(IReadOnlyCollection<TValueType>? values) : CoreLineSeries
    where TVisual : BaseRectangleGeometry, new()
    where TPath : BaseVectoryGeometry<CubicBezierSegment>, new()
{
    private readonly Dictionary<Point, SeriesVisual> _caches = [];

    private TPath? _vectorGeometry;

    public Paint? StrokeGeometryPaint { get; set; }
    public Paint? FillGeometryPaint { get; set; }

    public override void Invalidate(CoreChart chart)
    {
        var cartesianChart = (CartesianChart)chart;
        var primaryAxis = cartesianChart.XAxes![XIndex];
        var secondaryAxis = cartesianChart.YAxes![YIndex];

        var drawnLocation = cartesianChart.DrawnLocation;
        var drawnSize = cartesianChart.DrawnSize;

        var primaryScaler = new Scaler(primaryAxis,
            new Point(primaryAxis.LabelDesiredRect.X, drawnLocation.Y),
            primaryAxis.NameDesiredRect.Size);
        var secondaryScaler = new Scaler(secondaryAxis,
            new Point(drawnLocation.X, secondaryAxis.LabelDesiredRect.Y),
            secondaryAxis.NameDesiredRect.Size);


        var coors = Fetch().ToList();

        int GetIndex(double val)
        {
            int index = (int)(val / SampleInterval);
            return index;
        }

        Bound GetBound(int i1, int i2)
        {
            double min = double.MaxValue, max = double.MinValue;
            for (int i = i1; i <= i2; i++)
            {
                if (min > coors[i].Y) min = coors[i].Y;
                if (max < coors[i].Y) max = coors[i].Y;
            }

            return new(min, max);
        }


        var width = primaryAxis.Max - primaryAxis.Min;
        var countPerPx =
            (width / SampleInterval) // 需要多少像素
            / primaryAxis.NameDesiredRect.Width; // 当前像素是否大于总像素


        var coordinates = new List<Coordinate>();

        if (countPerPx > 1)
        {
            double unitsPerPx = width / primaryAxis.NameDesiredRect.Width;

            for (int i = 0; i < primaryAxis.NameDesiredRect.Width; i++)
            {
                float px = primaryAxis.LabelDesiredRect.X + i;
                double min = primaryScaler.ToValue(px);
                double max = min + Math.Abs(unitsPerPx);

                int i1 = GetIndex(min);
                int i2 = GetIndex(max);

                if (i1 < 0 || i2 > (coors.Count - 1)) continue;

                var bound = GetBound(i1, i2);

                coordinates.Add(new(min, bound.Maximum));
            }
        }
        else
        {
            int i1 = GetIndex(primaryAxis.Min);
            int i2 = GetIndex(primaryAxis.Max);

            for (int i = i1; i <= i2; i++)
            {
                if (i < 0 || i > (coors.Count - 1)) continue;

                coordinates.Add(coors[i]);
            }
        }

        if (SeriesPaint is null) return;

        if (_vectorGeometry is null)
        {
            _vectorGeometry = new TPath();

            SeriesPaint.Style = PaintStyle.Stroke;
            _vectorGeometry.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
            chart.Canvas.AddDrawnTask(SeriesPaint, _vectorGeometry);
        }

        _vectorGeometry.Segments.Clear();

        var g = new HashSet<SeriesVisual>();


        foreach (var bezierData in GetCubicBezierSegment(coordinates.ToList()))
        {
            #region Vector 

            // Update
            if (bezierData.Head)
            {
                var start = new Point(primaryScaler.ToPixel(bezierData.Start.X), secondaryScaler.ToPixel(bezierData.Start.Y));

                _vectorGeometry.X = start.X;
                _vectorGeometry.Y = start.Y;
            }


            var end = new Point(primaryScaler.ToPixel(bezierData.End.X), secondaryScaler.ToPixel(bezierData.End.Y));
            var control1 = new Point(primaryScaler.ToPixel(bezierData.Control1.X), secondaryScaler.ToPixel(bezierData.Control1.Y));
            var control2 = new Point(primaryScaler.ToPixel(bezierData.Control2.X), secondaryScaler.ToPixel(bezierData.Control2.Y));


            if (!_caches.TryGetValue(end, out var seriesVisual))
            {
                seriesVisual = new SeriesVisual()
                {
                    End = bezierData.End,
                    Control1 = bezierData.Control1,
                    Control2 = bezierData.Control2,
                };

                _caches[end] = seriesVisual;
            }

            if (seriesVisual.Segment is null)
            {
                seriesVisual.Segment = new CubicBezierSegment();

                //seriesVisual.Segment.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
            }

            UpdateSegment(end, control1, control2, seriesVisual.Segment);

            _vectorGeometry.Segments.Add(seriesVisual.Segment);


            #endregion

            #region Mark
            if (StrokeGeometryPaint is not null)
            {
                if (seriesVisual.StrokeVisual is null)
                {
                    seriesVisual.StrokeVisual = new TVisual();
                    seriesVisual.StrokeVisual.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                }

                UpdateVisual(end, seriesVisual.StrokeVisual, VisualState.Display);

                StrokeGeometryPaint.Style = PaintStyle.Stroke;
                StrokeGeometryPaint.ZIndex = 1000;
                chart.Canvas.AddDrawnTask(StrokeGeometryPaint, seriesVisual.StrokeVisual);
            }

            if (FillGeometryPaint is not null)
            {
                if (seriesVisual.FillVisual is null)
                {
                    seriesVisual.FillVisual = new TVisual();
                    seriesVisual.FillVisual.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
                }

                UpdateVisual(end, seriesVisual.FillVisual, VisualState.Display);

                FillGeometryPaint.Style = PaintStyle.Fill;
                FillGeometryPaint.ZIndex = 999;
                chart.Canvas.AddDrawnTask(FillGeometryPaint, seriesVisual.FillVisual);
            }
            #endregion

            _ = g.Add(seriesVisual);
        }

        #region Old Remove
        foreach (var item in _caches.ToArray())
        {
            var seriesVisual = item.Value;
            if (g.Contains(seriesVisual)) continue;

            var end = new Point(primaryScaler.ToPixel(seriesVisual.End.X), secondaryScaler.ToPixel(seriesVisual.End.Y));
            var control1 = new Point(primaryScaler.ToPixel(seriesVisual.Control1.X), secondaryScaler.ToPixel(seriesVisual.Control1.Y));
            var control2 = new Point(primaryScaler.ToPixel(seriesVisual.Control2.X), secondaryScaler.ToPixel(seriesVisual.Control2.Y));


            if (seriesVisual.FillVisual is not null)
            {
                UpdateVisual(end, seriesVisual.FillVisual, VisualState.Remove);
            }

            if (seriesVisual.StrokeVisual is not null)
            {
                UpdateVisual(end, seriesVisual.StrokeVisual, VisualState.Remove);
            }

            if (seriesVisual.Segment is not null)
            {
                UpdateSegment(end, control1, control2, seriesVisual.Segment);
            }

            _caches.Remove(item.Key);
        }

        #endregion
    }

    private void UpdateVisual(
        Point end,
        BaseRectangleGeometry geometry,
        VisualState visualState)
    {
        geometry.X = end.X;
        geometry.Y = end.Y;
        geometry.Width = VisualGeometrySize / 2f;
        geometry.Height = VisualGeometrySize / 2f;

        ChangeVisualState(geometry, visualState);
    }

    private void UpdateSegment(
        Point end,
        Point c1,
        Point c2,
        CubicBezierSegment segment)
    {
        segment.End = end;
        segment.Control1 = c1;
        segment.Control2 = c2;
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

            yield return new BezierData(i == 0)
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
        double index = 0;
        if (values is null) yield break;

        foreach (var value in values.Cast<TValueType>())
        {
            yield return parser(index * SampleInterval, value!);
            index++;
        }
    }

    private void ChangeVisualState(DrawnGeometry geometry, VisualState visualState)
    {
        switch (visualState)
        {
            case VisualState.Display:
                geometry.Opacity = 1f;
                break;
            case VisualState.Remove:
                geometry.Opacity = 0f;
                geometry.Remove = true;
                break;
        }
    }

    public class BezierData(bool head)
    {
        public Coordinate Start { get; set; }
        public Coordinate Control1 { get; set; }
        public Coordinate Control2 { get; set; }
        public Coordinate End { get; set; }
        public bool Head { get; set; } = head;
    }

    public class SeriesVisual
    {
        public Coordinate Start { get; set; }
        public Coordinate Control1 { get; set; }
        public Coordinate Control2 { get; set; }
        public Coordinate End { get; set; }

        public TVisual? StrokeVisual { get; set; }
        public TVisual? FillVisual { get; set; }
        public CubicBezierSegment? Segment { get; set; }
    }
}

public enum VisualState
{
    Display, // 1
    Remove // 0
}
