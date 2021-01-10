using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace WpfApp1
{
    public class PipeSegmentCollection
    {
        public Dictionary<Visual3D, PipeSegment> Map { get; set; } = new Dictionary<Visual3D, PipeSegment>();

        public void Add(Visual3D visual)
        {
            Map.Add(visual, new PipeSegment(visual));
        }

        public void Add(PipeSegment cube)
        {
            Map.Add(cube.Visual, cube);
        }

        public bool Remove(Visual3D visual)
        {
            return Map.Remove(visual);
        }

        public bool Remove(PipeSegment cube)
        {
            return Remove(cube.Visual);
        }

        public int Count { get { return Map.Count; } }

        public bool Contains(Visual3D visual)
        {
            return Map.ContainsKey(visual);
        }
    }

    public class PipeSegment
    {
        public PipeSegment(Visual3D visual)
        {
            Visual = visual;
        }

        public Visual3D Visual { get; set; }
        public Point3D Point { get; set; }
    }
}
