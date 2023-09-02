using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using AForge.Video;
using LibVLCSharp.Shared;

namespace CyanVideos
{
    public class Iconxx
    {
        //Logic:                 File level                                     Implementation level                   reliability
        //                      void or null                       principal_film==null && sec_films.Count==0               0
        //            principal_film title:AsItIsPowerVideos          principal_film.title==AsItIsPowerVideos               1
        //           principal_film title:NotFoundPowerVideos        principal_film.title==NotFoundPowerVideos              2
        //                   No principal_film                      principal_film==null && sec_films.Count!=0              3
        //                     Everything ok                          principal_film!=null && (generic title)               4
        public void Dispose()
        {
            // return;
            if(Source.verbose) Console.WriteLine("Disposing icon: "+title);
            if (Image != null) Image.Dispose();
            if (ImageBlurred != null) ImageBlurred.Dispose();
            if (checkImage_timer != null) checkImage_timer.Dispose();
            if (save_timer != null) save_timer.Dispose();
            if (principal_film != null) principal_film.Disposer();
            if (sec_films.Count != 0) foreach(Film film in sec_films) film.Disposer();
        }
        public string title;
        public string sourcepath;
        public string fullpath;
        public string infopath;
        public bool series;
        public string imagePath;
        public bool folder;
        public Bitmap Image;
        public Bitmap ImageBlurred;
        public Point blur = new Point();
        public bool image_validated = true;
        public int forcedHeight;
        public Log ContinueToWatch = null;

        private System.Windows.Forms.Timer pauseFromClick = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false};

        public Film principal_film = null;
        bool principal_modified = false;
        public List<Film> sec_films = new List<Film>();
        bool sec_modified = false;

        public int reliability = -1;
        private System.Threading.Timer save_timer, checkImage_timer;
        public bool in_first_panel = false;

        public static System.Threading.Timer LoadSegments;

        public void SetP(Film film) { principal_film = film; principal_modified = true; }
        public void AddS(Film item)
        {
            if (item == null) return;
            if (item.title == "AsItIsPowerVideos" || item.title == "NotFoundPowerVideos" || item.title == "") return;
            if (item.InsertInCollection(sec_films)) sec_modified = true;
        }
        private void SaveIcon()
        {
            try
            {
                if (series && !in_first_panel) return;
                Console.WriteLine("Saving Info for: " + fullpath);
                List<string> already_written = new List<string>();
                using (StreamWriter sw = File.CreateText(infopath))
                {
                    if (principal_film != null)
                    {
                        sw.WriteLine("principal" + principal_film.serialize());
                        already_written.Add(principal_film.serialize());
                    }
                    if (sec_films.Count == 0) return;
                    foreach (Film film in sec_films)
                    {
                        if (film != null)
                        {
                            string write = film.serialize();
                            if (!already_written.Contains(write))
                            {
                                sw.WriteLine("sec" + write);
                                already_written.Add(write);
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("Error while saving: " + e.Message); }
        }

        public Iconxx(string sourcepath, string fullpath, bool series, int forcedHeight = 0, bool extremis_series = false, bool in_first_panel = false)
        {
            this.series = series;
            if (Source.verbose) Console.WriteLine("       Iconxx" + (this.series == true ? ("(Series)") : "") + ": " + fullpath + "  has been added");
            this.sourcepath = sourcepath;
            infopath = fullpath + @"\infopowervideos.txt";
            this.fullpath = fullpath;
            this.forcedHeight = forcedHeight;
            imagePath = "";
            title = Program.CleanName(Path.GetFileNameWithoutExtension(fullpath));
            if (this.series) this.in_first_panel = in_first_panel;
            if (series) folder = true;
            else folder = isFolder(fullpath, this.series);
            Check_Image();
            if (extremis_series) { return; }
            UpdateContinue();
            CheckExclamation();
            if (folder && (in_first_panel || !series)) Check_SubFolders();

            save_timer = new System.Threading.Timer(TimerSaving, null, 5000, System.Threading.Timeout.Infinite);
            checkImage_timer = new System.Threading.Timer(TimerImage, null, 5000, System.Threading.Timeout.Infinite);
        }
        void TimerSaving(Object state)
        {
            try
            {
                if (fault > 3) return;
                Check_Image();
                CheckExclamation();
                Check_Save();
                if (save_timer != null) save_timer.Change(500, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex) { Console.WriteLine("Exception TimeSaving: " + ex.Message); }
        }
        void TimerImage(Object state)
        {
            if (fault > 3) return;
            try
            {
                Check_Image();
                if (checkImage_timer != null) checkImage_timer.Change(1500, System.Threading.Timeout.Infinite);
            }
            catch (Exception) { }
        }
        int bypass = 0;
        private void Check_Save()
        {
            if (principal_modified || sec_modified) {
                if (CheckExclamation() || bypass > 5)
                {
                    bypass = 0;
                    principal_modified = false;
                    sec_modified = false;
                    SaveIcon();
                }
                else bypass++;
            }
        }
        public void UpdateContinue()
        {
            ContinueToWatch = Log.FindByParent(Window.logs, fullpath);
            //if (ContinueToWatch != null) { Console.Write(fullpath + " "); ContinueToWatch.Print(); }
        }

        public static bool isFolder(string path, bool series)
        {
            try
            {
                string[] directories;
                try { directories = Directory.GetDirectories(path); } catch (Exception) { directories = null; }
                if (directories == null || directories.Length == 0)
                {
                    if (series)
                    {
                        if (Program.GetAllVideos(path).Length > 0) return true;
                    }
                    return false;
                }
                else
                {
                    bool multimedia_file_found = false;
                    foreach (string directory in directories)
                    {
                        string[] files = Directory.GetFiles(directory);
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (Program.IsVideo(files[i])) { multimedia_file_found = true; break; }
                        }
                    }
                    if (!multimedia_file_found) { return false; }
                    return true;
                }
            }
            catch (Exception) { return false; }
        }

        //bool mustFindImage = false;
        //int iterations = 10;
        //bool waiting_check = false;
        //int to_be_sure = 0;
        IMDB download_info;
        public bool initial_download = false;
        public void Initialize()
        {
            reliability = -1;
            principal_film = null;
            sec_films.Clear();
            initial_download = false;
        }
        int fault = 0;
        private bool CheckExclamation()    // Return true if there's no need to iterate again
        {
            try
            {
                if ((folder || series) && !in_first_panel) return true;

                if (reliability == -1)
                {
                    INFO info = GetINFO(infopath);
                    GetData(infopath);
                    reliability = info.reliability;
                    FillResearch(GetFilm_ByLines(info.lines));

                    return false;
                }
                else
                {
                    if (principal_film == null && sec_films.Count == 0) reliability = 0;
                    else if (principal_film == null && sec_films.Count != 0) reliability = 3;
                    else if (principal_film.title == "AsItIsPowerVideos") reliability = 1;
                    else if (principal_film.title == "NotFoundPowerVideos") reliability = 2;
                    else { reliability = 4; }
                    //if (in_first_panel) Console.WriteLine(title + "   "+ reliability);
                }
                if (reliability == 0 && !initial_download)
                {
                    initial_download = true;
                    Console.WriteLine("Downloading info for: " + title);
                    download_info = new IMDB(this);
                    return false;
                }
                else if (reliability == 3)
                {
                    if (sec_films.Count == 1)
                    {
                        principal_film = sec_films[0];
                        sec_films.Clear();
                        principal_modified = true;
                        sec_modified = true;
                    }
                }
                else if (reliability == 4)
                {
                    if (principal_film.changed) { principal_film.changed = false; SaveIcon(); return false; }

                    if (imagePath == "" || imagePath == "folder" || imagePath == "null")
                    {
                        if (!principal_film.downloading_image)
                        {
                            principal_film.downloading_image = true;
                            Console.WriteLine("Downloading image for: " + title +", address: "+principal_film.poster_path);
                            if(!in_first_panel) principal_film.FindImage_Thread(fullpath);
                            else principal_film.FindBackImage_Thread(fullpath + @"\imagefromPowerVideos.jpg");
                            return false;
                        }
                    }
                    if (!principal_film.complete)
                    {
                        if (!principal_film.downloading_castInfo)
                        {
                            principal_film.downloading_castInfo = true;
                            Console.WriteLine("Downloading cast info for: " + title);
                            principal_film.FindDetails_Thread();
                            return false;
                        }
                    }
                    else { FillResearch(film); return false; }
                }
                return true;

            }
            catch (Exception a) { Console.WriteLine("Exception CheckExclamation: " + a.Message); return false; }
        }


        List<Film> films_for_backdrop = new List<Film>();
        List<string> dirs_for_backdrop = new List<string>();
        List<string> dir_found = new List<string>();
        string[] sub_dir;
        Film film; INFO info;
        int index_backdrop = 1;
        int num_exclamations = 0;
        int num_questions = 0;
        public void Check_SubFolders(bool forced = false)
        {
            try
            {
                sub_dir = Directory.GetDirectories(fullpath);
                dir_found = sub_dir.ToList();
                num_exclamations = 0;
                num_questions = 0;
                foreach (string dir in sub_dir)
                {
                    film = GetPrincipalFilm(dir + @"\infopowervideos.txt");
                    if (film != null)
                    {
                        if (film.backdrop_path != "") if (!films_for_backdrop.Contains(film)) { films_for_backdrop.Add(film); dirs_for_backdrop.Add(dir); }
                        FillResearch(film);
                        info = GetINFO(dir + @"\infopowervideos.txt");
                        if (info.reliability == 0) num_questions++;
                        else if (info.reliability == 2 || info.reliability == 3) num_exclamations++;
                        info = null;
                        film.Disposer();
                    }
                    film = null;
                }
                if (in_first_panel) return;

                if ((imagePath == "" || imagePath == "folder" || imagePath == "null" || forced) && films_for_backdrop.Count > 0)
                {
                    if (films_for_backdrop.Count == 1) index_backdrop = 0;
                    film = films_for_backdrop[index_backdrop];

                    if (!film.downloading_backdrop) Console.WriteLine("Downloading backdrop icon for: " + title);
                    film.downloading_backdrop = true;
                    film.FindBackImage_Thread(fullpath + @"\imagefromPowerVideos.jpg", this, dirs_for_backdrop[index_backdrop] + @"\infopowervideos.txt");

                    if (index_backdrop == films_for_backdrop.Count - 1) index_backdrop = 0;
                    else index_backdrop++;

                    film.Disposer();
                    film = null;
                }
                else if ((imagePath == "" || imagePath == "folder" || imagePath == "null" || forced) && films_for_backdrop.Count == 0)
                {
                    if (sub_dir.Length == 1) index_backdrop = 0;
                    if (sub_dir.Length > 0)
                    {
                        foreach (string file in Directory.GetFiles(sub_dir[index_backdrop]))
                        {
                            if (file.Contains(".jpg"))
                            {
                                imagePath = file;
                                SetImage();
                                if (index_backdrop == sub_dir.Length - 1) index_backdrop = 0;
                                else index_backdrop++;
                            }
                        }
                    }
                }
                if (num_exclamations != 0) reliability = 3;
                else if (num_questions != 0) reliability = 2;
                else reliability = 4;
                //Program.win.RefreshFirstPanel(true);
                //Program.win.RefreshSecondPanel(true);
            }
            catch (Exception ex) { Console.WriteLine("Exception CheckSubFolders: " + ex.Message); }
        }

        private void FindBackIcon(object sender, EventArgs e)
        {
            try
            {
                foreach (string image in Program.GetAllImages(System.IO.Directory.GetFiles(fullpath))) System.IO.File.Delete(image);
                Console.WriteLine("All images for " + fullpath + " deleted");
                imagePath = "null";
                if (!folder) { Image = Properties.Resources._null; }
                else { Image = Properties.Resources.folder; }
                ImageBlurred = new Bitmap(Image, takeBlurLevel(Image).X, takeBlurLevel(Image).Y);
            }
            catch (Exception ex) { Console.WriteLine("Exception FindBackIcons: " + ex.Message); }
        }

        private Point takeBlurLevel(Image image)
        {
            return new Point((int)(image.Width*0.5), (int)(image.Height * 0.2));
        }

        public static void FillResearch(Film film)
        {
            if (film == null) return;
            foreach (string genre in film.genres) if (!PanelResearch.allGenres.Contains(genre))
                { PanelResearch.allGenres.Add(genre); PanelResearch.new_genre = true; }
            foreach (Credits credit in film.lcredits)
            {
                if (credit.department == "" || credit.department == "Actors") continue;
                if (!PanelResearch.allRoles.Contains(credit.department)) { PanelResearch.allRoles.Add(credit.department); PanelResearch.new_role = true; }
            }
        }


        int n_fault = 0;
        public void Check_Image(bool forced = false)
        {
            if (n_fault > 10) return;
            try
            {
                if(principal_film != null && principal_film.download_image_complete)
                {
                    principal_film.download_image_complete = false;
                    imagePath = "";
                }
                if (!File.Exists(imagePath) || imagePath == "" || imagePath == "folder" || imagePath == "null" || forced)
                {
                    GetImagePath();
                    SetImage();
                }
            }
            catch (Exception e) { n_fault++; Console.WriteLine("Exception from check_image: "+e.Message); }

        }

        private void GetImagePath()
        {
            //Console.WriteLine(title + "   " + imagePath + "     ");
            string new_imagePath = Program.ImagePath(fullpath);
            if (new_imagePath == "fault")
            {
                new_imagePath = ""; fault++;
            };
            
            if (imagePath == "" && series)
            {
                //string[] subfiles = Directory.GetFiles(fullpath);
                string path_we = "";
                string path_r = "";
                if (Program.IsVideo(fullpath))
                {
                    path_we = Directory.GetParent(fullpath).FullName + @"\" + Path.GetFileNameWithoutExtension(fullpath);
                    path_r = fullpath;
                }
                else if (Program.GetAllVideos(fullpath).Length == 1)
                {
                    path_we = Directory.GetParent(Directory.GetFiles(fullpath)[0]).FullName + @"\" + Path.GetFileNameWithoutExtension(Directory.GetFiles(fullpath)[0]);
                    path_r = Directory.GetFiles(fullpath)[0];
                }

                if (path_we != "")
                {
                    string result;
                    if (File.Exists(path_we + ".bmp")) result = path_we + ".bmp";
                    else
                    {
                        bool found = false;
                        foreach(Iconxx icon in imagesSnapping)
                        {
                            if (icon.fullpath == fullpath) { found = true; break; }
                        }
                        if(!found) imagesSnapping.Add(this);

                    }
                }
            }
            if (new_imagePath == "" && imagePath == "null")
            {
                return;
            }
            if (new_imagePath == "" && imagePath == "folder")
            {
                return;
            }
            imagePath = new_imagePath;

            if (imagePath == "")
            {
                Image = (Bitmap)Properties.Resources.folder.Clone();
                ImageBlurred = new Bitmap(Image, takeBlurLevel(Image).X, takeBlurLevel(Image).Y);
                if (series && !in_first_panel)
                {
                    bool found = false;
                    bool end = false;
                    string path = fullpath;
                    bool sharedFolder = false;
                    
                    try
                    {
                        if (Program.IsVideo(path) || Program.GetAllVideos(path).Length == 0) { path = Directory.GetParent(path).FullName; sharedFolder = true; }
                    }
                    catch (Exception e) { path = Directory.GetParent(path).FullName; sharedFolder = true; Console.WriteLine("Exception from GetImagePath: "+e.Message); }
                    

                    for (int i = 0; i < 15; i++)
                    {
                        if (!end && !found)
                        {
                            foreach (Source source in Window.Sources) { if (source.directory == path) end = true; }
                            try
                            {
                                string[] images = Program.GetAllImages(Directory.GetFiles(path));
                                if (images.Length > 0)
                                {
                                    if(sharedFolder && i == 0)
                                    {
                                        if (images.Length > 0) foreach (string img in images)
                                            {
                                                if (img.Contains(Directory.GetParent(fullpath).FullName + @"\imagefromPowerVideos_" + Path.GetFileNameWithoutExtension(fullpath))) { found = true; imagePath = img; end = true; }
                                            }
                                    }
                                    else
                                    {
                                        found = true; imagePath = images[0];
                                    }
                                    
                                }
                            }
                            catch (Exception) { }
                            path = Directory.GetParent(path).FullName;
                            //if (File.Exists(path + @"\imagefromPowerVideos.jpg")) { found = true; imagePath = path + @"\imagefromPowerVideos.jpg"; }
                        }
                        else break;
                    }
                    if (!found) {
                        imagePath = "folder";
                        List<string> fakelist = new List<string>();
                        GetFirstSubImage(fullpath, fakelist, 0);
                        if (fakelist.Count > 0) imagePath = fakelist[0];
                    }
                    return;
                }
                if (!folder) { Image = (Bitmap)Properties.Resources._null.Clone(); imagePath = "null"; }
                else { Image = (Bitmap)Properties.Resources.folder.Clone(); imagePath = "folder"; }
                ImageBlurred = new Bitmap(Image, takeBlurLevel(Image).X, takeBlurLevel(Image).Y);

                Program.win.RefreshFirstPanelImages();
                Program.win.RefreshSecondPanelImages();
            }
        }
        public void SetImage()
        {
            try
            {
                if (imagePath == "null" || imagePath == "folder") return;
                StreamReader streamReader = new StreamReader(imagePath);
                Bitmap btm = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();

                int height = Program.defaultIconHeight;
                int width = (int)(height * Window.prop_width_height);
                if (forcedHeight != 0)
                {
                    height = forcedHeight;
                    if (folder) width = (int)(2 * height * Window.prop_width_height + Window.intraDistanceX);
                    else width = (int)(height * Window.prop_width_height);
                }
                btm = new Bitmap(btm, new Size(width, height));
                Image = (Bitmap)btm.Clone();
                ImageBlurred = new Bitmap(Image, takeBlurLevel(Image).X, takeBlurLevel(Image).Y);
                image_validated = false;

                Program.win.refreshImages = true;
                btm = null;
            }
            catch (Exception) { }
        }
        private static string Serialize(List<Iconxx> list)
        {
            string result = "";
            foreach (Iconxx icon in list) result += icon.fullpath + ", ";
            return result;
        }
        private static List<Iconxx> imagesSnapping = new List<Iconxx>();
        static string working_on = "";
        public static void TakeSnap(Object state)
        {
            if (imagesSnapping.Count == 0) { working_on = ""; LoadSegments.Change(200, System.Threading.Timeout.Infinite); return; }
            //Console.WriteLine("2 " + Serialize(imagesSnapping));
            if (working_on == imagesSnapping[0].fullpath) { LoadSegments.Change(200, System.Threading.Timeout.Infinite); return; }

            string fullpath = "";
            if (!Program.IsVideo(imagesSnapping[0].fullpath) && Program.GetAllVideos(imagesSnapping[0].fullpath).Count() == 1)
            {
                fullpath = Program.GetAllVideos(imagesSnapping[0].fullpath)[0];
            }
            else fullpath = imagesSnapping[0].fullpath;

            working_on = fullpath;

            string pathh = Directory.GetParent(fullpath) + @"\imagefromPowerVideos_" + Path.GetFileNameWithoutExtension(fullpath) + ".bmp";
            if (File.Exists(pathh)) { imagesSnapping.RemoveAt(0); LoadSegments.Change(200, System.Threading.Timeout.Infinite); return; }
            
            Console.WriteLine("Snapping: "+working_on);
            //LoadSegments.Change(200, System.Threading.Timeout.Infinite);
            string result = "";
            try
            { 
                Accord.Video.FFMPEG.VideoFileReader reader;
                reader = new Accord.Video.FFMPEG.VideoFileReader();
                reader.Open(fullpath);
                //Console.WriteLine("width:  " + reader.Width);
                //Console.WriteLine("height: " + reader.Height);
                //Console.WriteLine("fps:    " + reader.FrameRate);
                //Console.WriteLine("codec:  " + reader.CodecName);
                int afterSecond = 180;
                int frameToRead = (int)reader.FrameRate * afterSecond;
                reader.ReadVideoFrame(frameToRead);
                

                Bitmap bitmap = reader.ReadVideoFrame();
                result = Directory.GetParent(fullpath).FullName + @"\imagefromPowerVideos_" + Path.GetFileNameWithoutExtension(fullpath) + ".bmp";
                if (bitmap != null)
                {
                    bitmap.Save(result);
                    imagesSnapping[0].imagePath = result;
                    Program.Compress(result, "", (int)(Program.defaultIconHeight * Window.prop_width_height) * 2 + Window.intraDistanceX, Program.defaultIconHeight);
                    imagesSnapping[0].SetImage();
                    imagesSnapping.RemoveAt(0);
                }
            }
            catch (Exception) { imagesSnapping.RemoveAt(0); }
            LoadSegments.Change(200, System.Threading.Timeout.Infinite);

        }
        bool allowClick = true;
        public void ClickEvent(object sender, MouseEventArgs e)
        {
            if (e!= null && e.Button == MouseButtons.Right) return;
            else if (allowClick)
            {
                Console.WriteLine("ClickEvent");
                pauseFromClick.Enabled = true;
                pauseFromClick.Tick += (o, exa) => { allowClick = true; pauseFromClick.Enabled = false;};
                ClickIconxx(this);
                // allowClick = false;
            }
            return;
        }
        public static void ClickIconxx(Iconxx icon)
        {
            Console.WriteLine("ClickIconxx");
            MediaPanel.PlayList.Clear();
            Program.win.mediaPanel.numFilm = 0;
            string[] directories = new string[0];
            Program.EnableLoading();
            try
            {
                directories = Directory.GetDirectories(icon.fullpath);
                Console.WriteLine(Program.GetAllVideos(icon.fullpath, 10).Length);
                if ((icon.series && Program.GetAllVideos(icon.fullpath, 10).Length > 1) ||
                    (!icon.series && isFolder(icon.fullpath, false)))
                {
                    PreparePanel2(icon.fullpath, icon.series);

                    if (Program.win.ricerca.Visible) Program.win.OpenRicerca_Click(null, null);
                    Program.EnableLoading(false);
                    return;
                }
                else if (!icon.series) {
                    icon.StartApp();
                    Program.EnableLoading(false);
                    return; }
            }
            catch (Exception) { Console.WriteLine("Exception from ClickIconxx1"); Program.EnableLoading(false); if (!icon.series) { return; } }
            Program.EnableLoading(false);

            if (icon.series)
            {
                //          icon.fullpath = folder (father)  
                if (Program.IsVideo(icon.fullpath))
                {
                    foreach (string extension in Program.videoExtensions)
                    {
                        if (icon.fullpath.Substring(icon.fullpath.Length - 5).ToLower().Contains("." + extension))
                        {
                            if (Properties.Settings.Default.internalPlayer && Program.VLC_Installed) { LoadSeries(icon.fullpath); }
                            else System.Diagnostics.Process.Start(icon.fullpath);
                        }
                    }
                    return;
                }
                else
                {
                    Program.EnableLoading();
                    string[] videos = Program.GetAllVideos(icon.fullpath, 5);
                    foreach (string video in videos) Console.WriteLine(video);
                    if (videos.Length > 1)                                        // if there are multiple videos opens the sec panel
                    {
                        PreparePanel2(icon.fullpath, icon.series, icon.series);
                    }
                    else if (videos.Length == 1)                                                             // else starts the video
                    {
                        if (Properties.Settings.Default.internalPlayer && Program.VLC_Installed) LoadSeries(videos[0]);
                        else System.Diagnostics.Process.Start(videos[0]);
                        
                    }
                    else { 
                        MessageBox.Show("Videos not found!"); 
                    }
                    Program.EnableLoading(false);
                }
                
            }
        }
        public static void LoadSeries(string FilePath)
        {
            //MediaPanel.PlayList.Clear();
            foreach (string path in Ordering.OrderAlphanumeric(GetSeriesEpisodes(FilePath).ToArray()))
            {
                string seriesName = FindTopInfo(path).title;
                if (seriesName == "AsItIsPowerVideos" || seriesName == "NotFoundPowerVideos") seriesName = "";
                if (seriesName != "") seriesName += " - ";
                string str_n = Program.CleanZeros(Path.GetFileName(Path.GetDirectoryName(path)));
                MediaPanel.PlayList.Add(new Vision(path, seriesName + Path.GetFileNameWithoutExtension(path) + " - " + str_n));
            }
            for (int i = 0; i < MediaPanel.PlayList.Count; i++)
                if (MediaPanel.PlayList[i].filename == FilePath)
                {
                    Program.win.mediaPanel.LoadFilm(i, true);
                    Program.win.mediaPanel.ToFront();
                }
        }
        public static List<string> GetSeriesEpisodes(string path)
        {
            List<string> lista = new List<string>();
            string parentPath = path;
            for (int i = 0; i<1; )
            {
                path = parentPath;
                parentPath = Directory.GetParent(path).FullName;
                foreach (Source source in Window.Sources) if (parentPath == source.directory) { i = 1; break; };
            }
            string firstPath = path;
            lista.AddRange(Program.GetAllVideos(path, 10));
            return lista;
        }
        public static void GetFirstSubImage(string path, List<string> listAdd, int iteration = 0)
        {
            if (listAdd.Count > 0) return;
            int maximumIteration = 5;      // maximum level of deepth
            if (iteration >= maximumIteration) return;
            string[] directories = Directory.GetDirectories(path);
            if (directories.Length != 0)
            {
                foreach (string dir in directories)
                {
                    try { GetFirstSubImage(dir, listAdd, iteration + 1); } catch (Exception) { }
                }
            }
            foreach(string file in Directory.GetFiles(path))
            {
                if (Program.IsImage(file)) { listAdd.Add(file); Console.WriteLine(file); }
            }
        }
        public static void AddToList(string path, List<string> listAdd, int iteration = 0)
        {
            int maximumIteration = 5;      // maximum level of deepth

            if (iteration >= maximumIteration) return;
            string[] directories = Directory.GetDirectories(path);
            if (directories.Length != 0)
            {
                foreach (string dir in directories)
                {
                    try { AddToList(dir, listAdd, iteration+1); } catch (Exception) { }
                }
            }
            else listAdd.AddRange(Program.GetAllVideos(path, 10));
        }
        public static void ClickIconxx(string fullpath, bool series)
        {
            string[] directories = new string[0];
            try { directories = Directory.GetDirectories(fullpath); } catch (Exception ex) { MessageBox.Show("No match has been found: "+ ex.Message); return; }
            if (directories.Length > 0)
            {
                PreparePanel2(fullpath, series);
            }
        }
        private static void PreparePanel2(string fullpath, bool series=false, bool extremis_series = false)
        {
            Program.win.OpenRicerca_Click(true, false);
            Program.win.firstpanel.Refresh(true, true, true);
            Source source = new Source(fullpath, Program.CleanName(Path.GetFileNameWithoutExtension(fullpath)), series, extremis_series);
            if (source.Icons().Count == 0)
            {
                MessageBox.Show("This folder is empty!");
                Program.EnableLoading(false);
                return;
            }
            Program.win.DisposeDeepSource();
            Program.win.DeepSource = source;
            Program.win.secondpanel.ToFront();
            Program.win.ResizePanel2();
            Program.win.secondpanel.Refresh(true);
        }

        public void ClickExclamation(object sender, EventArgs e)
        {
            if (folder && !in_first_panel) { ClickEvent(null, null); return; }
            if (reliability == 2) {
                if (!NotFound.active) { NotFound message = new NotFound(this); message.Show(); }
                //MessageBox.Show("Film sconosciuto.. prova a rinominare il file!");
                return; }
            if (FilmSelection.actual_form != null) FilmSelection.actual_form.Close();
            FilmSelection selection = new FilmSelection(this);
            selection.Show();
        }
        private void StartApp()
        {
            int count = 0;
            string filename = "";
            try
            {
                foreach (string file in Directory.GetFiles(fullpath))
                {
                    foreach (string extension in Program.videoExtensions)
                    {
                        if (file.Substring(file.Length-5).ToLower().Contains("." + extension)) { count++; filename = file; }
                    }
                }
                if (count == 0) { Program.EnableLoading(false); MessageBox.Show("No video found"); return; }
                if (count > 1) { Program.EnableLoading(false); MessageBox.Show("Folders must contain only one video file"); return; }
            }
            catch (Exception) { Program.EnableLoading(false); MessageBox.Show("Error while fetching information from directory"); return; }
            //Program.win.WindowState = FormWindowState.Minimized;
            if (filename == "") return;
            if (Properties.Settings.Default.internalPlayer && Program.VLC_Installed
                )
            {
                //MediaPanel.PlayList.Clear();
                string header = Program.CleanName(Path.GetFileNameWithoutExtension(filename));
                INFO info = GetINFO(Directory.GetParent(filename) + @"\infopowervideos.txt");
                if (info.title != "" && info.title != "AsItIsPowerVideos" && info.title != "NotFoundPowerVideos") header = info.title;
                MediaPanel.PlayList.Add(new Vision(filename, header));
                //Console.WriteLine(filename + "     " + MediaPanel.PlayList.Count);
                Program.win.mediaPanel.ToFront();
                Program.win.mediaPanel.ToolBarShow();
                Program.win.mediaPanel.LoadFilm(0);
            }
            else { Program.EnableLoading(false); System.Diagnostics.Process.Start(filename);}
            Program.EnableLoading(false);
        }


        public void Info(object sender, EventArgs e)
        {
            if (folder && !in_first_panel) { ClickEvent(null, null); return; }
            if (InfoClass.actual_form != null) InfoClass.actual_form.Close();
            if (principal_film == null)
            {
                if (FilmSelection.actual_form != null)
                {
                    FilmSelection.actual_form.Close();
                }
                FilmSelection form = new FilmSelection(this);
                form.Show();
                return;
            }
            else if (principal_film.title == "AsItIsPowerVideos" || principal_film.title == "NotFoundPowerVideos")
            {
                if (FilmSelection.actual_form != null)
                {
                    FilmSelection.actual_form.Close();
                }
                FilmSelection form = new FilmSelection(this);
                form.Show();
                return;
            }
            else if (principal_film.lcast == null || principal_film.lcredits == null) return;
            InfoClass info = new InfoClass(this);
            info.Show();
            info.Text = "Info for  -  " + title + "  -";
        }

        public void GetData(string info_path)
        {
            try
            {
                if (!System.IO.File.Exists(info_path)) return;
                foreach (string line in System.IO.File.ReadAllLines(info_path))
                {
                    string[] data = line.Split(new string[] { "^|^" }, StringSplitOptions.None);
                    Film film = GetFilm_ByLines(data);

                    if (data[0] == "principal") { principal_film = film; }
                    else sec_films.Add(film);
                }
            }
            catch (Exception e) { Console.WriteLine("Exception GetData - " + e.Message); return; }
        }

        public static Film GetPrincipalFilm(string info_path)
        {
            try
            {
                Film film;
                if (!System.IO.File.Exists(info_path)) return null;
                foreach (string line in System.IO.File.ReadAllLines(info_path))
                {
                    string[] data = line.Split(new string[] { "^|^" }, StringSplitOptions.None);
                    if (data[0] == "principal") { film = GetFilm_ByLines(data); return film; }
                }
                return null;
            }
            catch (Exception) { return null; }
        }

        private static Film GetFilm_ByLines(string[] data)
        {
            if (data.Length == 0) return null;
            Film film = new Film();
            film.title = data[1];
            Int32.TryParse(data[2], out film.runtime);
            film.popularity = data[3];
            film.id = data[4];
            film.original_language = data[5];
            film.vote_average = data[6];
            film.vote_count = data[7];
            film.poster_path = data[8];
            if (film.poster_path.Length > 0) if (film.poster_path.Substring(film.poster_path.Length - 1, 1) == "\"") film.poster_path = film.poster_path.Substring(0, film.poster_path.Length - 1);
            film.backdrop_path = data[9];
            if (film.backdrop_path.Length > 0) if (film.backdrop_path.Substring(film.backdrop_path.Length - 1, 1) == "\"") film.backdrop_path = film.backdrop_path.Substring(0, film.backdrop_path.Length - 1);
            film.release_date = data[10];
            film.overview = data[11];
            Int64.TryParse(data[12], out film.revenue);
            string[] genres = data[13].Split(new string[] { "_^_" }, StringSplitOptions.None);
            foreach (string genre in genres) if (genre != "") film.genres.Add(genre);
            string[] companies = data[14].Split(new string[] { "_^_" }, StringSplitOptions.None);
            foreach (string company in companies) if (company != "") film.production_companies.Add(company);
            film.complete = Convert.ToBoolean(data[15]);

            if (data.Length > 16)
            {
                string[] casts = data[16].Split(new string[] { "_^_" }, StringSplitOptions.None);
                foreach (string cast in casts)
                {
                    Casting class_cast = new Casting();
                    string[] sub_cast = cast.Split(new string[] { "_*_" }, StringSplitOptions.None);
                    if (sub_cast.Length == 3)
                    {
                        class_cast.name = sub_cast[0];
                        class_cast.character = sub_cast[1];
                        film.lcast.Add(class_cast);
                    }
                }
            }
            if (data.Length > 17)
            {
                string[] credits = data[17].Split(new string[] { "_^_" }, StringSplitOptions.None);
                foreach (string credit in credits)
                {
                    Credits class_credit = new Credits();
                    string[] sub_credit = credit.Split(new string[] { "_*_" }, StringSplitOptions.None);

                    if (sub_credit.Length == 4)
                    {
                        class_credit.name = sub_credit[0];
                        class_credit.department = sub_credit[1];
                        class_credit.job = sub_credit[2];
                        film.lcredits.Add(class_credit);
                    }
                }
            }
            return film;
        }


        public class INFO
        {
            int count_pr = 0;
            int count_sec = 0;
            public string path = "";
            public string title = "";
            public int reliability = -1;
            public string[] lines;
            public INFO(int count_pr, int count_sec, string title, string[] lines)
            {
                this.lines = lines;
                this.count_pr = count_pr;
                this.count_sec = count_sec;
                this.title = title;
                if (count_pr == 0 && count_sec == 0) reliability = 0;
                else if (title == "AsItIsPowerVideos") reliability = 1;
                else if (title == "NotFoundPowerVideos") reliability = 2;
                else if (count_pr == 0 && count_sec != 0) reliability = 3;
                else reliability = 4;
            }
            public INFO()
            {
                this.reliability = 0;
                this.lines = new string[] {};
            }
        }

        public static INFO GetINFO(string info_path)   //Ritorna il numero dei principali (1), la lunghezza del titolo (0) e il numero dei secondari (N)
        {
            string title = "";
            int count_pr = 0;
            int count_sec = 0;
            try
            {
                string[] lines = new string[] {};
                if (!System.IO.File.Exists(info_path)) return new INFO();
                foreach (string line in System.IO.File.ReadAllLines(info_path))
                {
                    string[] data = line.Split(new string[] { "^|^" }, StringSplitOptions.None);

                    if (data[0] == "principal") { title = data[1]; lines = data;  count_pr++; }
                    else count_sec++;
                }

                INFO info = new INFO(count_pr, count_sec, title, lines);
                info.path = info_path;
                return info;
            }
            catch (Exception e) { Console.WriteLine("Exception GetData - " + e.Message); return new INFO(); }
        }
        public static INFO FindTopInfo(string path)
        {
            string parentPath = path;
            for (int i = 0; ; i++)
            {
                parentPath = Directory.GetParent(parentPath).FullName;
                foreach (Source source in Window.Sources) if (parentPath == source.directory) { return new INFO(); }
                foreach (string file in Directory.GetFiles(parentPath)) if (Path.GetFileName(file) == "infopowervideos.txt") { return GetINFO(file); }
            }
        }

    }
}
