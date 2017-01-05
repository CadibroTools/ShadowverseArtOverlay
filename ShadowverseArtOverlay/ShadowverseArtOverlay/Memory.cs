using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShadowverseArtOverlay
{
    public class Memory : IDisposable
    {
        public readonly int BaseAddress;
        public readonly int MonoBaseAddress;
        private bool _closed;
        private IntPtr _procHandle;

        public Memory(int pId)
        {
            try
            {
                Process = Process.GetProcessById(pId);
                BaseAddress = Process.MainModule.BaseAddress.ToInt32();
                for (int i = 0; i < Process.Modules.Count; i++)
                {
                    if (Process.Modules[i].ModuleName == "mono.dll")
                    {
                        MonoBaseAddress = Process.Modules[i].BaseAddress.ToInt32();
                    }
                }
                Open();
            }
            catch (Win32Exception ex)
            {
                throw new Exception("Insufficient Privileges, try running as Administrator", ex);
            }
        }

        public Process Process { get; }

        public void Dispose()
        {
            if (!_closed)
            {
                _closed = true;
                Interops.CloseHandle(_procHandle);
            }
        }

        private void Open()
        {
            _procHandle = Interops.OpenProcess(2035711u, false, Process.Id);
        }

        public int ReadInt(int addr)
        {
            return BitConverter.ToInt32(ReadMem(addr, 4), 0);
        }

        public int ReadInt(int addr, params int[] offsets)
        {
            int num = ReadInt(addr);
            var ret = offsets.Aggregate(num, (current, num2) => ReadInt(current + num2));
            return ret;
        }

        private static string RTrimNull(string text)
        {
            int num = text.IndexOf('\0');
            return num > 0 ? text.Substring(0, num) : text;
        }

        public string ReadStringU(int addr, int length = 256, bool replaceNull = true)
        {
            if (addr <= 65536 && addr >= -1)
            {
                return string.Empty;
            }
            byte[] mem = ReadMem(addr, length);
            if (mem[0] == 0 && mem[1] == 0)
                return string.Empty;
            string @string = Encoding.Unicode.GetString(mem);
            return replaceNull ? RTrimNull(@string) : @string;
        }

        private byte[] ReadMem(long addr, int size)
        {
            byte[] array = new byte[size];
            Interops.ReadProcessMemory(_procHandle, (int)addr, array, size, 0);
            return array;
        }
    }
}
