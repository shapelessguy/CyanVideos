using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;

namespace CyanVideos.SeasonEditor
{
    public class SeasonPanel : Panel
    {
        public SeasonEditor editor;
        private List<Reference> references;
        private Label seasonName;
        private TextBox seasonTextBox;
        private Button selectBtn;
        public int index = 0;
        public static string supplement_name = "Supplement";
        public SeasonPanel(int n)
        {
            references = new List<Reference>();
            BackColor = Color.FromArgb(10, 10, 10);
            Location = new Point(0, 0);
            Name = "s_panel" + n;
            AllowDrop = true;
            Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

            seasonName = new Label();
            seasonName.Location = new Point(0, 0);
            seasonName.Size = n > 0 ? new Size(430, 30) : new Size(490, 30);
            seasonName.ForeColor = Color.White;
            seasonName.BackColor = BackColor;
            seasonName.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            if (n == 0)
            {
                seasonName.Name = supplement_name.ToLower();
                seasonName.Text = supplement_name;
            }
            else
            {
                seasonName.Name = "s" + n;
                seasonName.Text = "Season " + n;
            }
            index = n;
            seasonName.TabIndex = 0;
            seasonName.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(seasonName);

            seasonTextBox = new TextBox();
            seasonTextBox.Location = new Point(440, 2);
            seasonTextBox.BackColor = BackColor;
            seasonTextBox.ForeColor = Color.White;
            seasonTextBox.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            seasonTextBox.Name = "seasonTextBox_" + n;
            seasonTextBox.Text = seasonName.Text;
            seasonTextBox.Size = new Size(300, 30);
            seasonTextBox.TabIndex = 0;
            seasonTextBox.Visible = n > 0 ? true : false;
            Controls.Add(seasonTextBox);

            selectBtn = new Button();
            selectBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            selectBtn.FlatAppearance.BorderSize = 0;
            selectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            selectBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            selectBtn.Location = new System.Drawing.Point(745, 5);
            selectBtn.Name = "selectAll_" + n;
            selectBtn.Size = new System.Drawing.Size(80, 20);
            selectBtn.TabIndex = 4;
            selectBtn.Text = "Select All";
            selectBtn.UseVisualStyleBackColor = false;
            selectBtn.Click += new System.EventHandler(this.button1_Click);
            Controls.Add(selectBtn);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selected_n = 0;
            foreach (Reference ref_ in references)
            {
                if (ref_.isSelected()) selected_n += 1;
            }

            editor.DeselectKnownPanel(this);
            editor.DeselectUnKnownPanel();

            foreach (Reference ref_ in references)
            {
                if (selected_n == references.Count) ref_.ChangeSelect();
                else ref_.ChangeSelect(true);
            }
        }

        public void Clear()
        {
            references.Clear();
        }

        public void setSeasonTextBox(string text)
        {
            if (text != "") seasonTextBox.Text = text;
        }
        public List<Reference> getReferences()
        {
            return references;
        }
        public void AddReference(Reference reference)
        {
            reference.ShowTextbox(true);
            references.Add(reference);
        }
        public void RemoveReference(Reference reference)
        {
            references.Remove(reference);
        }

        public void Draw()
        {
            references = references.OrderBy(x => x.index).ToList();
            Controls.Clear();
            Controls.Add(seasonName);
            Controls.Add(seasonTextBox);
            Controls.Add(selectBtn);
            int height = seasonName.Height;
            for (int i = 0; i < references.Count; i++)
            {
                references[i].Location = new Point(references[i].Location.X, height);
                Controls.Add(references[i]);
                height += references[i].Height; // + 5;
            }
            Size = new Size(860, height);
        }

        public List<Action> getActions(string working_dir)
        {
            List<Action> movements = new List<Action>();
            string seas_name = seasonTextBox.Text != "" ? seasonTextBox.Text : seasonName.Text;
            string index_text = Program.FormatNumberZero(index);
            string seasonFolder = Path.Combine(working_dir, "(" + index_text + ")" + seas_name);
            movements.Add(new Create(seasonFolder));
            for (int i = 0; i < references.Count; i++)
            {
                Reference ref_ = references[i];
                string n_index_text = Program.FormatNumberZero(i + 1);
                string new_dir = Path.Combine(seasonFolder, index_text + "x" + n_index_text);
                Create create_dir = new Create(new_dir);
                string name = ref_.textBox.Text != "" ? ref_.textBox.Text : ref_.clean_name;
                Movement move_movie = new Movement(ref_.path, Path.Combine(seasonFolder, new_dir, name + ref_.extension));

                string[] directories = Directory.GetDirectories(ref_.directory);
                string[] files = Directory.GetFiles(ref_.directory);

                movements.Add(create_dir);
                movements.Add(move_movie);

                int n_images = 0;
                foreach (var file in files) if (Path.GetFileName(file).Contains("imagefromPowerVideos")) n_images++;
                foreach (var file in files)
                {
                    if (!Program.IsVideo(file))
                    {
                        if (!Path.GetFileName(file).Contains("imagefromPowerVideos") || n_images == 1)
                        {
                            Copy copy_file = new Copy(file, Path.Combine(new_dir, Path.GetFileName(file)));
                            movements.Add(copy_file);
                        }
                    }
                }
                foreach (var dir in directories)
                {
                    Copy copy_folders = new Copy(dir, Path.Combine(new_dir, Path.GetFileName(dir)));
                    movements.Add(copy_folders);
                }
            }
            return movements;
        }
    }
}
