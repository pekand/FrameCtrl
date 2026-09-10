using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Numerics;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace FrameCtrl
{
    public partial class FrameCtrl : Form
    {
        public Config config = new Config();

        private LibVLC libVLC;
        private Media media;
        private MediaPlayer mediaplayer;

        private bool lockMove = false;

        public HistoryItem video = null;
        public PlaylistHistoryItem playlist = null;

        // CONSTRUCTOR
        public FrameCtrl(Config config, string filePath = null)
        {
            this.config = config;

            InitializeComponent();
            SetupPlayer();

            string playlistPath = null;
            string videoPath = null;

            if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath);

                if (extension == Program.defaultExtension)
                {
                    playlistPath = filePath;
                }
                else
                {
                    videoPath = filePath;
                }

            }

            if (playlistPath != null && File.Exists(playlistPath))
            {
                playlist = config.AddOrFindPlaylistItem(playlistPath);
                LoadPlaylist();
            }

            if (videoPath != null && File.Exists(videoPath) && VideoHelper.IsVideoFile(videoPath))
            {
                this.playlist = null;
                video = config.AddOrFindHistoryItem(videoPath);
            }
        }

        // EVENT LOAD
        private void FrameCtrl_Load(object sender, EventArgs e)
        {
            lockMove = true;
            this.Left = this.config.Left;
            this.Top = this.config.Top;
            this.Width = this.config.Width;
            this.Height = this.config.Height;
            this.TopMost = this.config.MostTop;

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

            if (!this.IsWindowSuccessfullyVisible())
            {
                this.Width = 300;
                this.Height = 300;
                this.CenterOnCurrentScreen();
            }

            lockMove = false;

            if (video != null)
            {
                this.openVideoFile(video.VideoPath, video.Position);
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

            if (this.video != null && mediaplayer != null)
            {
                this.video.Position = mediaplayer.Time;
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
                    if (this.video != null) this.video.finished = !this.video.finished;
                    return true;

                case Keys.S:
                    ShowLabel(this.video.VideoPath);
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
                string playlistPath = null;

                List<string> videos = new List<string>();
                foreach (string filePath in files)
                {
                    if (File.Exists(filePath))
                    {
                        if (VideoHelper.IsVideoFile(filePath))
                        {
                            videos.Add(filePath);
                            playlistPath = null;
                        }

                        if (Path.GetExtension(filePath) == Program.defaultExtension)
                        {
                            playlistPath = filePath;
                        }
                    }
                }

                if (playlistPath != null)
                {
                    playlist = config.AddOrFindPlaylistItem(playlistPath);
                    LoadPlaylist();

                    if (this.video != null)
                    {
                        this.openVideoFile(this.video.VideoPath, this.video.Position);
                    }

                }
                else if (videos.Count > 0)
                {
                    playlist = config.AddOrFindPlaylistItem("");
                    playlist.playlistPosition = 0;
                    playlist.playlistPath = "";

                    foreach (string video in videos)
                    {
                        playlist.videList.Add(video);
                    }

                    this.openVideoFile(playlist.videList[playlist.playlistPosition]);
                }

            }
        }

        // CONTEXTMENU OPEN
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "FrameCtrl PlayList|*.FrameCtrl|Video Files|*.mp4;*.mkv;*.avi;*.webm|All files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
                {
                    OpenFile(openFileDialog.FileName);
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

        public void BringFormTofront()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            this.BringToFront();
            this.Activate();
        }

        public bool HasVideoAssigned() {
            if (this.video != null) {
                return true;
            }

            return false;
        }

        public void OpenFile(string filePath)
        {
            string playlistPath = null;
            string videoPath = null;

            if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath);

                if (extension == Program.defaultExtension)
                {
                    playlistPath = filePath;
                    playlist = config.AddOrFindPlaylistItem(playlistPath);
                    LoadPlaylist();

                    if (video != null)
                    {
                        this.openVideoFile(video.VideoPath, video.Position);
                    }
                }
                else
                {
                    this.playlist = null;
                    videoPath = filePath;
                    this.openVideoFile(filePath);
                }
            }
        }

        public bool openVideoFile(string videoPath, long skipTime = 0)
        {

            if (!File.Exists(videoPath))
            {
                return false;
            }

            if (this.video != null && mediaplayer != null)
            {
                this.video.Position = mediaplayer.Time;
            }

            this.video = config.AddOrFindHistoryItem(videoPath);

            if (skipTime == 0 && this.video != null && this.video.Position != 0)
            {
                skipTime = this.video.Position;
            }

            if (mediaplayer != null)
            {
                this.videoView.MediaPlayer = null;
                this.mediaplayer.Dispose();
                this.media.Dispose();
            }

            this.video.VideoPath = videoPath;
            this.media = new Media(this.libVLC, new Uri(videoPath), ":play-and-pause");
            this.mediaplayer = new MediaPlayer(media);
            this.videoView.MediaPlayer = this.mediaplayer;
            this.videoView.ContextMenuStrip = contextMenuStrip;
            mediaplayer.EnableMouseInput = false;
            mediaplayer.EnableKeyInput = false;
            this.video.Position = 0;

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

            ShowLabel(videoPath);

            return true;
        }

        private void PositionChange()
        {
            this.video.Position = mediaplayer.Time;
        }

        private void EndReached()
        {
            if (this.video != null) // reset to beggining but mark as finished
            {
                this.video.Position = 0;
                this.video.finished = true;
                this.video.Position = 0;
                this.mediaplayer.Time = 0;
            }
            PlaylistNext();
        }

        private void UpdateTitle(long currentTime)
        {
            if (mediaplayer == null) return;

            long totalTime = mediaplayer.Length; // Total duration in ms

            string currentStr = FormatTime(currentTime);
            string totalStr = FormatTime(totalTime);
            string finished = (this.video != null && this.video.finished) ? " - FINISHED" : "";

            this.Text = $"FrameCtrl - {currentStr} / {totalStr}" + finished;
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

            if (this.WindowState == FormWindowState.Normal)
            {
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

        private void PlaylistNext()
        {
            if (this.playlist != null && this.playlist.videList.Count > 0 && this.playlist.playlistPosition < this.playlist.videList.Count - 1)
            {
                this.playlist.playlistPosition++;
                this.openVideoFile(this.playlist.videList[this.playlist.playlistPosition]);
            }
        }

        private void PlaylistPrev()
        {
            if (this.playlist != null && this.playlist.videList.Count > 0 && this.playlist.playlistPosition > 0)
            {
                this.playlist.playlistPosition--;
                this.openVideoFile(this.playlist.videList[this.playlist.playlistPosition]);
            }

        }

        private void ShowLabel(string text)
        {
            StatusLabel.Text = text;
            StatusLabel.Visible = true;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000; // 5 seconds
            timer.Tick += (s, e) =>
            {
                StatusLabel.Visible = false;
                timer.Stop();
                timer.Dispose(); // Clean up memory
            };

            timer.Start();
        }

        public void LoadPlaylist()
        {
            if (!File.Exists(playlist.playlistPath))
            {
                return;
            }

            List<string> videoFiles = FileHelper.GetFullPathsFromFileLines(playlist.playlistPath);
            foreach (string videoFile in videoFiles)
            {
                if (VideoHelper.IsVideoFile(videoFile))
                {
                    this.playlist.videList.Add(videoFile);
                }
            }


            if (this.playlist.videList.Count > 0)
            {
                HistoryItem video = null;
                this.playlist.playlistPosition = 0;
                foreach (string videoFile in this.playlist.videList)
                {

                    video = config.AddOrFindHistoryItem(videoFile);
                    if (!video.finished)
                    {
                        break;
                    }

                    this.playlist.playlistPosition++;
                }

                if (video != null)
                {
                    this.video = video;
                }
            }
        }

        private void videoView_DragEnter(object sender, DragEventArgs e)
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

        private void videoView_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                OpenFile(files[0]);
                return;
            }

            foreach (string filePath in files)
            {
                string[] args = new string[] { filePath };
                Program.context.CreateNewForm(args, this.config);
            }
        }

        private void mostTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.config.MostTop = !this.config.MostTop;
            mostTopToolStripMenuItem.Checked = this.config.MostTop;
            this.TopMost = this.config.MostTop;
        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mostTopToolStripMenuItem.Checked = this.config.MostTop;
        }
    }
}
