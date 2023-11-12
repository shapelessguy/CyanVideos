using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace CyanVideos
{
    public partial class TagWin : Form
    {
        static public List<string> tags = new List<string>();
        public TagWin()
        {
            LoadTags();
            InitializeComponent();
            foreach(string tag in tags)
            {
                AddTag(tag);
            }
            Reshape();
        }

        private void AddTag(string name, bool save = false)
        {
            Button tag_btn = new Button();
            Button tag_del_btn = new Button();
            int index = panel1.Controls.Count / 2;
            // 
            // button2
            // 
            tag_btn.BackColor = System.Drawing.Color.White;
            tag_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            tag_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tag_btn.Location = new System.Drawing.Point(10, 10 + 25 * index);
            tag_btn.Size = new System.Drawing.Size(122, 22);
            tag_btn.TabIndex = 0;
            tag_btn.Text = name;
            tag_btn.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            tag_del_btn.BackColor = System.Drawing.Color.White;
            tag_del_btn.BackgroundImage = Properties.Resources.X;
            tag_del_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            tag_del_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            tag_del_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tag_del_btn.Location = new System.Drawing.Point(138, 12 + 25 * index);
            tag_del_btn.Size = new System.Drawing.Size(18, 18);
            tag_del_btn.TabIndex = 1;
            tag_del_btn.UseVisualStyleBackColor = false;
            tag_del_btn.Click += new System.EventHandler(deleteTag);

            panel1.Controls.Add(tag_btn);
            panel1.Controls.Add(tag_del_btn);

            if (save)
            {
                textBox1.Text = "";
                tags.Add(name);
<<<<<<< HEAD
=======
                PanelResearch.new_tag = true;
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
                Save();
            }
        }
        private void Reshape()
        {
            for (int i = 0; i< tags.Count; i++)
            {
                Button tag_btn = (Button)panel1.Controls[2 * i];
                Button tag_del_btn = (Button)panel1.Controls[2 * i + 1];
                tag_btn.Location = new Point(tag_btn.Location.X, 10 + 25 * i);
                tag_del_btn.Location = new Point(tag_del_btn.Location.X, 12 + 25 * i);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && !tags.Contains(textBox1.Text)) AddTag(textBox1.Text, true);
<<<<<<< HEAD
            PanelResearch.new_tag = true;
=======
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
        }
        private void deleteTag(object sender, EventArgs e)
        {
            Button tag_del_btn = ((Button)sender);
            for (int i = tags.Count - 1; i >= 0; i--)
            {
                if (panel1.Controls[2 * i + 1] == tag_del_btn)
                {
                    tags.Remove(panel1.Controls[2 * i].Text);
                    panel1.Controls.RemoveAt(2 * i);
                    panel1.Controls.RemoveAt(2 * i - 1);
                    break;
                }
            }
            Reshape();
<<<<<<< HEAD
=======
            PanelResearch.new_tag = true;
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
            Save();
        }
        private void LoadTags()
        {
            tags = Properties.Settings.Default.tags.Split(new string[] { "|-.-|" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        private void Save()
        {
            Properties.Settings.Default.tags = string.Join("|-.-|", tags);
            Properties.Settings.Default.Save();
        }
<<<<<<< HEAD
=======

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1_Click(sender, e);
            }
        }
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
    }
}
