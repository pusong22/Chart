using Core.Kernel.Visual;
using Core.Primitive;

namespace Core.Kernel;
public class ChatModelSnapshot
{
    public Size ControlSize { get; init; }

    public IBaseLabelVisual? Title { get; init; }

    public IEnumerable<ICartesianAxis>? XAxes { get; init; }
    public IEnumerable<ICartesianAxis>? YAxes { get; init; }
    public IEnumerable<ICartesianSeries>? Series { get; init; }
}
