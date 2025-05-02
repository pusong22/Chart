using Core.Primitive;

namespace Core.Kernel.Painting;
public class DashEffectSetting : PathEffectSetting
{
    public DashEffectSetting(float[] intervals, float phase = 0)
    {
        EffectType = PathEffectType.Dash;
        Intervals = intervals;
        Phase = phase;
    }

    public float[] Intervals { get; }
    public float Phase { get; }
}
