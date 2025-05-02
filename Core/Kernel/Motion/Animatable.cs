namespace Core.Kernel.Motion;

public abstract class Animatable
{
    public bool Remove { get; protected internal set; }

    public bool IsCompleted
    {
        get
        {
            foreach (var property in Motions.Keys)
            {
                if (!Motions.TryGetValue(property, out var motionProperty))
                {
                    throw new Exception(
                        $"The property '{property}' is not a transition property of this instance.");
                }

                if (motionProperty.Animation is null)
                    continue;

                if (!motionProperty.IsCompleted)
                    return false; // 有未完成动画，直接返回 false
            }

            return true; // 所有动画都完成了
        }
    }

    public long CurrentTime { get; protected internal set; } = long.MaxValue;

    public Dictionary<string, IMotionProperty> Motions { get; protected internal set; } = [];

    public void SetMotion(Animation? animation, params string[]? propertyName)
    {
        var a = animation?.Duration == 0 ? null : animation;
        if (propertyName is null || propertyName.Length == 0)
            propertyName = [.. Motions.Keys];

        foreach (var name in propertyName)
            Motions[name].Animation = a;
    }


    public void RemoveMotion(params string[]? propertyName)
    {
        if (propertyName is null || propertyName.Length == 0)
            propertyName = [.. Motions.Keys];

        foreach (var name in propertyName)
        {
            Motions[name].Animation = null;
        }
    }

    protected T RegisterMotionProperty<T>(T motionProperty) where T : IMotionProperty
    {
        Motions[motionProperty.PropertyName] = motionProperty;
        return motionProperty;
    }
}
