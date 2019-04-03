using System.Drawing;

namespace WinCad
{
    public class Box : Entity
    {
        public Box()
        {
            Color = Color.Purple;
        }

        public Box(Point firstCorner, Size size)
        {
            FirstCorner = firstCorner;
            Size = size;
        }

        public Point FirstCorner { get; set; }
        public Size Size { get; set; }
    }
}
