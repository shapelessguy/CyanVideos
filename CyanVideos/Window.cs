using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public partial class Window : Form
    {
        static public Size standard;
        static public Size standard2;
        static public int intraDistanceX = 15;
        static public int intraDistanceY = 15;
        static public int intraDistanceX2 = 30;
        static public int intraDistanceY2 = 15;
        static public float prop_width_height = 0.70F;
        public MyPanel firstpanel = new MyPanel(1);
        public MyPanel secondpanel = new MyPanel(2);
        public MediaPanel mediaPanel;
        public static List<Log> logs = new List<Log>();
        public static List<Source> Sources = new List<Source>();
        public static Dictionary<string, string> FaultSources = new Dictionary<string, string>();
        public List<Iconxx> Research_Categories = new List<Iconxx>();
        public Source DeepSource;

        public TextBox hintText = new TextBox();
        public static string null_value = "..Search film..";
        public static string[] null_values;
        public Label OpenRicerca;
        public PanelResearch ricerca;

        System.Windows.Forms.Timer time;
        System.Windows.Forms.Timer timerDoubleClick;

        public bool suspendedResize = false;


        private void Window_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Window loading");
            timerDoubleClick = new System.Windows.Forms.Timer() { Enabled = true, Interval = 400 };
            standard = new Size((int)(Properties.Settings.Default.grandezza_icon1 * prop_width_height), Properties.Settings.Default.grandezza_icon1);
            standard2 = new Size((int)(Properties.Settings.Default.grandezza_icon2 * prop_width_height), Properties.Settings.Default.grandezza_icon2);

            //ResearchClass.Initialize();
            timerInitializer = new System.Windows.Forms.Timer() { Enabled = true, Interval = 100 };
            timerInitializer.Tick += UpdateSources;
            //timerDoubleClick = new System.Windows.Forms.Timer() { Enabled = true, Interval = 1000 };
            secondpanel.Size = new Size(1, 1);
            secondpanel.Location = new Point(1, 1);
            //ResizeWin(true, false);
            Keygrip1.Focus();
            //Properties.Settings.Default.Logs = "";
            if (Iconxx.LoadSegments == null && System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")=="AMD64")
                Iconxx.LoadSegments = new System.Threading.Timer(Iconxx.TakeSnap, null, 200, System.Threading.Timeout.Infinite);
            if (Properties.Settings.Default.Logs.Length > 2)
            {
                string[] logs_str = Properties.Settings.Default.Logs.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < logs_str.Length; i++)
                {
                    Log log = Log.Load(logs_str[i]);
                    if(log!= null) logs.Add(log);
                }
            }

            Program.enabledToSave = true;
            Console.WriteLine("Window loaded");
        }
        System.Windows.Forms.Timer timerInitializer;
        bool initialized = false;
        private void UpdateSources(object sender, EventArgs e)
        {
            Console.WriteLine("Timer update");
            timerInitializer.Dispose();
            UpdateSources();
            Program.EnableLoading(false);
            initialized = true;
            //firstpanel.EnableControls();
        }
        public Source GetDeepSource()
        {
            return DeepSource;
        }
        public void DisposeDeepSource()
        {
            if (DeepSource != null && !DeepSource.Null) { DeepSource.Dispose(true); Console.WriteLine("here1"); }
            DeepSource = new Source("","");
        }
        static bool res_touching = false;
        public Window()
        {
            DoubleBuffered = true;
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            Menu.Visible = false;
            Menu.Renderer = new MyRenderer();
            ClientSize = new Size(Program.defaultScreen.Bounds.Size.Width / 2, Program.defaultScreen.Bounds.Size.Height / 2);
            Location = Program.defaultScreen.Bounds.Location; new Point(Program.defaultScreen.Bounds.Location.X + Program.defaultScreen.Bounds.Size.Width / 4, Program.defaultScreen.Bounds.Location.Y + Program.defaultScreen.Bounds.Size.Height / 4);
            Controls.Add(firstpanel);
            Controls.Add(secondpanel);
            BackColor = Color.Black;
            ResizeBegin += (o, e) => { firstpanel.ToFront();};
            Resize += (o, e) => { Menu.Location = new Point(hintText.Location.X + hintText.Width + 20, Menu.Location.Y); };
            ResizeEnd += ResizeWin;
            KeyDown += Down;
            Click += ClickNull;
            ResizeBegin += (o, e) => { res_touching = true; };
            ResizeEnd += (o, e) => { res_touching = false; };
            FormClosing += (o, e) => { Program.Save(); Program.loadingForm.Close(); };
            soloFilmToolStripMenuItem.Checked = Properties.Settings.Default.valore_mostra == 1;
            soloSerieTVToolStripMenuItem.Checked = Properties.Settings.Default.valore_mostra == 2;
            tuttoToolStripMenuItem.Checked = Properties.Settings.Default.valore_mostra == 3;
            visualizzaControversieToolStripMenuItem.Checked = Properties.Settings.Default.exclamationsEnabled;
            compact.Checked = Properties.Settings.Default.compact;
            if(Program.VLC_Installed) internalPlayer.Checked = Properties.Settings.Default.internalPlayer;
            else internalPlayer.Checked = false;

            secondpanel.Location = new Point();
            mediaPanel = new MediaPanel();
            Controls.Add(mediaPanel);
            mediaPanel.Location = new Point(0, 0);
            mediaPanel.KeyDown += Down;
            mediaPanel.MouseClick += ClickNull;
            firstpanel.Location = new Point(0, 0);
            firstpanel.KeyDown += Down;
            secondpanel.KeyDown += Down;
            Keygrip1.KeyDown += Down;
            Keygrip2.KeyDown += Down;

            hintText = new TextBox
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                TabStop = true,
                TabIndex = 1,
                Text = null_value,
            };
            Controls.Add(hintText);
            time = new System.Windows.Forms.Timer() { Enabled = true, Interval = 80 };
            time.Tick += Time;
            hintText.TextChanged += (o, e) => { time_slice = 0; };
            hintText.KeyDown += (o, e) => { if (e.KeyCode == Keys.Enter) { if (time_slice < 10) time_slice = 9; e.SuppressKeyPress = true; };  };
            hintText.Click += Selection;
            hintText.GotFocus += Selection;
            hintText.LostFocus += ToNullValue;
            hintText.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left);

            ricerca = new PanelResearch();
            Controls.Add(ricerca);
            OpenRicerca = new Label()
            {
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = Properties.Resources.find,
                Location = new Point(-1000, -1000),
                Size = new Size(40, 40),
            };
            Controls.Add(OpenRicerca);
            OpenRicerca.Click += OpenRicerca_Click;
            OpenRicerca.MouseEnter += (o, e) => {
                OpenRicerca.Location = new Point(OpenRicerca.Location.X - 1, OpenRicerca.Location.Y - 1);
                OpenRicerca.Size = new Size(OpenRicerca.Width + 2, OpenRicerca.Height + 2);
            };
            OpenRicerca.MouseLeave += (o, e) => {
                OpenRicerca.Location = new Point(OpenRicerca.Location.X + 1, OpenRicerca.Location.Y + 1);
                OpenRicerca.Size = new Size(OpenRicerca.Width - 2, OpenRicerca.Height - 2);
            };
            firstpanel.ToFront();
            mediaPanel.SendToBack();
        }
        private void Selection(object sender, EventArgs e)
        {
            if (firstpanel.myposition != MyPanel.maxposition) firstpanel.ToFront();
            if (hintText.Text == null_value) hintText.Text = "";
        }
        public int time_slice = 0;
        public int delay = 0;
        public bool refreshImages = false;
        private void Time(object sender, EventArgs e)
        {
            time_slice++;
            if (time_slice > 100) time_slice = 100;
            if (time_slice == 10) Supervisor.ResearchAsync();
            delay++;
            if(lastWidth != Width || lastHeight != Height) if(!res_touching && WindowState != FormWindowState.Minimized) ResizeWin(false, false, true);
            if (delay == 1)
            {
                if (refreshImages)
                {
                    refreshImages = false;
                    Program.win.RefreshFirstPanelImages();
                    Program.win.RefreshSecondPanelImages();
                }
                delay = 0;
            }
        }

        private void ToNullValue(object sender, EventArgs e)
        {
            if (hintText.Text == "") hintText.Text = null_value;
        }
        public void ResizeWin(object sender, EventArgs e)
        {
            ResizeWin(true, true, true);
        }

        public static int lastWidth = -1;
        public static int lastHeight = -1;
        public void ResizeWin(bool first = false, bool second = true, bool onlyLocation = false, bool panel2_sameof1 = true)
        {
            if (!initialized) return;
            //if (Width == lastWidth && Height == lastHeight) return;
            lastWidth = Width;
            lastHeight = Height;
            Console.WriteLine("Resizing!");
            if (!mediaPanel.onTop)
            {
                firstpanel.ToFront();
                mediaPanel.Visible = false;
                firstpanel.ResizeForm();
            }
            hintText.Size = new Size(Width / 2, 50);
            hintText.Location = new Point(Width / 4 - firstpanel.leftmargin/2 - 20, 10);
            Menu.Location = new Point(hintText.Width + hintText.Location.X + 20, Menu.Location.Y);
            Menu.Visible = true;
            OpenRicerca.Location = new Point(hintText.Location.X - 2 * OpenRicerca.Width, 10);
            System.Windows.Forms.Timer timerResize = new System.Windows.Forms.Timer() { Enabled = true, Interval = 100 };
            timerResize.Tick += (o, e) => { timerResize.Dispose(); mediaPanel.ResizeForm(); };
            
            if (ricerca != null)
            {
                ricerca.Size = new Size(Width - 20, 212);
                ricerca.Location = new Point(0, hintText.Height + hintText.Location.Y + 20);
                ricerca.ResizePanel();
            }

            bool onlyLocation2 = onlyLocation;
            if (!panel2_sameof1) onlyLocation2 = !onlyLocation2;

            if (mediaPanel.onTop) suspendedResize = true;
            else if (suspendedResize) { ResizePanels(true, true, true); Console.WriteLine("Suspended Resize!"); suspendedResize = false; }
            else ResizePanels(first, second, onlyLocation, onlyLocation2);
        }
        public void OpenRicerca_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            ClickNull(sender, e);
            ricerca.rolebox.ClearSelected();
            if (!ricerca.Visible)
            {
                ricerca.Visible = true;
                //ricerca.BringToFront();
                //OpenRicerca.BringToFront();
            }
            else
            {
                ricerca.Visible = false;
            }
            ResumeLayout(false);
            ResizePanels(false, false);
            lastWidth = Width;
            lastHeight = Height;
        }

        private void ResizePanels(bool first=true, bool second= true, bool onlyLocation = false, bool onlyLocation2 = false)
        {
            if (ricerca.Visible)
            {
                MyPanel.firstPanelLocation = new Point(0, 60 + ricerca.Height + 5);
                MyPanel.firstPanelSize = new Size(Width, Height - 60 - ricerca.Location.Y - ricerca.Height + 10 + 5);
            }
            else
            {
                MyPanel.firstPanelLocation = new Point(0, 60);
                MyPanel.firstPanelSize = new Size(Width, Height - 60 - hintText.Location.Y - hintText.Height +1);
            }

            ResizePanel2();
            //firstpanel.Location = new Point(0,0);
            RefreshFirstPanel(first, onlyLocation);
            RefreshSecondPanel(second, onlyLocation2);
        }
        public void RefreshFirstPanel(bool paint, bool onlyLocation)
        {
            if (paint) { Program.EnableLoading(); }//firstpanel.ClearPanel(); }//firstpanel.Refresh(); }
            System.Windows.Forms.Timer ResizeTimer = new System.Windows.Forms.Timer() { Enabled = true, Interval = 100 };
            ResizeTimer.Tick += ResizeWinThread;
            void ResizeWinThread(object sender, EventArgs e)
            {
                try
                {
                    ResizeTimer.Dispose();
                    firstpanel.Refresh(paint, onlyLocation);
                    mediaPanel.Visible = true;
                    Program.EnableLoading(false);
                }
                catch (Exception) { Program.EnableLoading(false); }
            }
        }
        public void RefreshFirstPanelImages()
        {
            firstpanel.RefreshImages();
        }
        public void RefreshSecondPanel(bool paint, bool onlyLocation)
        {
            if (paint) { Program.EnableLoading(true); secondpanel.ClearPanel(); }//secondpanel.Refresh(); }
            System.Windows.Forms.Timer ResizeTimer = new System.Windows.Forms.Timer() { Enabled = true, Interval = 100 };
            ResizeTimer.Tick += ResizeWinThread;
            void ResizeWinThread(object sender, EventArgs e)
            {
                try
                {
                    ResizeTimer.Dispose();
                    secondpanel.Refresh(paint, onlyLocation);
                    //mediaPanel.Visible = true;
                    Program.EnableLoading(false);
                }
                catch (Exception) { Program.EnableLoading(false); }
            }
        }
        public void RefreshSecondPanelImages()
        {
            secondpanel.RefreshImages();
        }
        public void ResizePanel2()
        {
            try
            {
                if (secondpanel == null) return;
                if (secondpanel.myposition != MyPanel.maxposition) return;
                if (DeepSource == null) return;
                secondpanel.Bounds = new Rectangle(1, 1, 1, 1);
                int panelWidth = 0;

                int num_icons = 0;
                foreach (Iconxx icon in DeepSource.Icons()) if (icon.folder) num_icons += 2; else num_icons++;
                int num_icon = num_icons, num_iconx = 0, num_icony = 1;
                if (num_icon < 5) num_iconx = num_icon; else num_iconx = 4;
                if (num_icon >= 5) num_icony = 2;

                if (num_icon <= 8)
                {
                    int iconHeight = 0;
                    if (num_icon == 1) iconHeight = (int)(Height * 0.6);
                    else if (num_icon == 2) iconHeight = (int)(Height * 0.6);
                    else if (num_icon == 3) iconHeight = (int)(Height * 0.5);
                    else if (num_icon == 4) iconHeight = (int)(Height * 0.46);
                    else iconHeight = (int)(Height * 0.38);
                    int iconWidth = (int)(iconHeight * prop_width_height);
                    int intraDistanceX = Window.intraDistanceX2;
                    int intraDistanceY = Window.intraDistanceY2;
                    panelWidth = num_iconx*(iconWidth+intraDistanceX) + intraDistanceX;
                    int panelHeight = num_icony*(iconHeight + intraDistanceY) + SourceTag.height + intraDistanceY;
                    MyPanel.secPanelSize = new Size(panelWidth, panelHeight);
                }
                else
                {
                    panelWidth = (int)(Width * 0.8);
                    MyPanel.secPanelSize = new Size(panelWidth, (int)(Height*0.8));
                }
                MyPanel.secPanelLocation = new Point((int)(MyPanel.firstPanelSize.Width - MyPanel.secPanelSize.Width) / 2 + MyPanel.firstPanelLocation.X -30, (int)(Height - MyPanel.secPanelSize.Height ) / 2);
            }
            catch (Exception) { Console.WriteLine("Exception from ResizePanel2 in Window"); }
        }
        public void ClickNull(object sender, EventArgs e)
        {
            if (firstpanel.myposition != MyPanel.maxposition && !mediaPanel.onTop) firstpanel.ToFront();
        }
        public void FocusGrip()
        {
            Keygrip1.Focus();
        }
        void AddSource(object sender, EventArgs e)
        {
            AddSource();
        }
        void AddSourceSeries(object sender, EventArgs e)
        {
            AddSource(true);
        }
        void AddSource(bool series = false)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string dir_path = folderBrowserDialog1.SelectedPath;
                    Program.EnableLoading(true);
                    AddNewSource(dir_path, dir_path, series);
                    Program.EnableLoading(true);
                    Program.Save();
                    UpdateSources();
                    Program.EnableLoading(false);
                }
                catch (Exception) { Program.EnableLoading(false); MessageBox.Show("Non è possibile selezionare questa cartella a causa di mancata autorizzazione"); }
            }
        }

        int passo = 50;
        int passo2 = 30;
        public void Down(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Subtract)
            {
                firstpanel.bias = 0;
                secondpanel.bias = 0;
                if (firstpanel.myposition == MyPanel.maxposition)
                {
                    if (standard.Width < 130) return;
                    else
                    {
                        standard.Width -= (int)(passo * prop_width_height); standard.Height -= passo;
                        Properties.Settings.Default.grandezza_icon1 -= passo;
                        MyPanel.allowDraw = false;
                        firstpanel.Refresh(true);
                        MyPanel.allowDraw = true;
                    }
                }
                else if (secondpanel.myposition == MyPanel.maxposition)
                {
                    int num_icon = DeepSource.Icons().Count;
                    foreach (Iconxx icon in DeepSource.Icons()) if (icon.folder) num_icon++;
                    if (num_icon <= 8) return;
                    if (Properties.Settings.Default.grandezza_icon2 < 130) return;
                    else
                    {
                        standard2.Width -= (int)(passo2 * prop_width_height); standard2.Height -= passo2;
                        Properties.Settings.Default.grandezza_icon2 -= passo2;
                        MyPanel.allowDraw = false;
                        secondpanel.Refresh(true);
                        MyPanel.allowDraw = true;
                    }
                }
            }
            if (e.KeyCode == Keys.Add)
            {
                firstpanel.bias = 0;
                secondpanel.bias = 0;
                if (firstpanel.myposition == MyPanel.maxposition)
                {
                    if (standard.Width > 700) return;
                    else
                    {
                        standard.Width += (int)(passo * prop_width_height); standard.Height += passo;
                        Properties.Settings.Default.grandezza_icon1 += passo;
                        MyPanel.allowDraw = false;
                        firstpanel.Refresh(true);
                        MyPanel.allowDraw = true;
                    }
                }
                else if (secondpanel.myposition == MyPanel.maxposition)
                {
                    int num_icon = DeepSource.Icons().Count;
                    foreach (Iconxx icon in DeepSource.Icons()) if (icon.folder) num_icon++;
                    if (num_icon <= 8) return;
                    if (Properties.Settings.Default.grandezza_icon2 > 800) return;
                    else
                    {
                        standard2.Width += (int)(passo2 * prop_width_height); standard2.Height += passo2;
                        Properties.Settings.Default.grandezza_icon2 += passo2;
                        MyPanel.allowDraw = false;
                        secondpanel.Refresh(true);
                        MyPanel.allowDraw = true;
                    }
                }
            }
            if (e.KeyCode == Keys.Down)
            {
                if (firstpanel.myposition == MyPanel.maxposition)
                {
                    firstpanel.Scrolling(-1);
                }
                else if (secondpanel.myposition == MyPanel.maxposition)
                {
                    secondpanel.Scrolling(-1);
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                if (firstpanel.myposition == MyPanel.maxposition)
                {
                    firstpanel.Scrolling(1);
                }
                else if (secondpanel.myposition == MyPanel.maxposition)
                {
                    secondpanel.Scrolling(1);
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                if (mediaPanel.onTop)
                {
                    mediaPanel.Media.Back();
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                if (mediaPanel.onTop)
                {
                    mediaPanel.Media.Forward();
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                if (mediaPanel.onTop)
                {
                    if (mediaPanel.fullscreen) mediaPanel.FullScreen();
                    else QuitMediaPanel();
                }
                else if(secondpanel.myposition == MyPanel.maxposition)
                {
                    firstpanel.ToFront();
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Space)// || e.KeyCode == Keys.MediaPlayPause)
            {
                if (mediaPanel.onTop)
                {
                    MediaPlayPause();
                }
            }
            if (e.KeyCode == Keys.H)
            {
                if (mediaPanel.onTop)
                {
                    //for (int i = 1; i < Program.win.mediaPanel.Media.subtitle.count + 1; i++) Console.WriteLine(Program.win.mediaPanel.Media.subtitle.description(i - 1));
                    //Console.WriteLine(Program.win.mediaPanel.Media.audio.count);
                }
            }
        }
        private void MediaPlayPause()
        {
            if (!Program.win.mediaPanel.Media.IsPlaying()) Program.win.mediaPanel.Play();
            else Program.win.mediaPanel.Pause();
            Program.win.mediaPanel.indexHide = 0;
            System.Threading.Thread.Sleep(200);
        }

        public void QuitMediaPanel(object sender, EventArgs e)
        {
            QuitMediaPanel();
        }
        public void QuitMediaPanel()
        {
            mediaPanel.Quit();
            mediaPanel.onTop = false;
            if (mediaPanel.fullscreen) mediaPanel.FullScreen();
            Console.WriteLine("Close Media Panel");
            if (suspendedResize)
            {
                ResizeWin(true, true, true);
            }
            mediaPanel.ToBack();
        }
        void Exclamative_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.exclamationsEnabled)
            {
                Properties.Settings.Default.exclamationsEnabled = false;
                 visualizzaControversieToolStripMenuItem.Checked = false;
            }
            else
            {
                Properties.Settings.Default.exclamationsEnabled = true;
                visualizzaControversieToolStripMenuItem.Checked = true;
            }
        }
        void Film_Click(object sender, EventArgs e)
        {
            //firstpanel.ToFront();
            if (Properties.Settings.Default.valore_mostra == 1) return;
            Properties.Settings.Default.valore_mostra = 1;
            soloSerieTVToolStripMenuItem.Checked = false;
            tuttoToolStripMenuItem.Checked = false;
            Program.Save();
            UpdateSources();
        }
        void Series_Click(object sender, EventArgs e)
        {
            //firstpanel.ToFront();
            if (Properties.Settings.Default.valore_mostra == 2) return;
            Properties.Settings.Default.valore_mostra = 2;
            soloFilmToolStripMenuItem.Checked = false;
            tuttoToolStripMenuItem.Checked = false;
            Program.Save();
            UpdateSources();
        }
        void ViewAll_Click(object sender, EventArgs e)
        {
            //firstpanel.ToFront();
            if (Properties.Settings.Default.valore_mostra == 3) return;
            Properties.Settings.Default.valore_mostra = 3;
            soloFilmToolStripMenuItem.Checked = false;
            soloSerieTVToolStripMenuItem.Checked = false;
            Program.Save();
            UpdateSources();
        }
        
        public void UpdateSources()
        {
            Console.WriteLine("Updating sources..");
            firstpanel.bias = 0;
            SuspendLayout();
            Text = "CyanVideos" + " - Aggiornamento in corso..";
            Supervisor.Disposer();
            foreach (Source source in Sources) {source.Dispose(); }
            Sources.Clear();
            FaultSources.Clear();
            

            if (Properties.Settings.Default.Sources.Length > 2)
            {
                string[] pairDirName = Properties.Settings.Default.Sources.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
                for(int i=0; i<pairDirName.Length/2; i++)
                {
                    bool series = false;
                    if (File.Exists(pairDirName[2 * i] + @"\powervideos_series.txt")) series = true;
                    try
                    {
                        AddNewSource(pairDirName[2 * i], pairDirName[2 * i + 1], series);
                    }
                    catch (Exception)
                    {
                        FaultSources.Add(pairDirName[2 * i], pairDirName[2 * i + 1]);
                    }
                }
            }
            ResizeWin(true, false);
            //Update();
            Text = "CyanVideos";
            ResumeLayout(false);
            Supervisor.ResearchINFOS();
        }

        private void AddNewSource(string directory, string name, bool series = false)
        {
            bool found = false;
            foreach (var source in Sources) if (source.directory == directory) found = true;
            if (found) return;

            Text = "CyanVideos" + " - Aggiornamento in corso..";
            Program.EnableLoading(true);
            if (series)
            {
                try
                {
                    if (File.Exists(directory + @"\infopowervideos.txt") && !File.Exists(directory + @"\powervideos_series.txt")) File.Delete(directory + @"\infopowervideos.txt");
                    File.CreateText(directory + @"\powervideos_series.txt");
                }
                catch (Exception) { }
            }
            else
            {
                if (File.Exists(directory + @"\infopowervideos.txt") && File.Exists(directory + @"\powervideos_series.txt")) File.Delete(directory + @"\infopowervideos.txt");
                if(File.Exists(directory + @"\powervideos_series.txt")) File.Delete(directory + @"\powervideos_series.txt");
            }
            Sources.Add(new Source(directory, name, false, false, true));
            Console.WriteLine("Adding resource: " + directory + " - " + name);
            Program.EnableLoading(false);
        }

        bool gate = false;
        private void mDoubleClick(object sender, EventArgs e)
        {
            gate = false;
        }
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == 0x0312)
                {
                    if ((Keys)((int)m.LParam >> 16 & 0xFFFF) == Keys.MediaPlayPause)
                    {
                        MediaPlayPause();
                    }
                    if ((Keys)((int)m.LParam >> 16 & 0xFFFF) == Keys.MediaNextTrack)
                    {
                        mediaPanel.toolbar.NextClick(null, null);
                    }
                    if ((Keys)((int)m.LParam >> 16 & 0xFFFF) == Keys.MediaPreviousTrack)
                    {
                        mediaPanel.toolbar.PreviousClick(null, null);
                    }
                }
                if (m.Msg == 0x210 && m.WParam.ToInt32() == 513)
                {
                    if (!mediaPanel.Media.Bounds.Contains(PointToClient(MousePosition)) || !mediaPanel.onTop) return;
                    timerDoubleClick.Tick -= mDoubleClick;
                    timerDoubleClick.Stop();
                    timerDoubleClick.Start();

                    if (mediaPanel.toolbar.Bounds.Contains(mediaPanel.toolbar.PointToClient(MousePosition))) { gate = false; return; }
                    if (mediaPanel.trackbar.Bounds.Contains(mediaPanel.trackbar.PointToClient(MousePosition))) { gate = false; return; }
                    if (gate)
                    {
                        mediaPanel.FullScreen();
                        gate = false; return;
                    }
                    gate = true;
                    timerDoubleClick.Tick += mDoubleClick;
                }
                base.WndProc(ref m);
                if (m.Msg == 0x0112)
                {
                    if (m.WParam == new IntPtr(0xF030) || m.WParam == new IntPtr(0xF032))
                    {
                        ResizeWin(true, true, true, false);
                    }
                }
                if (m.Msg == 0x0112)
                {
                    if (m.WParam == new IntPtr(0xF122))
                    {
                        ResizeWin(true, true, true, false);
                    }
                    if (m.WParam == new IntPtr(0xF120)) if (WindowState != FormWindowState.Maximized) ResizeWin(true, true, true, false);
                }
            }
            catch (Exception) { Close(); }
        }

        private void update_source_Click(object sender, EventArgs e)
        {
            UpdateSources();
        }
        private void MenuClick(object sender, EventArgs e)
        {
            RefreshMenu();
        }
        List<ToolStripMenuItem> listMenu = new List<ToolStripMenuItem>();
        private void RefreshMenu()
        {
            listMenu.Clear();
            this.monitorPredefinitoToolStripMenuItem.DropDownItems.Clear();
            foreach (Screen screen in Screen.AllScreens)
            {
                ToolStripMenuItem neww;
                string screenName = ScreenClass.GetScreenName(screen);
                listMenu.Add(neww = new ToolStripMenuItem()
                {
                    BackColor = Color.Black,
                    CheckOnClick = true,
                    Checked = screenName == ScreenClass.GetScreenName(Program.defaultScreen),
                    ForeColor = System.Drawing.Color.White,
                    Name = screenName,
                    Size = new System.Drawing.Size(190, 24),
                    Text = screenName + "  ("+screen.Bounds.Width +" x "+ screen.Bounds.Height + ")",
                });
                neww.Click += new System.EventHandler(SetDefaultMonitor);
            }
            ToolStripMenuItem[] array = listMenu.ToArray();
            this.monitorPredefinitoToolStripMenuItem.DropDownItems.AddRange(array);


            this.soloFilmToolStripMenuItem.Checked = Properties.Settings.Default.valore_mostra == 1;
            this.soloSerieTVToolStripMenuItem.Checked = Properties.Settings.Default.valore_mostra == 2;
            this.tuttoToolStripMenuItem.Checked = Properties.Settings.Default.valore_mostra == 3;
        }
        private void SetDefaultMonitor(object sender, EventArgs e)
        {
            ToolStripMenuItem toolScreen = (ToolStripMenuItem)sender;
            foreach (Screen screen in Screen.AllScreens) if (ScreenClass.GetScreenName(screen) == toolScreen.Name) Program.defaultScreen = screen;
            Properties.Settings.Default.defDevice = toolScreen.Name;
            Properties.Settings.Default.Save();
        }

        private void compact_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.compact)
            {
                Properties.Settings.Default.compact = false;
            }
            else
            {
                Properties.Settings.Default.compact = true;
            }
            Properties.Settings.Default.Save();
            firstpanel.Refresh(true, true, true);
        }

        private void internalPlayer_Click(object sender, EventArgs e)
        {
            if (internalPlayer.Checked && (!Directory.Exists(Program.libvlc) || Program.libvlc==""))
            {
                internalPlayer.Checked = false;
                Program.VLC_Installed = false;
                MessageBox.Show(Program.libvlc+" non esiste. Copiare libvlc (libreria di VLC) all'interno della cartella d'installazione per utilizzare il lettore interno!");
            }
            else
            {
                Program.VLC_Installed = true;
                if (Properties.Settings.Default.internalPlayer)
                {
                    Properties.Settings.Default.internalPlayer = false;
                }
                else
                {
                    Properties.Settings.Default.internalPlayer = true;
                }
                Properties.Settings.Default.Save();
            }
        }

        private void consigliaStrutturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (StructureAdvice.active) StructureAdvice.win.BringToFront();
            else
            {
                StructureAdvice.win = new StructureAdvice();
                StructureAdvice.win.Show();
            }
        }
    }

}
