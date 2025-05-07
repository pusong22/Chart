using Core.Primitive;

namespace Core.Kernel.Painting;
public class Paint
{
    public int ZIndex { get; set; } = -1;
    public string? FontFamily { get; set; }
    public bool IsAntialias { get; set; }

    public Color Color { get; set; }
    public PaintStyle? Style { get; set; }

    public PathEffectSetting? PathEffect { get; set; }
}
