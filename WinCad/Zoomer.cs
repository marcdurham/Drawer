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

        public ZoomBox ZoomFactorForExtents(SysSize size, Canvas canvas)
        {
            var extents = ExtentsFrom(canvas);
            
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

            double zoomFactor = Math.Min(xRatio, yRatio);

            var offset = new SysPoint(
                x: (int)(zoomFactor * extents.UpperLeft.X),
                y: (int)(zoomFactor * extents.UpperLeft.Y));

            return new ZoomBox { ZoomFactor = zoomFactor, Offset = offset };
        }

        static (SysPoint UpperLeft, SysPoint LowerRight) 
            ExtentsFrom(Canvas canvas)
        {
            List<Point> allPoints = AllPointsFrom(canvas);

            if (allPoints.Count == 0)
            {
                return  
                (
                    UpperLeft: new SysPoint(0, 0),
                    LowerRight: new SysPoint(0, 0)
                );
            }

            var maxX = (int)allPoints.Max(p => p.X);
            var minX = (int)allPoints.Min(p => p.X);
            var maxY = (int)allPoints.Max(p => p.Y);
            var minY = (int)allPoints.Min(p => p.Y);

            return
            (
                UpperLeft:  new SysPoint(minX, minY),
                LowerRight: new SysPoint(maxX, maxY)
            );
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
    }

    public class ZoomBox
    {
        public double ZoomFactor { get; set; }
        public SysPoint Offset { get; set; } = new SysPoint();
    }
}
