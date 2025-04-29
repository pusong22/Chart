using Core.Kernel.Axis;
using Plot.WinForm;
using SkiaSharp;
using SkiaSharpBackend;

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
                NamePaint = new SolidColorPaint(),
                Name = "TEST1\r\nTEST2\r\nTEST3",
                NameSize = 12f,
                NamePadding = new Core.Primitive.Padding(25f)
            }
        ];

        CoreAxis[] yAxes =
        [
            new Axis()
            {
                NamePaint = new SolidColorPaint(),
                Name = "TEST1\r\nTEST2\r\nTEST3",
                NameSize = 12f,
                //NamePadding = new Core.Primitive.Padding(15f)
            }
        ];


        CartesianChartControl chart = new()
        {
            Dock = DockStyle.Fill,
            XAxes = xAxes,
            YAxes = yAxes,
        };

        Controls.Add(chart);
    }
}
