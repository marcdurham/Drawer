using sys = System.Drawing;

namespace WinCad
{
    public struct Offset
    {
        public Offset(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public static implicit operator sys.Point(Offset p)
            => new sys.Point(p.X, p.Y);

        public static implicit operator Offset(sys.Point p)
            => new Offset(p.X, p.Y);

        public override bool Equals(object obj)
        {
            var offset = (Offset)obj;

            return offset.X == X && offset.Y == Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static bool operator ==(Offset left, Offset right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Offset left, Offset right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
