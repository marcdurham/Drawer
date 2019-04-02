using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinCad
{
    public partial class DrawTester : Form
    {
        List<Image> images = new List<Image>();

        public DrawTester()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            images.Add(Bitmap.FromFile(@"X:\Leads\L-1000 TO L-2000\L-1008 Schliebus, Judy\PR-1064 Schliebus\Garage.TIF"));
            mainStatus.Text = "Image added";
            mainPicture.Invalidate();
        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (var image in images)
                g.DrawImage(image, new Point(5, 5));

            mainStatus.Text = $"Image Count: {images.Count}";
        }
    }
}
