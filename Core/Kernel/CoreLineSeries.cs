using Core.Helper;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;

public abstract class CoreLineSeries<TValueType, TVisual, TPath> : ILineSeries
    where TVisual : BaseRectangleGeometry, new()
    where TPath : BaseVectoryGeometry<CubicBezierSegment>, new()
{
    public IReadOnlyCollection<TValueType>? Values { get; set; }

    private class BezierData(bool head)
    {
        public Coordinate Start { get; set; }
        public Coordinate Control1 { get; set; }
        public Coordinate Control2 { get; set; }
        public Coordinate End { get; set; }
        public bool Head { get; set; } = head;
    }

    private class SeriesVisual
    {
        public Coordinate Start { get; set; }
        public Coordinate Control1 { get; set; }
        public Coordinate Control2 { get; set; }
        public Coordinate End { get; set; }

        public TVisual? StrokeVisual { get; set; }
        public TVisual? FillVisual { get; set; }
    }

    private TPath? _vectorGeometry;

    public object? Tag { get; }
    public Paint? StrokeGeometryPaint { get; set; }
    public Paint? FillGeometryPaint { get; set; }

    private Paint? _seriesPaint = new Pen();
    private float _lineSmoothness;
    private int _xIndex;
    private int _yIndex;

    public float VisualGeometrySize { get; set; }
    public double SampleInterval { get; set; } = 1d;


    public Paint? SeriesPaint
    {
        get => _seriesPaint;
        set
        {
            if (value != _seriesPaint)
            {
                _seriesPaint = value;
            }
        }
    }

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

    public double XOffset { get; set; } = 0d;

    public virtual SeriesBound GetBound()
    {
        var primaryBound = new Bound(double.PositiveInfinity, double.NegativeInfinity);
        var secondaryBound = new Bound(double.PositiveInfinity, double.NegativeInfinity);
        foreach (var item in Fetch())
        {
            primaryBound.AppendValue(item.X);
            secondaryBound.AppendValue(item.Y);
        }

        primaryBound.Expand(0.15d);
        secondaryBound.Expand(0.15d);

        return new SeriesBound()
        {
            PrimaryBound = primaryBound,
            SecondaryBound = secondaryBound
        };
    }

    public void Invalidate(CartesianChart chart)
    {
        if (SeriesPaint is null) return;

        var primaryAxis = chart.XAxes![XIndex];
        var secondaryAxis = chart.YAxes![YIndex];

        var primaryScaler = new Scaler(false,
            primaryAxis.LabelDesiredRect.X,
            primaryAxis.LabelDesiredRect.X + primaryAxis.NameDesiredRect.Width,
            primaryAxis.Min,
            primaryAxis.Max);

        var secondaryScaler = new Scaler(true,
            secondaryAxis.LabelDesiredRect.Y,
            secondaryAxis.LabelDesiredRect.Y + secondaryAxis.NameDesiredRect.Height,
            secondaryAxis.Min,
            secondaryAxis.Max);

        var coordinates = ReduceDensity(primaryAxis, primaryScaler);

        _vectorGeometry = new TPath();
        chart.RequestGeometry(SeriesPaint, _vectorGeometry);

        _vectorGeometry.Segments.Clear();

        foreach (var bezierData in GetCubicBezierSegment([.. coordinates]))
        {
            var start = ToPixel(bezierData.Start, primaryScaler, secondaryScaler);

            if (bezierData.Head)
            {
                _vectorGeometry.X = start.X;
                _vectorGeometry.Y = start.Y;
            }

            var end = ToPixel(bezierData.End, primaryScaler, secondaryScaler);
            var control1 = ToPixel(bezierData.Control1, primaryScaler, secondaryScaler);
            var control2 = ToPixel(bezierData.Control2, primaryScaler, secondaryScaler);

            var seriesVisual = new SeriesVisual()
            {
                End = bezierData.End,
                Control1 = bezierData.Control1,
                Control2 = bezierData.Control2,
            };

            var segment = new CubicBezierSegment();

            UpdateSegment(end, control1, control2, segment);

            _vectorGeometry.Segments.Add(segment);

            DrawnStrokeGeometry(chart, end, seriesVisual);

            DrawnFillGeometry(chart, end, seriesVisual);
        }
    }

    #region Draw
    private void DrawnFillGeometry(CartesianChart chart, Point end, SeriesVisual seriesVisual)
    {
        if (FillGeometryPaint is null) return;

        seriesVisual.FillVisual = new TVisual();
        chart.RequestGeometry(FillGeometryPaint, seriesVisual.FillVisual);

        UpdateVisual(end, seriesVisual.FillVisual);

        FillGeometryPaint.Style = PaintStyle.Fill;
        FillGeometryPaint.ZIndex = 999;
    }

    private void DrawnStrokeGeometry(CartesianChart chart, Point end, SeriesVisual seriesVisual)
    {
        if (StrokeGeometryPaint is null) return;

        seriesVisual.StrokeVisual = new TVisual();
        chart.RequestGeometry(StrokeGeometryPaint, seriesVisual.StrokeVisual);

        UpdateVisual(end, seriesVisual.StrokeVisual);

        StrokeGeometryPaint.Style = PaintStyle.Stroke;
        StrokeGeometryPaint.ZIndex = 1000;
    }

    #endregion

    #region Update Visual
    private void UpdateVisual(
        Point end,
        BaseRectangleGeometry geometry)
    {
        geometry.X = end.X;
        geometry.Y = end.Y;
        geometry.Width = VisualGeometrySize / 2f;
        geometry.Height = VisualGeometrySize / 2f;
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

    #endregion

    public IEnumerable<Coordinate> Fetch()
    {
        var parser = ChartConfig.Instance.GetParser<TValueType>();
        double index = 0;
        if (Values is null) yield break;

        foreach (var value in Values)
        {
            yield return parser(XOffset + index * SampleInterval, value!);
            index++;
        }
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


    private Point ToPixel(Coordinate coordinate, Scaler primaryScaler, Scaler secondaryScaler)
    {
        return new Point(primaryScaler.ToPixel(coordinate.X), secondaryScaler.ToPixel(coordinate.Y));
    }

    private IEnumerable<Coordinate> ReduceDensity(ICartesianAxis primaryAxis, Scaler primaryScaler)
    {
        var coors = Fetch().ToList();

        int GetIndex(double val)
        {
            int index = (int)Math.Floor((val - XOffset) / SampleInterval);
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
            width / SampleInterval // 需要多少像素
            / primaryAxis.NameDesiredRect.Width; // 当前像素是否大于总像素


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

                if (i1 < 0 || i2 > coors.Count - 1) continue;

                var bound = GetBound(i1, i2);

                yield return new Coordinate(min, bound.Maximum);
            }
        }
        else
        {
            int i1 = GetIndex(primaryAxis.Min);
            int i2 = GetIndex(primaryAxis.Max);

            for (int i = i1; i <= i2; i++)
            {
                if (i < 0 || i > coors.Count - 1) continue;

                yield return coors[i];
            }
        }
    }

}

