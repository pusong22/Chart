using Core.Kernel.Drawing;
using SkiaSharp;

namespace SkiaSharpBackend;
public abstract class SkiaSharpPaint : Paint
{
    public SKFontStyle? SKFontStyle { get; set; }

    public SKTypeface? SKTypeface { get; set; }

    protected internal SKTypeface GetSKTypeface()
    {
        // return the defined typeface.
        if (SKTypeface is not null) return SKTypeface;

        // create one from the font family.
        if (FontFamily is not null) return SKTypeface.FromFamilyName(FontFamily, SKFontStyle ?? new SKFontStyle());

        // other wise ose the globally defined typeface.
        return SKTypeface.Default;
    }
}
