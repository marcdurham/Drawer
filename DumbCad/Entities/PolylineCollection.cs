using System.Collections;
using System.Collections.Generic;

namespace DumbCad.Entities
{
    public class PolylineCollection : IEnumerable<Polyline>
    {
        List<Polyline> polylines = new List<Polyline>();

        public IEnumerator<Polyline> GetEnumerator()
        {
            return polylines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return polylines.GetEnumerator();
        }

        public int Count { get { return polylines.Count; } }

        public void Add(Polyline polyline)
        {
            polylines.Add(polyline);
        }

        public void Add(object visual)
        {
            var polyline = new Polyline
            {
                Visual = visual,
            };

            polylines.Add(polyline);
        }
    }
}
