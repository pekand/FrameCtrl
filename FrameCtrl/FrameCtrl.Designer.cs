namespace FrameCtrl
{
    partial class FrameCtrl
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameCtrl));
            videoView = new LibVLCSharp.WinForms.VideoView();
            contextMenuStrip = new ContextMenuStrip(components);
            openToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            mostTopToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            StatusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)videoView).BeginInit();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // videoView
            // 
            videoView.AllowDrop = true;
            videoView.BackColor = Color.Black;
            videoView.Dock = DockStyle.Fill;
            videoView.Location = new Point(0, 0);
            videoView.MediaPlayer = null;
            videoView.Name = "videoView";
            videoView.Size = new Size(800, 450);
            videoView.TabIndex = 0;
            videoView.TabStop = false;
            videoView.Text = "videoView1";
            videoView.DragDrop += videoView_DragDrop;
            videoView.DragEnter += videoView_DragEnter;
            videoView.KeyDown += FrameCtrl_KeyDown;
            videoView.MouseClick += videoView_MouseClick;
            videoView.MouseDown += videoView_MouseDown;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, optionsToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(131, 76);
            contextMenuStrip.Opening += contextMenuStrip_Opening;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(130, 24);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mostTopToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(130, 24);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // mostTopToolStripMenuItem
            // 
            mostTopToolStripMenuItem.Name = "mostTopToolStripMenuItem";
            mostTopToolStripMenuItem.Size = new Size(138, 24);
            mostTopToolStripMenuItem.Text = "Most top";
            mostTopToolStripMenuItem.Click += mostTopToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(130, 24);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.BackColor = Color.Black;
            StatusLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            StatusLabel.ForeColor = Color.White;
            StatusLabel.Location = new Point(12, 9);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(89, 21);
            StatusLabel.TabIndex = 1;
            StatusLabel.Text = "StatusLabel";
            StatusLabel.Visible = false;
            // 
            // FrameCtrl
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(StatusLabel);
            Controls.Add(videoView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrameCtrl";
            StartPosition = FormStartPosition.Manual;
            Text = "FrameCtrl";
            FormClosing += FrameCtrl_FormClosing;
            FormClosed += FrameCtrl_FormClosed;
            Load += FrameCtrl_Load;
            DragDrop += FrameCtrl_DragDrop;
            DragEnter += FrameCtrl_DragEnter;
            Move += FrameCtrl_Move;
            Resize += FrameCtrl_Resize;
            ((System.ComponentModel.ISupportInitialize)videoView).EndInit();
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private Label StatusLabel;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem mostTopToolStripMenuItem;
    }
}
