using System.Drawing;

namespace WinCad
{
    public class DrawingSession
    {
        public DrawModes Mode { get; set; } = DrawModes.Ready;
        public Point FirstCorner { get; set; } = Point.Empty;
        public Point SecondCorner { get; set; } = Point.Empty;
        public Polyline CurrentPolyline { get; set; } = null;
        public Canvas Canvas { get; set; } = new Canvas();
        public string OpenInsertPictureFileName { get; set; }
        public string FileName { get; set; }
        public double ZoomFactor { get; set; } = 1.0;
        public System.Drawing.Point PanningOffset { get; set; } = new System.Drawing.Point(0, 0);
        public System.Drawing.Point StartPanningFrom { get; set; } = System.Drawing.Point.Empty;
        public System.Drawing.Point EndPanningAt { get; set; } = System.Drawing.Point.Empty;
    }
}
