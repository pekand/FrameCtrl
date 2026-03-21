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
        public string videoPath = null;
        public long currentPosition = 0;
        public float currentPercent = 0;
        public string playlistPath = null;
        public int playlistPosition = 0;
        public List<string> playlist = new List<string>();
        public List<HistoryItem> history = new List<HistoryItem>();

        public int Left = -1;
        public int Top = -1;
        public int Width = 300;
        public int Height = 300;
    }
}
