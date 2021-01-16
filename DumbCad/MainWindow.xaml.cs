using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace DumbCad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DrawMode mode = DrawMode.Ready;
        DrawMode previousMode = DrawMode.Ready;
        float zoomFactor = 1f;
        List<SKPath> paths = new List<SKPath>();

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

        List<Circle> Circles = new List<Circle>();
        Circle circleStarting = new Circle();
        SKPoint circleFinishingPoint = new SKPoint();
        SKPoint polylineStarting = new SKPoint();
        SKPath polylineFinishing = new SKPath();
        SKPath polylineNextSegment = new SKPath();
        int xCenter = 0;
        int yCenter = 0;
        SKPoint panStart = new SKPoint();
        SKPoint panOffset = new SKPoint();

        public MainWindow()
        {
            InitializeComponent();

            SetMode(DrawMode.Ready);
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            viewPort.InvalidateVisual();
        }

        private void viewPort_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Beige);
            //canvas.Scale(zoomFactor);
            canvas.Scale(zoomFactor, -zoomFactor);
            xCenter = e.Info.Width/2;
            yCenter = e.Info.Height/2;

            panOffsetLabel.Content = $"PanOffset: {panOffset.X:F3}, {panOffset.Y:F3}";

            canvas.Translate(panOffset.X, panOffset.Y);

            //canvas.Translate(-xCenter, -yCenter);
            //SKMatrix matrix = SKMatrix.CreateTranslation(-xCenter, -yCenter);
            //var rotation = SKMatrix44.CreateIdentity();
            //rotation.PostConcat(SKMatrix44.CreateRotationDegrees(1f, 0, 0, 180f));
            //matrix.PostConcat(rotation.Matrix);
            //canvas.SetMatrix(matrix);

            foreach (var circle in Circles)
            {
                canvas.DrawCircle(circle.Location, circle.Radius, paintCircleFinished);
            }

            foreach(var path in paths)
            {
                // TODO: Change path into polyline? check for click
                canvas.DrawPath(path, paintCircleFinished);
            }

            if (mode == DrawMode.CircleFinish && circleStarting != null)
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
            else if (mode == DrawMode.PolylineFinish)
            {
                canvas.DrawPath(polylineFinishing, paintCircleStarting);
                canvas.DrawPath(polylineNextSegment, paintCircleStarting);
            }
        }

        private void viewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void viewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickAt(e.GetPosition(viewPort), e.LeftButton == MouseButtonState.Pressed);
        }

        private void ClickAt(Point point, bool leftButtonDown)
        {
            SKPoint worldPoint = WorldPointFrom(point);
            if (mode == DrawMode.CircleStart)
            {
                circleStarting = new Circle()
                {
                    Location = worldPoint
                };

                SetMode(DrawMode.CircleFinish);
                Cursor = Cursors.Cross;
            }
            else if (mode == DrawMode.CircleFinish)
            {
                if (circleStarting.Radius > 0)
                {
                    Circles.Add(circleStarting);
                }

                circleStarting = null;

                SetMode(DrawMode.CircleStart);
                Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineStart)
            {
                polylineStarting = worldPoint;
                polylineNextSegment = new SKPath();

                SetMode(DrawMode.PolylineFinish);
                Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineFinish)
            {
                polylineFinishing.AddPoly(new SKPoint[] { polylineStarting, worldPoint });
                //paths.Add(polylineFinishing);
                coordinatesLabel.Content = $"Paths: {paths.Count}";
                polylineStarting = worldPoint;

                viewPort.InvalidateVisual();
            }
            //else if(mode == DrawMode.PanStart)
            //{
            //    panStart = WorldOffsetFrom(point);
            //    SetMode(DrawMode.PanFinish);
            //    panOffsetLabel.Content = $"Move mouse";
            //    startPointLabel.Content = $"StPt: {panStart.X:F2}, {panStart.Y:F2}";
            //    viewPort.InvalidateVisual();
            //}
            //else if (mode == DrawMode.PanFinish)
            //{
            //    var p = WorldOffsetFrom(point);

            //    var newOffset = new SKPoint(
            //        x: p.X - panStart.X,
            //        y: p.Y - panStart.Y);

            //    panOffset = new SKPoint(
            //       x: panOffset.X + newOffset.X,
            //       y: panOffset.Y + newOffset.Y);

            //    SetMode(DrawMode.PanStart);
            //    panOffsetLabel.Content = $"Click start point";
            //    startPointLabel.Content = $"StPt: {panStart.X:F2}, {panStart.Y:F2}";
            //    viewPort.InvalidateVisual();
            //}
            else if (mode == DrawMode.PanStartLive)
            {
                panStart = WorldOffsetFrom(point);
                SetMode(DrawMode.PanFinishLive);
                panOffsetLabel.Content = $"Move mouse";
                startPointLabel.Content = $"StPt: {panStart.X:F2}, {panStart.Y:F2}";
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PanFinishLive)
            {
                SetMode(DrawMode.PanStartLive);
                panOffsetLabel.Content = $"Click start point";
                startPointLabel.Content = $"StPt: {panStart.X:F2}, {panStart.Y:F2}";
                viewPort.InvalidateVisual();
            }
        }

        private Point PointFromWorld(SKPoint point)
        {
            float zoomFactorNoZero = zoomFactor == 0 ? 1f : zoomFactor;

            return new Point(
                x: (float)(point.X + panOffset.X) * zoomFactorNoZero,
                y: (float)-(point.Y + panOffset.X) * zoomFactorNoZero);
        }

        private SKPoint WorldOffsetFrom(Point screenPoint)
        {
            float zoomFactorNoZero = zoomFactor == 0 ? 1f : zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero) - panOffset.X,
                y: (float)-(screenPoint.Y / zoomFactorNoZero) - panOffset.Y);
        }

        private SKPoint WorldPointFrom(Point screenPoint)
        {
            float zoomFactorNoZero = zoomFactor == 0 ? 1f : zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero) - panOffset.X, 
                y: (float)-(screenPoint.Y / zoomFactorNoZero) - panOffset.Y);
        }

        private SKPoint WorldOffsetFrom(Point screenPoint, SKPoint worldPoint)
        {
            float zoomFactorNoZero = zoomFactor == 0 ? 1f : zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero) - worldPoint.X,
                y: (float)-(screenPoint.Y / zoomFactorNoZero) - worldPoint.Y);
        }

        private void drawCircleButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.CircleStart);
        }

        void SetMode(DrawMode mode)
        {
            this.mode = mode;
            this.modeLabel.Content = mode.ToString();
        }

        private void viewPort_MouseMove(object sender, MouseEventArgs e)
        {
            var screenPoint = e.GetPosition(viewPort);
            var worldPoint = WorldPointFrom(screenPoint);
            if (mode == DrawMode.CircleFinish && circleStarting != null)
            {
                circleFinishingPoint = worldPoint;
                float radius = (float)Geometry.Distance(circleStarting.Location, circleFinishingPoint);

                circleStarting.Radius = radius;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineStart)
            {
                coordinatesLabel.Content = $"Paths: {paths.Count}";
            }
            else if (mode == DrawMode.PolylineFinish)
            {
                coordinatesLabel.Content = $"Paths: {paths.Count}";

                var lastPoint = polylineFinishing.PointCount == 0
                    ? polylineStarting
                    : polylineFinishing.Points[polylineFinishing.PointCount - 1];

                polylineNextSegment = new SKPath();
                polylineNextSegment.AddPoly(new SKPoint[] { lastPoint, worldPoint });
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PanFinish)
            {
                var p = WorldOffsetFrom(screenPoint);

                var newOffset = new SKPoint(
                   x: p.X - panStart.X,
                   y: p.Y - panStart.Y);

                  //panOffset = new SKPoint(
                  // x: panOffset.X + newOffset.X,
                  // y: panOffset.Y + newOffset.Y);

                startPointLabel.Content = $"PanSart: {panStart.X:F2}, {panStart.Y:F2}";
                panOffsetLabel.Content = $"PO: {panOffset.X:F3}, {panOffset.Y:F3}/NO: {newOffset.X:F3}, {newOffset.Y:F3}";
                viewPort.InvalidateVisual();
            }
            else if(mode == DrawMode.PanFinishLive)
            {
                var p = WorldPointFrom(screenPoint);
                var newOffset = new SKPoint(
                   x: p.X - panStart.X,
                   y: p.Y - panStart.Y);

                panStart = p;

                panOffset = new SKPoint(
                   x: panOffset.X + newOffset.X,
                   y: panOffset.Y + newOffset.Y);

                startPointLabel.Content = $"PanSart: {panStart.X:F2}, {panStart.Y:F2}";
                panOffsetLabel.Content = $"PO: {panOffset.X:F3}, {panOffset.Y:F3}/NO: {newOffset.X:F3}, {newOffset.Y:F3}";
                viewPort.InvalidateVisual();
            }


            cursorLocationLabel.Content = $"Screen: {screenPoint.X:F3},{screenPoint.Y:F3}";
            coordinatesLabel.Content = $"Coordinates: {worldPoint.X:F3}, {worldPoint.Y:F3}";
        }

        private void viewPort_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(mode == DrawMode.CircleFinish)
            {
                SetMode(DrawMode.CircleStart);
                viewPort.InvalidateVisual();
            }
            else if(mode == DrawMode.CircleStart)
            {
                SetMode(DrawMode.Ready);
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineFinish)
            {
                if (polylineFinishing.PointCount > 0)
                {
                    paths.Add(polylineFinishing);
                }

                polylineFinishing = new SKPath();

                SetMode(DrawMode.PolylineStart);
                viewPort.InvalidateVisual();
                coordinatesLabel.Content = $"Paths: {paths.Count}";
            }
            else if (mode == DrawMode.PolylineStart)
            {
                SetMode(DrawMode.Ready);
                viewPort.InvalidateVisual();
                coordinatesLabel.Content = $"Paths: {paths.Count}";
            }
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.Select);
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor *= 2f;
            zoomFactorLabel.Content = $"Z:{zoomFactor:F4}";
            viewPort.InvalidateVisual();
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor *= 0.5f;
            zoomFactorLabel.Content = $"Z:{zoomFactor:F4}";
            viewPort.InvalidateVisual();
        }

        private void drawPolylineButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.PolylineStart);
        }

        private void panButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.PanStart);
            Cursor = Cursors.Hand;
        }

        private void viewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
           if(e.MiddleButton == MouseButtonState.Pressed)
            {
                previousMode = mode;
                SetMode(DrawMode.PanFinishLive);
                panStart = WorldOffsetFrom(e.GetPosition(viewPort));
                viewPort.InvalidateVisual();
            }
            if (e.LeftButton == MouseButtonState.Pressed && mode == DrawMode.PanStart)
            {
                panStart = WorldOffsetFrom(e.GetPosition(viewPort));
                previousMode = mode;
                SetMode(DrawMode.PanFinishLive);
                viewPort.InvalidateVisual();
            }
        }

        private void viewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Released && mode == DrawMode.PanFinishLive)
            {
                SetMode(previousMode);
                viewPort.InvalidateVisual();
            }
            else if (e.LeftButton == MouseButtonState.Released && mode == DrawMode.PanFinishLive)
            {
                SetMode(previousMode);
                viewPort.InvalidateVisual();
            }
        }

        private void viewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var screenPoint = e.GetPosition(viewPort);
            var worldPoint = WorldPointFrom(screenPoint);

            float multiplier = 0.75f;
            float adjustment = e.Delta < 0 ? multiplier : (1f/multiplier);
            zoomFactor = zoomFactor * adjustment;
            zoomFactorLabel.Content = $"Z:{zoomFactor:F4}";

            panOffset = new SKPoint(
                x: (float)(screenPoint.X / zoomFactor) - worldPoint.X,
                y: (float)-(screenPoint.Y / zoomFactor) - worldPoint.Y);

            viewPort.InvalidateVisual();
        }

        private void zoomResetButton_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor = 1.0f;
            panOffset = new SKPoint();
            viewPort.InvalidateVisual();
        }
    }

    public class Circle
    {
        public SKPoint Location { get; set; }
        public float Radius { get; set; }
    }
}
