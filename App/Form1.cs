using Core.Kernel.Painting;
using Plot.WinForm;
using SkiaSharpBackend.Drawing;

namespace App;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        Core.Primitive.Color gray = new(169, 169, 169, 169);
        Core.Primitive.Color red = new(255, 0, 0);

        DrawnDataArea area = new()
        {
            Fill = new Paint() { Color = gray },
            Stroke = new Paint() { Color = red },
        };


        CartesianChartControl chart = new()
        {
            Dock = DockStyle.Fill,
            XAxes = [new Axis() { Name = "X Axis", ShowSeparatorLine = false, DrawTickPath = false, }],
            YAxes = [
                new Axis() { Name = "Y Axis1", ShowSeparatorLine = false, DrawTickPath = false, },
                //new Axis() { Name = "Y Axis2"},
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
            CoreDrawnDataArea = area
        };

        Controls.Add(chart);
    }
}
