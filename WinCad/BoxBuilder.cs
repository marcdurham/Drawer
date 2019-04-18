using System;

namespace WinCad
{
    public class BoxBuilder
    {
        public Polyline Draw(Point corner, Point opposite)
        {
            var line = new Polyline();
            if (corner == null || opposite == null)
                return line;

            var upperLeft = new Point(
                x: Math.Min(corner.X, opposite.X),
                y: Math.Min(corner.Y, opposite.Y));

            var upperRight = new Point(
                x: Math.Max(corner.X, opposite.X),
                y: Math.Min(corner.Y, opposite.Y));

            var lowerRight = new Point(
                x: Math.Max(corner.X, opposite.X),
                y: Math.Max(corner.Y, opposite.Y));

            var lowerLeft = new Point(
                x: Math.Min(corner.X, opposite.X),
                y: Math.Max(corner.Y, opposite.Y));

            line.Vertices.Add(upperLeft);
            line.Vertices.Add(upperRight);
            line.Vertices.Add(lowerRight);
            line.Vertices.Add(lowerLeft);
            line.Vertices.Add(upperLeft);

            return line;
        }
    }
}
