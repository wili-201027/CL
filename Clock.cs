using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Terminal
{
    internal class Clock : VentanaBase
    {
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Label _lblTime;
        public Clock() : base(300, 150)
        {
            this.Text = "RELOJ";

            _lblTime = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Consolas", 24F, FontStyle.Bold)
            };

            MainContainer.Controls.Add(_lblTime);

            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += (s, e) => _lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
            _timer.Start();

            _lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        public static Clock Spawn()
        {
            Clock instance = new Clock();
            instance.Show();
            return instance;
        }

    }
}
