using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ShadowverseArtOverlay
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Directory.Exists("Art"))
            {
                Process.Start("DownloadShadowverseArt.exe");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var dxOverlay = new DxOverlay();
            dxOverlay.Show();
            Application.Run(new Settings(dxOverlay));
        }
    }
}
