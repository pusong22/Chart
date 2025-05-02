namespace Core.Kernel.Motion;

public class Animation(Func<float, float>? func, TimeSpan ts)
{
    public Func<float, float>? EasingFunction { get; protected internal set; } = func;
    public long Duration { get; protected internal set; } = (long)ts.TotalMilliseconds;
}
