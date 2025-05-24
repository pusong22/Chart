using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Painting;
using Core.Kernel.Series;
using SkiaSharpBackend;
using SkiaSharpBackend.Drawing;

namespace WpfSamples.ViewModel;
public class ChartViewModel
{
    public ChartViewModel()
    {
        ChartConfig.DisabledAnimation = true;
    }

    public CoreAxis[] XAxes { get; set; } = [
        new Axis() { Name = "Time", Labeler=l=>l.ToString("N2"),TickPaint = new Pen() }


    ];

    public CoreAxis[] YAxes { get; set; } = [
        new Axis() { Name = "Magnitude", Labeler=l=>l.ToString("N2"),TickPaint = new Pen() },
        //new Axis() { Name = "Y Axis2", ShowSeparatorLine = false, DrawLine = false }


    ];

    public CoreLineSeries[] Series { get; set; } = [
        new LineSeries<double>(Fetch().ToList())
        {
            LineSmoothness = 0.85f,
            VisualGeometrySize = 20f,
            SampleInterval = 1/10d,
        },
        //new LineSeries<double>([-1, -5, 10, 3, -20, 2, 7, 7, 8, 10])
        //{
        //    StrokeGeometryPaint = new Paint() { IsAntialias = true, Color = new(0, 0, 255)  },
        //    FillGeometryPaint = new Paint() { IsAntialias = true, Color = new(255, 255, 255) },
        //    LineSmoothness = 0.5f,
        //    VisualGeometrySize = 20f,
        //    YIndex = 1
        //},
    ];

    private static IEnumerable<double> Fetch()
    {
        for (double x = 0; x < 100; x += 0.1)
        {
            yield return Math.Sin(x);
        }
    }
}
