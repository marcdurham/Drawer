using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinCad
{
    public partial class DrawTester : Form, IDrawingView
    {
        readonly int NearDistance = 5;
        readonly int HighlightRadius = 5;
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

        void drawPolylineButton_Click(object sender, EventArgs e)
        {
            controller.DrawPolyline();
            RenderLayers();
        }

        void importPictureButton_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
                controller.ImportPicture(openFileDialog.FileName);
        }

        void drawRectangle_Click(object sender, EventArgs e)
        {
            controller.DrawRectangle();
        }

        void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            RenderEntities(g, Canvas.Highlights);
        }

        static void RenderEntities(Graphics g, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
                g.DrawImage(image.Image, RectangleFrom(image.Box));

            foreach (var box in layer.Boxes)
                g.DrawRectangle(new Pen(box.Color), RectangleFrom(box));

            foreach (var pline in layer.Polylines)
                if (pline.Vertices.Count > 1)
                    for (int i = 1; i < pline.Vertices.Count; i++)
                        g.DrawLine(new Pen(pline.Color), pline.Vertices[i - 1], pline.Vertices[i]);

            foreach (var circle in layer.Circles)
            {
                var corner = new Point(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius);

                int side = circle.Radius * 2;

                g.DrawEllipse(new Pen(circle.Color), corner.X, corner.Y, side, side);
            }
        }

        static Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(box.FirstCorner, box.Size);
        }

        void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                controller.ClickAt(new Point(e.X, e.Y), e.Button != MouseButtons.Left);

                RenderLayers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Clicking On Picture");
            }
        }

        void RenderLayers()
        {
            var image = new Bitmap(mainPicture.Width, mainPicture.Height);

            var graphics = Graphics.FromImage(image);

            foreach (var layer in Canvas.Layers)
                RenderEntities(graphics, layer);

            mainPicture.Image = image;

            mainPicture.Invalidate();
        }

        void mainPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (controller.session.Mode == DrawModes.DrawingPolylineSecondaryVertices)
            {
                Canvas.NewLineStart = controller.session.CurrentPolyline.Vertices.Last();
                Canvas.NewLineEnd = e.Location;

                var rubberband = new Polyline()
                {
                    Color = Color.Blue,
                    Vertices = new List<Point> { Canvas.NewLineStart, Canvas.NewLineEnd }
                };

                Canvas.Highlights.Polylines.Clear();
                Canvas.Highlights.Polylines.Add(rubberband);
            }

            if (controller.session.Mode == DrawModes.DrawingRectangleSecondCorner
                || controller.session.Mode == DrawModes.ImportingPictureSecondCorner)
            {
                var size = new Size(controller.session.FirstCorner)
                {
                    Height = Math.Abs(controller.session.FirstCorner.Y - e.Location.Y),
                    Width = Math.Abs(controller.session.FirstCorner.X - e.Location.X)
                };

                var box = new Box(controller.session.FirstCorner, size)
                {
                    Color = Color.Blue
                };

                Canvas.Highlights.Boxes.Clear();
                Canvas.Highlights.Boxes.Add(box);
            }

            var circle = new Circle()
            {
                Radius = HighlightRadius,
                Color = Color.Blue
            };

            bool nearSomething = false;
            foreach (var layer in Canvas.Layers)
            {
                foreach (var entity in layer.Entities())
                {
                    foreach (var p in entity.Points())
                    {
                        if (AreNear(p, e.Location))
                        {
                            circle.Center = p;

                            Canvas.Highlights.Circles.Clear();
                            Canvas.Highlights.Circles.Add(circle);
                            nearSomething = true;
                        }
                    }
                }
            }

            if (!nearSomething)
                Canvas.Highlights.Circles.Clear();

            mainPicture.Invalidate();
        }

        bool AreNear(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) <= NearDistance
                && Math.Abs(a.Y - b.Y) <= NearDistance;
        }
    }
}
