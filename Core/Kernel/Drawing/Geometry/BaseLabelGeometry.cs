using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseLabelGeometry : DrawnGeometry
{
    public Padding? Padding { get; protected internal set; }
    public string? Text { get; protected internal set; }
    public float TextSize { get; protected internal set; }
    public Align VerticalAlign { get; protected internal set; } = Align.Middle;
    public Align HorizontalAlign { get; protected internal set; } = Align.Middle;
    public float LineHeight { get; protected internal set; } = 1.45f;

    public override bool TryReset()
    {
        Padding = null;
        Text = null;
        TextSize = 0f;
        VerticalAlign = Align.Middle;
        HorizontalAlign = Align.Middle;
        LineHeight = 1.45f;

        return base.TryReset();
    }
}
