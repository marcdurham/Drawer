using System.Collections.Generic;

namespace WinCad
{
    public class Layer
    {
        public List<Box> Boxes { get; set; } = new List<Box>();
        public List<Polyline> Polylines { get; set; } = new List<Polyline>();
        public List<InsertedImage> InsertedImages { get; set; } = new List<InsertedImage>();
        public List<Circle> Circles { get; set; } = new List<Circle>();

        public List<Entity> Entities()
        {
            var list = new List<Entity>();

            list.AddRange(Boxes);
            list.AddRange(Polylines);
            list.AddRange(InsertedImages);
            list.AddRange(Circles);

            return list;
        }
    }
}
