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
                    var vertexes = new List<Vector2>();
                    foreach (var point in pline.Points())
                    {
                        var v = new Vector2(point.X, point.Y);
                        vertexes.Add(v);
                    }

                    var p = new LwPolyline(vertexes);
                    p.Thickness = pline.Width;
                    p.Color = new AciColor(pline.Color.R, pline.Color.G, pline.Color.B);
                    dxf.AddEntity(p);
                }

                foreach (var box in layer.Boxes)
                {
                    var vertexes = new List<Vector2>();
                    foreach (var point in box.Points())
                    {
                        var v = new Vector2(point.X, point.Y);
                        vertexes.Add(v);
                    }
                    vertexes.Add(
                        new Vector2(
                            box.Points().First().X,
                            box.Points().First().Y));

                    var p = new LwPolyline(vertexes);
                    p.Thickness = box.Width;
                    p.Color = new AciColor(box.Color.R, box.Color.G, box.Color.B);
                    dxf.AddEntity(p);
                }

                foreach (var image in layer.InsertedImages)
                {
                    var idef = new ImageDefinition(image.File);
                    var position = new Vector2(image.Box.FirstCorner.X, image.Box.FirstCorner.Y);

                    var dxfImage = new netDxf.Entities.Image(
                        idef,
                        position,
                        image.Box.Size.Height,
                        image.Box.Size.Width);

                    dxf.AddEntity(dxfImage);
                }
            }

            dxf.Save(file);
        }
    }
}
