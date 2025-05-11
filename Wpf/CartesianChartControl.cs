using Core.Kernel;
using Core.Kernel.Chart;
using Core.Kernel.View;
using Core.Primitive;
using System.Windows;

namespace Wpf;
public class CartesianChartControl : ChartControl, ICartesianChartView
{
    private CoreChart? _coreChart;

    #region DP

    public CoreDrawnDataArea CoreDrawnDataArea
    {
        get { return (CoreDrawnDataArea)GetValue(DrawnDataAreaProperty); }
        set { SetValue(DrawnDataAreaProperty, value); }
    }

    public static readonly DependencyProperty DrawnDataAreaProperty =
        DependencyProperty.Register(
            "DrawnDataArea",
            typeof(CoreDrawnDataArea),
            typeof(CartesianChartControl),
            new PropertyMetadata(null,
                (d, e) =>
                {

                },
                (d, e) =>
                {
                    return e;
                }));



    #endregion

    public LayoutKind LayoutKind { get; set; } = LayoutKind.Stack;

    protected override CoreChart? CoreChart
    {
        get
        {
            _coreChart ??= new CartesianChart(this, CanvasControl!.Canvas);
            return _coreChart;
        }
    }
}
