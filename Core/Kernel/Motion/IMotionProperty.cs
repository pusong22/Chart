namespace Core.Kernel.Motion;
public abstract class IMotionProperty(string propertyName)
{
    public bool IsCompleted { get; protected set; }

    public string PropertyName { get; } = propertyName;

    public Animation? Animation { get; set; }
}

public abstract class IMotionProperty<T>(
    string propertyName,
    T fromValue,
    T toValue)
    : IMotionProperty(propertyName)
{
    private long _startTime = -1;

    public T FromValue { get; private set; } = fromValue;
    public T ToValue { get; private set; } = toValue;


    public void Set(T value, Animatable animatable)
    {
        FromValue = Get(animatable);
        ToValue = value;

        if (Animation is not null && Animation.Duration > 0)
        {
            _startTime = -1; // 标记为未初始化
            IsCompleted = false;
        }
        else
        {
            // 没有动画，立即完成
            _startTime = 0;
            IsCompleted = true;
        }
    }

    public T Get(Animatable animatable)
    {
        if (Animation?.EasingFunction is null
            || Animation.Duration <= 0
            || IsCompleted)
        {
            return Interpolate(1f);
        }

        // 
        if (_startTime < 0)
        {
            _startTime = animatable.CurrentTime;
        }

        var elapsed = animatable.CurrentTime - _startTime;
        var duration = (float)(Animation.Duration);

        float progress = elapsed / duration;
        if (progress >= 1f)
        {
            IsCompleted = true;
            return Interpolate(1f);
        }

        float eased = Animation.EasingFunction(progress);
        return Interpolate(eased);
    }

    protected abstract T Interpolate(float progress);
}
