using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Primitive;
public readonly struct Coordinate(double x, double y)
{
    public double X { get; } = x;
    public double Y { get; } = y;


    public static Coordinate operator +(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.X + b.X, a.Y + b.Y);
    }

    public static Coordinate operator -(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.X - b.X, a.Y - b.Y);
    }

    public static Coordinate operator *(Coordinate a, double mul)
    {
        return new Coordinate(a.X * mul, a.Y * mul);
    }

    public static Coordinate operator /(Coordinate a, double div)
    {
        return new Coordinate(a.X / div, a.Y / div);
    }
}
