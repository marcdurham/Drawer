using System.Collections.Generic;
using System.Drawing;

namespace WinCad
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
        public float Width { get; set; } = 3.0f;

        public override List<Point> Points()
        {
            return Vertices;
        }
    }
}
