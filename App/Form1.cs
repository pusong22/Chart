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
                Name = "TEST\r\naaaa",
                NameSize = 12f
            }
        ];

        CoreAxis[] yAxes =
        [
            new Axis()
            {

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
