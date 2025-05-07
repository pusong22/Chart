using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Drawing;

public abstract class DrawnContext
{
    public virtual void BeginDraw() { }

    public virtual void EndDraw() { }

    public abstract void Draw(DrawnGeometry drawable);

    public abstract void InitializePaint(Paint paint);
    public abstract void DisposePaint();


    public abstract Rect MeasureText(string text);

    public abstract void DrawRect(Rect rect);

    public abstract void DrawText(string text, Point p);

    public abstract void DrawLine(Point p1, Point p2);
    public abstract void DrawCircle(Point p, float rd);
}
