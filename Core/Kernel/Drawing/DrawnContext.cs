using Core.Kernel.Drawing.Geometry;

namespace Core.Kernel.Drawing;

public abstract class DrawnContext
{
    public Paint? ActivatePaint { get; set; }
    public virtual void BeginDraw() { }

    public virtual void EndDraw() { }

    public abstract void Draw(DrawnGeometry drawable);

    public abstract void InitializePaint(Paint paint);
    public abstract void DisposePaint();
}
