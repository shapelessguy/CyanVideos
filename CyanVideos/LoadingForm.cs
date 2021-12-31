using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public partial class LoadingForm : Form
    {
        bool enabled = true;
        Timer timer, timerHide;
        Color color1;
        Color color2;

        public LoadingForm()
        {
            InitializeComponent();
            int minLocationY = 0;
            foreach (Screen screen in Screen.AllScreens) if (screen.Bounds.Location.Y < minLocationY) minLocationY = screen.Bounds.Location.Y;
            Location = new Point(0, minLocationY - 2* Height);
            TopMost = true;

            color1 = circularProgressBar1.OuterColor;
            color2 = circularProgressBar1.ProgressColor;
            timer = new Timer() { Enabled = true, Interval = 6 };
            timer.Tick += TimerTick;
            timerHide = new Timer() { Enabled = true, Interval = 500 };
            
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private void TimerTick(object sender, EventArgs e)
        {
            if (!enabled) return;
            try
            {
                if (Program.GetLoading() && Program.win.WindowState != FormWindowState.Minimized)
                {
                    timerHide.Tick -= HideForm;
                    if (!Visible) circularProgressBar1.Value = 0;
                    Location = new Point(Program.win.Location.X + (Program.win.Width - Width) / 2, Program.win.Location.Y + Program.win.Height - 2* Height);//Program.win.Location.Y + (Program.win.ClientRectangle.Height - Height) / 2);
                    if (!Visible) SetForegroundWindow(this.Handle);
                    Show();
                    BringToFront();
                }
                else timerHide.Tick += HideForm;

                int value = circularProgressBar1.Value;
                value++;
                if (value > 100)
                {
                    value = 0;
                    if (circularProgressBar1.OuterColor == color1)
                    {
                        circularProgressBar1.OuterColor = color2; circularProgressBar1.ProgressColor = color1;
                        circularProgressBar1.OuterMargin = -30;
                        circularProgressBar1.OuterWidth = 31;
                        circularProgressBar1.ProgressWidth = 25;
                    }
                    else
                    {
                        circularProgressBar1.OuterColor = color1; circularProgressBar1.ProgressColor = color2;
                        circularProgressBar1.OuterMargin = -29;
                        circularProgressBar1.OuterWidth = 26;
                        circularProgressBar1.ProgressWidth = 28;
                    }
                    //if (circularProgressBar1.ProgressColor == color2) circularProgressBar1.ProgressColor = color1; else circularProgressBar1.ProgressColor = color2;
                }
                circularProgressBar1.Value = value;
            }
            catch (Exception) { }
        }

        private void HideForm(object sender, EventArgs e)
        {
            Hide();
            timerHide.Tick -= HideForm;
        }
        
    }
}