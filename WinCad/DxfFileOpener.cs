using netDxf;
using netDxf.Entities;
using netDxf.Header;
using System;
using Color = System.Drawing.Color;
using Bitmap = System.Drawing.Bitmap;
using SysImage = System.Drawing.Image;
using DxfImage = netDxf.Entities.Image;

namespace WinCad
{
    public class DxfFileOpener
    {
        internal static Canvas OpenFile(string file)
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
                polyline.Vertices.Add(
                    new Point((int)vertex.Position.X, (int)vertex.Position.Y));
            }

            return polyline;
        }

        static InsertedImage InsertedImageFrom(DxfImage image)
        {
            return new InsertedImage(
                image: FromFile(image.Definition.File),
                box: new Box(
                    firstCorner: new Point(
                        x: (int)image.Position.X,
                        y: (int)image.Position.Y),
                    size: new Size((int)image.Width, (int)image.Height)),
                file: image.Definition.File);
        }

        static SysImage FromFile(string file)
        {
            try
            {
                return Bitmap.FromFile(file);
            }
            catch (Exception)
            {
                return new Bitmap(10, 10);
            }
        }
    }
}
