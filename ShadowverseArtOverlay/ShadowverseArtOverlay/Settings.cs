using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;
using ShadowverseArtOverlay.Properties;

namespace ShadowverseArtOverlay
{
    public partial class Settings : Form
    {
        private readonly DxOverlay _overlay;
        private readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

        public Settings(DxOverlay overlay)
        {
            InitializeComponent();
            _overlay = overlay;
            notifyIcon1.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon1.Icon = Resources.Daria;
            Resize += Form_Resize;

            var contextMenu1 = new ContextMenu();
            contextMenu1.MenuItems.Add("Open", NotifyIcon_DoubleClick);
            contextMenu1.MenuItems.Add("Exit", (s, e) => Application.Exit());
            notifyIcon1.ContextMenu = contextMenu1;

            float cardScale = .6f;
            int normalX = 1000;
            int normalY = 500;
            int evolvedX = 1000;
            int evolvedY = 800;

            var cardScaleText = _config.AppSettings.Settings["CardScale"]?.Value;
            var normalXText = _config.AppSettings.Settings["NormalX"]?.Value;
            var normalYText = _config.AppSettings.Settings["NormalY"]?.Value;
            var evolvedXText = _config.AppSettings.Settings["EvolvedX"]?.Value;
            var evolvedYText = _config.AppSettings.Settings["EvolvedY"]?.Value;

            if (!string.IsNullOrEmpty(cardScaleText))
                float.TryParse(cardScaleText, out cardScale);
            if (!string.IsNullOrEmpty(normalXText))
                int.TryParse(normalXText, out normalX);
            if (!string.IsNullOrEmpty(normalYText))
                int.TryParse(normalYText, out normalY);
            if (!string.IsNullOrEmpty(evolvedXText))
                int.TryParse(evolvedXText, out evolvedX);
            if (!string.IsNullOrEmpty(evolvedYText))
                int.TryParse(evolvedYText, out evolvedY);

            _overlay.CardScale = cardScale;
            _overlay.NormalX = normalX;
            _overlay.NormalY = normalY;
            _overlay.EvolvedX = evolvedX;
            _overlay.EvolvedY = evolvedY;

            textBox1.Text = (cardScale*100).ToString(CultureInfo.InvariantCulture);
            textBox2.Text = normalX.ToString();
            textBox3.Text = normalY.ToString();
            textBox4.Text = evolvedX.ToString();
            textBox5.Text = evolvedY.ToString();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                ShowInTaskbar = false;
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            Visible = true;
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            _overlay.KillOverlay();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();

                _overlay.KillOverlay();
                _overlay.Dispose();
            }
            base.Dispose(disposing);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int scale;
            if (int.TryParse(textBox1.Text.Replace("%", ""), out scale))
            {
                if (trackBar1.Value != scale && scale >= trackBar1.Minimum && scale <= trackBar1.Maximum)
                {
                    trackBar1.Value = scale;
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _overlay.CardScale = (float)trackBar1.Value / 100;

            if (textBox1.Text != $"{trackBar1.Value}%" && textBox1.Text != $"{trackBar1.Value}")
            {
                textBox1.Text = $"{trackBar1.Value}%";
                SaveConfiguration("CardScale", ((float)trackBar1.Value / 100).ToString(CultureInfo.InvariantCulture));
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int newX;
            if (int.TryParse(textBox2.Text, out newX))
            {
                _overlay.NormalX = newX;
                SaveConfiguration("NormalX", textBox2.Text);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int newY;
            if (int.TryParse(textBox3.Text, out newY))
            {
                _overlay.NormalY = newY;
                SaveConfiguration("NormalY", textBox3.Text);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int newX;
            if (int.TryParse(textBox4.Text, out newX))
            {
                _overlay.EvolvedX = newX;
                SaveConfiguration("EvolvedX", textBox4.Text);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int newY;
            if (int.TryParse(textBox5.Text, out newY))
            {
                _overlay.EvolvedY = newY;
                SaveConfiguration("EvolvedY", textBox5.Text);
            }
        }

        private void SaveConfiguration(string key, string value)
        {
            _config.AppSettings.Settings.Remove(key);
            _config.AppSettings.Settings.Add(key, value);

            _config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
