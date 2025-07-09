using Core.Kernel.Drawing;
using Core.Kernel.Painting;
using System.Collections.ObjectModel;

namespace Core.Kernel;
public class ChartDrawingCommand(IEnumerable<Paint> paints)
{
    public ReadOnlyCollection<Paint> Paints { get; } = new ReadOnlyCollection<Paint>([.. paints]);

    public void Execute<TDrawnContext>(TDrawnContext context)
       where TDrawnContext : DrawnContext
    {
        if (!Paints.Any()) return;

        context.BeginDraw();

        // 绘制命令内部直接处理绘制逻辑
        foreach (var paint in Paints.Where(x => x is not null).OrderBy(x => x.ZIndex))
        {
            context.InitializePaint(paint);

            foreach (var geometry in paint.Geometries)
                context.Draw(geometry);

            context.DisposePaint();
        }

        context.EndDraw();
    }
}
