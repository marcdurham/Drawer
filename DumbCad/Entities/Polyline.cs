using System;
using System.Collections.Generic;

namespace DumbCad.Entities
{
    public class Polyline : Entity
    {
        public Polyline()
        {
        }

        public Polyline(Point first, Point second)
        {
            Color = Color.Red;

            Vertices.Add(first);
            Vertices.Add(second);
        }

        public List<Point> Vertices { get; } = new List<Point>();
        public bool IsHovered { get; internal set; }

        public override List<Point> Points()
        {
            return Vertices;
        }


        public bool IsNear(Point m, float near = 5f)
        {
            //if(path.Bounds.Contains(worldPoint.X, worldPoint.Y))
            //{
            for (int i = 1; i < Vertices.Count; i++)
            {
                if (IsNear(m, Vertices[i-1], Vertices[i], near))
                {
                    return true;
                }
            }
            //}
            return false;
        }

        bool IsNear(Point m, Point s, Point e, float near)
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
