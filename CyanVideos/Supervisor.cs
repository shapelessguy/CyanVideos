using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CyanVideos
{
    public class Supervisor
    {
        static bool forceDimIcon = false; //if true, program gains on speed
        public Supervisor() { }
        public static List<string> StructureAdvice = new List<string>();
        static bool icons_pending = false;
        static string text = "";
        static public Source researchSource;
        public static void Disposer()
        {
            DisposeResearch();
            Program.win.firstpanel.Disposer();
            Program.win.secondpanel.Disposer();
        }
        public static void DisposeResearch()
        {
            if (researchSource != null) researchSource.Dispose(false);
            foreach (Iconxx icon in Program.win.Research_Categories)
            {
                bool found = false;
                foreach(Source source in Window.Sources)
                {
                    foreach (Iconxx icon2 in source.Icons()) if (icon == icon2) found = true;
                }
                if(!found) icon.Dispose();
            }
        }
        public static void ImportIconxx()
        {
            Program.win.firstpanel.SourcesToShow.Clear();
            if (researching && researchSource == null) return;
            if (ResearchClass.StrangeFirstResearch() || ResearchClass.StrangeDeepResearch())
            {
                if (!researching)
                {
                    Program.win.firstpanel.SourcesToShow.Add(researchSource);
                    if (icons_pending)
                    {
                        Program.win.firstpanel.Refresh(true, forceDimIcon);
                        icons_pending = false;
                    }
                }
            }
            else
            {
                if (!researching)
                {
                    researchSource = null;
                    foreach (Source source in Window.Sources)
                    {
                        if(Properties.Settings.Default.valore_mostra == 3 || Properties.Settings.Default.valore_mostra == 2 && source.series
                             || Properties.Settings.Default.valore_mostra == 1 && !source.series)
                        Program.win.firstpanel.SourcesToShow.Add(source);
                    }
                }
            }

            Program.win.secondpanel.SourcesToShow.Clear();
            if (Program.win.DeepSource != null && !Program.win.DeepSource.Null)
            {
                Program.win.secondpanel.SourcesToShow.Add(Program.win.DeepSource);
            }
        }

        static Thread research;
        public static void ResearchAsync()
        {


            if (research != null) research.Abort();
            research = new Thread(Research);
            research.Start();
        }

        static bool researching = false;
        //static string last_research = "";
        //static bool last_something_strange = false;
        public static void Research()
        {
            try
            {
                bool isNewResearch = ResearchClass.IsNewResearch();
                if (!isNewResearch) return;
                DisposeResearch();


                if (!ResearchClass.StrangeDeepResearch() && !ResearchClass.StrangeFirstResearch())
                {
                    researching = true;
                    Program.win.firstpanel.Refresh(true);
                    Program.win.Research_Categories.Clear();
                    Console.WriteLine("EndResearch");
                    researching = false;
                    Program.win.ricerca.FindingHide();
                    Program.EnableLoading(false);
                    Program.win.firstpanel.Refresh();
                    Program.win.firstpanel.Focus();
                    return;
                }
                Program.EnableLoading();
                Program.win.ricerca.FindingShow();

                researching = true;
                Program.win.firstpanel.Refresh(true);
                Program.win.Research_Categories.Clear();
                string name;
                int dim;
                Film film;

                foreach (Source source in Window.Sources)
                {
                    if (source.series) continue;
                    foreach (Iconxx icon in source.Icons())
                    {
                        if (icon.folder)
                        {
                            string[] directories = DirWithoutSeries(icon.fullpath);
                            if (directories.Length == 0) continue;

                            foreach (string dir in directories)
                            {
                                try
                                {
                                    //Console.WriteLine(dir);
                                    string[] subdirectories = DirWithoutSeries(dir);
                                    if (subdirectories.Length != 0)
                                    {
                                        foreach (string subdir in subdirectories)
                                        {
                                            try
                                            {
                                                string[] subsubdirectories = DirWithoutSeries(subdir);
                                                if (subsubdirectories.Length != 0)
                                                {
                                                    foreach (string subsubdir in subsubdirectories)
                                                    {
                                                        try
                                                        {
                                                            string[] subsubsubdirectories = DirWithoutSeries(subsubdir);
                                                            if (subsubsubdirectories.Length != 0)
                                                            {
                                                                foreach (string subsubsubdir in subsubsubdirectories) { if (!AddIconxx(subsubsubdir)) continue; }
                                                                //if (!AddIconxx(subdir)) continue;
                                                            }
                                                            else
                                                            {
                                                                AddIconxx(subsubdir);// if (!AddIconxx(subdir)) continue;
                                                            }
                                                        }
                                                        catch (Exception) { Console.WriteLine("Exception in Supervisor"); AddIconxx(subsubdir); }
                                                    }
                                                    //if (!AddIconxx(subdir)) continue;
                                                }
                                                else
                                                {
                                                    AddIconxx(subdir);// if (!AddIconxx(subdir)) continue;
                                                }
                                            }
                                            catch (Exception) { Console.WriteLine("Exception in Supervisor"); AddIconxx(subdir); }
                                        }
                                        //AddIconxx(dir);//if (!AddIconxx(dir)) continue;
                                    }
                                    else
                                    {
                                        AddIconxx(dir);// if (!AddIconxx(dir)) continue;
                                    }
                                }
                                catch(Exception) { Console.WriteLine("Exception in Supervisor"); AddIconxx(dir); }
                            }
                        }
                        else
                        {
                            name = icon.title;
                            film = icon.principal_film;
                            //if (!Compatible(name, text, film) && !TextNull(text, PanelResearch.null_values)) { continue; }
                            if (!Program.win.ricerca.Filter(film, icon.fullpath, ResearchClass.actualResearch)) { continue; }
                            dim = Window.standard.Height;
                            if (!forceDimIcon) dim = 0;
                            bool found = false;
                            foreach (Iconxx icon2 in Program.win.Research_Categories) if (icon.fullpath == icon2.fullpath) found = true;
                            if(!found) Program.win.Research_Categories.Add(icon);
                        }
                    }
                }
                if(Program.win.Research_Categories.Count==0) researchSource = new Source("La ricerca non ha prodotto risultati.", "La ricerca non ha prodotto risultati.", Program.win.Research_Categories);
                else
                {
                    foreach (Iconxx icon in Program.win.Research_Categories) icon.Check_Image();
                    Program.win.Research_Categories = Program.win.Research_Categories.OrderBy(o => o.title).ToList();
                    text = Program.win.hintText.Text;
                    string displayText = "Risultati per: " + text+" ";
                    if (TextNull(text, PanelResearch.null_values)) displayText = "Risultati ";
                    if (ResearchClass.StrangeDeepResearch()) displayText += "mediante filtro";
                    displayText += " (" + Program.win.Research_Categories.Count + ")";
                    researchSource = new Source(displayText, displayText, Program.win.Research_Categories);
                }
                icons_pending = true;
                researching = false;
                Program.win.firstpanel.Refresh();
                Console.WriteLine("EndResearch");
                Program.EnableLoading(false);
            }
            catch (Exception) { researching = false; Program.EnableLoading(false); Console.WriteLine("Exception from Research in Supervisor"); }
        }
        private static string[] DirWithoutSeries(string path)
        {
            try
            {
                string[] directories = Directory.GetDirectories(path);
                foreach (string direct in directories) if (direct.Contains("powervideos_series.txt")) return new string[] { };
                return directories;
            }
            catch (Exception) { return new string[] { }; }
        }
        private static bool AddIconxx(string dir)
        {
            try
            {
                string name = dir.Substring(1 + dir.LastIndexOf("/"));
                name = Iconxx.CleanName(name);
                Film film = Iconxx.GetPrincipalFilm(dir + @"\infopowervideos.txt");
                //if (!Compatible(name, text, film) && !TextNull(text, PanelResearch.null_values)) { return false; }
                if (!Program.win.ricerca.Filter(film, dir, ResearchClass.actualResearch)) { return false; }
                int dim = Window.standard.Height;
                if (!forceDimIcon) dim = 0;
                Iconxx iconxx = new Iconxx(Directory.GetParent(dir).FullName, dir, false, dim);
                bool found = false;
                foreach (Iconxx icon2 in Program.win.Research_Categories) if (iconxx.fullpath == icon2.fullpath) found = true;
                if (!found) Program.win.Research_Categories.Add(iconxx);
                return true;
            }
            catch (Exception) { Console.WriteLine("Exception in AddIconxx"); return false; }
        }
        public static bool TextNull(string text, string[] null_values)
        {
            foreach (string value in null_values) { if (text.Contains(value) && value != "") return true; if (text == "") return true; }
            return false;
        }

        public static bool Compatible(string A, string B)
        {
            if (A.ToLower().Contains(B.ToLower())) return true;
            return false;
        }

        public static void ResearchINFOS()
        {
            Console.WriteLine("Researching infos");
            Dictionary<string, int> infoList = new Dictionary<string, int>();
            foreach (Source source in Window.Sources)
            {
                if (source.series) continue;
                foreach (Iconxx icon in source.Icons())
                {
                    if (icon.folder)
                    {
                        string[] directories = DirWithoutSeries(icon.fullpath);
                        if (directories.Length == 0) continue;
                        foreach (string dir in directories)
                        {
                            try
                            {
                                string[] subdirectories = DirWithoutSeries(dir);
                                if (subdirectories.Length != 0)
                                {
                                    foreach (string subdir in subdirectories)
                                    {

                                        try
                                        {
                                            string[] subsubdirectories = DirWithoutSeries(subdir);
                                            if (subsubdirectories.Length != 0)
                                            {
                                                foreach (string subsubdir in subsubdirectories)
                                                {
                                                    try
                                                    {
                                                        string[] subsubsubdirectories = DirWithoutSeries(subsubdir);
                                                        if (subsubsubdirectories.Length != 0)
                                                        {
                                                            foreach (string subsubsubdir in subsubsubdirectories) { infoList[subsubsubdir] =4; }
                                                        }
                                                        else infoList[subsubdir] = 3; 
                                                    }
                                                    catch (Exception) { Console.WriteLine("Exception in Supervisor"); infoList[subsubdir] = 3; }
                                                }
                                            }
                                            else infoList[subdir] = 2; 
                                        }
                                        catch (Exception) { Console.WriteLine("Exception in Supervisor"); infoList[subdir] = 2; }
                                    }
                                }
                                else infoList[dir] = 1;
                            }
                            catch (Exception) { Console.WriteLine("Exception in Supervisor"); infoList[dir] = 1; }
                        }
                    }
                }
            }

            void dictAddGenres(Dictionary<List<string>, List<string>> dictionary, List<string> list, string title)
            {
                bool contains = false;
                List<string> key_ = new List<string>();
                foreach (var key in dictionary.Keys)
                {
                    if (key.Count != list.Count) continue;
                    contains = true;
                    for (int i = 0; i < key.Count; i++) if (key[i] != list[i]) contains = false;
                    if (contains) { key_ = key; break; }
                }
                if (contains) dictionary[key_].Add(title);
                else { dictionary[list] = new List<string>() { title, }; }
            }

            Dictionary<List<string>, List<string>> genreDistribution = new Dictionary<List<string>, List<string>>();
            foreach (var key in infoList.Keys)
            {
                try
                {
                    string filepath = Path.Combine(key, "infopowervideos.txt");
                    Film film;
                    if (File.Exists(filepath))
                    {
                        film = Iconxx.GetPrincipalFilm(filepath);
                        Iconxx.FillResearch(film);
                    }
                    else continue;
                    if (film!= null && film.genres.Count > 0) {
                        List<string> genres = new List<string>();
                        for (int i = 0; i < 2; i++) if (film.genres.Count - 1 >= i) genres.Add(film.genres[i]);
                        dictAddGenres(genreDistribution, genres, film.title);
                    }
                }
                catch (Exception) { }
            }


            List<List<string>> keysToRemove = new List<List<string>>();
            Dictionary<string, List<string>> Simple_genreDistribution = new Dictionary<string, List<string>>();
            Dictionary<List<string>, List<string>> newGenreDistribution = new Dictionary<List<string>, List<string>>();
            foreach (var key in genreDistribution.Keys)
            {
                if (!Simple_genreDistribution.Keys.Contains(key[0])) Simple_genreDistribution[key[0]] = new List<string>();
                Simple_genreDistribution[key[0]].AddRange(genreDistribution[key]);
            }
            foreach(string key in Simple_genreDistribution.Keys)
            {
                if(Simple_genreDistribution[key].Count<40)
                {
                    for(int i=genreDistribution.Count-1; i>=0; i--)
                    {
                        if (genreDistribution.Keys.ToArray()[i][0] == key) genreDistribution.Remove(genreDistribution.Keys.ToList()[i]);
                    }
                    newGenreDistribution[new List<string>() { key }] = Simple_genreDistribution[key];
                }
            }
            genreDistribution = genreDistribution.OrderBy(x => x.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            genreDistribution = genreDistribution.OrderBy(x => x.Key[0]).ToDictionary(x => x.Key, x => x.Value);
            List<string> onGenres = new List<string>();
            foreach (var key in genreDistribution.Keys) if (!onGenres.Contains(key[0])) onGenres.Add(key[0]);
            foreach(string key in onGenres)
            {
                int maxCount = 0;
                int count = 1;
                int lenght = 0;
                foreach(var keyList in genreDistribution.Keys)
                {
                    if(keyList[0] == key)
                    {
                        lenght++;
                        maxCount = genreDistribution[keyList].Count;
                    }
                }
                int cumulativeInt = 0;
                foreach (var keyList in genreDistribution.Keys)
                {
                    if (keyList[0] == key)
                    {
                        cumulativeInt += genreDistribution[keyList].Count;
                        if (cumulativeInt > maxCount) break;
                        else count++;
                    }
                }
                int iteration = 0;
                foreach (var keyList in genreDistribution.Keys)
                {
                    if (keyList[0] == key)
                    {
                        iteration++;
                        if (iteration > count) break;
                        keysToRemove.Add(keyList.ToList());
                        List<string> newKey = new List<string>() { key, "Various" };
                        List<string> oldKey = ListContains(newGenreDistribution.Keys.ToList(), newKey);
                        if (oldKey == null) { newGenreDistribution[newKey] = new List<string>(); oldKey = newKey; }
                        newGenreDistribution[oldKey].AddRange(genreDistribution[keyList]);
                    }
                }
            }
            for (int i = genreDistribution.Count - 1; i >= 0; i--)
            {
                if (ListContains(keysToRemove, genreDistribution.Keys.ToList()[i]) != null) genreDistribution.Remove(genreDistribution.Keys.ToList()[i]);
            }

            foreach(var key in genreDistribution.Keys)
            {
                newGenreDistribution[key] = genreDistribution[key];
            }

            List<string> ListContains(List<List<string>> container, List<string> list2)
            {
                bool output = false;
                foreach(List<string> list1 in container)
                {
                    if (list1.SequenceEqual(list2)) return list1;
                }
                return null;
            }
            
            List<string> prev_key = null;
            keysToRemove.Clear();
            genreDistribution.Clear();
            foreach (var key in newGenreDistribution.Keys)
            {
                if (key.Count == 1 && newGenreDistribution[key].Count<5)
                {
                    if (prev_key != null)
                    {
                        keysToRemove.Add(prev_key);
                        keysToRemove.Add(key);
                        List<string> newKey = new List<string>() { prev_key[0], key[0] };
                        genreDistribution[newKey] = new List<string>();
                        genreDistribution[newKey].AddRange(newGenreDistribution[prev_key]);
                        genreDistribution[newKey].AddRange(newGenreDistribution[key]);
                        prev_key = null;
                    }
                    else prev_key = key;
                }
            }
            foreach (var key in genreDistribution.Keys) newGenreDistribution[key] = genreDistribution[key];
            for (int i = newGenreDistribution.Count - 1; i >= 0; i--)
            {
                if (ListContains(keysToRemove, newGenreDistribution.Keys.ToList()[i]) != null) newGenreDistribution.Remove(newGenreDistribution.Keys.ToList()[i]);
            }

            newGenreDistribution = newGenreDistribution.OrderBy(x => x.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            newGenreDistribution = newGenreDistribution.OrderBy(x => x.Key.Count).ToDictionary(x => x.Key, x => x.Value);
            foreach(var key in newGenreDistribution.Keys)
            {
                newGenreDistribution[key].Sort();
            }


            int sum = 0;
            foreach (var key in genreDistribution.Keys) sum += genreDistribution[key].Count;


            StructureAdvice.Add("RECOMMENDED GENRES                                                 FILMS");
            StructureAdvice.Add("");
            foreach (List<string> key in newGenreDistribution.Keys)
            {
                StructureAdvice.Add("______________________________________________________________");
                StructureAdvice.Add("");
                foreach (string value in key) StructureAdvice[StructureAdvice.Count -1] += value+" & ";
                StructureAdvice[StructureAdvice.Count - 1] = StructureAdvice[StructureAdvice.Count - 1].Substring(0,StructureAdvice[StructureAdvice.Count - 1].Length-2) + ":";
                foreach(string value in newGenreDistribution[key])
                    StructureAdvice.Add("                                                                                               " + value);
            }
            StructureAdvice.Add("______________________________________________________________");

        }

    }
}
