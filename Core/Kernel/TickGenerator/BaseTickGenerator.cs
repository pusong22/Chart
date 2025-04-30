using Core.Kernel.Drawing;
using Core.Kernel.Drawing.Geometry;
using Core.Primitive;

namespace Core.Kernel.TickGenerator
{
    public abstract class BaseTickGenerator<TTLabelGeometry>
        where TTLabelGeometry : BaseLabelGeometry, new()
    {
        protected BaseTickGenerator()
        {
            MinorCount = 5;
        }

        protected int MinorCount { get; set; }

        public Paint? LabelPaint { get; set; }
        public string? MaxLabel { get; protected set; }

        public abstract IEnumerable<Tick> Generate(Bound range, bool vertical, float axisLength);

        protected IEnumerable<string> MeasuredLabels(IEnumerable<double> positions,
           bool vertical, ref string maxText, ref float maxSize)
        {
            IEnumerable<string> labels = FormatLabels(positions);
            foreach (string label in labels)
            {

                var geometry = new TTLabelGeometry()
                {
                    Text = label,
                    TextSize = 12f,
                    Paint = LabelPaint,
                };

                Size measuredValue = geometry.Measure();
                float size;
                if (vertical)
                    size = measuredValue.Height;
                else
                    size = measuredValue.Width;

                if (size > maxSize)
                {
                    maxSize = size;
                    maxText = label;
                }
            }

            return labels;
        }

        protected IEnumerable<Tick> CombineTicks(
            IReadOnlyList<double> majorPositions, IReadOnlyList<string> majorLabels,
            IReadOnlyList<double> minorPositions)
        {
            // 主刻度
            for (int i = 0; i < majorPositions.Count; i++)
            {
                yield return Tick.Major(majorPositions[i], majorLabels[i]);
            }

            // 次刻度
            foreach (double pos in minorPositions)
            {
                yield return Tick.Minor(pos);
            }
        }

        protected IEnumerable<double> GenerateMinorPositions(
            IReadOnlyList<double> majorTicks, Bound range)
        {
            if (majorTicks.Count < 2) yield break;

            double majorSpace = majorTicks[1] - majorTicks[0];
            double minorSpace = majorSpace / 5;

            // 生成主刻度之前的次刻度
            for (double majorPos = majorTicks[0] - majorSpace; majorPos >= range.Minimum; majorPos -= majorSpace)
            {
                foreach (double minorPos in GenerateMinorsForMajor(majorPos, minorSpace, range))
                {
                    yield return minorPos;
                }
            }

            // 生成所有主刻度之间的次刻度
            foreach (double majorPos in majorTicks)
            {
                foreach (double minorPos in GenerateMinorsForMajor(majorPos, minorSpace, range))
                {
                    yield return minorPos;
                }
            }
        }

        protected abstract string GetPositionLabel(double value);

        private IEnumerable<double> GenerateMinorsForMajor(
            double majorPos, double minorSpacing, Bound range)
        {
            for (int i = 1; i < MinorCount; i++)
            {
                double pos = majorPos + minorSpacing * i;
                if (pos > range.Maximum) yield break;
                if (pos >= range.Minimum) yield return pos;
            }
        }

        private IEnumerable<string> FormatLabels(IEnumerable<double> positions)
        {
            foreach (double pos in positions)
            {
                yield return GetPositionLabel(pos);
            }
        }
    }
}
