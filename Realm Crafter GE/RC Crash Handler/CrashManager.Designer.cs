namespace RealmCrafter_GE.RC_Crash_Handler
{
    partial class CrashManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashManager));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.CrashLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ButtonTooltip = new System.Windows.Forms.Label();
            this.SaveLabel = new System.Windows.Forms.Label();
            this.MinBackupSave = new System.Windows.Forms.Button();
            this.ButtonBackupSave = new System.Windows.Forms.Button();
            this.ButtonQuit = new System.Windows.Forms.Button();
            this.CrashText = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox2);
            this.splitContainer1.Panel1.Controls.Add(this.CrashLabel);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(563, 564);
            this.splitContainer1.SplitterDistance = 157;
            this.splitContainer1.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBox2.Location = new System.Drawing.Point(0, 152);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(563, 5);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // CrashLabel
            // 
            this.CrashLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashLabel.AutoSize = true;
            this.CrashLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.CrashLabel.ForeColor = System.Drawing.Color.White;
            this.CrashLabel.Location = new System.Drawing.Point(3, 123);
            this.CrashLabel.Name = "CrashLabel";
            this.CrashLabel.Size = new System.Drawing.Size(417, 26);
            this.CrashLabel.TabIndex = 1;
            this.CrashLabel.Text = "Unfortunately, Realm Crafter has come across an unexpected error and needs to clo" +
                "se.\r\nBelow contains information pertaining to the crash";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.pictureBox1.BackgroundImage = global::RealmCrafter_GE.Properties.Resources.company_logo;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(563, 157);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ButtonTooltip);
            this.splitContainer2.Panel1.Controls.Add(this.SaveLabel);
            this.splitContainer2.Panel1.Controls.Add(this.MinBackupSave);
            this.splitContainer2.Panel1.Controls.Add(this.ButtonBackupSave);
            this.splitContainer2.Panel1.Controls.Add(this.ButtonQuit);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.CrashText);
            this.splitContainer2.Size = new System.Drawing.Size(563, 403);
            this.splitContainer2.SplitterDistance = 60;
            this.splitContainer2.TabIndex = 0;
            // 
            // ButtonTooltip
            // 
            this.ButtonTooltip.AutoSize = true;
            this.ButtonTooltip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonTooltip.Location = new System.Drawing.Point(252, 43);
            this.ButtonTooltip.Name = "ButtonTooltip";
            this.ButtonTooltip.Size = new System.Drawing.Size(0, 13);
            this.ButtonTooltip.TabIndex = 4;
            // 
            // SaveLabel
            // 
            this.SaveLabel.AutoSize = true;
            this.SaveLabel.Location = new System.Drawing.Point(3, 0);
            this.SaveLabel.Name = "SaveLabel";
            this.SaveLabel.Size = new System.Drawing.Size(467, 26);
            this.SaveLabel.TabIndex = 3;
            this.SaveLabel.Text = "Realm Crafter can still attempt to save your unsaved data! Use one of the followi" +
                "ng options below.\r\nIt is recommended you create your own periodic backups.";
            // 
            // MinBackupSave
            // 
            this.MinBackupSave.Location = new System.Drawing.Point(6, 33);
            this.MinBackupSave.Name = "MinBackupSave";
            this.MinBackupSave.Size = new System.Drawing.Size(125, 23);
            this.MinBackupSave.TabIndex = 2;
            this.MinBackupSave.Text = "Minimal backup save";
            this.MinBackupSave.UseVisualStyleBackColor = true;
            this.MinBackupSave.MouseLeave += new System.EventHandler(this.MinBackupSave_MouseLeave);
            this.MinBackupSave.Click += new System.EventHandler(this.MinBackupSave_Click);
            this.MinBackupSave.MouseEnter += new System.EventHandler(this.MinBackupSave_MouseEnter);
            // 
            // ButtonBackupSave
            // 
            this.ButtonBackupSave.Location = new System.Drawing.Point(134, 33);
            this.ButtonBackupSave.Name = "ButtonBackupSave";
            this.ButtonBackupSave.Size = new System.Drawing.Size(112, 23);
            this.ButtonBackupSave.TabIndex = 1;
            this.ButtonBackupSave.Text = "Full backup save";
            this.ButtonBackupSave.UseVisualStyleBackColor = true;
            this.ButtonBackupSave.MouseLeave += new System.EventHandler(this.ButtonBackupSave_MouseLeave);
            this.ButtonBackupSave.Click += new System.EventHandler(this.ButtonBackupSave_Click);
            this.ButtonBackupSave.MouseEnter += new System.EventHandler(this.ButtonBackupSave_MouseEnter);
            // 
            // ButtonQuit
            // 
            this.ButtonQuit.Location = new System.Drawing.Point(458, 33);
            this.ButtonQuit.Name = "ButtonQuit";
            this.ButtonQuit.Size = new System.Drawing.Size(93, 23);
            this.ButtonQuit.TabIndex = 0;
            this.ButtonQuit.Text = "Close Program";
            this.ButtonQuit.UseVisualStyleBackColor = true;
            this.ButtonQuit.Click += new System.EventHandler(this.ButtonQuit_Click);
            // 
            // CrashText
            // 
            this.CrashText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrashText.Location = new System.Drawing.Point(0, 0);
            this.CrashText.MaxLength = 327670;
            this.CrashText.Multiline = true;
            this.CrashText.Name = "CrashText";
            this.CrashText.ReadOnly = true;
            this.CrashText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.CrashText.Size = new System.Drawing.Size(563, 339);
            this.CrashText.TabIndex = 1;
            this.CrashText.WordWrap = false;
            // 
            // CrashManager
            // 
            this.AcceptButton = this.ButtonQuit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 564);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Realm Crafter Exception";
            this.Load += new System.EventHandler(this.CrashManager_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label CrashLabel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button ButtonQuit;
        private System.Windows.Forms.Button ButtonBackupSave;
        private System.Windows.Forms.Button MinBackupSave;
        private System.Windows.Forms.Label SaveLabel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label ButtonTooltip;
        private System.Windows.Forms.TextBox CrashText;
    }
}