namespace RAW
{
    partial class RAWImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RAWImportDialog));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.HeaderSizeTextbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.TerrainWidthTextbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.YScaleTextbox = new System.Windows.Forms.TextBox();
            this.AutoDetectCheckbox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.FormatCombobox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Tex4PathButton = new System.Windows.Forms.Button();
            this.Tex3PathButton = new System.Windows.Forms.Button();
            this.Tex2PathButton = new System.Windows.Forms.Button();
            this.Tex1PathButton = new System.Windows.Forms.Button();
            this.Tex0PathButton = new System.Windows.Forms.Button();
            this.HMPathButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.Tex4PathTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Tex3PathTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Tex2PathTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Tex1PathTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Tex0PathTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.HMPathTextbox = new System.Windows.Forms.TextBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.OpenRAWDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenTextureDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.HeaderSizeTextbox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.TerrainWidthTextbox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.YScaleTextbox);
            this.groupBox2.Controls.Add(this.AutoDetectCheckbox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.FormatCombobox);
            this.groupBox2.Location = new System.Drawing.Point(326, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 173);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Header Size:";
            // 
            // HeaderSizeTextbox
            // 
            this.HeaderSizeTextbox.Location = new System.Drawing.Point(82, 72);
            this.HeaderSizeTextbox.Name = "HeaderSizeTextbox";
            this.HeaderSizeTextbox.Size = new System.Drawing.Size(157, 20);
            this.HeaderSizeTextbox.TabIndex = 7;
            this.HeaderSizeTextbox.Text = "0";
            this.HeaderSizeTextbox.TextChanged += new System.EventHandler(this.HeaderSizeTextbox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Width:";
            // 
            // TerrainWidthTextbox
            // 
            this.TerrainWidthTextbox.Location = new System.Drawing.Point(82, 98);
            this.TerrainWidthTextbox.Name = "TerrainWidthTextbox";
            this.TerrainWidthTextbox.ReadOnly = true;
            this.TerrainWidthTextbox.Size = new System.Drawing.Size(157, 20);
            this.TerrainWidthTextbox.TabIndex = 5;
            this.TerrainWidthTextbox.Text = "0";
            this.TerrainWidthTextbox.TextChanged += new System.EventHandler(this.TerrainWidthTextbox_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Y Scale:";
            // 
            // YScaleTextbox
            // 
            this.YScaleTextbox.Location = new System.Drawing.Point(82, 46);
            this.YScaleTextbox.Name = "YScaleTextbox";
            this.YScaleTextbox.Size = new System.Drawing.Size(157, 20);
            this.YScaleTextbox.TabIndex = 3;
            this.YScaleTextbox.Text = "1.00";
            this.YScaleTextbox.TextChanged += new System.EventHandler(this.YScaleTextbox_TextChanged);
            // 
            // AutoDetectCheckbox
            // 
            this.AutoDetectCheckbox.AutoSize = true;
            this.AutoDetectCheckbox.Checked = true;
            this.AutoDetectCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoDetectCheckbox.Location = new System.Drawing.Point(82, 124);
            this.AutoDetectCheckbox.Name = "AutoDetectCheckbox";
            this.AutoDetectCheckbox.Size = new System.Drawing.Size(112, 17);
            this.AutoDetectCheckbox.TabIndex = 2;
            this.AutoDetectCheckbox.Text = "Auto-detect Width";
            this.AutoDetectCheckbox.UseVisualStyleBackColor = true;
            this.AutoDetectCheckbox.CheckedChanged += new System.EventHandler(this.AutoDetectCheckbox_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Format:";
            // 
            // FormatCombobox
            // 
            this.FormatCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FormatCombobox.FormattingEnabled = true;
            this.FormatCombobox.Items.AddRange(new object[] {
            "8-bit unsigned",
            "16-bit signed",
            "16-bit unsigned",
            "32-bit floating point"});
            this.FormatCombobox.Location = new System.Drawing.Point(82, 19);
            this.FormatCombobox.Name = "FormatCombobox";
            this.FormatCombobox.Size = new System.Drawing.Size(157, 21);
            this.FormatCombobox.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Tex4PathButton);
            this.groupBox1.Controls.Add(this.Tex3PathButton);
            this.groupBox1.Controls.Add(this.Tex2PathButton);
            this.groupBox1.Controls.Add(this.Tex1PathButton);
            this.groupBox1.Controls.Add(this.Tex0PathButton);
            this.groupBox1.Controls.Add(this.HMPathButton);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Tex4PathTextbox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Tex3PathTextbox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Tex2PathTextbox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Tex1PathTextbox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Tex0PathTextbox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.HMPathTextbox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 173);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Paths";
            // 
            // Tex4PathButton
            // 
            this.Tex4PathButton.Location = new System.Drawing.Point(280, 145);
            this.Tex4PathButton.Name = "Tex4PathButton";
            this.Tex4PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex4PathButton.TabIndex = 19;
            this.Tex4PathButton.Text = "...";
            this.Tex4PathButton.UseVisualStyleBackColor = true;
            this.Tex4PathButton.Click += new System.EventHandler(this.TexPathButton_Click);
            // 
            // Tex3PathButton
            // 
            this.Tex3PathButton.Location = new System.Drawing.Point(280, 119);
            this.Tex3PathButton.Name = "Tex3PathButton";
            this.Tex3PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex3PathButton.TabIndex = 18;
            this.Tex3PathButton.Text = "...";
            this.Tex3PathButton.UseVisualStyleBackColor = true;
            this.Tex3PathButton.Click += new System.EventHandler(this.TexPathButton_Click);
            // 
            // Tex2PathButton
            // 
            this.Tex2PathButton.Location = new System.Drawing.Point(280, 93);
            this.Tex2PathButton.Name = "Tex2PathButton";
            this.Tex2PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex2PathButton.TabIndex = 17;
            this.Tex2PathButton.Text = "...";
            this.Tex2PathButton.UseVisualStyleBackColor = true;
            this.Tex2PathButton.Click += new System.EventHandler(this.TexPathButton_Click);
            // 
            // Tex1PathButton
            // 
            this.Tex1PathButton.Location = new System.Drawing.Point(280, 67);
            this.Tex1PathButton.Name = "Tex1PathButton";
            this.Tex1PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex1PathButton.TabIndex = 16;
            this.Tex1PathButton.Text = "...";
            this.Tex1PathButton.UseVisualStyleBackColor = true;
            this.Tex1PathButton.Click += new System.EventHandler(this.TexPathButton_Click);
            // 
            // Tex0PathButton
            // 
            this.Tex0PathButton.Location = new System.Drawing.Point(280, 41);
            this.Tex0PathButton.Name = "Tex0PathButton";
            this.Tex0PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex0PathButton.TabIndex = 15;
            this.Tex0PathButton.Text = "...";
            this.Tex0PathButton.UseVisualStyleBackColor = true;
            this.Tex0PathButton.Click += new System.EventHandler(this.TexPathButton_Click);
            // 
            // HMPathButton
            // 
            this.HMPathButton.Location = new System.Drawing.Point(280, 15);
            this.HMPathButton.Name = "HMPathButton";
            this.HMPathButton.Size = new System.Drawing.Size(24, 20);
            this.HMPathButton.TabIndex = 14;
            this.HMPathButton.Text = "...";
            this.HMPathButton.UseVisualStyleBackColor = true;
            this.HMPathButton.Click += new System.EventHandler(this.HMPathButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Texture 5:";
            // 
            // Tex4PathTextbox
            // 
            this.Tex4PathTextbox.Location = new System.Drawing.Point(74, 145);
            this.Tex4PathTextbox.Name = "Tex4PathTextbox";
            this.Tex4PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex4PathTextbox.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Texture 4:";
            // 
            // Tex3PathTextbox
            // 
            this.Tex3PathTextbox.Location = new System.Drawing.Point(74, 119);
            this.Tex3PathTextbox.Name = "Tex3PathTextbox";
            this.Tex3PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex3PathTextbox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Texture 3:";
            // 
            // Tex2PathTextbox
            // 
            this.Tex2PathTextbox.Location = new System.Drawing.Point(74, 93);
            this.Tex2PathTextbox.Name = "Tex2PathTextbox";
            this.Tex2PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex2PathTextbox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Texture 2:";
            // 
            // Tex1PathTextbox
            // 
            this.Tex1PathTextbox.Location = new System.Drawing.Point(74, 67);
            this.Tex1PathTextbox.Name = "Tex1PathTextbox";
            this.Tex1PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex1PathTextbox.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Texture 1:";
            // 
            // Tex0PathTextbox
            // 
            this.Tex0PathTextbox.Location = new System.Drawing.Point(74, 41);
            this.Tex0PathTextbox.Name = "Tex0PathTextbox";
            this.Tex0PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex0PathTextbox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Height Data:";
            // 
            // HMPathTextbox
            // 
            this.HMPathTextbox.Location = new System.Drawing.Point(74, 15);
            this.HMPathTextbox.Name = "HMPathTextbox";
            this.HMPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.HMPathTextbox.TabIndex = 0;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(496, 191);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 15;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(415, 191);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 14;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // OpenRAWDialog
            // 
            this.OpenRAWDialog.Filter = "RAW file|*.raw|All files|*.*";
            // 
            // OpenTextureDialog
            // 
            this.OpenTextureDialog.Filter = resources.GetString("OpenTextureDialog.Filter");
            // 
            // RAWImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 219);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RAWImportDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Import RAW file...";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TerrainWidthTextbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox YScaleTextbox;
        private System.Windows.Forms.CheckBox AutoDetectCheckbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox FormatCombobox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Tex4PathButton;
        private System.Windows.Forms.Button Tex3PathButton;
        private System.Windows.Forms.Button Tex2PathButton;
        private System.Windows.Forms.Button Tex1PathButton;
        private System.Windows.Forms.Button Tex0PathButton;
        private System.Windows.Forms.Button HMPathButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Tex4PathTextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Tex3PathTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Tex2PathTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Tex1PathTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Tex0PathTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox HMPathTextbox;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox HeaderSizeTextbox;
        private System.Windows.Forms.OpenFileDialog OpenRAWDialog;
        private System.Windows.Forms.OpenFileDialog OpenTextureDialog;
    }
}