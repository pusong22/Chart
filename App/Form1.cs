using Core.Kernel.Axis;
using Plot.WinForm;
using SkiaSharp;
using SkiaSharpBackend;
using SkiaSharpBackend.Painting;

namespace App;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        CoreAxis[] xAxes =
        [
            new Axis()
            {
                LabelPaint = new SolidColorPaint(),
                NamePaint = new SolidColorPaint(),
                Name = "TEST1",
                LabelSize = 12f,
                NameSize = 12f,
                NamePadding = new Core.Primitive.Padding(14f)
            }
        ];

        CoreAxis[] yAxes =
        [
            new Axis()
            {
                LabelPaint = new SolidColorPaint(),
                NamePaint = new SolidColorPaint(),
                Name = "TEST1",
                LabelSize = 12f,
                NameSize = 12f,
                NamePadding = new Core.Primitive.Padding(14f)
            }
        ];

        //DrawnDataArea area = new()
        //{
        //    Fill = new SolidColorPaint(new(60, 60, 60)),
        //    Stroke = new SolidColorPaint(new(255, 0, 0))
        //};


        CartesianChartControl chart = new()
        {
            Dock = DockStyle.Fill,
            XAxes = xAxes,
            YAxes = yAxes,
            //CoreDrawnDataArea = area
        };

        Controls.Add(chart);
    }
}
