using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class DrawnGeometry
{
    public float Opacity { get; protected internal set; } = 1f;


    public float X { get; protected internal set; }

    public float Y { get; protected internal set; }

    public float RotateTransform { get; protected internal set; }

    public bool HasRotation => RotateTransform != 0;

    public Paint? Paint { get; protected internal set; }

    public abstract void Draw<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext;
    public abstract Size Measure();
}
