using Core.Primitive;

namespace Core.Kernel.Visual;
public abstract class VisualElement : ChartElement
{
    public float X { get; protected internal set; }
    public float Y { get; protected internal set; }

    public abstract Size Measure();
}
