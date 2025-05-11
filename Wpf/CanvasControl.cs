using Core.Helper;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpBackend.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Wpf;
public class CanvasControl : UserControl
{
    private bool _invalidating = false;
    private SKElement? _skElement;
    //private SKGLElement? _skGLElement;

    public CanvasControl()
    {
        Loaded += OnLoad;
        Unloaded += OnUnLoad;
    }

    public Core.Kernel.Chart.Canvas Canvas { get; } = new();

    private void InitializeSkElement()
    {
        if (ChartConfig.USE_GPU)
        {
            //Content = _skGLElement = new SKGLElement();
            //_skGLElement.PaintSurface += OnPaintGLSurface;
        }
        else
        {
            Content = _skElement = new SKElement();
            _skElement.PaintSurface += OnPaintSurface;
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var context = new SkiaSharpDrawnContext(e.Surface, e.Info);
        Canvas.DrawFrame(context);
    }

    private void OnPaintGLSurface(object sender, SKPaintGLSurfaceEventArgs e)
    {
        var context = new SkiaSharpDrawnContext(e.Surface, e.Info);
        Canvas.DrawFrame(context);
    }

    private void OnLoad(object sender, RoutedEventArgs e)
    {
        InitializeSkElement();
        Canvas.InvalidatedHandler += OnInvalidate;
    }

    private void OnUnLoad(object sender, RoutedEventArgs e)
    {
        Canvas.InvalidatedHandler -= OnInvalidate;
    }

    private void OnInvalidate(object sender, EventArgs e)
    {
        Loop();
    }

    private async void Loop()
    {
        if (_invalidating) return; // 丢弃一些绘制
        _invalidating = true;

        var ts = TimeSpan.FromSeconds(1 / ChartConfig.MaxFPS);

        while (!Canvas.IsCompleted)
        {
            _skElement?.InvalidateVisual();
            //_skGLElement?.InvalidateVisual();

            await Task.Delay(ts);
        }

        _invalidating = false;
    }
}
