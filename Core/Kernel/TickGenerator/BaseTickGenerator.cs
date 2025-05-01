using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.Kernel.TickGenerator
{
    public abstract class BaseTickGenerator(CoreCartesianAxis axis)
    {
        protected Func<double, string>? _formatLabel = axis.Labeler;
        protected int _minorCount = 5;
        protected bool _vertical = axis.Orientation == AxisOrientation.Y;
        protected Bound _bound = new(axis.Min, axis.Max);

        public string? MaxLabel { get; protected set; }
        public IEnumerable<Tick> Ticks { get; protected set; } = [];

        public abstract void Generate(float axisLength);

        protected IEnumerable<string> MeasuredLabels(
            IEnumerable<double> positions,
            ref string maxText,
            ref float maxSize)
        {
            IList<string> labels = [];
            foreach (double position in positions)
            {
                string label = GetPositionLabel(position);

                Size measuredValue = axis.MeasureLabelSize(label);
                float size = _vertical
                    ? measuredValue.Height
                    : measuredValue.Width;

                if (size > maxSize)
                {
                    maxSize = size;
                    maxText = label;
                }

                labels.Add(label);
            }

            return labels;
        }

        protected IEnumerable<Tick> CombineTicks(
            IReadOnlyList<double> majorPositions,
            IReadOnlyList<string> majorLabels,
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

        protected IEnumerable<double> GenerateMinorPositions(IReadOnlyList<double> majorTicks)
        {
            if (majorTicks.Count < 2) yield break;

            double majorSpace = majorTicks[1] - majorTicks[0];
            double minorSpace = majorSpace / 5;

            // 生成主刻度之前的次刻度
            for (double majorPos = majorTicks[0] - majorSpace; majorPos >= _bound.Minimum; majorPos -= majorSpace)
            {
                foreach (double minorPos in GenerateMinorsForMajor(majorPos, minorSpace, _bound))
                {
                    yield return minorPos;
                }
            }

            // 生成所有主刻度之间的次刻度
            foreach (double majorPos in majorTicks)
            {
                foreach (double minorPos in GenerateMinorsForMajor(majorPos, minorSpace, _bound))
                {
                    yield return minorPos;
                }
            }
        }

        protected abstract string GetPositionLabel(double value);

        private IEnumerable<double> GenerateMinorsForMajor(
            double majorPos, double minorSpacing, Bound range)
        {
            for (int i = 1; i < _minorCount; i++)
            {
                double pos = majorPos + minorSpacing * i;
                if (pos > range.Maximum) yield break;
                if (pos >= range.Minimum) yield return pos;
            }
        }
    }
}
