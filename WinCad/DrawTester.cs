﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinCad
{
    public partial class DrawTester : Form, IDrawingView
    {
        readonly DrawingController controller;
        
        public DrawTester()
        {
            InitializeComponent();

            controller = new DrawingController(this);
        }
    
        public string Status
        {
            set { mainStatus.Text = value; }
        }

        public Canvas Canvas { get; set; }

        private void drawPolylineButton_Click(object sender, EventArgs e)
        {
            controller.DrawPolyline();
            DrawLayers();
        }

        private void importPictureButton_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
                controller.ImportPicture(openFileDialog.FileName);
        }

        private void drawRectangle_Click(object sender, EventArgs e)
        {
            controller.DrawRectangle();
        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            DrawObjectsOn(g, Canvas.Highlights);

            if (controller.session.CurrentPolyline != null)
                g.DrawLine(Pens.Blue, Canvas.NewLineStart, Canvas.NewLineEnd);
        }

        private static void DrawObjectsOn(Graphics g, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
                g.DrawImage(image.Image, image.Rectangle);

            foreach (var r in layer.Rectangles)
                g.DrawRectangle(Pens.Green, r);

            foreach (var p in layer.Polylines)
                if (p.Vertices.Count > 1)
                    for (int i = 1; i < p.Vertices.Count; i++)
                        g.DrawLine(Pens.Green, p.Vertices[i - 1], p.Vertices[i]);

            foreach (var circle in layer.Circles)
            {
                var corner = new Point(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius);

                int side = circle.Radius * 2;

                g.DrawEllipse(Pens.Red, corner.X, corner.Y, side, side);
            }
        }

        private void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                controller.ClickAt(new Point(e.X, e.Y), e.Button != MouseButtons.Left);

                DrawLayers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Clicking On Picture");
            }
        }

        private void DrawLayers()
        {
            var image = new Bitmap(mainPicture.Width, mainPicture.Height);

            var graphics = Graphics.FromImage(image);

            foreach (var layer in Canvas.Layers)
                DrawObjectsOn(graphics, layer);

            mainPicture.Image = image;

            mainPicture.Invalidate();
        }

        private void mainPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (controller.session.CurrentPolyline?.Vertices?.Count > 0)
            {
                Canvas.NewLineStart = controller.session.CurrentPolyline.Vertices.Last();
                Canvas.NewLineEnd = e.Location;
            }

            if (controller.session.Mode == DrawModes.DrawingRectangleSecondCorner
                || controller.session.Mode == DrawModes.ImportingPictureSecondCorner)
            {
                Status = "Click second corner...";
                var size = new Size(controller.session.FirstCorner);
                size.Height = Math.Abs(controller.session.FirstCorner.Y - e.Location.Y);
                size.Width = Math.Abs(controller.session.FirstCorner.X - e.Location.X);
                var newRectangle = new Rectangle(controller.session.FirstCorner, size);
                Canvas.Highlights.Rectangles.Clear();
                Canvas.Highlights.Rectangles.Add(newRectangle);
                controller.session.CurrentRectangle = newRectangle;
            }

            bool nearSomething = false;
            foreach (var layer in Canvas.Layers)
                foreach (var poly in layer.Polylines)
                {
                    int radius = 5;
                    foreach (var vertex in poly.Vertices)
                    {
                        if (Math.Abs(e.X - vertex.X) <= radius
                            &&  Math.Abs(e.Y - vertex.Y) <= radius)
                        {
                            var circle = new Circle()
                            {
                                Center = vertex,
                                Radius = radius
                            };

                            Canvas.Highlights.Circles.Clear();
                            Canvas.Highlights.Circles.Add(circle);
                            nearSomething = true;
                        }
                    }
                }

            if (!nearSomething)
                Canvas.Highlights.Circles.Clear();

            mainPicture.Invalidate();
        }
    }
}
