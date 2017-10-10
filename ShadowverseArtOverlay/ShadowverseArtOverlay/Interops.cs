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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

    }
}
