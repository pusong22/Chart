using Core.Primitive;

namespace Core.Kernel.Drawing;
public abstract class DrawnGeometry
{
    private float _rotateTransform;

    public float Xo { get; set; }
    public float Yo { get; set; }

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

    public abstract void Draw(DrawnContext context);
    public abstract Size Measure();
}
