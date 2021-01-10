using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp1
{
    public class PipeSegment
    {
        public PipeSegment(Visual3D visual, GeometryModel3D model)
        {
            Visual = visual;
            Model = model;
        }

        public Visual3D Visual { get; set; }
        public GeometryModel3D Model { get; set; }
        public Point3D Point { get; set; }
        public Brush Brush { get; set; }
    }

}
