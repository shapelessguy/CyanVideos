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
    public partial class NotFound : Form
    {
        static public bool active = false;
        private Iconxx icon;
        public NotFound(Iconxx icon)
        {
            this.icon = icon;

            active = true;
            InitializeComponent();
            this.LostFocus += Exit;
            this.FormClosing += Exit;
        }

        public void Activer(object sender, EventArgs e)
        {
            active = false;
        }
        public void Exit(object sender, EventArgs e)
        {
            Activer(null, null);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Trying to remove file: "+icon.infopath);
            if (icon.infopath != "") if (System.IO.File.Exists(icon.infopath)) try {
                        System.IO.File.Delete(icon.infopath);
                        //icon.Initialize();
                    } catch (Exception) { };
            Exit(null, null);
        }
    }
}
