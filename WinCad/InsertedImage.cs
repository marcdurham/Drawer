using System.Drawing;

namespace WinCad
{
    public class InsertedImage
    {
        public Image Image { get; set; }
        public Rectangle Rectangle { get; set; } = Rectangle.Empty;
    }
}
