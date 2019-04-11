using netDxf;
using netDxf.Entities;
using netDxf.Objects;
using System.Collections.Generic;
using System.Linq;

namespace WinCad
{
    public class DxfFileSaver
    {
        public static void SaveAs(Canvas canvas, string file)
        {
            // By default it will create an AutoCad2000 DXF version
            var dxf = new DxfDocument();

            foreach (var layer in canvas.Layers)
            {
                foreach (var pline in layer.Polylines)
                {
                    dxf.AddEntity(LwPolylineFrom(pline));
                }

                foreach (var box in layer.Boxes)
                {
                    dxf.AddEntity(LwPolylineFrom(box));
                }

                foreach (var image in layer.InsertedImages)
                {
                    dxf.AddEntity(ImageFrom(image));
                }
            }

            dxf.Save(file);
        }

        static LwPolyline LwPolylineFrom(Polyline pline)
        {
            var vertexes = new List<Vector2>();

            foreach (var point in pline.Points())
            {
                vertexes.Add(new Vector2(point.X, point.Y));
            }

            return new LwPolyline(vertexes)
            {
                Thickness = pline.Width,
                Color = new AciColor(
                    pline.Color.R, 
                    pline.Color.G, 
                    pline.Color.B)
            };
        }

        static LwPolyline LwPolylineFrom(Box box)
        {
            var vertexes = new List<Vector2>();

            foreach (var point in box.Points())
            {
                vertexes.Add(new Vector2(point.X, point.Y));
            }

            // Connect last vertex to first, closing the polygon
            vertexes.Add(
                new Vector2(
                    x: box.Points().First().X,
                    y: box.Points().First().Y));

            return new LwPolyline(vertexes)
            {
                Thickness = box.Width,
                Color = new AciColor(box.Color.R, box.Color.G, box.Color.B)
            };
        }

        static Image ImageFrom(InsertedImage image)
        {
            return new Image(
                imageDefinition: new ImageDefinition(image.File),
                position: new Vector2(
                    x: image.Box.FirstCorner.X,
                    y: image.Box.FirstCorner.Y),
                width: image.Box.Size.Width,
                height: image.Box.Size.Height);
        }
    }
}
