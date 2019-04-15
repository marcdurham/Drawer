﻿using System;
using System.Drawing;
using System.IO;
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

        public void RefreshImage()
        {
            mainPicture.Invalidate();
        }

        public void RenderLayers()
        {
            var image = new Bitmap(mainPicture.Width, mainPicture.Height);

            var graphics = Graphics.FromImage(image);

            foreach (var layer in Canvas.Layers)
            {
                engine.Render(graphics, layer);
            }

            mainPicture.Image = image;

            mainPicture.Invalidate();
        }

        public UserAnswer AskUser(string question)
        {
            var answer = MessageBox.Show(
                question,
                Properties.Resources.AskUserDialogCaption,
                MessageBoxButtons.YesNoCancel);

            if (answer == DialogResult.Yes)
            {
                return UserAnswer.Yes;
            }
            else if (answer == DialogResult.No)
            {
                return UserAnswer.No;
            }
            else if (answer == DialogResult.Cancel)
            {
                return UserAnswer.Cancel;
            }
            else
            {
                return UserAnswer.UnknownAnswer;
            }
        }

        void drawPolylineButton_Click(object sender, EventArgs e)
        {
            controller.DrawPolyline();
        }

        void importPictureButton_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                controller.InsertImage(openFileDialog.FileName);
            }
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
                controller.ClickAt(e.Location, e.Button != MouseButtons.Left);
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
                controller.SaveAs(controller.session.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                controller.OpenFile(openFileDialog1.FileName);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.NewFile();
        }

        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            controller.ZoomInClick();
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            controller.ZoomOutClick();
        }

        void AskUserToSaveAs()
        {
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(
                controller.session.FileName);

            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(
                controller.session.FileName);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                controller.SaveAs(saveFileDialog1.FileName);
            }
        }

        private void DrawTester_SizeChanged(object sender, EventArgs e)
        {
            RenderLayers();
        }
    }
}
