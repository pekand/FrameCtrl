using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Numerics;
using System.Windows.Forms;

namespace FrameCtrl
{
    public partial class FrameCtrl : Form
    {

        private string videoPath = null;
        private string playlistPath = null;

        private LibVLC libVLC;
        private Media media;
        private MediaPlayer mediaplayer;

        public FrameCtrl(string videoPath = null, string playlistPath = null)
        {
            this.videoPath = videoPath;
            this.playlistPath = playlistPath;

            InitializeComponent();
            SetupPlayer();

            if (this.videoPath != null && File.Exists(this.videoPath)) {
                this.openVideoFile(this.videoPath);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetupPlayer()
        {
            Core.Initialize();
            var options = new string[]
            {
                "--no-mouse-events",
                "--no-keyboard-events"
            };

            this.libVLC = new LibVLC(true, options);
        }

        public void openVideoFile(string videoPath) 
        {
            if (mediaplayer != null)
            {
                this.videoView.MediaPlayer = null;
                this.mediaplayer.Dispose();
                this.media.Dispose();
            }


            this.media = new Media(this.libVLC, new Uri(videoPath));
            this.mediaplayer = new MediaPlayer(media);
            this.videoView.MediaPlayer = this.mediaplayer;
            this.videoView.ContextMenuStrip = contextMenuStrip;
            mediaplayer.EnableMouseInput = false;
            mediaplayer.EnableKeyInput = false;

            mediaplayer.TimeChanged += (sender, e) =>
            {
                this.BeginInvoke(new Action(() => {
                    UpdateTitle(e.Time);
                }));
            };

            mediaplayer.Playing += (sender, e) =>
            {
                this.BeginInvoke(new Action(() => {
                    UpdateTitle(mediaplayer.Time);
                }));
            };

            mediaplayer.Play(media);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video Files|*.mp4;*.mkv;*.avi;*.webm|All files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.openVideoFile(openFileDialog.FileName);
                }
            }
        }

        private void UpdateTitle(long currentTime)
        {
            if (mediaplayer == null) return;

            long totalTime = mediaplayer.Length; // Total duration in ms

            string currentStr = FormatTime(currentTime);
            string totalStr = FormatTime(totalTime);

            this.Text = $"FrameCtrl - {currentStr} / {totalStr}";
        }

        private string FormatTime(long managedTime)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(managedTime);
            return t.TotalHours >= 1
                ? t.ToString(@"hh\:mm\:ss")
                : t.ToString(@"mm\:ss");
        }

        private void FrameCtrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mediaplayer != null)
            {
                this.videoView.MediaPlayer = null;
                this.mediaplayer.Dispose();
                this.media.Dispose();
                this.libVLC.Dispose();
            }
        }

        private void FrameCtrl_Load(object sender, EventArgs e)
        {

        }

        private void FrameCtrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (mediaplayer == null) return;

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (mediaplayer == null) return base.ProcessCmdKey(ref msg, keyData);

            // Detekcia Shiftu pomocou bitovej masky
            bool isShiftPressed = (keyData & Keys.Shift) == Keys.Shift;

            // Extrahovanie samotného klávesu (bez modifikátorov)
            Keys keyCode = (keyData & Keys.KeyCode);

            switch (keyCode)
            {
                case Keys.Left:
                    SeekRelative(-1, isShiftPressed);
                    return true; // Povieme systému, že sme kláves spracovali

                case Keys.Right:
                    SeekRelative(1, isShiftPressed);
                    return true;

                case Keys.Space:
                    if (mediaplayer.IsPlaying) mediaplayer.Pause(); else mediaplayer.Play();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SeekRelative(int direction, bool isShiftPressed)
        {
            long interval = isShiftPressed ? 60000 : 1000;
            long jump = direction * interval;

            long newTime = mediaplayer.Time + jump;

            if (newTime < 0) newTime = 0;
            if (newTime > mediaplayer.Length) newTime = mediaplayer.Length;

            mediaplayer.Time = newTime;
        }

        private void FrameCtrl_Load_1(object sender, EventArgs e)
        {

        }

        private void videoView_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void videoView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(this.videoView, e.Location);
            }
        }
    }
}
