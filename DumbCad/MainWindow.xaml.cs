using DumbCad.Entities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

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
        //List<SKPath> paths = new List<SKPath>();
        PolylineViewCollection paths = new PolylineViewCollection();
        List<Circle> Circles = new List<Circle>();
        Circle circleStarting = new Circle();
        SKPoint circleFinishingPoint = new SKPoint();
        SKPoint polylineStarting = new SKPoint();
        SKPath polylineFinishing = new SKPath();
        SKPath polylineNextSegment = new SKPath();
        SKPoint panStart = new SKPoint();
        SKPoint panOffset = new SKPoint();

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
            canvas.Scale(zoomFactor, -zoomFactor);

            panOffsetLabel.Content = $"PanOffset: {panOffset.X:F3}, {panOffset.Y:F3}";

            canvas.Translate(panOffset.X, panOffset.Y);

            // Prevent entities from becoming invisible, smaller than a pixel
            float pixelWidth = 1 / zoomFactor;
            if(paintCircleFinished.StrokeWidth < pixelWidth)
            {
                paintCircleFinished.StrokeWidth = pixelWidth;
                paintCircleStarting.StrokeWidth = pixelWidth;
            }
            else
            {
                paintCircleFinished.StrokeWidth = 1f;
                paintCircleStarting.StrokeWidth = 1f;
            }

            foreach (var circle in Circles)
            {
                canvas.DrawCircle(circle.Location, circle.Radius, paintCircleFinished);
            }

            foreach(var polyline in paths)
            {
                var color = new SKColor(
                    red: polyline.Polyline.Color.R,
                    green: polyline.Polyline.Color.G,
                    blue: polyline.Polyline.Color.B,
                    alpha: polyline.Polyline.Color.A);

                var polylinePaint = new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1f,
                    Color = color
                };

                bool selected = polyline.IsSelected;

                if (selected)
                {
                    canvas.DrawPath(
                        path: polyline.Path,
                        paint: selected ? paintIsSelected : polylinePaint);
                }
                else
                {
                    canvas.DrawPath(
                       path: polyline.Path,
                       paint: selected ? paintIsSelected : polylinePaint);
                }
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
            if (mode == DrawMode.Select)
            {
                int p = 0;
                foreach(var polyline in paths)
                {
                    var path = polyline.Path;

                    //if(path.Bounds.Contains(worldPoint.X, worldPoint.Y))
                    {
                        for (int i = 0; i < path.Points.Length; i += 2)
                        {
                            Debug.WriteLine($"Poly: {p} Seg: {i}");
                            if(CheckSegment(worldPoint, path.Points[i], path.Points[i+1], path))
                            {
                                break;
                            }
                        }

                        //CheckSegment(worldPoint, path.Points.Last(), path);
                    }

                    p++;
                }

                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.CircleStart)
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

        private bool CheckSegment(SKPoint m, SKPoint s, SKPoint e, SKPath path)
        {
            double a = Geometry.Distance(m, s);
            double b = Geometry.Distance(m, e);
            //System.Diagnostics.Debug.WriteLine($"  Vertex {i-1}: {path.Points[i-1].X:F3}, {path.Points[i-1].Y:F3}");
            //System.Diagnostics.Debug.WriteLine($"  Vertex {i}: {path.Points[i].X:F3}, {path.Points[i].Y:F3} Distance: {dist:F3} {dist <= 5f}");
            if (a <= 5.0f || b <= 5.0f)
            {
                Debug.WriteLine($"  Selected by end point a: {a:F3} b: {b:F3}");
                paths.Select(path);
                return true;
            }

            // H * Sin Theta = Opposite
            // Inverse Cos
            double c = Geometry.Distance(s, e);
            double cosB = (Math.Pow(a, 2) + Math.Pow(c, 2) - Math.Pow(b, 2)) / (2 * a * c);
            double B = Math.Acos(cosB);

            double cosA = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);
            double A = Math.Acos(cosA);


            if (A <= 0 || A > (Math.PI / 2) || B <= 0 || B > (Math.PI / 2))
            {
                //Debug.WriteLine($"  Return by Angle: A:{A} B:{B}");
                return false;
            }

            double dist = a * Math.Sin(B);

            if (dist <= 5.0f)
            {
                //Debug.WriteLine($"  Selected by distance: {dist:F3} B angle: {B:F2} cosB: {cosB:F4}");
               // Debug.WriteLine($"  Selected by distance: {dist:F3} A angle: {A:F2} cosA: {cosA:F4}");
                paths.Select(path);
                return true;
            }
            //else 
            //{
               // Debug.WriteLine($"  NOT Selected by distance: {dist:F3} angle: {B:F2} cosB: {cosB:F4}");
                //Debug.WriteLine($"  NOT Selected by distance: {dist:F3} angle: {A:F2} cosA: {cosA:F4}");
           // }

            return false;
        }

        private SKPoint WorldOffsetFrom(Point screenPoint)
        {
            float zoomFactorNoZero = zoomFactor == 0 ? 1f : zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero),
                y: (float)-(screenPoint.Y / zoomFactorNoZero));
        }

        private SKPoint WorldPointFrom(Point screenPoint)
        {
            float zoomFactorNoZero = zoomFactor == 0 ? 1f : zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero) - panOffset.X, 
                y: (float)-(screenPoint.Y / zoomFactorNoZero) - panOffset.Y);
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

                startPointLabel.Content = $"PanSart: {panStart.X:F2}, {panStart.Y:F2}";
                panOffsetLabel.Content = $"PO: {panOffset.X:F3}, {panOffset.Y:F3}/NO: {newOffset.X:F3}, {newOffset.Y:F3}";
                viewPort.InvalidateVisual();
            }
            else if(mode == DrawMode.PanFinishLive)
            {
                var p = WorldOffsetFrom(screenPoint);
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
