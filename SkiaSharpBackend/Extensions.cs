using Core.Helper;
using Core.Kernel.Axis;
using Core.Kernel.Painting;
using Core.Kernel.Series;
using Core.Primitive;
using SkiaSharp;

namespace SkiaSharpBackend;
public static class Extensions
{
    public static SKTypeface ToSKTypeface(this Paint paint)
    {
        if (paint.FontFamily is not null)
            return SKTypeface.FromFamilyName(paint.FontFamily);

        return SKTypeface.Default;
    }

    public static SKPaintStyle ToSKStyle(this Paint paint)
    {
        return paint.Style switch
        {
            PaintStyle.Fill => SKPaintStyle.Fill,
            PaintStyle.Stroke => SKPaintStyle.Stroke,
            _ => SKPaintStyle.Fill,
        };
    }

    public static SKColor ToSKColor(this Paint paint)
    {
        var color = paint.Color;

        return new SKColor(color.Red, color.Green, color.Blue);
    }

    public static SKPoint ToSKPoint(this Point p)
    {
        return new SKPoint(p.X, p.Y);
    }

    public static SKRect ToSKRect(this Rect rect)
    {
        return new SKRect(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
    }

    public static SKPathEffect? ToSKPathEffect(this Paint paint)
    {
        if (paint.PathEffect is DashEffectSetting dashSettings)
        {
            return SKPathEffect.CreateDash(dashSettings.Intervals, dashSettings.Phase);
        }

        return null;
    }

    public static Color ToColor(this SKColor color)
    {
        return new Color(color.Red, color.Green, color.Blue, color.Alpha);
    }

    public static void UseDefault(this ChartConfig chartConfig)
    {
        chartConfig.SetProvider(new SkiaSharpProvider());

        // TODO: paint的Style根据不同功能自动设置
        chartConfig.ApplyStyle(config =>
        {
            config
            .AddRuleForAxes(axis =>
            {
                axis.NameSize ??= 16f;
                axis.LabelSize ??= 16f;
                axis.NameRotation ??= 0f;
                axis.LabelRotation ??= 0f;
                axis.NamePaint ??= new Paint();
                axis.LabelPaint ??= new Paint();
                axis.NamePadding ??= new Padding(5f);
                axis.LabelPadding ??= new Padding(15f);
                axis.Labeler ??= v => v.ToString($"N1");
                if (axis is CoreCartesianAxis cartesianAxis)
                {
                    cartesianAxis.ShowSeparatorLine ??= true;
                    cartesianAxis.LabelDensity ??= 1f;
                    cartesianAxis.TickLength ??= 5f;
                    cartesianAxis.DrawTickPath ??= true;
                    cartesianAxis.SeparatorCount ??= 9;
                    cartesianAxis.TickPaint ??= new Paint();
                    cartesianAxis.SubTickPaint ??= new Paint();
                    cartesianAxis.SeparatorPaint ??= new Paint()
                    {
                        PathEffect = new DashEffectSetting([3, 3])
                    };
                    cartesianAxis.SubSeparatorPaint ??= new Paint()
                    {
                        PathEffect = new DashEffectSetting([3, 3])
                    };
                }
            })
            .AddRuleForSeries(series =>
            {
                if (series is CoreCartesianSeries cartesianSeries)
                {
                    cartesianSeries.LinePaint ??= new Paint();
                }
            });
        });

        chartConfig
            .AddValueTypeParser<short>((x, y) => new(y, y))
            .AddValueTypeParser<int>((x, y) => new(x, y))
            .AddValueTypeParser<double>((x, y) => new(x, y))
            .AddValueTypeParser<float>((x, y) => new(x, y))
            .AddValueTypeParser<long>((x, y) => new(x, y));
    }
}
