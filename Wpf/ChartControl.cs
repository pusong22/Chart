using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Kernel.Series;
using Core.Kernel.View;
using SkiaSharpBackend;
using System.Windows;
using System.Windows.Controls;

namespace Wpf;
public abstract class ChartControl : UserControl, IChartView
{
    protected ChartControl()
    {
        Content = CanvasControl;

        ChartConfig.Configure(config => config.UseDefault());

        Loaded += OnLoad;
        Unloaded += OnUnLoad;
        SizeChanged += OnSizeChanged;
    }

    #region DP

    public IEnumerable<CoreSeries> Series
    {
        get { return (IEnumerable<CoreSeries>)GetValue(SeriesProperty); }
        set { SetValue(SeriesProperty, value); }
    }

    public IEnumerable<CoreAxis> XAxes
    {
        get { return (IEnumerable<CoreAxis>)GetValue(XAxesProperty); }
        set { SetValue(XAxesProperty, value); }
    }

    public IEnumerable<CoreAxis> YAxes
    {
        get { return (IEnumerable<CoreAxis>)GetValue(YAxesProperty); }
        set { SetValue(YAxesProperty, value); }
    }

    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(
            "Series",
            typeof(IEnumerable<CoreSeries>),
            typeof(ChartControl),
            new PropertyMetadata(null,
                (d, e) =>
                {

                },
                (d, e) =>
                {
                    return e;
                }));


    public static readonly DependencyProperty XAxesProperty =
        DependencyProperty.Register(
            "XAxes",
            typeof(IEnumerable<CoreAxis>),
            typeof(ChartControl),
            new PropertyMetadata(null,
                (d, e) =>
                {

                },
                (d, e) =>
                {
                    return e;
                }));

    public static readonly DependencyProperty YAxesProperty =
       DependencyProperty.Register(
           "YAxes",
           typeof(IEnumerable<CoreAxis>),
           typeof(ChartControl),
           new PropertyMetadata(null,
               (d, e) =>
               {

               },
               (d, e) =>
               {
                   return e;
               }));


    #endregion

    protected CanvasControl? CanvasControl { get; } = new();
    protected abstract CoreChart? CoreChart { get; }

    public Core.Primitive.Size ControlSize
    {
        get
        {
            return new((float)CanvasControl!.ActualWidth, (float)CanvasControl!.ActualHeight);
        }
    }

    public void InvokeUIThread(Action action)
    {
        if (Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            _ = Dispatcher.BeginInvoke(action);
        }
    }

    private void OnLoad(object sender, RoutedEventArgs e)
    {
        if (CoreChart is null || CoreChart.IsLoad) return;
        CoreChart.Load();
    }

    private void OnUnLoad(object sender, RoutedEventArgs e)
    {
        if (CoreChart is null || !CoreChart.IsLoad) return;
        CoreChart.UnLoad();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (CoreChart is null || !CoreChart.IsLoad) return;
        CoreChart.Update();
    }
}
