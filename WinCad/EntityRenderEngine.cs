using System.Drawing;

namespace WinCad
{
    public class EntityRenderEngine
    {
        public void Render(Graphics graphics, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
                graphics.DrawImage(image.Image, RectangleFrom(image.Box));

            foreach (var box in layer.Boxes)
                graphics.DrawRectangle(new Pen(box.Color), RectangleFrom(box));

            foreach (var pline in layer.Polylines)
                for (int i = 1; i < pline.Vertices.Count; i++)
                    graphics.DrawLine(
                        new Pen(pline.Color) {  Width = pline.Width },
                        pline.Vertices[i - 1], pline.Vertices[i]);

            foreach (var circle in layer.Circles)
            {
                // Draws circles from rectangles, convert from center & radius
                var corner = new Point(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius);

                int side = circle.Radius * 2;

                graphics.DrawEllipse(
                    new Pen(circle.Color), 
                    corner.X, 
                    corner.Y, 
                    side, 
                    side);
            }
        }

        static Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(box.FirstCorner, box.Size);
        }
    }
}
