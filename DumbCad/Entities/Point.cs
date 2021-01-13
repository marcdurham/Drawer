namespace DumbCad.Entities
{
    public class EmptyPoint : Point
    {
        public override bool IsEmpty => true;
    }

    public class Point
    {
        public static Point Empty => new EmptyPoint();

        public Point()
        {
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public virtual bool IsEmpty => false;

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
