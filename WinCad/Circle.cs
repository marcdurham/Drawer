using System.Drawing;

namespace WinCad
{
    public class Circle : Entity
    {
        public Circle()
        {
            Color = Color.Turquoise;
        }

        public Point Center { get; set; }
        public int Radius { get; set; }
    }
}