using Core.Kernel.Motion;
using Core.Primitive;
using static System.Net.Mime.MediaTypeNames;

namespace Core.Kernel.Drawing.Geometry;
public abstract class BaseLineGeometry : DrawnGeometry
{
    private readonly FloatMotionProperty _x1Property;
    private readonly FloatMotionProperty _y1Property;

    protected BaseLineGeometry()
    {
        _x1Property = RegisterMotionProperty(new FloatMotionProperty(nameof(X1), 0f));
        _y1Property = RegisterMotionProperty(new FloatMotionProperty(nameof(Y1), 0f));
    }

    public float X1
    {
        get => _x1Property.Get(this);
        protected internal set => _x1Property.Set(value, this);
    }
    public float Y1
    {
        get => _y1Property.Get(this);
        protected internal set => _y1Property.Set(value, this);
    }

    public override Size Measure()
    {
        return new Size(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
    }

    public override bool TryReset()
    {
        X1 = 0f;
        Y1 = 0f;

        return base.TryReset();
    }
}
