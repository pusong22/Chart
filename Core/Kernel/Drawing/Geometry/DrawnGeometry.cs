using Core.Kernel.Motion;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Drawing.Geometry;
public abstract class DrawnGeometry : Animatable
{
    private readonly FloatMotionProperty _opacityProperty;
    private readonly FloatMotionProperty _xProperty;
    private readonly FloatMotionProperty _yProperty;

    private float _rotateTransform;

    protected DrawnGeometry()
    {
        _opacityProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Opacity), 1f));
        _xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0f));
        _yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0f));
    }


    public float Opacity
    {
        get => _opacityProperty.Get(this);
        protected internal set
        {
            if (value < 0f) value = 0f;
            if (value > 1f) value = 1f;

            _opacityProperty.Set(value, this);
        }
    }

    public float X
    {
        get => _xProperty.Get(this);
        protected internal set => _xProperty.Set(value, this);
    }

    public float Y
    {
        get => _yProperty.Get(this);
        protected internal set => _yProperty.Set(value, this);
    }

    public float RotateTransform
    {
        get => _rotateTransform;
        protected internal set => _rotateTransform = value;
    }

    public bool HasRotation => RotateTransform != 0;

    public Paint? Paint { get; protected internal set; }

    public abstract void Draw<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext;
    public abstract Size Measure();
}
