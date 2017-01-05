using System;
using System.Runtime.InteropServices;

namespace ShadowverseArtOverlay
{
    public class Interops
    {
        [DllImport("kernel32.dll")]
        public static extern void ReadProcessMemory(IntPtr hProcess, int baseAddress, byte[] buffer, int size, int bytesRead);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint access, bool inheritHandle, int processId);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

    }
}
