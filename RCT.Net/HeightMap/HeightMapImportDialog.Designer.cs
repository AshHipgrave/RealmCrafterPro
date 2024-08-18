namespace HeightMap
{
    partial class HeightMapImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeightMapImportDialog));
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HMPathTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Tex0PathTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Tex1PathTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Tex2PathTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Tex3PathTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Tex4PathTextbox = new System.Windows.Forms.TextBox();
            this.HMPathButton = new System.Windows.Forms.Button();
            this.Tex0PathButton = new System.Windows.Forms.Button();
            this.Tex1PathButton = new System.Windows.Forms.Button();
            this.Tex2PathButton = new System.Windows.Forms.Button();
            this.Tex3PathButton = new System.Windows.Forms.Button();
            this.Tex4PathButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GradientBox = new System.Windows.Forms.PictureBox();
            this.ConfigMaxTextbox = new System.Windows.Forms.TextBox();
            this.ConfigMinTextbox = new System.Windows.Forms.TextBox();
            this.OpenHeightmapDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenTextureDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GradientBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(366, 191);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 11;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(285, 191);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 10;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
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
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Paths";
            // 
            // HMPathTextbox
            // 
            this.HMPathTextbox.Location = new System.Drawing.Point(74, 15);
            this.HMPathTextbox.Name = "HMPathTextbox";
            this.HMPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.HMPathTextbox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "HeightMap:";
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
            // Tex0PathButton
            // 
            this.Tex0PathButton.Location = new System.Drawing.Point(280, 41);
            this.Tex0PathButton.Name = "Tex0PathButton";
            this.Tex0PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex0PathButton.TabIndex = 15;
            this.Tex0PathButton.Text = "...";
            this.Tex0PathButton.UseVisualStyleBackColor = true;
            this.Tex0PathButton.Click += new System.EventHandler(this.Tex0PathButton_Click);
            // 
            // Tex1PathButton
            // 
            this.Tex1PathButton.Location = new System.Drawing.Point(280, 67);
            this.Tex1PathButton.Name = "Tex1PathButton";
            this.Tex1PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex1PathButton.TabIndex = 16;
            this.Tex1PathButton.Text = "...";
            this.Tex1PathButton.UseVisualStyleBackColor = true;
            this.Tex1PathButton.Click += new System.EventHandler(this.Tex1PathButton_Click);
            // 
            // Tex2PathButton
            // 
            this.Tex2PathButton.Location = new System.Drawing.Point(280, 93);
            this.Tex2PathButton.Name = "Tex2PathButton";
            this.Tex2PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex2PathButton.TabIndex = 17;
            this.Tex2PathButton.Text = "...";
            this.Tex2PathButton.UseVisualStyleBackColor = true;
            this.Tex2PathButton.Click += new System.EventHandler(this.Tex2PathButton_Click);
            // 
            // Tex3PathButton
            // 
            this.Tex3PathButton.Location = new System.Drawing.Point(280, 119);
            this.Tex3PathButton.Name = "Tex3PathButton";
            this.Tex3PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex3PathButton.TabIndex = 18;
            this.Tex3PathButton.Text = "...";
            this.Tex3PathButton.UseVisualStyleBackColor = true;
            this.Tex3PathButton.Click += new System.EventHandler(this.Tex3PathButton_Click);
            // 
            // Tex4PathButton
            // 
            this.Tex4PathButton.Location = new System.Drawing.Point(280, 145);
            this.Tex4PathButton.Name = "Tex4PathButton";
            this.Tex4PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex4PathButton.TabIndex = 19;
            this.Tex4PathButton.Text = "...";
            this.Tex4PathButton.UseVisualStyleBackColor = true;
            this.Tex4PathButton.Click += new System.EventHandler(this.Tex4PathButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ConfigMinTextbox);
            this.groupBox2.Controls.Add(this.ConfigMaxTextbox);
            this.groupBox2.Controls.Add(this.GradientBox);
            this.groupBox2.Location = new System.Drawing.Point(326, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(115, 173);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // GradientBox
            // 
            this.GradientBox.Location = new System.Drawing.Point(6, 19);
            this.GradientBox.Name = "GradientBox";
            this.GradientBox.Size = new System.Drawing.Size(32, 148);
            this.GradientBox.TabIndex = 0;
            this.GradientBox.TabStop = false;
            this.GradientBox.Paint += new System.Windows.Forms.PaintEventHandler(this.GradientBox_Paint);
            // 
            // ConfigMaxTextbox
            // 
            this.ConfigMaxTextbox.Location = new System.Drawing.Point(44, 19);
            this.ConfigMaxTextbox.Name = "ConfigMaxTextbox";
            this.ConfigMaxTextbox.Size = new System.Drawing.Size(62, 20);
            this.ConfigMaxTextbox.TabIndex = 1;
            this.ConfigMaxTextbox.Text = "128.00";
            this.ConfigMaxTextbox.TextChanged += new System.EventHandler(this.ConfigMaxTextbox_TextChanged);
            // 
            // ConfigMinTextbox
            // 
            this.ConfigMinTextbox.Location = new System.Drawing.Point(44, 148);
            this.ConfigMinTextbox.Name = "ConfigMinTextbox";
            this.ConfigMinTextbox.Size = new System.Drawing.Size(62, 20);
            this.ConfigMinTextbox.TabIndex = 2;
            this.ConfigMinTextbox.Text = "0.00";
            this.ConfigMinTextbox.TextChanged += new System.EventHandler(this.ConfigMinTextbox_TextChanged);
            // 
            // OpenHeightmapDialog
            // 
            this.OpenHeightmapDialog.Filter = "All supported files|*.bmp; *.jpg; *.png|All files|*.*";
            // 
            // OpenTextureDialog
            // 
            this.OpenTextureDialog.Filter = resources.GetString("OpenTextureDialog.Filter");
            // 
            // HeightMapImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 220);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeightMapImportDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Import HeightMap...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GradientBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.Button Tex4PathButton;
        private System.Windows.Forms.Button Tex3PathButton;
        private System.Windows.Forms.Button Tex2PathButton;
        private System.Windows.Forms.Button Tex1PathButton;
        private System.Windows.Forms.Button Tex0PathButton;
        private System.Windows.Forms.Button HMPathButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox ConfigMinTextbox;
        private System.Windows.Forms.TextBox ConfigMaxTextbox;
        private System.Windows.Forms.PictureBox GradientBox;
        private System.Windows.Forms.OpenFileDialog OpenHeightmapDialog;
        private System.Windows.Forms.OpenFileDialog OpenTextureDialog;
    }
}