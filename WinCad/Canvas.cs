using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class Canvas
    {
        public Canvas()
        {
            Highlights = new Layer();
            Selections = new Layer();
            CurrentLayer = new Layer();
            Layers.Add(CurrentLayer);
        }

        public List<Layer> Layers = new List<Layer>();
        public Layer CurrentLayer { get; set; }
        public Layer Highlights { get; set; }
        public Layer Selections { get; set; }
        public Point NewLineStart { get; set; } = Point.Empty;
        public Point NewLineEnd { get; set; } = Point.Empty;

        public List<Entity> Entities()
        {
            var all = new List<Entity>();
            foreach (var layer in Layers)
                all.AddRange(layer.Entities());
            return all;
        }
    }
}
