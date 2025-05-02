namespace Core.Primitive;
public readonly struct Color(byte r, byte g, byte b, byte a = 255)
{
    public byte Red { get; } = r;
    public byte Green { get; } = g;
    public byte Blue { get; } = b;
    public byte Alpha { get; } = a;
}
