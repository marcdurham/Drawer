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

        internal void InsertImage(string fileName)
        {
            session.OpenInsertPictureFileName = fileName;
            session.Mode = DrawModes.InsertingImageFirstCorner;
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
            var selected = new List<Entity>();
            foreach (var entity in session.Canvas.Entities())
            {
                if (entity.IsSelected)
                {
                    selected.Add(entity);
                }
            }
            
            var delete = view
                .AskUser($"Delete the {selected.Count} selected entities?");

            if (delete != UserAnswer.Yes)
            {
                return;
            }

            foreach (var entity in selected)
            {
                session.Canvas.Delete(entity);
            }

            ShowSelections();

            view.RenderLayers();
        }

        internal void ClickAt(Point point, bool cancel, bool isMiddleButton)
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
                case DrawModes.InsertingImageFirstCorner:
                    StartImportingPictureAt(point);
                    break;
                case DrawModes.InsertingImageSecondCorner:
                    FinishInsertingImageAt(point);
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
                case DrawModes.Panning:
                    // Do nothing, but don't CancelMode();
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
                || session.Mode == DrawModes.InsertingImageSecondCorner)
            {
                ShowNewRectangle(location);
            }
 
            HoverOverPointsAt(location);

            view.RefreshImage();
        }

        internal void OpenFile(string fileName)
        {
            session.Canvas = DxfFileOpener.OpenFile(fileName);

            view.Canvas = session.Canvas;

            session.FileName = fileName;

            view.RenderLayers();
        }

        internal void NewFile()
        {
            session.Canvas.CurrentLayer.Clear();
            session.FileName = null;
            view.RenderLayers();
        }

        internal void StartPanning()
        {
            session.Mode = DrawModes.Panning;
            view.Status = Properties.Resources.PanningStatus;
        }

        internal void ZoomInClick()
        {
            session.ZoomFactor *= 2;
            view.Status = Properties.Resources.ZoomingInStatus;
            view.RenderLayers();
        }

        internal void ZoomOutClick()
        {
            session.ZoomFactor /= 2;
            view.Status = Properties.Resources.ZoomingOutStatus;
            view.RenderLayers();
        }

        internal void InsertBlock()
        {
            session.Mode = DrawModes.StartInsertingBlock;
            view.Status = "Inserting block: Click insertion point:";
        }

        internal void MouseDownAt(System.Drawing.Point point, bool isMiddleButton)
        {
            if (session.Mode == DrawModes.Panning || isMiddleButton)
            {
                session.StartPanningFrom = point;
                view.SecondStatus = $"Start panning: {session.StartPanningFrom.X}, {session.StartPanningFrom.Y} ...";
            }
        }

        internal void MouseUpAt(System.Drawing.Point point, bool isMiddleButton)
        {
            if (session.Mode == DrawModes.Panning || isMiddleButton)
            {
                session.EndPanningAt = point;

                int panX = 0;
                int panY = 0;

                if (!session.StartPanningFrom.IsEmpty
                    && !session.EndPanningAt.IsEmpty)
                {
                    System.Drawing.Point started = session.StartPanningFrom;
                    System.Drawing.Point ended = session.EndPanningAt;
                    panX = ended.X - started.X;
                    panY = ended.Y - started.Y;
                    session.PanningOffset = new System.Drawing.Point(
                        x: session.PanningOffset.X + panX,
                        y: session.PanningOffset.Y + panY);
                }

                view.SecondStatus = $"Start panning: {session.StartPanningFrom.X}, {session.StartPanningFrom.Y} to {point.X}, {point.Y}";

                view.RenderLayers();
            }
        }

        internal void ShowSelections()
        {
            int radius = 5;
            session.Canvas.Selections.Grips.Clear();

            foreach (var layer in session.Canvas.Layers)
                foreach (var entity in layer.Entities())
                    if (entity.IsSelected)
                        foreach (var point in entity.Points())
                        {
                            session.Canvas.Selections.Grips.Add(
                                new Circle()
                                {
                                    Center = point,
                                    Color = Color.Magenta,
                                    Radius = radius,
                                });
                        }
        }

        internal void SaveAs(string fileName)
        {
            DxfFileSaver.SaveAs(session.Canvas, fileName);
            session.FileName = fileName;
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

                            session.Canvas.Highlights.Grips.Clear();
                            session.Canvas.Highlights.Grips.Add(circle);
                            nearSomething = true;
                        }
                    }
                }
            }

            if (!nearSomething)
                session.Canvas.Highlights.Grips.Clear();
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
            session.Canvas.CurrentLayer.Grips.Add(
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

            session.CurrentPolyline.Color = Color.DarkRed;

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
            session.Mode = DrawModes.InsertingImageSecondCorner;
            view.Status = Properties.Resources.StartImportingPictureStatus;
        }

        void FinishInsertingImageAt(Point point)
        {
            session.SecondCorner = point;
            var image = new InsertedImage(
                image: Bitmap.FromFile(session.OpenInsertPictureFileName),
                file: session.OpenInsertPictureFileName,
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
            return new Size(
                width: Math.Abs((int)a.X - (int)b.X),
                height: Math.Abs((int)a.Y - (int)b.Y));
        }

        static int Angle(Point a, Point b)
        {
            return (int) (Math.Atan2(b.Y - a.Y, b.X - a.X) * 180 / Math.PI);
        }
    }
}
