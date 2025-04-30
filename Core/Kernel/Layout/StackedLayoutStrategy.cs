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
                var labelSize = axis.MeasureLabelSize();

                var temp = nameSize.Height + labelSize.Height;

                // Bottom
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > b) b = temp;

                    axis.NameDesiredRect = new Rect(
                        new Point(0f, chart.ScaledControlSize.Height - nameSize.Height),
                        new Size(chart.ScaledControlSize.Width, nameSize.Height));

                    axis.LabelDesiredRect = new Rect(
                        new Point(0f, axis.NameDesiredRect.Y - labelSize.Height),
                        new Size(chart.ScaledControlSize.Width, labelSize.Height));
                }
                else
                {
                    if (temp > t) t = temp;
                }

                axis.Y = nameSize.Height + labelSize.Height * 0.5f;

                if (labelSize.Width * 0.5f > l) l = labelSize.Width * 0.5f;
                if (labelSize.Width * 0.5f > r) r = labelSize.Width * 0.5f;
            }

            float yaxisLength = chart.ScaledControlSize.Height / yaxisCount;
            foreach (var axis in chart.YAxes)
            {
                axis.GenerateTick(yaxisLength);
                var nameSize = axis.MeasureNameLabelSize();
                var labelSize = axis.MeasureLabelSize();

                var temp = nameSize.Width + labelSize.Width;

                // Left
                if (axis.Position == AxisPosition.Start)
                {
                    if (temp > l) l = temp;

                    axis.NameDesiredRect = new Rect(
                        new Point(0f, 0f),
                        new Size(nameSize.Width, chart.ScaledControlSize.Height));

                    axis.LabelDesiredRect = new Rect(
                        new Point(nameSize.Width, 0f),
                        new Size(labelSize.Width, chart.ScaledControlSize.Height));
                }
                else
                {
                    if (temp > r) r = temp;
                }

                axis.X = nameSize.Width + labelSize.Width * 0.5f;

                if (labelSize.Height * 0.5f > t) t = labelSize.Height * 0.5f;
                if (labelSize.Height * 0.5f > b) b = labelSize.Height * 0.5f;
            }

            Point location = new(l, t);
            Size size = new(
                chart.ScaledControlSize.Width - r - l,
                chart.ScaledControlSize.Height - b - t);

            return new(location, size);
        }
    }
}
