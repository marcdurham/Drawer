using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public abstract class Entity
    {
        public Color Color { get; set; }
        public bool IsSelected { get; set; }

        public abstract List<Point> Points();
    }
}
