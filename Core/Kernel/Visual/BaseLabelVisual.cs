using Core.Kernel.Drawing.Geometry;
using Core.Kernel.Painting;
using Core.Primitive;

namespace Core.Kernel.Visual;

public abstract class BaseLabelVisual<TLabel> : IBaseLabelVisual
    where TLabel : BaseLabelGeometry, new()
{
    private TLabel? _labelGeometry;
    private Paint? _textPaint = new Brush();
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

    public Rect TextDesiredRect { get; set; }
    public object? Tag { get; set; }

    public void CalculateGeometries(CartesianChart chart)
    {
        if (TextPaint is null || Text is null) return;

        _labelGeometry = new TLabel();
        chart.RequestGeometry(TextPaint, _labelGeometry);

        _labelGeometry.Text = Text;
        _labelGeometry.Paint = TextPaint;
        _labelGeometry.RotateTransform = TextRotation;
        _labelGeometry.Padding = TextPadding;
        _labelGeometry.X = TextDesiredRect.X + TextDesiredRect.Width * 0.5f;
        _labelGeometry.Y = TextDesiredRect.Y + TextDesiredRect.Height * 0.5f;
    }

    public Size Measure()
    {
        if (TextPaint is null || string.IsNullOrWhiteSpace(Text))
            return new Size(0f, 0f);

        var geometry = new TLabel
        {
            Text = Text,
            Paint = TextPaint,
            RotateTransform = TextRotation,
            Padding = TextPadding
        };

        // TODO: 性能
        return geometry.Measure();
    }
}
