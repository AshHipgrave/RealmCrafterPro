namespace RealmCrafter_GE
{
    partial class ShaderManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShaderManager));
            this.ShaderListBox = new System.Windows.Forms.ListBox();
            this.NewShaderButton = new System.Windows.Forms.Button();
            this.EditShaderButton = new System.Windows.Forms.Button();
            this.DeleteShaderButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DeleteProfileButton = new System.Windows.Forms.Button();
            this.EditProfileButton = new System.Windows.Forms.Button();
            this.NewProfileButton = new System.Windows.Forms.Button();
            this.ProfileListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ShaderListBox
            // 
            this.ShaderListBox.FormattingEnabled = true;
            this.ShaderListBox.Location = new System.Drawing.Point(12, 25);
            this.ShaderListBox.Name = "ShaderListBox";
            this.ShaderListBox.Size = new System.Drawing.Size(237, 186);
            this.ShaderListBox.TabIndex = 0;
            // 
            // NewShaderButton
            // 
            this.NewShaderButton.Location = new System.Drawing.Point(12, 217);
            this.NewShaderButton.Name = "NewShaderButton";
            this.NewShaderButton.Size = new System.Drawing.Size(75, 23);
            this.NewShaderButton.TabIndex = 1;
            this.NewShaderButton.Text = "New";
            this.NewShaderButton.UseVisualStyleBackColor = true;
            this.NewShaderButton.Click += new System.EventHandler(this.NewShaderButton_Click);
            // 
            // EditShaderButton
            // 
            this.EditShaderButton.Location = new System.Drawing.Point(93, 217);
            this.EditShaderButton.Name = "EditShaderButton";
            this.EditShaderButton.Size = new System.Drawing.Size(75, 23);
            this.EditShaderButton.TabIndex = 2;
            this.EditShaderButton.Text = "Edit";
            this.EditShaderButton.UseVisualStyleBackColor = true;
            this.EditShaderButton.Click += new System.EventHandler(this.EditShaderButton_Click);
            // 
            // DeleteShaderButton
            // 
            this.DeleteShaderButton.Location = new System.Drawing.Point(174, 217);
            this.DeleteShaderButton.Name = "DeleteShaderButton";
            this.DeleteShaderButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteShaderButton.TabIndex = 3;
            this.DeleteShaderButton.Text = "Delete";
            this.DeleteShaderButton.UseVisualStyleBackColor = true;
            this.DeleteShaderButton.Click += new System.EventHandler(this.DeleteShaderButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Shaders:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Profiles (Shader Groups):";
            // 
            // DeleteProfileButton
            // 
            this.DeleteProfileButton.Location = new System.Drawing.Point(171, 451);
            this.DeleteProfileButton.Name = "DeleteProfileButton";
            this.DeleteProfileButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteProfileButton.TabIndex = 8;
            this.DeleteProfileButton.Text = "Delete";
            this.DeleteProfileButton.UseVisualStyleBackColor = true;
            this.DeleteProfileButton.Click += new System.EventHandler(this.DeleteProfileButton_Click);
            // 
            // EditProfileButton
            // 
            this.EditProfileButton.Location = new System.Drawing.Point(90, 451);
            this.EditProfileButton.Name = "EditProfileButton";
            this.EditProfileButton.Size = new System.Drawing.Size(75, 23);
            this.EditProfileButton.TabIndex = 7;
            this.EditProfileButton.Text = "Edit";
            this.EditProfileButton.UseVisualStyleBackColor = true;
            this.EditProfileButton.Click += new System.EventHandler(this.EditProfileButton_Click);
            // 
            // NewProfileButton
            // 
            this.NewProfileButton.Location = new System.Drawing.Point(9, 451);
            this.NewProfileButton.Name = "NewProfileButton";
            this.NewProfileButton.Size = new System.Drawing.Size(75, 23);
            this.NewProfileButton.TabIndex = 6;
            this.NewProfileButton.Text = "New";
            this.NewProfileButton.UseVisualStyleBackColor = true;
            this.NewProfileButton.Click += new System.EventHandler(this.NewProfileButton_Click);
            // 
            // ProfileListBox
            // 
            this.ProfileListBox.FormattingEnabled = true;
            this.ProfileListBox.Location = new System.Drawing.Point(9, 259);
            this.ProfileListBox.Name = "ProfileListBox";
            this.ProfileListBox.Size = new System.Drawing.Size(237, 186);
            this.ProfileListBox.TabIndex = 5;
            // 
            // ShaderManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 483);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DeleteProfileButton);
            this.Controls.Add(this.EditProfileButton);
            this.Controls.Add(this.NewProfileButton);
            this.Controls.Add(this.ProfileListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DeleteShaderButton);
            this.Controls.Add(this.EditShaderButton);
            this.Controls.Add(this.NewShaderButton);
            this.Controls.Add(this.ShaderListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShaderManager";
            this.Text = "Shader Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ShaderListBox;
        private System.Windows.Forms.Button NewShaderButton;
        private System.Windows.Forms.Button EditShaderButton;
        private System.Windows.Forms.Button DeleteShaderButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DeleteProfileButton;
        private System.Windows.Forms.Button EditProfileButton;
        private System.Windows.Forms.Button NewProfileButton;
        private System.Windows.Forms.ListBox ProfileListBox;
    }
}