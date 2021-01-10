using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class PipeCollection
    {
        public List<Pipe> pipes { get; set; } = new List<Pipe>();

        public void Add(Pipe pipe)
        {
            pipes.Add(pipe);
        }

        public void Remove(Pipe pipe)
        {
            pipes.Remove(pipe);
        }

        public Pipe Get(PipeSegment segment)
        {
            foreach(var pipe in pipes)
            {
                if(pipe.Contains(segment))
                {
                    return pipe;
                }
            }

            return null;
        }

        public void ResetColor()
        {
            foreach(var pipe in pipes)
            {
                pipe.ResetColor();
            }
        }

        public void Reset()
        {
            foreach (var pipe in pipes)
            {
                pipe.ResetColor();
                pipe.IsHovered = false;
            }
        }
    }
}
