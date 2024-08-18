namespace RenderingServices
{
    partial class SliderEditorForm
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
            this.XSlide = new System.Windows.Forms.PictureBox();
            this.YSlide = new System.Windows.Forms.PictureBox();
            this.ZSlide = new System.Windows.Forms.PictureBox();
            this.WSlide = new System.Windows.Forms.PictureBox();
            this.XLabel = new System.Windows.Forms.Label();
            this.YLabel = new System.Windows.Forms.Label();
            this.ZLabel = new System.Windows.Forms.Label();
            this.WLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.XSlide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YSlide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZSlide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WSlide)).BeginInit();
            this.SuspendLayout();
            // 
            // XSlide
            // 
            this.XSlide.Location = new System.Drawing.Point(12, 12);
            this.XSlide.Name = "XSlide";
            this.XSlide.Size = new System.Drawing.Size(128, 12);
            this.XSlide.TabIndex = 0;
            this.XSlide.TabStop = false;
            this.XSlide.MouseMove += new System.Windows.Forms.MouseEventHandler(this.XSlide_MouseMove);
            this.XSlide.MouseDown += new System.Windows.Forms.MouseEventHandler(this.XSlide_MouseDown);
            this.XSlide.Paint += new System.Windows.Forms.PaintEventHandler(this.XSlide_Paint);
            this.XSlide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.XSlide_MouseUp);
            // 
            // YSlide
            // 
            this.YSlide.Location = new System.Drawing.Point(12, 30);
            this.YSlide.Name = "YSlide";
            this.YSlide.Size = new System.Drawing.Size(128, 12);
            this.YSlide.TabIndex = 1;
            this.YSlide.TabStop = false;
            this.YSlide.MouseMove += new System.Windows.Forms.MouseEventHandler(this.YSlide_MouseMove);
            this.YSlide.MouseDown += new System.Windows.Forms.MouseEventHandler(this.YSlide_MouseDown);
            this.YSlide.Paint += new System.Windows.Forms.PaintEventHandler(this.YSlide_Paint);
            this.YSlide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.YSlide_MouseUp);
            // 
            // ZSlide
            // 
            this.ZSlide.Location = new System.Drawing.Point(12, 48);
            this.ZSlide.Name = "ZSlide";
            this.ZSlide.Size = new System.Drawing.Size(128, 12);
            this.ZSlide.TabIndex = 2;
            this.ZSlide.TabStop = false;
            this.ZSlide.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ZSlide_MouseMove);
            this.ZSlide.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ZSlide_MouseDown);
            this.ZSlide.Paint += new System.Windows.Forms.PaintEventHandler(this.ZSlide_Paint);
            this.ZSlide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ZSlide_MouseUp);
            // 
            // WSlide
            // 
            this.WSlide.Location = new System.Drawing.Point(12, 66);
            this.WSlide.Name = "WSlide";
            this.WSlide.Size = new System.Drawing.Size(128, 12);
            this.WSlide.TabIndex = 3;
            this.WSlide.TabStop = false;
            this.WSlide.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WSlide_MouseMove);
            this.WSlide.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WSlide_MouseDown);
            this.WSlide.Paint += new System.Windows.Forms.PaintEventHandler(this.WSlide_Paint);
            this.WSlide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WSlide_MouseUp);
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Location = new System.Drawing.Point(146, 11);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(35, 13);
            this.XLabel.TabIndex = 4;
            this.XLabel.Text = "label1";
            // 
            // YLabel
            // 
            this.YLabel.AutoSize = true;
            this.YLabel.Location = new System.Drawing.Point(146, 29);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(35, 13);
            this.YLabel.TabIndex = 5;
            this.YLabel.Text = "label2";
            // 
            // ZLabel
            // 
            this.ZLabel.AutoSize = true;
            this.ZLabel.Location = new System.Drawing.Point(146, 47);
            this.ZLabel.Name = "ZLabel";
            this.ZLabel.Size = new System.Drawing.Size(35, 13);
            this.ZLabel.TabIndex = 6;
            this.ZLabel.Text = "label3";
            // 
            // WLabel
            // 
            this.WLabel.AutoSize = true;
            this.WLabel.Location = new System.Drawing.Point(146, 65);
            this.WLabel.Name = "WLabel";
            this.WLabel.Size = new System.Drawing.Size(70, 13);
            this.WLabel.TabIndex = 7;
            this.WLabel.Text = "1000.123456";
            // 
            // SliderEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 89);
            this.Controls.Add(this.WLabel);
            this.Controls.Add(this.ZLabel);
            this.Controls.Add(this.YLabel);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.WSlide);
            this.Controls.Add(this.ZSlide);
            this.Controls.Add(this.YSlide);
            this.Controls.Add(this.XSlide);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SliderEditorForm";
            this.Text = "SliderEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.XSlide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YSlide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZSlide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WSlide)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox XSlide;
        private System.Windows.Forms.PictureBox YSlide;
        private System.Windows.Forms.PictureBox ZSlide;
        private System.Windows.Forms.PictureBox WSlide;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.Label ZLabel;
        private System.Windows.Forms.Label WLabel;
    }
}