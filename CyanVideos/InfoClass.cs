using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public partial class InfoClass : Form
    {
        public static Form actual_form;
        Iconxx icon;

        public InfoClass(Iconxx icon)
        {
            this.icon = icon;
            actual_form = this;
            InitializeComponent();
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




            ResumeLayout(false);
            panel1.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
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
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.BringToFront();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string path = icon.fullpath;
            if (Program.IsVideo(path)) path = Directory.GetParent(path).FullName;
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }
}
