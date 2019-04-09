using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCad
{
    public partial class DrawTester : Form, IDrawingView
    {
        readonly DrawingController controller;
        readonly EntityRenderEngine engine;
        
        public DrawTester()
        {
            InitializeComponent();

            controller = new DrawingController(this);
            engine = new EntityRenderEngine();
        }
    
        public Canvas Canvas { get; set; }

        public string Status
        {
            set { mainStatus.Text = value; }
        }

        public string SecondStatus
        {
            set { secondStatus.Text = value; }
        }

        public bool OrthoIsOn
        {
            get { return orthoButton.Checked; }
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
                engine.Render(graphics, layer);

            mainPicture.Image = image;

            mainPicture.Invalidate();
        }

        void drawPolylineButton_Click(object sender, EventArgs e)
        {
            controller.DrawPolyline();
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
            engine.Render(e.Graphics, Canvas.Selections);
            engine.Render(e.Graphics, Canvas.Highlights);
        }

        void mainPicture_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                controller.ClickAt(new Point(e.X, e.Y), e.Button != MouseButtons.Left);
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

        void orthoButton_Click(object sender, EventArgs e)
        {
            orthoButton.Checked = !orthoButton.Checked;
        }

        private void insertBlock_Click(object sender, EventArgs e)
        {
            controller.InsertBlock();
        }

        private void selectEntityButtonh_Click(object sender, EventArgs e)
        {
            controller.SelectEntity();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            controller.DeleteSelectedEntities();
        }
    }
}
