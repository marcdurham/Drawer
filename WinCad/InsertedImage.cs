using System.Drawing;

namespace WinCad
{
    public class InsertedImage
    {
        public InsertedImage(Image image, Rectangle rectangle)
        {
            Image = image;
            Rectangle = rectangle;
        }

        public Image Image { get; set; }
        public Rectangle Rectangle { get; set; } = Rectangle.Empty;
    }
}
