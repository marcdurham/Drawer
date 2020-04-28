using sys = System.Drawing;

namespace WinCad
{
    public struct Pixel
    {
        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int X;
        public int Y;

        public static implicit operator sys.Point(Pixel p) 
            => new sys.Point(p.X, p.Y);

        public static implicit operator Pixel(sys.Point p) 
            => new Pixel(p.X, p.Y);

        public override bool Equals(object obj)
        {
            var pixel = (Pixel)obj;
            
            return pixel.X == X && pixel.Y == Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static bool operator ==(Pixel left, Pixel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pixel left, Pixel right)
        {
            return !(left == right);
        }
    }
}
