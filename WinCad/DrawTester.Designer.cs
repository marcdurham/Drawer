namespace WinCad
{
    partial class DrawTester
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawTester));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.drawPolylineButton = new System.Windows.Forms.ToolStripButton();
            this.drawRectangle = new System.Windows.Forms.ToolStripButton();
            this.importPictureButton = new System.Windows.Forms.ToolStripButton();
            this.mainPicture = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPicture)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawPolylineButton,
            this.drawRectangle,
            this.importPictureButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // drawPolylineButton
            // 
            this.drawPolylineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.drawPolylineButton.Name = "drawPolylineButton";
            this.drawPolylineButton.Size = new System.Drawing.Size(53, 22);
            this.drawPolylineButton.Text = "Polyline";
            this.drawPolylineButton.Click += new System.EventHandler(this.drawPolylineButton_Click);
            // 
            // drawRectangle
            // 
            this.drawRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.drawRectangle.Image = ((System.Drawing.Image)(resources.GetObject("drawRectangle.Image")));
            this.drawRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.drawRectangle.Name = "drawRectangle";
            this.drawRectangle.Size = new System.Drawing.Size(63, 22);
            this.drawRectangle.Text = "Rectangle";
            this.drawRectangle.ToolTipText = "Draw Rectangle";
            this.drawRectangle.Click += new System.EventHandler(this.drawRectangle_Click);
            // 
            // importPictureButton
            // 
            this.importPictureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.importPictureButton.Image = ((System.Drawing.Image)(resources.GetObject("importPictureButton.Image")));
            this.importPictureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importPictureButton.Name = "importPictureButton";
            this.importPictureButton.Size = new System.Drawing.Size(87, 22);
            this.importPictureButton.Text = "Import Picture";
            this.importPictureButton.Click += new System.EventHandler(this.importPictureButton_Click);
            // 
            // mainPicture
            // 
            this.mainPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPicture.Location = new System.Drawing.Point(0, 25);
            this.mainPicture.Name = "mainPicture";
            this.mainPicture.Size = new System.Drawing.Size(800, 425);
            this.mainPicture.TabIndex = 1;
            this.mainPicture.TabStop = false;
            this.mainPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPicture_Paint);
            this.mainPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainPicture_MouseClick);
            this.mainPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainPicture_MouseMove);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mainStatus
            // 
            this.mainStatus.Name = "mainStatus";
            this.mainStatus.Size = new System.Drawing.Size(39, 17);
            this.mainStatus.Text = "Ready";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Image Files|*.jpg;*.tif;*.tif;*.bmp;*.png";
            // 
            // DrawTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mainPicture);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DrawTester";
            this.Text = "Draw Tester";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPicture)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton drawPolylineButton;
        private System.Windows.Forms.PictureBox mainPicture;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel mainStatus;
        private System.Windows.Forms.ToolStripButton importPictureButton;
        private System.Windows.Forms.ToolStripButton drawRectangle;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

