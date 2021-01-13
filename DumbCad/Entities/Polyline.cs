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

        public override List<Point> Points()
        {
            return Vertices;
        }
    }
}
