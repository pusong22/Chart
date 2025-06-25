using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Measuring;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;
public abstract class CoreHeatSeries<TValueType, TVisual>()
    : IHeatSeries
    where TVisual : BaseBitmapGeometry, new()
{
    private readonly List<Tuple<double, Color>> _heatStops = [];
    private Color[] _heatMap =
    [
        new (87, 103, 222, 255), // cold (min value)
        new (95, 207, 249, 255) // hot (max value)
    ];

    private readonly Bound _weightBound = new(0d, 0d);

    public IReadOnlyCollection<TValueType>? Values { get; set; }

    public Color[] HeatMap
    {
        get => _heatMap;
        set => _heatMap = value;
    }


    public double[]? ColorStops { get; set; }

    public int XIndex { get; set; }
    public int YIndex { get; set; }

    public object? Tag { get; set; }

    public Paint? HeatPaint { get; set; }

    public float CellHeight { get; set; } = 1f;
    public float CellWidth { get; set; } = 1f;


    public byte[] GenerateHeatMapBitmapData(int width, int height, IEnumerable<Coordinate> points)
    {
        var pixels = new byte[width * height * 4];
        var heatStops = BuildColorStops(HeatMap, ColorStops);

        foreach (var pt in points)
        {
            int x = (int)pt.X;
            int y = (int)pt.Y;
            double z = pt.Z;

            var color = InterpolateColor(z, _weightBound, HeatMap, heatStops);

            int i = (y * width + x) * 4;

            pixels[i + 0] = color.Red;
            pixels[i + 1] = color.Green;
            pixels[i + 2] = color.Blue;
            pixels[i + 3] = color.Alpha;
        }

        return pixels;
    }

    public void Invalidate(CartesianChart chart)
    {
        if (HeatPaint is null) return;

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


        //var uws = secondaryScaler.MeasureInPixels(1d);
        //var uwp = primaryScaler.MeasureInPixels(1d);


        //_heatStops = BuildColorStops(HeatMap, ColorStops);


        //foreach (var point in Fetch())
        //{
        //    var coordinate = point.Coordinate;

        //    var x = primaryScaler.ToPixel(coordinate.X);
        //    var y = secondaryScaler.ToPixel(coordinate.Y);
        //    var weight = coordinate.Z;

        //    var baseColor = InterpolateColor(weight, _weightBound, HeatMap, _heatStops);


        //    var heatVisual = new TVisual
        //    {
        //        X = x - uwp * 0.5f,
        //        Y = y - uws * 0.5f,
        //        Width = uwp,
        //        Height = uws,
        //        Color = new Color(baseColor.Red, baseColor.Green, baseColor.Blue, baseColor.Alpha)
        //    };

        //    chart.RequestGeometry(HeatPaint, heatVisual);
        //}


        var points = Fetch().ToList();
        if (!points.Any()) return;

        int width = (int)(points.Max(p => p.X) + 1);
        int height = (int)(points.Max(p => p.Y) + 1);


        var pixelData = GenerateHeatMapBitmapData(width, height, points);

        float left = primaryScaler.ToPixel(0);
        float right = primaryScaler.ToPixel(width);
        float top = secondaryScaler.ToPixel(height);
        float bottom = secondaryScaler.ToPixel(0);

        var bitmapGeometry = new TVisual
        {
            Width = width,
            Height = height,
            PixelData = pixelData,
            DestRect = new Rect(left, top, right - left, bottom - top),
        };

        chart.RequestGeometry(HeatPaint, bitmapGeometry);
    }

    public SeriesBound GetBound()
    {
        var primaryBound = new Bound(double.PositiveInfinity, double.NegativeInfinity);
        var secondaryBound = new Bound(double.PositiveInfinity, double.NegativeInfinity);
        foreach (var item in Fetch())
        {
            primaryBound.AppendValue(item.X);
            secondaryBound.AppendValue(item.Y);
            _weightBound.AppendValue(item.Z);
        }

        primaryBound.Expand(0.5d);
        secondaryBound.Expand(0.5d);

        return new SeriesBound()
        {
            PrimaryBound = primaryBound,
            SecondaryBound = secondaryBound
        };
    }

    public IEnumerable<Coordinate> Fetch()
    {
        if (Values is null) yield break;

        foreach (var value in Values.Cast<Coordinate>())
            yield return value;
    }

    public static List<Tuple<double, Color>> BuildColorStops(Color[] heatMap, double[]? colorStops)
    {
        if (heatMap.Length < 2) throw new Exception("At least 2 colors are required in a heat map.");

        if (colorStops is null)
        {
            var s = 1 / (double)(heatMap.Length - 1);
            colorStops = new double[heatMap.Length];
            var x = 0d;
            for (var i = 0; i < heatMap.Length; i++)
            {
                colorStops[i] = x;
                x += s;
            }
        }

        if (colorStops.Length != heatMap.Length)
            throw new Exception($"ColorStops and HeatMap must have the same length.");

        var heatStops = new List<Tuple<double, Color>>();
        for (var i = 0; i < colorStops.Length; i++)
        {
            heatStops.Add(new Tuple<double, Color>(colorStops[i], heatMap[i]));
        }

        return heatStops;
    }


    public static Color InterpolateColor(
        double weight,
        Bound weightBounds,
        Color[] heatMap,
        List<Tuple<double, Color>> heatStops)
    {
        var range = weightBounds.Maximum - weightBounds.Minimum;
        if (range == 0) range = double.Epsilon;
        var p = (weight - weightBounds.Minimum) / range;
        if (p < 0) p = 0;
        if (p > 1) p = 1;

        var previous = heatStops[0];

        for (var i = 1; i < heatStops.Count; i++)
        {
            var next = heatStops[i];

            if (next.Item1 < p)
            {
                previous = heatStops[i];
                continue;
            }

            var px = (p - previous.Item1) / (next.Item1 - previous.Item1);

            return new Color(
                (byte)(previous.Item2.Red + px * (next.Item2.Red - previous.Item2.Red)),
                (byte)(previous.Item2.Green + px * (next.Item2.Green - previous.Item2.Green)),
                (byte)(previous.Item2.Blue + px * (next.Item2.Blue - previous.Item2.Blue)),
                (byte)(previous.Item2.Alpha + px * (next.Item2.Alpha - previous.Item2.Alpha)));
        }

        return heatMap[heatMap.Length - 1];
    }
}
