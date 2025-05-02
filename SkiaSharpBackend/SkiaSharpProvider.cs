using Core.Helper;
using Core.Kernel.Axis;
using SkiaSharpBackend.Drawing;

namespace SkiaSharpBackend;
public class SkiaSharpProvider : Provider
{
    public override CoreAxis GetAxis()
    {
        return new Axis();
    }
}
