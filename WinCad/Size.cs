using sys = System.Drawing;

namespace WinCad
{
    public struct Size
    {
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width;
        public int Height;

        public static implicit operator sys.Size(Size s)
            => new sys.Size(s.Width, s.Height);

        public static implicit operator Size(sys.Size s)
            => new Size(s.Width, s.Height);

        public override bool Equals(object obj)
        {
            var size = (Size)obj;

            return size.Width == Width && size.Height == Height;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() + Height.GetHashCode();
        }

        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{Width},{Height}";
        }
    }
}
