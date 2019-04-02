using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinCad
{
    public enum DrawModes { Ready, ImportingPicture }
    public partial class DrawTester : Form
    {
        List<Image> images = new List<Image>();
        List<Point> imagePoints = new List<Point>();
        DrawModes drawMode = DrawModes.Ready;

        public DrawTester()
        {
            InitializeComponent();
        }

        private void importPictureButton_Click(object sender, EventArgs e)
        {
            drawMode = DrawModes.ImportingPicture;
        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (var image in images)
                g.DrawImage(image, new Point(5, 5));

            mainStatus.Text = $"Image Count: {images.Count}";
        }

        private void mainPicture_Click(object sender, EventArgs e)
        {
           
        }

        private void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawMode == DrawModes.ImportingPicture)
            {
                images.Add(Bitmap.FromFile(@"X:\Leads\L-1000 TO L-2000\L-1008 Schliebus, Judy\PR-1064 Schliebus\Garage.TIF"));
                mainStatus.Text = "Image added";
                mainPicture.Invalidate();
                drawMode = DrawModes.Ready;
            }
        }
    }
}
