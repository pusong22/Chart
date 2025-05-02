using Core.Kernel.Axis;
using Core.Primitive;

namespace Core.Kernel.Measuring;
public class Scaler
{
    private readonly float _minPx, _maxPx, _deltaPx;
    private readonly double _minVal, _maxVal, _deltaVal;

    private readonly double _valPerPx, _pxPerVal;

    private readonly AxisOrientation _orientation;

    public Scaler(CoreCartesianAxis axis, Point p, Size s)
        : this(axis, new Rect(p, s)) { }


    public Scaler(CoreCartesianAxis axis, Rect dataRect)
    {
        if (axis.Orientation == AxisOrientation.Unknown)
            throw new Exception("The axis is not ready to be scaled.");

        _orientation = axis.Orientation;

        if (axis.Orientation == AxisOrientation.X)
        {
            _minPx = dataRect.X;
            _maxPx = dataRect.X + dataRect.Width;
        }
        else
        {
            _minPx = dataRect.Y;
            _maxPx = dataRect.Y + dataRect.Height;
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

    public float MeasureInPixels(double value)
    {
        unchecked
        {
            return Math.Abs(
                _orientation == AxisOrientation.X
                    ? (float)(_minPx + (value - _minVal) * _pxPerVal - (_minPx + (0 - _minVal) * _pxPerVal))
                    : (float)(_minPx + (0 - _minVal) * _pxPerVal - (_minPx + (value - _minVal) * _pxPerVal)));
        }
    }


    public float ToPixel(double value)
    {
        return _orientation == AxisOrientation.X
            ? unchecked(_minPx + (float)((value - _minVal) * _pxPerVal))
            : unchecked(_maxPx - (float)((value - _minVal) * _pxPerVal));
    }

    public double ToValue(float pixel)
    {
        return _orientation == AxisOrientation.X
            ? _minVal + (pixel - _minPx) * _valPerPx
            : _maxVal - (pixel - _minPx) * _valPerPx;
    }
}
