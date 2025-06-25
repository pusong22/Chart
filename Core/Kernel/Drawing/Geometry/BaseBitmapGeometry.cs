using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseBitmapGeometry : DrawnGeometry
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Rect DestRect { get; set; }
    public byte[]? PixelData { get; set; }

    public override Size Measure()
    {
        return new Size(Width, Height);
    }
}
