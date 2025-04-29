
using Core.Primitive;

namespace Core.Kernel.Drawing;
public abstract class BaseLabelGeometry : DrawnGeometry
{
    public Padding? NamePadding { get; set; }
    public string? Text { get; set; }
    public float TextSize { get; set; }
    public Paint? Paint { get; set; }
}
