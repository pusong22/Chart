namespace Core.Primitive;

public readonly struct Size(float w, float h)
{
    public float Width => w;
    public float Height => h;

    public static Size operator +(Size a, Size b)
    {
        return new Size(
            a.Width + b.Width,
            a.Height + b.Height);
    }
}
