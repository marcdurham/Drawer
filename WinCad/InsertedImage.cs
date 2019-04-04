using System.Collections.Generic;
using System.Drawing;

namespace WinCad
{
    public class InsertedImage : Entity
    {
        public InsertedImage(Image image, Box box)
        {
            Image = image;
            Box = box;
        }

        public Image Image { get; set; }

        public Box Box { get; }

        public override List<Point> Points()
        {
            return Box.Points();
        }
    }
}
