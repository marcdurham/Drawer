using System.Drawing;
using SysPoint = System.Drawing.Point;

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

        public SysPoint SysPointFrom(Point point)
        {
            return new SysPoint((int)point.X, (int)point.Y);
        }

        public Point PointFrom(SysPoint point)
        {
            return new Point(point.X, point.Y);
        }

        Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(SysPointFrom(box.FirstCorner), box.Size);
        }
    }
}
