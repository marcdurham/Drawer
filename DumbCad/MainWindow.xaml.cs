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

        SKPaint paint = new SKPaint()
        {
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
                canvas.DrawCircle(circle.Location, circle.Radius, paint);
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

                mode = DrawMode.CircleFinish;

                viewPort.InvalidateVisual();
            }
            else if(mode == DrawMode.CircleFinish)
            {
                var point = e.GetPosition(viewPort);
                circleStarting.Radius = 30f;
                Circles.Add(circleStarting);
                circleStarting = null;

                mode = DrawMode.CircleStart;

                viewPort.InvalidateVisual();
            }
        }

        private void drawCircleButton_Click(object sender, RoutedEventArgs e)
        {
            mode = DrawMode.CircleStart;
        }
    }

    public class Circle
    {
        public SKPoint Location { get; set; }
        public float Radius { get; set; }
    }
}
