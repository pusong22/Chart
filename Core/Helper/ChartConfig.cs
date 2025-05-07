using Core.Kernel.Axis;
using Core.Kernel.Series;
using Core.Primitive;

namespace Core.Helper;
public class ChartConfig
{
    private Provider? _provider;
    private List<Action<CoreAxis>> _axisBuilder = [];
    private List<Action<CoreSeries>> _seriesBuilder = [];
    private readonly Dictionary<Type, Delegate> _mappers = [];

    public static ChartConfig Instance { get; } = new();

    public static bool EnableLog { get; set; } = false;
    public static double MaxFPS { get; set; } = 60d;
    public static bool ShowFPS { get; set; } = false;
    public static bool USE_GPU { get; set; } = false;

    public static Func<float, float> AnimateFunc { get; set; } = t => t * t;
    public static TimeSpan AnimateDuration { get; set; } = TimeSpan.FromMilliseconds(800);

    public static void Configure(Action<ChartConfig> defaultSetting)
    {
        if (defaultSetting is null)
            throw new ArgumentNullException(nameof(defaultSetting));

        defaultSetting(Instance);
    }

    public void SetProvider(Provider provider)
    {
        _provider = provider;
    }

    public Provider GetProvider()
    {
        return _provider ?? throw new ArgumentNullException($"{nameof(_provider)}");
    }

    #region Apply Style

    public ChartConfig AddRuleForAxes(Action<CoreAxis> predicate)
    {
        _axisBuilder.Add(predicate);
        return this;
    }

    public ChartConfig AddRuleForSeries(Action<CoreSeries> predicate)
    {
        _seriesBuilder.Add(predicate);
        return this;
    }

    public void ApplyStyleToAxis(CoreAxis axis)
    {
        foreach (var action in _axisBuilder)
            action(axis);
    }

    public void ApplyStyleToSeries(CoreSeries series)
    {
        foreach (var action in _seriesBuilder)
            action(series);
    }

    public void ApplyStyle(Action<ChartConfig> builder)
    {
        builder(Instance);
    }
    #endregion

    #region Coordinate Mapper
    public ChartConfig AddValueTypeParser<TValueType>(Func<double, TValueType, Coordinate> parser)
    {
        var type = typeof(TValueType);
        _mappers[type] = parser;
        return this;
    }

    public Func<double, TValueType, Coordinate> GetParser<TValueType>()
    {
        if (_mappers.TryGetValue(typeof(TValueType), out var map))
        {
            return (Func<double, TValueType, Coordinate>)map;
        }

        throw new NotImplementedException($"Don`t parse the ${typeof(TValueType)} type.");
    }
    #endregion
}
