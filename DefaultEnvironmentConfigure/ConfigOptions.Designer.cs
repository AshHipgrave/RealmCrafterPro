namespace DefaultEnvironmentConfigure
{
    partial class ConfigOptions
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
            this.StarsButton = new System.Windows.Forms.Button();
            this.CloudsButton = new System.Windows.Forms.Button();
            this.GradientButton = new System.Windows.Forms.Button();
            this.FlareButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.CnlButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Stars:";
            // 
            // StarsButton
            // 
            this.StarsButton.Location = new System.Drawing.Point(91, 12);
            this.StarsButton.Name = "StarsButton";
            this.StarsButton.Size = new System.Drawing.Size(140, 23);
            this.StarsButton.TabIndex = 1;
            this.StarsButton.Text = "button1";
            this.StarsButton.UseVisualStyleBackColor = true;
            this.StarsButton.Click += new System.EventHandler(this.StarsButton_Click);
            // 
            // CloudsButton
            // 
            this.CloudsButton.Location = new System.Drawing.Point(91, 41);
            this.CloudsButton.Name = "CloudsButton";
            this.CloudsButton.Size = new System.Drawing.Size(140, 23);
            this.CloudsButton.TabIndex = 2;
            this.CloudsButton.Text = "button2";
            this.CloudsButton.UseVisualStyleBackColor = true;
            this.CloudsButton.Click += new System.EventHandler(this.CloudsButton_Click);
            // 
            // GradientButton
            // 
            this.GradientButton.Location = new System.Drawing.Point(91, 70);
            this.GradientButton.Name = "GradientButton";
            this.GradientButton.Size = new System.Drawing.Size(140, 23);
            this.GradientButton.TabIndex = 3;
            this.GradientButton.Text = "button3";
            this.GradientButton.UseVisualStyleBackColor = true;
            this.GradientButton.Click += new System.EventHandler(this.GradientButton_Click);
            // 
            // FlareButton
            // 
            this.FlareButton.Location = new System.Drawing.Point(91, 99);
            this.FlareButton.Name = "FlareButton";
            this.FlareButton.Size = new System.Drawing.Size(140, 23);
            this.FlareButton.TabIndex = 4;
            this.FlareButton.Text = "button4";
            this.FlareButton.UseVisualStyleBackColor = true;
            this.FlareButton.Click += new System.EventHandler(this.FlareButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(75, 141);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 5;
            this.OKButton.Text = "Save";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CnlButton
            // 
            this.CnlButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CnlButton.Location = new System.Drawing.Point(156, 141);
            this.CnlButton.Name = "CnlButton";
            this.CnlButton.Size = new System.Drawing.Size(75, 23);
            this.CnlButton.TabIndex = 6;
            this.CnlButton.Text = "Cancel";
            this.CnlButton.UseVisualStyleBackColor = true;
            this.CnlButton.Click += new System.EventHandler(this.CnlButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Clouds:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Gradient:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Flare:";
            // 
            // ConfigOptions
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CnlButton;
            this.ClientSize = new System.Drawing.Size(242, 170);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CnlButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.FlareButton);
            this.Controls.Add(this.GradientButton);
            this.Controls.Add(this.CloudsButton);
            this.Controls.Add(this.StarsButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Configure Environment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StarsButton;
        private System.Windows.Forms.Button CloudsButton;
        private System.Windows.Forms.Button GradientButton;
        private System.Windows.Forms.Button FlareButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CnlButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}