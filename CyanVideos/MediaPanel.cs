using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class Vision
    {
        public string filename;
        public string header;
        public Vision(string filename, string header)
        {
            this.filename = filename;
            this.header = header;
        }
    }
    public class MediaPanel : Panel
    {
        public static List<Vision> PlayList = new List<Vision>();

        public bool onTop = false;
        public MyToolBar toolbar;
        public TrackBar trackbar;
        static public bool active = false;
        Timer timerHide = new Timer();
        Label Header;

        Point mouseposition;
        bool mousePressed = false;

        public Label time1;
        public Label time2;
        public Panel MediaControls;

        public Player Media;
        private void InitializeComponents()
        {
            Console.WriteLine("Creating a new MediaPanel");
            Media = new Player();
            Controls.Add(Media);

            MediaControls = new Panel() { BackColor = Color.Black, AutoSize = false, Bounds = new Rectangle(-1, -1, 1, 1), };
            Controls.Add(MediaControls);

            time1 = new Label()
            {
                AutoSize = false,
                Size = new Size(100, 20),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Text = "00:00:00",
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Arial", 10, FontStyle.Bold),
            };
            MediaControls.Controls.Add(time1);
            time2 = new Label()
            {
                AutoSize = false,
                Size = new Size(100, 20),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Text = "00:00:00",
                TextAlign = ContentAlignment.TopRight,
                Font = new Font("Arial", 10, FontStyle.Bold),
            };
            MediaControls.Controls.Add(time2);
            Console.WriteLine("new MediaPanel created");
        }

        int hideInterval = 10;
        public MediaPanel()
        {
            InitializeComponents();
            BackColor = Color.Black;
            timerHide = new Timer() { Enabled = true, Interval = hideInterval, };
            timerHide.Tick += HidingBar;
            Header = new Label() { 
                TextAlign = ContentAlignment.BottomCenter, 
                Font = new Font("Algerian", 24, FontStyle.Bold), 
                ForeColor = Color.White, Location = new Point(10000, 100000)};
            Controls.Add(Header);

            trackbar = new TrackBar() { AutoSize = false, Location = new Point(0, 0), Maximum = 1000, TickFrequency = 100, BackColor = Color.Black};
            trackbar.MouseDown += (o, e) => {
                indexHide = 0;
                mousePressed = true;
                UpdateValue(trackbar.PointToClient(MousePosition));
            };
            trackbar.MouseUp += (o, e) => {
                indexHide = 0;
                mousePressed = false;
                UpdateValue(trackbar.PointToClient(MousePosition));
                UpdateVideo();
                Program.win.Keygrip1.Focus();
            };
            MediaControls.Controls.Add(trackbar);
            toolbar = new MyToolBar() { AutoSize = false };
            MediaControls.Controls.Add(toolbar);
            toolbar.Location = new Point(100, 100);

            trackbar.Visible = true;
        }
        

        public int indexHide = 0;
        private int resizeInd = 0;
        int timeToolbarHide = 2000; //milliseconds
        int timeHeaderHide = 3500; //milliseconds
        public void HidingBar(object sender, EventArgs e)
        {
            if (Media.logFlag)
            {
                Media.logFlag = false;
                foreach (var item in Media.logs)
                {
                    for (int i = Window.logs.Count - 1; i >= 0; i--)
                    {
                        if (item.filename == Window.logs[i].filename) Window.logs.RemoveAt(i);
                    }
                    List<string> playlist = new List<string>();
                    foreach (Vision vision in PlayList) { playlist.Add(vision.filename); playlist.Add(vision.header); }
                    Window.logs.Add(new Log(item.filename, item.position, item.data, playlist));
                }
                Media.logs.Clear();
                System.Threading.Thread Update = new System.Threading.Thread(UpdateContinue); Update.Start();
            }
            if (!Media.IsReady()) return;
            resizeInd++;
            if (resizeInd > 10000) resizeInd = 10;

            if (mousePressed) { UpdateTrackBar(0); UpdateValue(trackbar.PointToClient(MousePosition)); indexHide = 0; }
            else UpdateTrackBar(Media.InputPosition());
            indexHide++;
            if (indexHide > 10000) indexHide = timeHeaderHide / hideInterval + 1;
            if (toolbar != null)
            {
                try
                {
                    if (toolbar.DisplayRectangle.Contains(toolbar.PointToClient(MousePosition)) || MyToolBar.pressed) indexHide = 0;
                    if (PointToClient(MousePosition) != mouseposition && Bounds.Contains(PointToClient(MousePosition)) && onTop)
                    {
                        if (resizeInd > 10)
                        {
                            ToolBarShow();
                        }
                        mouseposition = PointToClient(MousePosition); indexHide = 0;
                    }
                }
                catch (Exception) { }
            }
            if (indexHide == timeToolbarHide / hideInterval) ToolBarHide();
            if (indexHide == timeHeaderHide / hideInterval) HeaderHide();
            SetMediaBounds();
        }
        public void ToolBarShow(bool onlyTitle = false, bool null_indexHide = false)
        {
            if (!onlyTitle)
            {
                ToolBarResize();
            }
            Header.Bounds = new Rectangle(0, 0, Width, 50);
            Header.BringToFront();
            if(onlyTitle || null_indexHide) indexHide = 0;
            toolbar.BringToFront();
            resizeInd = 0;
            time1.BringToFront();
            time2.BringToFront();
        }

        Rectangle lastBounds;
        private void ToolBarResize()
        {
            Screen act_screen = ActualScreen();
            int topMargin = 0;
            int dim = Width - 200;
            int trackbarDim = 25;
            Size newSize;
            Point newLocation;
            Rectangle newTrackBounds;
            Rectangle newToolBounds;
            if (!onTop) return;
            if (fullscreen)
            {
                newLocation = new Point(0, act_screen.Bounds.Height - MyToolBar.TrackHeight - topMargin - 2 * trackbarDim + 10);
                newSize = new Size(Width, Height - newLocation.Y);
                newTrackBounds = new Rectangle(0, 0, act_screen.Bounds.Width, trackbarDim);
                int minus = (act_screen.Bounds.Width - dim) / 2;
                newToolBounds = new Rectangle(minus, 2 * trackbarDim - 13, act_screen.Bounds.Width - minus * 2, MyToolBar.TrackHeight);
            }
            else
            {
                newLocation = new Point(0, Height - MyToolBar.TrackHeight - topMargin - 2 * trackbarDim + 10);
                newSize = new Size(Width, Height - newLocation.Y);
                newTrackBounds = new Rectangle(50, 0, Width - 100, trackbarDim);
                int minus = (Width - 100 - dim) / 2;
                newToolBounds = new Rectangle(50 + minus, 2 * trackbarDim - 13, Width - 100 - minus * 2, MyToolBar.TrackHeight);
            }
            Rectangle newBoundsMedia = new Rectangle(newLocation, newSize);
            if (lastBounds != null && lastBounds == newBoundsMedia) return;

            SuspendLayout();
            MediaControls.Location = newLocation;
            MediaControls.Size = newSize;
            trackbar.Bounds = newTrackBounds;
            toolbar.Bounds = newToolBounds;
            Program.win.mediaPanel.time1.Location = new Point(trackbar.Location.X + 15, trackbar.Height - 5);
            Program.win.mediaPanel.time2.Location = new Point(trackbar.Location.X + trackbar.Width - Program.win.mediaPanel.time2.Width - 20, trackbar.Height - 5);
            lastBounds = MediaControls.Bounds;

            toolbar.ResizeButtons();
            MediaControls.Update();
            MediaControls.BringToFront();
            trackbar.Refresh();
            trackbar.BringToFront();
            ResumeLayout(true);
        }
        public void ToolBarHide()
        {
            MediaControls.Location = new Point(10000, 10000);
            lastBounds = new Rectangle(1, 1, 10000, 10000);
        }
        public void HeaderHide()
        {
            Header.Bounds = new Rectangle(10000, 10000, 1, 1);
        }
        public void UpdateValue(Point p)
        {
            int firstPoint = 12; int lastPoint = trackbar.Width - firstPoint - firstPoint/3;
            double value = ((double)(p.X - firstPoint) / (double)(lastPoint - firstPoint));
            UpdateTrackBar(value);
        }
        public void UpdateTrackBar(double value)
        {
            if (value < 0) value = 0;
            if (value > 0.999) value = 0.999f;
            trackbar.Value = (int)(value * 1000);
        }
        public bool IsBuffering()
        {
            return Media.IsBuffering();
        }
        public void UpdateVideo()
        {
            Media.SetInputPosition((double)(trackbar.Value)/(double)(1000));
            UpdateTime();
        }
        public void UpdateTime()
        {
            toolbar.UpdateTime(Media.InputTime(), Media.InputLength());
        }

        public bool fullscreen;
        Rectangle bounds;
        public void FullScreen()
        {
            if (!fullscreen)
            {
                fullscreen = true;
                Program.win.WindowState = FormWindowState.Normal;
                bounds = Program.win.Bounds;
                Program.win.TopMost = true;
                Program.win.FormBorderStyle = FormBorderStyle.None;
                Screen act_screen = ActualScreen();
                if (act_screen != null) { Program.win.Bounds = act_screen.Bounds; }
            }
            else
            {
                fullscreen = false;
                Program.win.Bounds = bounds;
                ToolBarShow();
                Program.win.TopMost = false;
                Program.win.FormBorderStyle = FormBorderStyle.Sizable;
                Program.win.WindowState = FormWindowState.Maximized;
            }
            ResizeForm();
            MediaControls.BringToFront();
        }

        private Screen ActualScreen()
        {
            Point center = new Point(Program.win.Bounds.Location.X + Program.win.Width / 2, Program.win.Bounds.Location.Y + Program.win.Height / 2);
            foreach (Screen screen in Screen.AllScreens) if (screen.Bounds.Contains(center)) return screen;
            return null;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public void ToFront()
        {
            RegisterHotKey(Program.win.Handle, 100, 0, Keys.MediaPlayPause.GetHashCode());
            RegisterHotKey(Program.win.Handle, 101, 0, Keys.MediaNextTrack.GetHashCode());
            RegisterHotKey(Program.win.Handle, 102, 0, Keys.MediaPreviousTrack.GetHashCode());
            onTop = true;
            BringToFront();
        }

        public void ResizeForm()
        {
            try
            {
                Screen act_screen = ActualScreen();
                
                if (fullscreen)
                {
                    Size = act_screen.Bounds.Size;
                }
                else
                {
                    Size = new Size(Program.win.Size.Width - 15, Program.win.Size.Height-35);
                }
                ToolBarResize();
                trackbar.BringToFront();
                time1.BringToFront(); time2.BringToFront();
            }
            catch (Exception) { Console.WriteLine("Exception raised by ResizeForm from MediaPanel"); }
        }
        private void SetMediaBounds()
        {
            if (fullscreen)
            {
                Screen act_screen = ActualScreen();
                Media.SetBounds(new Rectangle(0, 0, act_screen.Bounds.Width, act_screen.Bounds.Height));
            }
            else
            {
                Media.SetBounds(new Rectangle(50, 60, Width - 100, Height - 130));
            }
        }

        public void Play()
        {
            toolbar.play.BackgroundImage = Properties.Resources.pause;
            toolbar.Update();
            toolbar.Volume.UpdateVol(true);
            Program.win.mediaPanel.Media.Play();
            ToolBarShow();
        }
        
        public void Pause()
        {
            toolbar.play.BackgroundImage = Properties.Resources.play;
            toolbar.Update();
            MyVolume.MuteApplication();
            Program.win.mediaPanel.Media.Pause();
            ToolBarShow();
        }

        public bool GetFlag(bool restore = false)
        {
            if (Media == null) return false;
            if (restore) return Media.GetFlag(true);
            else return Media.GetFlag();
        }

        public void UpdateContinue()
        {
            Log.ClearLogs(Window.logs);
            //foreach (var log in Window.logs) log.Print();
            foreach (Source source in Window.Sources) foreach (Iconxx icon in source.Icons()) icon.UpdateContinue();
            foreach (Iconxx icon in Program.win.Research_Categories) icon.UpdateContinue();
            foreach (Iconxx icon in Program.win.DeepSource.Icons()) icon.UpdateContinue();
        }

        public void Quit()
        {
            UnregisterHotKey(Program.win.Handle, 100);
            UnregisterHotKey(Program.win.Handle, 101);
            UnregisterHotKey(Program.win.Handle, 102);
            onTop = false;
            UpdateTrackBar(0);
            time1.Text = "00:00:00";
            time2.Text = "00:00:00";
            Media.Stop();
        }
        public void ToBack()
        {
            SendToBack();
            ToolBarHide();
            HeaderHide();
        }
        
        public int numFilm = 0;
        string actPath = "";
        public void LoadFilm(int num, bool force = false, float position = 0)
        {
            if (!Program.VLC_Installed)
            {
                MessageBox.Show("No libvlc found in the installation folder. Please add it to that folder to use the internal player!");
                return;
            }
            try
            {
                if (force) numFilm = num;
                Header.Text = MediaPanel.PlayList[num].header;
                toolbar.play.BackgroundImage = Properties.Resources.pause;
                Timer after = new Timer() { Enabled = true, Interval = 500 }; after.Tick += (o, e) =>
                {
                    //ToolBarShow();
                    trackbar.Value = 0;

                    Media.SetVisible(true);
                    toolbar.Volume.UpdateVol(true);
                    ToolBarShow();
                    after.Dispose();
                };
                actPath = MediaPanel.PlayList[num].filename;
                Media.LoadFilm(actPath, position);
                ResizeForm();
            }
            catch (Exception e) { Console.WriteLine("Exception from LoadFilm: "+e.Message); }
        }


    }
}
