using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel;
public interface ICartesianAxis : IChartElement
{
    Paint? AxisLinePaint { get; set; }
    Paint? LabelPaint { get; set; }
    Paint? NamePaint { get; set; }
    Paint? SeparatorPaint { get; set; }
    Paint? SubSeparatorPaint { get; set; }
    Paint? SubTickPaint { get; set; }
    Paint? TickPaint { get; set; }

    float LabelDensity { get; set; }
    Rect LabelDesiredRect { get; set; }
    Func<double, string>? Labeler { get; set; }
    Padding LabelPadding { get; set; }
    float LabelRotation { get; set; }
    double Max { get; }
    double Min { get; }
    string? Name { get; set; }
    Rect NameDesiredRect { get; set; }
    Padding NamePadding { get; set; }
    
    float NameRotation { get; set; }
    AxisOrientation Orientation { get; }
    AxisPosition Position { get; set; }
    int SeparatorCount { get; set; }
 
    float TickLength { get; set; }
    float X { get; set; }
    float Y { get; set; }

    void Reset(AxisOrientation orientation);
    void SetBound(double min, double max);

    Size MeasureMaxLabelSize();
    Size MeasureNameLabelSize();
    Size MeasureLabelSize(Size size);
}
