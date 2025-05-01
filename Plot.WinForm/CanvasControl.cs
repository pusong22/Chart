using Core.Kernel.Chart;
using SkiaSharp.Views.Desktop;
using SkiaSharpBackend.Drawing;

namespace Plot.WinForm;
public partial class CanvasControl : UserControl
{
    private bool _invalidating = false;
    public CanvasControl()
    {
        InitializeComponent();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        Canvas.InvalidatedHandler += OnInvalidate;
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

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

        var ts = TimeSpan.FromSeconds(1 / 60d);

        while (!Canvas.IsCompleted)
        {
            _skControl?.Invalidate();
            _skGLControl?.Invalidate();

            await Task.Delay(ts);
        }

        _invalidating = false;
    }

    public Canvas Canvas { get; } = new();

    private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var context = new SkiaSharpDrawnContext(e.Surface, e.Info);
        Canvas.DrawFrame(context);
    }


    private void SkglControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
    {
        var context = new SkiaSharpDrawnContext(e.Surface, e.Info);
        Canvas.DrawFrame(context);
    }
}
