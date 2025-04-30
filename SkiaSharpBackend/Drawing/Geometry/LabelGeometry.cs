using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Primitive;
using SkiaSharp;
using SkiaSharpBackend.Painting;

namespace SkiaSharpBackend.Drawing.Geometry;
public class LabelGeometry : BaseLabelGeometry
{
    private float _maxTextHeight = 0f;
    private int _lines = 0;

    public override void Draw<TDrawnContext>(TDrawnContext context)
    {
        if (context is not SkiaSharpDrawnContext skContext) return;
        if (Paint is null)
            throw new ArgumentNullException(nameof(Paint));

        var p = Padding ?? new Padding(0f);

        var size = Measure();
        var skPaint = (SkiaSharpPaint)Paint;
        var typeface = skPaint.GetSKTypeface();

        using var font = new SKFont
        {
            Size = TextSize,
            Typeface = typeface,
        };

        float h = 0f;// -_lines * _maxTextHeight * 0.5f;

        //#if DEBUG
        //        using var r = new SKPaint { Color = new SKColor(255, 0, 0), Style = SKPaintStyle.Stroke };
        //using var b = new SKPaint { Color = new SKColor(0, 155, 0), Style = SKPaintStyle.Stroke };
        //        skContext.Canvas.DrawRect(Xo, Yo - size.Height * 0.5f, size.Width, size.Height, r);
        //skContext.Canvas.DrawRect(Xo, Yo, size.Width, size.Height, b);
        //skContext.Canvas.DrawText(Text, Xo, Yo, font, skContext.ActivateSkPaint);
        //#endif

        foreach (var line in GetLines())
        {
            font.MeasureText(line, out var bound, skContext.ActivateSkPaint);

            var xo = GetAlignmentOffset(bound);

            skContext.Canvas.DrawText(line, Xo + xo.X + p.Left, Yo + p.Top + xo.Y + h, font, skContext.ActivateSkPaint);

            h += _maxTextHeight * LineHeight;
        }
    }

    public override Size Measure()
    {
        if (Paint is null)
            throw new ArgumentNullException(nameof(Paint));

        var skPaint = (SkiaSharpPaint)Paint;
        var typeface = skPaint.GetSKTypeface();

        using var font = new SKFont
        {
            Size = TextSize,
            Typeface = typeface
        };

        using var p = new SKPaint
        {
            IsAntialias = Paint.IsAntialias
        };

        var w = 0f;
        _maxTextHeight = 0f;
        _lines = 0;

        foreach (var line in GetLines())
        {
            font.MeasureText(line, out var bound, p);

            if (bound.Width > w) w = bound.Width;
            if (bound.Height > _maxTextHeight) _maxTextHeight = bound.Height;
            _lines++;
        }

        var h = _maxTextHeight * _lines * LineHeight;

        var padding = Padding ?? new Padding(0f);

        var size = new Size(
            w + padding.Left + padding.Right,
            h + padding.Top + padding.Bottom);

        return size.GetRotatedSize(RotateTransform);
    }


    private IEnumerable<string> GetLines()
    {
        if (Text is null)
            throw new ArgumentNullException(nameof(Text));

        IEnumerable<string> lines = Text.Split([Environment.NewLine], StringSplitOptions.None);

        return lines;
    }


    private Point GetAlignmentOffset(SKRect bounds)
    {
        var p = Padding ?? new Padding(0f);

        var w = bounds.Width + p.Left + p.Right;
        var h = bounds.Height * LineHeight + p.Top + p.Bottom;

        float l = -bounds.Left, t = -bounds.Top;

        switch (VerticalAlign)
        {
            case Align.Start: t += 0; break;
            case Align.Middle: t -= h * 0.5f; break;
            case Align.End: t -= h + 0; break;
            default:
                break;
        }
        switch (HorizontalAlign)
        {
            case Align.Start: l += 0; break;
            case Align.Middle: l -= w * 0.5f; break;
            case Align.End: l -= w + 0; break;
            default:
                break;
        }

        return new(l, t);
    }
}
