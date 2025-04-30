using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.Kernel.Measuring;
public class Scaler
{
    private readonly float _minPx, _maxPx, _deltaPx;
    private readonly double _minVal, _maxVal, _deltaVal;

    private readonly double _valPerPx, _pxPerVal;

    public Scaler(CoreCartesianAxis axis, Rect dataRect)
    {
        if (axis.Orientation == AxisOrientation.Unknown)
            throw new Exception("The axis is not ready to be scaled.");

        if (axis.Orientation == AxisOrientation.X)
        {
            _minPx = dataRect.X;
            _maxPx = dataRect.X + dataRect.Width;
        }
        else
        {
            _minPx = dataRect.X;
            _maxPx = dataRect.X + dataRect.Width;
        }

        _deltaPx = _maxPx - _minPx;

        _minVal = axis.Min;
        _maxVal = axis.Max;
        _deltaVal = _maxVal - _minVal;

        _valPerPx = _deltaVal / _deltaPx;
        _pxPerVal = _deltaPx / _deltaVal;

        if (double.IsNaN(_valPerPx) && double.IsInfinity(_valPerPx))
        {
            _valPerPx = 0d;
            _pxPerVal = 0d;
        }
    }


    public float ToPixel(double value)
    {
        return unchecked(_minPx + (float)((value - _minVal) * _pxPerVal));
    }

    public double ToValue(float pixel)
    {
        return _minVal + (pixel - _minPx) * _valPerPx;
    }
}
