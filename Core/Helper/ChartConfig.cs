using Core.Kernel.Axis;

namespace Core.Helper;
public class ChartConfig
{
    private Provider? _provider;

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

    public List<Action<CoreAxis>> AxisBuilder { get; set; } = [];

    public void ApplyStyleToAxis(CoreAxis axis)
    {
        foreach (var action in AxisBuilder)
            action(axis);
    }
}
