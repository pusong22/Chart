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
        new Axis() { Name = "Time", ShowSeparatorLine = true, DrawTickPath = true, LabelRotation = -45 }


    ];

    public CoreAxis[] YAxes { get; set; } = [
        new Axis() { Name = "Magnitude", ShowSeparatorLine = false, DrawTickPath = true , LabelRotation = -45},
        new Axis() { Name = "Y Axis2", ShowSeparatorLine = false, DrawTickPath = false }


    ];

    public CoreLineSeries[] Series { get; set; } = [
        new LineSeries<double>([-1, -5, 10, 3, 4, 5, 6, 7, 8, 9])
        {
            StrokeGeometryPaint = new Paint() { IsAntialias = true, Color = new(0, 0, 255)  },
            FillGeometryPaint = new Paint() { IsAntialias = true, Color = new(255, 255, 255) },
            LineSmoothness = 0.5f,
            VisualGeometrySize = 20f,
            SampleInterval = 1/1d,
        },
        new LineSeries<double>([-1, -5, 10, 3, -20, 2, 7, 7, 8, 10])
        {
            StrokeGeometryPaint = new Paint() { IsAntialias = true, Color = new(0, 0, 255)  },
            FillGeometryPaint = new Paint() { IsAntialias = true, Color = new(255, 255, 255) },
            LineSmoothness = 0.5f,
            VisualGeometrySize = 20f,
            YIndex = 1
        },
    ];

}
