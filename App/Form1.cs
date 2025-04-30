using Core.Kernel.Axis;
using Plot.WinForm;
using SkiaSharp;
using SkiaSharpBackend;
using SkiaSharpBackend.Drawing;
using SkiaSharpBackend.Painting;

namespace App;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        SKColor gray = new(169, 169, 169, 169);
        SKColor red = new(255, 0, 0);

        CoreCartesianAxis[] xAxes =
        [
            new Axis()
            {
                LabelPaint = new SolidColorPaint(),
                NamePaint = new SolidColorPaint(),
                TickPaint = new SolidColorPaint(),
                Name = "X Axis",
                LabelSize = 12f,
                NameSize = 12f,
                NamePadding = new Core.Primitive.Padding(5f),
                LabelPadding = new Core.Primitive.Padding(15f),
            }
        ];

        CoreCartesianAxis[] yAxes =
        [
            new Axis()
            {
                LabelPaint = new SolidColorPaint(),
                NamePaint = new SolidColorPaint(),
                TickPaint = new SolidColorPaint(),
                Name = "Y Axis",
                LabelSize = 12f,
                NameSize = 12f,
                NamePadding = new Core.Primitive.Padding(5f),
                LabelPadding = new Core.Primitive.Padding(15f),
            }
        ];

        DrawnDataArea area = new()
        {
            Fill = new SolidColorPaint(gray),
            Stroke = new SolidColorPaint(red)
        };


        CartesianChartControl chart = new()
        {
            Dock = DockStyle.Fill,
            XAxes = xAxes,
            YAxes = yAxes,
            CoreDrawnDataArea = area
        };

        Controls.Add(chart);
    }
}
