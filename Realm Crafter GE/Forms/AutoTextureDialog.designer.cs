namespace RCTTest
{
    partial class AutoTextureDialog
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
            this.ConfigureSlope = new RCTPlugin.VariableSlope();
            this.ConfigureHeight = new RCTPlugin.VariableHeight();
            this.ConfigureTexture0 = new RCTPlugin.AutoTextureNode();
            this.ConfigureTexture1 = new RCTPlugin.AutoTextureNode();
            this.ConfigureTexture2 = new RCTPlugin.AutoTextureNode();
            this.ConfigureTexture3 = new RCTPlugin.AutoTextureNode();
            this.ConfigureTexture4 = new RCTPlugin.AutoTextureNode();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConfigureSlope
            // 
            this.ConfigureSlope.Location = new System.Drawing.Point(12, 12);
            this.ConfigureSlope.Max = 1;
            this.ConfigureSlope.Min = 0;
            this.ConfigureSlope.Name = "ConfigureSlope";
            this.ConfigureSlope.Size = new System.Drawing.Size(256, 474);
            this.ConfigureSlope.TabIndex = 0;
            // 
            // ConfigureHeight
            // 
            this.ConfigureHeight.High = 100;
            this.ConfigureHeight.Location = new System.Drawing.Point(274, 12);
            this.ConfigureHeight.Low = -50;
            this.ConfigureHeight.Max = 500;
            this.ConfigureHeight.Min = -100;
            this.ConfigureHeight.Name = "ConfigureHeight";
            this.ConfigureHeight.Size = new System.Drawing.Size(64, 474);
            this.ConfigureHeight.TabIndex = 1;
            // 
            // ConfigureTexture0
            // 
            this.ConfigureTexture0.AllowTextureChange = true;
            this.ConfigureTexture0.HeightMax = 0;
            this.ConfigureTexture0.HeightMin = 0;
            this.ConfigureTexture0.Location = new System.Drawing.Point(344, 12);
            this.ConfigureTexture0.Name = "ConfigureTexture0";
            this.ConfigureTexture0.Selected = false;
            this.ConfigureTexture0.Size = new System.Drawing.Size(491, 90);
            this.ConfigureTexture0.SlopeMax = 0;
            this.ConfigureTexture0.SlopeMin = 0;
            this.ConfigureTexture0.TabIndex = 2;
            this.ConfigureTexture0.TexturePath = null;
            this.ConfigureTexture0.Click += new System.EventHandler(this.ConfigureTexture_Click);
            // 
            // ConfigureTexture1
            // 
            this.ConfigureTexture1.AllowTextureChange = true;
            this.ConfigureTexture1.HeightMax = 0;
            this.ConfigureTexture1.HeightMin = 0;
            this.ConfigureTexture1.Location = new System.Drawing.Point(344, 108);
            this.ConfigureTexture1.Name = "ConfigureTexture1";
            this.ConfigureTexture1.Selected = false;
            this.ConfigureTexture1.Size = new System.Drawing.Size(491, 90);
            this.ConfigureTexture1.SlopeMax = 0;
            this.ConfigureTexture1.SlopeMin = 0;
            this.ConfigureTexture1.TabIndex = 3;
            this.ConfigureTexture1.TexturePath = null;
            this.ConfigureTexture1.Click += new System.EventHandler(this.ConfigureTexture_Click);
            // 
            // ConfigureTexture2
            // 
            this.ConfigureTexture2.AllowTextureChange = true;
            this.ConfigureTexture2.HeightMax = 0;
            this.ConfigureTexture2.HeightMin = 0;
            this.ConfigureTexture2.Location = new System.Drawing.Point(344, 204);
            this.ConfigureTexture2.Name = "ConfigureTexture2";
            this.ConfigureTexture2.Selected = false;
            this.ConfigureTexture2.Size = new System.Drawing.Size(491, 90);
            this.ConfigureTexture2.SlopeMax = 0;
            this.ConfigureTexture2.SlopeMin = 0;
            this.ConfigureTexture2.TabIndex = 4;
            this.ConfigureTexture2.TexturePath = null;
            this.ConfigureTexture2.Click += new System.EventHandler(this.ConfigureTexture_Click);
            // 
            // ConfigureTexture3
            // 
            this.ConfigureTexture3.AllowTextureChange = true;
            this.ConfigureTexture3.HeightMax = 0;
            this.ConfigureTexture3.HeightMin = 0;
            this.ConfigureTexture3.Location = new System.Drawing.Point(344, 300);
            this.ConfigureTexture3.Name = "ConfigureTexture3";
            this.ConfigureTexture3.Selected = false;
            this.ConfigureTexture3.Size = new System.Drawing.Size(491, 90);
            this.ConfigureTexture3.SlopeMax = 0;
            this.ConfigureTexture3.SlopeMin = 0;
            this.ConfigureTexture3.TabIndex = 5;
            this.ConfigureTexture3.TexturePath = null;
            this.ConfigureTexture3.Click += new System.EventHandler(this.ConfigureTexture_Click);
            // 
            // ConfigureTexture4
            // 
            this.ConfigureTexture4.AllowTextureChange = true;
            this.ConfigureTexture4.HeightMax = 0;
            this.ConfigureTexture4.HeightMin = 0;
            this.ConfigureTexture4.Location = new System.Drawing.Point(344, 396);
            this.ConfigureTexture4.Name = "ConfigureTexture4";
            this.ConfigureTexture4.Selected = false;
            this.ConfigureTexture4.Size = new System.Drawing.Size(491, 90);
            this.ConfigureTexture4.SlopeMax = 0;
            this.ConfigureTexture4.SlopeMin = 0;
            this.ConfigureTexture4.TabIndex = 6;
            this.ConfigureTexture4.TexturePath = null;
            this.ConfigureTexture4.Click += new System.EventHandler(this.ConfigureTexture_Click);
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(679, 492);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 15;
            this.OKBtn.Text = "Texture";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(760, 492);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 16;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // AutoTextureDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 521);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.ConfigureTexture4);
            this.Controls.Add(this.ConfigureTexture3);
            this.Controls.Add(this.ConfigureTexture2);
            this.Controls.Add(this.ConfigureTexture1);
            this.Controls.Add(this.ConfigureTexture0);
            this.Controls.Add(this.ConfigureHeight);
            this.Controls.Add(this.ConfigureSlope);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoTextureDialog";
            this.ShowIcon = false;
            this.Text = "AutoTexture...";
            this.ResumeLayout(false);

        }

        #endregion

        private RCTPlugin.VariableSlope ConfigureSlope;
        private RCTPlugin.VariableHeight ConfigureHeight;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        public RCTPlugin.AutoTextureNode ConfigureTexture0;
        public RCTPlugin.AutoTextureNode ConfigureTexture1;
        public RCTPlugin.AutoTextureNode ConfigureTexture2;
        public RCTPlugin.AutoTextureNode ConfigureTexture3;
        public RCTPlugin.AutoTextureNode ConfigureTexture4;
    }
}