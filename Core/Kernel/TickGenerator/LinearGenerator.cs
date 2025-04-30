using Core.Kernel.Axis;
using Core.Kernel.Drawing.Geometry;
using System.Runtime.CompilerServices;

namespace Core.Kernel.TickGenerator
{
    public class LinearGenerator<TTLabelGeometry>(CoreCartesianAxis axis)
        : BaseTickGenerator<TTLabelGeometry>(axis)
        where TTLabelGeometry : BaseLabelGeometry, new()
    {
        private readonly double[] _divBy10 = [2.0, 2.0, 2.5]; // 静态预定义除数

        public override void Generate(float axisLength)
        {
            GenerateTicks(axisLength, 12f);
        }

        private void GenerateTicks(float axisLength, float labelLength)
        {
            float currentLabelLength = labelLength;
            float maxSize = float.NegativeInfinity;
            string maxText = string.Empty;

            IEnumerable<double> majorPositions;
            IEnumerable<string> majorLabels;
            do
            {
                majorPositions = GenerateNumericTickPositions(axisLength, currentLabelLength);

                majorLabels = MeasuredLabels(majorPositions, ref maxText, ref maxSize);

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
                = GenerateMinorPositions(majorPositionsList).ToList();
            List<string> majorLabelsList = majorLabels.ToList();

            Ticks = CombineTicks(majorPositionsList, majorLabelsList, minorPositionsList);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override string GetPositionLabel(double value)
        {
            if (_formatLabel != null)
                return _formatLabel.Invoke(value);

            return value.ToString();
        }

        private IEnumerable<double> GenerateNumericTickPositions(float axisLength, float labelWidth)
        {
            double idealSpace = GetIdealTickSpace(axisLength, labelWidth);
            double firstTick = _bound.Minimum / idealSpace * idealSpace;

            for (double pos = firstTick; pos <= _bound.Maximum; pos += idealSpace)
            {
                yield return pos;
            }
        }

        private double GetIdealTickSpace(float axisLength, float labelWidth)
        {
            // 通过像素来计算个数
            int targetTickCount = Math.Max(1, (int)(axisLength / labelWidth));
            // 通过实际范围来计算个数
            double rangeSpan = _bound.Span;
            int exponent = (int)Math.Log(rangeSpan, 10);
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
