using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCtrl
{
    public class Config
    {        
        public List<HistoryItem> videoFilesHistory = new List<HistoryItem>();
        public List<PlaylistHistoryItem> playlistFilesHistory = new List<PlaylistHistoryItem>();

        public int Left = -1;
        public int Top = -1;
        public int Width = 300;
        public int Height = 300;
        public bool MostTop = false;
        public bool Muted = false;

        public PlaylistHistoryItem AddOrFindPlaylistItem(string playlistPath)
        {
            if (!File.Exists(playlistPath))
            {
                return null;
            }

            foreach (PlaylistHistoryItem item in playlistFilesHistory)
            {
                if (item.playlistPath == playlistPath)
                {
                    return item;
                }
            }

            PlaylistHistoryItem playlistHistoryItem = new PlaylistHistoryItem();
            playlistHistoryItem.playlistPath = playlistPath;
            this.playlistFilesHistory.Add(playlistHistoryItem);
            return playlistHistoryItem;
        }

        public HistoryItem AddOrFindHistoryItem(string videoPath)
        {
            if (!File.Exists(videoPath))
            {
                return null;
            }

            foreach (HistoryItem item in videoFilesHistory)
            {
                if (item.VideoPath == videoPath)
                {
                    return item;
                }
            }

            string hash = FileHelper.GetPartialHash(videoPath);

            foreach (HistoryItem item in this.videoFilesHistory)
            {
                if (item.VideoHash == hash)
                {
                    item.VideoPath = videoPath;
                    return item;
                }
            }

            HistoryItem historyItem = new HistoryItem();
            historyItem.VideoHash = hash;
            historyItem.VideoPath = videoPath;
            this.videoFilesHistory.Add(historyItem);
            return historyItem;
        }
    }
}
