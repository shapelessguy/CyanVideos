using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyanVideos
{
    public class Source
    {
        public static bool verbose = false;
        private List<Iconxx> icons = new List<Iconxx>();
        public SourceTag tag;
        public string directory;
        public bool series;
        public bool Null = true;
        public void Dispose(bool disposeIcons = true)
        {
            if (verbose) Console.WriteLine("Disposing source: " + directory);
            if (icons.Count>0 && disposeIcons) foreach (Iconxx icon in icons) if(icon != null) icon.Dispose();
            if(tag != null) tag.Disposer();
            GC.Collect();
        }
        public Source(string directory, string name, bool series = false, bool extremis_series = false, bool in_first_panel = false)
        {
            if (directory == "" && name == "") return;
            Null = false;
            this.directory = directory;
            if (!series) this.series = Is_Serie(directory);
            else this.series = true;
            try {
                //if (!Directory.Exists(directory)) return;
                string[] directories = Directory.GetDirectories(directory);
                if (extremis_series) directories = Program.GetAllVideos(Directory.GetFiles(directory));
                foreach (string dir in Ordering.OrderAlphanumeric(directories))
                {
                    icons.Add(new Iconxx(directory, dir, this.series, 0, extremis_series, in_first_panel));
                }
            }
            catch (Exception) { Console.WriteLine("Exception here"); }

            string name_empty = name;
            if (icons.Count == 0) name_empty += " (empty)";
            tag = new SourceTag(directory, name_empty);
            if (verbose) Console.WriteLine("Source" + (this.series == true ? ("(Serie)") : "") + ": " + directory + " : " + name + "  has been added");
            //CopyRoot();
        }
        public Source(string directory, string name, List<Iconxx> icons)
        {
            if (verbose) Console.WriteLine("Source: " + directory + " : " + name + "  has been added");
            this.directory = directory;
            series = false;
            try
            {
                foreach (Iconxx icon in icons)
                {
                    this.icons.Add(icon);
                }
            }
            catch (Exception) { }

            string name_empty = name;
            if (icons.Count == 0) name_empty += " (empty)";
            tag = new SourceTag(directory, name_empty);
        }
        public void ClickPrevious(string directory, bool series)
        {
            Iconxx.ClickIconxx(Directory.GetParent(directory).FullName, series);
        }
        public List<Iconxx> Icons()
        {
            return icons;
        }

        public static bool Is_Serie(string dir)
        {
            if (Directory.GetFiles(dir).Contains(dir + @"\powervideos_series.txt")) { return true; }
            else return false;
        }

        private void CopyRoot()
        {
            string destpath = @"C:\Users\Claudio\Desktop" + @"\" + Path.GetFileName(directory);
            Directory.CreateDirectory(destpath);
            GetFilesAndDirectories(directory, destpath);

        }
        private string[] GetFilesAndDirectories(string directory, string destpath)
        {
            GetFiles(directory, destpath);
            return GetDirectories(directory, destpath);
        }
        private void GetFiles(string frompath, string destpath)
        {
            //Console.WriteLine(frompath);
            foreach (string file in Directory.GetFiles(frompath))
            {
                bool video = false;
                foreach (string pun in Program.videoExtensions) if (file.ToLower().Contains(pun)) video = true;
                if (!video) File.Copy(file, destpath + @"\" + Path.GetFileName(file));
            }
        }
        private string[] GetDirectories(string frompath, string destpath)
        {
            string[] directories = Directory.GetDirectories(frompath);
            foreach (string file in directories)
            {
                //Console.WriteLine(file);
                Directory.CreateDirectory(destpath + @"\" + Path.GetFileName(file));
            }
            return directories;
        }
    }
}
