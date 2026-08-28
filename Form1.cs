using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Terminal
{
    public partial class Form1 : VentanaBase
    {
        private readonly Queue<string> _commandHistory = new Queue<string>();
        private readonly Queue<string> _outputLog = new Queue<string>();
        private const int MaxHistoryLines = 5;
        private readonly List<VentanaBase> _windowInstances = new List<VentanaBase>();

        private Panel InputWrapperPanel;
        private Label LblStatusTop;
        private Label LblEnergy;

        private System.Windows.Forms.Timer _animationTimer;
        private float _dnaRotationAngle = 0f;

        public event EventHandler HotkeyPressed;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Tab))
            {
                // Obtenemos el comando más reciente en la cola
                inputTextBox.Text = _commandHistory.LastOrDefault() + ".";

                // Posicionamos el cursor al final del texto asignado
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void inputTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true; // Informa al sistema que no transfiera el foco
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeTerminalUI();
            StartDnaAnimation();

            this.TargetWidth = 550;
            this.TargetHeight = 700;

            this.LblTitle.Text = "TERMINAL";

            this.is_active = true;
        }

        private void InitializeTerminalUI()
        {
            // Fuente base. Si tienes "Gunship" instalada, cámbiala aquí.
            Font terminalFont = new Font("Gunship", 10F, FontStyle.Bold);

            // 2. Texto de Energía (Derecha)
            LblEnergy = new Label
            {
                Text = "REMAINING ENERGY : 500",
                ForeColor = ThemeColors.Foreground,
                Font = terminalFont,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            MainContainer.Controls.Add(LblEnergy);
            // Posicionamiento dinámico a la derecha
            LblEnergy.Location = new Point(MainContainer.Width - LblEnergy.PreferredWidth - 25, 75);

            // 3. Panel Contenedor del Input (El rectángulo central)
            InputWrapperPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(MainContainer.Width - 40, 90),
                BackColor = Color.Transparent
            };
            InputWrapperPanel.Paint += InputWrapperPanel_Paint;
            MainContainer.Controls.Add(InputWrapperPanel);
        }

        private void StartDnaAnimation()
        {
            _animationTimer = new System.Windows.Forms.Timer { Interval = 30 };
            _animationTimer.Tick += (s, e) =>
            {
                _dnaRotationAngle += 0.08f;
                InputWrapperPanel.Invalidate(); // Redibuja el panel visualizador
            };
            _animationTimer.Start();
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            { 
                if (e.KeyCode == Keys.Tab)
                {
                    e.SuppressKeyPress = true; // Cancela la acción habitual
                }
                return;
            }

            string command = inputTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(command))
            {
                AppendToHistory(command);
                ProcessCommand(command);
            }

            inputTextBox.Clear();
            e.SuppressKeyPress = true; // Previene el 'beep' del sistema al presionar Enter
        }

        private void AppendToHistory(string command)
        {
            _commandHistory.Enqueue(command);

            // Mantiene el tamaño del buffer dentro del límite establecido (FIFO)
            while (_commandHistory.Count > MaxHistoryLines)
            {
                _commandHistory.Dequeue();
            }

            // Renderiza el historial ordenado de más reciente a más antiguo o viceversa
            LastCommand.Text = string.Join(Environment.NewLine, _commandHistory.Reverse());
        }

        private void ProcessCommand(string command)
        {
            string response;

            switch (command.ToLowerInvariant())
            {
                case "clear":
                    _outputLog.Clear();
                    Output.Text = string.Empty;
                    return;

                case "help":
                    response = "Comandos disponibles: help, clear, ping";
                    break;

                case "ping":
                    response = "pong";
                    break;

                case "clock":
                case "time":
                case "hour":
                    // Búsqueda de la instancia activa en la lista mediante LINQ
                    var clockInstance = _windowInstances.OfType<Clock>().FirstOrDefault(w => w.is_active && !w.IsDisposed);

                    if (clockInstance != null)
                    {
                        _ = clockInstance.ASK_Close();
                        _windowInstances.Remove(clockInstance);
                        response = "Cerrando Clock";
                    }
                    else
                    {
                        Clock ins = Clock.Spawn();
                        _windowInstances.Add(ins);
                        response = "Abriendo Clock";
                    }
                    break;

                default:
                    response = $"Comando no reconocido: '{command}'";
                    break;
            }

            AppendToOutput(response);
        }

        private void AppendToOutput(string text)
        {
            _outputLog.Enqueue(text);

            if (_outputLog.Count > 50) // Límite de salida de consola
            {
                _outputLog.Dequeue();
            }

            Output.Text = string.Join(Environment.NewLine, _outputLog.Reverse());
        }

        // Evento que dibuja los bordes y los gráficos (barras + simulación de ADN) en el panel de Input
        private void InputWrapperPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Borde del Contenedor Principal
            using (Pen p = new Pen(ThemeColors.CyanPrimary, 1.5f))
            {
                g.DrawRectangle(p, 0, 0, InputWrapperPanel.Width - 1, InputWrapperPanel.Height - 1);
                g.DrawLine(p, 180, 0, 180, InputWrapperPanel.Height); // Línea vertical separadora
                g.DrawLine(p, 180, 40, InputWrapperPanel.Width, 40);  // Línea horizontal interior
            }

            // A) Barras de Nivel (Izquierda)
            using (SolidBrush barBrush = new SolidBrush(ThemeColors.CyanPrimary))
            {
                g.FillRectangle(barBrush, 15, 10, 18, 70);
                g.FillRectangle(barBrush, 40, 10, 18, 70);
            }

            // B) Renderizado ADN Tridimensional Rotatorio
            DrawAnimatedDNA(g, new Point(115, 65));
        }

        // Renderizado del gráfico central del ADN en código GDI+
        private void DrawAnimatedDNA(Graphics g, Point center)
        {
            int numRungs = 12;
            int height = 80;
            int radius = 25;
            int halfH = height / 2;

            for (int i = 0; i < numRungs; i++)
            {
                float progress = (float)i / numRungs;
                int y = center.Y - halfH + (int)(progress * height);

                // Cálculo de oscilación sinusoidal tridimensional
                double currentAngle = _dnaRotationAngle + (progress * Math.PI * 2);
                int x1 = center.X + (int)(Math.Sin(currentAngle) * radius);
                int x2 = center.X - (int)(Math.Sin(currentAngle) * radius);

                // Profundidad para el grosor/brillo del punto ($Z$-Index)
                double z1 = Math.Cos(currentAngle);
                double z2 = Math.Cos(currentAngle + Math.PI);

                int size1 = z1 > 0 ? 5 : 3;
                int size2 = z2 > 0 ? 5 : 3;

                // Peldaño conector
                using (Pen linePen = new Pen(ThemeColors.CyanDark, 1f))
                {
                    g.DrawLine(linePen, x1, y, x2, y);
                }

                // Nodos fosforescentes
                using (SolidBrush node1 = new SolidBrush(z1 > 0 ? ThemeColors.GreenNode : ThemeColors.CyanPrimary))
                using (SolidBrush node2 = new SolidBrush(z2 > 0 ? ThemeColors.GreenNode : ThemeColors.CyanPrimary))
                {
                    g.FillEllipse(node1, x1 - size1 / 2, y - size1 / 2, size1, size1);
                    g.FillEllipse(node2, x2 - size2 / 2, y - size2 / 2, size2, size2);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _animationTimer?.Stop();
            _animationTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}