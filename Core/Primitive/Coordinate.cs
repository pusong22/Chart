namespace Core.Primitive;
public readonly struct Coordinate(double x, double y)
{
    public double X { get; } = x;
    public double Y { get; } = y;
}
