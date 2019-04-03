using System;
using System.Drawing;
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
        }

        private void importPictureButton_Click(object sender, EventArgs e)
        {
            controller.ImportPicture();
        }

        private void drawRectangle_Click(object sender, EventArgs e)
        {
            controller.DrawRectangle();
        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (var image in Canvas.InsertedImages)
                g.DrawImage(image.Image, image.Rectangle);

            foreach (var r in Canvas.Rectangles)
                g.DrawRectangle(Pens.Blue, r);

            foreach (var p in Canvas.Polylines)
                if (p.Vertices.Count > 1)
                    for (int i = 1; i < p.Vertices.Count; i++)
                        g.DrawLine(Pens.Green, p.Vertices[i - 1], p.Vertices[i]);

            foreach (var circle in Canvas.Circles)
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

        private void mainPicture_Move(object sender, EventArgs e)
        {
           
        }

        private void mainPicture_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (var poly in Canvas.Polylines)
            {
                foreach (var vertex in poly.Vertices)
                {
                    if (Math.Abs(e.X - vertex.X) <= 5 
                        &&  Math.Abs(e.Y - vertex.Y) <= 5)
                    {
                        var circle = new Circle() { Center = vertex, Radius = 5 };
                        Canvas.Circles.Clear();
                        Canvas.Circles.Add(circle);
                        mainPicture.Invalidate();
                    }
                }
            }
        }
    }
}
