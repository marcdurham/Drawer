using DumbCad.Entities;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using Microsoft.Win32;
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
        Polyline polyStarting = new Polyline();
        PolylineView polylineViewStarting = new PolylineView();
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

        public MainWindow()
        {
            InitializeComponent();

            SetMode(DrawMode.Ready);
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "AutoCAD Drawing Exchange (DXF)|*.dxf|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    //paper = PaperBuilder.GetPaper();
                    //myViewport3D.Children.Clear();
                    //myViewport3D.Children.Add(paper);

                    var dxfFile = DxfFile.Load(dialog.FileName);

                    foreach (DxfEntity entity in dxfFile.Entities)
                    {
                        if (entity.Layer.ToUpper().StartsWith("PIPE"))
                        {
                            switch (entity.EntityType)
                            {
                                case DxfEntityType.Polyline:
                                    var dxfPolyline = (DxfPolyline)entity;
                                    var poly = new Polyline
                                    {
                                        Color = Color.Red
                                    };

                                    var path = new SKPath();
                                    var skvertices = new SKPoint[dxfPolyline.Vertices.Count];
                                    int i = 0;
                                    foreach(var v in dxfPolyline.Vertices)
                                    {
                                        var vertex = new Entities.Point(
                                            v.Location.X,
                                            v.Location.Y);
                                        skvertices[i] = new SKPoint((float)vertex.X, (float)vertex.Y);
                                        poly.Vertices.Add(vertex);
                                    }

                                    path.AddPoly(skvertices, close: false);


                                    var polyView = new PolylineView
                                    {
                                         Polyline = poly,
                                         Path = path
                                    };

                                    paths.Add(polyView);
                                    //DrawPolyline((DxfPolyline)entity);
                                   

                                    break;
                                case DxfEntityType.LwPolyline:
                                    // DrawLwPolyline((DxfLwPolyline)entity);
                                    var dxfLwPolyline = (DxfLwPolyline)entity;
                                    var poly2 = new Polyline
                                    {
                                        Color = Color.Red
                                    };

                                    var path2 = new SKPath();
                                    var skvertices2 = new SKPoint[dxfLwPolyline.Vertices.Count];
                                    int i2 = 0;
                                    foreach (var v in dxfLwPolyline.Vertices)
                                    {
                                        var vertex = new Entities.Point(v.X, v.Y);
                                        skvertices2[i2] = new SKPoint((float)vertex.X, (float)vertex.Y);
                                        poly2.Vertices.Add(vertex);
                                    }

                                    path2.AddPoly(skvertices2, close: false);


                                    var polyView2 = new PolylineView
                                    {
                                        Polyline = poly2,
                                        Path = path2
                                    };

                                    paths.Add(polyView2);
                                    break;
                            }
                        }
                    }
                }
            }
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

                SKPaint polyPaint = polylinePaint;
                if (polyline.IsHovered && polyline.IsSelected)
                {
                    polyPaint = paintIsHoveredAndSelected;
                }
                else if (polyline.IsHovered)
                {
                    polyPaint = paintIsHovered;
                }
                else if (polyline.IsSelected)
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
                //canvas.DrawPath(polylineViewStarting.Path, paintCircleStarting);
                canvas.DrawPath(polylineNextSegment, paintCircleStarting);
                for(int i = 1; i < polylineViewStarting.Polyline.Vertices.Count; i++)
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
            
            // 5f is five pixels at any zoom factor
            float near = 5f / zoomFactor;
            if (mode == DrawMode.Select)
            {
                foreach (var polyline in paths) 
                {
                    if(polyline.IsNear(worldPoint, near))
                    {
                        polyline.IsSelected = !polyline.IsSelected;
                    }
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
                viewPort.Cursor = Cursors.Cross;
            }
            else if (mode == DrawMode.CircleFinish)
            {
                if (circleStarting.Radius > 0)
                {
                    Circles.Add(circleStarting);
                }

                circleStarting = null;

                SetMode(DrawMode.CircleStart);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineStart)
            {
                polylineStarting = worldPoint;
                
                
                //polyStarting = new Polyline()
                //{
                //    Color = Color.Red,
                //    Width = 3f,
                //};
                polylineViewStarting = new PolylineView
                {
                    Path = new SKPath(),
                    Polyline = new Polyline()
                    {
                        Color = Color.Red,
                        Width = 3f,
                    }
                };

                //*//polylineViewStarting.Path.AddPoly(new SKPoint[] { worldPoint });
                polylineViewStarting.Polyline.Vertices.Add(
                    new Entities.Point(
                        x: worldPoint.X, 
                        y: worldPoint.Y));
                // TODO: Not yet I think
                //paths.Add(polylineViewStarting);

                // This moves as the cursor moves
                //polylineNextSegment = new SKPath();
                //polylineNextSegment.AddPoly(new SKPoint[] { worldPoint });

                SetMode(DrawMode.PolylineFinish);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineFinish)
            {
                //polylineFinishing.AddPoly(new SKPoint[] { polylineStarting, worldPoint });

                //*//polylineViewStarting.Path.LineTo(worldPoint);
                polylineViewStarting.Polyline.Vertices.Add(
                    new Entities.Point(
                        x: worldPoint.X,
                        y: worldPoint.Y));

                //var lastPoint = polylineViewStarting.Polyline.Vertices.Last();
                
                //////polylineNextSegment = new SKPath();
                //////polylineNextSegment.LineTo(polylineStarting);
                //////polylineNextSegment.LineTo(worldPoint);
                
                
                //polylineNextSegment.AddPoly(new SKPoint[] { worldPoint });

                // No, not added until it's finished
                //paths.Add(polylineFinishing);
                //paths.Add(
                //    new PolylineView
                //    {
                //         Polyline = polyStarting,
                //         Path = polylineFinishing
                //    });

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
                viewPort.Cursor = Cursors.Hand;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PanFinishLive)
            {
                SetMode(DrawMode.PanStartLive);
                panOffsetLabel.Content = $"Click start point";
                startPointLabel.Content = $"StPt: {panStart.X:F2}, {panStart.Y:F2}";
                viewPort.Cursor = Cursors.SizeAll;
                viewPort.InvalidateVisual();
            }
        }

        private bool CheckSegment(SKPoint m, SKPoint s, SKPoint e, SKPath path, float near)
        {
            double a = Geometry.Distance(m, s);
            double b = Geometry.Distance(m, e);
            if (a <= near || b <= near)
            {
                paths.Select(path);
                return true;
            }

            double c = Geometry.Distance(s, e);
            double cosB = (Math.Pow(a, 2) + Math.Pow(c, 2) - Math.Pow(b, 2)) / (2 * a * c);
            double B = Math.Acos(cosB);

            double cosA = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);
            double A = Math.Acos(cosA);

            double rightAngle = Math.PI / 2;
            if (A <= 0 || A > rightAngle || B <= 0 || B > rightAngle)
            {
                return false;
            }

            double dist = a * Math.Sin(B);

            if (dist <= near)
            {
                paths.Select(path);
                return true;
            }

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
            if (mode == DrawMode.Select)
            {
                foreach(var polyline in paths)
                {
                    if(polyline.IsNear(worldPoint))
                    {
                        polyline.IsHovered = true;
                    }
                    else
                    {
                        polyline.IsHovered = false;
                    }
                }

                viewPort.InvalidateVisual();
            }

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

                //var lastPoint = polylineFinishing.PointCount == 0
                //    ? polylineStarting
                //    : polylineFinishing.Points[polylineFinishing.PointCount - 1];
                var lastPoint = polylineViewStarting.Polyline.Vertices.Last();
                var lp = new SKPoint((float)lastPoint.X, (float)lastPoint.Y);

                polylineNextSegment = new SKPath();
                polylineNextSegment.AddPoly(new SKPoint[] { lp, worldPoint });
                //polylineNextSegment = new SKPath();
                //polylineNextSegment.LineTo(polylineStarting);
                //polylineNextSegment.LineTo(worldPoint);

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
            else if((mode == DrawMode.PanFinishLive && e.MiddleButton == MouseButtonState.Pressed)
                || (mode == DrawMode.PanFinishLiveLeft && e.LeftButton == MouseButtonState.Pressed))
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
            else if ((mode == DrawMode.PanFinishLive && e.MiddleButton == MouseButtonState.Released)
                || (mode == DrawMode.PanFinishLiveLeft && e.LeftButton == MouseButtonState.Released))
            {
                SetMode(previousMode);
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
                if (polylineViewStarting.Polyline.Vertices.Count > 0)
                {
                    // TODO: Maybe add that last point from polylineFinishing here?
                    paths.Add(polylineViewStarting);
                }

                polylineFinishing = new SKPath();
                polylineViewStarting = new PolylineView();
                polyStarting = new Polyline();

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
            viewPort.Cursor = Cursors.Cross;
        }

        private void panButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.PanStart);
            viewPort.Cursor = Cursors.Arrow;
        }

        private void viewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                previousMode = mode;
                SetMode(DrawMode.PanFinishLive);
                panStart = WorldOffsetFrom(e.GetPosition(viewPort));
                viewPort.Cursor = Cursors.SizeAll;
                viewPort.InvalidateVisual();
            }
            else if (e.LeftButton == MouseButtonState.Pressed && mode == DrawMode.PanStart)
            {
                previousMode = mode;
                SetMode(DrawMode.PanFinishLiveLeft);
                panStart = WorldOffsetFrom(e.GetPosition(viewPort));
                viewPort.Cursor = Cursors.SizeAll;
                viewPort.InvalidateVisual();
            }
        }

        private void viewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Released && mode == DrawMode.PanFinishLive)
            {
                SetMode(previousMode);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (e.LeftButton == MouseButtonState.Released && mode == DrawMode.PanFinishLive)
            {
                SetMode(previousMode);
                viewPort.Cursor = Cursors.Cross;
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
