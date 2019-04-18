﻿using System;
using System.Collections.Generic;
using System.Linq;
using SysSize = System.Drawing.Size;

namespace WinCad
{
    public class Zoomer
    {
        const int Padding = 30;

        public double ZoomFactorForExtents(SysSize size, Canvas canvas)
        {
            var allPoints = new List<Point>();

            foreach (var l in canvas.Layers)
            {
                foreach (var e in l.Entities())
                {
                    allPoints.AddRange(e.Points());
                }
            }

            if (allPoints.Count == 0)
            {
                return 0;
            }

            var paddedSize = new SysSize(
              width: size.Width - Padding,
              height: size.Height - Padding);

            double maxX = allPoints.Max(p => p.X);
            double minX = allPoints.Min(p => p.X);
            double maxY = allPoints.Max(p => p.Y);
            double minY = allPoints.Min(p => p.Y);

            double xDelta = maxX - minX;
            double yDelta = maxY - minY;

            double xRatio = paddedSize.Width == 0 
                ? 0 
                : paddedSize.Width / xDelta;

            double yRatio = paddedSize.Height == 0 
                ? 0 
                : paddedSize.Height / yDelta;

            return Math.Min(xRatio, yRatio);
        }
    }
}
