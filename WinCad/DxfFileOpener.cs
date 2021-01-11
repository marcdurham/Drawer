using netDxf;
using netDxf.Entities;
using netDxf.Header;
using System;
using Color = System.Drawing.Color;
using Bitmap = System.Drawing.Bitmap;
using DxfImage = netDxf.Entities.Image;

namespace WinCad
{
    public class DxfFileOpener
    {
        public static Canvas OpenFile(string file)
        {
            var canvas = new Canvas();

            DxfVersion dxfVersion = DxfDocument
                .CheckDxfFileVersion(file, out bool isBinary);

            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                return canvas;
            }

            var loaded = DxfDocument.Load(file);

            foreach (var lwPline in loaded.LwPolylines)
            {
                canvas.CurrentLayer.Polylines.Add(PolylineFrom(lwPline));
            }

            foreach (var image in loaded.Images)
            {
                canvas.CurrentLayer.InsertedImages.Add(
                    InsertedImageFrom(image));
            }

            foreach (var box in loaded.Texts)
            {
                canvas.CurrentLayer.TextBoxes.Add(
                    TextBoxFrom(box));
            }

            return canvas;
        }

        static Polyline PolylineFrom(LwPolyline lwPolyline)
        {
            var polyline = new Polyline
            {
                Color = Color.FromArgb(
                    red: lwPolyline.Color.R, 
                    green: lwPolyline.Color.G, 
                    blue: lwPolyline.Color.B),
                Width = (int)lwPolyline.Thickness
            };

            foreach (var vertex in lwPolyline.Vertexes)
            {
                polyline.Vertices.Add(PointFrom(vertex.Position));
            }

            return polyline;
        }

        static InsertedImage InsertedImageFrom(DxfImage image)
        {
            return new InsertedImage(
                box: new Box(
                    firstCorner: PointFrom(image.Position),
                    size: new Size((int)image.Width, (int)image.Height)),
                file: image.Definition.File);
        }

        static TextBox TextBoxFrom(Text text)
        {
            return new TextBox()
            {
                Text = text.Value,
                Location = PointFrom(text.Position)
            };
        }

        static Point PointFrom(Vector2 vector)
        {
            // Y is positive in the up direction in a DXF file
            // but Y is postive in a down direction on the screen
            return new Point(vector.X, -vector.Y);
        }

        static Point PointFrom(Vector3 vector)
        {
            // Y is positive in the up direction in a DXF file
            // but Y is postive in a down direction on the screen
            return new Point(vector.X, -vector.Y);
        }
    }
}
