using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCtrl
{
    public class VideoHelper
    {
        private static readonly HashSet<string> VideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm", ".m4v"
        };

        public static bool IsVideoFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            string extension = Path.GetExtension(path);
            return VideoExtensions.Contains(extension);
        }
    }
}
