using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DumbCad
{
    public class Viewer
    {
        public DrawMode mode = DrawMode.Ready;
        public DrawMode previousMode = DrawMode.Ready;
        public float zoomFactor = 1f;
        public SKPoint panStart = new SKPoint();
        public SKPoint panOffset = new SKPoint();
    }
}
