using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseLabelGeometry : DrawnGeometry
{
    public Padding? Padding { get; set; }
    public string? Text { get; set; }
    public float TextSize { get; set; }
    public Align VerticalAlign { get; set; } = Align.Middle;
    public Align HorizontalAlign { get; set; } = Align.Middle;
    public float LineHeight { get; set; } = 1.45f;
}
