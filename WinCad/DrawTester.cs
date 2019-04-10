using netDxf;
using netDxf.Entities;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Point = System.Drawing.Point;

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

        public void RefreshImage()
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

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskUserToSaveAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(controller.session.FileName))
            {
                AskUserToSaveAs();
            }
            else
            {
                SaveAs(controller.session.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                OpenFile(openFileDialog1.FileName);
                controller.session.FileName = openFileDialog1.FileName;
            }
        }

        void AskUserToSaveAs()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveAs(saveFileDialog1.FileName);
                controller.session.FileName = saveFileDialog1.FileName;
            }
        }

        public void SaveAs(string file)
        {
            // by default it will create an AutoCad2000 DXF version
            DxfDocument dxf = new DxfDocument();

            foreach (var layer in Canvas.Layers)
            {
                foreach (var pline in layer.Polylines)
                {
                    var vertexes = new List<Vector2>();
                    foreach (var point in pline.Points())
                    {
                        var v = new Vector2(point.X, point.Y);
                        vertexes.Add(v);
                    }

                    var p = new LwPolyline(vertexes);
                    p.Thickness = pline.Width;
                    p.Color = new AciColor(pline.Color.R, pline.Color.G, pline.Color.B);
                    dxf.AddEntity(p);
                }
            }

            dxf.Save(file);
        }

        private void OpenFile(string file)
        {
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);
            if (dxfVersion < DxfVersion.AutoCad2000) return;
            var loaded = DxfDocument.Load(file);

            foreach (var lwPline in loaded.LwPolylines)
            {
                var pline = new Polyline();
                foreach(var vertex in lwPline.Vertexes)
                {
                    var p = new Point((int)vertex.Position.X, (int)vertex.Position.Y);
                    pline.Vertices.Add(p);
                    pline.Color = Color.FromArgb(lwPline.Color.R, lwPline.Color.G, lwPline.Color.B);
                    pline.Width = (int)lwPline.Thickness;
                }

                Canvas.CurrentLayer.Polylines.Add(pline);
            }

            RenderLayers();
        }
    }
}
