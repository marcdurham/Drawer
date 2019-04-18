using System;
using System.Collections.Generic;
using System.Linq;
using SysSize = System.Drawing.Size;
using SysPoint = System.Drawing.Point;

namespace WinCad
{
    public class Zoomer
    {
        readonly int padding = 30;

        public Zoomer(int padding)
        {
            this.padding = padding;
        }

        public double ZoomFactorForExtents(SysSize size, Canvas canvas)
        {
            Extents extents = ExtentsFrom(canvas);

            double xDelta = extents.LowerRight.X - extents.UpperLeft.X;
            double yDelta = extents.LowerRight.Y - extents.UpperLeft.Y;

            var paddedSize = new SysSize(
                width: size.Width - padding,
                height: size.Height - padding);

            double xRatio = paddedSize.Width == 0
                ? 0
                : paddedSize.Width / xDelta;

            double yRatio = paddedSize.Height == 0
                ? 0
                : paddedSize.Height / yDelta;

            return Math.Min(xRatio, yRatio);
        }

        static Extents ExtentsFrom(Canvas canvas)
        {
            List<Point> allPoints = AllPointsFrom(canvas);

            if (allPoints.Count == 0)
            {
                return new Extents();
            }

            var maxX = (int)allPoints.Max(p => p.X);
            var minX = (int)allPoints.Min(p => p.X);
            var maxY = (int)allPoints.Max(p => p.Y);
            var minY = (int)allPoints.Min(p => p.Y);

            return new Extents()
            {
                UpperLeft = new SysPoint(minX, minY),
                LowerRight = new SysPoint(maxX, maxY)
            };
        }

        static List<Point> AllPointsFrom(Canvas canvas)
        {
            var allPoints = new List<Point>();

            foreach (var l in canvas.Layers)
            {
                foreach (var e in l.Entities())
                {
                    allPoints.AddRange(e.Points());
                }
            }

            return allPoints;
        }

        class Extents
        {
            public SysPoint UpperLeft { get; set; }
            public SysPoint LowerRight { get; set; }
        }
    }
}
