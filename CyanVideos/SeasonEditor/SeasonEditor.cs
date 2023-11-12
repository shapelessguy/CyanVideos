using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos.SeasonEditor
{
    public partial class SeasonEditor : Form
        {
        public UnknownPanel unk_panel = new UnknownPanel();
        public KnownPanel known_panel = new KnownPanel();
        public string main_folder;
        public SeasonEditor(string main_folder)
        {
            InitializeComponent();
            reload_icon.BackgroundImage = Properties.Resources.Reload_icon;
            reload_icon.BackgroundImageLayout = ImageLayout.Stretch;
            this.main_folder = main_folder;

            Controls.Add(unk_panel);
            Controls.Add(known_panel);
            shapeButton.Text = "Shape Folder \u27F6";
            Initialize();
        }

        private void Initialize()
        {
            unk_panel.Clear();
            known_panel.Clear();
            unk_panel.Initialize(new ReferencesInjector(main_folder).GetReferences(), this);
            known_panel.Initialize(this);
            Draw();
        }

        public void MoveToSeason(Reference x, SeasonPanel season, bool draw = false)
        {
            unk_panel.removeReference(x);
            foreach (SeasonPanel panel in known_panel.getSeasonList())
            {
                if (panel.getReferences().Contains(x)) panel.RemoveReference(x);
            }
            season.AddReference(x);
            if (draw) Draw();
        }

        public void MoveToUnknownPanel(Reference x, bool draw=false)
        {
            foreach (SeasonPanel season in known_panel.getSeasonList())
            {
                season.RemoveReference(x);
            }
            unk_panel.addReference(x);
            if (draw) Draw();
        }

        private void Draw()
        {
            SuspendLayout();
            unk_panel.Draw();
            known_panel.Draw();
            ResumeLayout(false);
        }

        private void shapeButton_Click(object sender, EventArgs e)
        {
            if (unk_panel.getReferences().Count > 0)
            {
                string msg = "Unliked references will be moved to " + SeasonPanel.supplement_name + ". Are you sure this behaviour is intended?";
                DialogResult dialogResult = MessageBox.Show(msg, "Missing links", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            log.Text = "Reshaping ...";
            ResumeLayout(true);

            bool to_draw = false;
            if (known_panel.getSeasonList().Count == 0)
            {
                to_draw = true;
                known_panel.getSeasonList().Add(known_panel.addSeason());
            }
            for (int i = unk_panel.getReferences().Count - 1; i >= 0; i--)
            {
                to_draw = true;
                Reference x = unk_panel.getReferences()[i];
                MoveToSeason(x, known_panel.getSeasonList()[0]);
            }
            if (to_draw) Draw();

            string working_dir = Path.Combine(Path.GetDirectoryName(main_folder), "CyanVisionWorkingDirectory");
            string old_folder = Path.Combine(Path.GetDirectoryName(main_folder), "CyanVisionTrash_" + Path.GetFileName(main_folder));
            List<Action> actions = new List<Action>();
            actions.Add(new Create(working_dir));
            foreach (SeasonPanel panel in known_panel.getSeasonList()) actions.AddRange(panel.getActions(working_dir));
            foreach (var file in Directory.GetFiles(main_folder))
            {
                if (Path.GetFileName(file) == "imagefromPowerVideos.jpg" || Path.GetFileName(file) == "infopowervideos.txt")
                {
                    actions.Add(new Copy(file, Path.Combine(working_dir, Path.GetFileName(file))));
                }
            }
            actions = orderActions(actions);
            actions.Add(new Create(main_folder, true));
            actions.Add(new Movement(working_dir, main_folder));
            if (ActionPlayer(actions))
            {
                log.Text = "Reshaping successful!";
                Initialize();
            }
            else
            {
                log.Text = "Error while reshaping :(";
            }
        }

        private List<Action> orderActions(List<Action> actions)
        {
            List<Action> ordered_actions = new List<Action>();
            for (int i = 0; ; i++)
            {
                if (i >= actions.Count) break;
                if (actions[i].GetType().Name == "Create" && !((Create)actions[i]).rev_op)
                {
                    ordered_actions.Add(actions[i]);
                    actions.RemoveAt(i);
                    i -= 1;
                }
            }
            for (int i = 0; ; i++)
            {
                if (i >= actions.Count) break;
                if (actions[i].GetType().Name == "Movement" && !((Movement)actions[i]).is_dir)
                {
                    ordered_actions.Add(actions[i]);
                    actions.RemoveAt(i);
                    i -= 1;
                }
            }
            for (int i = 0; ; i++)
            {
                if (i >= actions.Count) break;
                if (actions[i].GetType().Name == "Copy")
                {
                    ordered_actions.Add(actions[i]);
                    actions.RemoveAt(i);
                    i -= 1;
                }
            }
            for (int i = 0; ; i++)
            {
                if (i >= actions.Count) break;
                ordered_actions.Add(actions[i]);
                actions.RemoveAt(i);
                i -= 1;
            }
            return ordered_actions;
        }

        private bool ActionPlayer(List<Action> actions)
        {
            int i = 0;
            try
            {
                for (i = 0; i < actions.Count; i++)
                {
                    bool performed = false;
                    actions[i].toString();
                    for (int j = 0; j < 5; j++)
                    {
                        try
                        {
                            actions[i].PerformAction();
                            performed = true;
                            break;
                        }
                        catch { Thread.Sleep(500); }
                    }
                    if (!performed) { throw new Exception(); }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\nNow I am trying to revert all the changes!");
                Console.WriteLine("ERROR");
                i -= 1;
                try
                {
                    for (; i >= 0; i--)
                    {
                        actions[i].PerformAction(true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nSorry I messed up!!!");
                    return false;
                }
                return false;
            }
            return true;
        }

        private void reload_icon_Click(object sender, EventArgs e)
        {
            Initialize();
        }

        public void DeselectUnKnownPanel()
        {
            foreach (Reference ref_ in unk_panel.getReferences())
            {
                ref_.ChangeSelect(true, false);
            }
        }

        public void DeselectKnownPanel(SeasonPanel except = null)
        {
            foreach (SeasonPanel pan in known_panel.getSeasonList())
            {
                if (pan != except)
                {
                    foreach (Reference ref_ in pan.getReferences())
                    {
                        ref_.ChangeSelect(true, false);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selected_n = 0;
            foreach (Reference ref_ in unk_panel.getReferences())
            {
                if (ref_.isSelected()) selected_n += 1;
            }

            DeselectKnownPanel();

            foreach (Reference ref_ in unk_panel.getReferences())
            {
                if (selected_n == unk_panel.getReferences().Count) ref_.ChangeSelect();
                else ref_.ChangeSelect(true);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DeselectKnownPanel();
            DeselectUnKnownPanel();
        }
    }
}