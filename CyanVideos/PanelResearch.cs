using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class PanelResearch : Panel
    {
        public static List<string> allGenres = new List<string>();
        public static List<string> allRoles = new List<string>();
        static public bool new_genre = false;
        static public bool new_role = false;
        private Timer timer, only_checked, time;
        public CheckedListBox listbox;
        public CheckedListBox rolebox;
        public TextBox hintText = new TextBox();
        static public string super_null_value = "...Search by...";
        static public string null_value = "...Search by...";
        static public string[] null_values;

        private Label Finding;
        Label DateLabel;
        Label RevenueLabel;
        public TextBox leftDate;
        public TextBox rightDate;
        public TextBox leftRevenue;
        public TextBox rightRevenue;
        public string stocercando = "Sto cercando..";

        //static public bool something_changed = false;
        static public bool something_strange = false;
        public PanelResearch()
        {
            SuspendLayout();
            BackColor = Color.Black;
            BorderStyle = BorderStyle.FixedSingle;
            Visible = false;
            Click += ClickNull;
            Size = new Size(0, 0);

            DateLabel = new Label()
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "\u2190 Intervallo date \u2192",
                Size = new Size(40, 40),
            };
            Controls.Add(DateLabel);
            RevenueLabel = new Label()
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "\u2190 Intervallo revenue ($) \u2192",
                Size = new Size(40, 40),
            };
            Controls.Add(RevenueLabel);
            leftDate = new TextBox
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                TabStop = false,
                TabIndex = 0,
                Text = "",
            };
            Controls.Add(leftDate);
            rightDate = new TextBox
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                TabStop = false,
                TabIndex = 0,
                Text = "",
            };
            Controls.Add(rightDate);
            leftRevenue = new TextBox
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                TabStop = false,
                TabIndex = 0,
                Text = "",
            };
            Controls.Add(leftRevenue);
            rightRevenue = new TextBox
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                TabStop = false,
                TabIndex = 0,
                Text = "",
            };
            Controls.Add(rightRevenue);

            Finding = new Label()
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.Blue,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "",
                Size = new Size(40, 40),
            };
            Controls.Add(Finding);

            listbox = new CheckedListBox()
            {
                BackColor = Color.Black,
                BorderStyle = BorderStyle.Fixed3D,
                ForeColor = Color.White,
                Font = new Font(Font, FontStyle.Bold),
                CheckOnClick = true,
            };
            Controls.Add(listbox);
            allRoles.Add("Acting");
            rolebox = new CheckedListBox()
            {
                BackColor = Color.Black,
                BorderStyle = BorderStyle.Fixed3D,
                ForeColor = Color.White,
                Font = new Font(Font, FontStyle.Bold),
                CheckOnClick = true,
                SelectionMode = SelectionMode.One,
            };
            Controls.Add(rolebox);
            listbox.ItemCheck += Item_Click;
            rolebox.ItemCheck += RoleItem_Click;

            hintText = new TextBox
            {
                Enabled = false,
                Location = new Point(-1000, -1000),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                TabStop = false,
                TabIndex = 0,
                Text = null_value,
            };
            Controls.Add(hintText);

            time = new Timer() { Enabled = true, Interval = 80 };
            time.Tick += Time;
            hintText.TextChanged += (o, e) => {time_slice = 0; };
            hintText.Click += Selection;
            hintText.KeyDown += (o, e) => { if (e.KeyCode == Keys.Enter) { Program.win.ricerca.FindingShow(); time_slice = 9; e.SuppressKeyPress = true; }; };
            hintText.LostFocus += ToNullValue;


            timer = new Timer()
            {
                Enabled = true,
                Interval = 5000,
            };
            timer.Tick += CheckGenres;
            only_checked = new Timer()
            {
                Enabled = true,
                Interval = 50,
            };
            only_checked.Tick += CheckChecked;


            PanelResearch.null_values = new string[] { "", PanelResearch.null_value };
            Window.null_values = new string[] { "", Window.null_value };

            TextBox[] textboxs = new TextBox[] { leftDate, rightDate, leftRevenue, rightRevenue };
            foreach (TextBox txtbox in textboxs)
            {
                txtbox.KeyDown += (o, e) =>
                {
                    if (e.KeyCode == Keys.Enter) { Program.win.ricerca.FindingShow(); time_slice = 9; e.SuppressKeyPress = true; }
                };

                txtbox.KeyPress += (o, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
                };
            }

            ResumeLayout(false);

        }
        public int time_slice = 11;
        private void Time(object sender, EventArgs e)
        {
            time_slice++;
            if (time_slice > 100) time_slice = 100;
            if (time_slice == 10) Supervisor.ResearchAsync();
        }

        //private string prev_text = "";
        private void ClickNull(object sender, EventArgs e)
        {
            Program.win.FocusGrip();
        }
        private void Selection(object sender, EventArgs e)
        {
            if (hintText.Text == null_value) hintText.Text = "";
        }
        private void ToNullValue(object sender, EventArgs e)
        {
            if (hintText.Text == "") hintText.Text = null_value;
        }

        public void ResizePanel()
        {
            SuspendLayout();
            listbox.Size = new Size(Width / 7, Height - 20);
            listbox.Location = new Point(Width - 20 - listbox.Width, 10);
            rolebox.Location = new Point(10, 10);
            rolebox.Size = new Size(Width / 7 -36, Height - 2 * rolebox.Location.Y);
            hintText.Location = new Point(rolebox.Location.X + rolebox.Width + 50, 20);
            hintText.Size = new Size(listbox.Location.X - rolebox.Location.X - rolebox.Width - 100 - 20, 25);
            DateLabel.Size = new Size(hintText.Width / 3, hintText.Height);
            DateLabel.Location = new Point((hintText.Width-DateLabel.Width)/ 2 + hintText.Location.X, hintText.Location.Y + hintText.Height + 10);
            leftDate.Size = new Size(DateLabel.Width / 4, DateLabel.Height);
            leftDate.Location = new Point(DateLabel.Location.X - leftDate.Width, DateLabel.Location.Y);
            rightDate.Size = new Size(DateLabel.Width / 4, DateLabel.Height);
            rightDate.Location = new Point(DateLabel.Location.X +DateLabel.Width, DateLabel.Location.Y);
            RevenueLabel.Location = new Point((hintText.Width - DateLabel.Width) / 2 + hintText.Location.X, hintText.Location.Y + 2 * hintText.Height+10);
            RevenueLabel.Size = new Size(hintText.Width / 3, hintText.Height);
            leftRevenue.Size = new Size(RevenueLabel.Width / 2, RevenueLabel.Height);
            leftRevenue.Location = new Point(RevenueLabel.Location.X - leftRevenue.Width, RevenueLabel.Location.Y);
            rightRevenue.Size = new Size(RevenueLabel.Width / 2, RevenueLabel.Height);
            rightRevenue.Location = new Point(RevenueLabel.Location.X + RevenueLabel.Width, RevenueLabel.Location.Y);
            Finding.Location = new Point(hintText.Location.X + hintText.Width / 4, hintText.Location.Y + 4 * hintText.Height);
            Finding.Size = new Size(hintText.Width / 2, hintText.Height);
            ResumeLayout(false);
        }
        public void FindingShow()
        {
            Finding.Text = "Sto cercando..";
        }
        public void FindingHide()
        {
            Finding.Text = "";
        }

        public bool Filter(Film film, string path, ResearchClass research)
        {
            bool output = true;
            try
            {

                string name = path.Substring(path.LastIndexOf(@"\") + 1);
                name = Iconxx.CleanName(name);
                bool titleCompatible = true;
                if (film != null) titleCompatible = Supervisor.Compatible(film.title, research.searchByName);
                bool nameCompatible = Supervisor.Compatible(name, research.searchByName);
                //Console.WriteLine(name + "    " + nameCompatible + "    " + titleCompatible + "    " + ResearchClass.StrangeDeepResearch());
                if (!titleCompatible && !nameCompatible) return false;
                if (!ResearchClass.StrangeDeepResearch() && nameCompatible) {  return true; }

                if (film == null || film.title == "AsItIsPowerVideos" || film.title == "NotFoundPowerVideos" || film.title == "") return false;
                if (film != null)
                {
                    int film_year = film.GetYear();
                    long film_revenue = film.revenue;
                    if (film_year < research.leftDate || film_year > research.rightDate) return false;
                    if (film_revenue < research.leftRevenue || film_revenue > research.rightRevenue) return false;
                    
                    if (research.Genres.Count > 0)
                    {
                        output = true;
                        if (film.genres.Count == 0) return false;
                        foreach (string genre in research.Genres)
                        {
                            if (!film.genres.Contains(genre)) output = false;
                        }
                        if (!output) { return false; }
                    }

                    if (!Supervisor.TextNull(research.searchByRole, PanelResearch.null_values))
                    {
                        output = false;
                        if (research.Role == "Acting")
                        {
                            if (film.lcast.Count == 0) return false;
                            foreach (Casting cast in film.lcast)
                            {
                                if (Supervisor.Compatible(cast.name, research.searchByRole)) output = true;
                            }
                        }
                        else
                        {
                            if (film.lcredits.Count == 0) return false;
                            foreach (Credits credit in film.lcredits)
                            {
                                if (credit.department == research.Role)
                                {
                                    if (Supervisor.Compatible(credit.name, research.searchByRole)) output = true;
                                }
                            }
                        }
                        if (!output) { return false; }

                    }



                }
                else return false;
            }
            catch (Exception e) { Console.WriteLine("Message from Filter: " + e.Message); return true; }




            return output;
        }

        private void CheckGenres(object sender, EventArgs e)
        {
            try
            {
                if (new_genre)
                {
                    listbox.Items.Clear();
                    listbox.Items.AddRange(allGenres.ToArray());
                    new_genre = false;
                }

                if (new_role)
                {
                    List<string> okRoles = new List<string>();
                    foreach (string role in allRoles) if (!role.Contains("\\")) okRoles.Add(role);
                    rolebox.Items.Clear();
                    rolebox.Items.AddRange(okRoles.ToArray());
                    new_role = false;
                }
            }
            catch (Exception) { }

        }

        int last_checked;
        bool ready_research = false;
        int iterations = 0;
        private void CheckChecked(object sender, EventArgs e)
        {
            try
            {
                CheckStrangeness();
                iterations++;
                if (rolebox.CheckedIndices.Count == 0)
                {
                    if (hintText.Text != super_null_value)
                    {
                        hintText.Enabled = false;
                        hintText.Text = super_null_value;
                        null_value = hintText.Text;
                    }
                }
                if (rolebox.CheckedIndices.Count == 1) { last_checked = rolebox.CheckedIndices[0]; }
                if (rolebox.CheckedIndices.Count < 2) return;
                if (!rolebox.CheckedIndices.Contains(last_checked)) last_checked = rolebox.CheckedIndices[0];
                rolebox.SetItemCheckState(last_checked, CheckState.Unchecked);

                if (iterations != 10) return;
                if (ready_research) { ready_research = false; Supervisor.ResearchAsync(); }
                else { hintText.Enabled = true; }
            }
            catch (Exception) { }
        }

        private void CheckStrangeness()
        {
            bool output = false;
            if (listbox.CheckedItems.Count != 0) output = true;
            TextBox[] textboxs = new TextBox[] { leftDate, rightDate, leftRevenue, rightRevenue };
            foreach (TextBox txt in textboxs) if (txt.Text != "") output = true;
            //if (rolebox.CheckedItems.Count != 0) { Console.WriteLine("HEY"); }
            if (!hintText.Text.Contains(super_null_value) && hintText.Text != "") { output = true; }
            //Console.WriteLine(output);
            if (output) something_strange = true;
            else something_strange = false;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            //something_changed = true;
            listbox.SetSelected(listbox.SelectedIndex, false);
            time_slice = 0;
            ready_research = true;
        }

        private void RoleItem_Click(object sender, EventArgs e)
        {
            //something_changed = true;
            try
            {
                if (hintText.Text.Contains(super_null_value) || hintText.Text == "") hintText.Enabled = false;
                hintText.Text = super_null_value + rolebox.Items[rolebox.SelectedIndex];
                null_value = hintText.Text;
                hintText.Enabled = true;
            }
            catch (Exception) { }
        }
    }

    public class ResearchClass
    {
        public static ResearchClass previousResearch;
        public static ResearchClass actualResearch;
        public string searchByName = "";
        public string searchByRole = "";
        public int leftDate = 0;
        public int rightDate = 1000000000;
        public long leftRevenue = 0;
        public long rightRevenue = 100000000000;
        public List<string> Genres = new List<string>();
        public string Role = "";
        ResearchClass(string nametextbox, string roletextbox, string leftdate, string rightdate, string leftrevenue, string rightrevenue, List<string> genres, string Role)
        {

            this.searchByName = nametextbox;
            if (Supervisor.TextNull(searchByName, Window.null_values)) searchByName = "";
            this.searchByRole = roletextbox;
            if (Supervisor.TextNull(searchByRole, PanelResearch.null_values) || searchByName.Contains(PanelResearch.super_null_value)) searchByRole = "";

            if (leftdate != "") leftDate = Convert.ToInt32(leftdate);
            if (rightdate != "") rightDate = Convert.ToInt32(rightdate);
            if (leftrevenue != "") leftRevenue = Convert.ToInt64(leftrevenue);
            if (rightrevenue != "") rightRevenue = Convert.ToInt64(rightrevenue);
            this.Role = Role;
            foreach (string genre in genres) Genres.Add(genre);
        }
        ResearchClass(string nametextbox, string roletextbox, int leftdate, int rightdate, long leftrevenue, long rightrevenue, List<string> genres, string Role)
        {

            this.searchByName = nametextbox;
            if (Supervisor.TextNull(searchByName, Window.null_values)) searchByName = "";
            this.searchByRole = roletextbox;
            if (Supervisor.TextNull(searchByRole, PanelResearch.null_values) || searchByName.Contains(PanelResearch.super_null_value)) searchByRole = "";

            leftDate = leftdate;
            rightDate = rightdate;
            leftRevenue = leftrevenue;
            rightRevenue = rightrevenue;
            this.Role = Role;
            foreach (string genre in genres) Genres.Add(genre);
        }
        public static void Initialize()
        {
            Console.WriteLine("Initializing Research..");
            previousResearch = new ResearchClass("","","","","","",new List<string>(), "");
        }
        public static bool IsNewResearch()
        {
            if (previousResearch == null) Initialize();
            ResearchClass research = CreateNewResearch();
            actualResearch = research;
            actualResearch.Save();
            if (Equals(research, previousResearch)) { return false; }
            else
            {
                //previousResearch.Verbose();
                //actualResearch.Verbose();
                previousResearch = new ResearchClass(actualResearch.searchByName, actualResearch.searchByRole, actualResearch.leftDate, actualResearch.rightDate,
                    actualResearch.leftRevenue, actualResearch.rightRevenue, actualResearch.Genres, actualResearch.Role);
                return true;
            }
        }
        public static ResearchClass CreateNewResearch()
        {
            string role = "";
            if (Program.win.ricerca.rolebox.CheckedIndices.Count == 1) { role = (string)Program.win.ricerca.rolebox.CheckedItems[0]; }
            List<string> genres = new List<string>();
            foreach (string genre in Program.win.ricerca.listbox.CheckedItems) genres.Add(genre);
            genres = genres.OrderBy(o => o).ToList();


            ResearchClass research;
            research = new ResearchClass(Program.win.hintText.Text, Program.win.ricerca.hintText.Text, Program.win.ricerca.leftDate.Text,
                Program.win.ricerca.rightDate.Text, Program.win.ricerca.leftRevenue.Text, Program.win.ricerca.rightRevenue.Text,
                genres, role);
            return research;
        }

        public static bool Equals(ResearchClass first, ResearchClass second)
        {
            if (first == null && second != null) { return false; }
            if (first != null && second == null) { Initialize(); return true; }
            if (first.Genres.Count != second.Genres.Count) { return false; }
            for (int i = 0; i < first.Genres.Count; i++) if (first.Genres[i] != second.Genres[i]) { return false; }
            if (first.searchByName != second.searchByName) { return false; }
            if (first.searchByRole != second.searchByRole) { return false; }
            if (first.leftDate != second.leftDate) { return false; }
            if (first.rightDate != second.rightDate) { return false; }
            if (first.leftRevenue != second.leftRevenue) { return false; }
            if (first.rightRevenue != second.rightRevenue) { return false; }
            return true;
        }
        public static bool StrangeDeepResearch()
        {
            if (actualResearch == null) return false;
            if (actualResearch.Genres.Count != 0) { return true; }
            if (actualResearch.searchByRole != "") { return true; }
            if (actualResearch.leftDate != 0) { return true; }
            if (actualResearch.rightDate != 1000000000) { return true; }
            if (actualResearch.leftRevenue != 0) { return true; }
            if (actualResearch.rightRevenue != 100000000000) { return true; }
            return false;
        }
        public static bool StrangeFirstResearch()
        {
            if (actualResearch == null) return false;
            if (actualResearch.searchByName != "") { return true; }
            return false;
        }

        public void Verbose()
        {
            Console.WriteLine("New Research");
            Console.WriteLine("Search by name: |"+ searchByName+"|");
            Console.WriteLine("Search by role: |"+searchByRole+"|");
            Console.WriteLine("from |" + leftDate + "| to |" + rightDate + "|");
            Console.WriteLine("from |" + leftRevenue + "| to |" + rightRevenue + "|");
            foreach (string genre in Genres) Console.Write(genre + ", ");
        }
        public static List<string> GetDefaultResearch()
        {
            if (Properties.Settings.Default.savedResearch == "")
            { Initialize(); return null; }
            string[] fragments = Properties.Settings.Default.savedResearch.Split(new string[] { "|^|" }, StringSplitOptions.None);
            if(fragments.Length<8) { Initialize(); return null; }
            string[] genres_ = fragments[7].Split(new string[] { "^_^" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> ListGenres = genres_.ToList();
            previousResearch = new ResearchClass("", fragments[1], fragments[2], fragments[3], fragments[4], fragments[5], ListGenres, fragments[6]);
            List<string> list = new List<string>();
            list.Add(Convert.ToString(fragments[2]));
            list.Add(Convert.ToString(fragments[3]));
            list.Add(Convert.ToString(fragments[4]));
            list.Add(Convert.ToString(fragments[5]));
            return list;

            //bool found = false;
            for (int i = 0; i < Program.win.ricerca.rolebox.Items.Count; i++) if (fragments[6] == (string)Program.win.ricerca.rolebox.Items[i])
                {
                    Program.win.ricerca.rolebox.SetItemChecked(i, true);
                    Program.win.ricerca.hintText.Text = PanelResearch.super_null_value + fragments[6];
                    PanelResearch.null_value = Program.win.ricerca.hintText.Text;
                    Program.win.ricerca.hintText.Enabled = true;
                    //found = true;
                    break; }

            //if (fragments[1] != "") Program.win.ricerca.hintText.Text = fragments[1];
            //if(fragments[1] == "")
            {
                //if (found)
                {
                    //Program.win.ricerca.hintText.Text = PanelResearch.super_null_value + fragments[6];
                    //PanelResearch.null_value = Program.win.ricerca.hintText.Text;
                    //Program.win.ricerca.hintText.Enabled = true;
                }
                //else 
                //Program.win.ricerca.hintText.Text = PanelResearch.super_null_value;
            }
            //for (int i = 0; i < Program.win.ricerca.listbox.Items.Count; i++) if (ListGenres.Contains((string)Program.win.ricerca.listbox.Items[i]))
                    //Program.win.ricerca.listbox.SetItemChecked(i, true);

        }
        public void Save()
        {
            string saved = "";
            saved += searchByName + "|^|";
            saved += searchByRole + "|^|";
            saved += (leftDate == 0) ? "" : Convert.ToString(leftDate); saved += "|^|";
            saved += (rightDate == 1000000000) ? "" : Convert.ToString(rightDate); saved += "|^|";
            saved += (leftRevenue == 0) ? "" : Convert.ToString(leftRevenue); saved += "|^|";
            saved += (rightRevenue == 100000000000) ? "" : Convert.ToString(rightRevenue); saved += "|^|";
            saved += Role + "|^|";
            foreach (string genre in Genres) saved += genre + "^_^"; saved += "|^|";
            Properties.Settings.Default.savedResearch = saved;
            Properties.Settings.Default.Save();
        }

    }
}
