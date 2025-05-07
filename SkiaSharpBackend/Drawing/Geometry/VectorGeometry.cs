using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Series;
using Core.Primitive;
using SkiaSharp;

namespace SkiaSharpBackend.Drawing.Geometry;
public abstract class VectorGeometry<TSegment> : BaseVectoryGeometry<TSegment>
    where TSegment : CubicBezierSegment
{
    protected virtual void OnOpen(DrawnContext context, SKPath path, TSegment segment)
    { }
    protected virtual void OnClose(DrawnContext context, SKPath path, TSegment segment)
    { }
    protected virtual void OnDrawSegment(DrawnContext context, SKPath path, TSegment segment)
    { }

    public override void Draw<TDrawnContext>(TDrawnContext context)
    {
        var skContext = context as SkiaSharpDrawnContext;

        var removePoints = new List<TSegment>();

        using var path = new SKPath();

        TSegment? last = null;
        bool first = true;
        foreach (var segment in Segments)
        {
            if (first)
            {
                first = false;
                OnOpen(context, path, segment);
            }

            OnDrawSegment(context, path, segment);

            removePoints.Add(segment);
            last = segment;
        }

        if (last is not null)
            OnClose(context, path, last);

        skContext?.DrawPath(path);

        //if (removePoints.Count > 0)
        //{
        //    foreach (var segment in removePoints)
        //    {
        //        Segments.Remove(segment);
        //    }
        //}
    }

    public override Size Measure()
    {
        return new Size(0, 0);
    }
}


public class CubicBezierVectorGeometry : VectorGeometry<CubicBezierSegment>
{
    protected override void OnOpen(DrawnContext context, SKPath path, CubicBezierSegment segment)
    {
        path.MoveTo(segment.Start.ToSKPoint());
    }

    protected override void OnClose(DrawnContext context, SKPath path, CubicBezierSegment segment)
    {
    }

    protected override void OnDrawSegment(DrawnContext context, SKPath path, CubicBezierSegment segment)
    {
        path.CubicTo(segment.Control1.ToSKPoint(), segment.Control2.ToSKPoint(), segment.End.ToSKPoint());
    }
}
