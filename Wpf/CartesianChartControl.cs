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

    public CoreDrawnRect CoreDrawnDataArea
    {
        get { return (CoreDrawnRect)GetValue(DrawnDataAreaProperty); }
        set { SetValue(DrawnDataAreaProperty, value); }
    }

    public static readonly DependencyProperty DrawnDataAreaProperty =
        DependencyProperty.Register(
            "DrawnRect",
            typeof(CoreDrawnRect),
            typeof(CartesianChartControl),
            new PropertyMetadata(null));



    #endregion

    public LayoutKind LayoutKind { get; set; } = LayoutKind.Stack;

    protected override CoreChart? CoreChart
    {
        get
        {
            _coreChart ??= new CartesianChart(this);
            return _coreChart;
        }
    }
}
