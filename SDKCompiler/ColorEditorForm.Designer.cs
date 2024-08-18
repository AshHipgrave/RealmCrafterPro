namespace RenderingServices
{
    partial class ColorEditorForm
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
            this.HPick = new System.Windows.Forms.PictureBox();
            this.SPick = new System.Windows.Forms.PictureBox();
            this.BPick = new System.Windows.Forms.PictureBox();
            this.NewColor = new System.Windows.Forms.PictureBox();
            this.OldColor = new System.Windows.Forms.PictureBox();
            this.BBox = new System.Windows.Forms.TextBox();
            this.GBox = new System.Windows.Forms.TextBox();
            this.RBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.APick = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.HPick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SPick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BPick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NewColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OldColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.APick)).BeginInit();
            this.SuspendLayout();
            // 
            // HPick
            // 
            this.HPick.Location = new System.Drawing.Point(12, 46);
            this.HPick.Name = "HPick";
            this.HPick.Size = new System.Drawing.Size(128, 12);
            this.HPick.TabIndex = 0;
            this.HPick.TabStop = false;
            this.HPick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HPick_MouseMove);
            this.HPick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HPick_MouseDown);
            this.HPick.Paint += new System.Windows.Forms.PaintEventHandler(this.HPick_Paint);
            this.HPick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HPick_MouseUp);
            // 
            // SPick
            // 
            this.SPick.Location = new System.Drawing.Point(12, 64);
            this.SPick.Name = "SPick";
            this.SPick.Size = new System.Drawing.Size(128, 12);
            this.SPick.TabIndex = 1;
            this.SPick.TabStop = false;
            this.SPick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SPick_MouseMove);
            this.SPick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SPick_MouseDown);
            this.SPick.Paint += new System.Windows.Forms.PaintEventHandler(this.SPick_Paint);
            this.SPick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SPick_MouseUp);
            // 
            // BPick
            // 
            this.BPick.Location = new System.Drawing.Point(12, 82);
            this.BPick.Name = "BPick";
            this.BPick.Size = new System.Drawing.Size(128, 12);
            this.BPick.TabIndex = 2;
            this.BPick.TabStop = false;
            this.BPick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BPick_MouseMove);
            this.BPick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BPick_MouseDown);
            this.BPick.Paint += new System.Windows.Forms.PaintEventHandler(this.BPick_Paint);
            this.BPick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BPick_MouseUp);
            // 
            // NewColor
            // 
            this.NewColor.Location = new System.Drawing.Point(12, 12);
            this.NewColor.Name = "NewColor";
            this.NewColor.Size = new System.Drawing.Size(61, 28);
            this.NewColor.TabIndex = 3;
            this.NewColor.TabStop = false;
            this.NewColor.Paint += new System.Windows.Forms.PaintEventHandler(this.NewColor_Paint);
            // 
            // OldColor
            // 
            this.OldColor.Location = new System.Drawing.Point(79, 12);
            this.OldColor.Name = "OldColor";
            this.OldColor.Size = new System.Drawing.Size(61, 28);
            this.OldColor.TabIndex = 4;
            this.OldColor.TabStop = false;
            this.OldColor.Paint += new System.Windows.Forms.PaintEventHandler(this.OldColor_Paint);
            // 
            // BBox
            // 
            this.BBox.Location = new System.Drawing.Point(115, 118);
            this.BBox.Name = "BBox";
            this.BBox.Size = new System.Drawing.Size(25, 20);
            this.BBox.TabIndex = 5;
            this.BBox.Text = "255";
            this.BBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BBox_KeyUp);
            // 
            // GBox
            // 
            this.GBox.Location = new System.Drawing.Point(71, 118);
            this.GBox.Name = "GBox";
            this.GBox.Size = new System.Drawing.Size(27, 20);
            this.GBox.TabIndex = 6;
            this.GBox.Text = "255";
            this.GBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GBox_KeyUp);
            // 
            // RBox
            // 
            this.RBox.Location = new System.Drawing.Point(25, 118);
            this.RBox.Name = "RBox";
            this.RBox.Size = new System.Drawing.Size(26, 20);
            this.RBox.TabIndex = 7;
            this.RBox.Text = "255";
            this.RBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RBox_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "R:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "B:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "G:";
            // 
            // APick
            // 
            this.APick.Location = new System.Drawing.Point(12, 100);
            this.APick.Name = "APick";
            this.APick.Size = new System.Drawing.Size(128, 12);
            this.APick.TabIndex = 11;
            this.APick.TabStop = false;
            this.APick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.APick_MouseMove);
            this.APick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.APick_MouseDown);
            this.APick.Paint += new System.Windows.Forms.PaintEventHandler(this.APick_Paint);
            this.APick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.APick_MouseUp);
            // 
            // ColorEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 147);
            this.Controls.Add(this.APick);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RBox);
            this.Controls.Add(this.GBox);
            this.Controls.Add(this.BBox);
            this.Controls.Add(this.OldColor);
            this.Controls.Add(this.NewColor);
            this.Controls.Add(this.BPick);
            this.Controls.Add(this.SPick);
            this.Controls.Add(this.HPick);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ColorEditorForm";
            this.Text = "ColorEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.HPick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SPick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BPick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NewColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OldColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.APick)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox HPick;
        private System.Windows.Forms.PictureBox SPick;
        private System.Windows.Forms.PictureBox BPick;
        private System.Windows.Forms.PictureBox NewColor;
        private System.Windows.Forms.PictureBox OldColor;
        private System.Windows.Forms.TextBox BBox;
        private System.Windows.Forms.TextBox GBox;
        private System.Windows.Forms.TextBox RBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox APick;

    }
}