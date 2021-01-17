using SkiaSharp;
using System.Collections;
using System.Collections.Generic;

namespace DumbCad.Entities
{
    public class PolylineViewCollection : IEnumerable<PolylineView>
    {
        List<PolylineView> polylines = new List<PolylineView>();

        public IEnumerator<PolylineView> GetEnumerator()
        {
            return polylines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return polylines.GetEnumerator();
        }

        public int Count { get { return polylines.Count; } }

        public void Add(PolylineView polyline)
        {
            polylines.Add(polyline);
        }

        public void Add(SKPath path)
        {
            var polyline = new PolylineView
            {
                Path = path,
            };

            polylines.Add(polyline);
        }
    }
}
