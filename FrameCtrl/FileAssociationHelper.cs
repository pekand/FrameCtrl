using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;

namespace FrameCtrl
{
    public static class FileAssociationHelper
    {

        public static bool IsAlreadyAssociated(string extension, string progId)
        {
            using (RegistryKey extKey = Registry.CurrentUser.OpenSubKey($@"Software\Classes\{extension}"))
            {
                if (extKey == null || extKey.GetValue("")?.ToString() != progId)
                    return false;
            }

            string currentExe = Process.GetCurrentProcess().MainModule.FileName;
            string registryCommandPath = $@"Software\Classes\{progId}\shell\open\command";

            using (RegistryKey cmdKey = Registry.CurrentUser.OpenSubKey(registryCommandPath))
            {
                if (cmdKey == null) return false;

                string registeredCommand = cmdKey.GetValue("")?.ToString();

                return registeredCommand != null && registeredCommand.Contains(currentExe);
            }
        }

        public static void AssociateExtension(string extension, string progId, string description)
        {
            if (!extension.StartsWith(".")) extension = "." + extension;

            string applicationPath = Process.GetCurrentProcess().MainModule.FileName;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($@"Software\Classes", true))
            {
                using (RegistryKey extKey = key.CreateSubKey(extension))
                {
                    extKey.SetValue("", progId);
                }

                using (RegistryKey progKey = key.CreateSubKey(progId))
                {
                    progKey.SetValue("", description);
                    using (RegistryKey shellKey = progKey.CreateSubKey(@"shell\open\command"))
                    {
                        // The "%1" is a placeholder for the file path being opened
                        shellKey.SetValue("", $"\"{applicationPath}\" \"%1\"");
                    }
                }
            }

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
