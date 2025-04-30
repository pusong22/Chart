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

            int xaxisCount = chart.XAxes.Count();
            int yaxisCount = chart.YAxes.Count();

            //var cached = new Dictionary<ICartesianAxis, (Size, Size)>();

            float xaxisLength = chart.ScaledControlSize.Width / xaxisCount;
            foreach (var axis in chart.XAxes)
            {
                axis.GenerateTick(xaxisLength);
                var nameSize = axis.MeasureNameLabelSize();
                var tickLabelSize = axis.MeasureTickLabelSize();

                var temp = nameSize.Height + tickLabelSize.Height;

                // Bottom
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > b) b = temp;

                    axis.NameDesiredRect = new Rect(
                        new Point(0f, chart.ScaledControlSize.Height - nameSize.Height),
                        new Size(chart.ScaledControlSize.Width, nameSize.Height));
                    
                    axis.LabelDesiredRect = new Rect(
                        new Point(0f, axis.NameDesiredRect.Y - tickLabelSize.Height),
                        new Size(chart.ScaledControlSize.Width, tickLabelSize.Height));
                }
                else
                {
                    if (temp > t) t = temp;

                }
            }

            float yaxisLength = chart.ScaledControlSize.Height / yaxisCount;
            foreach (var axis in chart.YAxes)
            {
                axis.GenerateTick(yaxisLength);
                var nameSize = axis.MeasureNameLabelSize();
                var tickLabelSize = axis.MeasureTickLabelSize();

                var temp = nameSize.Width + tickLabelSize.Width;

                // Left
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > l) l = temp;

                    axis.NameDesiredRect = new Rect(
                        new Point(0f, 0f),
                        new Size(nameSize.Width, chart.ScaledControlSize.Height));
                    
                    axis.LabelDesiredRect = new Rect(
                        new Point(nameSize.Width, 0f),
                        new Size(tickLabelSize.Width, chart.ScaledControlSize.Height));
                }
                else
                {
                    if (temp > r) r = temp;
                }
            }

            float xOffset = 0f, yOffset = 0f;
            foreach (var axis in chart.XAxes)
            {
                axis.Xo = l + xOffset;
                if (axis.Position == AxisPosition.Start)
                {
                    axis.Yo = chart.ScaledControlSize.Height - b;
                }
                else
                {
                    axis.Yo = t;
                }

                axis.Size = xaxisLength;

                xOffset += xaxisLength;
            }

            foreach (var axis in chart.YAxes)
            {
                axis.Yo = t + yOffset;
                if (axis.Position == AxisPosition.Start)
                {
                    axis.Xo = l;
                }
                else
                {
                    axis.Xo = chart.ScaledControlSize.Width - r;
                }

                axis.Size = yaxisLength;

                yOffset += yaxisLength;
            }

            Point location = new(l, t);
            Size size = new(
                chart.ScaledControlSize.Width - r - l,
                chart.ScaledControlSize.Height - b - t);

            return new(location, size);
        }
    }
}
