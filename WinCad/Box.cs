using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Box : Entity
    {
        public Box()
        {
            Initialize();
        }

        public Box(Point firstCorner, Size size)
        {
            Initialize();
            FirstCorner = firstCorner;
            Size = size;
        }

        void Initialize()
        {
            Color = Color.Purple;
        }

        public Point FirstCorner { get; set; }
        public Size Size { get; set; }

        public override List<Point> Points()
        {
            var points = new List<Point>();

            points.Add(FirstCorner);

            points.Add(
                new Point(
                    FirstCorner.X + Size.Width,
                    FirstCorner.Y));

            points.Add(
                new Point(
                    FirstCorner.X + Size.Width,
                    FirstCorner.Y + Size.Height));

            points.Add(
                new Point(
                    FirstCorner.X,
                    FirstCorner.Y + Size.Height));

            return points;
        }
    }
}
