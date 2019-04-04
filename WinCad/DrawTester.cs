using System;
using System.Drawing;
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
    
        public Canvas Canvas { get; set; }

        public string Status
        {
            set { mainStatus.Text = value; }
        }

        public void InvalidateImage()
        {
            mainPicture.Invalidate();
        }

        public void RenderLayers()
        {
            var image = new Bitmap(mainPicture.Width, mainPicture.Height);

            var graphics = Graphics.FromImage(image);

            foreach (var layer in Canvas.Layers)
                RenderEntities(graphics, layer);

            mainPicture.Image = image;

            mainPicture.Invalidate();
        }

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
                // Circles drawn from rectangle, convert from center & radius
                var corner = new Point(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius);

                int side = circle.Radius * 2;

                g.DrawEllipse(new Pen(circle.Color), corner.X, corner.Y, side, side);
            }
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

        void mainPicture_MouseMove(object sender, MouseEventArgs e)
        {
            controller.HoverAt(e.Location);
        }

        static Rectangle RectangleFrom(Box box)
        {
            return new Rectangle(box.FirstCorner, box.Size);
        }
    }
}
