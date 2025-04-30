using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;

namespace Core.Kernel.Chart;
public class Canvas
{
    private readonly object _sync = new();

    private readonly Dictionary<Paint, HashSet<DrawnGeometry>> _paintTask = [];
    private readonly HashSet<Paint> _deletedpaintTask = [];
    public event EventHandler? InvalidatedHandler;

    public void DrawFrame<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext
    {
        // 
        lock (_sync)
        {
            _deletedpaintTask.Clear();

            context.BeginDraw();

            foreach (var item in _paintTask)
            {
                var paint = item.Key;

                if (paint is null) continue;

                if (paint.RemoveOnCompleted)
                    _deletedpaintTask.Add(paint);

                context.InitializePaint(paint);

                foreach (var geometry in item.Value)
                {
                    if (geometry is null) continue;

                    context.Draw(geometry);
                }

                context.DisposePaint();
            }

            ReleasePaint(_deletedpaintTask);

            context.EndDraw();
        }
    }

    public void AddDrawnTask(Paint paint, DrawnGeometry geometry)
    {
        if (_paintTask.TryGetValue(paint, out var g))
        {
            _ = g.Add(geometry);
        }
        else
        {
            g = [];
            _ = g.Add(geometry);

            _paintTask[paint] = g;
        }
    }

    public void Invalidate()
    {
        InvalidatedHandler?.Invoke(this, null);
    }

    public void ReleasePaint(Paint? paint)
    {
        if (paint is null) return;

        _paintTask[paint].Clear();
        _paintTask.Remove(paint);
    }

    public void ReleasePaint(IEnumerable<Paint?>? paints)
    {
        if (paints is null) return;

        foreach (var item in paints)
        {
            if (item is null) continue;

            _paintTask[item].Clear();
            _paintTask.Remove(item);
        }
    }
}
