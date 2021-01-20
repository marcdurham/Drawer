using System.Collections.Generic;

namespace DumbCad.Entities
{
    public abstract class Entity
    {
        public Color Color { get; set; }
        public float Width { get; set; } = 3.0f;
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public abstract List<Point> Points();
    }
}
