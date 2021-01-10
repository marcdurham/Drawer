using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp1
{

    public class Pipe
    {
        List<PipeSegment> segments = new List<PipeSegment>();

        public Brush Brush { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public bool Contains(PipeSegment segment)
        {
            return segments.Contains(segment);
        }

        public void Add(PipeSegment segment)
        {
            segments.Add(segment);
        }

        public void Remove(PipeSegment segment)
        {
            segments.Remove(segment);
        }

        public void SetColor(Brush brush)
        {
            foreach(var segment in segments)
            {
                segment.Model.Material = new DiffuseMaterial(brush);
                segment.Model.BackMaterial = new DiffuseMaterial(brush);
            }    
        }

        public void ResetColor()
        {
            foreach (var segment in segments)
            {
                if (IsHovered && IsSelected)
                {
                    segment.Model.Material = new DiffuseMaterial(Brushes.BlueViolet);
                    segment.Model.BackMaterial = new DiffuseMaterial(Brushes.BlueViolet);
                }
                else if (IsHovered)
                {
                    segment.Model.Material = new DiffuseMaterial(Brushes.CornflowerBlue);
                    segment.Model.BackMaterial = new DiffuseMaterial(Brushes.CornflowerBlue);
                }
                else if (IsSelected)
                {
                    segment.Model.Material = new DiffuseMaterial(Brushes.Green);
                    segment.Model.BackMaterial = new DiffuseMaterial(Brushes.Green);
                }
                else
                {
                    segment.Model.Material = new DiffuseMaterial(segment.Brush);
                    segment.Model.BackMaterial = new DiffuseMaterial(segment.Brush);
                }
            }
        }
    }
}
