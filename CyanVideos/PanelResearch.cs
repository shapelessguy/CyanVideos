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
        static public bool new_tag = true;
        private Timer timer, only_checked, time;
        public CheckedListBox listbox;
        public CheckedListBox rolebox;
        public CheckedListBox tagbox;
        public TextBox hintText = new TextBox();
        static public string super_null_value = "...Search by...";
        static public string null_value = "...Search by...";
        static public string[] null_values;

        private Label Finding;
        Label DateLabel;
        Label RevenueLabel;
        Label TagLabel;
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
            BackgroundImage = Properties.Resources.Background2;
            BackgroundImageLayout = ImageLayout.Stretch;
            BorderStyle = BorderStyle.FixedSingle;
            Visible = false;
            Click += ClickNull;
            Size = new Size(0, 0);

            DateLabel = new Label()
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "\u2190 Date range \u2192",
                Size = new Size(40, 40),
            };
            Controls.Add(DateLabel);
            RevenueLabel = new Label()
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "\u2190 Revenue range (M$) \u2192",
                Size = new Size(40, 40),
            };
            Controls.Add(RevenueLabel);
            TagLabel = new Label()
            {
                Location = new Point(-1000, -1000),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "TAGS",
                Size = new Size(40, 40),
            };
            Controls.Add(TagLabel);
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
                BackColor = Color.Transparent,
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

            tagbox = new CheckedListBox()
            {
                BackColor = Color.Black,
                BorderStyle = BorderStyle.Fixed3D,
                ForeColor = Color.White,
                Font = new Font(Font, FontStyle.Bold),
                CheckOnClick = true,
            };
            Controls.Add(tagbox);

            listbox.ItemCheck += Item_Click;
            rolebox.ItemCheck += RoleItem_Click;
            tagbox.ItemCheck += TagItem_Click;

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
                Interval = 1000,
            };
            timer.Tick += CheckGenres_Tags;
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
            rolebox.Size = new Size(Width / 7 - 36, Height - 2 * rolebox.Location.Y);
            hintText.Location = new Point(rolebox.Location.X + rolebox.Width + 50, 20);
            hintText.Size = new Size(listbox.Location.X - rolebox.Location.X - rolebox.Width - 100 - 20, 25);

            int shift_l = (int)(hintText.Width * 0.1);
            DateLabel.Size = new Size(hintText.Width / 5, hintText.Height);
            DateLabel.Location = new Point((hintText.Width-DateLabel.Width)/ 2 + hintText.Location.X - shift_l, hintText.Location.Y + hintText.Height * 3 + 10);
            leftDate.Size = new Size(DateLabel.Width / 4, DateLabel.Height);
            leftDate.Location = new Point(DateLabel.Location.X - leftDate.Width, DateLabel.Location.Y);
            rightDate.Size = new Size(DateLabel.Width / 4, DateLabel.Height);
            rightDate.Location = new Point(DateLabel.Location.X +DateLabel.Width, DateLabel.Location.Y);
            RevenueLabel.Location = new Point((hintText.Width - DateLabel.Width) / 2 + hintText.Location.X - shift_l, hintText.Location.Y + 2 * hintText.Height+10);
            RevenueLabel.Size = new Size(hintText.Width / 5, hintText.Height);
            leftRevenue.Size = new Size(RevenueLabel.Width / 2, RevenueLabel.Height);
            leftRevenue.Location = new Point(RevenueLabel.Location.X - leftRevenue.Width, RevenueLabel.Location.Y);
            rightRevenue.Size = new Size(RevenueLabel.Width / 2, RevenueLabel.Height);
            rightRevenue.Location = new Point(RevenueLabel.Location.X + RevenueLabel.Width, RevenueLabel.Location.Y);

            TagLabel.Location = new Point(rightRevenue.Location.X + (int)(rightRevenue.Width * 2.8) + 10, rightRevenue.Location.Y - hintText.Height);
            TagLabel.Size = new Size(Width / 13, hintText.Height);
            tagbox.Location = new Point(rightRevenue.Location.X + (int)(rightRevenue.Width * 2.6) + 10, rightRevenue.Location.Y);
            tagbox.Size = new Size(Width / 10, Height / 2);
            Finding.Location = new Point(hintText.Location.X + hintText.Width / 4, hintText.Location.Y + 4 * hintText.Height);
            Finding.Size = new Size(hintText.Width / 2, hintText.Height);
            ResumeLayout(false);
        }
        public void FindingShow()
        {
            Finding.Text = "Searching..";
        }
        public void FindingHide()
        {
            Finding.Text = "";
        }

        public bool Filter(Film film, string path, ResearchClass research)
        {
            if (film == null) film = Iconxx.GetPrincipalFilm(path + @"\infopowervideos.txt");
            bool output = true;
            try
            {
                List<string> tags = InfoClass.GetTags(path);
                string name = path.Substring(path.LastIndexOf(@"\") + 1);
                name = Program.CleanName(name);
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
                    if (research.Tags.Count > 0)
                    {
                        output = true;
                        if (tags == null)
                        {
                            return false;
                        }
                        if (tags.Count == 0)
                        {
                            return false;
                        }
                        foreach (string tag in research.Tags)
                        {
                            if (!tags.Contains(tag)) output = false;
                        }
                        if (!output)
                        {
                            return false;
                        }
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
            catch (Exception e) 
            { 
                Console.WriteLine("Message from Filter: " + e.Message + e.ToString()); 
                return true; 
            }




            return output;
        }

        private void CheckGenres_Tags(object sender, EventArgs e)
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

                if (new_tag)
                {
                    List<string> tags = Properties.Settings.Default.tags.Split(new string[] { "|-.-|" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    tagbox.Items.Clear();
                    tagbox.Items.AddRange(tags.ToArray());
                    new_tag = false;
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
        private void TagItem_Click(object sender, EventArgs e)
        {
            tagbox.SetSelected(tagbox.SelectedIndex, false);
            time_slice = 0;
            ready_research = true;
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
        public List<string> Tags = new List<string>();
        public string Role = "";
        ResearchClass(string nametextbox, string roletextbox, string leftdate, string rightdate, string leftrevenue, string rightrevenue, 
            List<string> genres, List<string> tags, string Role)
        {

            this.searchByName = nametextbox;
            if (Supervisor.TextNull(searchByName, Window.null_values)) searchByName = "";
            this.searchByRole = roletextbox;
            if (Supervisor.TextNull(searchByRole, PanelResearch.null_values) || searchByName.Contains(PanelResearch.super_null_value)) searchByRole = "";

            if (leftdate != "") leftDate = Convert.ToInt32(leftdate);
            if (rightdate != "") rightDate = Convert.ToInt32(rightdate);
            if (leftrevenue != "") leftRevenue = (int)(Convert.ToDouble(leftrevenue) * 1000000);
            if (rightrevenue != "") rightRevenue = (int)(Convert.ToDouble(rightrevenue) * 1000000);
            this.Role = Role;
            foreach (string genre in genres) Genres.Add(genre);
            foreach (string tag in tags) Tags.Add(tag);
        }
        ResearchClass(string nametextbox, string roletextbox, int leftdate, int rightdate, long leftrevenue, long rightrevenue, 
            List<string> genres, List<string> tags, string Role)
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
            foreach (string tag in tags) Tags.Add(tag);
        }
        public static void Initialize()
        {
            Console.WriteLine("Initializing Research..");
            previousResearch = new ResearchClass("","","","","","", new List<string>(), new List<string>(), "");
        }
        public static bool IsNewResearch()
        {
            if (previousResearch == null) Initialize();
            ResearchClass research = CreateNewResearch();
            actualResearch = research;
            if (Equals(research, previousResearch)) { return false; }
            else
            {
                //previousResearch.Verbose();
                //actualResearch.Verbose();
                previousResearch = new ResearchClass(actualResearch.searchByName, actualResearch.searchByRole, actualResearch.leftDate, actualResearch.rightDate,
                    actualResearch.leftRevenue, actualResearch.rightRevenue, actualResearch.Genres, actualResearch.Tags, actualResearch.Role);
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
            List<string> tags = new List<string>();
            foreach (string tag in Program.win.ricerca.tagbox.CheckedItems) tags.Add(tag);
            tags = tags.OrderBy(o => o).ToList();


            ResearchClass research;
            research = new ResearchClass(Program.win.hintText.Text, Program.win.ricerca.hintText.Text, Program.win.ricerca.leftDate.Text,
                Program.win.ricerca.rightDate.Text, Program.win.ricerca.leftRevenue.Text, Program.win.ricerca.rightRevenue.Text,
                genres, tags, role);
            return research;
        }

        public static bool Equals(ResearchClass first, ResearchClass second)
        {
            if (first == null && second != null) { return false; }
            if (first != null && second == null) { Initialize(); return true; }
            if (first.Genres.Count != second.Genres.Count) { return false; }
            for (int i = 0; i < first.Genres.Count; i++) if (first.Genres[i] != second.Genres[i]) { return false; }
            if (first.Tags.Count != second.Tags.Count) { return false; }
            for (int i = 0; i < first.Tags.Count; i++) if (first.Tags[i] != second.Tags[i]) { return false; }
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
            if (actualResearch.Tags.Count != 0) { return true; }
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
            Console.WriteLine();
            foreach (string tag in Tags) Console.Write(tag + ", ");
            Console.WriteLine();
        }

    }
}
