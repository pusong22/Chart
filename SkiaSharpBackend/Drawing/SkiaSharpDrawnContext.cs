using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using Core.Primitive;
using SkiaSharp;

namespace SkiaSharpBackend.Drawing;
public class SkiaSharpDrawnContext(SKSurface surface, SKImageInfo info)
    : DrawnContext
{
    public SKCanvas Canvas { get; } = surface.Canvas;
    public SKImageInfo Info { get; } = info;
    public SKPaint? ActivateSkPaint { get; protected internal set; }
    public SKFont? ActivateSkFont { get; protected internal set; }

    public override void BeginDraw()
    {
        Canvas.Clear();
        using var p = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255)
        };

        Canvas.DrawRect(Info.Rect, p);
    }

    public override void Draw(DrawnGeometry drawable)
    {
        Canvas.Save();

        var p = new SKPoint(drawable.X, drawable.Y);

        if (drawable.HasRotation)
        {
            // 先更新旋转起点，然在更新绘制起点
            Canvas.Translate(p.X, p.Y);
            Canvas.RotateDegrees(drawable.RotateTransform);
            Canvas.Translate(-p.X, -p.Y);
        }

        ActivateSkPaint!.Color =
            ActivateSkPaint.Color.WithAlpha((byte)(255 * drawable.Opacity));


        drawable.Draw(this);

        Canvas.Restore();
    }

    public override void DisposePaint()
    {
        ActivateSkPaint?.Dispose();
        ActivateSkFont?.Dispose();

        ActivateSkPaint = null;
        ActivateSkFont = null;
    }

    public override void InitializePaint(Paint paint)
    {
        ActivateSkPaint ??= new SKPaint();

        ActivateSkPaint.Color = paint.ToSKColor();
        ActivateSkPaint.IsAntialias = paint.IsAntialias;

        ActivateSkPaint.Style = paint.ToSKStyle();

        ActivateSkPaint.PathEffect = paint.ToSKPathEffect();

        ActivateSkFont ??= new SKFont();

        ActivateSkFont.Typeface = paint.ToSKTypeface();
    }

    public override Rect MeasureText(string text)
    {
        ActivateSkFont!.MeasureText(text, out var bound, ActivateSkPaint!);

        return new Rect(new Point(bound.Left, bound.Top), new Size(bound.Width, bound.Height));
    }

    public override void DrawRect(Rect rect)
    {
        Canvas.DrawRect(rect.ToSKRect(), ActivateSkPaint!);
    }

    public override void DrawText(string text, Point p)
    {
        Canvas.DrawText(text, p.ToSKPoint(), ActivateSkFont!, ActivateSkPaint!);
    }

    public override void DrawLine(Point p1, Point p2)
    {
        Canvas.DrawLine(p1.ToSKPoint(), p2.ToSKPoint(), ActivateSkPaint!);
    }
}
