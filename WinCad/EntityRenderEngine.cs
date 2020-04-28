using System;
using System.Drawing;
//using SysPoint = System.Drawing.Point;
//using SysSize = System.Drawing.Size;

namespace WinCad
{
    public class EntityRenderEngine
    {
        readonly DrawingSession session;

        public EntityRenderEngine(DrawingSession session)
        {
            this.session = session;
        }

        public void Render(Graphics graphics, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
            {
                RenderInsertedImage(graphics, image);
            }

            foreach (var box in layer.Boxes)
            {
                graphics.DrawRectangle(new Pen(box.Color), RectangleFrom(box));
            }

            foreach (var pline in layer.Polylines)
            {
                RenderPolyline(graphics, pline);
            }

            foreach (var box in layer.TextBoxes)
            {
                RenderTextBox(graphics, box);
            }

            foreach (var grip in layer.Grips)
            {
                RenderGrip(graphics, grip);
            }
        }

        /// <summary>
        /// SysPoint is an alias for System.Drawing.Point
        /// </summary>
        public Pixel PixelFrom(Point point)
        {
            return new Pixel(
                x: session.PanningOffset.X 
                    + session.ZoomOffset.X
                    + (int)(point.X * session.ZoomFactor), 
                y: session.PanningOffset.Y
                    + session.ZoomOffset.Y
                    + (int)(point.Y * session.ZoomFactor));
        }

        public Point PointFrom(Pixel pixel)
        {
            return new Point(
                x: (pixel.X - session.PanningOffset.X - session.ZoomOffset.X) 
                    / session.ZoomFactor, 
                y: (pixel.Y - session.PanningOffset.Y - session.ZoomOffset.Y) 
                    / session.ZoomFactor);
        }

        public Size PixelSizeFrom(Size size)
        {
            return new Size( 
                width: (int)(size.Width * session.ZoomFactor),
                height: (int)(size.Height * session.ZoomFactor));
        }

        public Size SizeFrom(Size size)
        {
            return new Size(
                width: (int)(size.Width / session.ZoomFactor), 
                height: (int)(size.Height / session.ZoomFactor));
        }

        static Image FromFile(string file)
        {
            try
            {
                return Bitmap.FromFile(file);
            }
            catch (Exception)
            {
                return new Bitmap(10, 10);
            }
        }

        void RenderInsertedImage(Graphics graphics, InsertedImage image)
        {
            graphics.DrawRectangle(
                pen: new Pen(Color.Magenta),
                rect: RectangleFrom(image.Box));

            graphics.DrawImage(
                image: FromFile(image.File),
                rect: RectangleFrom(image.Box));
        }

        void RenderTextBox(Graphics graphics, TextBox box)
        {
            graphics.DrawString(
                s: box.Text, 
                font: new Font("Arial", 10.0f),
                brush: Brushes.Black, 
                x: (float)box.Location.X, 
                y: (float)box.Location.Y);
        }

        void RenderGrip(Graphics graphics, Grip grip)
        {
            var corner = PixelFrom(
                new Point(
                    x: grip.Center.X,
                    y: grip.Center.Y));

            var sysCorner = new Pixel(
                x: corner.X - grip.Radius,
                y: corner.Y - grip.Radius);

            graphics.DrawRectangle(
                new Pen(grip.Color),
                new Rectangle(
                    location: sysCorner,
                    size: new Size(grip.Radius * 2, grip.Radius * 2)));
        }

        void RenderPolyline(Graphics graphics, Polyline pline)
        {
            for (int i = 1; i < pline.Vertices.Count; i++)
            {
                graphics.DrawLine(
                    new Pen(pline.Color) { Width = pline.Width },
                    PixelFrom(pline.Vertices[i - 1]),
                    PixelFrom(pline.Vertices[i]));
            }
        }

        Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(
                location: PixelFrom(box.FirstCorner), 
                size: PixelSizeFrom(box.Size));
        }
    }
}
