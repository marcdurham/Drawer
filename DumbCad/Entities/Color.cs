namespace DumbCad.Entities
{
    public class Color
    {
        public Color()
        {
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color Red = new Color(255, 0, 0, 255);

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
    }
}
