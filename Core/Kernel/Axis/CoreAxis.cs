using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreAxis : ChartElement
{
    public double Min { get; set; } = double.NaN;
    public double Max { get; set; } = double.NaN;
    public string? Name { get; set; }
    public float? NameRotation { get; set; }
    public float? NameSize { get; set; }
    public Padding? NamePadding { get; set; }
    public Paint? NamePaint { get; set; }

    public Func<double, string>? Labeler { get; set; }

    public float? LabelRotation { get; set; }
    public float? LabelSize { get; set; }
    public Padding? LabelPadding { get; set; }
    public Paint? LabelPaint { get; set; }

    // 保存测量后的坐标信息
    public Rect NameDesiredRect { get; protected internal set; }
    public Rect LabelDesiredRect { get; protected internal set; }

    public abstract Size MeasureNameLabelSize();
    public abstract Size MeasureMaxLabelSize();
    public abstract Size MeasureLabelSize(Size size);
}
