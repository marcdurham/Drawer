using netDxf;
using netDxf.Entities;
using netDxf.Objects;
using System;
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
                    try
                    {
                        dxf.AddEntity(ImageFrom(image));
                    }
                    catch (Exception)
                    {
                        dxf.AddEntity(TextFrom(image));
                    }
                }
            }

            dxf.Save(file);
        }

        static LwPolyline LwPolylineFrom(Polyline pline)
        {
            var vertexes = new List<Vector2>();

            foreach (var point in pline.Points())
            {
                vertexes.Add(Vector2From(point));
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
                vertexes.Add(Vector2From(point));
            }

            // Connect last vertex to first, closing the polygon
            vertexes.Add(Vector2From(box.Points().First()));

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
                position: Vector2From(image.Box.FirstCorner),
                width: image.Box.Size.Width,
                height: image.Box.Size.Height);
        }

        static Text TextFrom(InsertedImage image)
        {
            var text = new Text(
                text: $"Missing Image File: {image.File}",
                position: Vector2From(image.Box.FirstCorner),
                height: 10.0f);

            text.Layer = new netDxf.Tables.Layer("Missing Images");

            return text;
        }

        static Vector2 Vector2From(Point point)
        {
            // Y is positive in the up direction in a DXF file
            // but Y is postive in a down direction on the screen
            return new Vector2(point.X, -point.Y);
        }
    }
}
