namespace Core.Kernel.Motion;
public class DoubleMotionProperty : IMotionProperty<double>
{
    public DoubleMotionProperty(string propertyName)
        : base(propertyName, 0d, 0d) { }

    public DoubleMotionProperty(string propertyName, double value)
      : base(propertyName, value, value) { }

    protected override double Interpolate(float progress)
    {
        return FromValue + progress * (ToValue - FromValue);
    }
}
