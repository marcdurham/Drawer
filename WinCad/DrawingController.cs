using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WinCad
{
    public class DrawingController
    {
        static readonly int NearDistance = 5;
        static readonly int HighlightRadius = 5;

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
            view.Status = Properties.Resources.DrawPolylineStatus;
            view.RenderLayers();
        }

        internal void ImportPicture(string fileName)
        {
            session.OpenFileName = fileName;
            session.Mode = DrawModes.ImportingPictureFirstCorner;
            view.Status = Properties.Resources.ImportPictureStatus;
        }

        internal void DrawRectangle()
        {
            session.Mode = DrawModes.DrawingRectangleFirstCorner;
            view.Status = Properties.Resources.DrawRectangleStatus;
        }

        internal void SelectEntity()
        {
            session.Mode = DrawModes.SelectEntity;
            view.Status = Properties.Resources.SelectEntityStatus;
        }

        internal void DeleteSelectedEntities()
        {
            var trash = new List<Entity>();
            foreach (var entity in session.Canvas.Entities())
            {
                if (entity.IsSelected)
                    trash.Add(entity);
            }

            foreach (var entity in trash)
                session.Canvas.Delete(entity);

            ShowSelections();

            view.RenderLayers();
        }

        internal void ClickAt(Point point, bool cancel)
        {
            if (cancel) // TODO: detect mode switching
            {
                if (session.Mode == DrawModes.DrawingPolylineExtraVertices)
                {
                    session.Mode = DrawModes.DrawingPolylineFirstVertex;
                    view.SecondStatus = string.Empty;
                    session.CurrentPolyline = null;
                    session.Canvas.Highlights.Polylines.Clear();
                }
                else
                    CancelMode();
                return;
            }

            switch(session.Mode)
            {
                case DrawModes.DrawingPolylineFirstVertex:
                    // TODO: This is the only method here that does not call RenderLayers()
                    StartDrawingPolylineAt(point); 
                    break;
                case DrawModes.DrawingPolylineSecondVertex:
                    AddSecondPolylineVertexAt(point);
                    break;
                case DrawModes.DrawingPolylineExtraVertices:
                    AddExtraPolylineVertexAt(point);
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
                case DrawModes.StartInsertingBlock:
                    InsertBlockAt(point);
                    break;
                case DrawModes.SelectEntity:
                    SelectEntityAt(point);
                    break;
                default:
                    CancelMode();
                    break;
            }
        }

        internal void HoverAt(Point location)
        {
            if (session.Mode == DrawModes.DrawingPolylineExtraVertices
                || session.Mode == DrawModes.DrawingPolylineSecondVertex)
            {
                ShowNewPolylineSegment(location);
            }

            if (session.Mode == DrawModes.DrawingRectangleSecondCorner
                || session.Mode == DrawModes.ImportingPictureSecondCorner)
            {
                ShowNewRectangle(location);
            }

            HoverOverPointsAt(location);

            view.RefreshImage();
        }

        internal void InsertBlock()
        {
            session.Mode = DrawModes.StartInsertingBlock;
            view.Status = "Inserting block: Click insertion point:";
        }

        internal void ShowSelections()
        {
            int radius = 5;
            session.Canvas.Selections.Circles.Clear();

            foreach (var layer in session.Canvas.Layers)
                foreach (var entity in layer.Entities())
                    if (entity.IsSelected)
                        foreach (var point in entity.Points())
                        {
                            session.Canvas.Selections.Circles.Add(
                                new Circle()
                                {
                                    Center = point,
                                    Color = Color.Magenta,
                                    Radius = radius,
                                });
                        }
        }

        void ShowNewPolylineSegment(Point point)
        {
            session.Canvas.NewLineStart = session
                .CurrentPolyline?
                .Vertices
                .Last() ?? session.FirstCorner;

            int angle = Angle(session.Canvas.NewLineStart, point);
            view.SecondStatus = $"{session.Canvas.NewLineStart.X}, {session.Canvas.NewLineStart.Y} -> {point.X}, {point.Y} = {angle}deg";

            session.Canvas.NewLineEnd = OrthoPointFrom(session.Canvas.NewLineStart, point);

            var rubberband = new Polyline(
                session.Canvas.NewLineStart,
                session.Canvas.NewLineEnd)
            {
                Color = Color.Blue,
            };

            session.Canvas.Highlights.Polylines.Clear();
            session.Canvas.Highlights.Polylines.Add(rubberband);
        }

        Point OrthoPointFrom(Point previousPoint, Point point)
        {
            if (!view.OrthoIsOn)
                return point;

            int angle = Angle(previousPoint, point);

            if (Math.Abs(angle) >= 0 && Math.Abs(angle) < 45
                || Math.Abs(angle) <= 180 && Math.Abs(angle) > 135)
                return new Point(point.X, previousPoint.Y);
            else
                return new Point(previousPoint.X, point.Y);
        }

        void ShowNewRectangle(Point location)
        {
            var size = SizeFrom(session.FirstCorner, location);

            var box = new Box(session.FirstCorner, size)
            {
                Color = Color.Blue
            };

            session.Canvas.Highlights.Boxes.Clear();
            session.Canvas.Highlights.Boxes.Add(box);
        }

        void HoverOverPointsAt(Point cursor)
        {
            var circle = new Circle()
            {
                Radius = HighlightRadius,
                Color = Color.Blue
            };

            bool nearSomething = false;
            foreach (var layer in session.Canvas.Layers)
            {
                foreach (var entity in layer.Entities())
                {
                    foreach (var point in entity.Points())
                    {
                        if (AreNear(point, cursor))
                        {
                            circle.Center = point;

                            session.Canvas.Highlights.Circles.Clear();
                            session.Canvas.Highlights.Circles.Add(circle);
                            nearSomething = true;
                        }
                    }
                }
            }

            if (!nearSomething)
                session.Canvas.Highlights.Circles.Clear();
        }

        void StartDrawingRectangleAt(Point point)
        {
            session.FirstCorner = point;
            session.Mode = DrawModes.DrawingRectangleSecondCorner;
            view.Status = Properties.Resources.StartDrawingRectangleStatus;
        }

        void FinishDrawingRectangleAt(Point point)
        {
            session.SecondCorner = point;
            var box = new Box(
                firstCorner: session.FirstCorner,
                size: SizeFrom(session.FirstCorner, session.SecondCorner));

            box.Color = Color.Green;

            session.Canvas.CurrentLayer.Boxes.Add(box);

            session.Canvas.Highlights.Boxes.Clear();

            session.Mode = DrawModes.DrawingRectangleFirstCorner;

            view.RenderLayers();
        }

        void InsertBlockAt(Point point)
        {
            // Use a box for testing, but insert a whole canvas, all layers...
            var box = new Box(
              firstCorner: new Point(point.X - 4, point.Y -4),
              size: new Size(8, 8));

            box.Color = Color.Red;

            session.Canvas.CurrentLayer.Boxes.Add(box);
            session.Canvas.CurrentLayer.Circles.Add(
                new Circle()
                {
                    Center = point,
                    Radius = 6,
                    Color = Color.Red
                });

            view.RenderLayers();
        }

        void SelectEntityAt(Point point)
        {
            foreach (var entity in session.Canvas.Entities())
            {
                foreach (var vertex in entity.Points())
                {
                    if (AreNear(vertex, point))
                    {
                        entity.IsSelected = !entity.IsSelected;
                    }
                }
            }

            ShowSelections();

            view.RenderLayers();
        }

        void StartDrawingPolylineAt(Point point)
        {
            session.FirstCorner = point;
            session.Mode = DrawModes.DrawingPolylineSecondVertex;
            view.Status = Properties.Resources.StartDrawingPolylineStatus;
        }

        void AddSecondPolylineVertexAt(Point point)
        {
            session.CurrentPolyline = new Polyline(
                session.FirstCorner, 
                session.Canvas.NewLineEnd);

            session.Canvas.CurrentLayer.Polylines.Add(session.CurrentPolyline);
            session.Mode = DrawModes.DrawingPolylineExtraVertices;
            view.Status = Properties.Resources.DrawPolylineStatus;
            view.RenderLayers();
        }

        void AddExtraPolylineVertexAt(Point point)
        {
            session.CurrentPolyline.Vertices.Add(session.Canvas.NewLineEnd);

            view.Status = Properties.Resources.DrawPolylineStatus;
            view.RenderLayers();
        }

        void StartImportingPictureAt(Point point)
        {
            session.FirstCorner = point;
            session.Mode = DrawModes.ImportingPictureSecondCorner;
            view.Status = Properties.Resources.StartImportingPictureStatus;
        }

        void FinishImportingPictureAt(Point point)
        {
            session.SecondCorner = point;
            var image = new InsertedImage(
                image: Bitmap.FromFile(session.OpenFileName),
                box: new Box(
                    firstCorner: new Point(
                        session.FirstCorner.X, 
                        session.FirstCorner.Y),
                    size: SizeFrom(
                        session.FirstCorner, 
                        session.SecondCorner)));

            session.Canvas.CurrentLayer.InsertedImages.Add(image);

            CancelMode();

            view.RenderLayers();
        }

        void CancelMode()
        {
            session.Canvas.Highlights.Polylines.Clear();
            session.Canvas.Highlights.Boxes.Clear();
            session.CurrentPolyline = null;
            session.FirstCorner = Point.Empty;
            session.SecondCorner = Point.Empty;
            session.Mode = DrawModes.Ready;
            view.Status = Properties.Resources.ReadyStatus;
            view.SecondStatus = string.Empty;
        }

        static bool AreNear(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) <= NearDistance
                && Math.Abs(a.Y - b.Y) <= NearDistance;
        }

        static Size SizeFrom(Point a, Point b)
        {
            return new Size(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        static int Angle(Point a, Point b)
        {
            return (int) (Math.Atan2(b.Y - a.Y, b.X - a.X) * 180 / Math.PI);
        }
    }
}
