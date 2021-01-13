using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DumbCad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DrawMode mode = DrawMode.Ready;

        SKPaint paintCircleFinished = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Brown
        };

        SKPaint paintCircleStarting = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue
        };

        List<Circle> Circles = new List<Circle>();
        Circle circleStarting = new Circle();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            Circles.Add(new Circle { Location = new SKPoint(x: 1f, y: 2f), Radius = 20f });

            viewPort.InvalidateVisual();
        }

        private void viewPort_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ff")}: Painting Surface...");
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Beige);

            foreach (var circle in Circles)
            {
                canvas.DrawCircle(circle.Location, circle.Radius, paintCircleFinished);
            }

            if(mode == DrawMode.CircleFinish && circleStarting != null)
            {
                canvas.DrawCircle(circleStarting.Location, circleStarting.Radius, paintCircleStarting);
            }
        }

        private void viewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void viewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mode == DrawMode.CircleStart)
            {
                var point = e.GetPosition(viewPort);

                circleStarting = new Circle()
                {
                    Location = new SKPoint((float)point.X, (float)point.Y)
                };

                SetMode(DrawMode.CircleFinish);

                viewPort.InvalidateVisual();
            }
            else if(mode == DrawMode.CircleFinish)
            {
                var point = e.GetPosition(viewPort);
                if (circleStarting.Radius > 0)
                {
                    Circles.Add(circleStarting);
                }

                circleStarting = null;

                SetMode(DrawMode.CircleStart);

                viewPort.InvalidateVisual();
            }
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
            if(mode == DrawMode.CircleFinish && circleStarting != null)
            {
                var point = e.GetPosition(viewPort);
                float radius = (float)Math.Sqrt(
                    Math.Pow(circleStarting.Location.X - point.X, 2) +
                    Math.Pow(circleStarting.Location.Y - point.Y, 2));


                circleStarting.Radius = radius;
                viewPort.InvalidateVisual();
            }
        }
    }

    public class Circle
    {
        public SKPoint Location { get; set; }
        public float Radius { get; set; }
    }
}
