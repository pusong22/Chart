using Core.Helper;
using SkiaSharpBackend;
using SkiaSharpBackend.Drawing;
using WinForm;

namespace App;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        ChartConfig.DisabledAnimation = true;

        Core.Primitive.Color gray = new(169, 169, 169, 169);
        Core.Primitive.Color red = new(255, 0, 0);

        DrawnDataArea area = new()
        {
            Fill = new Core.Kernel.Painting.Brush() { Color = gray },
            Stroke = new Core.Kernel.Painting.Pen() { Color = red },
        };


        CartesianChartControl chart = new()
        {
            Dock = DockStyle.Fill,
            XAxes = [new Axis() { Name = "X Axis" }],
            YAxes = [
                new Axis() { Name = "Y Axis1"},
                //new Axis() { Name = "Y Axis2", ShowSeparatorLine = false, DrawLine = false, },
                //new Axis() { Name = "Y Axis3"},
                //new Axis() { Name = "Y Axis4"},
                //new Axis() { Name = "Y Axis5"},
                //new Axis() { Name = "Y Axis6"},
                //new Axis() { Name = "Y Axis7"},
                //new Axis() { Name = "Y Axis8"},
                //new Axis() { Name = "Y Axis9"},
                //new Axis() { Name = "Y Axis10"},
                //new Axis() { Name = "Y Axis11"},
            ],
            Series = [
                new LineSeries<double>([-1,-5,10,3,4,5,6,7,8,9])
                {
                    LineSmoothness = 0.5f,
                    VisualGeometrySize = 20f,
                    SampleInterval = 1/5d,
                },
                //new LineSeries<double>([-1,-5,10,3,4,5,6,7,8,9])
                //{
                //    StrokeGeometryPaint = new Paint(){IsAntialias = true, Color=new(0, 0, 255)  },
                //    FillGeometryPaint = new Paint(){IsAntialias = true, Color=new(255, 255, 255) },
                //    LineSmoothness = 0.5f,
                //    VisualGeometrySize = 20f,
                //    YIndex = 1
                //},
            ]
            //CoreDrawnDataArea = area
        };

        Controls.Add(chart);
    }
}
