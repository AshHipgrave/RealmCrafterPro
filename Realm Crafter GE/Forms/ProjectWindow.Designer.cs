namespace RealmCrafter_GE.Forms
{
    partial class ProjectWindow
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
            System.Windows.Forms.GroupBox HostGroup;
            System.Windows.Forms.Label MaxAccountsCharsLabel;
            this.UpdateTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MaxCharsPerAccount = new System.Windows.Forms.NumericUpDown();
            this.AllowAccountCreation = new System.Windows.Forms.CheckBox();
            this.ConfigManagerButton = new System.Windows.Forms.Button();
            this.ProjectVerifyButton = new System.Windows.Forms.Button();
            this.BProjectBuildServer = new System.Windows.Forms.Button();
            this.BProjectBuildUpdate = new System.Windows.Forms.Button();
            this.BProjectBuildFull = new System.Windows.Forms.Button();
            this.ProjectInfoGroup = new System.Windows.Forms.GroupBox();
            this.ProjectZones = new System.Windows.Forms.Label();
            this.ProjectItems = new System.Windows.Forms.Label();
            this.ProjectActors = new System.Windows.Forms.Label();
            this.ProjectMusic = new System.Windows.Forms.Label();
            this.ProjectSounds = new System.Windows.Forms.Label();
            this.ProjectTextures = new System.Windows.Forms.Label();
            this.ProjectMeshes = new System.Windows.Forms.Label();
            this.ProjectName = new System.Windows.Forms.Label();
            HostGroup = new System.Windows.Forms.GroupBox();
            MaxAccountsCharsLabel = new System.Windows.Forms.Label();
            HostGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCharsPerAccount)).BeginInit();
            this.ProjectInfoGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // HostGroup
            // 
            HostGroup.Controls.Add(this.UpdateTextBox);
            HostGroup.Controls.Add(this.label2);
            HostGroup.Controls.Add(this.IPTextBox);
            HostGroup.Controls.Add(this.label1);
            HostGroup.Controls.Add(this.MaxCharsPerAccount);
            HostGroup.Controls.Add(MaxAccountsCharsLabel);
            HostGroup.Controls.Add(this.AllowAccountCreation);
            HostGroup.Location = new System.Drawing.Point(21, 215);
            HostGroup.Name = "HostGroup";
            HostGroup.Size = new System.Drawing.Size(269, 175);
            HostGroup.TabIndex = 11;
            HostGroup.TabStop = false;
            HostGroup.Text = "Host settings";
            // 
            // UpdateTextBox
            // 
            this.UpdateTextBox.Location = new System.Drawing.Point(12, 134);
            this.UpdateTextBox.Name = "UpdateTextBox";
            this.UpdateTextBox.Size = new System.Drawing.Size(222, 20);
            this.UpdateTextBox.TabIndex = 16;
            this.UpdateTextBox.Text = "localhost";
            this.UpdateTextBox.TextChanged += new System.EventHandler(this.HostDataChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Update Address:";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(12, 94);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(222, 20);
            this.IPTextBox.TabIndex = 14;
            this.IPTextBox.Text = "localhost";
            this.IPTextBox.TextChanged += new System.EventHandler(this.HostDataChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Server Address:";
            // 
            // MaxCharsPerAccount
            // 
            this.MaxCharsPerAccount.Location = new System.Drawing.Point(179, 42);
            this.MaxCharsPerAccount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.MaxCharsPerAccount.Name = "MaxCharsPerAccount";
            this.MaxCharsPerAccount.Size = new System.Drawing.Size(55, 20);
            this.MaxCharsPerAccount.TabIndex = 12;
            this.MaxCharsPerAccount.ValueChanged += new System.EventHandler(this.MaxCharsPerAccount_ValueChanged);
            // 
            // MaxAccountsCharsLabel
            // 
            MaxAccountsCharsLabel.AutoSize = true;
            MaxAccountsCharsLabel.Location = new System.Drawing.Point(6, 44);
            MaxAccountsCharsLabel.Name = "MaxAccountsCharsLabel";
            MaxAccountsCharsLabel.Size = new System.Drawing.Size(167, 13);
            MaxAccountsCharsLabel.TabIndex = 11;
            MaxAccountsCharsLabel.Text = "Maximum characters per account:";
            // 
            // AllowAccountCreation
            // 
            this.AllowAccountCreation.AutoSize = true;
            this.AllowAccountCreation.Location = new System.Drawing.Point(6, 19);
            this.AllowAccountCreation.Name = "AllowAccountCreation";
            this.AllowAccountCreation.Size = new System.Drawing.Size(214, 17);
            this.AllowAccountCreation.TabIndex = 10;
            this.AllowAccountCreation.Text = "Allow account creation from game client";
            this.AllowAccountCreation.UseVisualStyleBackColor = true;
            this.AllowAccountCreation.CheckedChanged += new System.EventHandler(this.AllowAccountCreation_CheckedChanged);
            // 
            // ConfigManagerButton
            // 
            this.ConfigManagerButton.Location = new System.Drawing.Point(170, 84);
            this.ConfigManagerButton.Name = "ConfigManagerButton";
            this.ConfigManagerButton.Size = new System.Drawing.Size(120, 30);
            this.ConfigManagerButton.TabIndex = 13;
            this.ConfigManagerButton.Text = "Configuration";
            this.ConfigManagerButton.UseVisualStyleBackColor = true;
            this.ConfigManagerButton.Click += new System.EventHandler(this.ConfigManagerButton_Click);
            // 
            // ProjectVerifyButton
            // 
            this.ProjectVerifyButton.Enabled = false;
            this.ProjectVerifyButton.Location = new System.Drawing.Point(170, 179);
            this.ProjectVerifyButton.Name = "ProjectVerifyButton";
            this.ProjectVerifyButton.Size = new System.Drawing.Size(120, 30);
            this.ProjectVerifyButton.TabIndex = 12;
            this.ProjectVerifyButton.Text = "Verify project";
            this.ProjectVerifyButton.UseVisualStyleBackColor = true;
            // 
            // BProjectBuildServer
            // 
            this.BProjectBuildServer.Location = new System.Drawing.Point(170, 120);
            this.BProjectBuildServer.Name = "BProjectBuildServer";
            this.BProjectBuildServer.Size = new System.Drawing.Size(120, 30);
            this.BProjectBuildServer.TabIndex = 10;
            this.BProjectBuildServer.Text = "Build server";
            this.BProjectBuildServer.UseVisualStyleBackColor = true;
            this.BProjectBuildServer.Click += new System.EventHandler(this.BProjectBuildServer_Click);
            // 
            // BProjectBuildUpdate
            // 
            this.BProjectBuildUpdate.Location = new System.Drawing.Point(170, 48);
            this.BProjectBuildUpdate.Name = "BProjectBuildUpdate";
            this.BProjectBuildUpdate.Size = new System.Drawing.Size(120, 30);
            this.BProjectBuildUpdate.TabIndex = 8;
            this.BProjectBuildUpdate.Text = "Generate update";
            this.BProjectBuildUpdate.UseVisualStyleBackColor = true;
            this.BProjectBuildUpdate.Click += new System.EventHandler(this.BProjectBuildUpdate_Click);
            // 
            // BProjectBuildFull
            // 
            this.BProjectBuildFull.Location = new System.Drawing.Point(170, 12);
            this.BProjectBuildFull.Name = "BProjectBuildFull";
            this.BProjectBuildFull.Size = new System.Drawing.Size(120, 30);
            this.BProjectBuildFull.TabIndex = 9;
            this.BProjectBuildFull.Text = "Build full client";
            this.BProjectBuildFull.UseVisualStyleBackColor = true;
            this.BProjectBuildFull.Click += new System.EventHandler(this.BProjectBuildFull_Click);
            // 
            // ProjectInfoGroup
            // 
            this.ProjectInfoGroup.Controls.Add(this.ProjectZones);
            this.ProjectInfoGroup.Controls.Add(this.ProjectItems);
            this.ProjectInfoGroup.Controls.Add(this.ProjectActors);
            this.ProjectInfoGroup.Controls.Add(this.ProjectMusic);
            this.ProjectInfoGroup.Controls.Add(this.ProjectSounds);
            this.ProjectInfoGroup.Controls.Add(this.ProjectTextures);
            this.ProjectInfoGroup.Controls.Add(this.ProjectMeshes);
            this.ProjectInfoGroup.Location = new System.Drawing.Point(21, 12);
            this.ProjectInfoGroup.Name = "ProjectInfoGroup";
            this.ProjectInfoGroup.Size = new System.Drawing.Size(120, 197);
            this.ProjectInfoGroup.TabIndex = 7;
            this.ProjectInfoGroup.TabStop = false;
            this.ProjectInfoGroup.Text = "Project Information";
            // 
            // ProjectZones
            // 
            this.ProjectZones.AutoSize = true;
            this.ProjectZones.Location = new System.Drawing.Point(6, 173);
            this.ProjectZones.Name = "ProjectZones";
            this.ProjectZones.Size = new System.Drawing.Size(49, 13);
            this.ProjectZones.TabIndex = 0;
            this.ProjectZones.Text = "Zones: 0";
            // 
            // ProjectItems
            // 
            this.ProjectItems.AutoSize = true;
            this.ProjectItems.Location = new System.Drawing.Point(6, 149);
            this.ProjectItems.Name = "ProjectItems";
            this.ProjectItems.Size = new System.Drawing.Size(44, 13);
            this.ProjectItems.TabIndex = 0;
            this.ProjectItems.Text = "Items: 0";
            // 
            // ProjectActors
            // 
            this.ProjectActors.AutoSize = true;
            this.ProjectActors.Location = new System.Drawing.Point(6, 125);
            this.ProjectActors.Name = "ProjectActors";
            this.ProjectActors.Size = new System.Drawing.Size(49, 13);
            this.ProjectActors.TabIndex = 0;
            this.ProjectActors.Text = "Actors: 0";
            // 
            // ProjectMusic
            // 
            this.ProjectMusic.AutoSize = true;
            this.ProjectMusic.Location = new System.Drawing.Point(6, 93);
            this.ProjectMusic.Name = "ProjectMusic";
            this.ProjectMusic.Size = new System.Drawing.Size(47, 13);
            this.ProjectMusic.TabIndex = 0;
            this.ProjectMusic.Text = "Music: 0";
            this.ProjectMusic.Click += new System.EventHandler(this.ProjectMusic_Click);
            // 
            // ProjectSounds
            // 
            this.ProjectSounds.AutoSize = true;
            this.ProjectSounds.Location = new System.Drawing.Point(6, 69);
            this.ProjectSounds.Name = "ProjectSounds";
            this.ProjectSounds.Size = new System.Drawing.Size(55, 13);
            this.ProjectSounds.TabIndex = 0;
            this.ProjectSounds.Text = "Sounds: 0";
            // 
            // ProjectTextures
            // 
            this.ProjectTextures.AutoSize = true;
            this.ProjectTextures.Location = new System.Drawing.Point(6, 47);
            this.ProjectTextures.Name = "ProjectTextures";
            this.ProjectTextures.Size = new System.Drawing.Size(60, 13);
            this.ProjectTextures.TabIndex = 0;
            this.ProjectTextures.Text = "Textures: 0";
            // 
            // ProjectMeshes
            // 
            this.ProjectMeshes.AutoSize = true;
            this.ProjectMeshes.Location = new System.Drawing.Point(6, 25);
            this.ProjectMeshes.Name = "ProjectMeshes";
            this.ProjectMeshes.Size = new System.Drawing.Size(56, 13);
            this.ProjectMeshes.TabIndex = 0;
            this.ProjectMeshes.Text = "Meshes: 0";
            // 
            // ProjectName
            // 
            this.ProjectName.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ProjectName.AutoSize = true;
            this.ProjectName.Location = new System.Drawing.Point(-98, 9);
            this.ProjectName.Name = "ProjectName";
            this.ProjectName.Size = new System.Drawing.Size(43, 13);
            this.ProjectName.TabIndex = 6;
            this.ProjectName.Text = "Project:";
            this.ProjectName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ProjectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 424);
            this.Controls.Add(this.ConfigManagerButton);
            this.Controls.Add(this.ProjectVerifyButton);
            this.Controls.Add(HostGroup);
            this.Controls.Add(this.BProjectBuildServer);
            this.Controls.Add(this.BProjectBuildUpdate);
            this.Controls.Add(this.BProjectBuildFull);
            this.Controls.Add(this.ProjectInfoGroup);
            this.Controls.Add(this.ProjectName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Project Settings";
            this.Load += new System.EventHandler(this.ProjectWindowcs_Load);
            HostGroup.ResumeLayout(false);
            HostGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCharsPerAccount)).EndInit();
            this.ProjectInfoGroup.ResumeLayout(false);
            this.ProjectInfoGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConfigManagerButton;
        private System.Windows.Forms.Button ProjectVerifyButton;
        public System.Windows.Forms.NumericUpDown MaxCharsPerAccount;
        public System.Windows.Forms.CheckBox AllowAccountCreation;
        private System.Windows.Forms.Button BProjectBuildServer;
        private System.Windows.Forms.Button BProjectBuildUpdate;
        private System.Windows.Forms.Button BProjectBuildFull;
        private System.Windows.Forms.GroupBox ProjectInfoGroup;
        public System.Windows.Forms.Label ProjectZones;
        public System.Windows.Forms.Label ProjectItems;
        public System.Windows.Forms.Label ProjectActors;
        public System.Windows.Forms.Label ProjectMusic;
        public System.Windows.Forms.Label ProjectSounds;
        public System.Windows.Forms.Label ProjectTextures;
        public System.Windows.Forms.Label ProjectMeshes;
        public System.Windows.Forms.Label ProjectName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox UpdateTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPTextBox;
    }
}