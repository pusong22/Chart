using Core.Kernel.Painting;

namespace Core.Kernel.Series;

public abstract class CoreSeries : ChartElement
{
    private Paint? _seriesPaint = new Pen();

    public Paint? SeriesPaint
    {
        get => _seriesPaint;
        set
        {
            if (value != _seriesPaint)
            {
                _seriesPaint = value;
            }
        }
    }
}
