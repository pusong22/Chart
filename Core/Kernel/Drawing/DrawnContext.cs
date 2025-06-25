using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;

namespace Core.Kernel.Drawing;

public abstract class DrawnContext
{
    public virtual void BeginDraw() { }

    public virtual void EndDraw() { }

    public abstract void Draw(DrawnGeometry drawable);

    public abstract void InitializePaint(Paint paint);
    public abstract void DisposePaint();


    public abstract TRect MeasureText<TRect>(string text);
    public abstract void DrawRect<TRect>(TRect rect);
    public abstract void DrawBitmap<TBitmap, TRect>(TBitmap bmp, TRect rect);
    public abstract void DrawText<TPoint>(string text, TPoint p);
    public abstract void DrawLine<TPoint>(TPoint p1, TPoint p2);
    public abstract void DrawCircle<TPoint>(TPoint p, float rd);
    public abstract void DrawPath<TPath>(TPath path);
}
