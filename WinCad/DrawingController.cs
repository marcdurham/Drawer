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

        internal void ImportPicture(string fileName)
        {
            session.OpenFileName = fileName;
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
                CancelMode();
                return;
            }

            switch(session.Mode)
            {
                case DrawModes.DrawingPolylineFirstVertex:
                    StartDrawingPolylineAt(point);
                    break;
                case DrawModes.DrawingPolylineSecondaryVertices:
                    AddPolylineVertexAt(point);
                    break;
                case DrawModes.ImportingPictureFirstCorner:
                    StartImportingPictureAt(point);
                    break;
                case DrawModes.ImportingPictureSecondCorner:
                    FinishImportingPictureAt(point);
                    break;
                case DrawModes.DrawingRectangleFirstCorner:
                    StartDrawingRectangleAt(point);
                    break;
                case DrawModes.DrawingRectangleSecondCorner:
                    FinishDrawingRectangleAt(point);
                    break;
                default:
                    view.Status = "Ready";
                    break;
            }
        }

        private void FinishDrawingRectangleAt(Point point)
        {
            session.SecondCorner = point;
            var box = new Box(
                    firstCorner: session.FirstCorner,
                    size: new Size(
                        Math.Abs(session.FirstCorner.X - session.SecondCorner.X),
                        Math.Abs(session.FirstCorner.Y - session.SecondCorner.Y)));

            box.Color = Color.Green;

            session.Canvas.CurrentLayer.Boxes.Add(box);

            session.Canvas.Highlights.Boxes.Clear();

            session.Mode = DrawModes.Ready;
            view.Status = "Ready";
            session.FirstCorner = Point.Empty;
            session.SecondCorner = Point.Empty;
        }

        private void StartDrawingRectangleAt(Point point)
        {
            session.FirstCorner = point;
            session.Mode = DrawModes.DrawingRectangleSecondCorner;
            view.Status = "Click second corner:";
        }

        private void AddPolylineVertexAt(Point point)
        {
            session.CurrentPolyline.Vertices.Add(point);
        }

        private void FinishImportingPictureAt(Point point)
        {
            session.SecondCorner = point;
            var image = new InsertedImage(
                image: Bitmap.FromFile(session.OpenFileName),
                box: new Box(
                    firstCorner: new Point(session.FirstCorner.X, session.FirstCorner.Y),
                    size: new Size(Math.Abs(session.FirstCorner.X - session.SecondCorner.X),
                    Math.Abs(session.FirstCorner.Y - session.SecondCorner.Y))));

            session.Canvas.CurrentLayer.InsertedImages.Add(image);

            session.Mode = DrawModes.Ready;
            view.Status = "Ready";
            session.FirstCorner = Point.Empty;
            session.SecondCorner = Point.Empty;
            session.Canvas.Highlights.Boxes.Clear();
        }

        private void StartImportingPictureAt(Point point)
        {
            session.FirstCorner = point;
            session.Mode = DrawModes.ImportingPictureSecondCorner;
            view.Status = "Click second corner:";
        }

        private void StartDrawingPolylineAt(Point point)
        {
            session.CurrentPolyline = new Polyline();
            session.CurrentPolyline.Vertices.Add(point);
            session.Canvas.CurrentLayer.Polylines.Add(session.CurrentPolyline);
            session.Mode = DrawModes.DrawingPolylineSecondaryVertices;
            view.Status = "Click to add vertices to the polyline:";
        }

        private void CancelMode()
        {
            session.CurrentPolyline = null;
            session.Mode = DrawModes.Ready;
            view.Status = "Ready";
        }
    }
}
