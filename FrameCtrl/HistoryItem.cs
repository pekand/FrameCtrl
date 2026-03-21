using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCtrl
{
    public class HistoryItem
    {
        public string VideoHash = "";
        public string VideoPath = "";
        public long Position = 0;
        public bool finished = false;
    }
}
