namespace Core.Primitive;

public readonly struct Rect(Point p, Size s)
{
    public float X => (float)p.X;
    public float Y => (float)p.Y;

    public float Height => s.Height;
    public float Width => s.Width;
}
