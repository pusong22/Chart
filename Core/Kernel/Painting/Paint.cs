using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace Core.Kernel.Painting;
public abstract class Paint
{
    public int ZIndex { get; set; } = -1;
    public string? FontFamily { get; set; }
    public float FontSize { get; set; } = 12f;
    public bool IsAntialias { get; set; }

    public Color Color { get; set; }
    public PaintStyle Style { get; set; }

    public PathEffectSetting? PathEffect { get; set; }

    public HashSet<DrawnGeometry> Geometries { get; } = [];
}

public class Brush : Paint
{
    public Brush()
    {
        Style = PaintStyle.Fill;
    }
}

public class Pen : Paint
{
    public Pen()
    {
        Style = PaintStyle.Stroke;
    }

    public Pen(DashEffectSetting dashEffectSetting)
    {
        PathEffect = dashEffectSetting;
    }
}
