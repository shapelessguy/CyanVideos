using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class Film
    {
        public string popularity = "";
        public string poster_path = "";
        public string backdrop_path = "";
        public string id = "";
        public string original_language = "";
        public string vote_average = "";
        public string vote_count = "";
        public string release_date = "";
        public string title = "";
        public int runtime = 0;
        public string overview = "";
        public long revenue = 0;
        public List<string> genres_id = new List<string>();
        public List<string> production_companies = new List<string>();
        public List<string> genres = new List<string>();
        public List<Casting> lcast = new List<Casting>();
        public List<Credits> lcredits = new List<Credits>();
        public bool complete = false;
        public bool changed = false;
        public bool series = false;

        System.Threading.Thread findDetails, findImage;

        public bool downloading_first = false;
        public int tentatives_first = 0, maxtentatives_first = 2;
        public bool download_first_complete = false;

        public bool downloading_castInfo = false;
        public int tentatives_castInfo = 0, maxtentatives_castInfo = 2;
        public bool download_castInfo_complete = false;

        public bool downloading_image = false;
        public int tentatives_image = 0, maxtentatives_image = 2;
        public bool download_image_complete = false;

        public bool downloading_backdrop = false;
        public int tentatives_backdrop = 0, maxtentatives_backdrop = 2;
        public bool download_backdrop_complete = false;



        private string dest_path;
        private string back_path;
        public void Disposer()
        {
            genres.Clear();
            genres_id.Clear();
            production_companies.Clear();
            foreach (Casting cast in lcast) cast.Disposer();
            foreach (Credits credit in lcredits) credit.Disposer();
            lcast.Clear();
            lcredits.Clear();
        }
        public Film() { }

        public void FindDetails_Thread()
        {
            findDetails = new System.Threading.Thread(FindCastDetails);
            findDetails.Start();
            return;
        }
        public void FindImage_Thread(string dest_path, Iconxx icon = null)
        {
            this.dest_path = dest_path;
            if (icon != null) control_to = icon; else control_to = null;
            findImage = new System.Threading.Thread(FindImage);
            findImage.Start();
            return;
        }
        string textToUpdate = "";
        public void FindBackImage_Thread(string dest_path, Iconxx icon = null, string textToUpdate = "")
        {
            this.textToUpdate = textToUpdate;
            this.back_path = dest_path;
            if (icon != null) control_to = icon; else control_to = null;
            findImage = new System.Threading.Thread(FindBackImage);
            findImage.Start();
            return;
        }

        Iconxx control_to;
        public void FindDetails()
        {
            if (tentatives_first > maxtentatives_first) return;
            downloading_first = true;
            tentatives_first++;
            string field = "movie/"; if (series) field = "tv/";
            string req = "https://api.themoviedb.org/3/"+field + id + "?api_key=92876a1248b045812aa538bce52d53c8";
            try
            {
                System.Net.WebRequest request = WebRequest.Create(req);

                WebResponse response = request.GetResponse();
                using (System.IO.Stream dataStream = response.GetResponseStream())
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    GetAttributes(responseFromServer);
                }
                downloading_first = false;
            }
            catch (Exception e) { Console.WriteLine("Exception in FindDetails. Query: " + req + " - " + e.Message); downloading_first = false; }

        }
        private void GetAttributes(string text)
        {
            Console.WriteLine("Getting Attributes from "+this.title);
            string[] strings = text.Split(',');

            bool rightBackDrop_found = false;
            for (int i = 0; i < strings.Length; i++)
            {
                string stringa = strings[i];
                //Console.WriteLine(stringa);
                if (stringa.Contains("\"production_companies\":"))
                {
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j].Replace("}", "").Replace("{", "");
                        string[] multiple_strings = stringa.Split(new char[] { '[', ']' });
                        foreach (string multiple_string in multiple_strings)
                        {
                            if (multiple_string.Contains("\"name\":"))
                            {
                                string value = multiple_string.Substring(multiple_string.LastIndexOf("\":") + 3);
                                if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                                value = value.Replace(@"\/", "");
                                production_companies.Add(value);
                            }
                        }
                        if (stringa.Contains("]")) { i = j; break; }
                    }
                }

                if (stringa.Contains("\"genres\":"))
                {
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j].Replace("}", "").Replace("{", "");
                        string[] multiple_strings = stringa.Split(new char[] { '[', ']' });
                        foreach (string multiple_string in multiple_strings)
                        {
                            if (multiple_string.Contains("\"name\":"))
                            {
                                string value = multiple_string.Substring(multiple_string.LastIndexOf("\":") + 3);
                                if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                                value = value.Replace(@"\/", "");
                                genres.Add(value);
                            }
                        }
                        if (stringa.Contains("]")) { i = j; break; }
                    }
                }

                if (stringa.Contains("\"backdrop_path\":\"") && stringa.Contains("}"))
                {
                    rightBackDrop_found = true;
                    string value = stringa.Substring(stringa.LastIndexOf("\":\"") + 3);
                    value = value.Substring(0, value.Length - 2);
                    backdrop_path = "https://image.tmdb.org/t/p/original" + value;
                }
                if (stringa.Contains("\"poster_path\":\""))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":\"") + 3);
                    value = value.Substring(0, value.Length - 1);
                    poster_path = "https://image.tmdb.org/t/p/original" + value;
                }
                if (stringa.Contains("\"runtime\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    int value_run = 0;
                    Int32.TryParse(value, out value_run);
                    runtime = value_run;
                }
                if (stringa.Contains("\"revenue\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    long value_rev = 0;
                    Int64.TryParse(value, out value_rev);
                    revenue = value_rev;
                }
            }
            if (!rightBackDrop_found)
            {
                for (int i = 0; i < strings.Length; i++)
                {
                    string stringa = strings[i];
                    if (stringa.Contains("\"backdrop_path\":\""))
                    {
                        rightBackDrop_found = true;
                        string value = stringa.Substring(stringa.LastIndexOf("\":\"") + 3);
                        value = value.Substring(0, value.Length - 1);
                        backdrop_path = "https://image.tmdb.org/t/p/original" + value;
                    }
                }
            }
        }
        private void GetCastAttributes(string text)
        {
            string[] strings = text.Split(',');
            //foreach(var s in strings) { Console.WriteLine(s); }

            for (int i = 0; i < strings.Length; i++)
            {
                string stringa = strings[i];
                if (stringa.Contains("\"known_for_department\"") && stringa.Contains(":\"Acting\""))
                {
                    Casting cast = new Casting();
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j];
                        if (stringa.Contains("\"character\":\""))
                        {
                            cast.character = stringa.Substring(13, stringa.Length - 14);
                        }
                        if (stringa.Contains("\"name\":\""))
                        {
                            cast.name = stringa.Substring(8, stringa.Length - 9);
                        }
                        if (stringa.Contains("}")) { lcast.Add(cast); i = j; break; }
                    }
                }
                if (stringa.Contains("\"known_for_department\"") && !stringa.Contains(":\"Acting\""))
                {
                    Credits credits = new Credits();
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j];
                        if (stringa.Contains("\"department\":\""))
                        {
                            credits.department = stringa.Substring(14, stringa.Length - 15);
                            credits.department = credits.department.Replace("\\u0026", "&");
                        }
                        if (stringa.Contains("\"name\":\""))
                        {
                            credits.name = stringa.Substring(8, stringa.Length - 9);
                        }
                        if (stringa.Contains("\"job\":\""))
                        {
                            string nstringa = stringa.Replace("}", "").Replace("]", "");
                            credits.job = nstringa.Substring(7, nstringa.Length - 9);
                        }
                        if (stringa.Contains("}")) { if (credits.department != "Actors") lcredits.Add(credits); i = j; break; }
                    }
                }
            }
        }
        private void GetSeriesCastAttributes(string text)
        {
            List<Film> filmList = new List<Film>();
            string[] strings = text.Split(',');
            foreach (var s in strings) Console.WriteLine(s);
            for (int i = 0; i < strings.Length; i++)
            {
                string stringa = strings[i];
                if (stringa.Contains("\"adult\":"))
                {

                    Credits credits = new Credits();
                    Casting cast = new Casting();
                    cast.character = "null";
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j];
                        //Console.WriteLine(stringa);
                        if (stringa.Contains("\"department\":\""))
                        {
                            credits.department = stringa.Substring(14, stringa.Length - 15);
                            credits.department = credits.department.Replace("}", "").Replace("]", "");
                            credits.department = credits.department.Replace("\\u0026", "&");
                        }
                        if (stringa.Contains("\"name\":\""))
                        {
                            credits.name = stringa.Substring(8, stringa.Length - 9);
                            cast.name = stringa.Substring(8, stringa.Length - 9);
                        }
                        if (stringa.Contains("\"character\":\""))
                        {
                            int ind_character = stringa.IndexOf("character\"");
                            cast.character = stringa.Substring(12 + ind_character, stringa.Length - 13 - ind_character);
                        }
                        if (stringa.Contains("\"job\":\""))
                        {
                            string nstringa = stringa.Replace("}", "").Replace("]", "");
                            Console.WriteLine(nstringa);
                            credits.job = nstringa.Substring(7, nstringa.Length - 8);
                            credits.job = credits.job.Replace("\\u0026", "&");
                        }
                        if (stringa.Contains("}"))
                        {
                            if (cast.character != "null") {lcast.Add(cast); }
                            else if (credits.department != "Actors") { lcredits.Add(credits); }
                            i = j; break; }
                    }
                }
            }
        }

        public void FindCastDetails()
        {
            if (tentatives_castInfo > maxtentatives_castInfo) return;
            string field = "movie/"; if (series) field = "tv/";
            string req = "https://api.themoviedb.org/3/" + field + id + "/credits?api_key=92876a1248b045812aa538bce52d53c8";
            try
            {
                downloading_castInfo = true;
                tentatives_castInfo++;
                System.Net.WebRequest request = WebRequest.Create(req);

                WebResponse response = request.GetResponse();
                using (System.IO.Stream dataStream = response.GetResponseStream())
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    if(series) GetSeriesCastAttributes(responseFromServer);
                    else GetCastAttributes(responseFromServer);
                }
                downloading_castInfo = false;
                complete = true;
                changed = true;
            }
            catch (Exception e) { Console.WriteLine("Exception in FindAllDetails. Query: " + req + " - " + e.Message); downloading_castInfo = false; }
        }
        public void FindImage()
        {
            if (tentatives_image > maxtentatives_image) return;
            string req = "";
            try
            {
                Directory.GetFiles(dest_path);
                downloading_image = true;
                tentatives_image++;
                if (poster_path == "") return;
                string req_path = dest_path + @"\imagefromPowerVideos.jpg";
                if (System.IO.File.Exists(req_path)) System.IO.File.Delete(req_path);
                using (WebClient client = new WebClient())
                {
                    req = poster_path;
                    client.DownloadFile(new Uri(req), req_path);
                }
                Program.Compress(req_path);

                if (control_to != null)
                {
                    control_to.Check_Image(true);
                }
                downloading_image = false;
                download_image_complete = true;
                if (backdrop_path == "") {  return; }
                FindBackImage_Thread(System.IO.Directory.GetParent(dest_path).FullName + @"\imagefromPowerVideos.jpg");
            }
            catch (Exception e) { Console.WriteLine("Exception in FindImage. Query: " + req + " - " + e.Message); downloading_image = false; }
        }

        int attempt_download = 0;
        public void FindBackImage()
        {
            //Console.WriteLine(back_path);
            foreach (Source source in Window.Sources) if (back_path == source.directory) return;
            if (tentatives_backdrop > maxtentatives_backdrop) return;
            string req = "";
            try
            {
                Directory.GetFiles(Directory.GetParent(back_path).FullName);
                downloading_backdrop = true;
                tentatives_backdrop++;
                string req_path = "";
                req_path = back_path;
                if (System.IO.File.Exists(req_path)) System.IO.File.Delete(req_path);
                using (WebClient client = new WebClient())
                {
                    while (attempt_download<2)
                    {
                        req = backdrop_path;
                        FileInfo file = null;
                        try
                        {
                            client.DownloadFile(new Uri(req), req_path);
                            file = new FileInfo(req_path);
                        }
                        catch (Exception) { }
                        Console.WriteLine("Attempt "+attempt_download+": "+req);
                        if (file == null || file.Length < 5000) {
                            if (attempt_download == 0)
                                try {
                                    string prev_backdrop_path = backdrop_path;
                                    FindDetails();
                                    if(prev_backdrop_path != backdrop_path) UpdatePrincipalFilm(textToUpdate);
                                }
                                catch (Exception) { }
                            attempt_download++;
                            File.Delete(req_path); }
                        else { break; }
                    }
                }
                downloading_backdrop = false;
                Program.Compress(req_path, "", (int)(Program.defaultIconHeight * Window.prop_width_height) * 2 + Window.intraDistanceX, Program.defaultIconHeight);
                if (control_to != null)
                {
                    control_to.Check_Image(true);
                }
            }
            catch (Exception e) { Console.WriteLine("Exception in FindBackImage. Query: " + req + " - " + e.Message); downloading_backdrop = false; }
        }

        public void printFilm()
        {
            Console.WriteLine("Title: " + title);
            Console.WriteLine("ID: " + id);
            Console.WriteLine("Language: " + original_language);
            Console.WriteLine("Runtime: " + runtime);
            Console.WriteLine("Vote count: " + vote_count);
            Console.WriteLine("Vote average: " + vote_average);
            Console.WriteLine("Poster: " + poster_path);
            Console.WriteLine("Backdrop: " + backdrop_path);

            string dollar = "";
            //if (revenue.Length > 0) dollar = "$";
            Console.WriteLine("Revenue: " + revenue + dollar);

            Console.WriteLine("Release date: " + release_date);

            Console.Write("Genres: ");
            string genre = "";
            foreach (string value in genres) genre += value + ", ";
            if (genres.Count > 0) genre = genre.Substring(0, genre.Length - 2);
            Console.WriteLine(genre);

            Console.Write("Production: ");
            string production = "";
            foreach (string value in production_companies) production += value + ", ";
            if (production_companies.Count > 0) production = production.Substring(0, production.Length - 2);
            Console.WriteLine(production);

            Console.WriteLine("Overview: " + overview);
        }

        public string serialize()
        {
            string data = "";
            data += "^|^"; data += title;
            data += "^|^"; data += runtime.ToString();
            data += "^|^"; data += popularity;
            data += "^|^"; data += id;
            data += "^|^"; data += original_language;
            data += "^|^"; data += vote_average;
            data += "^|^"; data += vote_count;
            data += "^|^"; data += poster_path;
            data += "^|^"; data += backdrop_path;
            data += "^|^"; data += release_date;
            data += "^|^"; data += overview;
            data += "^|^"; data += revenue.ToString();
            data += "^|^"; foreach (string genre in genres) { data += "_^_"; data += genre; }
            data += "^|^"; foreach (string company in production_companies) { data += "_^_"; data += company; }
            data += "^|^"; data += Convert.ToString(complete);
            if (lcast.Count > 0)
            {
                data += "^|^"; foreach (Casting cast in lcast)
                { data += cast.name + "_*_"; data += cast.character + "_*_"; data += "_^_"; }
            }
            if (lcredits.Count > 0)
            {
                data += "^|^"; foreach (Credits credit in lcredits)
                { data += credit.name + "_*_"; data += credit.department + "_*_"; data += credit.job + "_*_"; data += "_^_"; }
            }
            return data;
        }

        public string GetReleaseDate()
        {
            return release_date;
        }
        public string GetRevenue()
        {
            if (revenue != 0)
            {
                if (revenue > 999999) return Convert.ToString(revenue / 1000000) + "M$";
                else if (revenue > 999) return Convert.ToString(revenue / 1000) + "k$";
                else return Convert.ToString(revenue) + "$";
            }
            else return "//";
        }
        public int GetYear()
        {
            int date = 0;
            if(release_date.Length>=4) date = Convert.ToInt32(release_date.Substring(0, 4));
            return date;
        }

        public string verbose()
        {
            string text = ""; text += title;

            text += " -  Runtime: "; text += runtime.ToString();

            text += " -  Revenue: "; text += GetRevenue();

            if (release_date != "") text += " -  Release: "; text += release_date;

            if (popularity != "") text += ",  Pop: "; text += popularity;

            if (genres.Count > 0)
            {
                text += " -  Genres: ";
                for (int i = 0; i < genres.Count; i++) { text += genres[i]; if (i < genres.Count - 1) text += ", "; };
            }

            if (production_companies.Count > 0)
            {
                text += " -  Producers: ";
                for (int i = 0; i < production_companies.Count; i++)
                {
                    text += production_companies[i]; if (i < production_companies.Count - 1) text += ", ";
                };
            }

            if (overview.Length > 2) { text += " -  Plot: " + overview; }

            return text;
        }

        public void UpdatePrincipalFilm(string path)
        {
            Console.WriteLine("Saving Info for: " + path);
            string[] lines = File.ReadAllLines(path);
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("principal" + serialize());
                foreach(string stringa in lines)
                {
                    if (stringa.Substring(0, 9) != "principal") sw.WriteLine(stringa);
                }
            }
        }

        public bool InsertInCollection(List<Film> list)
        {
            //Console.WriteLine(this.verbose());
            if (list.Contains(this)) return false;

            foreach (Film element in list)
            {
                if (element.title != title) continue;
                if ((element.revenue == 0 && revenue != 0) || (element.revenue != 0 && revenue == 0)) continue;
                if (element.runtime != runtime) continue;
                if (element.overview != overview) continue;

                try
                {
                    if (element.popularity != popularity)
                    {
                        if (Convert.ToDouble(element.popularity) > Convert.ToDouble(popularity)) { list.Remove(element); list.Add(this); Console.WriteLine("                     " + this.verbose()); return true; }
                    }
                    else return false;
                }
                catch (Exception) { Console.WriteLine("Exception in InsertInCollection"); continue; }
            }

            list.Add(this);
            //Console.WriteLine("                     " + this.verbose());
            return true;
        }
    }

    public class Casting
    {
        public string name;
        public string character;
        public void Disposer()
        {

        }
        public Casting() { }
        public void print()
        {
            Console.WriteLine("Cast --->  Name: "+name+", character: "+character);
        }
    }
    public class Credits
    {
        public string name;
        public string department;
        public string job;
        public void Disposer()
        {

        }
        public Credits() { }
        public void print()
        {
            Console.WriteLine("Credits --->  Name: " + name + ", department: " + department+", job: "+ job);
        }
    }

}
