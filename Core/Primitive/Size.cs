
namespace Core.Primitive;

public readonly struct Size(float w, float h)
{
    public float Width { get; } = w;
    public float Height { get; } = h;

    public Size GetRotatedSize(float degrees)
    {
        if (Math.Abs(degrees) < 0.001) return this;

        const double toRadians = Math.PI / 180;

        degrees %= 360;
        if (degrees < 0) degrees += 360;

        if (degrees > 180) degrees = 360 - degrees;
        if (degrees is > 90 and <= 180) degrees = 180 - degrees;

        var rRadians = degrees * toRadians;

        var w = (float)(Math.Cos(rRadians) * Width + Math.Sin(rRadians) * Height);
        var h = (float)(Math.Sin(rRadians) * Width + Math.Cos(rRadians) * Height);

        return new(w, h);
    }
}
