using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinCad
{
    public partial class DrawTester : Form, IDrawingView
    {
        readonly DrawingController controller;
        
        DrawingSession session = new DrawingSession();

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
            //var g = mainPicture.CreateGraphics();
            //g.DrawRectangle(Pens.Black, new Rectangle(new Point(20, 20), new Size(10, 10)));
            var image = new Bitmap(mainPicture.Width, mainPicture.Height);
            var gg = mainPicture.CreateGraphics();
           
            mainPicture.Image = image;
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

            //foreach (var layer in Canvas.Layers)
            //    DrawObjectsOn(g, layer);

            DrawObjectsOn(g, Canvas.Highlights);

            if (controller.session.CurrentPolyline != null)
                g.DrawLine(Pens.Blue, Canvas.NewLineStart, Canvas.NewLineEnd);
        }

        private static void DrawObjectsOn(Graphics g, Layer layer)
        {
            foreach (var image in layer.InsertedImages)
                g.DrawImage(image.Image, image.Rectangle);

            foreach (var r in layer.Rectangles)
                g.DrawRectangle(Pens.Blue, r);

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
                mainPicture.Invalidate();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Clicking On Picture");
            }
        }

        private void mainPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (controller.session.CurrentPolyline?.Vertices?.Count > 0)
            {
                Canvas.NewLineStart = controller.session.CurrentPolyline.Vertices.Last();
                Canvas.NewLineEnd = e.Location;
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
