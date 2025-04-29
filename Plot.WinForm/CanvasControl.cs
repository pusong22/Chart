using Core.Kernel.Chart;
using SkiaSharp.Views.Desktop;
using SkiaSharpBackend;

namespace Plot.WinForm;
public partial class CanvasControl : UserControl
{
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
        _skControl?.Invalidate();
        _skGLControl?.Invalidate();
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
