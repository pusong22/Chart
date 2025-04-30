using Core.Kernel.Drawing;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreAxis
{
    public Rect Rect { get; }
    public double Min { get; set; }
    public double Max { get; set; }
    public Bound? DataBound { get; protected set; }
    public string? Name { get; set; }
    public float NameSize { get; set; }
    public Padding? NamePadding { get; set; }
    public Paint? NamePaint { get; set; }

    public float LabelSize { get; set; }
    public Padding? LabelPadding { get; set; }
    public Paint? LabelPaint { get; set; }

    public Rect NameDesiredRect { get; set; }
    public Rect LabelDesiredRect { get; set; }

    public abstract Size MeasureNameLabelSize();
}
