using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Visual;
public interface IBaseLabelVisual : IChartElement
{
    Align HorizontalAlign { get; set; }
    string? Text { get; set; }
    Rect TextDesiredRect { get; set; }
    Padding TextPadding { get; set; }
    Paint? TextPaint { get; set; }
    float TextRotation { get; set; }
    float TextSize { get; set; }
    Align VerticalAlign { get; set; }

    Size Measure();
}
