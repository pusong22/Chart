namespace Core.Primitive;
public readonly struct Coordinate(double x, double y, double z = 0d)
{
    public double X { get; } = x;
    public double Y { get; } = y;
    public double Z { get; } = z;
}
