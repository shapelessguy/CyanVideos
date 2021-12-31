using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;

namespace CyanVideos
{
    class IMDB
    {
        static public List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
        string name;
        string path;
        public bool running = true;
        bool deeply = false;
        List<Film> films = new List<Film>();
        System.Threading.Thread findFilms;
        Iconxx icon;
        bool series = false;

        public IMDB(Iconxx icon, bool deeply = false)
        {
            this.icon = icon;
            this.deeply = deeply;
            this.series = icon.series;
            if (!deeply) findFilms = new System.Threading.Thread(FindFilms);
            else findFilms = new System.Threading.Thread(FindFilmsDeeply);
            threads.Add(findFilms);
            this.name = icon.title;
            this.path = icon.fullpath;
            findFilms.Start();
            return;

        }
        public IMDB(Iconxx icon)
        {
            this.icon = icon;
            this.name = icon.title;
            this.series = icon.series;
            FindFilms();
        }

        public void FindFilms()
        {
            running = true;
            List<Film> films = GetFilms_byName(name);
            try
            {
                #region New logic

                int considered = 0;   //numero di elementi strettamente uguali
                foreach (Film film in films)
                {
                    if (Regex.Replace(film.title.ToLower(), @"[^\w]", "") == Regex.Replace(name.ToLower(), @"[^\w]", ""))
                    {
                        considered++;
                    }
                }
                Console.WriteLine("Strong results for " + name + ": " + considered);



                if (films == null || films.Count == 0)
                {
                    if (icon.principal_film == null && icon.sec_films.Count == 0)
                    {
                        Film film = new Film() { title = "NotFoundPowerVideos", };
                        icon.SetP(film);
                    }
                }
                else if (films.Count == 1)
                {
                    films[0].FindDetails();
                    if (icon.principal_film != films[0]) icon.AddS(films[0]);
                }
                else
                {
                    if (considered == 1)
                    {
                        foreach (Film film in films)
                        {
                            if (Regex.Replace(film.title.ToLower(), @"[^\w]", "") == Regex.Replace(name.ToLower(), @"[^\w]", ""))
                            {
                                film.FindDetails();
                                if (icon.principal_film != film) icon.AddS(film);
                            }
                        }
                    }
                    else if (considered == 0)
                    {
                        int num = 0;
                        foreach (Film film in films)
                        {
                            if (num > 10) break;
                            num++;
                            film.FindDetails();
                            if (icon.principal_film != film) icon.AddS(film);
                        }
                    }
                    else
                    {
                        int num = 0;
                        foreach (Film film in films)
                        {
                            if (Regex.Replace(film.title.ToLower(), @"[^\w]", "") == Regex.Replace(name.ToLower(), @"[^\w]", ""))
                            {
                                if (num > 10) break;
                                num++;
                                film.FindDetails();
                                if (icon.principal_film != film) icon.AddS(film);
                            }
                        }
                    }
                }
                threads.Remove(findFilms); running = false;
                Console.WriteLine("Films found");
                return;
                #endregion
            }
            catch (Exception) { running = false; return; }
        }


        private void FindFilmsDeeply()
        {
            running = true;
            try
            {
                List<Film> films = GetFilms_byName(name, true);
                if (films == null || films.Count == 0)
                {
                    threads.Remove(findFilms);
                    running = false;
                    return;
                }
                foreach (Film film in films)
                {
                    film.FindDetails();
                }
                foreach (Film film in films)
                {
                    if (!icon.sec_films.Contains(film) && icon.principal_film != film) icon.AddS(film);
                }
                threads.Remove(findFilms);
            }
            catch (Exception) { running = false; }
            running = false;
        }
        public List<Film> GetFilms_byName(string name, bool deeply = false)
        {
            string text = PutQuery(name, deeply);
            if (text == "") return null;
            string[] strings = text.Split(',');
            if (series) return GetSeriesAttributes(strings);
            else return GetAttributes(strings);
        }

        private string PutQuery(string name, bool deeply = false)
        {
            string req = "";
            try
            {
                string text = "";
                string[] components = name.Split(' ');
                string query_name = "";
                foreach (string stringa in components) query_name += "+" + stringa;
                query_name = query_name.Substring(1);

                int page_int = 1;
                string page = Convert.ToString(page_int);
                string field = "movie"; if (series) field = "tv";
                req = "https://api.themoviedb.org/3/search/" + field + "?api_key=92876a1248b045812aa538bce52d53c8&query=" +
                    query_name + "&page=" + page;
                //Console.WriteLine(req);
                WebRequest request = WebRequest.Create(req);

                WebResponse response = request.GetResponse();
                using (System.IO.Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.  
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    // Read the content.  
                    string responseFromServer = reader.ReadToEnd();
                    // Display the content.  
                    text += responseFromServer;
                    string[] fragments = text.Split(',');
                    string pages_txt = "";
                    foreach (string fragm in fragments)
                    {
                        string p_word = "\"total_pages\":";
                        if (fragm.Contains(p_word)) pages_txt = 
                                fragm.Substring(fragm.IndexOf(p_word)+p_word.Length);
                    }
                    //Console.WriteLine(pages_txt);
                    // string total_pages_int = total_pages.Substring(total_pages.IndexOf("\"total_pages\":") + 1);
                    int total = Convert.ToInt32(pages_txt);
                    if (total > 5 && !deeply) total = 5;
                    if (pages_txt != "1") text += MultipleQueries(query_name, total);
                }
                //System.IO.File.WriteAllText("C://Users//Claudio//Desktop//" + query_name.Replace("+", " ") + ".txt", text);
                return text;
            }
            catch (Exception e) { Console.WriteLine("Exception in PutQuery. Query: " + req + " - " + e.Message); return ""; }
        }

        private string MultipleQueries(string query_name, int num_pages)
        {
            string text = "";
            for (int i = 2; i <= num_pages; i++)
            {
                string page = Convert.ToString(i);
                string field = "movie"; if (series) field = "tv";
                WebRequest request = WebRequest.Create(
                    "https://api.themoviedb.org/3/search/" + field + "?" +
                    "api_key=92876a1248b045812aa538bce52d53c8&query=" + query_name + "&page=" + page);

                WebResponse response = request.GetResponse();
                using (System.IO.Stream dataStream = response.GetResponseStream())
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    text += responseFromServer;
                }
            }
            return text;
        }

        private List<Film> GetAttributes(string[] strings)
        {
            List<Film> filmList = new List<Film>();
            for (int i = 0; i < strings.Length; i++)
            {
                string stringa = strings[i].Replace("{", "").Replace("}", "");
                //Console.WriteLine(stringa);
                if (stringa.Contains("\"backdrop_path\":"))
                {
                    filmList.Add(new Film());
                    filmList[filmList.Count - 1].series = this.series;
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 3);
                    if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                    if (value.Length > 0) if (value.Substring(value.Length - 1, 1) == "\"") value = value.Substring(0, value.Length - 1);
                    value = value.Replace(@"\/", "https://image.tmdb.org/t/p/original//");
                    if (value.Length > 10) filmList[filmList.Count - 1].backdrop_path = value;
                }
                if (stringa.Contains("\"popularity\":"))
                {
                    //Console.WriteLine();
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].popularity = value;
                }
                if (stringa.Contains("\"overview\":"))
                {
                    string content = stringa.Substring(stringa.IndexOf(":") + 2);
                    bool salta = false;
                    if (content.Substring(content.Length - 1) == "\"")
                    {
                        filmList[filmList.Count - 1].overview = content.Substring(0, content.Length - 1);
                        salta = true;
                    }
                    if (!salta)
                    {
                        for (int j = i + 1; ; j++)
                        {
                            content += "," + strings[j];
                            if (strings[j].Contains("\""))
                            {
                                filmList[filmList.Count - 1].overview = content.Substring(0, content.Length - 1);
                                i = j; break;
                            }
                        }
                    }
                    if (filmList[filmList.Count - 1].overview.Length > 0)
                        if (filmList[filmList.Count - 1].overview.Substring(filmList[filmList.Count - 1].overview.Length - 1, 1) == @"\")
                            filmList[filmList.Count - 1].overview = filmList[filmList.Count - 1].overview.Substring(0, filmList[filmList.Count - 1].overview.Length - 1) + "\"";
                    filmList[filmList.Count - 1].overview = filmList[filmList.Count - 1].overview.Replace("\\\"", "\"");
                    filmList[filmList.Count - 1].overview = filmList[filmList.Count - 1].overview.Replace("\\/", "/");
                }
                if (stringa.Contains("\"title\":") || stringa.Contains("\"name\":"))
                {
                    string content = stringa.Substring(stringa.IndexOf(":") + 2);
                    if (content.Substring(content.Length - 1) == "\"")
                    {
                        filmList[filmList.Count - 1].title = content.Substring(0, content.Length - 1);
                        continue;
                    }
                    for (int j = i + 1; ; j++)
                    {
                        content += "," + strings[j];
                        if (strings[j].Contains("\""))
                        {
                            filmList[filmList.Count - 1].title = content.Substring(0, content.Length - 1);
                            i = j; break;
                        }
                    }
                }
                if (stringa.Contains("\"id\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].id = value;
                }
                if (stringa.Contains("\"vote_average\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].vote_average = value;
                }
                if (stringa.Contains("\"vote_count\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].vote_count = value;
                }
                if (stringa.Contains("\"poster_path\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 3);
                    if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                    if (value.Length > 0) if (value.Substring(value.Length - 1, 1) == "\"") value = value.Substring(0, value.Length - 1);
                    value = value.Replace(@"\/", "https://image.tmdb.org/t/p/original//");
                    if (value.Length > 10) filmList[filmList.Count - 1].poster_path = value;
                }
                if (stringa.Contains("\"release_date\":") || stringa.Contains("\"first_air_date\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 3);
                    value = value.Replace("[", "").Replace("]", "");
                    if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                    value = value.Replace(@"\/", "");
                    if (value.Length == 10) filmList[filmList.Count - 1].release_date = value;
                }
                if (stringa.Contains("\"original_language\":"))
                {
                    string value = stringa.Substring(stringa.IndexOf("\":\"") + 3);
                    value = value.Substring(0, value.Length - 1);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].original_language = value;
                }
                if (stringa.Contains("\"genre_ids\":"))
                {
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j];
                        string[] multiple_strings = stringa.Split(new char[] { '[', ']' });
                        foreach (string multiple_string in multiple_strings)
                        {
                            if (Int32.TryParse(multiple_string, out i))
                            {
                                if (!filmList[filmList.Count - 1].genres_id.Contains(multiple_string))
                                    filmList[filmList.Count - 1].genres_id.Add(multiple_string);
                            }

                        }
                        if (stringa.Contains("]")) { i = j; break; }
                    }
                }
                //if(filmList.Count>0) filmList[filmList.Count - 1].printFilm();
            }

            return filmList;
        }


        private List<Film> GetSeriesAttributes(string[] strings)
        {
            List<Film> filmList = new List<Film>();
            for (int i = 0; i < strings.Length; i++)
            {
                string stringa = strings[i].Replace("{", "").Replace("}", "");
                //Console.WriteLine(stringa);
                if (stringa.Contains("\"backdrop_path\":"))
                {
                    filmList.Add(new Film());
                    filmList[filmList.Count - 1].series = this.series;
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 3);
                    if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                    if (value.Length > 0) if (value.Substring(value.Length - 1, 1) == "\"") value = value.Substring(0, value.Length - 1);
                    value = value.Replace(@"\/", "https://image.tmdb.org/t/p/original//");
                    if (value.Length > 10) filmList[filmList.Count - 1].backdrop_path = value;
                }
                if (stringa.Contains("\"genre_ids\":"))
                {
                    for (int j = i; ; j++)
                    {
                        stringa = strings[j];
                        string[] multiple_strings = stringa.Split(new char[] { '[', ']' });
                        foreach (string multiple_string in multiple_strings)
                        {
                            if (Int32.TryParse(multiple_string, out i))
                            {
                                if (!filmList[filmList.Count - 1].genres_id.Contains(multiple_string))
                                    filmList[filmList.Count - 1].genres_id.Add(multiple_string);
                            }

                        }
                        if (stringa.Contains("]")) { i = j; break; }
                    }
                }
                if (stringa.Contains("\"name\":"))
                {
                    string content = stringa.Substring(stringa.IndexOf(":") + 2);
                    if (content.Substring(content.Length - 1) == "\"")
                    {
                        filmList[filmList.Count - 1].title = content.Substring(0, content.Length - 1);
                        continue;
                    }
                    for (int j = i + 1; ; j++)
                    {
                        content += "," + strings[j];
                        if (strings[j].Contains("\""))
                        {
                            filmList[filmList.Count - 1].title = content.Substring(0, content.Length - 1);
                            i = j; break;
                        }
                    }
                }
                if (stringa.Contains("\"popularity\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].popularity = value;
                }
                if (stringa.Contains("\"vote_count\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].vote_count = value;
                }
                if (stringa.Contains("\"first_air_date\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 3);
                    value = value.Replace("[", "").Replace("]", "");
                    if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                    value = value.Replace(@"\/", "");
                    if (value.Length == 10) filmList[filmList.Count - 1].release_date = value;
                }
                if (stringa.Contains("\"original_language\":"))
                {
                    string value = stringa.Substring(stringa.IndexOf("\":\"") + 3);
                    value = value.Substring(0, value.Length - 1);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].original_language = value;
                }
                if (stringa.Contains("\"id\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].id = value;
                }
                if (stringa.Contains("\"vote_average\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 2);
                    value = value.Split(new string[] { @"\/" }, StringSplitOptions.None)[0];
                    filmList[filmList.Count - 1].vote_average = value;
                }
                if (stringa.Contains("\"overview\":"))
                {
                    string content = stringa.Substring(stringa.IndexOf(":") + 2);
                    bool salta = false;
                    if (content.Substring(content.Length - 1) == "\"")
                    {
                        filmList[filmList.Count - 1].overview = content.Substring(0, content.Length - 1);
                        salta = true;
                    }
                    if (!salta)
                    {
                        for (int j = i + 1; ; j++)
                        {
                            content += "," + strings[j];
                            if (strings[j].Contains("\""))
                            {
                                filmList[filmList.Count - 1].overview = content.Substring(0, content.Length - 1);
                                i = j; break;
                            }
                        }
                    }
                    if (filmList[filmList.Count - 1].overview.Length > 0)
                        if (filmList[filmList.Count - 1].overview.Substring(filmList[filmList.Count - 1].overview.Length - 1, 1) == @"\")
                            filmList[filmList.Count - 1].overview = filmList[filmList.Count - 1].overview.Substring(0, filmList[filmList.Count - 1].overview.Length - 1) + "\"";
                    filmList[filmList.Count - 1].overview = filmList[filmList.Count - 1].overview.Replace("\\\"", "\"");
                    filmList[filmList.Count - 1].overview = filmList[filmList.Count - 1].overview.Replace("\\/", "/");
                }
                if (stringa.Contains("\"poster_path\":"))
                {
                    string value = stringa.Substring(stringa.LastIndexOf("\":") + 3);
                    if (value.Length > 0) value = value.Substring(0, value.Length - 1);
                    if (value.Length > 0) if (value.Substring(value.Length - 1, 1) == "\"") value = value.Substring(0, value.Length - 1);
                    value = value.Replace(@"\/", "https://image.tmdb.org/t/p/original//");
                    if (value.Length > 10) filmList[filmList.Count - 1].poster_path = value;
                }
                //if(filmList.Count>0) filmList[filmList.Count - 1].printFilm();
            }

            return filmList;
        }



    }

}
