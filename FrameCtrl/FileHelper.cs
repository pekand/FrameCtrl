using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FrameCtrl
{
    public class FileHelper
    {
        private const int TenMegaBytes = 10 * 1024 * 1024;

        public static string GetPartialHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    long bytesToRead = Math.Min(stream.Length, TenMegaBytes);
                    byte[] buffer = new byte[bytesToRead];

                    stream.Read(buffer, 0, buffer.Length);

                    byte[] hashBytes = sha256.ComputeHash(buffer);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
