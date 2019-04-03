using System;
using System.Drawing;

namespace WinCad
{
    public class DrawingController
    {
        readonly IDrawingView view;

        public DrawingController(IDrawingView view)
        {
            this.view = view;
            view.Canvas = session.Canvas;
        }

        public DrawingSession session { get; set; } = new DrawingSession();

        internal void DrawPolyline()
        {
            session.Mode = DrawModes.DrawingPolylineFirstVertex;
            view.Status = "Click first point of polyline:";
        }

        internal void ImportPicture()
        {
            session.Mode = DrawModes.ImportingPictureFirstCorner;
            view.Status = "Click first corner:";
        }

        internal void DrawRectangle()
        {
            session.Mode = DrawModes.DrawingRectangleFirstCorner;
            view.Status = "Click first corner:";
        }


        internal void ClickAt(Point point, bool cancel)
        {
            if (cancel)
            {
                session.Mode = DrawModes.Ready;
                view.Status = "Ready";
            }
            else if (session.Mode == DrawModes.DrawingPolylineFirstVertex)
            {
                session.CurrentPolyline = new Polyline();
                session.CurrentPolyline.Points.Add(point);
                session.Canvas.Polylines.Add(session.CurrentPolyline);
                session.Mode = DrawModes.DrawingPolylineSecondaryVertices;
                view.Status = "Click to add vertices to the polyline:";
            }
            else if (session.Mode == DrawModes.DrawingPolylineSecondaryVertices)
            {
                session.CurrentPolyline.Points.Add(point);
            }
            else if (session.Mode == DrawModes.ImportingPictureFirstCorner)
            {
                session.FirstCorner = point;
                session.Mode = DrawModes.ImportingPictureSecondCorner;
                view.Status = "Click second corner:";
            }
            else if (session.Mode == DrawModes.ImportingPictureSecondCorner)
            {
                session.SecondCorner = point;
                var image = new InsertedImage(
                    image: Bitmap.FromFile(@"C:\Store\Garage.TIF"),
                    rectangle: new Rectangle(
                    session.FirstCorner.X,
                    session.FirstCorner.Y,
                    Math.Abs(session.FirstCorner.X - session.SecondCorner.X),
                    Math.Abs(session.FirstCorner.Y - session.SecondCorner.Y)));

                session.Canvas.InsertedImages.Add(image);

                session.Mode = DrawModes.Ready;
                view.Status = "Ready";
                session.FirstCorner = Point.Empty;
                session.SecondCorner = Point.Empty;
            }
            else if (session.Mode == DrawModes.DrawingRectangleFirstCorner)
            {
                session.FirstCorner = point;
                session.Mode = DrawModes.DrawingRectangleSecondCorner;
                view.Status = "Click second corner:";
            }
            else if (session.Mode == DrawModes.DrawingRectangleSecondCorner)
            {
                session.SecondCorner = point;
                session.Canvas.Rectangles.Add(new Rectangle(
                    session.FirstCorner.X,
                    session.FirstCorner.Y,
                    Math.Abs(session.FirstCorner.X - session.SecondCorner.X),
                    Math.Abs(session.FirstCorner.Y - session.SecondCorner.Y)));

                session.Mode = DrawModes.Ready;
                view.Status = "Ready";
                session.FirstCorner = Point.Empty;
                session.SecondCorner = Point.Empty;
            }
            else
            {
                view.Status = "Clicked";
            }
        }
    }
}
