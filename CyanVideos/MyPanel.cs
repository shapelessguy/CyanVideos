using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class MyPanel : PictureBox
    {
        static public int maxposition = 0;
        static public Size firstPanelSize = new Size(1, 1);
        static public Point firstPanelLocation = new Point(1, 1);            
        static public Size secPanelSize = new Size(1,1);
        static public Point secPanelLocation = new Point(1,1);
        static public string fatherPanel2 = "";
        static public bool allowDraw = true;
        public int myposition = 0;
        List<Drawning> ToDraw = new List<Drawning>();
        public Label Scrollbar;
        private System.Windows.Forms.Timer timer, tooltip;
        public List<Source> SourcesToShow = new List<Source>();
        public int level;
        public int leftmargin = 50;
        public int rightmargin = 120;
        public int leftIconMargin;
        public int rightIconMargin;
        public static bool working = false;

        public void Disposer()
        {
            foreach (Drawning draw in ToDraw) draw.Dispose();
        }
        public MyPanel(int level)
        {
            this.level = level;
            if (level == 2) { leftmargin = 10; rightmargin = 50; }
            timer = new System.Windows.Forms.Timer() { Enabled = true, Interval = 5 };
            tooltip = new System.Windows.Forms.Timer() { Enabled = true, Interval = 50 };
            delayInvalidate = new System.Windows.Forms.Timer() { Enabled = true, Interval = 50 };
            delayInvalidate.Tick += (o, e) => { Invalidate(); delayInvalidate.Enabled = false; };
            tooltip.Tick += ToolTip;
            Location = new Point(-1,-1);
            Size = new Size(1,1);
            if (level == 1) BackColor = Color.Black;
            else
            {
                BackgroundImage = Properties.Resources.Background;
                //BackColor = Color.Transparent;
                // BackColor = Color.FromArgb(10, 10, 10);
            }

            Paint += new PaintEventHandler(Power_Paint);
            if (level == 2) LocationChanged += (o, e) => { //Console.WriteLine(Bounds);
                try
                {
                    //if (Location.X < 0 || Location.X > Program.win.Width || Location.Y < 0 || Location.Y > Program.win.Height) Bounds = new Rectangle(1, 1, 1, 1);
                }
                catch (Exception) { }
            };


            //Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);
            Scrollbar = new Label()
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom),
                Size = new Size(60, Height - 80),
                Location = new Point(Width - 100, 50),
                //BackColor = Color.Red,
                BackgroundImageLayout = ImageLayout.Stretch,
            };
            if (level != 1)
            {
                Scrollbar.Location = new Point(Width - 50, 50);
                Scrollbar.Size = new Size(40, Height - 80);
            }
            Controls.Add(Scrollbar);
            EnableControls();
            //if (level == 2) EnableControls();
        }

        private int n_maglie = 0;
        private int scrollheight = 0;
        public void RefreshScrollbarImage()
        {
            if (Scrollbar.Height != 0)
            {
                scrollheight = Scrollbar.Height;
                int new_n_maglie = (int)(8F- ((16*620F)/1378F) + (16F*Scrollbar.Height)/1378F + 0.5f);
                if(new_n_maglie != n_maglie)
                {
                    n_maglie = new_n_maglie;
                    if (n_maglie <= 8) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar8;
                    else if (n_maglie == 9) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar9;
                    else if (n_maglie == 10) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar10;
                    else if (n_maglie == 11) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar11;
                    else if (n_maglie == 12) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar12;
                    else if (n_maglie == 13) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar13;
                    else if (n_maglie == 14) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar14;
                    else if (n_maglie == 15) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar15;
                    else if (n_maglie == 16) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar16;
                    else if (n_maglie == 17) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar17;
                    else if (n_maglie == 18) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar18;
                    else if (n_maglie == 19) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar19;
                    else if (n_maglie == 20) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar20;
                    else if (n_maglie == 21) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar21;
                    else if (n_maglie == 22) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar22;
                    else if (n_maglie == 23) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar23;
                    else if (n_maglie >= 24) Scrollbar.BackgroundImage = Properties.Resources.Scrollbar24;
                }
            }
        }

        private bool controls_enabled = false;
        public void EnableControls()
        {
            if (controls_enabled) return;
            controls_enabled = true;
            MouseWheel += Scrolling;
            MouseMove += MovingCursor;
            MouseLeave += (o, e) => { actualDrawning = null; Invalidate(); };
            MouseDown += (o, e) => {
                mouseDown = true;
                mouseposition = new Point(e.X, e.Y);
            };
            MouseUp += (o, e) => { Mouse_Click(o, e); mouseDown = false; scrollbar_mouseDown = false; };
            MouseDown += MaybeClick;
            DoubleClick += Double_Click;
            Scrollbar.MouseDown += (o, e) => { scrollbar_mouseDown = true; };
            Scrollbar.MouseClick += (o, e) => { ClickOnScrollBar(e); };
            Scrollbar.MouseMove += MovingCursor;
            Scrollbar.MouseUp += (o, e) => { mouseDown = false; scrollbar_mouseDown = false; };
        }

        bool mouseDown = false;
        bool scrollbar_mouseDown = false;
        Point mouseposition;

        public void ToFront(bool timer = true)
        {
            System.Windows.Forms.Timer bringingToFront;
            myposition = maxposition + 1;
            maxposition = myposition;
            
            if (!timer) { Action(); }
            else
            {
                bringingToFront = new System.Windows.Forms.Timer() { Enabled = true, Interval = 10 };
                bringingToFront.Tick += (o, e) =>
                {
                    Action();
                    if(level == 2) BringToFront();
                    bringingToFront.Dispose();
                };
            }

            void Action()
            {
                if(!Program.win.mediaPanel.onTop) Program.win.Menu.BringToFront();
                if (level == 1)
                {
                    //Program.win.firstpanel.BackColor = Color.Black;
                    //Program.win.firstpanel.Update();
                    if (Program.win != null)
                    {
                        Program.win.DisposeDeepSource();
                        if (Program.win.secondpanel != null)
                        {
                            Program.win.secondpanel.Invalidate();
                            Program.win.secondpanel.Refresh();
                            Program.win.secondpanel.SendToBack();
                            //Program.win.secondpanel.Location = new Point(3000, 1000);
                        }
                    }
                }
                //else if (Location.X < 0 || Location.X > Program.win.Width || Location.Y < 0 || Location.Y > Program.win.Height) Bounds = new Rectangle(1, 1, 1, 1);
            }
            //BringToFront();
        }

        int maxdrawn = 100;
        double step = 0;
        public int bias = 0;
        public void Scrolling(object sender, MouseEventArgs e)
        {
            if (myposition != maxposition) return;
            delta = e.Delta; //MovingCursor(sender, e);
            Scrolling();
        }
        public static int i = 0;
        public static int initial_i = 5;
        public static int Final_i = 13;
        public void Scrolling(int delta_ = 0)
        {
            if (delta_ != 0) delta = delta_;
            step = 0.4;//(int)(Height * 0.5 + 2);

            steps = StepFunction(Final_i);

            if (i > 4) {
                if (i < 10) i -= 4;
                else i = initial_i;
            }
            else i = 0;
            if (!scroll_enabled) { i = initial_i; timer.Tick += Scroll; scroll_enabled = true; }
        }

        private int[] StepFunction(int n)
        {
            List<int> steps = new List<int>();
            List<int> steps_out = new List<int>();
            for (int i = 0; i < n; i++) steps.Add(i);
            for (int i = 0; i < n; i++) steps[i] = (int)(((float)(7/4)* steps[i] * steps[i] - 1* steps[i] + 70)*step);
            for (int i = n - 1; i >= 0; i--) steps_out.Add(steps[i]);
            return steps_out.ToArray();
        }

        int[] steps;
        int delta = 0;
        public static bool scroll_enabled = false;
        void Scroll(object sender2, EventArgs e2)
        {
            if (i == steps.Length) {
                StopScroll(); 
                return;
            }
            int result = CheckBorders();
            if ((result == 1 && delta>0) || (result == -1 && delta < 0) || result==-2)
            {
                //Console.WriteLine("Stop Forced!");
                StopScroll(); return; }
            if (delta > 0) bias += steps[i];
            if (delta < 0) bias -= steps[i];
            Invalidate();
            //Console.WriteLine(steps[i]);
            i++;

        }
        private void StopScroll()
        {
            timer.Tick -= Scroll;
            bool nulla = i == initial_i || i == 1;
            i = initial_i;
            scroll_enabled = false;
            if (!nulla)
            {
                previous_drawn = null;
                previousDrawning = null;
                MovingCursor(null, null);
                Invalidate();
            }
        }
        private int CheckBorders()
        {
            bool up = false;
            bool down = false;
            //Console.WriteLine("Height: "+Height+", bias: "+bias+", MaxDrawn: "+maxdrawn);
            if (bias >= 0) { bias = 0; up = true; }
            if (Height - bias >= maxdrawn) { if (maxdrawn - Height > 0) { bias = Height - maxdrawn; down = true; } else { up = true; down = true; bias = 0; } }

            if (up) { if (down) return -2; else return 1; }
            else if (down) return -1;
            else return 0;
        }

        private void Power_Paint(object sender, PaintEventArgs e)
        {
            PanelPaint(e);
        }
        private void ClearDrawning()
        {
            if (!onlyLocation)
            {
                foreach (Drawning draw in ToDraw) draw.Dispose();
                ToDraw.Clear();
                GC.Collect();
            }
        }
        public bool paint = false;
        public bool paint2 = false;
        public bool without_resize = false;
        public bool onlyLocation = false;
        public bool refreshLabel = false;

        int width = 0;
        int height = 0;

        public void ResizeForm(bool clear = true)
        {
            if (level == 1)
            {
                if (firstPanelSize == null || firstPanelLocation == null)
                {
                    Size = new Size(1, 1);
                    Location = new Point(-1, -1);
                }
                else
                {
                    Size = firstPanelSize;
                    Location = firstPanelLocation;
                }
            }
            else
            {
                if (secPanelSize == null || secPanelLocation == null)
                {
                    Size = new Size(1, 1);
                    Location = new Point(-1, -1);
                }

                Size = secPanelSize;
                Location = secPanelLocation;
            }
            if (width != Window.standard.Width && height != Window.standard.Height)
            {
                width = Window.standard.Width;
                height = Window.standard.Height;
            }
            RefreshScrollbarImage();
            //if (clear) { ClearPanel(); }
        }

        bool justClear = false;
        public void ClearPanel(bool clearDrawnings = false)
        {
            this.clearDrawnings = clearDrawnings;
            justClear = true;
            Refresh();
        }
        public void ClearPanel(Graphics g, bool clearDrawnings)
        {
            if (level == 2) g.Clear(Color.Black);
            else g.Clear(Color.Black);
            if(clearDrawnings) ClearDrawning();
            clearDrawnings = false;
            refreshLabel = false;
        }

        bool tooltiped = false;
        bool clearDrawnings = false;
        System.Windows.Forms.Timer delayInvalidate;
        private void PanelPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            if (justClear) { justClear = false; ClearPanel(g, clearDrawnings); return; }

            tooltiped = false;
            if (!onlyLocation) Supervisor.ImportIconxx();
            //if (SourcesToShow == null || SourcesToShow.Count == 0 || (SourcesToShow[0] == null) && level==2)
            {
              //  ClearPanel(g, true);
            }

            ResizeForm(false);


            if ((!paint && level==1) || (!paint2 && level == 2))
            {
                CheckBorders();
                try
                {
                    foreach (Drawning draw in ToDraw) draw.Draw(g, level, bias);
                }
                catch (Exception) { Console.WriteLine("Exception raised by PanelPaint in MyPanel"); }
            }
            else if (SourcesToShow.Count > 0)
            {
                int num_icon = SourcesToShow[0].Icons().Count;
                foreach (Iconxx icon in SourcesToShow[0].Icons()) if (icon.folder) num_icon++;
                if (num_icon <= 8 && level == 2)
                {
                    FinallyPaint2(g);
                    //g.DrawImage(Properties.Resources.panel2Background, 0, 0, Program.win.secondpanel.Width, Program.win.secondpanel.Height + 10);
                    paint2 = false;
                    CheckBorders();
                    Invalidate();

                    Program.win.Activate();
                }
                else
                {
                    //Console.WriteLine("Paint: "+this.paint +"   Only_location: "+ this.onlyLocation);
                    FinallyPaint(g);
                    if (level == 1) paint = false;
                    else paint2 = false;
                    CheckBorders();
                    Invalidate();
                    if (Program.win.WindowState != FormWindowState.Normal) Program.win.WindowState = FormWindowState.Maximized;
                    Program.win.Activate();
                }
            }
            
            Program.win.ricerca.FindingHide();
            onlyLocation = false; refreshLabel = false;
        }


        bool firstDraw = false;
        private void FinallyPaint(Graphics g, int bias = 0)
        {
            Console.WriteLine("Complete paint on First Panel - Sources: "+SourcesToShow.Count);
            //firstDraw = true;
            foreach (var source in SourcesToShow) Console.WriteLine("      -> "+source.directory);
            working = true;
            if(!Program.win.mediaPanel.onTop) Program.EnableLoading(true);
            int locationIndex = 0;
            if (!onlyLocation) ClearDrawning();
            if (refreshLabel) for (int i = ToDraw.Count - 1; i >= 0; i--) if (ToDraw[i].Icon == null) { ToDraw.RemoveAt(i); }
            Scrollbar.Show();
            int RealWidth = Width - leftmargin - rightmargin;
            int num_line;
            int iconWidth;
            int iconHeight;
            int intraDistanceX;
            int intraDistanceY;
            if (level == 1)
            {
                num_line = RealWidth / Window.standard.Width;
                iconWidth = Window.standard.Width;
                iconHeight = Window.standard.Height;
                intraDistanceX = Window.intraDistanceX;
                intraDistanceY = Window.intraDistanceY;
            }
            else
            {
                num_line = RealWidth / Window.standard2.Width;
                iconWidth = Window.standard2.Width;
                iconHeight = Window.standard2.Height;
                intraDistanceX = Window.intraDistanceX2;
                intraDistanceY = Window.intraDistanceY2;
            }
            int num_max_icons = (int)(RealWidth / (double)(iconWidth + intraDistanceX));
            int spaziox_initial = (RealWidth - num_max_icons * (iconWidth + intraDistanceX)) / 2 + leftmargin;
            int locationx = spaziox_initial;
            leftIconMargin = locationx;
            int locationy = 0;

            locationy += intraDistanceY;
            int holes = 0;
            int filled = 0;

            List<List<Iconxx>> MaskedSources = new List<List<Iconxx>>();
            if (Properties.Settings.Default.compact && level == 1 && SourcesToShow.Count>1)
            {
                List<Iconxx> films = new List<Iconxx>();
                List<Iconxx> series = new List<Iconxx>();
                foreach (Source source in SourcesToShow)
                {
                    if (!source.series) films.AddRange(source.Icons());
                    else series.AddRange(source.Icons());
                }
                films = films.OrderBy(o => o.title).ToList();
                series = series.OrderBy(o => o.title).ToList();
                MaskedSources.Add(series);
                MaskedSources.Add(films);
            }
            else
            {
                SourcesToShow = SourcesToShow.OrderBy(o => o.tag.name).ToList();
                foreach (Source source in SourcesToShow)
                {
                    List<Iconxx> icons = new List<Iconxx>();
                    foreach (Iconxx icon in source.Icons()) icons.Add(icon);
                    MaskedSources.Add(icons);
                }
            }

            try
            {
                for (int i= 0; i < MaskedSources.Count; i++)
                {
                    if (!Properties.Settings.Default.compact || Supervisor.researchSource != null || level == 2)
                    {
                        DrawSource(SourcesToShow[i], leftmargin, locationy, RealWidth, SourceTag.height, i);
                        locationy += SourceTag.height + intraDistanceY;
                    }
                    else if(MaskedSources[i].Count> 0 && level == 1)
                    {
                        if (Supervisor.researchSource != null)
                        {
                            DrawText(Supervisor.researchSource.directory, leftmargin, locationy, RealWidth, SourceTag.height, i);
                        }
                        else
                        {
                            string text = "";
                            if (i == 1) text = "Movies"; else text = "Series";
                            DrawText(text, leftmargin, locationy, RealWidth, SourceTag.height, i);
                        }
                        locationy += SourceTag.height + intraDistanceY;
                    }

                    List<Point> EmptySpaces = new List<Point>();
                    foreach (Iconxx label in MaskedSources[i])
                    {
                        if (!Program.win.mediaPanel.onTop) Program.EnableLoading();
                        if (!label.folder)
                        {
                            if (EmptySpaces.Count == 0)
                            {
                                if (locationx + iconWidth + intraDistanceX - leftmargin > RealWidth) { locationy += iconHeight + intraDistanceY; rightIconMargin = locationx; locationx = spaziox_initial; }
                                Draw(label, locationx, locationy, iconWidth, iconHeight, i);
                                locationx += iconWidth + intraDistanceX;
                            }
                            else
                            {
                                filled++;
                                Draw(label, EmptySpaces[EmptySpaces.Count - 1].X, EmptySpaces[EmptySpaces.Count - 1].Y, iconWidth, iconHeight, i);
                                EmptySpaces.RemoveAt(EmptySpaces.Count - 1);
                            }
                        }
                        else
                        {
                            if (locationx + 2 * iconWidth + intraDistanceX - leftmargin > RealWidth && locationx + iconWidth - leftmargin < RealWidth)
                            {
                                holes++;
                                EmptySpaces.Add(new Point(locationx, locationy));
                                locationy += iconHeight + intraDistanceY; locationx = spaziox_initial;
                            }
                            else if (locationx + 2 * iconWidth + intraDistanceX - leftmargin > RealWidth)
                            {
                                locationy += iconHeight + intraDistanceY; rightIconMargin = locationx; locationx = spaziox_initial;
                            }
                            Draw(label, locationx, locationy, 2 * iconWidth + intraDistanceX, iconHeight, i);
                            locationx += 2 * iconWidth + 2 * intraDistanceX;
                        }
                    }
                    if (i != MaskedSources.Count - 1 && MaskedSources[i].Count>0) locationy += intraDistanceY + iconHeight;
                    locationx = spaziox_initial;

                    try
                    {
                        if (holes > 0 && filled == 0) foreach (Drawning draw in ToDraw)
                            {
                                if (draw.Icon != null && draw.sourceNum == i)
                                {
                                    //Console.WriteLine(draw.Icon.title + "     " + draw.sourceNum + "  |  "+holes+" - "+filled );
                                    draw.Rect.Location = new Point(draw.Rect.Location.X + (iconWidth + intraDistanceX) / 2, draw.Rect.Location.Y);
                                }
                            }
                        foreach (Drawning draw in ToDraw) if(draw.sourceNum == i) { draw.Draw(g, level, bias); }
                    }
                    catch (Exception) { Console.WriteLine("Exception raised by FinallyPaint in MyPanel"); }
                    EmptySpaces.Clear();
                    holes = 0; filled = 0;
                }
            }
            catch (Exception e) { Console.WriteLine("Exception raised by FinallyPaint in MyPanel: " + e.Message); Program.EnableLoading(false); return; }

            if (locationx == spaziox_initial) locationy += intraDistanceY + iconHeight;
            maxdrawn = locationy + 2*intraDistanceY;
            
            without_resize = false;





            void Draw(Iconxx icon, int locx, int locy, int width, int height, int sourceNum)
            {
                try
                {
                    if (without_resize) ToDraw.Add(new Drawning(icon, locx, locy, level, sourceNum));
                    else if (onlyLocation && !refreshLabel) { if (ToDraw[locationIndex].Icon != null) { ToDraw[locationIndex].Rect.Location = new Point(locx, locy); ToDraw[locationIndex].sourceNum = sourceNum; } locationIndex++; }
                    else if (onlyLocation && refreshLabel) { foreach (Drawning draw in ToDraw) if (draw.Icon != null && draw.Icon.fullpath == icon.fullpath) { draw.Rect.Location = new Point(locx, locy); draw.sourceNum = sourceNum; } }
                    else { ToDraw.Add(new Drawning(icon, locx, locy, width, height, level, sourceNum)); }
                }
                catch (Exception) { Console.WriteLine("Exception raised by method Draw in FinallyPaint"); Program.EnableLoading(false); onlyLocation = false; refreshLabel = false; return; }
            }
            void DrawSource(Source source, int locx, int locy, int width, int height, int sourceNum)
            {
                try
                {
                    if (onlyLocation && !refreshLabel && locationIndex!= 0) { if (ToDraw[locationIndex].Icon == null) { ToDraw[locationIndex].Rect = new Rectangle(locx, locy, width, height); locationIndex++; } }
                    else ToDraw.Add(new Drawning(source, locx, locy, width, height, sourceNum));
                    //if (locationIndex == 0) locationIndex += 1;
                }
                catch (Exception) { Console.WriteLine("Exception raised by method Draw in FinallyPaint"); Program.EnableLoading(false); onlyLocation = false; refreshLabel = false; return; }
            }
            void DrawText(string text, int locx, int locy, int width, int height, int sourceNum)
            {
                try
                {
                    if (onlyLocation && !refreshLabel) { if (ToDraw[locationIndex].Icon == null) { ToDraw[locationIndex].Rect = new Rectangle(locx, locy, width, height); locationIndex++; } }
                    else ToDraw.Add(new Drawning(text, locx, locy, width, height, sourceNum));
                }
                catch (Exception) { Console.WriteLine("Exception raised by method Draw in FinallyPaint"); Program.EnableLoading(false); onlyLocation = false; refreshLabel = false; return; }
            }
            Program.EnableLoading(false);
            onlyLocation = false;
            refreshLabel = false;
            working = false;
        }


        int tentatives = 0;
        private void FinallyPaint2(Graphics g, int bias = 0)
        {
            working = true;
            if (!Program.win.mediaPanel.onTop) Program.EnableLoading();
            if (!onlyLocation) ClearDrawning();
            if (SourcesToShow == null) { return; }
            if (SourcesToShow.Count == 0) {  return; }
            Scrollbar.Hide();
            int locationIndex = 0;
            Console.WriteLine("Complete paint on panel2");
            int num_icon = SourcesToShow[0].Icons().Count;
            int num_iconx = 0, num_icony = 1;
            foreach (Iconxx icon in SourcesToShow[0].Icons()) if (icon.folder) num_icon++;
            if (num_icon < 5) num_iconx = num_icon;
            else { num_iconx = 4; num_icony = 2; }

            int intraDistanceX = Window.intraDistanceX2;
            int intraDistanceY = Window.intraDistanceY2;
            int iconHeight = (Height - SourceTag.height - (num_icony+1) *intraDistanceY) / num_icony;
            int iconWidth = (int)(iconHeight * Window.prop_width_height);
            
            int spaziox_initial = Width / 2 - (num_iconx * (iconWidth + intraDistanceX) - intraDistanceX) / 2;
            int locationx = 10;
            int locationy = 10;

            Size sourceSize = new Size(Width - 200, SourceTag.height);
            DrawSource(SourcesToShow[0], 0 + 100, locationy, sourceSize.Width, sourceSize.Height, 0);
            locationy += SourceTag.height + intraDistanceY;
            
            locationx = spaziox_initial;
            int n = 0;
            List<Point> EmptySpaces = new List<Point>();
            int holes = 0;
            int filled = 0;
            foreach (Iconxx icon in SourcesToShow[0].Icons())
            {
                if (!Program.win.mediaPanel.onTop) Program.EnableLoading();
                if (!icon.folder)
                {
                    if (EmptySpaces.Count == 0)
                    {
                        if (locationx + (iconWidth - intraDistanceX) > Width - spaziox_initial) { locationy += iconHeight + intraDistanceY; locationx = spaziox_initial; }
                        Draw(icon, locationx, locationy, iconWidth, iconHeight, 0);
                        locationx += iconWidth + intraDistanceX;
                    }
                    else
                    {
                        filled++;
                        Draw(icon, EmptySpaces[EmptySpaces.Count - 1].X, EmptySpaces[EmptySpaces.Count - 1].Y, iconWidth, iconHeight, 0);
                        EmptySpaces.RemoveAt(EmptySpaces.Count - 1);
                    }
                    n++;
                }
                else
                {
                    if (locationx + (iconWidth - intraDistanceX)*2 > Width - spaziox_initial && locationx + (iconWidth - intraDistanceX) < Width - spaziox_initial)
                    {
                        //Console.WriteLine(icon.name + "  addspace");
                        holes++;
                        EmptySpaces.Add(new Point(locationx, locationy));
                        locationy += iconHeight + intraDistanceY; locationx = spaziox_initial;
                    }
                    else if (locationx + (iconWidth - intraDistanceX) > Width - spaziox_initial)
                    {
                        locationy += iconHeight + intraDistanceY; locationx = spaziox_initial;
                    }
                    Draw(icon, locationx, locationy, 2 * iconWidth + intraDistanceX, iconHeight, 0);
                    locationx += 2*iconWidth + 2*intraDistanceX;
                    n += 2;
                }
                if (locationx == spaziox_initial) locationy += intraDistanceY + iconHeight;
                maxdrawn = locationy + 2 * intraDistanceY;
            }

            try
            {
                if (holes > 0 && filled == 0) foreach (Drawning draw in ToDraw) { if (draw.Icon != null) draw.Rect.Location = new Point(draw.Rect.Location.X +(iconWidth + intraDistanceX) / 2, draw.Rect.Location.Y); }
                foreach (Drawning draw in ToDraw) { draw.Draw(g, level, bias); }
            }
            catch (Exception) { Console.WriteLine("Exception raised by FinallyPaint2 in MyPanel"); }


            void Draw(Iconxx icon, int locx, int locy, int width, int height, int sourceNum)
            {
                try
                {
                    if (onlyLocation) { if(ToDraw[locationIndex].Icon!=null) ToDraw[locationIndex].Rect.Location = new Point(locx, locy); locationIndex++; }
                    else ToDraw.Add(new Drawning(icon, locx, locy, width, height, level, sourceNum));
                }
                catch (Exception) { Console.WriteLine("Exception raised by FinallyPaint2 in MyPanel"); onlyLocation = false; if (tentatives < 3) { tentatives++; paint = true; Invalidate(); } else { tentatives = 0; Program.EnableLoading(false); return; } }
            }
            void DrawSource(Source source, int locx, int locy, int width, int height, int sourceNum)
            {
                try
                {
                    if (onlyLocation) { if (ToDraw[locationIndex].source != null) ToDraw[locationIndex].Rect = new Rectangle(locx, locy, width, height); locationIndex++; }
                    else ToDraw.Add(new Drawning(source, locx, locy, width, height, sourceNum));
                }
                catch (Exception) { Console.WriteLine("Exception raised by FinallyPaint2 in MyPanel");  onlyLocation = false;if (tentatives < 3) { tentatives++; paint = true; Invalidate(); } else { tentatives = 0; Program.EnableLoading(false); return; } }
            }
            Program.EnableLoading(false);
            tentatives = 0; onlyLocation = false;
            working = false;
        }





        Graphics g;
        Drawning previousDrawning;
        Drawning actualDrawning;
        static int resting = 0;

        private void ClickOnScrollBar(MouseEventArgs e)
        {
            if (!allowDraw) return;
            Program.win.FocusGrip();
            float level = e.Y;
            if (e.Y < 0) level = 0;
            else if (e.Y > Scrollbar.Height) level = Scrollbar.Height;
            level = level / Scrollbar.Height;
            bias = (int)((Height - maxdrawn) * level);
            mouseposition = new Point(e.X, e.Y);
            CheckBorders();
            Invalidate();
        }
        private void MovingCursor(object sender, MouseEventArgs e)
        {
            if (!allowDraw) return;
            int delta = 0;
            bool foundactdrowning = false;
            if (e != null)
            {
                //foundactdrowning = GetActualDrawning(e.Location);
                //if(previousDrawning != actualDrawning) 
                delta = mouseposition.Y - e.Y;
                mouseposition = new Point(e.X, e.Y);
                previous_drawn = null;
            }
            if (scroll_enabled) { return; }
            if (mouseDown)
            {
                bias -= delta;
                CheckBorders();
                Invalidate();
            }
            else if (scrollbar_mouseDown)
            {
                if (e != null) ClickOnScrollBar(e);
            }
            else
            {
                Point location;
                if (e == null) { location = new Point(mouseposition.X, mouseposition.Y - bias); }
                else location = new Point(e.Location.X, e.Location.Y - bias);
                foundactdrowning = GetActualDrawning(location);
                if (!foundactdrowning) { actualDrawning = null; previous_drawn = null; }
                if (actualDrawning != previousDrawning)
                {
                    resting = 0;
                    if (previousDrawning != null) previousDrawning.HideExtra();
                    Refresh();
                    previousDrawning = actualDrawning;
                    if (actualDrawning != null)
                    {
                        resting = 0;
                        g = CreateGraphics();
                        actualDrawning.HideExtra();
                        actualDrawning.DrawMaximized(g, level, bias);
                        
                    }
                }
            }
        }

        private bool GetActualDrawning(Point location)
        {
            try
            {
                foreach (Drawning draw in ToDraw)
                {
                    if (draw.Rect.Contains(location))
                    {
                        actualDrawning = draw;
                        return true;
                    }
                }return false;
            }
            catch (Exception) { Console.WriteLine("Exception raised by GetActualDrawning in MyPanel"); return false; }
        }

        Drawning previous_drawn;
        Drawning act_drawn;
        bool reallyTooltiped = false;
        int inArow = 0;
        private void ToolTip(object sender, EventArgs e)
        {
            if (!tooltiped) inArow++;
            else inArow = 0;
            if (inArow > 100) inArow = 100;
            if (inArow > 5) reallyTooltiped = false; else reallyTooltiped = true;
            if (!allowDraw) { previous_drawn = null; actualDrawning = null; act_drawn = null; return; }
            if (myposition != maxposition) return;
            resting++;
            if (resting > 100) resting = 100;
            if (resting >= 10 && resting <13 && allowDraw)
            {
                act_drawn = actualDrawning;
                if (actualDrawning != null) if (actualDrawning.Icon != null && !scroll_enabled)
                    {
                        //if (!actualDrawning.tooltiped)
                        {
                            actualDrawning.ShowExtra();
                            g = CreateGraphics();
                            actualDrawning.DrawMaximized(g, level, bias);
                            tooltiped = true;
                            //tooltiped = true;
                        }
                        previous_drawn = act_drawn;
                    }
            }
        }

        Point mouseposition_click;
        private void MaybeClick(object sendeer, MouseEventArgs e)
        {
            resting = 21;
            mouseposition_click = e.Location;
            if (!allowDraw) return;
            Program.win.FocusGrip();
            if (e.Button == MouseButtons.Right) return;
            StopScroll();
        }
        private double dist(Point point1, Point point2)
        {
            return Math.Sqrt((point1.X - point2.X)* (point1.X - point2.X) + (point1.Y - point2.Y)* (point1.Y - point2.Y));
        }
        private void Mouse_Click(object sender, MouseEventArgs e)
        {
            if (!allowDraw) return;
            if (e.Button == MouseButtons.Right) return;
            Program.win.FocusGrip();
            mouseposition = e.Location;
            if (maxposition != myposition && level == 1) { ToFront(); return; }

            if (dist(mouseposition, mouseposition_click) > 3) return;

            location = new Point(e.Location.X, e.Location.Y - bias);
            timerClick = new System.Windows.Forms.Timer() { Enabled = true, Interval = 10, };
            timerClick.Tick += ClickReally;
        }
        System.Windows.Forms.Timer timerClick;
        Point location;
        private void ClickReally(object sender, EventArgs e)
        {
            Console.WriteLine("Real Click");
            timerClick.Dispose();
            try
            {
                foreach (Drawning draw in ToDraw)
                {
                    if (draw.Rect.Contains(location))
                    {
                        if (draw.source != null)
                        {
                            if (draw.PrevRect.Contains(location)) { draw.source.ClickPrevious(draw.source.directory, draw.source.series); }
                            return;
                        }
                        resting = -10;
                        if (draw.Annotation.Contains(location)) { draw.Icon.ClickExclamation(null, null); draw.tooltiped = false; }
                        else if (draw.Continue.Contains(location) && reallyTooltiped) { draw.Icon.ContinueToWatch.ClickEvent(sender, null); }
                        else if (draw.ExtraRect.Contains(location) && reallyTooltiped) { draw.Icon.Info(null, null); draw.tooltiped = false; }
                        else if (draw.ChangeImageRect.Contains(location) && reallyTooltiped) { draw.Icon.Check_SubFolders(true); draw.tooltiped = false; }
                        else { 
                            if (level == 1) { fatherPanel2 = draw.Icon.fullpath;} 
                            draw.Icon.ClickEvent(null, null);  }
                    }
                }
            }
            catch (Exception ex) { 
                Console.WriteLine("Exception raised by Mouse_Click in MyPanel");
                Console.WriteLine(ex.Message);
            }
        }
        private void Double_Click(object sender, EventArgs e)
        {
            //Focus();
            Point location = new Point(mouseposition.X, mouseposition.Y - bias);
            try
            {
                foreach (Drawning draw in ToDraw)
                {
                    if (draw.Rect.Contains(location) && draw.source != null)
                    {
                        Rename window = new Rename(draw.source);
                        window.Show();
                    }
                }
            }
            catch (Exception) { Console.WriteLine("Exception raised by Double_Click in MyPanel"); }
        }
        public void Refresh(bool paint = false, bool onlyLocation = false, bool refreshLabel = false)
        {
            // Console.WriteLine("paint:"+paint + " onlyLocation:"+onlyLocation + " refreshLabel:"+ refreshLabel);
            if (onlyLocation) paint = true;
            int prev_bias = bias;
            if (paint) { if (level == 1) { this.paint = paint; this.refreshLabel = refreshLabel; } else this.paint2 = paint; }// bias = 0; }
            if (onlyLocation) this.onlyLocation = true;
            if (!firstDraw) this.onlyLocation = false;
            base.Refresh();
            this.onlyLocation = false;
            this.refreshLabel = false;
            bias = prev_bias;
        }
        public void RefreshImages()
        {
            try
            {
                foreach (Drawning draw in ToDraw) 
                    if (draw.Icon != null) 
                        if (!draw.Icon.image_validated) { 
                            Console.WriteLine("Validating: "+draw.Icon.title); draw.ValidateImage(); 
                        }
                previous_drawn = null;
                Invalidate();
            }
            catch (Exception) { Console.WriteLine("Exception raised by RefreshImages in MyPanel"); }
        }

    }
}
