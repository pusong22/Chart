using Core.Helper;
using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Core.Kernel.Chart;

public class CanvasContext
{
    private readonly object _sync = new();
    private readonly Stopwatch _stopWatch = Stopwatch.StartNew();

    private readonly ConcurrentDictionary<Paint, HashSet<DrawnGeometry>> _paintTask = [];

    private readonly GeometryPoolProvider _geometryProvider = new();

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

                _geometryProvider.Return(item.Item2);

                isCompleted = false;
            }

            IsCompleted = isCompleted;

            context.EndDraw();
        }
    }

    public void AddDrawnTask<T>(Paint paint, out T geometry) where T : DrawnGeometry, new()
    {
        var tasks = _paintTask.GetOrAdd(paint, []);

        geometry = _geometryProvider.Rent<T>();

        SetDefaultAnimation(geometry);

        tasks.Add(geometry);
    }

    public void SetDefaultAnimation(DrawnGeometry geometry)
    {
        geometry.Animate(ChartConfig.AnimateFunc, ChartConfig.AnimateDuration);
    }

    public void Invalidate()
    {
        IsCompleted = false;
        InvalidatedHandler?.Invoke(this, null);
    }
}
