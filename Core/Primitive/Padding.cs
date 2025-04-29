namespace Core.Primitive;

public class Padding(float left, float top, float right, float bottom)
{
    public Padding(float x, float y) : this(x, y, x, y) { }
    public Padding(float p) : this(p, p, p, p) { }

    public float Left { get; set; } = left;
    public float Top { get; set; } = top;
    public float Right { get; set; } = right;
    public float Bottom { get; set; } = bottom;
}
