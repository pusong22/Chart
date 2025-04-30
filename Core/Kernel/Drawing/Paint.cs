using Core.Primitive;

namespace Core.Kernel.Drawing;
public abstract class Paint : IDisposable
{
    public string? FontFamily { get; set; }
    public bool IsAntialias { get; set; } = true;

    public PaintStyle Style { get; set; }
    public bool RemoveOnCompleted { get; set; }

    public abstract void Initialize(DrawnContext context);

    public abstract void Dispose();
}
