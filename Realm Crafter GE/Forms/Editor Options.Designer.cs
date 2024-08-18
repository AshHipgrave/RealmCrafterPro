namespace RealmCrafter_GE
{
    partial class Editor_Options
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
            System.Windows.Forms.Label GOOptionsCameraSpeedLabel;
            System.Windows.Forms.Label GOOptionsCameraSpeedPercentLabel;
            this.GEOptionsAccept = new System.Windows.Forms.Button();
            this.GEOptionsSaveIndicate = new System.Windows.Forms.CheckBox();
            this.GEOptionsResizing = new System.Windows.Forms.CheckBox();
            this.GEOptionsRememberLayout = new System.Windows.Forms.CheckBox();
            this.GEOptionsCameraSpeed = new System.Windows.Forms.NumericUpDown();
            this.GEOptionsShowGrid = new System.Windows.Forms.CheckBox();
            this.ShaderDetailCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CameraKeysButton = new System.Windows.Forms.Button();
            GOOptionsCameraSpeedLabel = new System.Windows.Forms.Label();
            GOOptionsCameraSpeedPercentLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.GEOptionsCameraSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // GOOptionsCameraSpeedLabel
            // 
            GOOptionsCameraSpeedLabel.AutoSize = true;
            GOOptionsCameraSpeedLabel.Location = new System.Drawing.Point(10, 111);
            GOOptionsCameraSpeedLabel.Name = "GOOptionsCameraSpeedLabel";
            GOOptionsCameraSpeedLabel.Size = new System.Drawing.Size(130, 13);
            GOOptionsCameraSpeedLabel.TabIndex = 4;
            GOOptionsCameraSpeedLabel.Text = "Camera movement speed:";
            // 
            // GOOptionsCameraSpeedPercentLabel
            // 
            GOOptionsCameraSpeedPercentLabel.AutoSize = true;
            GOOptionsCameraSpeedPercentLabel.Location = new System.Drawing.Point(104, 129);
            GOOptionsCameraSpeedPercentLabel.Name = "GOOptionsCameraSpeedPercentLabel";
            GOOptionsCameraSpeedPercentLabel.Size = new System.Drawing.Size(15, 13);
            GOOptionsCameraSpeedPercentLabel.TabIndex = 6;
            GOOptionsCameraSpeedPercentLabel.Text = "%";
            // 
            // GEOptionsAccept
            // 
            this.GEOptionsAccept.Location = new System.Drawing.Point(44, 233);
            this.GEOptionsAccept.Name = "GEOptionsAccept";
            this.GEOptionsAccept.Size = new System.Drawing.Size(75, 23);
            this.GEOptionsAccept.TabIndex = 0;
            this.GEOptionsAccept.Text = "Accept";
            this.GEOptionsAccept.UseVisualStyleBackColor = true;
            this.GEOptionsAccept.Click += new System.EventHandler(this.GEOptionsAccept_Click);
            // 
            // GEOptionsSaveIndicate
            // 
            this.GEOptionsSaveIndicate.AutoSize = true;
            this.GEOptionsSaveIndicate.Location = new System.Drawing.Point(12, 12);
            this.GEOptionsSaveIndicate.Name = "GEOptionsSaveIndicate";
            this.GEOptionsSaveIndicate.Size = new System.Drawing.Size(131, 17);
            this.GEOptionsSaveIndicate.TabIndex = 1;
            this.GEOptionsSaveIndicate.Text = "Indicate unsaved tabs";
            this.GEOptionsSaveIndicate.UseVisualStyleBackColor = true;
            // 
            // GEOptionsResizing
            // 
            this.GEOptionsResizing.AutoSize = true;
            this.GEOptionsResizing.Location = new System.Drawing.Point(12, 35);
            this.GEOptionsResizing.Name = "GEOptionsResizing";
            this.GEOptionsResizing.Size = new System.Drawing.Size(128, 17);
            this.GEOptionsResizing.TabIndex = 2;
            this.GEOptionsResizing.Text = "Allow window resizing";
            this.GEOptionsResizing.UseVisualStyleBackColor = true;
            // 
            // GEOptionsRememberLayout
            // 
            this.GEOptionsRememberLayout.AutoSize = true;
            this.GEOptionsRememberLayout.Location = new System.Drawing.Point(12, 58);
            this.GEOptionsRememberLayout.Name = "GEOptionsRememberLayout";
            this.GEOptionsRememberLayout.Size = new System.Drawing.Size(147, 17);
            this.GEOptionsRememberLayout.TabIndex = 3;
            this.GEOptionsRememberLayout.Text = "Remember window layout";
            this.GEOptionsRememberLayout.UseVisualStyleBackColor = true;
            // 
            // GEOptionsCameraSpeed
            // 
            this.GEOptionsCameraSpeed.Location = new System.Drawing.Point(24, 127);
            this.GEOptionsCameraSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GEOptionsCameraSpeed.Name = "GEOptionsCameraSpeed";
            this.GEOptionsCameraSpeed.Size = new System.Drawing.Size(74, 20);
            this.GEOptionsCameraSpeed.TabIndex = 5;
            this.GEOptionsCameraSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // GEOptionsShowGrid
            // 
            this.GEOptionsShowGrid.AutoSize = true;
            this.GEOptionsShowGrid.Location = new System.Drawing.Point(12, 81);
            this.GEOptionsShowGrid.Name = "GEOptionsShowGrid";
            this.GEOptionsShowGrid.Size = new System.Drawing.Size(73, 17);
            this.GEOptionsShowGrid.TabIndex = 7;
            this.GEOptionsShowGrid.Text = "Show grid";
            this.GEOptionsShowGrid.UseVisualStyleBackColor = true;
            // 
            // ShaderDetailCombo
            // 
            this.ShaderDetailCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ShaderDetailCombo.FormattingEnabled = true;
            this.ShaderDetailCombo.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.ShaderDetailCombo.Location = new System.Drawing.Point(24, 166);
            this.ShaderDetailCombo.Name = "ShaderDetailCombo";
            this.ShaderDetailCombo.Size = new System.Drawing.Size(121, 21);
            this.ShaderDetailCombo.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Shader Detail:";
            // 
            // CameraKeysButton
            // 
            this.CameraKeysButton.Location = new System.Drawing.Point(13, 204);
            this.CameraKeysButton.Name = "CameraKeysButton";
            this.CameraKeysButton.Size = new System.Drawing.Size(146, 23);
            this.CameraKeysButton.TabIndex = 10;
            this.CameraKeysButton.Text = "Controls Settings";
            this.CameraKeysButton.UseVisualStyleBackColor = true;
            this.CameraKeysButton.Click += new System.EventHandler(this.CameraKeysButton_Click);
            // 
            // Editor_Options
            // 
            this.AcceptButton = this.GEOptionsAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(187, 268);
            this.Controls.Add(this.CameraKeysButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ShaderDetailCombo);
            this.Controls.Add(this.GEOptionsShowGrid);
            this.Controls.Add(GOOptionsCameraSpeedPercentLabel);
            this.Controls.Add(this.GEOptionsCameraSpeed);
            this.Controls.Add(GOOptionsCameraSpeedLabel);
            this.Controls.Add(this.GEOptionsRememberLayout);
            this.Controls.Add(this.GEOptionsResizing);
            this.Controls.Add(this.GEOptionsSaveIndicate);
            this.Controls.Add(this.GEOptionsAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Editor_Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Realm Crafter GE Options";
            ((System.ComponentModel.ISupportInitialize)(this.GEOptionsCameraSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GEOptionsAccept;
        public System.Windows.Forms.CheckBox GEOptionsSaveIndicate;
        public System.Windows.Forms.CheckBox GEOptionsResizing;
        public System.Windows.Forms.CheckBox GEOptionsRememberLayout;
        public System.Windows.Forms.NumericUpDown GEOptionsCameraSpeed;
        public System.Windows.Forms.CheckBox GEOptionsShowGrid;
        public System.Windows.Forms.ComboBox ShaderDetailCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CameraKeysButton;
    }
}