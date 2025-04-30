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

    public bool ShowRect { get; set; }

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

        // 因为是每次画一行，初始化的x,y是中心点，经测试发现skia字体逻辑是
        // Align.Left, Align.Bottom，
        float h = _lines > 1
                    ? VerticalAlign switch
                    {
                        Align.Start => 0f,
                        Align.Middle => -(_lines - 1) * _maxTextHeight * LineHeight * 0.5f,
                        Align.End => -(_lines - 1) * _maxTextHeight * LineHeight,
                        _ => 0f
                    }
                    : 0f;

        foreach (var line in GetLines())
        {
            font.MeasureText(line, out var bound, skContext.ActivateSkPaint);

            var xo = GetAlignmentOffset(bound);

            skContext.Canvas.DrawText(line, X + xo.X, Y + xo.Y + h, font, skContext.ActivateSkPaint);

#if DEBUG
            if (ShowRect)
            {
                skContext.ActivateSkPaint.Style = SKPaintStyle.Stroke;

                skContext.Canvas.DrawRect(
                    X + xo.X,
                    Y + xo.Y + h - bound.Height,
                    bound.Width,
                    bound.Height * LineHeight,
                    skContext.ActivateSkPaint);


                skContext.Canvas.DrawRect(
                    X + xo.X - p.Left,
                    Y + xo.Y + h - bound.Height - p.Top,
                    bound.Width + p.Left + p.Right,
                    bound.Height * LineHeight + p.Top + p.Bottom,
                    skContext.ActivateSkPaint);


                skContext.ActivateSkPaint.Style = SKPaintStyle.Fill;
            }
#endif

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
        var w = bounds.Width;
        var h = bounds.Height * LineHeight;

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
