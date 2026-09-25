using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SerialPortListener
{
    class Utils
    {
        public static string hashPassword(string password)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] password_bytes = Encoding.ASCII.GetBytes(password);
            byte[] encrypted_bytes = sha1.ComputeHash(password_bytes);
            return Convert.ToBase64String(encrypted_bytes);
        }

        // Each branch/build has its own AssemblyProduct (Blue = "SerialPortListenerM", Pink = "SerialPortListenerS", ...).
        // Use it to derive a per-build AppData folder so Blue and Pink never read/write each other's config/log files
        // when installed on the same machine. "SerialPortListenerM" keeps the original folder name so existing Blue
        // installs keep their saved configuration; any other product name gets its own dedicated folder.
        public static readonly string AppDataDir = GetAppDataDir();

        private static string GetAppDataDir()
        {
            string product = System.Windows.Forms.Application.ProductName;
            string folderName = (product == "SerialPortListenerM")
                ? "SLCBlueSerialPortListener"
                : "SLC" + product;
            return System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                folderName);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);

        private const int AW_BLEND = 0x00080000;

        // แสดง control ด้วยเอฟเฟกต์ค่อยๆ ปรากฏ (fade-in) แทนการโผล่มาทันทีเหมือน .Show() เฉยๆ
        // ต้องเรียกตอน control ยังซ่อนอยู่ (Visible = false) ไม่งั้นจะไม่มีอะไรให้ animate
        public static void ShowWithFade(Control control, int durationMs = 220)
        {
            AnimateWindow(control.Handle, durationMs, AW_BLEND);
            control.Visible = true;
        }
    }
}
