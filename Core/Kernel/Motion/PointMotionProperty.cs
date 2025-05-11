using Core.Primitive;

namespace Core.Kernel.Motion;
public class PointMotionProperty : IMotionProperty<Point>
{
    public PointMotionProperty(string propertyName)
     : base(propertyName, new Point(0f, 0f), new Point(0f, 0f)) { }

    public PointMotionProperty(string propertyName, Point value)
      : base(propertyName, value, value) { }

    protected override Point Interpolate(float progress)
    {
        return new Point(
            FromValue.X + progress * (ToValue.X - FromValue.X),
            FromValue.Y + progress * (ToValue.Y - FromValue.Y));
    }
}
