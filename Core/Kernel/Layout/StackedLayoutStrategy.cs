using Core.Primitive;

namespace Core.Kernel.Layout
{
    public class StackedLayoutStrategy(CartesianChart chart) : BaseLayoutStrategy
    {
        public override void CalculateLayout(Margin margin)
        {
            float l = margin.Left,
                t = margin.Top,
                r = margin.Right,
                b = margin.Bottom;

            int xaxisCount = chart.XAxes!.Length;
            int yaxisCount = chart.YAxes!.Length;

            Size xmaxNameSize = new(0f, 0f);
            Size xmaxLabelSize = new(0f, 0f);

            Size ymaxNameSize = new(0f, 0f);
            Size ymaxLabelSize = new(0f, 0f);

            float xAxisBetweenPx = 0f;
            float yAxisBetweenPx = 0f;

            for (int i = 0; i < xaxisCount; i++)
            {
                ICartesianAxis axis = chart.XAxes[i];

                Size nameSize = axis.MeasureNameLabelSize();
                Size labelSize = axis.MeasureLabelSize(chart.ControlSize);

                float temp = nameSize.Height + labelSize.Height + axis.TickLength;

                if (labelSize.Width * 0.5f > l) l = labelSize.Width * 0.5f;
                if (labelSize.Width * 0.5f > r) r = labelSize.Width * 0.5f;
                if (labelSize.Width * 0.5f > xAxisBetweenPx)
                    xAxisBetweenPx = labelSize.Width * 0.5f;

                if (nameSize.Height > xmaxNameSize.Height) xmaxNameSize = nameSize;
                if (labelSize.Height > xmaxLabelSize.Height) xmaxLabelSize = labelSize;

                // Bottom
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > b) b = temp;
                }
                else
                {
                    if (temp > t) t = temp;
                }
            }

            for (int i = 0; i < yaxisCount; i++)
            {
                ICartesianAxis axis = chart.YAxes[i];

                Size nameSize = axis.MeasureNameLabelSize();
                Size labelSize = axis.MeasureLabelSize(chart.ControlSize);

                float temp = nameSize.Width + labelSize.Width + axis.TickLength;

                if (labelSize.Height * 0.5f > t) t = labelSize.Height * 0.5f;
                if (labelSize.Height * 0.5f > b) b = labelSize.Height * 0.5f;
                if (labelSize.Height * 0.5f > yAxisBetweenPx)
                    yAxisBetweenPx = labelSize.Height * 0.5f;

                if (nameSize.Width > ymaxNameSize.Width) ymaxNameSize = nameSize;
                if (labelSize.Width > ymaxLabelSize.Width) ymaxLabelSize = labelSize;

                // Left
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > l) l = temp;
                }
                else
                {
                    if (temp > r) r = temp;
                }
            }

            Point location = new(l, t);
            Size size = new(
                chart.ControlSize.Width - r - l,
                chart.ControlSize.Height - b - t);

            float subXPx = (xaxisCount - 1) * xAxisBetweenPx;
            float subYPx = (yaxisCount - 1) * yAxisBetweenPx;

            float xOffset = l;
            float xaxisLength = (size.Width - subXPx) / xaxisCount;
            foreach (var axis in chart.XAxes)
            {
                // Bottom
                if (axis.Position == AxisPosition.Start)
                {
                    axis.NameDesiredRect = new Rect(
                        new Point(xOffset, chart.ControlSize.Height - xmaxNameSize.Height),
                        new Size(xaxisLength, xmaxNameSize.Height));

                    axis.LabelDesiredRect = new Rect(
                        new Point(xOffset, axis.NameDesiredRect.Y - xmaxLabelSize.Height),
                        new Size(xaxisLength, xmaxLabelSize.Height));
                }
                else
                {

                }

                xOffset += xaxisLength + xAxisBetweenPx;

                axis.Y = xmaxNameSize.Height + xmaxLabelSize.Height * 0.5f;
            }


            float yOffset = t;
            float yaxisLength = (size.Height - subYPx) / yaxisCount;
            foreach (var axis in chart.YAxes)
            {
                // Left
                if (axis.Position == AxisPosition.Start)
                {
                    axis.NameDesiredRect = new Rect(
                        new Point(0f, yOffset),
                        new Size(ymaxNameSize.Width, yaxisLength));

                    axis.LabelDesiredRect = new Rect(
                        new Point(ymaxNameSize.Width, yOffset),
                        new Size(ymaxLabelSize.Width, yaxisLength));
                }
                else
                {

                }

                yOffset += yaxisLength + yAxisBetweenPx;

                axis.X = ymaxNameSize.Width + ymaxLabelSize.Width * 0.5f;
            }

            chart.DrawnLocation = location;
            chart.DrawnSize = size;
        }
    }
}
