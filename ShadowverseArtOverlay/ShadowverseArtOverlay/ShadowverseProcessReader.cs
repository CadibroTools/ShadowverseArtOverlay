using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShadowverseArtOverlay
{
    public static class ShadowverseProcessReader
    {
        private static readonly int IsCardSelectedBase;
        private static readonly int[] IsCardSelectedPointer;
        private static readonly int SelectedCardNameBase;
        private static readonly int[] SelectedCardNamePointer;
        private static readonly int SelectedCardNameOffset;

        //Pointer to an int that happens to have a value when a card is selected, this will change after game updates
        public static bool IsCardSelected
            =>
                Memory != null && _memory.ReadInt(_memory.MonoBaseAddress + IsCardSelectedBase, IsCardSelectedPointer) != 0;

        //Pointer to the selected card's name, this will change after game updates
        public static string SelectedCard
            =>
                Memory == null
                    ? string.Empty
                    : _memory.ReadStringU(_memory.ReadInt(_memory.MonoBaseAddress + SelectedCardNameBase, SelectedCardNamePointer) + SelectedCardNameOffset);

        public static bool IsShadowverseRunning { get; private set; }
        private static Rectangle _rect;

        public static Rectangle Window
        {
            get
            {
                if (_process == null)
                {
                    _rect = Rectangle.Empty;
                    return _rect;
                }

                if (_rect == Rectangle.Empty)
                {
                    IntPtr window = _process.MainWindowHandle;
                    Interops.Rect windowRect = new Interops.Rect();
                    Interops.GetWindowRect(window, ref windowRect);
                    _rect = new Rectangle(windowRect.Left, windowRect.Top, windowRect.Right - windowRect.Left,
                        windowRect.Bottom - windowRect.Top);
                }
                
                return _rect;
            }
        }

        static ShadowverseProcessReader()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            
            IsCardSelectedBase = Convert.ToInt32(config.AppSettings.Settings["IsCardSelectedBase"].Value, 16);
            string cardIsSelectedArrayString = config.AppSettings.Settings["IsCardSelected"].Value;
            IsCardSelectedPointer = cardIsSelectedArrayString.Split(',').Select(s => Convert.ToInt32(s, 16)).ToArray();

            SelectedCardNameBase = Convert.ToInt32(config.AppSettings.Settings["SelectedCardNameBase"].Value, 16);
            string selectedCardNameString = config.AppSettings.Settings["SelectedCardName"].Value;
            SelectedCardNamePointer = selectedCardNameString.Split(',').Select(s => Convert.ToInt32(s, 16)).ToArray();
            SelectedCardNameOffset = Convert.ToInt32(config.AppSettings.Settings["SelectedCardNameOffset"].Value, 16);

            SynchToShadowverseProcess();
        }
        private static Memory _memory;
        private static Process _process;
        private static Memory Memory
        {
            get
            {
                SynchToShadowverseProcess();
                return _memory;
            }
        }
        private static void SynchToShadowverseProcess()
        {
            _process = FindShadowverseProcess();
            if (_process == null)
            {
                IsShadowverseRunning = false;
                _process = null;
                return;
            }

            IsShadowverseRunning = true;

            _memory = new Memory(_process.Id);
        }

        private static Process FindShadowverseProcess()
        {
            var client = Process.GetProcessesByName("Shadowverse");
            if (client.Length == 0)
            {
                return null;
            }

            return client[0];
        }

        
    }
}
