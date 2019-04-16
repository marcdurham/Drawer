using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class InsertedImage : Entity
    {
        public InsertedImage(Box box, string file)
        {
            File = file;
            Box = box;
        }

        public string File { get; set; }

        public Box Box { get; }

        public override List<Point> Points()
        {
            return Box.Points();
        }
    }
}
