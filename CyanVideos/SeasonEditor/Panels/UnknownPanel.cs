using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;

namespace CyanVideos.SeasonEditor
{
    public class UnknownPanel : Panel
    {
        private List<Reference> references = new List<Reference>();
        private SeasonEditor parent;
        public UnknownPanel()
        {
            AllowDrop = true;
            Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left))));
            BackColor = Color.Transparent;
            Location = new System.Drawing.Point(10, 10);
            Name = "unk_panel";
            Size = new System.Drawing.Size(600, 600);
            TabIndex = 3;
            AllowDrop = true;
            DragEnter += new System.Windows.Forms.DragEventHandler(dragEnter);
            DragDrop += new System.Windows.Forms.DragEventHandler(dragDrop);
            AutoScroll = true;
        }

        public void Clear()
        {
            references.Clear();
        }

        public void Initialize(List<Reference> in_references, SeasonEditor parent)
        {
            parent.known_panel.addSeasonToList();
            this.parent = parent;
            foreach (Reference reference in in_references)
            {
                reference.MouseDown += new MouseEventHandler(DropLoading);
                reference.getTextLabel().MouseDown += new MouseEventHandler(DropLoading);
                addReference(reference);
            }
            Draw();
            preLinkReferences();
        }
        private void preLinkReferences()
        {
            int last_season = 0;
            bool check_all_folders = false;
            for (int j = references.Count - 1; j >= 0; j--)
            {
                Reference reference = references[j];
                string season = Path.GetDirectoryName(reference.directory);
                string season_name = Path.GetFileName(season);

                if (!check_all_folders)
                {
                    // Check if the season is a CyanVideos Season for each folder
                    foreach (string folder in Directory.GetDirectories(Path.GetDirectoryName(season))) 
                    {
                        string folder_name = Path.GetFileName(folder);
                        if (folder_name.Substring(0, 1) == "(")
                        {
                            int n = -1;
                            string str_n = "";
                            try
                            {
                                int i = 0;
                                for (i = 1; i < folder_name.Length; i++)
                                {
                                    if (folder_name.Substring(i, 1) == ")") break;
                                    else str_n += folder_name.Substring(i, 1);
                                }
                                n = Int32.Parse(str_n);
                                folder_name = folder_name.Substring(i + 1);
                            }
                            catch { }
                            if (n > -1 && n < 20)
                            {
                                last_season = Math.Max(last_season, n);
                                for (int i = parent.known_panel.getSeasonList().Count; i <= last_season; i++)
                                {
                                    parent.known_panel.addSeasonToList();
                                }
                                parent.known_panel.getSeasonList()[n].setSeasonTextBox(folder_name);
                            }
                        }
                    }
                    check_all_folders = true;
                }


                // Check if the season is a CyanVideos Season
                if (season_name.Substring(0, 1) == "(")
                {
                    int n = -1;
                    string str_n = "";
                    try
                    {
                        int i = 0;
                        for (i = 1; i < season.Length; i++)
                        {
                            if (season_name.Substring(i, 1) == ")") break;
                            else str_n += season_name.Substring(i, 1);
                        }
                        n = Int32.Parse(str_n);
                        season_name = season_name.Substring(i + 1);
                    }
                    catch { }
                    if (n > -1 && n < 20)
                    {
                        parent.MoveToSeason(reference, parent.known_panel.getSeasonList()[n]);
                    }
                    else if (Path.GetFileName(season) == SeasonPanel.supplement_name)
                    {
                        parent.MoveToSeason(reference, parent.known_panel.getSeasonList()[0]);
                    }
                }
            }
        }
        public List<Reference> getReferences()
        {
            return references;
        }
        public void addReference(Reference reference)
        {
            reference.ShowTextbox(false);
            references.Add(reference);
        }
        public void removeReference(Reference reference)
        {
            references.Remove(reference);
        }

        public void Draw()
        {
            Controls.Clear();
            references = references.OrderBy(x => x.index).ToList();
            int height = 10;
            for (int i = 0; i < references.Count; i++)
            {
                if (i != 0) height += references[i - 1].Height; // + 5;
                Controls.Add(references[i]);

                references[i].Location = new System.Drawing.Point(references[i].Location.X, height);
                references[i].BringToFront();
            }
            if (references.Count > 0) height += references[references.Count - 1].Height; // + 5;
        }
        void DropLoading(object sender, MouseEventArgs e)
        {
            Reference ref_ = null;
            try { ref_ = (Reference)((Label)sender).Parent; }
            catch { ref_ = (Reference)sender; }
            if (e.Button == MouseButtons.Right || ModifierKeys == Keys.Control)
            {
                ref_.ChangeSelect();
            }
            else
            {
                DoDragDrop(ref_.text.Name, System.Windows.Forms.DragDropEffects.Copy);
            }
        }

        void dragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.Text)) e.Effect = System.Windows.Forms.DragDropEffects.Copy;
        }
        void dragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            int dropped = 0;
            foreach (SeasonPanel ref_ in parent.known_panel.getSeasonList())
            {
                for (int i = ref_.getReferences().Count - 1; i >= 0; i--)
                {
                    Reference x = ref_.getReferences()[i];
                    if (x.isSelected() || x.text.Name == (string)e.Data.GetData(System.Windows.Forms.DataFormats.Text))
                    {
                        parent.MoveToUnknownPanel(x, false);
                        dropped += 1;
                    }
                }
            }
            if (dropped > 0)
            {
                Draw();
                parent.known_panel.Draw();
            }
        }
    }
}
