namespace RCTTest
{
    partial class GrassTypesSelector
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.CnlButton = new System.Windows.Forms.Button();
            this.TypesList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(78, 217);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CnlButton
            // 
            this.CnlButton.Location = new System.Drawing.Point(159, 217);
            this.CnlButton.Name = "CnlButton";
            this.CnlButton.Size = new System.Drawing.Size(75, 23);
            this.CnlButton.TabIndex = 7;
            this.CnlButton.Text = "Cancel";
            this.CnlButton.UseVisualStyleBackColor = true;
            this.CnlButton.Click += new System.EventHandler(this.CnlButton_Click);
            // 
            // TypesList
            // 
            this.TypesList.FormattingEnabled = true;
            this.TypesList.Location = new System.Drawing.Point(12, 12);
            this.TypesList.Name = "TypesList";
            this.TypesList.Size = new System.Drawing.Size(222, 199);
            this.TypesList.TabIndex = 6;
            this.TypesList.SelectedIndexChanged += new System.EventHandler(this.TypesList_SelectedIndexChanged);
            // 
            // GrassTypesSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 247);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.CnlButton);
            this.Controls.Add(this.TypesList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GrassTypesSelector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select a Grass Type...";
            this.Shown += new System.EventHandler(this.GrassTypesSelector_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CnlButton;
        private System.Windows.Forms.ListBox TypesList;

    }
}