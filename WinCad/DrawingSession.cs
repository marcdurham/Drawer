using System.Drawing;

namespace WinCad
{
    public class DrawingSession
    {
        public DrawModes Mode { get; set; } = DrawModes.Ready;
        public Point FirstCorner { get; set; } = Point.Empty;
        public Point SecondCorner { get; set; } = Point.Empty;
        public Polyline CurrentPolyline { get; set; } = null;
        public Rectangle CurrentRectangle { get; set; } = Rectangle.Empty;
        public Canvas Canvas { get; set; } = new Canvas();
        public string OpenFileName { get; internal set; }
    }
}
