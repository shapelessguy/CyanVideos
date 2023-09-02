using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace CyanVideos.SeasonEditor
{
    public class KnownPanel : Panel
    {
        private List<SeasonPanel> seasonList = new List<SeasonPanel>();
        private Label add_season = new Label();
        private Label remove_season = new Label();
        private SeasonEditor parent;
        public KnownPanel()
        {
            BackColor = Color.Transparent;
            Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            Location = new System.Drawing.Point(620, 10);
            Name = "known_panel";
            Size = new System.Drawing.Size(890, 600);
            TabIndex = 2;
            AutoScroll = true;
        }

        public void Clear()
        {
            Controls.Clear();
            foreach (var pan in seasonList) { pan.Clear(); }
            seasonList.Clear();
        }

        public void Initialize(SeasonEditor parent)
        {
            this.parent = parent;
            foreach(SeasonPanel panel in seasonList) { panel.editor = parent; }
            Label add_season_lbl = new Label();
            Label remove_season_lbl = new Label();
            // 
            // add_panel
            // 
            add_season.BackColor = System.Drawing.Color.Green;
            add_season.Controls.Add(add_season_lbl);
            add_season.Location = new System.Drawing.Point(200, 0);
            add_season.Name = "add_panel";
            add_season.Size = new System.Drawing.Size(250, 20);
            add_season.TabIndex = 1;
            // 
            // add_panel_label
            // 
            add_season_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            add_season_lbl.Location = new System.Drawing.Point(0, 0);
            add_season_lbl.Name = "add_panel_label";
            add_season_lbl.Size = new System.Drawing.Size(250, 20);
            add_season_lbl.TabIndex = 0;
            add_season_lbl.Text = "+";
            add_season_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            add_season_lbl.Click += new System.EventHandler(this.add_panel_Click);
            // 
            // remove_panel
            // 
            remove_season.BackColor = System.Drawing.SystemColors.ActiveCaption;
            remove_season.Controls.Add(remove_season_lbl);
            remove_season.Location = new System.Drawing.Point(460, 0);
            remove_season.Name = "remove_panel";
            remove_season.Size = new System.Drawing.Size(100, 20);
            remove_season.TabIndex = 2;
            // 
            // remove_panel_label
            // 
            remove_season_lbl.BackColor = System.Drawing.Color.Red;
            remove_season_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            remove_season_lbl.Location = new System.Drawing.Point(0, 0);
            remove_season_lbl.Name = "remove_panel_label";
            remove_season_lbl.Size = new System.Drawing.Size(100, 20);
            remove_season_lbl.TabIndex = 0;
            remove_season_lbl.Text = "-";
            remove_season_lbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            remove_season_lbl.Click += new System.EventHandler(this.remove_panel_Click);

            Controls.Add(add_season);
            Controls.Add(remove_season);
        }

        public void Draw()
        {
            Controls.Clear();
            Controls.Add(add_season);
            Controls.Add(remove_season);
            int height = 10;
            for (int i = 0; i < seasonList.Count; i++)
            {
                if (i != 0) height += seasonList[i - 1].Height + 5;
                Controls.Add(seasonList[i]);

                seasonList[i].Location = new Point(seasonList[i].Location.X, height);
                seasonList[i].BringToFront();
                seasonList[i].Draw();
            }
            if (seasonList.Count > 0) height += seasonList[seasonList.Count - 1].Height + 5;
            add_season.Location = new Point(add_season.Location.X, height);
            remove_season.Location = new Point(remove_season.Location.X, height);
        }

        public List<SeasonPanel> getSeasonList()
        {
            return seasonList;
        }
        public SeasonPanel addSeason(bool draw = false)
        {
            SeasonPanel new_season = new SeasonPanel(seasonList.Count);
            new_season.DragEnter += new DragEventHandler(dragEnter);
            new_season.DragDrop += new DragEventHandler(dragDrop);
            if (draw) Draw();
            return new_season;
        }

        void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text)) e.Effect = DragDropEffects.Copy;
        }
        void dragDrop(object sender, DragEventArgs e)
        {
            int dropped = 0;
            for (int i = parent.unk_panel.getReferences().Count - 1; i >= 0; i--)
            {
                Reference x = parent.unk_panel.getReferences()[i];
                if (x.isSelected() || x.text.Name == (string)e.Data.GetData(DataFormats.Text))
                {
                    parent.MoveToSeason(x, (SeasonPanel)sender, false);
                    dropped += 1;
                }
            }
            foreach (SeasonPanel pan in getSeasonList())
            {
                for (int i = pan.getReferences().Count - 1; i >= 0; i--)
                {
                    Reference x = pan.getReferences()[i];
                    if (x.isSelected() || x.text.Name == (string)e.Data.GetData(DataFormats.Text))
                    {
                        if (!((SeasonPanel)sender).getReferences().Contains(x))
                        {
                            parent.MoveToSeason(x, (SeasonPanel)sender, false);
                            dropped += 1;
                        }
                    }
                }
            }
                
            if (dropped > 0)
            {
                Draw();
                parent.unk_panel.Draw();
            }
        }

        public void addSeasonToList()
        {
            seasonList.Add(addSeason());
        }
        private void add_panel_Click(object sender, EventArgs e)
        {
            addSeasonToList();
            Draw();
        }

        private void remove_panel_Click(object sender, EventArgs e)
        {
            if (seasonList.Count <= 1) return;
            for (int i = seasonList[seasonList.Count - 1].getReferences().Count - 1; i >= 0; i--)
            {
                parent.MoveToUnknownPanel(seasonList[seasonList.Count - 1].getReferences()[i], false);
            }
            seasonList.RemoveAt(seasonList.Count - 1);
            parent.unk_panel.Draw();
            Draw();
        }
    }
}