﻿using System;
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
                session.Canvas.NewLineStart = session.CurrentPolyline?.Vertices.Last() ?? session.FirstCorner;
                session.Canvas.NewLineEnd = location;

                var rubberband = new Polyline(
                    session.Canvas.NewLineStart, 
                    session.Canvas.NewLineEnd)
                {
                    Color = Color.Blue,
                };

                session.Canvas.Highlights.Polylines.Clear();
                session.Canvas.Highlights.Polylines.Add(rubberband);
            }

            if (session.Mode == DrawModes.DrawingRectangleSecondCorner
                || session.Mode == DrawModes.ImportingPictureSecondCorner)
            {
                var size = SizeFrom(session.FirstCorner, location);

                var box = new Box(session.FirstCorner, size)
                {
                    Color = Color.Blue
                };

                session.Canvas.Highlights.Boxes.Clear();
                session.Canvas.Highlights.Boxes.Add(box);
            }

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
                    foreach (var p in entity.Points())
                    {
                        if (AreNear(p, location))
                        {
                            circle.Center = p;

                            session.Canvas.Highlights.Circles.Clear();
                            session.Canvas.Highlights.Circles.Add(circle);
                            nearSomething = true;
                        }
                    }
                }
            }

            if (!nearSomething)
                session.Canvas.Highlights.Circles.Clear();

            view.InvalidateImage();
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

            CancelMode();

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
            session.CurrentPolyline = new Polyline(session.FirstCorner, point);
            session.Canvas.CurrentLayer.Polylines.Add(session.CurrentPolyline);
            session.Mode = DrawModes.DrawingPolylineExtraVertices;
            view.Status = Properties.Resources.DrawPolylineStatus;
            view.RenderLayers();
        }

        void AddExtraPolylineVertexAt(Point point)
        {
            session.CurrentPolyline.Vertices.Add(point);
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
                    firstCorner: new Point(session.FirstCorner.X, session.FirstCorner.Y),
                    size: SizeFrom(session.FirstCorner, session.SecondCorner)));

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
    }
}
