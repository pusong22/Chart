using Core.Kernel.Motion;

namespace Core;
public static class Extensions
{
    public static void Animate(this Animatable animatable, Func<float, float>? easingFunction, TimeSpan speed, params string[]? properties) =>
        Animate(animatable, new Animation(easingFunction, speed), properties);

    public static void Animate(this Animatable animatable, Animation animation, params string[]? properties)
    {
        animatable.SetMotion(animation, properties);
    }
}
