using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public partial class Player1 : UserControl
    {
        //public AxAXVLC.AxVLCPlugin2 media;
        //public List<AxAXVLC.AxVLCPlugin2> medias = new List<AxAXVLC.AxVLCPlugin2>();

        public Player1()
        {
            InitializeComponent();
            NewMedia();
        }
        
        public double InputPosition()
        {
            return 0;
            //return media.input.Position;
        }
        public void SetInputPosition(double position)
        {
            //media.input.Position = position;
        }
        public double InputTime()
        {
            return 0;
            //return media.input.Time;
        }
        public double InputLength()
        {
            return 0;
            //return media.input.Length;
        }
        public void Play()
        {
            //media.playlist.play();
        }
        public void Pause()
        {
            //media.playlist.pause();
        }
        public void SetVisible(bool visible)
        {
           // media.Visible = visible;
        }
        public bool IsPlaying()
        {
            return false;
            //return media.playlist.isPlaying;
        }
        public void SetAudioTrack(int track)
        {
            //media.audio.track = track;
        }
        public int AudioTrack()
        {
            return 0;
            // return media.audio.track;
        }
        public void SetSubTrack(int track)
        {
           // media.subtitle.track = track;
        }
        public int SubTrack()
        {
            return 0;
            // return media.subtitle.track;
        }
        public string AudioDescription(int i)
        {
            return "";
            // return media.audio.description(i);
        }
        public string SubDescription(int i)
        {
            return "";
            //return media.subtitle.description(i);
        }
        public int AudioCount()
        {
            return 0;
            // return media.audio.count;
        }
        public int SubCount()
        {
            return 0;
           // return media.subtitle.count;
        }

        public ToolStripMenuItem[] GetAudioToolMenu()
        {
            List<ToolStripMenuItem> listMenuAudio = new List<ToolStripMenuItem>();
            for (int i = 0; i < AudioCount(); i++)
            {
                ToolStripMenuItem neww;
                listMenuAudio.Add(neww = new ToolStripMenuItem()
                {
                    BackColor = Color.Black,
                    CheckOnClick = true,
                    ForeColor = System.Drawing.Color.White,
                    Name = AudioDescription(i),
                    Size = new System.Drawing.Size(190, 60),
                    Text = AudioDescription(i),
                });

                neww.Checked = i == AudioTrack();

                neww.Click += (o, e) => {
                    for (int j = 0; j < AudioCount(); j++)
                    {
                        if (AudioDescription(j) == neww.Name)
                        {
                            Console.WriteLine("FOUND -------->" + j);
                            //if (j == 0) Program.win.mediaPanel.Media.SetAudioTrack(-1);
                            //else
                            {
                                SetAudioTrack(j);
                                Console.WriteLine(AudioDescription(j));
                                Console.WriteLine("Selected audio Track: " + AudioTrack());
                                Console.WriteLine("Audio Count: " + AudioCount());
                            }
                        }
                    }
                    Program.win.mediaPanel.toolbar.RefreshMenuAudio();
                };
            }
            return listMenuAudio.ToArray();
        }
        public ToolStripMenuItem[] GetSubToolMenu()
        {
            List<ToolStripMenuItem> listMenuSub = new List<ToolStripMenuItem>();
            for (int i = 0; i < SubCount(); i++)
            {
                ToolStripMenuItem neww;
                listMenuSub.Add(neww = new ToolStripMenuItem()
                {
                    BackColor = Color.Black,
                    CheckOnClick = true,
                    ForeColor = System.Drawing.Color.White,
                    Name = SubDescription(i),
                    Size = new System.Drawing.Size(190, 60),
                    Text = SubDescription(i),
                });
                if (i == 0) neww.Checked = SubTrack() == -1;
                else neww.Checked = i == SubTrack() - AudioCount() + 1;

                neww.Click += (o, e) => {
                    for (int j = 0; j < SubCount(); j++)
                    {
                        if (SubDescription(j) == neww.Name)
                        {
                            Console.WriteLine("FOUND -------->" + j);
                           // if (j == 0) Program.win.mediaPanel.Media.SetSubTrack(-1);
                            //else
                            {
                                SetSubTrack(j + AudioCount() - 1);
                                Console.WriteLine(SubDescription(j));
                                Console.WriteLine("Selected subtitle Track: " + SubTrack());
                                Console.WriteLine("Audio Count: " + AudioCount());
                            }
                        }
                    }
                    Program.win.mediaPanel.toolbar.RefreshMenuSub();
                };
            }
            return listMenuSub.ToArray();
        }

        public void SetBounds(Rectangle rect)
        {
            Bounds = rect;
        }

        public void Stop()
        {
            //medias[medias.Count - 1].playlist.togglePause();
            NewMedia();
            GetMedia();
        }

        public void GetMedia()
        {
           // media = medias[medias.Count - 1];

            Console.WriteLine();
            Console.WriteLine("Get Media");
            Console.WriteLine();
            
           // medias[medias.Count - 1].MediaPlayerPlaying += (o, e) => { MediaPanel.active = true; };
           // medias[medias.Count - 1].MediaPlayerPaused += (o, e) => { MediaPanel.active = false; };
            //medias[medias.Count - 1].MediaPlayerStopped += (o, e) => { MediaPanel.active = false; };
            //medias[medias.Count - 1].MediaPlayerTimeChanged += (o, e) => { Program.win.mediaPanel.UpdateTime(); };
            //medias[medias.Count - 1].MediaPlayerEndReached += (o, e) => 
            {
                if (Program.win.mediaPanel.numFilm == MediaPanel.PlayList.Count - 1) { return; }
                else { Program.win.mediaPanel.LoadFilm(Program.win.mediaPanel.numFilm + 1); Program.win.mediaPanel.numFilm++; }
            };
            //
           // medias[medias.Count - 1].Volume = 150;
           // medias[medias.Count - 1].Toolbar = false;
           // medias[medias.Count - 1].FullscreenEnabled = false;
        }

        public void LoadFilm(string path)
        {
            //medias[medias.Count - 1].playlist.items.clear();
            //medias[medias.Count - 1].playlist.add("file:///" + path, null);
            //medias[medias.Count - 1].playlist.play();

        }
        
        private void NewMedia()
        {
            /*
            if (medias.Count > 0) medias[medias.Count - 1].Dispose();
            ComponentResourceManager resources2 = new ComponentResourceManager(typeof(Player1));
            medias.Add(new AxAXVLC.AxVLCPlugin2());
            medias[medias.Count - 1] = new AxAXVLC.AxVLCPlugin2();
            ((ISupportInitialize)(medias[medias.Count - 1])).BeginInit();

            medias[medias.Count - 1].Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            medias[medias.Count - 1].Enabled = true;
            medias[medias.Count - 1].Dock = DockStyle.Fill;
            medias[medias.Count - 1].Location = new Point(0, 0);
            medias[medias.Count - 1].Name = "media";
            medias[medias.Count - 1].OcxState = (AxHost.State)(resources2.GetObject("media.OcxState"));
            medias[medias.Count - 1].Size = new System.Drawing.Size(1031, 610);
            medias[medias.Count - 1].TabIndex = 0;
            medias[medias.Count - 1].Visible = true;
            Controls.Add(medias[medias.Count - 1]);
            ((ISupportInitialize)(medias[medias.Count - 1])).EndInit();
            */
        }
    }
}
