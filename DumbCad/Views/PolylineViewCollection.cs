using DumbCad.Entities;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DumbCad.Views
{
    public class PolylineViewCollection : IEnumerable<PolylineView>
    {
        List<PolylineView> polylines = new List<PolylineView>();
        Dictionary<SKPath, PolylineView> map = new Dictionary<SKPath, PolylineView>();

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
            if (polyline.Path == null)
            {
                throw new ArgumentNullException(nameof(polyline.Path));
            }

            if (map.ContainsKey(polyline.Path))
            {
                throw new Exception($"{nameof(Polyline)} key ({nameof(SKPath)}) already exists!");
            }

            polylines.Add(polyline);
            map.Add(polyline.Path, polyline);
        }

        public void Add(SKPath path)
        {
            var polyline = new PolylineView
            {
                Path = path,
                Polyline = new Polyline() {  Color = Color.Red }
            };

            Add(polyline);
        }

        public Polyline GetPolyline(SKPath path)
        {
            return map[path].Polyline;
        }

        public void Select(SKPath path) 
        {
            map[path].IsSelected = !map[path].IsSelected;
        }

        internal void Clear()
        {
            polylines.Clear();
            map.Clear();
        }
    }
}
