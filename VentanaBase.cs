using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Terminal
{
    public static class ThemeColors
    {
        public static Color Background = Color.FromArgb(6, 32, 64);     // Azul oscuro (Fondo)
        public static Color Foreground = Color.FromArgb(45, 180, 200);  // Cian (Texto y bordes)
        public static Color Scanline = Color.FromArgb(25, 0, 0, 0);     // Oscurecimiento para líneas CRT   40
        public static Color Highlight = Color.FromArgb(80, 220, 255);   // Cian claro (Hover)

        public static Color CyanPrimary = Color.FromArgb(0, 210, 255);    // Cian brillante UI
        public static Color CyanDark = Color.FromArgb(0, 70, 110);        // Cian tenue (líneas de rejilla)
        public static Color GreenNode = Color.FromArgb(0, 255, 160);      // Verde neón (Nodos ADN)
    }
    public class VentanaBase : Form
    {
        // Importación de API Win32 para el arrastre de la ventana
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        public bool is_active { get; set; } = true;

        // Estructura de Controles
        protected Panel HeaderPanel;
        protected Panel FooterPanel;
        protected Panel BodyPanel;
        protected Panel LeftBorder;
        protected Panel RightBorder;

        protected TextButton BtnMaximize;
        protected TextButton BtnMinimize;
        protected SwitchButton BtnCloseSwitch;
        protected Label LblTitle;

        protected CyberContainer MainContainer;

        // Estado y Dimensiones
        private bool _isMinimized = false;
        private bool _isClosing = false;

        private int _originalLeft;
        private int _originalTop;
        private int _originalWidth;
        private int _originalHeight;
        public int TargetWidth { get; set; } = 900;
        public int TargetHeight { get; set; } = 550;

        public int CompactWidth { get; set; } = 900 / 2;
        public int CompactHeight { get; set; } = 550 / 2;

        // Propiedad Text sobrecargada para actualizar el Label del título
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                if (LblTitle != null)
                {
                    LblTitle.Text = value;
                }
            }
        }

        public VentanaBase() { InitForm(); }

        // Constructor sobrecargado para definir tamaño explícito al instanciar
        public VentanaBase(int targetWidth, int targetHeight)
        {
            TargetWidth = targetWidth;
            TargetHeight = targetHeight;
            InitForm();
        }

        private void InitForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.FromArgb(28, 170, 218);
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.Size = new Size(TargetWidth, TargetHeight);

            InitializeCustomUI();
        }

        private void InitializeCustomUI()
        {
            int headerH = 25;
            int footerH = 20;

            // 1. Header (Barra Superior)
            HeaderPanel = new Panel { Dock = DockStyle.Top, Height = headerH, BackColor = ThemeColors.Background };
            HeaderPanel.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); } };
            HeaderPanel.Paint += (s, e) =>
            {
                // Borde inferior cian de la cabecera
                using (Pen p = new Pen(ThemeColors.Foreground, 2))
                    e.Graphics.DrawLine(p, 0, HeaderPanel.Height - 1, HeaderPanel.Width, HeaderPanel.Height - 1);
            };

            // Botones de Texto (Izquierda)
            BtnMaximize = new TextButton { Text = "MAXIMIZAR", Location = new Point(10, 5) };
            BtnMinimize = new TextButton { Text = "MINIMIZAR", Location = new Point(105, 5) };

            HeaderPanel.Controls.Add(BtnMaximize);
            HeaderPanel.Controls.Add(BtnMinimize);

            BtnMaximize.Click += async (s, e) => await ExpandWindowAsync();
            BtnMinimize.Click += async (s, e) => await CollapseWindowAsync();

            LblTitle = new Label
            {
                Text = string.IsNullOrEmpty(this.Text) ? "" : this.Text,
                ForeColor = ThemeColors.Foreground,
                Font = new Font("Courier New", 10F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            HeaderPanel.Controls.Add(LblTitle);
            HeaderPanel.Resize += (s, e) => LblTitle.Location = new Point((HeaderPanel.Width - LblTitle.Width) / 2, 3);

            // Botón Switch Cerrar (Derecha)
            BtnCloseSwitch = new SwitchButton();
            BtnCloseSwitch.Click += async (s, e) => await CloseSequenceAsync();

            HeaderPanel.Controls.Add(BtnCloseSwitch);
            HeaderPanel.Resize += (s, e) => BtnCloseSwitch.Location = new Point(HeaderPanel.Width - BtnCloseSwitch.Width - 10, 3);

            // 2. Footer (Barra Inferior)
            FooterPanel = new Panel { Dock = DockStyle.Bottom, Height = footerH, BackColor = ThemeColors.Background };
            FooterPanel.Paint += FooterPanel_Paint;

            // 3. Cuerpo de la Ventana
            BodyPanel = new Panel { Dock = DockStyle.Fill, BackColor = ThemeColors.Background };
            MainContainer = new CyberContainer { Dock = DockStyle.Fill };

            BodyPanel.Controls.Add(MainContainer);

            // Ensamblado en el Formulario
            this.Controls.Add(BodyPanel);
            this.Controls.Add(FooterPanel);
            this.Controls.Add(HeaderPanel);
        }

        private void FooterPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen p = new Pen(ThemeColors.CyanPrimary, 1.5f))
            using (SolidBrush b = new SolidBrush(ThemeColors.CyanPrimary))
            {
                g.DrawLine(p, 0, 0, FooterPanel.Width, 0);

                // Indicador de segmento (Abajo Izquierda)
                g.FillRectangle(b, 10, 8, 30, 8);

                // Flechas de navegación (Abajo Derecha ▲ ▼)
                Point[] upArrow = { new Point(Width - 30, 16), new Point(Width - 25, 6), new Point(Width - 20, 16) };
                Point[] downArrow = { new Point(Width - 18, 6), new Point(Width - 13, 16), new Point(Width - 8, 6) };

                g.FillPolygon(b, upArrow);
                g.FillPolygon(b, downArrow);
            }
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await SpawnSequenceAsync();
        }

        #region Animaciones

        private int GetCollapsedHeight() => HeaderPanel.Height + FooterPanel.Height;

        private async Task SpawnSequenceAsync()
        {
            int collapsedH = GetCollapsedHeight();
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;

            int finalLeft = (screen.Width - TargetWidth) / 2;
            int finalTop = (screen.Height - TargetHeight) / 2;
            int initialTop = finalTop + ((TargetHeight - collapsedH) / 2);

            // Posición inicial colapsada
            this.Size = new Size(TargetWidth, collapsedH);
            this.Location = new Point(finalLeft, initialTop);

            await Task.Delay(300);

            // Transición de apertura
            BodyPanel.Visible = true;
            await AnimateBoundsAsync(finalLeft, finalTop, TargetWidth, TargetHeight, 450);

            _originalLeft = finalLeft;
            _originalTop = finalTop;
            _originalWidth = TargetWidth;
            _originalHeight = TargetHeight;
        }

        private async Task CollapseWindowAsync()
        {
            if (_isMinimized || _isClosing) return;

            _originalLeft = this.Left;
            _originalHeight = this.Height;
            _originalTop = this.Top;
            _originalWidth = this.Width;

            // Calcula nueva posición centrada para las dimensiones reducidas (CompactWidth x CompactHeight)
            int targetLeft = this.Left + ((this.Width - CompactWidth) / 2);
            int targetTop = this.Top + ((this.Height - CompactHeight) / 2);

            await AnimateBoundsAsync(targetLeft, targetTop, CompactWidth, CompactHeight, 350);
            _isMinimized = true;
        }

        private async Task ExpandWindowAsync()
        {
            if (!_isMinimized || _isClosing) return;

            await AnimateBoundsAsync(_originalLeft, _originalTop, _originalWidth, _originalHeight, 350);
            BodyPanel.Visible = true;
            _isMinimized = false;
        }

        public async Task ASK_Close ()
        {
            CloseSequenceAsync();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Marco Exterior Doble
            using (Pen pen = new Pen(ThemeColors.CyanPrimary, 2))
            {
                g.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
            }

            // Trama de líneas verticales de fondo
            using (Pen gridPen = new Pen(ThemeColors.CyanDark, 1))
            {
                for (int x = 0; x < Width; x += 15)
                {
                    g.DrawLine(gridPen, x, 32, x, Height - 24);
                }
            }
        }

        private async Task CloseSequenceAsync()
        {
            if (_isClosing) return;
            _isClosing = true;

            BtnCloseSwitch.IsOn = false;
            await Task.Delay(150);

            int collapsedH = GetCollapsedHeight();
            int targetTop = this.Top + ((this.Height - collapsedH) / 2);

            BodyPanel.Visible = false;
            await AnimateBoundsAsync(this.Left, targetTop, this.Width, collapsedH, 350);
            await Task.Delay(100);

            this.is_active = false;

            this.Close();
        }

        private async Task AnimateBoundsAsync(int targetLeft, int targetTop, int targetWidth, int targetHeight, int durationMs)
        {
            int startLeft = this.Left;
            int startTop = this.Top;
            int startWidth = this.Width;
            int startHeight = this.Height;

            int diffLeft = targetLeft - startLeft;
            int diffTop = targetTop - startTop;
            int diffWidth = targetWidth - startWidth;
            int diffHeight = targetHeight - startHeight;

            int fps = 60;
            int totalFrames = Math.Max(1, (durationMs * fps) / 1000);
            int frameDelay = 1000 / fps;

            for (int frame = 1; frame <= totalFrames; frame++)
            {
                float progress = (float)frame / totalFrames;
                // Curva Cúbica de Suavizado (Ease Out)
                float ease = 1f - (float)Math.Pow(1f - progress, 3);

                this.SuspendLayout();
                this.Left = startLeft + (int)(diffLeft * ease);
                this.Top = startTop + (int)(diffTop * ease);
                this.Width = startWidth + (int)(diffWidth * ease);
                this.Height = startHeight + (int)(diffHeight * ease);
                this.ResumeLayout(true);

                await Task.Delay(frameDelay);
            }

            this.SetBounds(targetLeft, targetTop, targetWidth, targetHeight);
        }

        #endregion

        #region Mover Ventana sin Bordes
        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
    }

    #region Controles Personalizados

    public class TextButton : Button
    {
        public TextButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.FromArgb(45, 45, 77);
            this.ForeColor = Color.FromArgb(173, 216, 230);
            this.Font = new Font("Imperial Code", 10F);
            this.Size = new Size(90, 24);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.ForeColor = Color.White;
            this.BackColor = Color.FromArgb(60, 60, 100);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.ForeColor = Color.FromArgb(173, 216, 230);
            this.BackColor = Color.FromArgb(45, 45, 77);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = this.Height;
                path.AddArc(0, 0, radius, radius, 90, 180);
                path.AddArc(this.Width - radius, 0, radius, radius, 270, 180);
                path.CloseFigure();
                this.Region = new Region(path);
            }
        }
    }

    public class SwitchButton : Control
    {
        private bool _isOn = true;
        public bool IsOn
        {
            get => _isOn;
            set { _isOn = value; Invalidate(); }
        }

        public SwitchButton()
        {
            this.Size = new Size(36, 18); // 36, 18
            this.Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;

            Color bgColor = _isOn ? Color.FromArgb(45, 45, 77) : Color.FromArgb(0, 204, 255);
            Color thumbColor = _isOn ? Color.FromArgb(126, 126, 255) : Color.FromArgb(0, 204, 255);

            using (Pen p = new Pen(ThemeColors.Foreground, 2))
            {
                e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
            }
            if (_isOn)
            {
                using (SolidBrush b = new SolidBrush(ThemeColors.Foreground))
                {
                    e.Graphics.FillRectangle(b, 4, 4, Width - 8, Height - 8);
                }
            }
        }
    }

    public class CyberContainer : Panel
    {
        public CyberContainer()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.Padding = new Padding(15);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            // 1. Dibujar Scanlines (Efecto CRT)
            using (Pen scanlinePen = new Pen(ThemeColors.Scanline, 1))
            {
                for (int y = 0; y < this.Height; y += 3)
                {
                    g.DrawLine(scanlinePen, 0, y, this.Width, y);
                }
            }

            // 2. Bordes contenedores laterales finos
            using (Pen borderPen = new Pen(ThemeColors.Foreground, 1))
            {
                g.DrawRectangle(borderPen, 5, 5, this.Width - 10, this.Height - 10);
            }
        }
    }

    #endregion
}