using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Axis;

public abstract class CoreAxis : ChartElement
{
    private Paint? _namePaint = new Brush();
    private Paint? _labelPaint = new Brush();

    public double Min { get; protected internal set; } = double.PositiveInfinity;
    public double Max { get; protected internal set; } = double.NegativeInfinity;

    public string? Name { get; set; }

    public float NameRotation { get; set; }
    public float LabelRotation { get; set; }

    public float NameSize { get; set; } = 16f;
    public float LabelSize { get; set; } = 16f;

    public Padding NamePadding { get; set; } = new(5f);
    public Padding LabelPadding { get; set; } = new(5f);

    public Paint? NamePaint
    {
        get => _namePaint;
        set
        {
            if (value != _namePaint)
            {
                _namePaint = value;
            }
        }
    }

    public Paint? LabelPaint
    {
        get => _labelPaint;
        set
        {
            if (value != _labelPaint)
            {
                _labelPaint = value;
            }
        }
    }

    public Func<double, string>? Labeler { get; set; }

    // 保存测量后的坐标信息
    public Rect NameDesiredRect { get; protected internal set; }
    public Rect LabelDesiredRect { get; protected internal set; }

    public abstract Size MeasureNameLabelSize();
    public abstract Size MeasureMaxLabelSize();
    public abstract Size MeasureLabelSize(Size size);
}
