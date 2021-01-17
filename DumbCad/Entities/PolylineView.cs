using SkiaSharp;

namespace DumbCad.Entities
{
    public class PolylineView 
    {
        public Polyline Polyline { get; set; }
        public SKPath Path { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }
    }
}
