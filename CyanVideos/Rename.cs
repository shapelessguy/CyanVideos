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
    public partial class Rename : Form
    {
        Source source;
        public Rename(Source source)
        {
            InitializeComponent();
            this.source = source;
            label1.Text = "La fonte denominata - " + source.tag.name+" -";
            Text = "Rinomina della cartella: "+source.directory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "") source.tag.name = textBox2.Text;
            if (source.Icons().Count == 0) source.tag.name += " (empty)";
            Program.win.firstpanel.Refresh(true);
            Program.Save();
            Close();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { button1_Click(sender, e); e.SuppressKeyPress = true; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Visible = false;
            Update();
            source.Dispose();
            Window.Sources.Remove(source);
            Program.win.firstpanel.Refresh(true);
            Program.Save();
            Close();
        }
    }
}
