﻿using System.Drawing;
using SysPoint = System.Drawing.Point;
using SysSize = System.Drawing.Size;

namespace WinCad
{
    public class EntityRenderEngine
    {
        DrawingSession session;

        public EntityRenderEngine(DrawingSession session)
        {
            this.session = session;
        }

        public void Render(Graphics graphics, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
                graphics.DrawImage(image.Image, RectangleFrom(image.Box));

            foreach (var box in layer.Boxes)
                graphics.DrawRectangle(new Pen(box.Color), RectangleFrom(box));

            foreach (var pline in layer.Polylines)
                for (int i = 1; i < pline.Vertices.Count; i++)
                    graphics.DrawLine(
                        new Pen(pline.Color) { Width = pline.Width },
                        SysPointFrom(pline.Vertices[i - 1]),
                        SysPointFrom(pline.Vertices[i]));

            foreach (var circle in layer.Circles)
            {
                // Draws circles from rectangles, convert from center & radius
                var corner = new Point(
                        circle.Center.X - circle.Radius,
                        circle.Center.Y - circle.Radius);

                int side = circle.Radius * 2;

                graphics.DrawEllipse(
                    new Pen(circle.Color), 
                    (float)corner.X, 
                    (float)corner.Y, 
                    side, 
                    side);
            }
        }

        /// <summary>
        /// SysPoint is an alias for System.Drawing.Point
        /// </summary>
        public SysPoint SysPointFrom(Point point)
        {
            return new SysPoint(
                x: session.PanningOffset.X + (int)(point.X * session.ZoomFactor), 
                y: session.PanningOffset.Y + (int)(point.Y * session.ZoomFactor));
        }

        public Point PointFrom(SysPoint point)
        {
            return new Point(point.X, point.Y);
        }

        public SysSize SysSizeFrom(Size size)
        {
          
            return new SysSize(
                width: (int)(size.Width * session.ZoomFactor),
                height: (int)(size.Height * session.ZoomFactor));
        }

        public Size SizeFrom(SysSize size)
        {
            return new Size(size.Width, size.Height);
        }

        Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(
                location: SysPointFrom(box.FirstCorner), 
                size: SysSizeFrom(box.Size));
        }
    }
}
