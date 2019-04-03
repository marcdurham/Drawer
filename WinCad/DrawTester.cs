using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinCad
{
    public enum DrawModes
    {
        Ready,
        ImportingPictureFirstCorner,
        ImportingPictureSecondCorner,
        DrawingRectangleFirstCorner,
        DrawingRectangleSecondCorner,
        StartDrawing,
        DrawingPolyline
    }

    public partial class DrawTester : Form
    {
        DrawModes drawMode = DrawModes.Ready;
        Point firstCorner = Point.Empty;
        Point secondCorner = Point.Empty;
        Polyline currentPolyline = null;
        Canvas canvas = new Canvas();

        public DrawTester()
        {
            InitializeComponent();
        }

        private void drawPolylineButton_Click(object sender, EventArgs e)
        {
            drawMode = DrawModes.StartDrawing;
            mainStatus.Text = "Start drawing polyline:";
        }

        private void importPictureButton_Click(object sender, EventArgs e)
        {
            drawMode = DrawModes.ImportingPictureFirstCorner;
            mainStatus.Text = "Click first corner:";
        }

        private void drawRectangle_Click(object sender, EventArgs e)
        {
            drawMode = DrawModes.DrawingRectangleFirstCorner;
            mainStatus.Text = "Click first corner:";
        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (var image in canvas.InsertedImages)
                g.DrawImage(image.Image, image.Rectangle);

            foreach (var r in canvas.Rectangles)
                g.DrawRectangle(Pens.Blue, r);

            foreach (var p in canvas.Polylines)
                if (p.Points.Count > 1)
                    for (int i = 1; i < p.Points.Count; i++)
                        g.DrawLine(Pens.Green, p.Points[i - 1], p.Points[i]);
        }

        private void mainPicture_Click(object sender, EventArgs e)
        {
           
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
                drawMode = DrawModes.Ready;
                mainStatus.Text = "Ready";
            }
            else if (drawMode == DrawModes.StartDrawing)
            {
                currentPolyline = new Polyline();
                currentPolyline.Points.Add(point);
                canvas.Polylines.Add(currentPolyline);
                drawMode = DrawModes.DrawingPolyline;
                mainStatus.Text = "Click to add points to the polyline:";
            }
            else if (drawMode == DrawModes.DrawingPolyline)
            {
                currentPolyline.Points.Add(point);
                mainPicture.Invalidate();

            }
            else if (drawMode == DrawModes.ImportingPictureFirstCorner)
            {
                firstCorner = point;
                drawMode = DrawModes.ImportingPictureSecondCorner;
                mainStatus.Text = "Click second corner:";
            }
            else if (drawMode == DrawModes.ImportingPictureSecondCorner)
            {
                secondCorner = point;
                var image = new InsertedImage(
                    image: Bitmap.FromFile(@"C:\Store\Garage.TIF"),
                    rectangle: new Rectangle(
                    firstCorner.X,
                    firstCorner.Y,
                    Math.Abs(firstCorner.X - secondCorner.X),
                    Math.Abs(firstCorner.Y - secondCorner.Y)));

                canvas.InsertedImages.Add(image);

                mainPicture.Invalidate();

                drawMode = DrawModes.Ready;
                mainStatus.Text = "Ready";
                firstCorner = Point.Empty;
                secondCorner = Point.Empty;
            }
            else if (drawMode == DrawModes.DrawingRectangleFirstCorner)
            {
                firstCorner = point;
                drawMode = DrawModes.DrawingRectangleSecondCorner;
                mainStatus.Text = "Click second corner:";
            }
            else if (drawMode == DrawModes.DrawingRectangleSecondCorner)
            {
                secondCorner = point;
                canvas.Rectangles.Add(new Rectangle(
                    firstCorner.X,
                    firstCorner.Y,
                    Math.Abs(firstCorner.X - secondCorner.X),
                    Math.Abs(firstCorner.Y - secondCorner.Y)));

                mainPicture.Invalidate();

                drawMode = DrawModes.Ready;
                mainStatus.Text = "Ready";
                firstCorner = Point.Empty;
                secondCorner = Point.Empty;
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
