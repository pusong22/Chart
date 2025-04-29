using Core.Kernel.Drawing;
using Core.Primitive;
using SkiaSharp;

namespace SkiaSharpBackend;
public class LabelGeometry : BaseLabelGeometry
{
    private float _maxTextHeight = 0f;
    private int _lines = 0;

    public override void Draw(DrawnContext context)
    {
        if (Paint is null)
            throw new ArgumentNullException(nameof(Paint));

        var skContext = (SkiaSharpDrawnContext)context;

        var p = NamePadding ?? new Padding(0f, 0f, 0f, 0f);

        _ = Measure();
        var skPaint = (SkiaSharpPaint)Paint;
        var typeface = skPaint.GetSKTypeface();

        using var font = new SKFont
        {
            Size = TextSize,
            Typeface = typeface,
        };

        float h = -_lines * _maxTextHeight * 0.5f;
        foreach (var line in GetLines())
        {
            //font.MeasureText(line, out var bound, skContext.ActivateSkPaint);

            skContext.Canvas.DrawText(line, Xo + p.Left, Yo + p.Top + h, font, skContext.ActivateSkPaint);

            h += _maxTextHeight;
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
            IsAntialias = Paint.IsAntialias,
            Style = Paint.Style == PaintStyle.Fill
                ? SKPaintStyle.Fill : SKPaintStyle.Stroke,
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

        var h = _maxTextHeight * _lines;

        var padding = NamePadding ?? new Padding(0f, 0f, 0f, 0f);

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
}
