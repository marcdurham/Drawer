using DumbCad.Entities;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
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
        PolylineViewCollection polylines = new PolylineViewCollection();
        PolylineViewCollection lines = new PolylineViewCollection();
        List<Image> images = new List<Image>();

        List<Circle> Circles = new List<Circle>();
        Circle circleStarting = new Circle();
        SKPoint circleFinishingPoint = new SKPoint();
        PolylineView polylineViewStarting = new PolylineView();
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

        public Entities.Point CursorPoint { get; private set; }

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
                        //if (entity.Layer.ToUpper().StartsWith("PIPE"))
                        {
                            switch (entity.EntityType)
                            {
                                case DxfEntityType.Image:
                                    var dxfImage = (DxfImage)entity;
                                    // TODO: Load images
                                    break;
                                case DxfEntityType.Line:
                                    var dxfLine = (DxfLine)entity;
                                    var line = new Polyline
                                    {
                                        Color = Color.Green,
                                        Width = 2f
                                    };

                                    var p1 = new Entities.Point(
                                        dxfLine.P1.X,
                                        dxfLine.P1.Y);

                                    var p2 = new Entities.Point(
                                        dxfLine.P2.X,
                                        dxfLine.P2.Y);

                                    line.Vertices.Add(p1);
                                    line.Vertices.Add(p2);

                                    var lineView = new PolylineView
                                    {
                                        Polyline = line,
                                        Path = new SKPath()
                                    };

                                    lines.Add(lineView);
                                    break;
                                case DxfEntityType.Polyline:
                                    var dxfPolyline = (DxfPolyline)entity;
                                    var poly = new Polyline
                                    {
                                        Color = Color.Red,
                                        Width = 2f
                                    };

                                    foreach(var v in dxfPolyline.Vertices)
                                    {
                                        var vertex = new Entities.Point(
                                            v.Location.X,
                                            v.Location.Y);

                                        poly.Vertices.Add(vertex);
                                    }

                                    var polyView = new PolylineView
                                    {
                                         Polyline = poly,
                                         Path = new SKPath()
                                    };

                                    polylines.Add(polyView);
                                    break;
                                case DxfEntityType.LwPolyline:
                                    var dxfLwPolyline = (DxfLwPolyline)entity;
                                    var poly2 = new Polyline
                                    {
                                        Color = Color.Red,
                                        Width = 3f
                                    };

                                    foreach (var v in dxfLwPolyline.Vertices)
                                    {
                                        var vertex = new Entities.Point(v.X, v.Y);

                                        poly2.Vertices.Add(vertex);
                                    }

                                    var polyView2 = new PolylineView
                                    {
                                        Polyline = poly2,
                                        Path = new SKPath()
                                    };

                                    polylines.Add(polyView2);
                                    break;
                            }
                        }
                    }
                }

                viewPort.InvalidateVisual();
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
            float pixelWidth = (float)(1 / (double)zoomFactor);
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

            foreach(var image in images)
            {
                //var skBitmap = SKBitmap.from
                //var myImg = ImageSource.FromFile("myImg.jpg");

                // open the stream
                var stream = new SKFileStream(image.FilePath);

                // create the codec
                var codec = SKCodec.Create(stream);

                // we need a place to store the bytes
                var bitmap = new SKBitmap(codec.Info);
                var img = SKImage.FromBitmap(bitmap);
                // decode!
                // result should be SKCodecResult.Success, but you may get more information
                //var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
                var p = new SKPoint(
                   x: (float)image.Location.X,
                   y: (float)image.Location.Y);

                canvas.DrawImage(img, p);
            }

            foreach (var circle in Circles)
            {
                canvas.DrawCircle(circle.Location, circle.Radius, paintCircleFinished);
            }

            foreach(var polyline in polylines)
            {
                DrawPolylineToCanvas(canvas, pixelWidth, polyline);
            }

            foreach (var line in lines)
            {
                DrawPolylineToCanvas(canvas, pixelWidth*3, line);
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
            Entities.Point wp = new Entities.Point(
                x: worldPoint.X,
                y: worldPoint.Y);

            CursorPoint = wp;

            // 5f is five pixels at any zoom factor
            float near = 5f / zoomFactor;
            if (mode == DrawMode.Select)
            {
                foreach (var polyline in polylines) 
                {
                    if(polyline.Polyline.IsNear(wp, near))
                    {
                        polyline.Polyline.IsSelected = !polyline.Polyline.IsSelected;
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
                polylineViewStarting = new PolylineView
                {
                    Path = new SKPath(),
                    Polyline = new Polyline()
                    {
                        Color = Color.Red,
                        Width = 3f,
                    }
                };

                polylineViewStarting.Polyline.Vertices.Add(CursorPoint);

                SetMode(DrawMode.PolylineFinish);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineFinish)
            {
                polylineViewStarting.Polyline.Vertices.Add(CursorPoint);

                viewPort.InvalidateVisual();
            }
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
            CursorPoint = new Entities.Point(
                x: worldPoint.X,
                y: worldPoint.Y);

            if (mode == DrawMode.Select)
            {
                foreach(var polyline in polylines)
                {
                    if(polyline.Polyline.IsNear(CursorPoint))
                    {
                        polyline.Polyline.IsHovered = true;
                    }
                    else
                    {
                        polyline.Polyline.IsHovered = false;
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
                coordinatesLabel.Content = $"Paths: {polylines.Count}";
            }
            else if (mode == DrawMode.PolylineFinish)
            {
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PanFinish)
            {
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

                viewPort.InvalidateVisual();
            }
            else if ((mode == DrawMode.PanFinishLive && e.MiddleButton == MouseButtonState.Released)
                || (mode == DrawMode.PanFinishLiveLeft && e.LeftButton == MouseButtonState.Released))
            {
                SetMode(previousMode);
                viewPort.InvalidateVisual();
            }

            cursorLocationLabel.Content = $"Screen: {screenPoint.X:F3},{screenPoint.Y:F3}";
            coordinatesLabel.Content = $"Coordinates: {CursorPoint.X:F3}, {CursorPoint.Y:F3}";
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
                    polylines.Add(polylineViewStarting);
                }

                SetMode(DrawMode.PolylineStart);
                viewPort.InvalidateVisual();
            }
            else if (mode == DrawMode.PolylineStart)
            {
                SetMode(DrawMode.Ready);
                viewPort.InvalidateVisual();
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

        private void zoomExtentsButton_Click(object sender, RoutedEventArgs e)
        {
            var screenWorldWidth = viewPort.ActualWidth / zoomFactor;
            var screenWorldHeight = viewPort.ActualHeight / zoomFactor;

            double leftMostX = 0.0;
            double rightMostX = 0.0;
            double topMostY = 0.0;
            double bottomMostY = 0.0;
            foreach (var polyline in polylines)
            {
                foreach(var vertex in polyline.Polyline.Vertices)
                {
                    if(leftMostX > vertex.X)
                    {
                        leftMostX = vertex.X;
                    }
                    if (rightMostX < vertex.X)
                    {
                        rightMostX = vertex.X;
                    }
                    if (topMostY < vertex.Y)
                    {
                        topMostY = vertex.Y;
                    }
                    if (bottomMostY > vertex.Y)
                    {
                        bottomMostY = vertex.Y;
                    }
                }
            }


        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "AutoCAD Drawing Exchange (DXF)|*.dxf|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    SaveDxfFile(dialog.FileName);
                }
            }
        }

        private void SaveDxfFile(string fileName)
        {
            //paper = PaperBuilder.GetPaper();
            //myViewport3D.Children.Clear();
            //myViewport3D.Children.Add(paper);

            var dxfFile = new DxfFile();
            dxfFile.Header.SetDefaults();
            dxfFile.Header.Version = DxfAcadVersion.R2000;
            dxfFile.Layers.Add(new DxfLayer("PIPES"));
            foreach (var polyline in polylines)
            {
                var vertices = new List<DxfLwPolylineVertex>();
                foreach (var v in polyline.Polyline.Vertices)
                {
                    vertices.Add(new DxfLwPolylineVertex { X = v.X, Y = v.Y });
                }

                var dxfPolyline = new DxfLwPolyline(vertices)
                {
                    LineTypeScale = 1,
                    IsClosed = false,
                    Layer = "PIPES",
                    Color = DxfColor.FromIndex(7),
                    Thickness = 3.0
                };

                dxfFile.Entities.Add(dxfPolyline);
            }

            dxfFile.ViewPorts.Clear();
            dxfFile.Save(fileName);
        }

        private void insertImageButton_Click(object sender, RoutedEventArgs e)
        {
            var location = new Entities.Point(10, -10);
            var image = new Image
            { 
                Location = location,
                FilePath = @"C:\Tests\Test.jpg"
            };

            images.Add(image);

        }
    }

    public class Circle
    {
        public SKPoint Location { get; set; }
        public float Radius { get; set; }
    }
}
