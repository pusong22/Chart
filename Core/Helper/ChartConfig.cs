using Core.Primitive;

namespace Core.Helper;
public class ChartConfig
{
    private readonly Dictionary<Type, Delegate> _mappers = [];

    public static ChartConfig Instance { get; } = new();

    public static bool ShowFPS { get; set; } = false;
    public static bool USE_GPU { get; set; } = false;

    public static void Configure(Action<ChartConfig> defaultSetting)
    {
        if (defaultSetting is null)
            throw new ArgumentNullException(nameof(defaultSetting));

        defaultSetting(Instance);
    }

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
