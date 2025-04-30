using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class DrawnGeometry
{
    private float _rotateTransform;

    public float X { get; set; }
    public float Y { get; set; }

    public float RotateTransform
    {
        get => _rotateTransform;
        set
        {
            _rotateTransform = value;
            HasTransform = true;
        }
    }

    public bool HasTransform { get; set; }
    public bool HasRotation => RotateTransform != 0;

    public Paint? Paint { get; set; }

    public abstract void Draw<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext;
    public abstract Size Measure();
}
