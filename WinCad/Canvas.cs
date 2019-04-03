using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Canvas
    {
        public Canvas()
        {
            TemporaryLayer = new Layer();
            CurrentLayer = new Layer();
            Layers.Add(CurrentLayer);
        }

        public List<Layer> Layers = new List<Layer>();
        public Layer CurrentLayer { get; set; }
        public Layer TemporaryLayer { get; set; }
        public Point NewLineStart { get; set; } = Point.Empty;
        public Point NewLineEnd { get; set; } = Point.Empty;
    }
}
