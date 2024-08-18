namespace RealmCrafter_GE
{
    partial class TextureFlags
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureFlags));
            this.TextureSettingsAccept = new System.Windows.Forms.Button();
            this.TextureSettingsColour = new System.Windows.Forms.CheckBox();
            this.TextureSettingsClampV = new System.Windows.Forms.CheckBox();
            this.TextureSettingsClampU = new System.Windows.Forms.CheckBox();
            this.TextureSettingsMipmapped = new System.Windows.Forms.CheckBox();
            this.TextureSettingsMasked = new System.Windows.Forms.CheckBox();
            this.TextureSettingsAlpha = new System.Windows.Forms.CheckBox();
            this.TextureSettingsCube = new System.Windows.Forms.CheckBox();
            this.TextureSettingsSphere = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // TextureSettingsAccept
            // 
            this.TextureSettingsAccept.Location = new System.Drawing.Point(20, 198);
            this.TextureSettingsAccept.Name = "TextureSettingsAccept";
            this.TextureSettingsAccept.Size = new System.Drawing.Size(75, 23);
            this.TextureSettingsAccept.TabIndex = 0;
            this.TextureSettingsAccept.Text = "Accept";
            this.TextureSettingsAccept.UseVisualStyleBackColor = true;
            this.TextureSettingsAccept.Click += new System.EventHandler(this.TextureSettingsAccept_Click);
            // 
            // TextureSettingsColour
            // 
            this.TextureSettingsColour.AutoSize = true;
            this.TextureSettingsColour.Checked = true;
            this.TextureSettingsColour.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TextureSettingsColour.Location = new System.Drawing.Point(12, 12);
            this.TextureSettingsColour.Name = "TextureSettingsColour";
            this.TextureSettingsColour.Size = new System.Drawing.Size(56, 17);
            this.TextureSettingsColour.TabIndex = 1;
            this.TextureSettingsColour.Text = "Colour";
            this.TextureSettingsColour.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsClampV
            // 
            this.TextureSettingsClampV.AutoSize = true;
            this.TextureSettingsClampV.Location = new System.Drawing.Point(12, 127);
            this.TextureSettingsClampV.Name = "TextureSettingsClampV";
            this.TextureSettingsClampV.Size = new System.Drawing.Size(65, 17);
            this.TextureSettingsClampV.TabIndex = 2;
            this.TextureSettingsClampV.Text = "Clamp V";
            this.TextureSettingsClampV.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsClampU
            // 
            this.TextureSettingsClampU.AutoSize = true;
            this.TextureSettingsClampU.Location = new System.Drawing.Point(12, 104);
            this.TextureSettingsClampU.Name = "TextureSettingsClampU";
            this.TextureSettingsClampU.Size = new System.Drawing.Size(66, 17);
            this.TextureSettingsClampU.TabIndex = 3;
            this.TextureSettingsClampU.Text = "Clamp U";
            this.TextureSettingsClampU.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsMipmapped
            // 
            this.TextureSettingsMipmapped.AutoSize = true;
            this.TextureSettingsMipmapped.Checked = true;
            this.TextureSettingsMipmapped.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TextureSettingsMipmapped.Location = new System.Drawing.Point(12, 81);
            this.TextureSettingsMipmapped.Name = "TextureSettingsMipmapped";
            this.TextureSettingsMipmapped.Size = new System.Drawing.Size(81, 17);
            this.TextureSettingsMipmapped.TabIndex = 4;
            this.TextureSettingsMipmapped.Text = "Mipmapped";
            this.TextureSettingsMipmapped.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsMasked
            // 
            this.TextureSettingsMasked.AutoSize = true;
            this.TextureSettingsMasked.Location = new System.Drawing.Point(12, 58);
            this.TextureSettingsMasked.Name = "TextureSettingsMasked";
            this.TextureSettingsMasked.Size = new System.Drawing.Size(64, 17);
            this.TextureSettingsMasked.TabIndex = 5;
            this.TextureSettingsMasked.Text = "Masked";
            this.TextureSettingsMasked.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsAlpha
            // 
            this.TextureSettingsAlpha.AutoSize = true;
            this.TextureSettingsAlpha.Location = new System.Drawing.Point(12, 35);
            this.TextureSettingsAlpha.Name = "TextureSettingsAlpha";
            this.TextureSettingsAlpha.Size = new System.Drawing.Size(53, 17);
            this.TextureSettingsAlpha.TabIndex = 6;
            this.TextureSettingsAlpha.Text = "Alpha";
            this.TextureSettingsAlpha.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsCube
            // 
            this.TextureSettingsCube.AutoSize = true;
            this.TextureSettingsCube.Location = new System.Drawing.Point(12, 173);
            this.TextureSettingsCube.Name = "TextureSettingsCube";
            this.TextureSettingsCube.Size = new System.Drawing.Size(74, 17);
            this.TextureSettingsCube.TabIndex = 7;
            this.TextureSettingsCube.Text = "Cube map";
            this.TextureSettingsCube.UseVisualStyleBackColor = true;
            // 
            // TextureSettingsSphere
            // 
            this.TextureSettingsSphere.AutoSize = true;
            this.TextureSettingsSphere.Location = new System.Drawing.Point(12, 150);
            this.TextureSettingsSphere.Name = "TextureSettingsSphere";
            this.TextureSettingsSphere.Size = new System.Drawing.Size(83, 17);
            this.TextureSettingsSphere.TabIndex = 8;
            this.TextureSettingsSphere.Text = "Sphere map";
            this.TextureSettingsSphere.UseVisualStyleBackColor = true;
            // 
            // TextureFlags
            // 
            this.AcceptButton = this.TextureSettingsAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(116, 233);
            this.Controls.Add(this.TextureSettingsSphere);
            this.Controls.Add(this.TextureSettingsCube);
            this.Controls.Add(this.TextureSettingsAlpha);
            this.Controls.Add(this.TextureSettingsMasked);
            this.Controls.Add(this.TextureSettingsMipmapped);
            this.Controls.Add(this.TextureSettingsClampU);
            this.Controls.Add(this.TextureSettingsClampV);
            this.Controls.Add(this.TextureSettingsColour);
            this.Controls.Add(this.TextureSettingsAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureFlags";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Texture Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TextureSettingsAccept;
        public System.Windows.Forms.CheckBox TextureSettingsColour;
        public System.Windows.Forms.CheckBox TextureSettingsClampV;
        public System.Windows.Forms.CheckBox TextureSettingsClampU;
        public System.Windows.Forms.CheckBox TextureSettingsMipmapped;
        public System.Windows.Forms.CheckBox TextureSettingsMasked;
        public System.Windows.Forms.CheckBox TextureSettingsAlpha;
        public System.Windows.Forms.CheckBox TextureSettingsCube;
        public System.Windows.Forms.CheckBox TextureSettingsSphere;
    }
}