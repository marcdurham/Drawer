using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace WpfApp1
{
    public class CubeCollection
    {
        Dictionary<Visual3D, Cube> Map { get; set; } = new Dictionary<Visual3D, Cube>();

        public void Add(Visual3D visual)
        {
            Map.Add(visual, new Cube(visual));
        }

        public void Add(Cube cube)
        {
            Map.Add(cube.Visual, cube);
        }

        public bool Remove(Visual3D visual)
        {
            return Map.Remove(visual);
        }

        public bool Remove(Cube cube)
        {
            return Remove(cube.Visual);
        }

        public int Count { get { return Map.Count; } }
    }

    public class Cube
    {
        public Cube(Visual3D visual)
        {
            Visual = visual;
        }

        public Visual3D Visual { get; set; }
        public Point3D Point { get; set; }
    }
}
