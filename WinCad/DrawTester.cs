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
        ImportingPictureSecondCorner
    }

    public partial class DrawTester : Form
    {
        List<Image> images = new List<Image>();
        List<Point> imagePoints = new List<Point>();
        List<Rectangle> imageRectangles = new List<Rectangle>();
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

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < images.Count; i++)
                g.DrawImage(images[i], imageRectangles[i]);
        }

        private void mainPicture_Click(object sender, EventArgs e)
        {
           
        }

        private void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawMode == DrawModes.ImportingPictureFirstCorner)
            {
                firstCorner= new Point(e.X, e.Y);
                drawMode = DrawModes.ImportingPictureSecondCorner;
                mainStatus.Text = "Click second corner:";
            }
            else if (drawMode == DrawModes.ImportingPictureSecondCorner)
            {
                secondCorner = new Point(e.X, e.Y);
                imageRectangles.Add(new Rectangle(firstCorner.X, firstCorner.Y, secondCorner.X, secondCorner.Y));
                images.Add(Bitmap.FromFile(@"X:\Leads\L-1000 TO L-2000\L-1008 Schliebus, Judy\PR-1064 Schliebus\Garage.TIF"));

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
