using Core.Kernel.Axis;
using Core.Kernel.Motion;
using Core.Primitive;

namespace Core.Helper;
public static class Extensions
{
    public static void Animate(this Animatable animatable, Func<float, float>? easingFunction, TimeSpan speed, params string[]? properties) =>
        animatable.Animate(new Animation(easingFunction, speed), properties);

    public static void Animate(this Animatable animatable, Animation animation, params string[]? properties)
    {
        animatable.SetMotion(animation, properties);
    }

    public static IEnumerable<double> EnumerateSeparators(
        this CoreCartesianAxis axis,
        double start,
        double end,
        double step)
    {
        var relativeEnd = end - start;

        if (relativeEnd / step > 1000)
            throw new Exception("Tick count is too larger");

        // start from -step to include the first separator/sub-separator
        // and end at relativeEnd + step to include the last separator/sub-separator

        for (var i = start; i <= end; i += step)
            yield return i;
    }

    public static double GetIdealStep(this CoreCartesianAxis axis, Size controlSize)
    {
        var maxLabelSize = axis.MeasureMaxLabelSize();

        var w = maxLabelSize.Width;
        var h = maxLabelSize.Height;

        //if (axis.Orientation == AxisOrientation.X) w *= axis.LabelDensity;
        //if (axis.Orientation == AxisOrientation.Y) h *= axis.LabelDensity;

        var density = (1 + axis.LabelDensity) / 2f;

        const float MinLabelSize = 10; // Assume the label size is at least 10px

        if (w < MinLabelSize) w = MinLabelSize;
        if (h < MinLabelSize) h = MinLabelSize;

        var range = axis.Max - axis.Min;

        if (range == 0) range = 0.15 * axis.Max;

        var separations = axis.Orientation == AxisOrientation.Y
            ? Math.Round(controlSize.Height / h, 0) * density
            : Math.Round(controlSize.Width / w, 0) * density;

        var minimum = range / separations;

        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum!.Value) / Math.Log(10)));
        // 倍数太大的话需要更大的magnitude来跟上
        var residual = minimum / magnitude;

        var tick = residual > 5 ? 10 * magnitude : residual > 2 ? 5 * magnitude : residual > 1 ? 2 * magnitude : magnitude;

        return tick;
    }
}
