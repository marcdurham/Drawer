using System;
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

        public void Delete(Entity entity)
        {
            if (entity is Box && Boxes.Contains((Box)entity))
                Boxes.Remove((Box)entity);
            if (entity is Polyline && Polylines.Contains((Polyline)entity))
                Polylines.Remove((Polyline)entity);
            if (entity is InsertedImage && InsertedImages.Contains((InsertedImage)entity))
                InsertedImages.Remove((InsertedImage)entity);
            if (entity is Circle && Circles.Contains((Circle)entity))
                Circles.Remove((Circle)entity);
        }

        internal void Clear()
        {
            Boxes.Clear();
            Polylines.Clear();
            InsertedImages.Clear();
            Circles.Clear();
        }
    }
}
