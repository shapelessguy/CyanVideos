using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        public static Screen defaultScreen;
        private static System.Threading.Thread Loading;
        public static LoadingForm loadingForm;
        private static bool loading_active = false;
        public static bool VLC_Installed = false;
        public static bool enabledToSave = false;

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

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("System architecture: "+ System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
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
                try { if (Directory.Exists(libvlc)) { VLC_Installed = true; } } catch (Exception e) { MessageBox.Show(e.Message); };
                // MessageBox.Show(libvlc + "  "+ VLC_Installed);
                if(Directory.Exists(@"C:\Program Files\VideoLAN\VLC")) libvlc = @"C:\Program Files\VideoLAN\VLC";
                SetMonitor();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Loading = new System.Threading.Thread(LOADING);
                Loading.Start();
                //foreach (string dir in Directory.GetDirectories(@"C:\Users\Claudio\Desktop\Film")) Directory.CreateDirectory(dir.Replace("Film", "FilmA"));
                Console.WriteLine("Application start");
                Application.Run(win = new Window());
            }
            catch (Exception ex) { MessageBox.Show("Errore: "+ex.Message); }
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
                Properties.Settings.Default.Sources = "";
                Properties.Settings.Default.Logs = "";
                foreach (Source source in Window.Sources)
                {
                    Properties.Settings.Default.Sources += source.directory + "|#|" + source.tag.name.Replace(" (empty)","") + "|#|";
                }
                foreach (string key in Window.FaultSources.Keys)
                {
                    string line = key + "|#|" + Window.FaultSources[key] + "|#|";
                    if (!Properties.Settings.Default.Sources.Contains(line)) Properties.Settings.Default.Sources += line;
                }
                //Console.WriteLine("saving "+Window.logs.Count+" logs");
                foreach (Log log in Window.logs)
                {
                    string line = log.serialize() + "|#|";
                    if (!Properties.Settings.Default.Logs.Contains(line)) Properties.Settings.Default.Logs += line;
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception) { return; }
        }
        private static void SetMonitor()
        {
            bool foundScreen = false;
            Screen[] allScreens = Screen.AllScreens;
            foreach (Screen screen in allScreens) if (ScreenClass.GetScreenName(screen) == Properties.Settings.Default.defDevice) foundScreen = true;
            if (!foundScreen) Properties.Settings.Default.defDevice = ScreenClass.GetScreenName(Screen.PrimaryScreen);
            if (Properties.Settings.Default.defDevice == "") Properties.Settings.Default.defDevice = ScreenClass.GetScreenName(GetMaxScreen());
            foreach (Screen screen in allScreens) if (ScreenClass.GetScreenName(screen) == Properties.Settings.Default.defDevice) defaultScreen = screen;
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
            catch (Exception) { Console.WriteLine("Errore di recupero informazioni"); return "fault"; }
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
        public static string[] GetAllVideos(string[] files)
        {
            List<string> final = new List<string>();
            foreach (string file in files)
            {
                //Console.WriteLine(file);
                foreach (string extension in videoExtensions)
                {
                    if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) final.Add(file);
                }
            }
            return final.ToArray();
        }
        public static string[] GetAllSubs(string[] files)
        {
            List<string> final = new List<string>();
            foreach (string file in files)
            {
                //Console.WriteLine(file);
                foreach (string extension in new string[] {"srt"})
                {
                    if (file.Substring(file.Length - 5).ToLower().Contains("." + extension)) final.Add(file);
                }
            }
            return final.ToArray();
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
            int video_num = GetAllVideos(Directory.GetFiles(path)).Length;
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
