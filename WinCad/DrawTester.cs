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
        DrawingRectangleSecondCorner
    }

    public partial class DrawTester : Form
    {
        List<Image> images = new List<Image>();
        List<Point> imagePoints = new List<Point>();
        List<Rectangle> imageRectangles = new List<Rectangle>();
        List<Rectangle> rectangles = new List<Rectangle>();
        DrawModes drawMode = DrawModes.Ready;
        Point firstCorner = Point.Empty;
        Point secondCorner = Point.Empty;

        public DrawTester()
        {
            InitializeComponent();
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
            for (int i = 0; i < images.Count; i++)
                g.DrawImage(images[i], imageRectangles[i]);

            var pen = Pens.Blue;
           // pen.Width = 3.0f;
            foreach (var r in rectangles)
                g.DrawRectangle(pen, r);
        }

        private void mainPicture_Click(object sender, EventArgs e)
        {
           
        }

        private void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                canvasClick(new Point(e.X, e.Y));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Importing Picture");
            }
        }


        void canvasClick(Point point)
        { 
            if (drawMode == DrawModes.ImportingPictureFirstCorner)
            {
                firstCorner= point;
                drawMode = DrawModes.ImportingPictureSecondCorner;
                mainStatus.Text = "Click second corner:";
            }
            else if (drawMode == DrawModes.ImportingPictureSecondCorner)
            {
                secondCorner = point;
                imageRectangles.Add(new Rectangle(
                    firstCorner.X, 
                    firstCorner.Y,
                    Math.Abs( firstCorner.X - secondCorner.X),
                    Math.Abs(firstCorner.Y - secondCorner.Y)));

                images.Add(Bitmap.FromFile(@"X:\Store\Garage.TIF"));

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
                rectangles.Add(new Rectangle(
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
