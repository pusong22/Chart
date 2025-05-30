using Core.Primitive;

namespace Core.Kernel.Series;

public abstract class CoreCartesianSeries : CoreSeries
{
    private int _xIndex;
    private int _yIndex;

    public int XIndex
    {
        get => _xIndex;
        set => _xIndex = value;
    }

    public int YIndex
    {
        get => _yIndex;
        set => _yIndex = value;
    }


    public abstract IEnumerable<Coordinate> Fetch();

    public virtual SeriesBound GetBound()
    {
        var primaryBound = new Bound(0d, 0d);
        var secondaryBound = new Bound(0d, 0d);
        foreach (var item in Fetch())
        {
            primaryBound.AppendValue(item.X);
            secondaryBound.AppendValue(item.Y);
        }

        primaryBound.Expand(0.15d);
        secondaryBound.Expand(0.15d);

        return new SeriesBound()
        {
            PrimaryBound = primaryBound,
            SecondaryBound = secondaryBound
        };
    }
}
