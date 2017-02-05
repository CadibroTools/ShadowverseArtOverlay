using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SlimDX;
using D3D = SlimDX.Direct3D9;

namespace ShadowverseArtOverlay
{
    public partial class DxOverlay : Form
    {
        private const int GwlExstyle = -20;
        private const int WsExLayered = 0x80000;
        private const int WsExTransparent = 0x20;
        private const int LwaAlpha = 0x2;

        private Margins _marg;

        private readonly D3D.Device _device;
        private bool _running;
        private bool _hudToggled = true;

        private readonly Dictionary<string, D3D.Texture> _textureCache = new Dictionary<string, D3D.Texture>();
        public float CardScale = .75f;
        public int NormalX = 1000;
        public int NormalY = 200;
        public int EvolvedX = 1000;
        public int EvolvedY = 600;

        public DxOverlay()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            ShowIcon = false;
            ShowInTaskbar = false;
            TopMost = true;
            WindowState = FormWindowState.Maximized;

            //Make the window's border completely transparant
            SetWindowLong(Handle, GwlExstyle,
                (IntPtr)(GetWindowLong(Handle, GwlExstyle) ^ WsExLayered ^ WsExTransparent));

            //Set the Alpha on the Whole Window to 255 (solid)
            SetLayeredWindowAttributes(Handle, 0, 255, LwaAlpha);

            //Init DirectX
            //This initializes the DirectX device. It needs to be done once.
            //The alpha channel in the backbuffer is critical.
            var presentParameters = new D3D.PresentParameters
            {
                Windowed = true,
                SwapEffect = D3D.SwapEffect.Discard,
                BackBufferFormat = D3D.Format.A8R8G8B8,
                BackBufferHeight = ClientRectangle.Height,
                BackBufferWidth = ClientRectangle.Width
            };

            _device = new D3D.Device(new D3D.Direct3D(), 0, D3D.DeviceType.Hardware, Handle, D3D.CreateFlags.HardwareVertexProcessing, presentParameters);

            _running = true;
            new Thread(DxThread) { IsBackground = true }.Start();
        }

        private D3D.Texture GetTexture(string textureName)
        {
            if (_textureCache.ContainsKey(textureName))
                return _textureCache[textureName];

            var filePath = Path.GetFullPath("Art/" + textureName + ".png");

            if (filePath != string.Empty)
            {
                D3D.ImageInformation info;
                D3D.PaletteEntry[] palette;
                var texture = D3D.Texture.FromFile(_device, filePath,
                    536, 698, 0, D3D.Usage.None, D3D.Format.A8B8G8R8, D3D.Pool.Default, D3D.Filter.Default,
                    D3D.Filter.Default, 0, out info, out palette);

                _textureCache[textureName] = texture;
                return texture;
            }

            return null;
        }

        private D3D.Texture GetEvolvedTexture(string textureName)
        {
            textureName += "_Evolved";
            if (_textureCache.ContainsKey(textureName))
                return _textureCache[textureName];

            var filePath = Path.GetFullPath("Art/" + textureName + ".png");

            if (filePath != string.Empty)
            {
                D3D.ImageInformation info;
                D3D.PaletteEntry[] palette;
                var texture = D3D.Texture.FromFile(_device, filePath,
                    536, 698, 0, D3D.Usage.None, D3D.Format.A8B8G8R8, D3D.Pool.Default, D3D.Filter.Default,
                    D3D.Filter.Default, 0, out info, out palette);

                _textureCache[textureName] = texture;
                return texture;
            }

            return null;
        }

        public void ToggleHudOff()
        {
            _hudToggled = false;
        }

        public void ToggleHudOn()
        {
            _hudToggled = true;
        }

        public void KillOverlay()
        {
            _running = false;
        }

        private void DxThread()
        {
            while (_running)
            {
                try
                {
                    //Place your update logic here
                    _device.Clear(D3D.ClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 1.0f, 0);
                    _device.SetRenderState(D3D.RenderState.ZEnable, true);
                    _device.SetRenderState(D3D.RenderState.Lighting, true);
                    _device.SetRenderState(D3D.RenderState.CullMode, D3D.Cull.None);
                    _device.SetTransform(D3D.TransformState.Projection, Matrix.OrthoOffCenterLH(0, Width, Height, 0, 0, 1));

                    _device.BeginScene();

                    if (_hudToggled)
                    {
                        if (ShadowverseProcessReader.IsCardSelected)
                        {
                            var sprite = new D3D.Sprite(_device);
                            sprite.Begin(D3D.SpriteFlags.AlphaBlend);
                            var texture = GetTexture(ShadowverseProcessReader.SelectedCard.Replace(" ", "_"));

                            if (texture != null)
                            {
                                //Scale the card
                                Matrix spriteTransformMatrix = Matrix.Scaling(CardScale, CardScale, CardScale);

                                //Move the card
                                Matrix spriteTranslationMatrix = Matrix.Translation(NormalX, NormalY, 0);
                                sprite.Transform = spriteTransformMatrix*spriteTranslationMatrix;

                                sprite.Draw(texture, null, null, null, Color.White);

                                sprite.End();
                                sprite.Dispose();
                            }

                            //Check for evolved
                            var evolvedTexture =
                                GetEvolvedTexture(ShadowverseProcessReader.SelectedCard.Replace(" ", "_"));
                            if (evolvedTexture != null)
                            {
                                var evolvedSprite = new D3D.Sprite(_device);
                                evolvedSprite.Begin(D3D.SpriteFlags.AlphaBlend);
                                //Scale the card
                                Matrix spriteTransformMatrix = Matrix.Scaling(CardScale, CardScale, CardScale);

                                //Move the card
                                Matrix spriteTranslationMatrix = Matrix.Translation(EvolvedX, EvolvedY, 0);
                                evolvedSprite.Transform = spriteTransformMatrix * spriteTranslationMatrix;

                                evolvedSprite.Draw(evolvedTexture, null, null, null, Color.White);

                                evolvedSprite.End();
                                evolvedSprite.Dispose();
                            }

                        }

                    }

                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                finally
                {
                    try
                    {
                        _device.EndScene();
                        _device.Present();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _running = false;
                _device.Dispose();
                Application.Exit();
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Create a margin (the whole form)
            _marg.Left = 0;
            _marg.Top = 0;
            _marg.Right = Width;
            _marg.Bottom = Height;

            //Expand the Aero Glass Effect Border to the WHOLE form.
            // since we have already had the border invisible we now
            // have a completely invisible window - apart from the DirectX
            // renders NOT in black.
            DwmExtendFrameIntoClientArea(Handle, ref _marg);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("dwmapi.dll")]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        //this is used to specify the boundaries of the transparent area
        internal struct Margins
        {
            public int Left, Right, Top, Bottom;
        }
    }
}