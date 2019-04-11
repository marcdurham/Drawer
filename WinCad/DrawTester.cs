using netDxf;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
                controller.InsertImage(openFileDialog.FileName);
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
                controller.session.Canvas = DxfFileOpener.OpenFile(
                    openFileDialog1.FileName);

                Canvas = controller.session.Canvas;

                controller.session.FileName = openFileDialog1.FileName;

                RenderLayers();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.NewFile();
        }

        private void DrawTester_ResizeEnd(object sender, EventArgs e)
        {
            RenderLayers();
        }

        void AskUserToSaveAs()
        {
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(
                controller.session.FileName);

            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(
                controller.session.FileName);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveAs(saveFileDialog1.FileName);
                controller.session.FileName = saveFileDialog1.FileName;
            }
        }

        public void SaveAs(string file)
        {
            // by default it will create an AutoCad2000 DXF version
            var dxf = new DxfDocument();

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

                foreach (var box in layer.Boxes)
                {
                    var vertexes = new List<Vector2>();
                    foreach (var point in box.Points())
                    {
                        var v = new Vector2(point.X, point.Y);
                        vertexes.Add(v);
                    }
                    vertexes.Add(
                        new Vector2(
                            box.Points().First().X, 
                            box.Points().First().Y));

                    var p = new LwPolyline(vertexes);
                    p.Thickness = box.Width;
                    p.Color = new AciColor(box.Color.R, box.Color.G, box.Color.B);
                    dxf.AddEntity(p);
                }

                foreach (var image in layer.InsertedImages)
                {
                    var idef = new ImageDefinition(image.File);
                    var position = new Vector2(image.Box.FirstCorner.X, image.Box.FirstCorner.Y);
                    
                    var dxfImage = new netDxf.Entities.Image(
                        idef, 
                        position, 
                        image.Box.Size.Height, 
                        image.Box.Size.Width);

                    dxf.AddEntity(dxfImage);
                }
            }

            dxf.Save(file);
        }
    }
}
