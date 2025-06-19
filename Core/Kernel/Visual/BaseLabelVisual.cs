using Core.Kernel.Chart;
using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Visual;
public abstract class BaseLabelVisual : VisualElement
{
    private Paint? _textPaint;
    public string? Text { get; set; }

    public float TextSize { get; set; } = 16f;

    public Paint? TextPaint
    {
        get => _textPaint;
        set
        {
            if (_textPaint != value)
            {
                _textPaint = value;
            }
        }
    }

    public Align VerticalAlign { get; set; } = Align.Middle;
    public Align HorizontalAlign { get; set; } = Align.Middle;

    public float TextRotation { get; set; }

    public Padding TextPadding { get; set; } = new Padding(5f);

    public Rect TextDesiredRect { get; protected internal set; }
}


public abstract class BaseLabelVisual<TLabel> : BaseLabelVisual
    where TLabel : BaseLabelGeometry, new()
{
    private TLabel? _labelGeometry;

    public override void Invalidate(CoreChart chart)
    {
        if (TextPaint is null || Text is null) return;

        _labelGeometry ??= chart.CanvasContext.RequestGeometry<TLabel>(TextPaint);

        _labelGeometry.Text = Text;
        _labelGeometry.TextSize = TextSize;
        _labelGeometry.Paint = TextPaint;
        _labelGeometry.RotateTransform = TextRotation;
        _labelGeometry.Padding = TextPadding;
        _labelGeometry.X = TextDesiredRect.X + TextDesiredRect.Width * 0.5f;
        _labelGeometry.Y = TextDesiredRect.Y + TextDesiredRect.Height * 0.5f;
    }

    public override Size Measure()
    {
        if (TextPaint is null || string.IsNullOrWhiteSpace(Text))
            return new Size(0f, 0f);

        var geometry = new TLabel
        {
            Text = Text,
            TextSize = TextSize,
            Paint = TextPaint,
            RotateTransform = TextRotation,
            Padding = TextPadding
        };

        // TODO: 性能
        return geometry.Measure();
    }
}
