using DumbCad.Entities;
using DumbCad.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbCad
{
    public class CanvasPainter
    {
        public Entities.Point CursorPoint { get; set; }
        public PolylineView polylineViewStarting = new PolylineView();
        public Circle circleStarting = new Circle();
        public SKPoint circleFinishingPoint = new SKPoint();
        
        SKPaint paintCircleFinished = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            Color = SKColors.Brown
        };

        SKPaint paintCircleStarting = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = SKColors.Blue
        };

        SKPaint paintIsSelected = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3f,
            Color = SKColors.DarkBlue
        };

        SKPaint paintIsHovered = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5f,
            Color = SKColors.CornflowerBlue
        };

        SKPaint paintIsHoveredAndSelected = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5f,
            Color = SKColors.Magenta
        };

        public void Paint(SKCanvas canvas, DrawingFile file, Viewer viewer)
        {
            canvas.Clear(SKColors.Beige);
            //**//panOffsetLabel.Content = $"PanOffset: {panOffset.X:F3}, {panOffset.Y:F3}";

            canvas.Scale(viewer.zoomFactor, - viewer.zoomFactor);
            canvas.Translate(viewer.panOffset.X, viewer.panOffset.Y);

            // Flip upside down 
            canvas.Scale(1, -1);

            foreach (var image in file.images)
            {
                var p = new SKPoint(
                    x: (float)image.Image.Location.X,
                    // Reverse the Y since the canvas is flipped
                    y: (float)-image.Image.Location.Y);

                //var rect = new SKRect(p.X, p.Y, image.SkImage.Width, image.SkImage.Height);
                //canvas.DrawImage(image.SkImage, dest: rect);
                canvas.DrawImage(image.SkImage, p);
                //canvas.DrawImage()
            }

            // Flip back up
            canvas.Scale(1, -1);

            // Prevent entities from becoming invisible, smaller than a pixel
            float pixelWidth = (float)(1 / (double) viewer.zoomFactor);
            if (paintCircleFinished.StrokeWidth < pixelWidth)
            {
                paintCircleFinished.StrokeWidth = pixelWidth;
                paintCircleStarting.StrokeWidth = pixelWidth;
            }
            else
            {
                paintCircleFinished.StrokeWidth = 1f;
                paintCircleStarting.StrokeWidth = 1f;
            }


            foreach (var circle in file.Circles)
            {
                canvas.DrawCircle(circle.Location, circle.Radius, paintCircleFinished);
            }

            foreach (var polyline in file.polylines)
            {
                DrawPolylineToCanvas(canvas, pixelWidth, polyline);
            }

            foreach (var line in file.lines)
            {
                DrawPolylineToCanvas(canvas, pixelWidth * 3, line);
            }

            if (viewer.mode == DrawMode.CircleFinish && circleStarting != null)
            {
                canvas.DrawCircle(circleStarting.Location, circleStarting.Radius, paintCircleStarting);
                canvas.DrawRect(
                    x: circleStarting.Location.X,
                    y: circleStarting.Location.Y,
                    w: 2f,
                    h: 2f,
                    paintCircleStarting);

                canvas.DrawLine(
                    circleStarting.Location,
                    circleFinishingPoint,
                    paintCircleStarting);
            }
            else if (viewer.mode == DrawMode.PolylineFinish)
            {
                var lastPoint = polylineViewStarting.Polyline.Vertices.Last();
                var st = new SKPoint((float)lastPoint.X, (float)lastPoint.Y);
                var en = new SKPoint((float)CursorPoint.X, (float)CursorPoint.Y);
                canvas.DrawLine(p0: st, p1: en, paint: paintCircleStarting);

                for (int i = 1; i < polylineViewStarting.Polyline.Vertices.Count; i++)
                {
                    var start = new SKPoint(
                        x: (float)polylineViewStarting.Polyline.Vertices[i - 1].X,
                        y: (float)polylineViewStarting.Polyline.Vertices[i - 1].Y);

                    var end = new SKPoint(
                        x: (float)polylineViewStarting.Polyline.Vertices[i].X,
                        y: (float)polylineViewStarting.Polyline.Vertices[i].Y);

                    canvas.DrawLine(
                        p0: start,
                        p1: end,
                        paint: paintCircleStarting);
                }
            }

            foreach (var image in file.images)
            {
                var p = new SKPoint(
                    x: (float)image.Image.Location.X,
                    y: (float)image.Image.Location.Y);

                if (image.Image.IsSelected || image.Image.IsHovered)
                {
                    var topRight = new SKPoint(p.X + image.SkImage.Width, p.Y);
                    var bottomRight = new SKPoint(p.X + image.SkImage.Width, p.Y - image.SkImage.Height);
                    var bottomLeft = new SKPoint(p.X, p.Y - image.SkImage.Height);
                    paintIsHovered.StrokeWidth = pixelWidth * 3;
                    var paint = image.Image.IsSelected ? paintIsSelected : paintIsHovered;
                    canvas.DrawLine(p, topRight, paint);
                    canvas.DrawLine(topRight, bottomRight, paint);
                    canvas.DrawLine(bottomRight, bottomLeft, paint);
                    canvas.DrawLine(bottomLeft, p, paint);
                }
            }
        }

        private void DrawPolylineToCanvas(SKCanvas canvas, float pixelWidth, PolylineView polyline)
        {
            var color = new SKColor(
                red: polyline.Polyline.Color.R,
                green: polyline.Polyline.Color.G,
                blue: polyline.Polyline.Color.B,
                alpha: polyline.Polyline.Color.A);

            var polylinePaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = pixelWidth,
                Color = color
            };

            SKPaint polyPaint = polylinePaint;
            if (polyline.Polyline.IsHovered && polyline.Polyline.IsSelected)
            {
                polyPaint = paintIsHoveredAndSelected;
            }
            else if (polyline.Polyline.IsHovered)
            {
                polyPaint = paintIsHovered;
            }
            else if (polyline.Polyline.IsSelected)
            {
                polyPaint = paintIsSelected;
            }

            for (int i = 1; i < polyline.Polyline.Vertices.Count; i++)
            {
                var start = new SKPoint(
                    x: (float)polyline.Polyline.Vertices[i - 1].X,
                    y: (float)polyline.Polyline.Vertices[i - 1].Y);

                var end = new SKPoint(
                    x: (float)polyline.Polyline.Vertices[i].X,
                    y: (float)polyline.Polyline.Vertices[i].Y);

                canvas.DrawLine(
                    p0: start,
                    p1: end,
                    paint: polyPaint);
            }
        }
    }
}
