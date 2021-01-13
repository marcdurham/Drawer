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
        SKPaint paint = new SKPaint()
        {
            Color = SKColors.Blue
        };

        public MainWindow()
        {
            InitializeComponent();
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
            canvas.DrawCircle(new SKPoint(0f, 0f), 10.0f, paint);
        }
    }
}
