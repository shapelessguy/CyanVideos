using CyanVideos.MultiMonitorTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
<<<<<<< HEAD
=======
using System.Reflection;
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    static class Program
    {
        public static Window win;
        public static int defaultIconHeight = 1000;
        public static string libvlc = "";
        public static string[] imageExtensions = { "png", "bmp", "gif", "png", "wmf", "jpeg", "jfif", "tiff", "jpg" };
        public static string[] videoExtensions = { "3gp","asf","avi","divx","flv","swf","mp4","mpg","ogm","wmv","mov",
            "mkv","nbr","rm","vob", "sfd","mpeg","webm","xvid" };
        public static string[] subExtensions = { "srt" };
        public static Screen defaultScreen;
        private static Thread Loading;
        public static LoadingForm loadingForm;
        private static bool loading_active = false;
        public static bool VLC_Installed = false;
        public static bool enabledToSave = false;
        public static MonitorCollection monitors;

        public static void EnableLoading(bool enable = true)
        {
            loading_active = enable;
        }
        public static bool GetLoading()
        {
            return loading_active;
        }

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
<<<<<<< HEAD
            monitors = MonitorCollection.getMonitorConfiguration();
=======
            monitors = MonitorCollection.getMonitorConfiguration(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MultiMonitorTool"));
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainView();
        }
        static void MainView()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("System architecture: " + System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
            Application.ThreadException += (sender, args) => {
                MessageBox.Show(string.Format("{0}:\n\n{1}", args.Exception.GetType().Name, args.Exception.Message),
                    string.Format(" {0}: Error", ""),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            };


            if (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") == "AMD64") libvlc = @"libvlc\win-x64";
            else libvlc = @"libvlc\win-x86";
            libvlc = Path.Combine(Environment.CurrentDirectory, libvlc);

            try
            {
                VLC_Installed = false;
                try 
                { 
                    if (Directory.Exists(libvlc)) { VLC_Installed = true; } 
                } 
                catch (Exception e) 
                { 
                    MessageBox.Show(e.Message); 
                };
                SetMonitor();

                Loading = new Thread(LOADING);
                Loading.Start();
                Application.Run(win = new Window());
            }
<<<<<<< HEAD
            catch (Exception ex) { MessageBox.Show("Errore: " + ex.Message); }
=======
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message + "\n" + ex.ToString()); }
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
        }
        public static void LOADING()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(loadingForm = new LoadingForm());
        }
        public static void Save()
        {
            if (!enabledToSave) return;
            try
            {
                Console.WriteLine("Saving..");
                Properties.Settings.Default.sources = "";
                Properties.Settings.Default.logs = "";
                foreach (Source source in Window.Sources)
                {
                    Properties.Settings.Default.sources += source.directory + "|#|" + source.tag.name.Replace(" (empty)","") + "|#|";
                }
                foreach (string key in Window.FaultSources.Keys)
                {
                    string line = key + "|#|" + Window.FaultSources[key] + "|#|";
                    if (!Properties.Settings.Default.sources.Contains(line)) Properties.Settings.Default.sources += line;
                }

                foreach (Log log in Window.logs)
                {
                    string line = log.serialize() + "|#|";
                    if (!Properties.Settings.Default.logs.Contains(line)) Properties.Settings.Default.logs += line;
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception) { return; }
        }
        private static void SetMonitor()
        {
            string foundScreen = "";

<<<<<<< HEAD
            foreach (string screen_id in monitors.getIds()) if (screen_id == Properties.Settings.Default.defDevice)
                {
                    foundScreen = screen_id;
                }
            if (foundScreen == "")
            {
                foundScreen = monitors.getIds()[0];
            }
            defaultScreen = monitors.getScreen(foundScreen);
=======
            if (monitors.getIds().Length != 0)
            {
                foreach (string screen_id in monitors.getIds()) if (screen_id == Properties.Settings.Default.defDevice)
                    {
                        foundScreen = screen_id;
                    }
                if (foundScreen == "")
                {
                    foundScreen = monitors.getIds()[0];
                }
                defaultScreen = monitors.getScreen(foundScreen);
            }
            else
            {
                foundScreen = "";
                defaultScreen = Screen.PrimaryScreen;
            }
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
            Properties.Settings.Default.defDevice = foundScreen;
            Properties.Settings.Default.Save();
        }

        public static string ImagePath(string dir)
        {
            if (IsVideo(dir) || IsImage(dir)) return "";
            List<string> images;
            try
            {
                images = GetAllImages(Directory.GetFiles(dir), false).ToList();
            }
            catch (Exception) { Console.WriteLine("Error while fetching information"); return "fault"; }
            if (images.Count == 0) return "";
            foreach (string img in images) { if (img.Contains(Directory.GetParent(dir).FullName + @"\" + Path.GetFileNameWithoutExtension(dir))) return img; }
            return images[0];
        }
        public static string[] GetAllImages(string[] files, bool images_fromPower = true)
        {
            List<string> final = new List<string>();
            List<string> imagesFromP = new List<string>();
            bool imagefrompowervideos = false;
            foreach (string file in files)
            {
                foreach (string extension in imageExtensions)
                {
                    if (file.Substring(file.Length - 5).ToLower().Contains("." + extension))
                    {
                        if (Path.GetFileName(file).Contains("imagefromPowerVideos_")) imagesFromP.Add(file);
                        else if (Path.GetFileName(file) != "imagefromPowerVideos.jpg") final.Add(file);
                        else imagefrompowervideos = true;
                    }
                }
            }
            if (imagefrompowervideos && files.Length > 0) final.Add(Directory.GetParent(files[0]).FullName + @"\imagefromPowerVideos.jpg");
            if(images_fromPower) final.AddRange(imagesFromP);
            return final.ToArray();
        }
        public static bool IsImage(string file)
        {
            foreach (string extension in imageExtensions)
            {
                if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) return true;
            }
            return false;
        }
        public static bool IsVideo(string file)
        {
            foreach (string extension in videoExtensions)
            {
                if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) return true;
            }
            return false;
        }
        public static string[] GetAllVideos(string directory, int recursion=0)
        {
            List<string> final = new List<string>();
            if (recursion > 0)
            {
                recursion -= 1;
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    final.AddRange(GetAllVideos(dir, recursion));
                }
            }
            foreach (string file in Directory.GetFiles(directory))
            {
                foreach (string extension in videoExtensions)
                {
                    if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) final.Add(file);
                }
            }
            return final.ToArray();
        }
        public static string[] GetAllSubs(string directory)
        {
            List<string> final = new List<string>();
            foreach (string dir in Directory.GetDirectories(directory))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    foreach (string extension in subExtensions)
                    {
                        if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) final.Add(file);
                    }
                }
            }
            foreach (string file in Directory.GetFiles(directory))
            {
                foreach (string extension in subExtensions)
                {
                    if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) final.Add(file);
                }
            }
            return final.ToArray();
        }
        public static string FormatNumberZero(int number, int length = 3)
        {
            string output = "";
            for (int i = number.ToString().Length; i < length; i++) output += "0";
            return output + number.ToString();
        }
        public static string CleanZeros(string text)
        {
            if (text.Contains("x"))
            {
                int x_pos = text.IndexOf("x"), n1 = -1, n2 = -1;
                try
                {
                    n1 = Int32.Parse(text.Substring(0, x_pos));
                    n2 = Int32.Parse(text.Substring(x_pos + 1));
                    return n1 + "x" + n2;
                }
                catch { }
            }
            return text;
        }
        public static string CleanName(string name)
        {
            string output = "";
            List<int> right_par = new List<int>();
            List<int> left_par = new List<int>();
            string new_name = name;
            bool exit = false;
            int iteration = 0;
            do
            {
                for (int i = 0; i < new_name.Length; i++)
                {
                    if (new_name.Substring(i, 1) == "(") right_par.Add(i);
                    if (new_name.Substring(i, 1) == ")") left_par.Add(i);
                }
                iteration++;
                exit = false;
                for (int i = 0; i < right_par.Count; i++)
                {
                    for (int j = 0; j < left_par.Count; j++)
                    {
                        int after_i = new_name.Length + 1;
                        if (i != right_par.Count - 1) after_i = right_par[i + 1];
                        if (left_par[j] > right_par[i] && left_par[j] < after_i)
                        {
                            output += new_name.Substring(0, right_par[i]);
                            output += new_name.Substring(left_par[j] + 1, new_name.Length - left_par[j] - 1);
                            right_par.Clear();
                            left_par.Clear();
                            new_name = output;
                            output = "";
                            exit = true;
                        }
                        if (exit) break;
                    }
                    if (exit) break;
                }
            }
            while (exit && iteration < 10);

            do
            {
                new_name = new_name.Replace("  ", " ");
            }
            while (new_name.Contains("  "));
            output = new_name.Trim();
            output = CleanZeros(output);
            return output;
        }
        public static void Compress(string image_path, string dest_path = "", int width = 0, int height = 0)
        {
            try
            {
                if (dest_path == "") dest_path = image_path;
                StreamReader streamReader = new StreamReader(image_path);
                Bitmap btm = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();

                if (width == 0 && height == 0)
                {
                    height = defaultIconHeight;
                    width = (int)(height * Window.prop_width_height);
                }
                btm = new Bitmap(btm, new Size(width, height));
                Console.WriteLine("Size: " + btm.PixelFormat);
                btm.Save(dest_path, ImageFormat.Jpeg);
            }
            catch (Exception) { }
        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
        private static Screen GetMaxScreen()
        {
            Screen output = Screen.PrimaryScreen;
            int max = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Width > max) { max = screen.Bounds.Width; output = screen; }
            }
            return output;
        }

        private static int iconType(string path)
        {
            if (IsVideo(path)) return -2;                                   // file multimediale
            int result = 0;
            int video_num = GetAllVideos(path).Length;
            int dir_num = Directory.GetDirectories(path).Length;
            try
            {
                if (video_num == 0 && dir_num == 0) return 0;               // directory vuota
                if (video_num == 1 && dir_num == 0) return -1;              // directory con un solo video
                if (video_num > 1 && dir_num == 0) return 3;                // directory con più video
                if (video_num == 0 && dir_num > 0) return 1;                // directory con solo directory al suo interno
                if (video_num == 1 && dir_num > 0) return 2;                // directory con un solo video e più directory
                if (video_num > 1 && dir_num > 0) return 4;                 // directory con più solo video e più directory
                
                return result;
            }
            catch (Exception) { return -3; }                                // file o directory particolare
        }
    }
}
