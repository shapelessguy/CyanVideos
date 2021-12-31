using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyanVideos
{
    public class Drawning
    {
        static string fontTitle = "Cambria";
        static string fontSource = "Cambria";

        static public Bitmap rightExclamation1;
        static public Bitmap rightQuestion1;
        static public int height_rightExclamation1;
        static public int height_rightQuestion1;
        static public Bitmap rightExclamation2;
        static public Bitmap rightQuestion2;
        static public int height_rightExclamation2;
        static public int height_rightQuestion2;
        static public Bitmap Previous;
        public Rectangle Annotation = new Rectangle(0, 0, 0, 0);
        public Rectangle ExtraRect = new Rectangle(0, 0, 0, 0);
        public Rectangle NameRect = new Rectangle(0, 0, 0, 0);
        public Rectangle ChangeImageRect = new Rectangle(0, 0, 0, 0);
        public Rectangle Continue = new Rectangle(0, 0, 0, 0);
        public Rectangle PrevRect;
        public bool prev = false;
        public bool extrainfo = false;   //Info
        public bool extraname = false;   // name label
        public bool extrachangeIcon = false;   //change icon to folders

        public Iconxx Icon;
        public Source source;
        public Bitmap image;
        public Bitmap imageBlurred;
        public Rectangle Rect;
        public string text;
        public bool tooltiped = false;

        public int sourceNum;

        public void Dispose()
        {
            if (image != null) image.Dispose();
            if (imageBlurred != null) imageBlurred.Dispose();
            //if (rightExclamation1 != null) rightExclamation1.Dispose();
            //if (rightQuestion1 != null) rightQuestion1.Dispose();
            //if (rightExclamation2 != null) rightExclamation2.Dispose();
            //if (rightQuestion2 != null) rightQuestion2.Dispose();
        }
        public Drawning(Iconxx icon, int locx, int locy, int width, int height, int panelLevel, int sourceNum)
        {
            this.Icon = icon;
            this.sourceNum = sourceNum;
            image = new Bitmap(width, height);
            var graph = Graphics.FromImage(image);
            try
            {
                graph.DrawImage(icon.Image, 0, 0, width, height);
                if (icon.series) DrawTitle(graph, width, height);
            }
            catch (Exception) { Console.WriteLine("Exception raised by Drawning -> "+icon.title); return; }
            SetAnnotations(height, panelLevel);
            imageBlurred = new Bitmap(icon.ImageBlurred, width, height);
            Icon.image_validated = true;
            //Console.WriteLine(icon.title + "  withResizing");

            this.Rect.Location = new Point(locx, locy);
            this.Rect.Size = new Size(width, height);
        }
        private void DrawTitle(Graphics graph, int width, int height)
        {
            Rectangle rect = new Rectangle(0, 2 * height / 3, width + 4, height / 3 + 8);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            graph.DrawImage(Properties.Resources.Namelabel, rect);
            float fontSize = (float)(width) / 400 * 28;
            //Console.WriteLine(fontSize);
            graph.DrawString(Icon.title, new Font(fontTitle, fontSize, FontStyle.Bold), Brushes.Red, rect, sf);
        }
        public Drawning(Iconxx icon, int locx, int locy, int panelLevel, int sourceNum)
        {
            this.Icon = icon;
            this.sourceNum = sourceNum;
            image = (Bitmap)icon.Image.Clone();
            imageBlurred = new Bitmap(icon.ImageBlurred, image.Width, image.Height);
            Icon.image_validated = true;
            //Console.WriteLine(icon.title + "  withoutResizing");
            this.Rect.Location = new Point(locx, locy);
            this.Rect.Size = new Size(image.Width, image.Height);
        }
        public Drawning(Source source, int locx, int locy, int width, int height, int sourceNum)
        {
            this.source = source;
            this.sourceNum = sourceNum;
            this.text = source.tag.name;
            if (locx <= height) { locx += height; width -= height; }
            if (source.directory != MyPanel.fatherPanel2) prev = true;
            if (prev)
            {
                Previous = new Bitmap(height, height);
                var graph = Graphics.FromImage(Previous);
                graph.DrawImage(Properties.Resources.left_arrow, 0, 0, height, height);
                this.PrevRect.Location = new Point(locx, locy);
                this.PrevRect.Size = new Size(height, height);
            }
            else
            {
                this.PrevRect.Location = new Point(0, 0);
                this.PrevRect.Size = new Size(0, 0);
            }

            this.Rect.Location = new Point(locx, locy);
            this.Rect.Size = new Size(width, height);
        }
        public Drawning(string text, int locx, int locy, int width, int height, int sourceNum)
        {
            this.sourceNum = sourceNum;
            this.text = text;
            this.Rect.Location = new Point(locx, locy);
            this.Rect.Size = new Size(width, height);
        }
        public void ShowExtra()
        {
            if (tooltiped) return;
            tooltiped = true;
            if (Icon != null)
            {
                if (Icon.series && !Icon.in_first_panel) return;
                if ((Icon.reliability == 4 || Icon.reliability == 1) && (!Icon.folder || Icon.in_first_panel)) extrainfo = true;
                if (Icon.reliability == 4 && (Icon.folder || Icon.in_first_panel)) extrachangeIcon = true;
                if (Icon.reliability < 4 || (Icon.folder || Icon.in_first_panel)) extraname = true;
            }
        }
        public void HideExtra()
        {
            tooltiped = false;
            extrainfo = false;
            extraname = false;
            extrachangeIcon = false;
        }
        public void ValidateImage()
        {
            bool nullIcon = true;
            if (Icon.imagePath != "null" && Icon.imagePath != "folder") nullIcon = false;
            image = new Bitmap(Rect.Width, Rect.Height);
            imageBlurred = new Bitmap(Icon.ImageBlurred, image.Width, image.Height);
            var graph = Graphics.FromImage(image);
            graph.DrawImage(Icon.Image, 0, 0, Rect.Width, Rect.Height);
            if (Icon.series) DrawTitle(graph, Rect.Width, Rect.Height);
            if (!nullIcon) { Icon.image_validated = true; Console.WriteLine("Icon validated: "+Icon.title + ":  " + Icon.imagePath); }
        }
        private void SetAnnotations(int iconheight, int panelLevel)
        {
            if (panelLevel == 1)
            {
                if (rightExclamation1 == null || height_rightExclamation1 != iconheight / 3)
                {
                    height_rightExclamation1 = iconheight / 3;
                    rightExclamation1 = new Bitmap(iconheight / 5, iconheight / 3);
                    var graph = Graphics.FromImage(rightExclamation1);
                    graph.DrawImage(Properties.Resources.exclamation, 0, 0, iconheight / 5, iconheight / 3);
                }
                if (rightQuestion1 == null || height_rightQuestion1 != iconheight / 3)
                {
                    height_rightQuestion1 = iconheight / 3;
                    rightQuestion1 = new Bitmap(iconheight / 5, iconheight / 3);
                    var graph = Graphics.FromImage(rightQuestion1);
                    graph.DrawImage(Properties.Resources._2753, 0, 0, iconheight / 5, iconheight / 3);
                }
            }
            else
            {
                if (rightExclamation2 == null || height_rightExclamation2 != iconheight / 3)
                {
                    height_rightExclamation2 = iconheight / 3;
                    rightExclamation2 = new Bitmap(iconheight / 5, iconheight / 3);
                    var graph = Graphics.FromImage(rightExclamation2);
                    graph.DrawImage(Properties.Resources.exclamation, 0, 0, iconheight / 5, iconheight / 3);
                }
                if (rightQuestion2 == null || height_rightQuestion2 != iconheight / 3)
                {
                    height_rightQuestion2 = iconheight / 3;
                    rightQuestion2 = new Bitmap(iconheight / 5, iconheight / 3);
                    var graph = Graphics.FromImage(rightQuestion2);
                    graph.DrawImage(Properties.Resources._2753, 0, 0, iconheight / 5, iconheight / 3);
                }
            }
        }
        public void Draw(Graphics graph, int panelLevel, int bias = 0)
        {
            if (Icon != null)
            {
                bool condition = MyPanel.i != MyPanel.initial_i && MyPanel.i != 1;
                if (MyPanel.scroll_enabled && condition) graph.DrawImage(imageBlurred, Rect.Location.X, Rect.Location.Y + bias);
                else graph.DrawImage(image, Rect.Location.X, Rect.Location.Y + bias);
                if (Properties.Settings.Default.exclamationsEnabled)
                {
                    if (panelLevel == 1)
                    {
                        if (Icon.reliability == 3) graph.DrawImage(rightExclamation1, Rect.Location.X, Rect.Location.Y + bias);
                        else if (Icon.reliability == 2) graph.DrawImage(rightQuestion1, Rect.Location.X, Rect.Location.Y + bias);
                    }
                    else
                    {
                        if (Icon.reliability == 3) graph.DrawImage(rightExclamation2, Rect.Location.X, Rect.Location.Y + bias);
                        else if (Icon.reliability == 2) graph.DrawImage(rightQuestion2, Rect.Location.X, Rect.Location.Y + bias);
                    }
                    AdjustOtherThings();
                }
            }
            else if(source != null)
            {
                Rectangle rect = new Rectangle(Rect.Location.X, Rect.Location.Y + bias, Rect.Width - 40, Rect.Height);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                //float fontSize = (float)(Rect.Width) / 400 * 28;
                graph.DrawString(text, new Font(fontSource, 24, FontStyle.Bold), Brushes.White, rect, sf);
                if(panelLevel == 2 && prev) graph.DrawImage(Previous, Rect.Location.X, Rect.Location.Y + bias);
            }
            else
            {
                Rectangle rect = new Rectangle(Rect.Location.X, Rect.Location.Y + bias, Rect.Width - 40, Rect.Height);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graph.DrawString(text, new Font(fontSource, 24, FontStyle.Bold), Brushes.White, rect, sf);
            }
        }
        private void AdjustOtherThings()
        {
            if (Icon != null && Icon.series && !Icon.in_first_panel) return;
            if (Icon.reliability == 2 || Icon.reliability == 3)
            {
                //Console.WriteLine(Icon.title);
                NameRect = new Rectangle(Rect.Location.X, Rect.Location.Y + 2 * Rect.Height / 3 -2, Rect.Width + 4, Rect.Height / 3 + 4);
                Annotation = new Rectangle(Rect.Location.X, Rect.Location.Y, image.Height / 5, image.Height / 3);
                ExtraRect = new Rectangle(0, 0, 0, 0);
            }
            else if ((Icon.reliability == 4 || Icon.reliability == 1) && (!Icon.folder || Icon.in_first_panel))
            {
                ExtraRect = new Rectangle(Rect.Location.X, Rect.Location.Y, Rect.Width + 4, Rect.Height / 5 + 4);
                Annotation = new Rectangle(0, 0, 0, 0);
                ChangeImageRect = new Rectangle(0, 0, 0, 0);
            }
            else if (Icon.reliability == 4 && Icon.folder && !Icon.in_first_panel)
            {
                ChangeImageRect = new Rectangle(Rect.Location.X + 7 * Rect.Width / 8 + 4, Rect.Location.Y, Rect.Width / 8 + 4, Rect.Height / 2 + 4);
                Annotation = new Rectangle(0, 0, 0, 0);
                ExtraRect = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                NameRect = new Rectangle(Rect.Location.X, Rect.Location.Y + 2 * Rect.Height / 3 -2 , Rect.Width + 4, Rect.Height / 3 + 4);
                ExtraRect = new Rectangle(0, 0, 0, 0);
                ChangeImageRect = new Rectangle(0, 0, 0, 0);
                Annotation = new Rectangle(0, 0, 0, 0);
            }
            if (Icon.ContinueToWatch != null)
            {
                int height = Rect.Height - (Rect.Height / 5 + 4) - (Rect.Height / 2 + 4);
                Continue = new Rectangle(Rect.Location.X + Rect.Width - (int)(height * 0.8), Rect.Location.Y + Rect.Height / 2 + 4 + (int)(height * 0.2), (int)(height * 0.6), (int)(height * 0.6));

                if (!extraname && !Icon.folder)
                {
                    Continue = new Rectangle(Rect.Location.X + Rect.Width - (int)(height * 0.6), Rect.Location.Y + Rect.Height - ((int)(height * 0.6)), (int)(height * 0.6), (int)(height * 0.6));
                }

            }
        }
        public void HighlightPrevious(Graphics graph, int panelLevel, int bias)
        {
            if (panelLevel == 2 && prev)
            {
                Bitmap clone = new Bitmap(Rect.Size.Height + 4, Rect.Size.Height + 4);
                var graph_temp = Graphics.FromImage(clone);
                graph_temp.DrawImage(Previous, 0, 0, Rect.Size.Height + 4, Rect.Size.Height + 4);
                graph.DrawImage(clone, Rect.Location.X - 2, Rect.Location.Y - 2 + bias, Rect.Size.Height + 4, Rect.Size.Height + 4);
            }
        }
        
        public void DrawMaximized(Graphics graph, int panelLevel, int bias)
        {
            try
            {
                if (source != null) HighlightPrevious(graph, panelLevel, bias);
                else if(Icon != null)
                {
                    Bitmap clone = new Bitmap(Rect.Size.Width + 4, Rect.Size.Height + 4);
                    var graph_temp = Graphics.FromImage(clone);
                    graph_temp.DrawImage(Icon.Image, 0, 0, Rect.Size.Width + 4, Rect.Size.Height + 4);
                    
                    if (Properties.Settings.Default.exclamationsEnabled)
                    {
                        if (panelLevel == 1)
                        {
                            if (Icon.reliability == 3) graph_temp.DrawImage(rightExclamation1, 2, 2);
                            if (Icon.reliability == 2) graph_temp.DrawImage(rightQuestion1, 2, 2);
                        }
                        else
                        {
                            if (Icon.reliability == 3) graph_temp.DrawImage(rightExclamation2, 2, 2);
                            if (Icon.reliability == 2) graph_temp.DrawImage(rightQuestion2, 2, 2);
                        }
                    }
                    if (extrainfo) graph_temp.DrawImage(Properties.Resources.INFO_, 0, 0, Rect.Width + 6, Rect.Height / 5 + 4);
                    if (extraname || Icon.series)
                    {
                        Rectangle rect = new Rectangle(0, 2 * Rect.Height / 3 + 2, Rect.Width + 10, Rect.Height / 3 + 10);
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        graph_temp.DrawImage(Properties.Resources.Namelabel, rect);
                        float fontSize = (float)(Rect.Width) / 400 * 28;
                        graph_temp.DrawString(Icon.title, new Font(fontTitle, fontSize, FontStyle.Bold), Brushes.Red, rect, sf);
                    }
                    if ((extrainfo || extrachangeIcon || extraname) && Icon.ContinueToWatch != null)
                    {
                        int height = Rect.Height - (Rect.Height / 5 + 4) - (Rect.Height / 2 + 4);
                        Rectangle rect1 = new Rectangle(0, Rect.Height / 2 + 4 + (int)(height * 0.4), Rect.Width + 10 - (int)(height * 0.4), (int)(height*0.4));
                        Rectangle rect2 = new Rectangle(Rect.Width - (int)(height * 0.4), Rect.Height / 2 + 4 + (int)(height * 0.4), (int)(height * 0.4), (int)(height * 0.4));

                        if (!extraname)
                        {
                            rect1 = new Rectangle(0, Rect.Height - (int)(height * 0.4), Rect.Width + 10 - (int)(height * 0.4), (int)(height * 0.4));
                            rect2 = new Rectangle(Rect.Width - (int)(height * 0.4), Rect.Height - ((int)(height * 0.4)), (int)(height * 0.4), (int)(height * 0.4));
                        }
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        graph_temp.DrawImage(Properties.Resources.Continue, rect1);
                        graph_temp.DrawImage(Properties.Resources.playH, rect2);
                        float fontSize = (float)(Rect.Width) / 400 * 16 + 7;
                        if (fontSize < 10) fontSize = 12;
                        graph_temp.DrawString("Continua visione", new Font(fontTitle, fontSize, FontStyle.Bold), Brushes.Black, rect1, sf);
                    }
                    if (extrachangeIcon && !Icon.in_first_panel)
                    {
                        Rectangle rect = new Rectangle(7 * Rect.Width / 8 + 4, 0, Rect.Width / 8 + 4, Rect.Height / 2 + 4);
                        graph_temp.DrawImage(Properties.Resources.Change, rect);
                    }
                    graph.DrawImage(clone, Rect.Location.X - 2, Rect.Location.Y - 2 + bias, Rect.Size.Width + 2, Rect.Size.Height + 4);
                }
            }
            catch (Exception) { Console.WriteLine("Exception from DrawMaximized"); }
        }
    }
}
