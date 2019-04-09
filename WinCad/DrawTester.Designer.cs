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
            this.insertImageButton = new System.Windows.Forms.ToolStripButton();
            this.scaleImageButton = new System.Windows.Forms.ToolStripButton();
            this.insertBlock = new System.Windows.Forms.ToolStripButton();
            this.drawRectangle = new System.Windows.Forms.ToolStripButton();
            this.drawPolylineButton = new System.Windows.Forms.ToolStripButton();
            this.orthoButton = new System.Windows.Forms.ToolStripButton();
            this.mainPicture = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.secondStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.selectEntityButtonh = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPicture)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectEntityButtonh,
            this.insertImageButton,
            this.scaleImageButton,
            this.insertBlock,
            this.drawRectangle,
            this.drawPolylineButton,
            this.orthoButton,
            this.deleteButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // insertImageButton
            // 
            this.insertImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.insertImageButton.Image = ((System.Drawing.Image)(resources.GetObject("insertImageButton.Image")));
            this.insertImageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.insertImageButton.Name = "insertImageButton";
            this.insertImageButton.Size = new System.Drawing.Size(76, 22);
            this.insertImageButton.Text = "Insert Image";
            this.insertImageButton.Click += new System.EventHandler(this.importPictureButton_Click);
            // 
            // scaleImageButton
            // 
            this.scaleImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.scaleImageButton.Image = ((System.Drawing.Image)(resources.GetObject("scaleImageButton.Image")));
            this.scaleImageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.scaleImageButton.Name = "scaleImageButton";
            this.scaleImageButton.Size = new System.Drawing.Size(74, 22);
            this.scaleImageButton.Text = "Scale Image";
            // 
            // insertBlock
            // 
            this.insertBlock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.insertBlock.Image = ((System.Drawing.Image)(resources.GetObject("insertBlock.Image")));
            this.insertBlock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.insertBlock.Name = "insertBlock";
            this.insertBlock.Size = new System.Drawing.Size(40, 22);
            this.insertBlock.Text = "Block";
            this.insertBlock.Click += new System.EventHandler(this.insertBlock_Click);
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
            // drawPolylineButton
            // 
            this.drawPolylineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.drawPolylineButton.Name = "drawPolylineButton";
            this.drawPolylineButton.Size = new System.Drawing.Size(53, 22);
            this.drawPolylineButton.Text = "Polyline";
            this.drawPolylineButton.Click += new System.EventHandler(this.drawPolylineButton_Click);
            // 
            // orthoButton
            // 
            this.orthoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.orthoButton.Image = ((System.Drawing.Image)(resources.GetObject("orthoButton.Image")));
            this.orthoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.orthoButton.Name = "orthoButton";
            this.orthoButton.Size = new System.Drawing.Size(42, 22);
            this.orthoButton.Text = "Ortho";
            this.orthoButton.Click += new System.EventHandler(this.orthoButton_Click);
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
            this.mainStatus,
            this.secondStatus});
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
            // secondStatus
            // 
            this.secondStatus.Name = "secondStatus";
            this.secondStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Image Files|*.jpg;*.tif;*.tif;*.bmp;*.png";
            // 
            // selectEntityButtonh
            // 
            this.selectEntityButtonh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.selectEntityButtonh.Image = ((System.Drawing.Image)(resources.GetObject("selectEntityButtonh.Image")));
            this.selectEntityButtonh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectEntityButtonh.Name = "selectEntityButtonh";
            this.selectEntityButtonh.Size = new System.Drawing.Size(42, 22);
            this.selectEntityButtonh.Text = "Select";
            this.selectEntityButtonh.ToolTipText = "Click to select entites";
            this.selectEntityButtonh.Click += new System.EventHandler(this.selectEntityButtonh_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(44, 22);
            this.deleteButton.Text = "Delete";
            this.deleteButton.ToolTipText = "Delete selected entities";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
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
        private System.Windows.Forms.ToolStripButton insertImageButton;
        private System.Windows.Forms.ToolStripButton drawRectangle;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripButton scaleImageButton;
        private System.Windows.Forms.ToolStripButton orthoButton;
        private System.Windows.Forms.ToolStripStatusLabel secondStatus;
        private System.Windows.Forms.ToolStripButton insertBlock;
        private System.Windows.Forms.ToolStripButton selectEntityButtonh;
        private System.Windows.Forms.ToolStripButton deleteButton;
    }
}

