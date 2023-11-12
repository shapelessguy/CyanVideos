using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using CheckBox = System.Windows.Forms.CheckBox;

namespace CyanVideos
{
    public partial class InfoClass : Form
    {
        public static Form actual_form;
        Iconxx icon;
        public static int last_panel = 1;

        public InfoClass(Iconxx icon)
        {
            this.icon = icon;

            actual_form = this;
            InitializeComponent();
            button1.Text = "Search";
            button3.Text = "Close";
            button6.Text = "Open folder";
            SuspendLayout();
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.Black;
            dataGridView1.RowsDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.RowsDefaultCellStyle.Font = new Font("Times New Roman", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.Black;
            dataGridView1.RowHeadersVisible = false;

            dataGridView2.EnableHeadersVisualStyles = false;
            dataGridView2.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.Black;
            dataGridView2.RowsDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.RowsDefaultCellStyle.Font = new Font("Times New Roman", 10, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.Black;
            dataGridView2.RowHeadersVisible = false;
            button7.Visible = icon.series;

            foreach (Casting cast in icon.principal_film.lcast) dataGridView1.Rows.Add(cast.name, cast.character);
            foreach (Credits credit in icon.principal_film.lcredits.OrderBy(x => x.job).ToList()) dataGridView2.Rows.Add(credit.name, credit.department, credit.job);

            label2.Text = icon.principal_film.title;
            label10.Text = "";
            label17.Text = "";
            if (icon.principal_film.genres.Count > 0)
            {
                foreach (string genre in icon.principal_film.genres) label10.Text += genre + ", ";
                label10.Text = label10.Text.Substring(0, label10.Text.Length - 2);
            }
            if (icon.principal_film.production_companies.Count > 0)
            {
                foreach (string company in icon.principal_film.production_companies) label17.Text += company + ", ";
                label17.Text = label17.Text.Substring(0, label17.Text.Length - 2);
            }

            label19.Text = icon.principal_film.GetReleaseDate();
            int k;
            if (Int32.TryParse(icon.principal_film.vote_average, out k)) label4.Text = icon.principal_film.vote_average + "/10";
            if (Int32.TryParse(icon.principal_film.vote_average, out k)) label6.Text = icon.principal_film.vote_count;
            label7.Text = icon.principal_film.original_language;
            label12.Text = icon.principal_film.runtime.ToString() + " min";
            label14.Text = icon.principal_film.GetRevenue();
            label16.Text = icon.principal_film.overview;

            initializePanel4();

            ResumeLayout(false);
            if (last_panel == 1) panel1.BringToFront();
            else if (last_panel == 2) panel2.BringToFront();
            else if (last_panel == 3) panel3.BringToFront();
            else if (last_panel == 4) panel4.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
            last_panel = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (FilmSelection.actual_form != null) FilmSelection.actual_form.Close();
                FilmSelection selection = new FilmSelection(icon);
                selection.Show();
                Close();
            }
            catch (Exception)
            {
                try
                {
                    FilmSelection selection = new FilmSelection(icon);
                    selection.Show();
                    Close();
                }
                catch (Exception) { }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel3.BringToFront();
            last_panel = 3;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.BringToFront();
            last_panel = 2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string path = icon.fullpath;
            if (Program.IsVideo(path)) path = Directory.GetParent(path).FullName;
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SeasonEditor.SeasonEditor editor = new SeasonEditor.SeasonEditor(icon.fullpath);
            editor.Show();
        }


        private void initializePanel4()
        {
            List<string> tags = Properties.Settings.Default.tags.Split(new string[] { "|-.-|" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i=0; i< tags.Count; i++)
            {
                CheckBox ch = new CheckBox();
                ch.AutoSize = true;
                ch.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                ch.ForeColor = System.Drawing.Color.White;
                ch.Location = new System.Drawing.Point(51, 22 + 30 * i);
                ch.Size = new System.Drawing.Size(146, 29);
                ch.TabIndex = 0;
                ch.Text = tags[i];
                ch.UseVisualStyleBackColor = true;
                ch.CheckedChanged += new System.EventHandler(CheckedChanged);
                tag_panel.Controls.Add(ch);
            }
            GetTags(icon.fullpath, tag_panel.Controls);
        }
        private void CheckedChanged(object sender, EventArgs e)
        {
            string tag_file = Path.Combine(icon.fullpath, "tagpowervideos.txt");
            string to_save = "";
            foreach (CheckBox c in tag_panel.Controls)
            {
                to_save += c.Text + "__:__" + c.Checked + "\n";
            }
            File.WriteAllText(tag_file, to_save);
        }
        public static List<string> GetTags(string path, System.Windows.Forms.Control.ControlCollection controls_to_check = null)
        {
            string tag_file = Path.Combine(path, "tagpowervideos.txt");
            if (!File.Exists(tag_file)) return null;
            string[] lines = File.ReadAllLines(tag_file);
            List<string> output = new List<string>();
            foreach (string line in lines)
            {
                string[] tags = line.Split(new string[] { "__:__" }, StringSplitOptions.RemoveEmptyEntries);
                if (controls_to_check != null)
                {
                    foreach (CheckBox c in controls_to_check)
                    {
                        if (c.Text == tags[0]) c.Checked = Boolean.Parse(tags[1]);
                    }
                }
                if (Boolean.Parse(tags[1])) output.Add(tags[0]);
            }
            return output;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            panel4.BringToFront();
            last_panel = 4;
        }
    }
}
