using System.Collections.Generic;

namespace DumbCad.Entities
{
    public class Image : Entity
    {
        public Point Location { get; set; }
        public double Scale { get; set; } = 1.0;
        public string FilePath { get; set; }

        public override List<Point> Points()
        {
            return new List<Point>();
        }
    }
}
