namespace L3DT
{
    partial class L3DTImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(L3DTImportDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HMPathButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.HMPathTextbox = new System.Windows.Forms.TextBox();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SplatAPathButton = new System.Windows.Forms.Button();
            this.SplatBPathButton = new System.Windows.Forms.Button();
            this.SplatGPathButton = new System.Windows.Forms.Button();
            this.SplatRPathButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.SplatAPathTextbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SplatBPathTextbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SplatGPathTextbox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SplatRPathTextbox = new System.Windows.Forms.TextBox();
            this.ImportXMLButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Tex4PathButton = new System.Windows.Forms.Button();
            this.Tex3PathButton = new System.Windows.Forms.Button();
            this.Tex2PathButton = new System.Windows.Forms.Button();
            this.Tex1PathButton = new System.Windows.Forms.Button();
            this.Tex0PathButton = new System.Windows.Forms.Button();
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
            this.OpenHFZDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenSplatDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenTextureDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenXMLDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.HMPathButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.HMPathTextbox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 43);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Height Map";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "HFZ File:";
            // 
            // HMPathTextbox
            // 
            this.HMPathTextbox.Location = new System.Drawing.Point(74, 15);
            this.HMPathTextbox.Name = "HMPathTextbox";
            this.HMPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.HMPathTextbox.TabIndex = 0;
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(164, 379);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 13;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(245, 379);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 14;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SplatAPathButton);
            this.groupBox2.Controls.Add(this.SplatBPathButton);
            this.groupBox2.Controls.Add(this.SplatGPathButton);
            this.groupBox2.Controls.Add(this.SplatRPathButton);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.SplatAPathTextbox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.SplatBPathTextbox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.SplatGPathTextbox);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.SplatRPathTextbox);
            this.groupBox2.Controls.Add(this.ImportXMLButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(308, 153);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Splat Maps";
            // 
            // SplatAPathButton
            // 
            this.SplatAPathButton.Location = new System.Drawing.Point(280, 126);
            this.SplatAPathButton.Name = "SplatAPathButton";
            this.SplatAPathButton.Size = new System.Drawing.Size(24, 20);
            this.SplatAPathButton.TabIndex = 30;
            this.SplatAPathButton.Text = "...";
            this.SplatAPathButton.UseVisualStyleBackColor = true;
            this.SplatAPathButton.Click += new System.EventHandler(this.SplatPathButton_Click);
            // 
            // SplatBPathButton
            // 
            this.SplatBPathButton.Location = new System.Drawing.Point(280, 100);
            this.SplatBPathButton.Name = "SplatBPathButton";
            this.SplatBPathButton.Size = new System.Drawing.Size(24, 20);
            this.SplatBPathButton.TabIndex = 29;
            this.SplatBPathButton.Text = "...";
            this.SplatBPathButton.UseVisualStyleBackColor = true;
            this.SplatBPathButton.Click += new System.EventHandler(this.SplatPathButton_Click);
            // 
            // SplatGPathButton
            // 
            this.SplatGPathButton.Location = new System.Drawing.Point(280, 74);
            this.SplatGPathButton.Name = "SplatGPathButton";
            this.SplatGPathButton.Size = new System.Drawing.Size(24, 20);
            this.SplatGPathButton.TabIndex = 28;
            this.SplatGPathButton.Text = "...";
            this.SplatGPathButton.UseVisualStyleBackColor = true;
            this.SplatGPathButton.Click += new System.EventHandler(this.SplatPathButton_Click);
            // 
            // SplatRPathButton
            // 
            this.SplatRPathButton.Location = new System.Drawing.Point(280, 48);
            this.SplatRPathButton.Name = "SplatRPathButton";
            this.SplatRPathButton.Size = new System.Drawing.Size(24, 20);
            this.SplatRPathButton.TabIndex = 27;
            this.SplatRPathButton.Text = "...";
            this.SplatRPathButton.UseVisualStyleBackColor = true;
            this.SplatRPathButton.Click += new System.EventHandler(this.SplatPathButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 129);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Splat A:";
            // 
            // SplatAPathTextbox
            // 
            this.SplatAPathTextbox.Location = new System.Drawing.Point(74, 126);
            this.SplatAPathTextbox.Name = "SplatAPathTextbox";
            this.SplatAPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.SplatAPathTextbox.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Splat B:";
            // 
            // SplatBPathTextbox
            // 
            this.SplatBPathTextbox.Location = new System.Drawing.Point(74, 100);
            this.SplatBPathTextbox.Name = "SplatBPathTextbox";
            this.SplatBPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.SplatBPathTextbox.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Splat G:";
            // 
            // SplatGPathTextbox
            // 
            this.SplatGPathTextbox.Location = new System.Drawing.Point(74, 74);
            this.SplatGPathTextbox.Name = "SplatGPathTextbox";
            this.SplatGPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.SplatGPathTextbox.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Splat R:";
            // 
            // SplatRPathTextbox
            // 
            this.SplatRPathTextbox.Location = new System.Drawing.Point(74, 48);
            this.SplatRPathTextbox.Name = "SplatRPathTextbox";
            this.SplatRPathTextbox.Size = new System.Drawing.Size(200, 20);
            this.SplatRPathTextbox.TabIndex = 19;
            // 
            // ImportXMLButton
            // 
            this.ImportXMLButton.Location = new System.Drawing.Point(6, 19);
            this.ImportXMLButton.Name = "ImportXMLButton";
            this.ImportXMLButton.Size = new System.Drawing.Size(75, 23);
            this.ImportXMLButton.TabIndex = 0;
            this.ImportXMLButton.Text = "Import XML";
            this.ImportXMLButton.UseVisualStyleBackColor = true;
            this.ImportXMLButton.Click += new System.EventHandler(this.ImportXMLButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Tex4PathButton);
            this.groupBox3.Controls.Add(this.Tex3PathButton);
            this.groupBox3.Controls.Add(this.Tex2PathButton);
            this.groupBox3.Controls.Add(this.Tex1PathButton);
            this.groupBox3.Controls.Add(this.Tex0PathButton);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.Tex4PathTextbox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.Tex3PathTextbox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.Tex2PathTextbox);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.Tex1PathTextbox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.Tex0PathTextbox);
            this.groupBox3.Location = new System.Drawing.Point(12, 220);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(308, 153);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Textures";
            // 
            // Tex4PathButton
            // 
            this.Tex4PathButton.Location = new System.Drawing.Point(280, 123);
            this.Tex4PathButton.Name = "Tex4PathButton";
            this.Tex4PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex4PathButton.TabIndex = 34;
            this.Tex4PathButton.Text = "...";
            this.Tex4PathButton.UseVisualStyleBackColor = true;
            this.Tex4PathButton.Click += new System.EventHandler(this.TexturePathButton_Click);
            // 
            // Tex3PathButton
            // 
            this.Tex3PathButton.Location = new System.Drawing.Point(280, 97);
            this.Tex3PathButton.Name = "Tex3PathButton";
            this.Tex3PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex3PathButton.TabIndex = 33;
            this.Tex3PathButton.Text = "...";
            this.Tex3PathButton.UseVisualStyleBackColor = true;
            this.Tex3PathButton.Click += new System.EventHandler(this.TexturePathButton_Click);
            // 
            // Tex2PathButton
            // 
            this.Tex2PathButton.Location = new System.Drawing.Point(280, 71);
            this.Tex2PathButton.Name = "Tex2PathButton";
            this.Tex2PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex2PathButton.TabIndex = 32;
            this.Tex2PathButton.Text = "...";
            this.Tex2PathButton.UseVisualStyleBackColor = true;
            this.Tex2PathButton.Click += new System.EventHandler(this.TexturePathButton_Click);
            // 
            // Tex1PathButton
            // 
            this.Tex1PathButton.Location = new System.Drawing.Point(280, 45);
            this.Tex1PathButton.Name = "Tex1PathButton";
            this.Tex1PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex1PathButton.TabIndex = 31;
            this.Tex1PathButton.Text = "...";
            this.Tex1PathButton.UseVisualStyleBackColor = true;
            this.Tex1PathButton.Click += new System.EventHandler(this.TexturePathButton_Click);
            // 
            // Tex0PathButton
            // 
            this.Tex0PathButton.Location = new System.Drawing.Point(280, 19);
            this.Tex0PathButton.Name = "Tex0PathButton";
            this.Tex0PathButton.Size = new System.Drawing.Size(24, 20);
            this.Tex0PathButton.TabIndex = 30;
            this.Tex0PathButton.Text = "...";
            this.Tex0PathButton.UseVisualStyleBackColor = true;
            this.Tex0PathButton.Click += new System.EventHandler(this.TexturePathButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Texture 5:";
            // 
            // Tex4PathTextbox
            // 
            this.Tex4PathTextbox.Location = new System.Drawing.Point(74, 123);
            this.Tex4PathTextbox.Name = "Tex4PathTextbox";
            this.Tex4PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex4PathTextbox.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Texture 4:";
            // 
            // Tex3PathTextbox
            // 
            this.Tex3PathTextbox.Location = new System.Drawing.Point(74, 97);
            this.Tex3PathTextbox.Name = "Tex3PathTextbox";
            this.Tex3PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex3PathTextbox.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Texture 3:";
            // 
            // Tex2PathTextbox
            // 
            this.Tex2PathTextbox.Location = new System.Drawing.Point(74, 71);
            this.Tex2PathTextbox.Name = "Tex2PathTextbox";
            this.Tex2PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex2PathTextbox.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Texture 2:";
            // 
            // Tex1PathTextbox
            // 
            this.Tex1PathTextbox.Location = new System.Drawing.Point(74, 45);
            this.Tex1PathTextbox.Name = "Tex1PathTextbox";
            this.Tex1PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex1PathTextbox.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Texture 1:";
            // 
            // Tex0PathTextbox
            // 
            this.Tex0PathTextbox.Location = new System.Drawing.Point(74, 19);
            this.Tex0PathTextbox.Name = "Tex0PathTextbox";
            this.Tex0PathTextbox.Size = new System.Drawing.Size(200, 20);
            this.Tex0PathTextbox.TabIndex = 20;
            // 
            // OpenHFZDialog
            // 
            this.OpenHFZDialog.Filter = "L3DT HFZ files|*.hfz|All files|*.*";
            // 
            // OpenSplatDialog
            // 
            this.OpenSplatDialog.Filter = "All supported files|*.bmp; *.jpg; *.png|All files|*.*";
            // 
            // OpenTextureDialog
            // 
            this.OpenTextureDialog.Filter = resources.GetString("OpenTextureDialog.Filter");
            // 
            // OpenXMLDialog
            // 
            this.OpenXMLDialog.Filter = "L3DT XML files|*.xml|All files|*.*";
            // 
            // L3DTImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 413);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.CancelBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "L3DTImportDialog";
            this.ShowIcon = false;
            this.Text = "L3DTImportDialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button HMPathButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox HMPathTextbox;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button SplatAPathButton;
        private System.Windows.Forms.Button SplatBPathButton;
        private System.Windows.Forms.Button SplatGPathButton;
        private System.Windows.Forms.Button SplatRPathButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox SplatAPathTextbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox SplatBPathTextbox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox SplatGPathTextbox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox SplatRPathTextbox;
        private System.Windows.Forms.Button ImportXMLButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Tex4PathButton;
        private System.Windows.Forms.Button Tex3PathButton;
        private System.Windows.Forms.Button Tex2PathButton;
        private System.Windows.Forms.Button Tex1PathButton;
        private System.Windows.Forms.Button Tex0PathButton;
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
        private System.Windows.Forms.OpenFileDialog OpenHFZDialog;
        private System.Windows.Forms.OpenFileDialog OpenSplatDialog;
        private System.Windows.Forms.OpenFileDialog OpenTextureDialog;
        private System.Windows.Forms.OpenFileDialog OpenXMLDialog;
    }
}