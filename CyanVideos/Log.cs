using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class Log
    {
        public string filename = "";
        public float position = 0;
        public long data = 0;
        public string parent = "";
        private int index = -2;
        private bool to_delete = false;
        public List<string> playlist = new List<string>();
        public List<string> parentFolders = new List<string>();
        public Log(string filename, float position, long data, List<string> playlist)
        {
            this.filename = filename;
            this.position = position;
            this.data = data;
            if (playlist != null)
            {
                foreach (string play in playlist) this.playlist.Add(play);

                if (playlist.Count > 1) for (int i = playlist.Count - 1; i >= 0; i -= 2)
                    {if (!File.Exists(playlist[i - 1])) { playlist.RemoveAt(i); playlist.RemoveAt(i - 1); } }
                for (int i = 0; i < playlist.Count; i+=2)
                {
                    if (playlist[i] == filename) { index = i/2; break; }
                }
                if(index == -2)
                {
                    if (playlist.Count == 0) filename = "";
                    else { filename = playlist[0]; index = 0; }
                }
            }
            else { Console.WriteLine("PLAYLIST NULL"); }
            FindParents();
            Smooth();
        }

        public void Smooth()
        {
            if (position < 0.05) position = 0;
            if ((index == 0 || index == -2) && position == 0)
            {
                playlist = null;
            }
            else if (playlist != null && playlist.Count / 2 > 1 && position > 0.95)
            {
                if (playlist.Count == 2 * index +2) { playlist = null; return; }
                filename = playlist[2 * index + 2];
                position = 0;
            }
        }

        public void FindParents()
        {
            if (playlist.Count > 0)
            {
                string parent = Directory.GetParent(playlist[0]).FullName;
                while (parent.Length > 2)
                {
                    parentFolders.Add(parent);
                    DirectoryInfo info = Directory.GetParent(parent);
                    if (info != null) parent = info.FullName;
                    else { parent = ""; break; }
                }
            }
            for(int i= parentFolders.Count-1; i>=0; i--)
            {
                bool all = true;
                for (int j = 0; j < playlist.Count; j += 2)
                {
                    if (playlist[j].Substring(0, parentFolders[i].Length) != parentFolders[i]) { all = false; break; }
                }
                if (!all) parentFolders.RemoveAt(i);
            }
            this.parent = parentFolders[0];
        }

        public string serialize()
        {
            string output = "";
            output += filename + "|^|";
            output += position + "|^|";
            output += data + "|^|";
            foreach(string file in playlist)
            {
                output += file + "|^|";
            }
            return output;
        }

        public static Log Load(string serialized)
        {
            try
            {
                string[] input = serialized.Split(new string[] { "|^|" }, StringSplitOptions.RemoveEmptyEntries);
                string filename = input[0];
                float position = (float)Convert.ToDouble(input[1]);
                long data = Convert.ToInt64(input[2]);
                List<string> playlist = new List<string>();
                if (input.Length > 2) for (int i = 3; i < input.Length; i++) playlist.Add(input[i]);
                Log finalLog = new Log(filename, position, data, playlist);
                if (finalLog.filename == "") return null;
                return finalLog;
            }
            catch (Exception e) { Console.WriteLine("EXCEPTION in LOAD: "+e.Message); }
            return null;
        }

        public static Log FindByParent(List<Log> list, string parent)
        {
            Log toReturn = null;
            int i = list.Count - 1;
            for (i=list.Count-1; i>=0; i--)
            {
                if (list[i].filename.Length >= parent.Length && list[i].filename.Substring(0, parent.Length) == parent)
                    if (list[i].playlist != null) { toReturn = list[i]; break; }
            }
            for (int j = i-1; j >= 0; j--)
            {
                if (list[j].filename.Length >= parent.Length && list[j].filename.Substring(0, parent.Length) == parent)
                    { list[j].to_delete = true;}
            }
            return toReturn;
        }

        public static void ClearLogs(List<Log> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null || list[i].playlist == null || list[i].to_delete) list.RemoveAt(i);
            }
        }

        public void Print()
        {
            Console.WriteLine(filename + "   " + position);
        }


        bool allowClick = true;
        private System.Windows.Forms.Timer pauseFromClick = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
        public void ClickEvent(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right) return;
            else if (allowClick)
            {
                pauseFromClick.Enabled = true;
                pauseFromClick.Tick += (o, exa) => { allowClick = true; pauseFromClick.Enabled = false; };
                Continue();
                allowClick = false;
            }
            return;
        }
        private void Continue()
        {
            MediaPanel.PlayList = new List<Vision>();
            for (int i = 0; i < playlist.Count; i += 2) MediaPanel.PlayList.Add(new Vision(playlist[i], playlist[i + 1]));
            //foreach (string file in playlist) { MediaPanel.PlayList.Add(new Vision(file, Path.GetFileNameWithoutExtension(file)));  }

            for (int i = 0; i < MediaPanel.PlayList.Count; i++)
                if (MediaPanel.PlayList[i].filename == filename)
                {
                    Program.win.mediaPanel.LoadFilm(i, true, position);
                    Program.win.mediaPanel.ToFront();
                }
        }
    }
}
