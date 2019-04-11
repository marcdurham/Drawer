using netDxf;
using netDxf.Header;
using System.Drawing;

namespace WinCad
{
    public class DxfFileOpener
    {
        internal static Canvas OpenFile(string file)
        {
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);

            var canvas = new Canvas();

            if (dxfVersion < DxfVersion.AutoCad2000)
                return canvas;

            var loaded = DxfDocument.Load(file);

            foreach (var lwPline in loaded.LwPolylines)
            {
                var pline = new Polyline();
                foreach (var vertex in lwPline.Vertexes)
                {
                    var p = new Point((int)vertex.Position.X, (int)vertex.Position.Y);
                    pline.Vertices.Add(p);
                    pline.Color = Color.FromArgb(lwPline.Color.R, lwPline.Color.G, lwPline.Color.B);
                    pline.Width = (int)lwPline.Thickness;
                }

                canvas.CurrentLayer.Polylines.Add(pline);
            }

            foreach (var image in loaded.Images)
            {
                var img = Bitmap.FromFile(image.Definition.File);
                var insertedImage = new InsertedImage(
                    image: img,
                    box: new Box(
                        new Point((int)image.Position.X, (int)image.Position.Y),
                        new Size((int)image.Width, (int)image.Height)),
                    file: image.Definition.File);

                canvas.CurrentLayer.InsertedImages.Add(insertedImage);
            }

            return canvas;
        }
    }
}
