using System.Collections.Generic;

namespace WinCad
{
    public class TextBox : Entity
    {
        public Point Location { get; set; }
        public string Text { get; set; } = string.Empty;

        public override List<Point> Points()
        {
            return new List<Point>() { Location };
        }
    }
}
