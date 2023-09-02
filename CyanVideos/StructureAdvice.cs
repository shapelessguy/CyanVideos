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
    public partial class StructureAdvice : Form
    {
        public static bool active = false;
        public static StructureAdvice win;
        public StructureAdvice()
        {
            InitializeComponent(); 
            Text = "Suggested structure";
            active = true;
            foreach (string stringa in Supervisor.StructureAdvice) textBox1.Text += stringa + "\r\n";
        }

        private void StructureAdvice_FormClosing(object sender, FormClosingEventArgs e)
        {
            active = false;
        }
    }
}
