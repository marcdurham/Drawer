using System;
using System.Collections.Generic;
using System.Linq;
using SysSize = System.Drawing.Size;
using SysPoint = System.Drawing.Point;

namespace WinCad
{
    public class Zoomer
    {
        readonly int padding = 0;

        public Zoomer(int padding)
        {
            this.padding = padding;
        }

        public ZoomBox ZoomExtents(SysSize size, Canvas canvas)
        {
            var extents = ExtentsFrom(canvas);
            
            var paddedSize = new SysSize(
                width: size.Width - padding,
                height: size.Height - padding);

            double xRatio = paddedSize.Width == 0
                ? 0
                : paddedSize.Width / (double)extents.Width;

            double yRatio = paddedSize.Height == 0
                ? 0
                : paddedSize.Height / (double)extents.Height;

            double zoomFactor = Math.Min(xRatio, yRatio);

            var offset = new SysPoint(
                x: -(int)(zoomFactor * extents.UpperLeft.X),
                y: -(int)(zoomFactor * extents.UpperLeft.Y));

            var zoomOffset = ZoomOffset(
                zoomFactor,
                size);

            var panningOffset = new SysPoint(
                x: offset.X-zoomOffset.X,
                y: offset.Y-zoomOffset.Y);

            return new ZoomBox
            {
                ZoomFactor = zoomFactor,
                PanningOffset = panningOffset,
                ZoomOffset = zoomOffset
            };
        }

        SysPoint ZoomOffset(double zoomFactor, System.Drawing.Size size)
        {
            var w = ((double)size.Width / 2)
                - ((double)size.Width * zoomFactor / 2);
            var h = ((double)size.Height / 2)
                - ((double)size.Height * zoomFactor / 2);

            return new SysPoint((int)w, (int)h);
        }

        static (SysPoint UpperLeft, SysPoint LowerRight, int Width, int Height) 
            ExtentsFrom(Canvas canvas)
        {
            List<Point> allPoints = AllPointsFrom(canvas);

            if (allPoints.Count == 0)
            {
                return  
                (
                    UpperLeft: new SysPoint(0, 0),
                    LowerRight: new SysPoint(0, 0),
                    Width: 0,
                    Height: 0
                );
            }

            var maxX = (int)allPoints.Max(p => p.X);
            var minX = (int)allPoints.Min(p => p.X);
            var maxY = (int)allPoints.Max(p => p.Y);
            var minY = (int)allPoints.Min(p => p.Y);

            var upperLeft = new SysPoint(minX, minY);
            var lowerRight = new SysPoint(maxX, maxY);

            return
            (
                UpperLeft: upperLeft,
                LowerRight: lowerRight,
                Width: lowerRight.X - upperLeft.X,
                Height: lowerRight.Y - upperLeft.Y
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
        public SysPoint PanningOffset { get; set; } = new SysPoint();
        public SysPoint ZoomOffset { get; set; } = new SysPoint();
    }
}
