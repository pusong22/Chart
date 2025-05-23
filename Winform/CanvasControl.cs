using Core.Helper;
using Core.Kernel.Chart;
using SkiaSharp.Views.Desktop;
using SkiaSharpBackend.Drawing;

namespace WinForm;
public partial class CanvasControl : UserControl
{
    private readonly struct Resolution(float dpiX, float dpiY)
    {
        public float DpiX { get; } = dpiX;
        public float DpiY { get; } = dpiY;
    }

    private const float DEFAULTDPI = 96.0f;

    private bool _invalidating = false;
    public CanvasControl()
    {
        InitializeComponent();
    }

    public CanvasContext CanvasContext { get; } = new();

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        CanvasContext.InvalidatedHandler += OnInvalidate;
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        CanvasContext.InvalidatedHandler -= OnInvalidate;
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

        while (!CanvasContext.IsCompleted)
        {
            _skControl?.Invalidate();
            //_skGLControl?.Invalidate();

            await Task.Delay(ts);
        }

        _invalidating = false;
    }

    private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (!_invalidating) return;
        var resolution = GetResolution();
        e.Surface.Canvas.Scale(resolution.DpiX, resolution.DpiY);

        var context = new SkiaSharpDrawnContext(e.Surface, e.Info);
        CanvasContext.DrawFrame(context);
    }

    private void SkglControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
    {
        if (!_invalidating) return;
        var resolution = GetResolution();
        e.Surface.Canvas.Scale(resolution.DpiX, resolution.DpiY);

        var context = new SkiaSharpDrawnContext(e.Surface, e.Info);
        CanvasContext.DrawFrame(context);
    }

    private Resolution GetResolution()
    {
        using var g = CreateGraphics();
        return new Resolution(g.DpiX / DEFAULTDPI, g.DpiY / DEFAULTDPI);
    }
}
