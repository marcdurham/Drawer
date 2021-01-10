using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WpfApp1
{
    public class CubeCollection
    {
        Dictionary<Visual3D, Cube> Map { get; set; } = new Dictionary<Visual3D, Cube>();

        public void Add(Visual3D visual)
        {
            Map.Add(visual, new Cube { Visual = visual });
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
        public Visual3D Visual { get; set; }
        public Point3D Point { get; set; }
    }
}
