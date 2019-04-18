using System;
using System.Linq;
using SysSize = System.Drawing.Size;

namespace WinCad
{
    public class Zoomer
    {
        public double ZoomFactorForExtents(SysSize size, Canvas canvas)
        {
            if (canvas.Layers.Count == 0
                || canvas.Layers.Sum(l => l.Entities().Count()) == 0)
                return 0;

            double maxX = canvas.Layers.Max(
                l => l.Entities().Max(
                    e => e.Points().Max(
                        p => p.X)));

            double minX = canvas.Layers.Min(
                l => l.Entities().Min(
                    e => e.Points().Min(
                        p => p.X)));

            double maxY = canvas.Layers.Max(
                l => l.Entities().Max(
                    e => e.Points().Max(
                        p => p.Y)));

            double minY = canvas.Layers.Min(
                l => l.Entities().Min(
                    e => e.Points().Min(
                        p => p.Y)));

            double xDelta = maxX - minX;
            double yDelta = maxY - minY;

            double xRatio = size.Width == 0 ? 0 : xDelta / size.Width;
            double yRatio = size.Height == 0 ? 0: yDelta / size.Height;

            return Math.Min(xRatio, yRatio);
        }
    }
}
