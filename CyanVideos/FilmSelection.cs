using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public partial class FilmSelection : Form
    {
        public static Form actual_form;
        private Timer timer_check, timer_button;
        string name;
        string path;
        private Dictionary<int, Film> ids = new Dictionary<int, Film>();
        
        public Iconxx icon;
        public FilmSelection(Iconxx icon)
        {
            this.icon = icon;
            this.name = icon.title;
            this.path = icon.fullpath;
            actual_form = this;
            InitializeComponent();
            Text = "Selezione corrispondenza per - " + name + " -";

            timer_check = new Timer()
            {
                Enabled = true,
                Interval = 50,
            };
            timer_button = new Timer()
            {
                Enabled = true,
                Interval = 50,
            };
            timer_button.Tick += TimerButton;

            try
            {
                button2.Text = "Ricerca approfondita";
                icon.sec_films = icon.sec_films.OrderBy(o => o.runtime).ToList();
                icon.sec_films = icon.sec_films.OrderBy(o => o.release_date).ToList();
                icon.sec_films = icon.sec_films.OrderBy(o => o.revenue).ToList();
                icon.sec_films.Reverse();

                if (icon.principal_film != null && icon.principal_film.title != "AsItIsPowerVideos" && icon.principal_film.title != "NotFoundPowerVideos")
                {
                    ids.Add(checkedListBox1.Items.Count, icon.principal_film);
                    checkedListBox1.Items.Add(icon.principal_film.verbose());
                }
                if (icon.sec_films.Count == 0) return;
                for (int i = 0; i < icon.sec_films.Count; i++)
                {
                    if (icon.sec_films[i].title != "" && icon.sec_films[i].title != "AsItIsPowerVideos" && icon.sec_films[i].title != "NotFoundPowerVideos")
                    {
                        ids.Add(checkedListBox1.Items.Count, icon.sec_films[i]);
                        checkedListBox1.Items.Add(icon.sec_films[i].verbose());
                    }
                }
            }
            catch (Exception) { Close(); }
        }

        private void TimerButton(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBox1.CheckedItems.Count > 0) button1.Enabled = true;
                else button1.Enabled = false;
            }
            catch (Exception) { }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                if (!checkedListBox1.CheckedItems.Contains(checkedListBox1.Items[e.Index]))
                {
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }
            }
            catch (Exception) { }
        }

        int prev_count = -1;
        int iterations = 0;
        //bool running = true;
        private void Check(object sender, EventArgs e)
        {
            iterations++;
            if (iterations > 10 && label2.Text != "Nessun risultato") { label2.ForeColor = Color.Blue; label2.Text = "Sto scaricando.."; }
            try
            {
                if (icon.sec_films.Count == prev_count)
                {
                    if (imdb != null)
                        if (!imdb.running)
                        {
                            button2.Text = "Ricerca completata";
                            if (checkedListBox1.Items.Count == 0) { label2.ForeColor = Color.Red; label2.Text = "Nessun risultato"; }
                            else label2.Text = "";
                            timer_check.Tick -= Check;
                        }
                    return;
                }
                else
                {
                    prev_count = icon.sec_films.Count;
                    checkedListBox1.Items.Clear();
                    ids.Clear();
                    icon.sec_films = icon.sec_films.OrderBy(o => o.runtime).ToList();
                    icon.sec_films = icon.sec_films.OrderBy(o => o.release_date).ToList();
                    icon.sec_films = icon.sec_films.OrderBy(o => o.revenue).ToList();
                    icon.sec_films.Reverse();


                    string verbose;
                    if (icon.principal_film != null)
                    {
                        verbose = icon.principal_film.verbose();
                        if (icon.principal_film.title != "" && icon.principal_film.title != "AsItIsPowerVideos"
                            && icon.principal_film.title != "NotFoundPowerVideos")
                        {
                            if (!checkedListBox1.Items.Contains(verbose))
                            {
                                ids.Add(checkedListBox1.Items.Count, icon.principal_film);
                                checkedListBox1.Items.Add(verbose);
                            }
                        }
                    }

                    for (int i = 0; i < icon.sec_films.Count; i++)
                    {
                        if (icon.sec_films[i].title == "" || icon.sec_films[i].title == "AsItIsPowerVideos" || icon.sec_films[i].title == "NotFoundPowerVideos") continue;
                        verbose = icon.sec_films[i].verbose();
                        if (!checkedListBox1.Items.Contains(verbose))
                        {
                            ids.Add(checkedListBox1.Items.Count, icon.sec_films[i]);
                            checkedListBox1.Items.Add(verbose);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Eccezione da TimerSelezione: " + ex.Message); }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                Console.WriteLine("Choosen: " + ids[checkedListBox1.CheckedIndices[0]].verbose());
                icon.AddS(icon.principal_film);
                icon.SetP(ids[checkedListBox1.CheckedIndices[0]]);
                //icon.HideE();                  ci vuole la reliability qui!!
                //icon.BackgroundImage = Properties.Resources._null;
                icon.reliability = 4;
                //Program.win.firstpanel.Refresh(true);

                timer_check.Dispose();


                if (System.IO.File.Exists(path + @"\imagefromPowerVideos.jpg"))
                {
                    System.IO.File.Delete(path + @"\imagefromPowerVideos.jpg");
                    foreach (string image in Program.GetAllImages(System.IO.Directory.GetFiles(path))) System.IO.File.Delete(image);
                    Console.WriteLine("All images for " + path + " deleted");
                }
            }
            catch (Exception) { }

            Close();
        }

        IMDB imdb;
        private void button2_Click(object sender, EventArgs e)
        {
            ids.Clear();
            checkedListBox1.Items.Clear();
            imdb = new IMDB(icon, true);
            imdb.running = true;
            button2.Enabled = false;
            button2.Text = "Aspetta..";
            label2.Text = "";
            timer_check.Tick += Check;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
            try
            {
                if (!icon.sec_films.Contains(icon.principal_film)) icon.AddS(icon.principal_film);
                Film film_null = new Film() { title = "AsItIsPowerVideos" };
                icon.SetP(film_null);
                if (System.IO.File.Exists(icon.fullpath + @"\imagefromPowerVideos.jpg"))
                    System.IO.File.Delete(icon.fullpath + @"\imagefromPowerVideos.jpg");
            }
            catch (Exception) { }
        }


    }
}
