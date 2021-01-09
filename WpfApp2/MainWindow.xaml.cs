using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfApp2
{
    public class Thing
    {
        public Thing(int id, DependencyObject obj)
        {
            Id = id;
            Object = obj;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public DependencyObject Object { get; private set; }
    }

    public partial class MainWindow : Window
    {
        List<Thing> Things = new List<Thing>();

        Polyline theLine;
        public MainWindow()
        {
            InitializeComponent();
            theLine = new Polyline();
            theLine.Points.Add(
                new Point(0, 0));

            Things.Add(new Thing(1, theLine));
            myCanvas.Children.Add(theLine);
        }

        // Respond to the left mouse button down event by initiating the hit test.
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the coordinate of the mouse position.
            Point pt = e.GetPosition((UIElement)sender);
            Debug.WriteLine($"Point clicked at {pt.X}, {pt.Y}");

            // Perform the hit test against a given portion of the visual object tree.
            HitTestResult result = VisualTreeHelper.HitTest(myCanvas, pt);

            if (result != null)
            {
                // Perform action on hit visual object.
                var line = result.VisualHit as Line;
                
                if (line != null)
                {
                    line.Stroke = Brushes.Red;
                    Debug.WriteLine($"  Line was clicked");
                    Debug.WriteLine($"  Searching {Things.Count} things...");
                    foreach(var thing in Things)
                    {
                        if(ReferenceEquals(line, thing.Object))
                        {
                            Debug.WriteLine($"  --Found it Id: {thing.Id}");
                            break;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"  Not a line was clicked");
                }
            }
            else
            {
                Debug.WriteLine($"  The result was null");
                theLine.Points.Add(pt);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Add a Line Element
            var myLine = new Line();
            myLine.Stroke = Brushes.LightSteelBlue;
            myLine.X1 = 1;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            myCanvas.Children.Add(myLine);
        }
    }
}
