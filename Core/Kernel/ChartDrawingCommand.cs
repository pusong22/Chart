using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using System.Collections.ObjectModel;

namespace Core.Kernel;
public class ChartDrawingCommand(IEnumerable<Paint> paints)
{
    private bool _drawing = false;

    public ReadOnlyCollection<Paint> Paints { get; } = new ReadOnlyCollection<Paint>([.. paints]);

    public void Execute<TDrawnContext>(TDrawnContext context)
       where TDrawnContext : DrawnContext
    {
        if (Paints.Count == 0 || _drawing) return;

        _drawing = true;

        List<Tuple<Paint, DrawnGeometry>> removeGeometries = [];
        context.BeginDraw();

        foreach (var paint in Paints.Where(x => x is not null).OrderBy(x => x.ZIndex))
        {
            context.InitializePaint(paint);

            foreach (var geometry in paint.Geometries)
            {
                context.Draw(geometry);

                if (geometry.Remove)
                    removeGeometries.Add(new Tuple<Paint, DrawnGeometry>(paint, geometry));
            }

            context.DisposePaint();
        }

        foreach (var item in removeGeometries)
        {
            Paint paint = item.Item1;
            DrawnGeometry geometry = item.Item2;

            paint.Geometries.Remove(geometry);
        }

        context.EndDraw();

        _drawing = false;
    }
}
