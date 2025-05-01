namespace Core.Kernel.Motion;

public class Animation(Func<float, float>? func, TimeSpan ts)
{
    public Func<float, float>? EasingFunction { get; set; } = func;
    public long Duration { get; set; } = (long)ts.TotalMilliseconds;
}
