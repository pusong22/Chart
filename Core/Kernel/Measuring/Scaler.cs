namespace Core.Kernel.Measuring;
public class Scaler
{
    private readonly float _minPx, _maxPx;
    private readonly double _minVal, _maxVal;

    private readonly double _valPerPx, _pxPerVal;

    private readonly bool _flip;

    public Scaler(bool flip, float start, float end, double min, double max)
    {
        _flip = flip;

        _minPx = start;
        _maxPx = end;

        _minVal = min;
        _maxVal = max;

        var deltaPx = _maxPx - _minPx;
        var deltaVal = _maxVal - _minVal;

        _valPerPx = deltaVal / deltaPx;
        _pxPerVal = deltaPx / deltaVal;
    }

    public float MeasureInPixels(double value)
    {
        return (float)Math.Abs(value * _pxPerVal);
    }


    public float ToPixel(double value)
    {
        return !_flip
            ? unchecked(_minPx + (float)((value - _minVal) * _pxPerVal))
            : unchecked(_maxPx - (float)((value - _minVal) * _pxPerVal));
    }

    public double ToValue(float pixel)
    {
        return !_flip
            ? _minVal + (pixel - _minPx) * _valPerPx
            : _maxVal - (pixel - _minPx) * _valPerPx;
    }
}
