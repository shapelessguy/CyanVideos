using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class MyToolBar : Panel
    {
        public static int TrackHeight = 70;
        public static bool pressed = false;


        public Label Previous;


        Label audioSwitch;
        Label subSwitch;
        Label previous;
        public Label play;
        Label next;
        public MyVolume Volume;
        private Bitmap Background;

        Label fullScreen;


        private MenuStrip menuAudio;
        public ToolStripMenuItem toolMenuAudio;

        private MenuStrip menuSub;
        public ToolStripMenuItem toolMenuSub;

        public MyToolBar()
        {
            Location = new Point(10000, 10000);
            InitializeLabels();

            ClientSizeChanged += CreateBackground;
            toolMenuAudio.DropDownOpened += (o, e) => { pressed = true; };
            toolMenuSub.DropDownOpened += (o, e) => { pressed = true; };
            toolMenuAudio.DropDownClosed += (o, e) => { pressed = false; };
            toolMenuSub.DropDownClosed += (o, e) => { pressed = false; };

            //BackColor = Color.Red;
            BackgroundImageLayout = ImageLayout.Center;

            Timer timer = new Timer() { Enabled = true, Interval = 100 };
            timer.Tick += (o, e) => { if (Program.win.mediaPanel.GetFlag()) { Program.win.mediaPanel.GetFlag(true); ChangePlayButton(Program.win.mediaPanel.Media.IsPlaying()); } };
        }

        private void CreateBackground(object sender, EventArgs e)
        {
            if (Background!= null && Background.Size == Size) return;
            Background = new Bitmap(Properties.Resources.toolbar, new Size((int)(Width*0.8), (int)(Height*0.9)));
            BackgroundImage = Background;
        }

        Size standarsSize1;
        Size standarsSize2;
        private bool playState = false; // pause
        private bool HState = false; // not highlighted
        private void InitializeLabels()
        {

            standarsSize1 = new Size(TrackHeight - 28, TrackHeight - 32);
            standarsSize2 = new Size(TrackHeight - 22, TrackHeight - 22);
            
            Previous = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize1, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.prev};
            Previous.MouseEnter += (o, e) => { Previous.BackgroundImage = Properties.Resources.prevH; };
            Previous.MouseLeave += (o, e) => { Previous.BackgroundImage = Properties.Resources.prev; };
            Controls.Add(Previous);
            Previous.Click += (o, e) => { Program.win.QuitMediaPanel(); };
            
            audioSwitch = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize1 };
            menuAudio = new MenuStrip() { AutoSize = false, Size = standarsSize1, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.audio, };
            menuAudio.MouseEnter += (o, e) => { if (toolMenuAudio.DropDownItems.Count == 0) return; menuAudio.BackgroundImage = Properties.Resources.audioH; };
            menuAudio.MouseLeave += (o, e) => { menuAudio.BackgroundImage = Properties.Resources.audio; };
            menuAudio.DefaultDropDownDirection = ToolStripDropDownDirection.AboveRight;
            Controls.Add(audioSwitch);
            audioSwitch.Controls.Add(menuAudio);
            toolMenuAudio = new ToolStripMenuItem() { Text = "  " };
            menuAudio.Items.Add(toolMenuAudio);
            menuAudio.Renderer = new MyRenderer();

            subSwitch = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize1 };
            menuSub = new MenuStrip() { Dock = DockStyle.Fill, AutoSize = false, BackColor = Color.Transparent, Size = standarsSize1, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.sub, };
            menuSub.MouseEnter += (o, e) => { if (toolMenuSub.DropDownItems.Count == 0) return; menuSub.BackgroundImage = Properties.Resources.subH; };
            menuSub.MouseLeave += (o, e) => { menuSub.BackgroundImage = Properties.Resources.sub; };
            menuSub.DefaultDropDownDirection = ToolStripDropDownDirection.AboveRight;
            Controls.Add(subSwitch);
            subSwitch.Controls.Add(menuSub);
            toolMenuSub = new ToolStripMenuItem() {Dock = DockStyle.Fill, Text = "  " };
            menuSub.Items.Add(toolMenuSub);
            menuSub.Renderer = new MyRenderer();

            previous = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize2, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.left, };
            previous.Click += PreviousClick;
            previous.MouseEnter += (o, e) => { if (Program.win.mediaPanel.numFilm == 0) return; previous.BackgroundImage = Properties.Resources.leftH; };
            previous.MouseLeave += (o, e) => { previous.BackgroundImage = Properties.Resources.left; };
            Controls.Add(previous);

            play = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize2, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.pause, };
            play.Click += (o, e) => {
                if (!Program.win.mediaPanel.Media.IsPlaying()) { Program.win.mediaPanel.Play(); play.BackgroundImage = Properties.Resources.pauseH; playState = false; }
                else { Program.win.mediaPanel.Pause(); play.BackgroundImage = Properties.Resources.playH; playState = true; }
                Program.win.mediaPanel.indexHide = 0;
            };
            play.MouseEnter += (o, e) =>
            {
                if (playState) { play.BackgroundImage = Properties.Resources.playH; HState = true; }
                else {play.BackgroundImage = Properties.Resources.pauseH; HState = true; }
            };
            play.MouseLeave += (o, e) => {
                if (playState) { play.BackgroundImage = Properties.Resources.play; HState = false; }
                else { play.BackgroundImage = Properties.Resources.pause; HState = false; }
            };
            Controls.Add(play);

            next = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize2, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.right, };
            next.Click += NextClick;
            next.MouseEnter += (o, e) => { if (Program.win.mediaPanel.numFilm == MediaPanel.PlayList.Count - 1) return; next.BackgroundImage = Properties.Resources.rightH; };
            next.MouseLeave += (o, e) => { next.BackgroundImage = Properties.Resources.right; };
            Controls.Add(next);

            Volume = new MyVolume() { AutoSize = false, BackColor = Color.Transparent, Size = new Size(TrackHeight*2, standarsSize1.Height - 5), BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.vol, };
            Controls.Add(Volume);

            fullScreen = new Label() { AutoSize = false, BackColor = Color.Transparent, Size = standarsSize1, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Properties.Resources.zoom, };
            fullScreen.Click += (o, e) => { Program.win.mediaPanel.FullScreen(); };
            fullScreen.MouseEnter += (o, e) => { fullScreen.BackgroundImage = Properties.Resources.zoomH; };
            fullScreen.MouseLeave += (o, e) => { fullScreen.BackgroundImage = Properties.Resources.zoom; };
            Controls.Add(fullScreen);
            
            ResizeButtons();
        }

        public void ChangePlayButton(bool playing)
        {
            playState = !playing;
            if (playing)
            {
                if (HState) play.BackgroundImage = Properties.Resources.pauseH;
                else play.BackgroundImage = Properties.Resources.pause;
            }
            else
            {
                if (HState) play.BackgroundImage = Properties.Resources.playH;
                else play.BackgroundImage = Properties.Resources.play;
            }
        }

        public void ResizeButtons()
        {
            int bias = 10;
            //play.Location = new Point(0, (TrackHeight - standarsSize2.Height) / 2);
            play.Location = new Point((Width - TrackHeight) / 2 + bias, (TrackHeight - standarsSize2.Height) / 2);

            Previous.Location = new Point(play.Location.X - (int)(5.5 * TrackHeight), (TrackHeight - standarsSize1.Height) / 2);
            audioSwitch.Location = new Point(play.Location.X - (int)(3.5*TrackHeight) + 15, (TrackHeight - standarsSize1.Height) / 2);
            subSwitch.Location = new Point(play.Location.X - (int)(2.5*TrackHeight), (TrackHeight - standarsSize1.Height) / 2 + 2);
            previous.Location = new Point(play.Location.X - TrackHeight, (TrackHeight - standarsSize2.Height) / 2);
            next.Location = new Point(play.Location.X + TrackHeight, (TrackHeight - standarsSize2.Height) / 2);
            Volume.Location = new Point(play.Location.X + (int)(2.5*TrackHeight), (TrackHeight - Volume.Height) / 2);
            //Volume.UpdateVol(true);

            fullScreen.Location = new Point(play.Location.X + 5*TrackHeight + 20 + bias, (TrackHeight - standarsSize1.Height) / 2);
        }

        public void NextClick(object sender, EventArgs e)
        {
            if (!Program.win.mediaPanel.Media.IsReady()) { return; }
            if (Program.win.mediaPanel.numFilm == MediaPanel.PlayList.Count - 1) { return; }
            else
            {
                if (Program.win.mediaPanel.numFilm == MediaPanel.PlayList.Count - 2) next.BackgroundImage = Properties.Resources.right;
                Program.win.mediaPanel.LoadFilm(Program.win.mediaPanel.numFilm + 1);
                Program.win.mediaPanel.numFilm++;
                Program.win.mediaPanel.indexHide = 0;
            }
        }
        public void PreviousClick(object sender, EventArgs e)
        {
            if (!Program.win.mediaPanel.Media.IsReady()) return;
            if (Program.win.mediaPanel.numFilm == 0) { return; }
            else
            {
                if (Program.win.mediaPanel.numFilm == 1) previous.BackgroundImage = Properties.Resources.left;
                if (Program.win.mediaPanel.Media.IsBuffering()) return;
                Program.win.mediaPanel.LoadFilm(Program.win.mediaPanel.numFilm - 1);
                Program.win.mediaPanel.numFilm--;
                Program.win.mediaPanel.indexHide = 0;
            }
        }

        public void UpdateTime(double time1, double time2)
        {
            try
            {
                Program.win.mediaPanel.time1.Text = GetTime(time1);
                Program.win.mediaPanel.time2.Text = GetTime(time2);
            }
            catch (Exception) { }
        }
        private string GetTime(double time)
        {
            int tot_sec = (int)(time / 1000);
            string stringa1 = ((int)(tot_sec / 60)).ToString();
            if (stringa1.Length == 1) stringa1 = "0" + stringa1;
            string stringa2 = (tot_sec - (int)(tot_sec / 60) * 60).ToString();
            if (stringa2.Length == 1) stringa2 = "0" + stringa2;
            return stringa1 + ":" + stringa2;
        }
        public void RefreshForm(bool rest = false)
        {
            if (!rest)
            {
                RefreshMenuAudio();
                RefreshMenuSub();
            }
            else RefreshMenuSub(true);
        }

        public void RefreshMenuAudio()
        {
            toolMenuAudio.DropDownItems.Clear();
            ToolStripMenuItem[] result = Program.win.mediaPanel.Media.GetAudioToolMenu();
            if(result != null) toolMenuAudio.DropDownItems.AddRange(result);
        }

        int prev_results = 0;
        public void RefreshMenuSub(bool rest = false)
        {
            if (!rest)
            {
                toolMenuSub.DropDownItems.Clear();
                ToolStripMenuItem[] result = Program.win.mediaPanel.Media.GetSubToolMenu();
                if (result != null) { toolMenuSub.DropDownItems.AddRange(result); prev_results = result.Count(); }
            }
            else
            {
                if(toolMenuSub.DropDownItems.Count > prev_results)
                {
                    for(int i= toolMenuSub.DropDownItems.Count-1; i>= prev_results; i--)
                    {
                        toolMenuSub.DropDownItems.RemoveAt(i);
                    }
                }
                ToolStripMenuItem[] result = Program.win.mediaPanel.Media.GetSubRestMenu();
                if (result != null) toolMenuSub.DropDownItems.AddRange(result);
            }
        }
    }
}