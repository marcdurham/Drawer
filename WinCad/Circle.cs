﻿using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Grip : Entity
    {
        public Grip()
        {
            Color = Color.Turquoise;
        }

        public Point Center { get; set; }
        public int Radius { get; set; }

        public override List<Point> Points()
        {
            return new List<Point>() { Center };
        }
    }
}