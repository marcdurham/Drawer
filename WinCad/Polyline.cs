using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Polyline : Entity
    {
        public Polyline(Point first, Point second)
        {
            Color = Color.Green;

            Vertices.Add(first);
            Vertices.Add(second);
        }

        public List<Point> Vertices { get; } = new List<Point>();

        public override List<Point> Points()
        {
            return Vertices;
        }
    }
}
