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
using System.Windows.Threading;
using AXVLC;

namespace CyanVideos
{
    public class Player : Panel
    {
        int minimumInterval = 500;
        public List<Log> logs = new List<Log>();
        public bool logFlag = false;
        private Vlc.DotNet.Forms.VlcControl media;
        public List<Vlc.DotNet.Forms.VlcControl> medias = new List<Vlc.DotNet.Forms.VlcControl>();
        
        public Player()
        {
            return;
            // System.Threading.Thread newThread = new System.Threading.Thread(NEWMEDIA);
            // newThread.Start();
        }

        private void NEWMEDIA()
        {
            if (Program.VLC_Installed) try { NewMedia(); } catch (Exception e) { Console.WriteLine("Exception from PlayerX: " + e.Message); }
        }

        public double InputPosition()
        {
            if (media == null) return 0;
            try { return (double)media.VlcMediaPlayer.Position; } catch (Exception) { return 0; }
        }
        
        public void SetInputPosition(double position)
        {
            float actual_position = (float)position;
            if (media == null || !activate) return;
            try
            {
                media.VlcMediaPlayer.Position = actual_position;
            }
            catch (Exception) { return; }
        }
        public long InputTime()
        {
            if (media == null) return 0;
            try
            {
                return media.VlcMediaPlayer.Time;
            }
            catch (Exception) { return 0; }
        }
        public double InputLength()
        {
            if (media == null) return 0;
            try
            {
                return media.VlcMediaPlayer.Length - 1000;
            }
            catch (Exception) { return 0; }
        }
        public void Play()
        {
            if (media == null) return;
            Console.WriteLine("PLAY");
            media.Play();
        }
        public void Pause()
        {
            if (media == null) return;
            try
            {
                CreateLog();
                media.Pause();
            }
            catch (Exception) { }
        }
        public void SetVisible(bool visible)
        {
            if (media == null) return;
            Visible = visible;
        }
        public bool IsPlaying()
        {
            if (media == null) return false;
            return media.IsPlaying;
        }
        public void SetAudioTrack(int track)
        {
            if (media == null) return;
            media.Audio.Tracks.Current = media.Audio.Tracks.All.ToArray()[track];
        }
        public int AudioTrack()
        {
            if (media == null) return 0;
            return media.Audio.Tracks.Current.ID;
        }
        public void SetSubTrack(int track)
        {
            if (media == null) return;
            media.SubTitles.Current = media.SubTitles.All.ToArray()[track];
        }
        public int SubTrack()
        {
            if (media == null) return 0;
            return media.SubTitles.Current.ID;
        }
        public string AudioDescription(int i)
        {
            if (media == null) return "";
            return media.Audio.Tracks.All.ToArray()[i].Name;
        }
        public string SubDescription(int i)
        {
            if (media == null) return "";
            return media.SubTitles.All.ToArray()[i].Name;
        }
        public int AudioCount()
        {
            if (media == null) return 0;
            return media.Audio.Tracks.Count;
        }
        public int SubCount()
        {
            if (media == null) return 0;
            return media.SubTitles.Count;
        }
        public void Forward()
        {
            if (media == null) return;
            float step = (float)((double)1 / (double)(media.Length / 59500)); //1min
            if ( (double)(step*118)/60 + media.Position >= 1) { OnEndReached(); return; }
            media.Position += step;
            Program.win.mediaPanel.UpdateTime();
        }
        public void Back()
        {
            if (media == null) return;
            float step = (float)((double)1 / (double)(media.Length / 59500)); //1min
            if (media.Position - step< 0) {media.Position = 0; return; }
            media.Position -= step;
            Program.win.mediaPanel.UpdateTime();
        }
        public void SetBounds(Rectangle rect)
        {
            if (media == null) return;
            Bounds = rect;
        }

        public ToolStripMenuItem[] GetAudioToolMenu()
        {
            try
            {
                if (media == null) return null;
                List<ToolStripMenuItem> listMenuAudio = new List<ToolStripMenuItem>();

                Dictionary<int, string> audioTrack = new Dictionary<int, string>();
<<<<<<< HEAD
                foreach (var audio in media.Audio.Tracks.All)
                {
                    if (audio.ID != -1) audioTrack.Add(audio.ID, audio.Name);
=======
                List<string> audioTracks = new List<string>();
                foreach (var audio in media.Audio.Tracks.All)
                {
                    if (audio.ID != -1 && !audioTracks.Contains(audio.Name)) audioTrack.Add(audio.ID, audio.Name);
                    audioTracks.Add(audio.Name);
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
                }
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
                    neww.Checked = item.Value == media.Audio.Tracks.Current.Name;

                    neww.Click += (o, e) =>
                    {
                        foreach (var audio in media.Audio.Tracks.All)
                        {
                            if (neww.Name == audio.Name)
                            {
                                media.Audio.Tracks.Current = audio;
                            }
                        }
                        Program.win.mediaPanel.toolbar.RefreshMenuAudio();
                    };
                }
                return listMenuAudio.ToArray();
            }
            catch (Exception) { return null; }
        }
        public ToolStripMenuItem[] GetSubToolMenu()
        {
            try
            {
                if (media == null) return null;
                List<ToolStripMenuItem> listMenuSub = new List<ToolStripMenuItem>();

                Dictionary<int, string> subTrack = new Dictionary<int, string>();
                foreach (var sub in media.SubTitles.All) subTrack.Add(sub.ID, sub.Name);

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

                    if (actSub != "") neww.Checked = false;
                    else neww.Checked = item.Value == media.SubTitles.Current.Name;

                    neww.Click += (o, e) =>
                    {
                        foreach (var sub in media.SubTitles.All)
                        {
                            if (neww.Name == sub.Name)
                            {
                                media.SubTitles.Current = sub;
                            }
                        }
                        actSub = "";
                        RefreshForm();
                    };
                }
                return listMenuSub.ToArray();
            }
            catch (Exception) { return null; }
        }

        string actSub = "";
        public ToolStripMenuItem[] GetSubRestMenu()
        {
            // return null;
            if (media == null) return null;
            List<ToolStripMenuItem> listMenuSub = new List<ToolStripMenuItem>();

            string[] subs = Program.GetAllSubs(Directory.GetParent(actPath).FullName);
            Dictionary<string, string> subTrack = new Dictionary<string, string>();
            foreach (var sub in subs)
            {
                string text = Path.GetFileNameWithoutExtension(sub).Substring(Path.GetFileNameWithoutExtension(sub).LastIndexOf(".") + 1);
                subTrack.Add(sub, text);
            }


            foreach (var item in subTrack)
            {
                ToolStripMenuItem neww;
                listMenuSub.Add(neww = new ToolStripMenuItem()
                {
                    BackColor = System.Drawing.Color.Black,
                    CheckOnClick = true,
                    ForeColor = System.Drawing.Color.White,
                    Name = item.Key,
                    Size = new System.Drawing.Size(190, 60),
                    Text = item.Value,
                });

                neww.Checked = item.Key == actSub;

                neww.Click += (o, e) => {
                    float position = media.Position;
                    string[] opts = { @"sub-file=" + neww.Name };

                    if (actSub == neww.Name)
                    {
                        medias[medias.Count - 1].SetMedia(new FileInfo(actPath));
                        actSub = "";
                    }
                    else
                    {
                        medias[medias.Count - 1].SetMedia(new FileInfo(actPath), opts);
                        actSub = neww.Name;
                    }

                    medias[medias.Count - 1].Play();
                    // System.Threading.Thread.Sleep(100);
                    media.Position = position;
                    RefreshForm();
                };
            }
            return listMenuSub.ToArray();
        }


        public void Stop()
        {
            if (media == null) return;
            CreateLog();
            //try { this.Invoke(new Action(() => ClearBuffer())); } catch (Exception) { }
            ClearBuffer();
            media = null;
            Console.WriteLine("Media Stopped");
        }

        private void CreateLog()
        {
            Console.WriteLine(" -- Creating log --");
            foreach (Log log in logs) { if (log.filename == actPath) { logs.Remove(log); break; } }
            float position = media.Position;
            if (position == -1) position = 0;
            logs.Add(new Log(actPath, position, DateTime.UtcNow.ToBinary(), new List<string>() { actPath }));
            logFlag = true;
        }

        List<Vlc.DotNet.Forms.VlcControl> buffer = new List<Vlc.DotNet.Forms.VlcControl>();
        private void ClearBuffer()
        {
            foreach (var Media in medias)
            {
                Media.VlcMediaPlayer.EncounteredError -= EncounteredError;
                Media.VlcMediaPlayer.Playing -= ActiveTrue;
                Media.VlcMediaPlayer.Paused -= ActiveFalse;
                Media.VlcMediaPlayer.Stopped -= ActiveFalse;
                Media.VlcMediaPlayer.TimeChanged -= TimeChanged;
                Media.VlcMediaPlayer.Playing -= Flag;
                Media.VlcMediaPlayer.Stopped -= Flag;
                Media.VlcMediaPlayer.Paused -= Flag;
                Media.Pause();
                Media.Visible = false;

                try { this.Invoke(new Action(() => {Controls.Remove(Media); })); }
                catch (Exception) { }

                
                if (!buffer.Contains(Media)) buffer.Add(Media); 

                System.Threading.Thread clearThread = new System.Threading.Thread(CLEAR);
                clearThread.Start();
            }
            medias.Clear();
        }
        

        bool clear_active = false;
        List<Vlc.DotNet.Forms.VlcControl> active_buffer = new List<Vlc.DotNet.Forms.VlcControl>();
        private void CLEAR()
        {
            if (clear_active) return;
            int delay = 1000;
            while (buffer.Count>0)
            {
                foreach (var Media in buffer) active_buffer.Add(Media);
                buffer.Clear();
                clear_active = true;
                System.Threading.Thread.Sleep(delay);
                foreach (var Media in active_buffer)
                {
                    Media.Dispose();
                    Console.WriteLine("Gently disposing VLC Control");
                    System.Threading.Thread.Sleep(delay);
                }
                active_buffer.Clear();
            }
            GC.Collect();
            clear_active = false;
        }
        public void GetMedia()
        {
            if (media == null) return;
            Console.WriteLine();
            Console.WriteLine("Get Media");
            Console.WriteLine();
        }
        

        string actPath;
        public bool activate = false;
        private float pre_position = 0;
        public void LoadFilm(string path, float position = 0)
        {
            if (buffering) return;
            Console.WriteLine("Loading: "+path);
            //timerstop = true;
            //counter = 0;
            //Stop();
            pre_position = position;
            actPath = path;
            activate = false;
            try { this.Invoke(new Action(() => EffectivelyLoad())); } catch (Exception) { }
        }

        public void EffectivelyLoad()
        {
            if (!Program.VLC_Installed) return;
            Console.WriteLine("Effectively Loaded");
            System.Threading.Thread playThread = new System.Threading.Thread(PLAY);
            playThread.Start();
            //Program.win.mediaPanel.ToolBarShow(true);
            flag = true;
        }
        private void PLAY()
        {
            actSub = "";
            ClearBuffer();
            NewMedia();
            media.SetMedia(new FileInfo(actPath), new string[] { "sub-autodetect-file" });
            media.Play();
            media.Position = pre_position;
            try { this.Invoke(new Action(() => RefreshForm())); } catch (Exception) { }
            
        }

        private void RefreshForm()
        {
            for (int i = 0; i < 10; i++)
            {
                Program.win.mediaPanel.toolbar.RefreshForm();
                if (Program.win.mediaPanel.toolbar.toolMenuAudio.DropDownItems.Count != 0)
                {
                    Program.win.mediaPanel.toolbar.RefreshForm(true);
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            System.Threading.Thread.Sleep(100);
            activate = true;
        }
        bool flag = false;
        public bool GetFlag(bool restore = false)
        {
            if (restore) { flag = false; return flag; }
            return flag;
        }
        public bool IsReady()
        {
            if (media == null) { return false; }
            if (buffering) return false;
            return activate;
        }
        public void OnEndReached()
        {
            if (media == null) return;
            Program.win.mediaPanel.ToolBarShow(false, true);
            //try { this.Invoke(new Action(() =>
            {
                if (Program.win.mediaPanel.numFilm == MediaPanel.PlayList.Count - 1)
                {
                    media.Position = 0;
                    Program.win.mediaPanel.UpdateTime();
                    Pause();
                    Program.win.mediaPanel.toolbar.play.BackgroundImage = Properties.Resources.play;
                }
                else
                {
                    //Console.WriteLine("Playlist position: "+Program.win.mediaPanel.numFilm);
                    if (!activate) return;
                    CreateLog();
                    Program.win.mediaPanel.LoadFilm(Program.win.mediaPanel.numFilm + 1);
                    Program.win.mediaPanel.numFilm++;
                    activate = false;
                }
           // }));
            }// catch (Exception) { }
        }

        private void NewMedia()
        {
            //if (!Directory.Exists(Environment.CurrentDirectory + "/" + Program.libvlc)) return;

            medias.Add(new Vlc.DotNet.Forms.VlcControl());
            medias[medias.Count - 1].BeginInit();
            medias[medias.Count - 1].BackColor = System.Drawing.Color.Black;
            medias[medias.Count - 1].Location = new System.Drawing.Point(501, 235);
            medias[medias.Count - 1].Name = "vlcControl1";
            medias[medias.Count - 1].Size = new System.Drawing.Size(75, 23);
            medias[medias.Count - 1].Spu = -1;
            medias[medias.Count - 1].TabIndex = 3;
            medias[medias.Count - 1].Text = "vlcControl1";
            medias[medias.Count - 1].Dock = DockStyle.Fill;
            if (Program.libvlc != "") { medias[medias.Count - 1].VlcLibDirectory = new DirectoryInfo(Program.libvlc);}
            else medias[medias.Count - 1].VlcLibDirectory = null;
            medias[medias.Count - 1].VlcMediaplayerOptions = null;
            try { if(Program.enabledToSave) this.Invoke(new Action(() => Controls.Add(medias[medias.Count - 1]))); } catch (Exception) { }
            
            medias[medias.Count - 1].EndInit();
            medias[medias.Count - 1].VlcMediaPlayer.EncounteredError += EncounteredError;
            medias[medias.Count - 1].VlcMediaPlayer.Playing += ActiveTrue;
            medias[medias.Count - 1].VlcMediaPlayer.Paused += ActiveFalse;
            medias[medias.Count - 1].VlcMediaPlayer.Stopped += ActiveFalse;
            medias[medias.Count - 1].VlcMediaPlayer.TimeChanged += TimeChanged;
            medias[medias.Count - 1].VlcMediaPlayer.Buffering += Buffering;

            medias[medias.Count - 1].VlcMediaPlayer.Playing += Flag;
            medias[medias.Count - 1].VlcMediaPlayer.Stopped += Flag;
            medias[medias.Count - 1].VlcMediaPlayer.Paused += Flag;

            medias[medias.Count - 1].Audio.Volume = 150;
            media = medias[medias.Count - 1];
        }

        private void EncounteredError(object Sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine("ERROR: "+ e.ToString()); Console.WriteLine();
        }
        bool buffering = false;
        private void WAIT() { System.Threading.Thread.Sleep(minimumInterval); buffering = false; }
        public bool IsBuffering()
        {
            return buffering;
        }
        Timer timerBuffering;
        private void Buffering(object Sender, VlcMediaPlayerBufferingEventArgs e)
        {
            if (e.NewCache == 100)
            {
                System.Threading.Thread waitThread = new System.Threading.Thread(WAIT);
                waitThread.Start();
                activate = true;
            }
            else { activate = false; buffering = true;
                if(timerBuffering!= null) timerBuffering.Dispose();
                timerBuffering = new Timer() { Enabled = true, Interval = 4000 };
                timerBuffering.Tick += (o, ee) => { buffering = false; timerBuffering.Dispose(); };
            }
        }
        private void ActiveTrue(object Sender, EventArgs e)
        {
            MediaPanel.active = true;
        }
        private void ActiveFalse(object Sender, EventArgs e)
        {
            MediaPanel.active = false;
        }
        private void TimeChanged(object Sender, EventArgs e)
        {
            Program.win.mediaPanel.UpdateTime(); if (media != null && media.Length - media.Time <= 1000) OnEndReached();
        }
        private void Flag(object Sender, EventArgs e)
        {
            flag = true;
        }
    }
}
