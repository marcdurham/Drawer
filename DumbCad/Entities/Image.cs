using System.Collections.Generic;

namespace DumbCad.Entities
{
    public class Image : Entity
    {
        public Point Location { get; set; }
        public double Height { get; set; }
        public string FilePath { get; set; }

        public override List<Point> Points()
        {
            return new List<Point>();
        }

        public bool IsNear(Point point, double near = 5.0)
        {
            double newLeft = Location.X - near;
            double newRight = Location.X + Width + near;
            double newTop = Location.Y + near;
            double newBottom = Location.Y - Height - near;

            return point.X >= newLeft
                && point.X <= newRight
                && point.Y >= newBottom
                && point.Y <= newTop;
        }
    }
}
