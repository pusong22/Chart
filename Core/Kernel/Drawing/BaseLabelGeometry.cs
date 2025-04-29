namespace Core.Kernel.Drawing;
public abstract class BaseLabelGeometry : DrawnGeometry
{
    public string? Text { get; set; }
    public float TextSize { get; set; }
    public Paint? Paint { get;set; }
}
