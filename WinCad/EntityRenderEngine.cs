using System.Drawing;

namespace WinCad
{
    public class EntityRenderEngine
    {
        public void Render(Graphics g, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
                g.DrawImage(image.Image, RectangleFrom(image.Box));

            foreach (var box in layer.Boxes)
                g.DrawRectangle(new Pen(box.Color), RectangleFrom(box));

            foreach (var pline in layer.Polylines)
                if (pline.Vertices.Count > 1)
                    for (int i = 1; i < pline.Vertices.Count; i++)
                        g.DrawLine(
                            new Pen(pline.Color),
                            pline.Vertices[i - 1], pline.Vertices[i]);

            foreach (var circle in layer.Circles)
            {
                // Draws circles from rectangles, convert from center & radius
                var corner = new Point(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius);

                int side = circle.Radius * 2;

                g.DrawEllipse(new Pen(circle.Color), corner.X, corner.Y, side, side);
            }
        }

        static Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(box.FirstCorner, box.Size);
        }
    }
}
