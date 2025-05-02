namespace Core.Primitive;
public readonly struct Point(float x, float y)
{
    public float X { get; } = x;
    public float Y { get; } = y;
}
