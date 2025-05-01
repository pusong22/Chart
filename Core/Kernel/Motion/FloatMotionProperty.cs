namespace Core.Kernel.Motion;
public class FloatMotionProperty : IMotionProperty<float>
{
    public FloatMotionProperty(string propertyName)
        : base(propertyName, 0f, 0f) { }

    public FloatMotionProperty(string propertyName, float value)
      : base(propertyName, value, value) { }

    protected override float Interpolate(float progress)
    {
        return FromValue + progress * (ToValue - FromValue);
    }
}
