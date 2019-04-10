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
            var result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveAs(saveFileDialog1.FileName);
            }
        }

        public void SaveAs(string file)
        {
            // your dxf file name
            //string file = "sample.dxf";

            // by default it will create an AutoCad2000 DXF version
            DxfDocument dxf = new DxfDocument();
            
            foreach (var layer in Canvas.Layers)
                foreach (var pline in layer.Polylines)
                {

                    var vertexes = new List<Vector2>();
                    foreach (var point in pline.Points())
                    {
                        var v = new Vector2(point.X, point.Y);
                        vertexes.Add(v);
                    }
                    var p = new LwPolyline(vertexes);
                    dxf.AddEntity(p);
                }
            
            // an entity



            Line entity = new Line(new Vector2(5, 5), new Vector2(10, 5));
            // add your entities here
            dxf.AddEntity(entity);
            // save to file
            dxf.Save(file);

            ////bool isBinary;
            // this check is optional but recommended before loading a DXF file
            ///DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);
            // /netDxf is only compatible with AutoCad2000 and higher DXF version
            ////if (dxfVersion < DxfVersion.AutoCad2000) return;
            // load file
            ////DxfDocument loaded = DxfDocument.Load(file);
        }
    }
}
