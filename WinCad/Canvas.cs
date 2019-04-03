﻿using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Canvas
    {
        public List<Rectangle> Rectangles { get; set; } = new List<Rectangle>();
        public List<Polyline> Polylines { get; set; } = new List<Polyline>();
        public List<InsertedImage> InsertedImages { get; set; } = new List<InsertedImage>();
        public List<Circle> Circles { get; set; } = new List<Circle>();
        public Point NewLineStart { get; set; }
        public Point NewLineEnd { get; set; }
    }
}
