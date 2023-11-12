namespace CyanVideos.SeasonEditor
{
    partial class SeasonEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeasonEditor));
            this.shapeButton = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.Label();
            this.reload_icon = new System.Windows.Forms.Label();
            this.selectAll = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // shapeButton
            // 
            this.shapeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.shapeButton.BackColor = System.Drawing.Color.ForestGreen;
            this.shapeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shapeButton.Location = new System.Drawing.Point(1231, 629);
            this.shapeButton.Name = "shapeButton";
            this.shapeButton.Size = new System.Drawing.Size(328, 110);
            this.shapeButton.TabIndex = 0;
            this.shapeButton.Text = "None";
            this.shapeButton.UseVisualStyleBackColor = false;
            this.shapeButton.Click += new System.EventHandler(this.shapeButton_Click);
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.log.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.log.ForeColor = System.Drawing.Color.White;
            this.log.Location = new System.Drawing.Point(782, 647);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(443, 75);
            this.log.TabIndex = 1;
            this.log.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // reload_icon
            // 
            this.reload_icon.Location = new System.Drawing.Point(725, 647);
            this.reload_icon.Name = "reload_icon";
            this.reload_icon.Size = new System.Drawing.Size(78, 78);
            this.reload_icon.TabIndex = 3;
            this.reload_icon.Text = "label1";
            this.reload_icon.Click += new System.EventHandler(this.reload_icon_Click);
            // 
            // selectAll
            // 
            this.selectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.selectAll.FlatAppearance.BorderSize = 0;
            this.selectAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.selectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAll.Location = new System.Drawing.Point(27, 678);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(192, 34);
            this.selectAll.TabIndex = 4;
            this.selectAll.Text = "Select All";
            this.selectAll.UseVisualStyleBackColor = false;
            this.selectAll.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Maroon;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(225, 678);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(192, 34);
            this.button1.TabIndex = 5;
            this.button1.Text = "Deselect All";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // SeasonEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1584, 761);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.selectAll);
            this.Controls.Add(this.reload_icon);
            this.Controls.Add(this.log);
            this.Controls.Add(this.shapeButton);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1560, 800);
            this.Name = "SeasonEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Season Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button shapeButton;
        private System.Windows.Forms.Label log;
        private System.Windows.Forms.Label reload_icon;
        private System.Windows.Forms.Button selectAll;
        private System.Windows.Forms.Button button1;
    }
}