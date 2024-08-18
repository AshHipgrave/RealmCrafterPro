namespace RealmCrafter_GE.Forms
{
    partial class LODEditor
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
            this.HighMeshButton = new System.Windows.Forms.Button();
            this.MediumMeshButton = new System.Windows.Forms.Button();
            this.LowMeshButton = new System.Windows.Forms.Button();
            this.GOButton = new System.Windows.Forms.Button();
            this.CnlButton = new System.Windows.Forms.Button();
            this.DistToMedSpinner = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DistToLowSpinner = new System.Windows.Forms.NumericUpDown();
            this.DistToHideSpinner = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DistToMedSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DistToLowSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DistToHideSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // HighMeshButton
            // 
            this.HighMeshButton.Location = new System.Drawing.Point(199, 110);
            this.HighMeshButton.Name = "HighMeshButton";
            this.HighMeshButton.Size = new System.Drawing.Size(162, 23);
            this.HighMeshButton.TabIndex = 0;
            this.HighMeshButton.Text = "button1";
            this.HighMeshButton.UseVisualStyleBackColor = true;
            this.HighMeshButton.Click += new System.EventHandler(this.HighMeshButton_Click);
            // 
            // MediumMeshButton
            // 
            this.MediumMeshButton.Location = new System.Drawing.Point(199, 139);
            this.MediumMeshButton.Name = "MediumMeshButton";
            this.MediumMeshButton.Size = new System.Drawing.Size(162, 23);
            this.MediumMeshButton.TabIndex = 1;
            this.MediumMeshButton.Text = "button2";
            this.MediumMeshButton.UseVisualStyleBackColor = true;
            this.MediumMeshButton.Click += new System.EventHandler(this.MediumMeshButton_Click);
            // 
            // LowMeshButton
            // 
            this.LowMeshButton.Location = new System.Drawing.Point(199, 168);
            this.LowMeshButton.Name = "LowMeshButton";
            this.LowMeshButton.Size = new System.Drawing.Size(162, 23);
            this.LowMeshButton.TabIndex = 2;
            this.LowMeshButton.Text = "button3";
            this.LowMeshButton.UseVisualStyleBackColor = true;
            this.LowMeshButton.Click += new System.EventHandler(this.LowMeshButton_Click);
            // 
            // GOButton
            // 
            this.GOButton.Location = new System.Drawing.Point(204, 299);
            this.GOButton.Name = "GOButton";
            this.GOButton.Size = new System.Drawing.Size(75, 23);
            this.GOButton.TabIndex = 3;
            this.GOButton.Text = "Go";
            this.GOButton.UseVisualStyleBackColor = true;
            this.GOButton.Click += new System.EventHandler(this.GOButton_Click);
            // 
            // CnlButton
            // 
            this.CnlButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CnlButton.Location = new System.Drawing.Point(285, 299);
            this.CnlButton.Name = "CnlButton";
            this.CnlButton.Size = new System.Drawing.Size(75, 23);
            this.CnlButton.TabIndex = 4;
            this.CnlButton.Text = "Close";
            this.CnlButton.UseVisualStyleBackColor = true;
            // 
            // DistToMedSpinner
            // 
            this.DistToMedSpinner.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.DistToMedSpinner.Location = new System.Drawing.Point(199, 197);
            this.DistToMedSpinner.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.DistToMedSpinner.Name = "DistToMedSpinner";
            this.DistToMedSpinner.Size = new System.Drawing.Size(162, 20);
            this.DistToMedSpinner.TabIndex = 5;
            this.DistToMedSpinner.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(349, 65);
            this.label1.TabIndex = 8;
            this.label1.Text = "This facility is provided to re-map LOD properties of meshes placed in the\r\ncurre" +
                "nt zone.\r\n\r\nNote: This tool will not update a full MegaTerrains zone, it will on" +
                "ly modify\r\nthe locked area.\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Mesh Instance to Modify:";
            // 
            // DistToLowSpinner
            // 
            this.DistToLowSpinner.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.DistToLowSpinner.Location = new System.Drawing.Point(199, 223);
            this.DistToLowSpinner.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.DistToLowSpinner.Name = "DistToLowSpinner";
            this.DistToLowSpinner.Size = new System.Drawing.Size(162, 20);
            this.DistToLowSpinner.TabIndex = 10;
            this.DistToLowSpinner.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // DistToHideSpinner
            // 
            this.DistToHideSpinner.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.DistToHideSpinner.Location = new System.Drawing.Point(198, 249);
            this.DistToHideSpinner.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.DistToHideSpinner.Name = "DistToHideSpinner";
            this.DistToHideSpinner.Size = new System.Drawing.Size(162, 20);
            this.DistToHideSpinner.TabIndex = 11;
            this.DistToHideSpinner.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "New Medium LOD Mesh:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "New Low LOD Mesh:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Distance To Medium:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 225);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Distance To Low:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 251);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Distance To Hide:";
            // 
            // LODEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CnlButton;
            this.ClientSize = new System.Drawing.Size(370, 334);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DistToHideSpinner);
            this.Controls.Add(this.DistToLowSpinner);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DistToMedSpinner);
            this.Controls.Add(this.CnlButton);
            this.Controls.Add(this.GOButton);
            this.Controls.Add(this.LowMeshButton);
            this.Controls.Add(this.MediumMeshButton);
            this.Controls.Add(this.HighMeshButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LODEditor";
            this.ShowInTaskbar = false;
            this.Text = "LOD Updater";
            this.Load += new System.EventHandler(this.LODEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DistToMedSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DistToLowSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DistToHideSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button HighMeshButton;
        private System.Windows.Forms.Button MediumMeshButton;
        private System.Windows.Forms.Button LowMeshButton;
        private System.Windows.Forms.Button GOButton;
        private System.Windows.Forms.Button CnlButton;
        private System.Windows.Forms.NumericUpDown DistToMedSpinner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown DistToLowSpinner;
        private System.Windows.Forms.NumericUpDown DistToHideSpinner;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}