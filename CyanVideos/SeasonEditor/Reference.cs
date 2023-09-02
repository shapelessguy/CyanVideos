using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace CyanVideos.SeasonEditor
{
    public class Reference : Label
    {
        public int index = 0;
        public Label text;
        public TextBox textBox;
        public string path;
        public string directory;
        public string name;
        public string clean_name;
        public string extension;
        private bool selected = false;

        public Reference(int index)
        {
            this.index = index;

            AutoSize = false;
            BackColor = Color.FromArgb(10, 10, 10);
            ForeColor = Color.White;

            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            Location = new Point(10, 0);
            Size = new Size(580, 20);
            TextAlign = ContentAlignment.MiddleCenter;

            text = new Label();
            text.AutoSize = false;
            text.Location = new Point(0, 0);
            text.BackColor = BackColor;
            text.ForeColor = ForeColor;
            text.Name = "text_" + index;
            text.Size = new System.Drawing.Size(460, 20);
            text.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(text);

            textBox = new TextBox();
            textBox.Location = new Point(500, 5);
            textBox.BackColor = BackColor;
            textBox.ForeColor = ForeColor;
            textBox.Name = "textBox_" + index;
            textBox.Size = new Size(300, 20);
            textBox.TabIndex = 0;
            textBox.Visible = true;
            Controls.Add(textBox);
        }
        public void Initialize()
        {
            clean_name = Program.CleanName(name);
            setText(name + extension);
        }
        public void setText(string text)
        {
            this.text.Text = text;
            textBox.Text = clean_name;
        }

        public Label getTextLabel()
        {
            return text;
        }

        public bool isSelected()
        {
            return selected;
        }

        public void ChangeSelect(bool force = false, bool seleted_ = true)
        {
            if (force) selected = seleted_;
            else selected = !selected;
            Selected();
        }
        public void Selected()
        {
            if (selected)
            {
                BackColor = Color.FromArgb(50, 50, 50);
                text.BackColor = Color.FromArgb(50, 50, 50);
            }
            else
            {
                BackColor = Color.FromArgb(10, 10, 10);
                text.BackColor = Color.FromArgb(10, 10, 10);
            }
        }

        public void ShowTextbox(bool value)
        {
            textBox.Visible = value;
            if (Parent != null)
            {
                if (value) // On Known Panel
                {
                    TextAlign = ContentAlignment.MiddleRight;
                    Size = new System.Drawing.Size(830, 30);
                    text.Size = new System.Drawing.Size(460, 20);
                    Location = new Point(20, textBox.Location.Y);
                    text.Location = new Point(20, textBox.Location.Y);
                }
                else // On Unknown Panel
                {
                    TextAlign = ContentAlignment.MiddleCenter;
                    Size = new System.Drawing.Size(580, 30);
                    text.Size = new System.Drawing.Size(560, 20);
                    Location = new Point(10, textBox.Location.Y);
                    text.Location = new Point(10, textBox.Location.Y);
                }
            }
            selected = false;
            Selected();
        }
    }
}
