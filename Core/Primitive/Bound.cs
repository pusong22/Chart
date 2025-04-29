namespace Core.Primitive;
public class Bound(double min, double max)
{
    public double Minimum { get; set; } = min;
    public double Maximum { get; set; } = max;
    public double Span => Maximum - Minimum;
}
