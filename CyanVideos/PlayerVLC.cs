using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace CyanVideos
{
    public class PlayerVLC : Panel
    {
        //public AxAXVLC.AxVLCPlugin2 media;
        //private System.Windows.Forms.Integration.ElementHost elementHost1;
        private LibVLCSharp.WinForms.VideoView media;
        public List<LibVLCSharp.WinForms.VideoView> medias = new List<LibVLCSharp.WinForms.VideoView>();
        private System.Threading.Timer TimerLoad;
        public static bool flag = false;
        public PlayerVLC()
        {
            //InitializeComponent();
            if (Program.VLC_Installed) NewMedia();
            TimerLoad = new System.Threading.Timer(TimingLoad, null, 500, System.Threading.Timeout.Infinite);
        }

        public double InputPosition()
        {
            if (media == null) return 0;
            if(!media.MediaPlayer.WillPlay) return 0;
            try { return (double)media.MediaPlayer.Position; } catch (Exception) { return 0; }

            //return media.input.Position;
        }
        public void SetInputPosition(double position)
        {
            if (media == null) return;
            media.MediaPlayer.Position = (float)position;
        }
        public long InputTime()
        {
            if (media == null) return 0;
            return media.MediaPlayer.Time;
        }
        public double InputLength()
        {
            if (media == null) return 0;
            return media.MediaPlayer.Length - 1000;
        }
        public void Play()
        {
            if (media == null) return;
            Console.WriteLine("PLAY");
            media.MediaPlayer.Play();
        }
        public void Pause()
        {
            if (media == null) return;
            media.MediaPlayer.Pause();
        }
        public void SetVisible(bool visible)
        {
            if (media == null) return;
            Visible = visible;
        }
        public bool IsPlaying()
        {
            if (media == null) return false;
            return media.MediaPlayer.IsPlaying;
        }
        public void Forward()
        {
            if (media == null) return;
            float step = (float)((double)1 / (double)(media.MediaPlayer.Length / 59500)); //1min
            if ((double)(step * 118) / 60 + media.MediaPlayer.Position >= 1) { OnEndReached(); return; }
            media.MediaPlayer.Position += step;
            Program.win.mediaPanel.UpdateTime();
        }
        public void Back()
        {
            if (media == null) return;
            float step = (float)((double)1 / (double)(media.MediaPlayer.Length / 59500)); //1min
            if (media.MediaPlayer.Position - step < 0) { media.MediaPlayer.Position = 0; return; }
            media.MediaPlayer.Position -= step;
            Program.win.mediaPanel.UpdateTime();
        }

        public ToolStripMenuItem[] GetAudioToolMenu()
        {
            int others = media.MediaPlayer.VideoTrackCount - 1;
            if (media == null) return null;
            List<ToolStripMenuItem> listMenuAudio = new List<ToolStripMenuItem>();

            Dictionary<int, string> audioTrack = new Dictionary<int, string>();
            foreach (var audio in media.MediaPlayer.AudioTrackDescription) audioTrack.Add(audio.Id - others, audio.Name);

            bool first = true;
            foreach (var item in audioTrack)
            {
                ToolStripMenuItem neww;
                listMenuAudio.Add(neww = new ToolStripMenuItem()
                {
                    BackColor = System.Drawing.Color.Black,
                    CheckOnClick = true,
                    ForeColor = System.Drawing.Color.White,
                    Name = item.Value,
                    Size = new System.Drawing.Size(190, 60),
                    Text = item.Value,
                });

                if(first) neww.Checked = media.MediaPlayer.AudioTrack == -1;
                else if(media.MediaPlayer.AudioTrack != -1) neww.Checked = item.Value == media.MediaPlayer.AudioTrackDescription[media.MediaPlayer.AudioTrack - others + 1].Name;

                neww.Click += (o, e) => {
                    foreach (var audio in media.MediaPlayer.AudioTrackDescription)
                    {
                        if (neww.Name == audio.Name)
                        {
                            media.MediaPlayer.SetAudioTrack(audio.Id);
                        }
                    }
                    Program.win.mediaPanel.toolbar.RefreshMenuAudio();
                };
                first = false;
            }
            return listMenuAudio.ToArray();
        }
        public ToolStripMenuItem[] GetSubToolMenu()
        {
            int others = media.MediaPlayer.VideoTrackCount + media.MediaPlayer.AudioTrackCount - 2;
            if (media == null) return null;
            List<ToolStripMenuItem> listMenuSub = new List<ToolStripMenuItem>();

            Dictionary<int, string> subTrack = new Dictionary<int, string>();
            foreach (var sub in media.MediaPlayer.SpuDescription) if(sub.Id != -1) subTrack.Add(sub.Id - others, sub.Name); else subTrack.Add(sub.Id, sub.Name);

            bool first = true;
            foreach (var item in subTrack)
            {
                ToolStripMenuItem neww;
                listMenuSub.Add(neww = new ToolStripMenuItem()
                {
                    BackColor = System.Drawing.Color.Black,
                    CheckOnClick = true,
                    ForeColor = System.Drawing.Color.White,
                    Name = item.Value,
                    Size = new System.Drawing.Size(190, 60),
                    Text = item.Value,
                });

                if (first) neww.Checked = media.MediaPlayer.Spu == -1;
                else if (media.MediaPlayer.Spu != -1) neww.Checked = item.Value == media.MediaPlayer.SpuDescription[media.MediaPlayer.Spu - others + 1].Name;

                neww.Click += (o, e) => {
                    foreach (var sub in media.MediaPlayer.SpuDescription)
                    {
                        if (neww.Name == sub.Name)
                        {
                            media.MediaPlayer.SetSpu(sub.Id);
                        }
                    }
                    Program.win.mediaPanel.toolbar.RefreshMenuSub();
                };
                first = false;
            }
            return listMenuSub.ToArray();
        }

        public void SetBounds(Rectangle rect)
        {
            if (media == null) return;
            Bounds = rect;
        }

        public void Stop()
        {
            if (media == null) return;
            Console.WriteLine("Media Stopped");
            System.Threading.Thread stopThread = new System.Threading.Thread(STOP);
            stopThread.Start();
        }
        private void STOP()
        {
            try
            {
                media.MediaPlayer.Pause();
                System.Threading.Thread.Sleep(1000);
                media = null;
                GC.Collect();
                media.MediaPlayer.Stop();
            }
            catch (Exception) { }
        }

        public void GetMedia()
        {
            if (media == null) return;
            Console.WriteLine();
            Console.WriteLine("Get Media");
            Console.WriteLine();
        }
        public bool GetFlag(bool restore = false)
        {
            if (restore) { flag = false; return flag; }
            return flag;
        }

        bool timeToLoad = false;
        public void TimingLoad(Object state)
        {
            if (timeToLoad)
            {
                Console.WriteLine("MEDIA PLAY: "+actPath);
                timeToLoad = false;
                EffectivelyLoad();
            }
            TimerLoad.Change(500, System.Threading.Timeout.Infinite);
        }

        string actPath;
        public bool activate = false;
        public void LoadFilm(string path)
        {
            activate = false;
            if (medias.Count == 0) NewMedia();
            actPath = path;
            timeToLoad = true;
        }
        public void EffectivelyLoad()
        {
            //if (!Program.VLC_Installed) return;

            //Program.win.mediaPanel.ToolBarShow(true);
            LibVLCSharp.Shared.Media act_media = new LibVLCSharp.Shared.Media(lib, actPath);
            media.MediaPlayer.Play(act_media);
            flag = true;
            //System.Threading.Thread.Sleep(500);
            Program.win.mediaPanel.toolbar.RefreshForm();
            System.Threading.Thread.Sleep(500);
            activate = true;
            Program.win.mediaPanel.toolbar.RefreshForm();
            for(int i=0; i<10; i++)
            {
                System.Threading.Thread.Sleep(500);
                if (Program.win.mediaPanel.toolbar.toolMenuAudio.DropDownItems.Count == 0) Program.win.mediaPanel.toolbar.RefreshForm();
                else break;
            }
        }
        public bool IsReady()
        {
            if (medias[0] == null) {  return false; }
            return activate;
        }
        public void OnEndReached()
        {
            if (media == null) return;
            if (Program.win.mediaPanel.numFilm == MediaPanel.PlayList.Count - 1)
            {
               // media.Position = 0;
                Program.win.mediaPanel.UpdateTime();
                Pause();
                Program.win.mediaPanel.toolbar.play.BackgroundImage = Properties.Resources.play;
            }
            else
            {
                Program.win.mediaPanel.numFilm++;
                Program.win.mediaPanel.LoadFilm(Program.win.mediaPanel.numFilm + 1);
            }
        }

        LibVLCSharp.Shared.LibVLC lib = null;
        private void NewMedia()
        {
            if (medias.Count > 0) return;
            //if (medias.Count > 0) { medias.Clear(); }
            medias.Add(new LibVLCSharp.WinForms.VideoView());
            medias[medias.Count - 1] = new LibVLCSharp.WinForms.VideoView();
            ((System.ComponentModel.ISupportInitialize)(medias[medias.Count - 1])).BeginInit();
            medias[medias.Count - 1].BackColor = System.Drawing.Color.Black;
            medias[medias.Count - 1].Location = new System.Drawing.Point(501, 235);
            medias[medias.Count - 1].Name = "vlcControl1";
            medias[medias.Count - 1].Size = new System.Drawing.Size(75, 23);
            medias[medias.Count - 1].TabIndex = 3;
            medias[medias.Count - 1].Text = "vlcControl1";
            medias[medias.Count - 1].Dock = DockStyle.Fill;
            string libPath = "";
            //if (Directory.Exists(@"C:\Program Files\VideoLAN\VLC")) libPath = @"C:\Program Files\VideoLAN\VLC";
            //else 
            if (Directory.Exists(@"C:\Program Files (x86)\VideoLAN\VLC")) libPath = @"C:\Program Files (x86)\VideoLAN\VLC";
            
            Controls.Add(medias[medias.Count - 1]);
            LibVLCSharp.Shared.Core.Initialize(libPath);
            lib = new LibVLCSharp.Shared.LibVLC();
            medias[medias.Count - 1].MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(lib);
            ((System.ComponentModel.ISupportInitialize)(medias[medias.Count - 1])).EndInit();
            medias[medias.Count - 1].MediaPlayer.SetSpu(-1);
            medias[medias.Count - 1].MediaPlayer.EncounteredError += (o, e) => { Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine("ERROR"); Console.WriteLine(); };
            medias[medias.Count - 1].MediaPlayer.Playing += (o, e) => { MediaPanel.active = true; };
            medias[medias.Count - 1].MediaPlayer.Paused += (o, e) => { MediaPanel.active = false; };
            medias[medias.Count - 1].MediaPlayer.Stopped += (o, e) => { MediaPanel.active = false; };
            medias[medias.Count - 1].MediaPlayer.Playing += (o, e) => { flag = true; };
            medias[medias.Count - 1].MediaPlayer.Stopped += (o, e) => { flag = true; };
            medias[medias.Count - 1].MediaPlayer.Paused += (o, e) => { flag = true; };
            medias[medias.Count - 1].MediaPlayer.TimeChanged += (o, e) => { Program.win.mediaPanel.UpdateTime(); if (media.MediaPlayer.Length - media.MediaPlayer.Time <= 1000) OnEndReached(); 
            };

            medias[medias.Count - 1].MediaPlayer.Volume = 150;
            media = medias[medias.Count - 1];

            Console.WriteLine("______________________________________________________________________");
            Console.WriteLine("");
            Console.WriteLine("______________________________________________________________________");
            Console.WriteLine("New MEDIA");
            lib.Log += (o, e) => { Console.WriteLine("------> "+ e.Message); };
        }
    }
}
