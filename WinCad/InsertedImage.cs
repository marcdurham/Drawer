using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class InsertedImage : Entity
    {
        public InsertedImage(Image image, Box box, string file)
        {
            Image = image;
            File = file;
            Box = box;
        }

        public Image Image { get; set; }
        public string File { get; set; }

        public Box Box { get; }

        public override List<Point> Points()
        {
            return Box.Points();
        }
    }
}
