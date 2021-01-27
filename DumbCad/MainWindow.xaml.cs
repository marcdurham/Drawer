using DumbCad.Entities;
using DumbCad.Views;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using Microsoft.Win32;
using SkiaSharp;
using System.Collections.Generic;
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
        readonly CanvasPainter painter = new CanvasPainter();
        readonly Viewer viewer = new Viewer();

        ////DrawMode mode = DrawMode.Ready;
        ////DrawMode previousMode = DrawMode.Ready;
        ////float zoomFactor = 1f;
        DrawingFile file = new DrawingFile();
      
        //Circle circleStarting = new Circle();
        //SKPoint circleFinishingPoint = new SKPoint();
        //PolylineView polylineViewStarting = new PolylineView();
        ////SKPoint panStart = new SKPoint();
        ////SKPoint panOffset = new SKPoint();
        string insertImagePath = string.Empty;

        //public Entities.Point CursorPoint { get; private set; }

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
                    file.Open(dialog.FileName);
                }

                viewPort.InvalidateVisual();
            }
        }

        private void viewPort_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            painter.Paint(canvas, file, viewer);
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

            painter.CursorPoint = wp;

            // 5f is five pixels at any zoom factor
            float near = 5f / viewer.zoomFactor;
            if (viewer.mode == DrawMode.Select)
            {
                foreach (var polyline in file.polylines) 
                {
                    if(polyline.Polyline.IsNear(wp, near))
                    {
                        polyline.Polyline.IsSelected = !polyline.Polyline.IsSelected;
                    }
                }

                foreach(var image in file.images)
                {
                    if(image.Image.IsNear(wp, near))
                    {
                        image.Image.IsSelected = !image.Image.IsSelected;
                    }
                }

                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.CircleStart)
            {
                painter.circleStarting = new Circle()
                {
                    Location = worldPoint
                };

                SetMode(DrawMode.CircleFinish);
                viewPort.Cursor = Cursors.Cross;
            }
            else if (viewer.mode == DrawMode.CircleFinish)
            {
                if (painter.circleStarting.Radius > 0)
                {
                    file.Circles.Add(painter.circleStarting);
                }

                painter.circleStarting = null;

                SetMode(DrawMode.CircleStart);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PolylineStart)
            {
                painter.polylineViewStarting = new PolylineView
                {
                    Path = new SKPath(),
                    Polyline = new Polyline()
                    {
                        Color = Color.Red,
                        Width = 3f,
                    }
                };

                painter.polylineViewStarting.Polyline.Vertices.Add(painter.CursorPoint);

                SetMode(DrawMode.PolylineFinish);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PolylineFinish)
            {
                painter.polylineViewStarting.Polyline.Vertices.Add(painter.CursorPoint);

                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PanStartLive)
            {
                viewer.panStart = WorldOffsetFrom(point);
                SetMode(DrawMode.PanFinishLive);
                panOffsetLabel.Content = $"Move mouse";
                startPointLabel.Content = $"StPt: { viewer.panStart.X:F2}, { viewer.panStart.Y:F2}";
                viewPort.Cursor = Cursors.Hand;
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PanFinishLive)
            {
                SetMode(DrawMode.PanStartLive);
                panOffsetLabel.Content = $"Click start point";
                startPointLabel.Content = $"StPt: { viewer.panStart.X:F2}, { viewer.panStart.Y:F2}";
                viewPort.Cursor = Cursors.SizeAll;
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.InsertImagePoint)
            {
                var image = new Image
                {
                    Location = painter.CursorPoint,
                    FilePath = insertImagePath
                };

                SKImage skImage = null;
                using (var stream = new SKFileStream(image.FilePath))
                {
                    stream.Seek(0);
                    var bitmap = SKBitmap.Decode(stream);
                    skImage = SKImage.FromBitmap(bitmap);
                }

                image.Width = skImage.Width;
                image.Height = skImage.Height;

                var view = new ImageView
                {
                    Image = image,
                    SkImage = skImage
                };

                file.images.Add(view);
                SetMode(DrawMode.Ready);

                viewPort.InvalidateVisual();
            }
        }

        private SKPoint WorldOffsetFrom(Point screenPoint)
        {
            float zoomFactorNoZero = viewer.zoomFactor == 0 ? 1f : viewer.zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero),
                y: (float)-(screenPoint.Y / zoomFactorNoZero));
        }

        private SKPoint WorldPointFrom(Point screenPoint)
        {
            float zoomFactorNoZero = viewer.zoomFactor == 0 ? 1f : viewer.zoomFactor;

            return new SKPoint(
                x: (float)(screenPoint.X / zoomFactorNoZero) - viewer.panOffset.X, 
                y: (float)-(screenPoint.Y / zoomFactorNoZero) - viewer.panOffset.Y);
        }

        private void drawCircleButton_Click(object sender, RoutedEventArgs e)
        {
            SetMode(DrawMode.CircleStart);
        }

        void SetMode(DrawMode mode)
        {
            viewer.mode = mode;
            this.modeLabel.Content = mode.ToString();
        }

        private void viewPort_MouseMove(object sender, MouseEventArgs e)
        {
            var screenPoint = e.GetPosition(viewPort);
            var worldPoint = WorldPointFrom(screenPoint);
            painter.CursorPoint = new Entities.Point(
                x: worldPoint.X,
                y: worldPoint.Y);

            if (viewer.mode == DrawMode.Select)
            {
                foreach(var polyline in file.polylines)
                {
                    if(polyline.Polyline.IsNear(painter.CursorPoint))
                    {
                        polyline.Polyline.IsHovered = true;
                    }
                    else
                    {
                        polyline.Polyline.IsHovered = false;
                    }
                }

                foreach(var image in file.images)
                {
                    if(image.Image.IsNear(painter.CursorPoint))
                    {
                        image.Image.IsHovered = true;
                    }
                    else
                    {
                        image.Image.IsHovered = false;
                    }
                }

                viewPort.InvalidateVisual();
            }

            if (viewer.mode == DrawMode.CircleFinish && painter.circleStarting != null)
            {
                painter.circleFinishingPoint = worldPoint;
                float radius = (float)Geometry.Distance(painter.circleStarting.Location, painter.circleFinishingPoint);

                painter.circleStarting.Radius = radius;
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PolylineStart)
            {
                coordinatesLabel.Content = $"Paths: {file.polylines.Count}";
            }
            else if (viewer.mode == DrawMode.PolylineFinish)
            {
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PanFinish)
            {
                viewPort.InvalidateVisual();
            }
            else if((viewer.mode == DrawMode.PanFinishLive && e.MiddleButton == MouseButtonState.Pressed)
                || (viewer.mode == DrawMode.PanFinishLiveLeft && e.LeftButton == MouseButtonState.Pressed))
            {
                var p = WorldOffsetFrom(screenPoint);
                var newOffset = new SKPoint(
                   x: p.X - viewer.panStart.X,
                   y: p.Y - viewer.panStart.Y);

                viewer.panStart = p;

                viewer.panOffset = new SKPoint(
                   x: viewer.panOffset.X + newOffset.X,
                   y: viewer.panOffset.Y + newOffset.Y);

                viewPort.InvalidateVisual();
            }
            else if ((viewer.mode == DrawMode.PanFinishLive && e.MiddleButton == MouseButtonState.Released)
                || (viewer.mode == DrawMode.PanFinishLiveLeft && e.LeftButton == MouseButtonState.Released))
            {
                SetMode(viewer.previousMode);
                viewPort.InvalidateVisual();
            }

            cursorLocationLabel.Content = $"Screen: {screenPoint.X:F3},{screenPoint.Y:F3}";
            coordinatesLabel.Content = $"Coordinates: {painter.CursorPoint.X:F3}, {painter.CursorPoint.Y:F3}";
        }

        private void viewPort_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (viewer.mode == DrawMode.CircleFinish)
            {
                SetMode(DrawMode.CircleStart);
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.CircleStart)
            {
                SetMode(DrawMode.Ready);
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PolylineFinish)
            {
                if (painter.polylineViewStarting.Polyline.Vertices.Count > 0)
                {
                    file.polylines.Add(painter.polylineViewStarting);
                }

                SetMode(DrawMode.PolylineStart);
                viewPort.InvalidateVisual();
            }
            else if (viewer.mode == DrawMode.PolylineStart)
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
            viewer.zoomFactor *= 2f;
            zoomFactorLabel.Content = $"Z:{ viewer.zoomFactor:F4}";
            viewPort.InvalidateVisual();
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            viewer.zoomFactor *= 0.5f;
            zoomFactorLabel.Content = $"Z:{ viewer.zoomFactor:F4}";
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
                viewer.previousMode = viewer.mode;
                SetMode(DrawMode.PanFinishLive);
                viewer.panStart = WorldOffsetFrom(e.GetPosition(viewPort));
                viewPort.Cursor = Cursors.SizeAll;
                viewPort.InvalidateVisual();
            }
            else if (e.LeftButton == MouseButtonState.Pressed && viewer.mode == DrawMode.PanStart)
            {
                viewer.previousMode = viewer.mode;
                SetMode(DrawMode.PanFinishLiveLeft);
                viewer.panStart = WorldOffsetFrom(e.GetPosition(viewPort));
                viewPort.Cursor = Cursors.SizeAll;
                viewPort.InvalidateVisual();
            }
        }

        private void viewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Released && viewer.mode == DrawMode.PanFinishLive)
            {
                SetMode(viewer.previousMode);
                viewPort.Cursor = Cursors.Cross;
                viewPort.InvalidateVisual();
            }
            else if (e.LeftButton == MouseButtonState.Released && viewer.mode == DrawMode.PanFinishLive)
            {
                SetMode(viewer.previousMode);
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
            viewer.zoomFactor = viewer.zoomFactor * adjustment;
            zoomFactorLabel.Content = $"Z:{ viewer.zoomFactor:F4}";

            viewer.panOffset = new SKPoint(
                x: (float)(screenPoint.X / viewer.zoomFactor) - worldPoint.X,
                y: (float)-(screenPoint.Y / viewer.zoomFactor) - worldPoint.Y);

            viewPort.InvalidateVisual();
        }

        private void zoomResetButton_Click(object sender, RoutedEventArgs e)
        {
            viewer.zoomFactor = 1.0f;
            viewer.panOffset = new SKPoint();
            viewPort.InvalidateVisual();
        }

        private void zoomExtentsButton_Click(object sender, RoutedEventArgs e)
        {
            var screenWorldWidth = viewPort.ActualWidth / viewer.zoomFactor;
            var screenWorldHeight = viewPort.ActualHeight / viewer.zoomFactor;

            double leftMostX = 0.0;
            double rightMostX = 0.0;
            double topMostY = 0.0;
            double bottomMostY = 0.0;
            foreach (var polyline in file.polylines)
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
            foreach (var polyline in file.polylines)
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

            foreach(var image in file.images)
            {
                var dxfPoint = new DxfPoint(
                    x: image.Image.Location.X,
                    y: image.Image.Location.Y,
                    z: 0);

                var dxfImage = new DxfImage(image.Image.FilePath,
                    location: dxfPoint,
                    imageWidth: (int)image.Image.Width,
                    imageHeight: (int)image.Image.Height,
                    displaySize: new DxfVector(1, 1, 1));

                dxfFile.Entities.Add(dxfImage);
            }

            dxfFile.ViewPorts.Clear();
            dxfFile.Save(fileName);
        }

        private void insertImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JPEG Files (*.jpg)|*.jpg|All Files (*.*)|*.*"
            };

            string path = string.Empty;
            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    path = dialog.FileName;
                }
            }

            if(string.IsNullOrWhiteSpace(path))
            {
                SetMode(DrawMode.Ready);
                return;
            }

            insertImagePath = path;

            SetMode(DrawMode.InsertImagePoint);
        }
    }

    public class Circle
    {
        public SKPoint Location { get; set; }
        public float Radius { get; set; }
    }
}
