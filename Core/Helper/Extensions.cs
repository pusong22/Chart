using Core.Kernel;
using Core.Primitive;

namespace Core.Helper;
public static class Extensions
{
    public static bool Validate(double val)
    {
        return !double.IsNaN(val) && !double.IsInfinity(val);
    }

    public static IEnumerable<double> EnumerateSeparators(
        double start,
        double end,
        double step)
    {
        var relativeEnd = end - start;

        if (relativeEnd / step > 1000)
            throw new Exception("Tick count is too larger");

        for (var i = start; i <= end; i += step)
            yield return i;
    }

    public static double GetIdealStep(this ICartesianAxis axis, Size controlSize)
    {
        var maxLabelSize = axis.MeasureMaxLabelSize();

        var w = maxLabelSize.Width;
        var h = maxLabelSize.Height;

        const float MinLabelSize = 10; // Assume the label size is at least 10px

        if (w < MinLabelSize) w = MinLabelSize;
        if (h < MinLabelSize) h = MinLabelSize;

        var density = (1 + axis.LabelDensity) / 2f;

        var range = axis.Max - axis.Min;

        if (range == 0) range = 0.15 * axis.Max;

        var separations = axis.Orientation == AxisOrientation.Y
            ? controlSize.Height / h * density
            : controlSize.Width / w * density;

        if (separations == 0) separations = 1;
        var minimum = range / separations;

        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));
        // 倍数太大的话需要更大的magnitude来跟上
        var residual = minimum / magnitude;

        var tick = residual > 5
            ? 10 * magnitude
            : residual > 2
            ? 5 * magnitude
            : residual > 1
            ? 2 * magnitude
            : magnitude;

        return tick;
    }
}
