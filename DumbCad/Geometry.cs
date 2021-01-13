using SkiaSharp;
using System;

namespace DumbCad
{
    public class Geometry
    {
        public static double Distance(SKPoint a, SKPoint b)
        {
            return (double)Math.Sqrt(
                Math.Pow(a.X - b.X, 2) +
                Math.Pow(a.Y - b.Y, 2));
        }
    }
}
