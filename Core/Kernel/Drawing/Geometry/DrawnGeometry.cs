using Core.Kernel.Motion;
using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class DrawnGeometry : Animatable
{
    private readonly FloatMotionProperty _opacityProperty;
    private readonly FloatMotionProperty _xProperty;
    private readonly FloatMotionProperty _yProperty;

    protected DrawnGeometry()
    {
        _opacityProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Opacity), 1f));
        _xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0f));
        _yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0f));
    }

    private float _rotateTransform;

    public float Opacity
    {
        get => _opacityProperty.Get(this);
        set => _opacityProperty.Set(value, this);
    }

    public float X
    {
        get => _xProperty.Get(this);
        set => _xProperty.Set(value, this);
    }

    public float Y
    {
        get => _yProperty.Get(this);
        set => _yProperty.Set(value, this);
    }

    public float RotateTransform
    {
        get => _rotateTransform;
        set
        {
            _rotateTransform = value;
            HasTransform = true;
        }
    }

    public bool HasTransform { get; private set; }
    public bool HasRotation => RotateTransform != 0;

    public Paint? Paint { get; set; }

    public abstract void Draw<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext;
    public abstract Size Measure();
}
