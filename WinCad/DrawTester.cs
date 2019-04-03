using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCad
{

    public partial class DrawTester : Form
    {
        DrawingSession session = new DrawingSession();

        public DrawTester()
        {
            InitializeComponent();
        }

        private void drawPolylineButton_Click(object sender, EventArgs e)
        {
            session.Mode = DrawModes.StartDrawing;
            mainStatus.Text = "Start drawing polyline:";
        }

        private void importPictureButton_Click(object sender, EventArgs e)
        {
            session.Mode = DrawModes.ImportingPictureFirstCorner;
            mainStatus.Text = "Click first corner:";
        }

        private void drawRectangle_Click(object sender, EventArgs e)
        {
            session.Mode = DrawModes.DrawingRectangleFirstCorner;
            mainStatus.Text = "Click first corner:";
        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (var image in session.Canvas.InsertedImages)
                g.DrawImage(image.Image, image.Rectangle);

            foreach (var r in session.Canvas.Rectangles)
                g.DrawRectangle(Pens.Blue, r);

            foreach (var p in session.Canvas.Polylines)
                if (p.Points.Count > 1)
                    for (int i = 1; i < p.Points.Count; i++)
                        g.DrawLine(Pens.Green, p.Points[i - 1], p.Points[i]);
        }

        private void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                canvasClick(new Point(e.X, e.Y), e.Button);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Importing Picture");
            }
        }


        void canvasClick(Point point, MouseButtons button)
        {
            if (button != MouseButtons.Left)
            {
                session.Mode = DrawModes.Ready;
                mainStatus.Text = "Ready";
            }
            else if (session.Mode == DrawModes.StartDrawing)
            {
                session.CurrentPolyline = new Polyline();
                session.CurrentPolyline.Points.Add(point);
                session.Canvas.Polylines.Add(session.CurrentPolyline);
                session.Mode = DrawModes.DrawingPolyline;
                mainStatus.Text = "Click to add points to the polyline:";
            }
            else if (session.Mode == DrawModes.DrawingPolyline)
            {
                session.CurrentPolyline.Points.Add(point);
                mainPicture.Invalidate();

            }
            else if (session.Mode == DrawModes.ImportingPictureFirstCorner)
            {
                session.FirstCorner = point;
                session.Mode = DrawModes.ImportingPictureSecondCorner;
                mainStatus.Text = "Click second corner:";
            }
            else if (session.Mode == DrawModes.ImportingPictureSecondCorner)
            {
                session.SecondCorner = point;
                var image = new InsertedImage(
                    image: Bitmap.FromFile(@"C:\Store\Garage.TIF"),
                    rectangle: new Rectangle(
                    session.FirstCorner.X,
                    session.FirstCorner.Y,
                    Math.Abs(session.FirstCorner.X - session.SecondCorner.X),
                    Math.Abs(session.FirstCorner.Y - session.SecondCorner.Y)));

                session.Canvas.InsertedImages.Add(image);

                mainPicture.Invalidate();

                session.Mode = DrawModes.Ready;
                mainStatus.Text = "Ready";
                session.FirstCorner = Point.Empty;
                session.SecondCorner = Point.Empty;
            }
            else if (session.Mode == DrawModes.DrawingRectangleFirstCorner)
            {
                session.FirstCorner = point;
                session.Mode = DrawModes.DrawingRectangleSecondCorner;
                mainStatus.Text = "Click second corner:";
            }
            else if (session.Mode == DrawModes.DrawingRectangleSecondCorner)
            {
                session.SecondCorner = point;
                session.Canvas.Rectangles.Add(new Rectangle(
                    session.FirstCorner.X,
                    session.FirstCorner.Y,
                    Math.Abs(session.FirstCorner.X - session.SecondCorner.X),
                    Math.Abs(session.FirstCorner.Y - session.SecondCorner.Y)));

                mainPicture.Invalidate();

                session.Mode = DrawModes.Ready;
                mainStatus.Text = "Ready";
                session.FirstCorner = Point.Empty;
                session.SecondCorner = Point.Empty;
            }
            else
            {
                if (mainStatus.Text == "Clicked")
                    mainStatus.Text = "Ready";
                else
                    mainStatus.Text = "Clicked";
            }
        }
    }
}
