using DumbCad.Views;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using SkiaSharp;
using System.Collections.Generic;

namespace DumbCad.Entities
{
    public class DrawingFile
    {
        public PolylineViewCollection polylines = new PolylineViewCollection();
        public PolylineViewCollection lines = new PolylineViewCollection();
        public List<ImageView> images = new List<ImageView>();
        public List<Circle> Circles = new List<Circle>();

        public void Open(string fileName)
        {
            //paper = PaperBuilder.GetPaper();
            //myViewport3D.Children.Clear();
            //myViewport3D.Children.Add(paper);

            var dxfFile = DxfFile.Load(fileName);

            this.polylines.Clear();
            this.images.Clear();

            foreach (DxfEntity entity in dxfFile.Entities)
            {
                //if (entity.Layer.ToUpper().StartsWith("PIPE"))
                {
                    switch (entity.EntityType)
                    {
                        case DxfEntityType.Image:
                            var dxfImage = (DxfImage)entity;
                            // TODO: Load images
                            var location = new Entities.Point(
                                x: dxfImage.Location.X,
                                y: dxfImage.Location.Y);

                            var image = new Image
                            {
                                Location = location,
                                FilePath = dxfImage.ImageDefinition.FilePath
                            };

                            SKImage skImage = null;
                            using (var stream = new SKFileStream(image.FilePath))
                            {
                                stream.Seek(0);
                                var bitmap = SKBitmap.Decode(stream);
                                skImage = SKImage.FromBitmap(bitmap);
                            }

                            image.Width = skImage.Width;
                            image.Height = skImage.Height;

                            var imageView = new ImageView
                            {
                                Image = image,
                                SkImage = skImage
                            };

                            images.Add(imageView);
                            break;
                        case DxfEntityType.Line:
                            var dxfLine = (DxfLine)entity;
                            var line = new Polyline
                            {
                                Color = Color.Green,
                                Width = 2f
                            };

                            var p1 = new Entities.Point(
                                dxfLine.P1.X,
                                dxfLine.P1.Y);

                            var p2 = new Entities.Point(
                                dxfLine.P2.X,
                                dxfLine.P2.Y);

                            line.Vertices.Add(p1);
                            line.Vertices.Add(p2);

                            var lineView = new PolylineView
                            {
                                Polyline = line,
                                Path = new SKPath()
                            };

                            lines.Add(lineView);
                            break;
                        case DxfEntityType.Polyline:
                            var dxfPolyline = (DxfPolyline)entity;
                            var poly = new Polyline
                            {
                                Color = Color.Red,
                                Width = 2f
                            };

                            foreach (var v in dxfPolyline.Vertices)
                            {
                                var vertex = new Entities.Point(
                                    v.Location.X,
                                    v.Location.Y);

                                poly.Vertices.Add(vertex);
                            }

                            var polyView = new PolylineView
                            {
                                Polyline = poly,
                                Path = new SKPath()
                            };

                            polylines.Add(polyView);
                            break;
                        case DxfEntityType.LwPolyline:
                            var dxfLwPolyline = (DxfLwPolyline)entity;
                            var poly2 = new Polyline
                            {
                                Color = Color.Red,
                                Width = 3f
                            };

                            foreach (var v in dxfLwPolyline.Vertices)
                            {
                                var vertex = new Entities.Point(v.X, v.Y);

                                poly2.Vertices.Add(vertex);
                            }

                            var polyView2 = new PolylineView
                            {
                                Polyline = poly2,
                                Path = new SKPath()
                            };

                            polylines.Add(polyView2);
                            break;
                    }
                }
            }
        }
    }
}
