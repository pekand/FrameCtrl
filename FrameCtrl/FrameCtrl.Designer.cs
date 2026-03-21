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
            exitToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)videoView).BeginInit();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // videoView
            // 
            videoView.BackColor = Color.Black;
            videoView.Dock = DockStyle.Fill;
            videoView.Location = new Point(0, 0);
            videoView.MediaPlayer = null;
            videoView.Name = "videoView";
            videoView.Size = new Size(800, 450);
            videoView.TabIndex = 0;
            videoView.TabStop = false;
            videoView.Text = "videoView1";
            videoView.KeyDown += FrameCtrl_KeyDown;
            videoView.MouseClick += videoView_MouseClick;
            videoView.MouseDown += videoView_MouseDown;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(115, 52);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(114, 24);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(114, 24);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // FrameCtrl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(videoView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrameCtrl";
            StartPosition = FormStartPosition.Manual;
            Text = "FrameCtrl";
            FormClosing += FrameCtrl_FormClosing;
            Load += FrameCtrl_Load_1;
            ((System.ComponentModel.ISupportInitialize)videoView).EndInit();
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
    }
}
