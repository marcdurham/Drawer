using SkiaSharp;
using System;

namespace DumbCad.Entities
{
    public class PolylineView 
    {
        public Polyline Polyline { get; set; }
        public SKPath Path { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public bool IsNear(SKPoint m, float near = 5f)
        {
            //if(path.Bounds.Contains(worldPoint.X, worldPoint.Y))
            //{
            for (int i = 0; i < Path.Points.Length; i += 2)
            {
                if (IsNear(m, Path.Points[i], Path.Points[i + 1], near))
                {
                    return true;
                }
            }
            //}
            return false;
        }

        bool IsNear(SKPoint m, SKPoint s, SKPoint e, float near)
        {
            double a = Geometry.Distance(m, s);
            double b = Geometry.Distance(m, e);
            if (a <= near || b <= near)
            {
                return true;
            }

            double c = Geometry.Distance(s, e);
            double cosB = (Math.Pow(a, 2) + Math.Pow(c, 2) - Math.Pow(b, 2)) / (2 * a * c);
            double B = Math.Acos(cosB);

            double cosA = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);
            double A = Math.Acos(cosA);

            double rightAngle = Math.PI / 2;
            if (A <= 0 || A > rightAngle || B <= 0 || B > rightAngle)
            {
                return false;
            }

            double dist = a * Math.Sin(B);

            if (dist <= near)
            {
                return true;
            }

            return false;
        }
    }
}
