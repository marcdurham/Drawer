using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Polyline : Entity
    {
        public Polyline()
        {
            Color = Color.Green;
        }

        public List<Point> Vertices = new List<Point>();
    }
}
