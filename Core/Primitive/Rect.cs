namespace Core.Primitive;

public readonly struct Rect(Point p, Size s)
{
    public Rect(float x, float y, float width, float height)
        : this(new Point(x, y), new Size(width, height)) { }

    public float X { get; } = p.X;
    public float Y { get; } = p.Y;


    public float Height { get; } = s.Height;
    public float Width { get; } = s.Width;

    public Point Location { get; } = p;
    public Size Size { get; } = s;
}
