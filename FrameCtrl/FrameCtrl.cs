using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Numerics;
using System.Windows.Forms;

namespace FrameCtrl
{
    public partial class FrameCtrl : Form
    {
        private Config config = null;
        private LibVLC libVLC;
        private Media media;
        private MediaPlayer mediaplayer;
        private bool lockMove = false;
        public HistoryItem historyItem = null;

        // CONSTRUCTOR
        public FrameCtrl(Config config)
        {
            this.config = config;

            InitializeComponent();
            SetupPlayer();
        }

        // EVENT LOAD
        private void FrameCtrl_Load(object sender, EventArgs e)
        {
            lockMove = true;
            this.Left = this.config.Left;
            this.Top = this.config.Top;
            this.Width = this.config.Width;
            this.Height = this.config.Height;

            if (this.Width < 50)
            {
                this.Width = 50;
            }

            if (this.Height < 50)
            {
                this.Height = 50;
            }

            if ((this.config.Left < 0 && this.config.Top < 0))
            {
                this.CenterOnCurrentScreen();
            }

            if (!this.IsWindowSuccessfullyVisible()) {
                this.Width = 300;
                this.Height = 300;
                this.CenterOnCurrentScreen();
            }

            lockMove = false;

            if (this.config.videoPath != null && File.Exists(this.config.videoPath) && VideoHelper.IsVideoFile(this.config.videoPath))
            {
                this.openVideoFile(this.config.videoPath, config.currentPosition);
            }
        }

        // EVENT CLOSINF
        private void FrameCtrl_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.config.Left = this.Left;
                this.config.Top = this.Top;
                this.config.Width = this.Width;
                this.config.Height = this.Height;
            }

            if (this.historyItem != null && mediaplayer != null)
            {
                this.historyItem.Position = mediaplayer.Time;
            }

            if (mediaplayer != null)
            {
                this.videoView.MediaPlayer = null;
                this.mediaplayer.Dispose();
                this.media.Dispose();
                this.libVLC.Dispose();
            }
        }

        // KEY
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (mediaplayer == null) return base.ProcessCmdKey(ref msg, keyData);


            bool isShiftPressed = (keyData & Keys.Shift) == Keys.Shift;

            Keys keyCode = (keyData & Keys.KeyCode);

            switch (keyCode)
            {

                case Keys.Up:
                    PlaylistPrev();
                    return true;

                case Keys.Down:
                    PlaylistNext();
                    return true;

                case Keys.Home:
                    ToBeggining();
                    UpdateTitle(mediaplayer.Time);
                    return true; 

                case Keys.End:
                    ToEnd();
                    UpdateTitle(mediaplayer.Time);
                    return true;

                case Keys.Left:
                    SeekRelative(-1, isShiftPressed);
                    UpdateTitle(mediaplayer.Time);
                    return true; 

                case Keys.Right:
                    SeekRelative(1, isShiftPressed);
                    UpdateTitle(mediaplayer.Time);
                    return true;

                case Keys.Space:
                    if (mediaplayer.IsPlaying) mediaplayer.Pause(); else mediaplayer.Play();
                    UpdateTitle(mediaplayer.Time);
                    return true;

                case Keys.F:
                    if (this.historyItem != null) this.historyItem.finished = !this.historyItem.finished;                    
                    return true;

                    

            }



            return base.ProcessCmdKey(ref msg, keyData);
        }

        // KEY
        private void FrameCtrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (mediaplayer == null) return;

        }

        // MOUSE
        private void videoView_MouseClick(object sender, MouseEventArgs e)
        {

        }

        // MOUSE
        private void videoView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(this.videoView, e.Location);
            }
        }

        // DRAG
        private void FrameCtrl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // DRAG
        private void FrameCtrl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
            {
                this.config.playlist.Clear();
                foreach (string filePath in files)
                {
                    if (File.Exists(filePath) && VideoHelper.IsVideoFile(filePath))
                    {
                        this.config.playlist.Add(filePath);
                    }
                }

                if (this.config.playlist.Count > 0)
                {
                    this.config.playlistPosition = 0;
                    this.config.videoPath = this.config.playlist[this.config.playlistPosition];
                    this.openVideoFile(this.config.videoPath);
                }

            }
        }

        // CONTEXTMENU OPEN
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video Files|*.mp4;*.mkv;*.avi;*.webm|All files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.config.videoPath = openFileDialog.FileName;
                    this.openVideoFile(this.config.videoPath);
                }
            }
        }

        // CONTEXTMENU EXIT
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

        public bool openVideoFile(string videoPath, long skipTime = 0)
        {

            if (!File.Exists(videoPath))
            {
                return false;
            }

            if (this.historyItem != null && mediaplayer != null)
            {
                this.historyItem.Position = mediaplayer.Time;
            }

            HistoryItem historyItem = this.AddOrFindHistoryItem(videoPath);

            if (skipTime == 0 && historyItem != null && historyItem.Position != 0) {
                skipTime = historyItem.Position;
            }

            if (mediaplayer != null)
            {
                this.videoView.MediaPlayer = null;
                this.mediaplayer.Dispose();
                this.media.Dispose();
            }

            this.config.videoPath = videoPath;
            this.media = new Media(this.libVLC, new Uri(videoPath), ":play-and-pause");
            this.mediaplayer = new MediaPlayer(media);
            this.videoView.MediaPlayer = this.mediaplayer;
            this.videoView.ContextMenuStrip = contextMenuStrip;
            mediaplayer.EnableMouseInput = false;
            mediaplayer.EnableKeyInput = false;
            config.currentPosition = 0;
            config.currentPercent = 0;

            mediaplayer.TimeChanged += (sender, e) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateTitle(e.Time);
                }));
            };

            mediaplayer.Playing += (sender, e) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateTitle(mediaplayer.Time);
                }));
            };

            mediaplayer.EndReached += (sender, e) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    EndReached();
                }));
            };

            mediaplayer.PositionChanged += (sender, e) =>
            {
                if (skipTime > 0)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (skipTime > 0)
                        {
                            mediaplayer.Time = skipTime;
                            skipTime = 0;
                        }
                    }));


                }

                if (e.Position >= 0.99)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        EndReached();
                    }));
                }
                else
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        PositionChange();
                    }));
                }
            };

            mediaplayer.Play(media);

            return true;
        }

        private void PositionChange()
        {
            config.currentPosition = mediaplayer.Time;
            config.currentPercent = mediaplayer.Position;
        }

        private void EndReached()
        {
            config.currentPosition = 0;
            config.currentPercent = 0;

            if (this.historyItem != null) {
                this.historyItem.finished = true;
            } 
        }

        private void UpdateTitle(long currentTime)
        {
            if (mediaplayer == null) return;

            long totalTime = mediaplayer.Length; // Total duration in ms

            string currentStr = FormatTime(currentTime);
            string totalStr = FormatTime(totalTime);
            string finished = (this.historyItem != null && this.historyItem.finished) ? " - FINISHED" : "";

            this.Text = $"FrameCtrl - {currentStr} / {totalStr}"+finished;
        }

        private string FormatTime(long managedTime)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(managedTime);
            return t.TotalHours >= 1
                ? t.ToString(@"hh\:mm\:ss")
                : t.ToString(@"mm\:ss");
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

        private void ToBeggining()
        {
            if (mediaplayer == null) return;

            mediaplayer.Position = 0.0f;
        }

        private void ToEnd()
        {
            if (mediaplayer == null) return;

            mediaplayer.Time = mediaplayer.Length - 5000;
        }

        private void FrameCtrl_Move(object sender, EventArgs e)
        {
            if (lockMove) return;

            if (this.WindowState == FormWindowState.Normal) {
                this.config.Left = this.Left;
                this.config.Top = this.Top;
            }
        }

        private void FrameCtrl_Resize(object sender, EventArgs e)
        {
            if (lockMove) return;

            if (this.WindowState == FormWindowState.Normal)
            {
                this.config.Width = this.Width;
                this.config.Height = this.Height;
            }
        }

        public void CenterOnCurrentScreen()
        {
            Screen currentScreen = Screen.FromControl(this);

            var workingArea = currentScreen.WorkingArea;

            this.Left = workingArea.Left + (workingArea.Width - this.Width) / 2;
            this.Top = workingArea.Top + (workingArea.Height - this.Height) / 2;
        }

        public bool IsWindowSuccessfullyVisible()
        {
            if (!this.Visible || this.WindowState == FormWindowState.Minimized)
            {
                return false;
            }

            bool isOnScreen = false;
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(this.Bounds))
                {
                    isOnScreen = true;
                    break;
                }
            }

            return isOnScreen;
        }

        public HistoryItem AddOrFindHistoryItem(string videoPath)
        {
            if (File.Exists(videoPath)) {        
                string hash = FileHelper.GetPartialHash(videoPath);

                foreach (HistoryItem item in config.history) {
                    if (item.VideoHash == hash) {
                        this.historyItem = item;
                        this.historyItem.VideoPath = videoPath;
                        return this.historyItem;
                    }
                }

                this.historyItem = new HistoryItem();
                this.historyItem.VideoHash = hash;
                this.historyItem.VideoPath = videoPath;
                config.history.Add(this.historyItem);
                return this.historyItem;
            }

            return null;
        }

        private void PlaylistNext()
        {
            if (this.config.playlist.Count > 0  && this.config.playlistPosition < this.config.playlist.Count-1) {
                this.config.playlistPosition++;
                this.openVideoFile(this.config.playlist[this.config.playlistPosition]);
            }
        }

        private void PlaylistPrev()
        {
            if (this.config.playlist.Count > 0 && this.config.playlistPosition > 0)
            {
                this.config.playlistPosition--;
                this.openVideoFile(this.config.playlist[this.config.playlistPosition]);
            }

        }

    }
}
