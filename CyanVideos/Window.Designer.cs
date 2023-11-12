using System;
using System.Drawing;

namespace CyanVideos
{
    partial class Window
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                base.Dispose(disposing);
            }
            catch (Exception) { }
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.menu1 = new System.Windows.Forms.ToolStripMenuItem();
            this.add_source = new System.Windows.Forms.ToolStripMenuItem();
            this.addSourceSeries = new System.Windows.Forms.ToolStripMenuItem();
            this.update_source = new System.Windows.Forms.ToolStripMenuItem();
            this.visualizzaControversieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compact = new System.Windows.Forms.ToolStripMenuItem();
            this.mostraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tuttoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soloFilmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soloSerieTVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorPredefinitoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.internalPlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.Keygrip1 = new System.Windows.Forms.TextBox();
            this.Keygrip2 = new System.Windows.Forms.TextBox();
            this.consigliaStrutturaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // Menu
            // 
            this.Menu.AutoSize = false;
            this.Menu.BackColor = System.Drawing.Color.Black;
            this.Menu.Dock = System.Windows.Forms.DockStyle.None;
            this.Menu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Menu.ForeColor = System.Drawing.Color.White;
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu1});
            this.Menu.Location = new System.Drawing.Point(953, 9);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(83, 34);
            this.Menu.TabIndex = 0;
            this.Menu.Text = "menuStrip1";
            // 
            // menu1
            // 
            this.menu1.BackColor = System.Drawing.Color.Black;
            this.menu1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menu1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.add_source,
            this.addSourceSeries,
            this.update_source,
            this.visualizzaControversieToolStripMenuItem,
            this.compact,
            this.mostraToolStripMenuItem,
            this.monitorPredefinitoToolStripMenuItem,
            this.internalPlayer,
            this.consigliaStrutturaToolStripMenuItem,
            this.languageTagsToolStripMenuItem
            });
            this.menu1.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu1.ForeColor = System.Drawing.Color.White;
            this.menu1.Name = "menu1";
            this.menu1.Size = new System.Drawing.Size(67, 30);
            this.menu1.Text = "MENU";
            this.menu1.Click += new System.EventHandler(this.MenuClick);
            // 
            // add_source
            // 
            this.add_source.BackColor = System.Drawing.Color.Black;
            this.add_source.ForeColor = System.Drawing.Color.White;
            this.add_source.Name = "add_source";
            this.add_source.Size = new System.Drawing.Size(293, 24);
            this.add_source.Click += new System.EventHandler(this.AddSource);
            // 
            // addSourceSeries
            // 
            this.addSourceSeries.BackColor = System.Drawing.Color.Black;
            this.addSourceSeries.ForeColor = System.Drawing.Color.White;
            this.addSourceSeries.Name = "addSourceSeries";
            this.addSourceSeries.Size = new System.Drawing.Size(293, 24);
            this.addSourceSeries.Click += new System.EventHandler(this.AddSourceSeries);
            // 
            // update_source
            // 
            this.update_source.BackColor = System.Drawing.Color.Black;
            this.update_source.ForeColor = System.Drawing.Color.White;
            this.update_source.Name = "update_source";
            this.update_source.Size = new System.Drawing.Size(293, 24);
            this.update_source.Click += new System.EventHandler(this.update_source_Click);
            // 
            // visualizzaControversieToolStripMenuItem
            // 
            this.visualizzaControversieToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.visualizzaControversieToolStripMenuItem.CheckOnClick = true;
            this.visualizzaControversieToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.visualizzaControversieToolStripMenuItem.Name = "visualizzaControversieToolStripMenuItem";
            this.visualizzaControversieToolStripMenuItem.Size = new System.Drawing.Size(293, 24);
            this.visualizzaControversieToolStripMenuItem.Click += new System.EventHandler(this.Exclamative_Click);
            // 
            // compact
            // 
            this.compact.BackColor = System.Drawing.Color.Black;
            this.compact.CheckOnClick = true;
            this.compact.ForeColor = System.Drawing.Color.White;
            this.compact.Name = "compact";
            this.compact.Size = new System.Drawing.Size(293, 24);
            this.compact.Click += new System.EventHandler(this.compact_Click);
            // 
            // mostraToolStripMenuItem
            // 
            this.mostraToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.mostraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tuttoToolStripMenuItem,
            this.soloFilmToolStripMenuItem,
            this.soloSerieTVToolStripMenuItem});
            this.mostraToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.mostraToolStripMenuItem.Name = "mostraToolStripMenuItem";
            this.mostraToolStripMenuItem.Size = new System.Drawing.Size(293, 24);
            // 
            // tuttoToolStripMenuItem
            // 
            this.tuttoToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.tuttoToolStripMenuItem.CheckOnClick = true;
            this.tuttoToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.tuttoToolStripMenuItem.Name = "tuttoToolStripMenuItem";
            this.tuttoToolStripMenuItem.Size = new System.Drawing.Size(190, 24);
            this.tuttoToolStripMenuItem.Click += new System.EventHandler(this.ViewAll_Click);
            // 
            // soloFilmToolStripMenuItem
            // 
            this.soloFilmToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.soloFilmToolStripMenuItem.CheckOnClick = true;
            this.soloFilmToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.soloFilmToolStripMenuItem.Name = "soloFilmToolStripMenuItem";
            this.soloFilmToolStripMenuItem.Size = new System.Drawing.Size(190, 24);
            this.soloFilmToolStripMenuItem.Click += new System.EventHandler(this.Film_Click);
            // 
            // soloSerieTVToolStripMenuItem
            // 
            this.soloSerieTVToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.soloSerieTVToolStripMenuItem.CheckOnClick = true;
            this.soloSerieTVToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.soloSerieTVToolStripMenuItem.Name = "soloSerieTVToolStripMenuItem";
            this.soloSerieTVToolStripMenuItem.Size = new System.Drawing.Size(190, 24);
            this.soloSerieTVToolStripMenuItem.Click += new System.EventHandler(this.Series_Click);
            // 
            // monitorPredefinitoToolStripMenuItem
            // 
            this.monitorPredefinitoToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.monitorPredefinitoToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.monitorPredefinitoToolStripMenuItem.Name = "monitorPredefinitoToolStripMenuItem";
            this.monitorPredefinitoToolStripMenuItem.Size = new System.Drawing.Size(293, 24);
            // 
            // internalPlayer
            // 
            this.internalPlayer.BackColor = System.Drawing.Color.Black;
            this.internalPlayer.CheckOnClick = true;
            this.internalPlayer.ForeColor = System.Drawing.Color.White;
            this.internalPlayer.Name = "internalPlayer";
            this.internalPlayer.Size = new System.Drawing.Size(293, 24);
            this.internalPlayer.Click += new System.EventHandler(this.internalPlayer_Click);
            // 
            // Keygrip1
            // 
            this.Keygrip1.Location = new System.Drawing.Point(425, -1000);
            this.Keygrip1.Name = "Keygrip1";
            this.Keygrip1.Size = new System.Drawing.Size(193, 20);
            this.Keygrip1.TabIndex = 0;
            // 
            // Keygrip2
            // 
            this.Keygrip2.Location = new System.Drawing.Point(425, -1000);
            this.Keygrip2.Name = "Keygrip2";
            this.Keygrip2.Size = new System.Drawing.Size(193, 20);
            this.Keygrip2.TabIndex = 2;
            // 
            // consigliaStrutturaToolStripMenuItem
            // 
            this.consigliaStrutturaToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.consigliaStrutturaToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.consigliaStrutturaToolStripMenuItem.Name = "consigliaStrutturaToolStripMenuItem";
            this.consigliaStrutturaToolStripMenuItem.Size = new System.Drawing.Size(293, 24);
            this.consigliaStrutturaToolStripMenuItem.Click += new System.EventHandler(this.consigliaStrutturaToolStripMenuItem_Click);
            // 
            // languageTagsToolStripMenuItem
            // 
            this.languageTagsToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.languageTagsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.languageTagsToolStripMenuItem.Name = "languageTagsToolStripMenuItem";
            this.languageTagsToolStripMenuItem.Size = new System.Drawing.Size(293, 24);
            this.languageTagsToolStripMenuItem.Click += new System.EventHandler(this.languageTagsToolStripMenuItem_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1284, 761);
            this.Controls.Add(this.Keygrip2);
            this.Controls.Add(this.Keygrip1);
            this.Controls.Add(this.Menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1300, 800);
            this.Name = "Window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Window_Load);
            RefreshMenu();
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public new System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem menu1;
        private System.Windows.Forms.ToolStripMenuItem add_source;
        private System.Windows.Forms.ToolStripMenuItem update_source;
        private System.Windows.Forms.ToolStripMenuItem visualizzaControversieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mostraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tuttoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soloFilmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soloSerieTVToolStripMenuItem;
        public System.Windows.Forms.TextBox Keygrip1;
        public System.Windows.Forms.TextBox Keygrip2;
        private System.Windows.Forms.ToolStripMenuItem monitorPredefinitoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSourceSeries;
        private System.Windows.Forms.ToolStripMenuItem compact;
        private System.Windows.Forms.ToolStripMenuItem internalPlayer;
        private System.Windows.Forms.ToolStripMenuItem consigliaStrutturaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageTagsToolStripMenuItem;
    }
}

