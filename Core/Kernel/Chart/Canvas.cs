using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using System.Diagnostics;

namespace Core.Kernel.Chart;
public class Canvas
{
    private readonly object _sync = new();
    private readonly Stopwatch _stopWatch = Stopwatch.StartNew();

    private readonly Dictionary<Paint, HashSet<DrawnGeometry>> _paintTask = [];
    public event EventHandler? InvalidatedHandler;

    public bool USE_GPU { get; set; } = true;
    public bool IsCompleted { get; internal set; }

    public void DrawFrame<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext
    {
        // 
        lock (_sync)
        {
            var isValid = true;
            context.BeginDraw();

            var frameTime = _stopWatch.ElapsedMilliseconds;

            foreach (var item in _paintTask)
            {
                var paint = item.Key;

                if (paint is null) continue;

                context.InitializePaint(paint);

                foreach (var geometry in item.Value)
                {
                    if (geometry is null) continue;

                    geometry.CurrentTime = frameTime;

                    isValid = isValid && geometry.IsValid;

                    context.Draw(geometry);
                }

                context.DisposePaint();
            }

            IsCompleted = isValid;

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

    public void ReleasePaint()
    {
        _paintTask.Clear();
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
