using System.Runtime.CompilerServices;
using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace Core.Kernel.TickGenerator
{
    public class LinearGenerator<TTLabelGeometry>(Func<double, string>? formatLabel)
        : BaseTickGenerator<TTLabelGeometry>
        where TTLabelGeometry : BaseLabelGeometry, new()
    {
        private readonly double[] _divBy10 = [2.0, 2.0, 2.5]; // 静态预定义除数

        public Func<double, string>? FormatLabel { get; set; } = formatLabel;

        public override IEnumerable<Tick> Generate(Bound range, bool vertical, float axisLength)
        {
            return GenerateTicks(range, vertical, axisLength, 12f);
        }

        private IEnumerable<Tick> GenerateTicks(Bound range, bool vertical, float axisLength, float labelLength)
        {
            float currentLabelLength = labelLength;
            float maxSize = float.NegativeInfinity;
            string maxText = string.Empty;

            IEnumerable<double> majorPositions;
            IEnumerable<string> majorLabels;
            do
            {
                majorPositions = GenerateNumericTickPositions(range, axisLength, currentLabelLength);

                majorLabels = MeasuredLabels(majorPositions, vertical, ref maxText, ref maxSize);

                // 使用预给出的labelLength值重新分配tick
                if (currentLabelLength < maxSize)
                    currentLabelLength = maxSize;
                else
                {
                    MaxLabel = maxText;
                    break;
                }

            } while (true);

            List<double> majorPositionsList = majorPositions.ToList();
            List<double> minorPositionsList
                = GenerateMinorPositions(majorPositionsList, range).ToList();
            List<string> majorLabelsList = majorLabels.ToList();

            return CombineTicks(majorPositionsList, majorLabelsList, minorPositionsList);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override string GetPositionLabel(double value)
        {
            if (FormatLabel != null)
                return FormatLabel.Invoke(value);

            return value.ToString();
        }

        private IEnumerable<double> GenerateNumericTickPositions(Bound range,
            float axisLength, float labelWidth)
        {
            double idealSpace = GetIdealTickSpace(range, axisLength, labelWidth);
            double firstTick = range.Minimum / idealSpace * idealSpace;

            for (double pos = firstTick; pos <= range.Maximum; pos += idealSpace)
            {
                yield return pos;
            }
        }

        private double GetIdealTickSpace(Bound range, float axisLength, float labelWidth)
        {
            // 通过像素来计算个数
            int targetTickCount = Math.Max(1, (int)(axisLength / labelWidth));
            // 通过实际范围来计算个数
            double rangeSpan = range.Span;
            int exponent = (int)Math.Log(range.Span, 10);
            double initialSpace = Math.Pow(10, exponent);
            double neededSpace = CalculateNeededSpace(labelWidth);

            IEnumerable<double> candidates
                = GenerateSpaceCandidates(initialSpace, rangeSpan, targetTickCount).Reverse();

            foreach (double space in candidates)
            {
                double tickCount = rangeSpan / space;
                double spacePerTick = axisLength / tickCount;

                if (spacePerTick >= neededSpace)
                    return space;
            }

            return initialSpace;
        }

        private IEnumerable<double> GenerateSpaceCandidates(
            double initialSpace, double rangeSpan, int targetTickCount)
        {
            double current = initialSpace;
            int divIndex = 0;

            yield return current;

            while (rangeSpan / current < targetTickCount)
            {
                current /= _divBy10[divIndex % _divBy10.Length];
                divIndex++;
                yield return current;
            }
        }

        private double CalculateNeededSpace(float labelWidth)
        {
            if (labelWidth < 10) return labelWidth * 2;
            if (labelWidth < 25) return labelWidth * 1.5;
            return labelWidth * 1.2;
        }
    }
}
