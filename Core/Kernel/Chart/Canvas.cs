using Core.Helper;
using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using System.Diagnostics;

namespace Core.Kernel.Chart;
public class Canvas
{
    private readonly object _sync = new();
    private readonly Stopwatch _stopWatch = Stopwatch.StartNew();

    private readonly Dictionary<Paint, HashSet<DrawnGeometry>> _paintTask = [];
    public event EventHandler? InvalidatedHandler;

    public bool IsCompleted { get; private set; }

    public void DrawFrame<TDrawnContext>(TDrawnContext context)
        where TDrawnContext : DrawnContext
    {
        // 
        lock (_sync)
        {
            var disabledAnimation = ChartConfig.DisabledAnimation;
            var isCompleted = true;

            var removeTask = new List<Tuple<Paint, DrawnGeometry>>();
            context.BeginDraw();

            var frameTime = _stopWatch.ElapsedMilliseconds;

            foreach (var item in _paintTask.OrderBy(x => x.Key.ZIndex))
            {
                var paint = item.Key;

                if (paint is null) continue;

                context.InitializePaint(paint);

                foreach (var geometry in item.Value)
                {
                    if (disabledAnimation)
                        geometry.RemoveMotion();

                    geometry.CurrentTime = frameTime;

                    context.Draw(geometry);

                    if (geometry.Remove)
                        removeTask.Add(new Tuple<Paint, DrawnGeometry>(paint, geometry));

                    isCompleted = isCompleted && geometry.IsCompleted;
                }

                context.DisposePaint();
            }

            foreach (var item in removeTask)
            {
                _ = _paintTask[item.Item1].Remove(item.Item2);

                isCompleted = false;
            }

            IsCompleted = isCompleted;

            context.EndDraw();
        }
    }

    public void AddDrawnTask(Paint paint, DrawnGeometry geometry)
    {
        if (!_paintTask.TryGetValue(paint, out var g))
        {
            g = [];
            _paintTask.Add(paint, g);
        }

        _ = g.Add(geometry);
    }

    public void Invalidate()
    {
        IsCompleted = false;
        InvalidatedHandler?.Invoke(this, null);
    }

    // TEST
    public void Clear()
    {
        _paintTask.Clear();
    }
}
