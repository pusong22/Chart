using Core.Kernel.Axis;
using Core.Kernel.Chart;
using Core.Primitive;

namespace Core.Kernel.Layout
{
    public class StackedLayoutStrategy(CartesianChart chart) : BaseLayoutStrategy
    {
        public override Rect CalculateLayout()
        {
            float l = 0f,
                t = 0f,
                r = 0f,
                b = 0f;

            int xaxisCount = chart.XAxes.Length;
            int yaxisCount = chart.YAxes.Length;

            var cached = new Dictionary<CoreCartesianAxis, (Size, Size)>();
            float axisBetweenPx = 5f;
            float xAxisBetweenPx = 0f;
            float yAxisBetweenPx = 0f;
            float subXPx = (xaxisCount - 1) * axisBetweenPx;
            float subYPx = (yaxisCount - 1) * axisBetweenPx;

            float totalXPx = chart.ScaledControlSize.Width - subXPx;
            float totalYPx = chart.ScaledControlSize.Height - subYPx;

            float xaxisLength = totalXPx / xaxisCount;
            for (int i = 0; i < xaxisCount; i++)
            {
                var axis = chart.XAxes[i];

                axis.GenerateTick(xaxisLength);
                var nameSize = axis.MeasureNameLabelSize();
                var labelSize = axis.MeasureMaxLabelSize();

                var temp = nameSize.Height + labelSize.Height;

                if (labelSize.Width * 0.5f > l) l = labelSize.Width * 0.5f;
                if (labelSize.Width * 0.5f > r) r = labelSize.Width * 0.5f;
                if (labelSize.Width * 0.5f > xAxisBetweenPx)
                    xAxisBetweenPx = labelSize.Width * 0.5f;

                cached[axis] = (nameSize, labelSize);

                // Bottom
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > b) b = temp;
                }
                else
                {
                    if (temp > t) t = temp;
                }

                axis.Y = nameSize.Height + labelSize.Height * 0.5f;
            }

            float yaxisLength = totalYPx / yaxisCount;
            for (int i = 0; i < yaxisCount; i++)
            {
                var axis = chart.YAxes[i];

                axis.GenerateTick(yaxisLength);
                var nameSize = axis.MeasureNameLabelSize();
                var labelSize = axis.MeasureMaxLabelSize();

                var temp = nameSize.Width + labelSize.Width;

                if (labelSize.Height * 0.5f > t) t = labelSize.Height * 0.5f;
                if (labelSize.Height * 0.5f > b) b = labelSize.Height * 0.5f;
                if (labelSize.Height * 0.5f > yAxisBetweenPx)
                    yAxisBetweenPx = labelSize.Height * 0.5f;

                cached[axis] = (nameSize, labelSize);

                // Left
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > l) l = temp;
                }
                else
                {
                    if (temp > r) r = temp;
                }

                axis.X = nameSize.Width + labelSize.Width * 0.5f;
            }

            Point location = new(l, t);
            Size size = new(
                chart.ScaledControlSize.Width - r - l,
                chart.ScaledControlSize.Height - b - t);

            subXPx = (xaxisCount - 1) * xAxisBetweenPx;
            subYPx = (yaxisCount - 1) * yAxisBetweenPx;

            float xOffset = l;
            xaxisLength = (size.Width - subXPx) / xaxisCount;
            foreach (var axis in chart.XAxes)
            {
                // Bottom
                if (axis.Position == AxisPosition.Start)
                {
                    axis.NameDesiredRect = new Rect(
                        new Point(xOffset, chart.ScaledControlSize.Height - cached[axis].Item1.Height),
                        new Size(xaxisLength, cached[axis].Item1.Height));

                    axis.LabelDesiredRect = new Rect(
                        new Point(xOffset, axis.NameDesiredRect.Y - cached[axis].Item2.Height),
                        new Size(xaxisLength, cached[axis].Item2.Height));
                }
                else
                {

                }

                xOffset += xaxisLength + xAxisBetweenPx;
            }


            float yOffset = t;
            yaxisLength = (size.Height - subYPx) / yaxisCount;
            foreach (var axis in chart.YAxes)
            {
                // Left
                if (axis.Position == AxisPosition.Start)
                {
                    axis.NameDesiredRect = new Rect(
                        new Point(0f, yOffset),
                        new Size(cached[axis].Item1.Width, yaxisLength));

                    axis.LabelDesiredRect = new Rect(
                        new Point(cached[axis].Item1.Width, yOffset),
                        new Size(cached[axis].Item2.Width, yaxisLength));
                }
                else
                {

                }

                yOffset += yaxisLength + yAxisBetweenPx;
            }


            return new(location, size);
        }
    }
}
