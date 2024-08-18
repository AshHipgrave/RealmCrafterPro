namespace RealmCrafter_GE.Forms
{
    partial class CreateTerrainDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.TerrainSizeCombo = new System.Windows.Forms.ComboBox();
            this.CreateButton = new System.Windows.Forms.Button();
            this.CnlButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Terrain Size:";
            // 
            // TerrainSizeCombo
            // 
            this.TerrainSizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TerrainSizeCombo.FormattingEnabled = true;
            this.TerrainSizeCombo.Items.AddRange(new object[] {
            "64x64 (30m x 30m)",
            "128x128 (60m x 60m)",
            "256x256 (125m x 125m)",
            "512x512 (250m x 250m)",
            "1024x1024 (500m x 500m)",
            "2048x2048 (1km x 1km)"});
            this.TerrainSizeCombo.Location = new System.Drawing.Point(84, 12);
            this.TerrainSizeCombo.Name = "TerrainSizeCombo";
            this.TerrainSizeCombo.Size = new System.Drawing.Size(162, 21);
            this.TerrainSizeCombo.TabIndex = 1;
            // 
            // CreateButton
            // 
            this.CreateButton.Location = new System.Drawing.Point(90, 39);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(75, 23);
            this.CreateButton.TabIndex = 2;
            this.CreateButton.Text = "Create";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // CnlButton
            // 
            this.CnlButton.Location = new System.Drawing.Point(171, 39);
            this.CnlButton.Name = "CnlButton";
            this.CnlButton.Size = new System.Drawing.Size(75, 23);
            this.CnlButton.TabIndex = 3;
            this.CnlButton.Text = "Cancel";
            this.CnlButton.UseVisualStyleBackColor = true;
            this.CnlButton.Click += new System.EventHandler(this.CnlButton_Click);
            // 
            // CreateTerrainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 69);
            this.Controls.Add(this.CnlButton);
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.TerrainSizeCombo);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTerrainDialog";
            this.ShowIcon = false;
            this.Text = "Create Terrain...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox TerrainSizeCombo;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Button CnlButton;
    }
}