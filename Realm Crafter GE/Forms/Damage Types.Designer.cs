namespace RealmCrafter_GE
{
    partial class Damage_Types
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
            this.DamageTypeText = new System.Windows.Forms.TextBox[20];
            this.DamageTypeRadio = new System.Windows.Forms.RadioButton[20];
            this.DamageTypeAccept = new System.Windows.Forms.Button();
            for (int i = 0; i < 20; ++i)
                this.DamageTypeText[i] = new System.Windows.Forms.TextBox();
            for (int i = 0; i < 20; ++i)
                this.DamageTypeRadio[i] = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // DamageTypeAccept
            // 
            this.DamageTypeAccept.Location = new System.Drawing.Point(113, 400);
            this.DamageTypeAccept.Name = "DamageTypeAccept";
            this.DamageTypeAccept.Size = new System.Drawing.Size(75, 23);
            this.DamageTypeAccept.TabIndex = 40;
            this.DamageTypeAccept.Text = "Accept";
            this.DamageTypeAccept.UseVisualStyleBackColor = true;
            this.DamageTypeAccept.Click += new System.EventHandler(this.DamageTypeAccept_Click);
            // 
            // DamageTypeText
            // 
            for (int i = 0; i < 20; ++i)
            {
                this.DamageTypeText[i].Location = new System.Drawing.Point(45, 12 + (i * 19));
                this.DamageTypeText[i].MaxLength = 100;
                this.DamageTypeText[i].Name = "DamageTypeText" + i.ToString();
                this.DamageTypeText[i].Size = new System.Drawing.Size(237, 20);
                this.DamageTypeText[i].TabIndex = i + 20;
                this.DamageTypeText[i].WordWrap = false;
            }
            // 
            // DamageTypeRadio
            // 
            for (int i = 0; i < 20; ++i)
            {
                this.DamageTypeRadio[i].AutoSize = true;
                this.DamageTypeRadio[i].Location = new System.Drawing.Point(12, 13 + (i * 19));
                this.DamageTypeRadio[i].Name = "DamageTypeRadio" + i.ToString();
                this.DamageTypeRadio[i].Size = new System.Drawing.Size(14, 13);
                this.DamageTypeRadio[i].TabIndex = i;
                this.DamageTypeRadio[i].TabStop = true;
                this.DamageTypeRadio[i].UseVisualStyleBackColor = true;
            }
            // 
            // Damage_Types
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 427);
            for (int i = 0; i < 20; ++i)
                this.Controls.Add(this.DamageTypeRadio[i]);
            for (int i = 0; i < 20; ++i)
                this.Controls.Add(this.DamageTypeText[i]);
            this.Controls.Add(this.DamageTypeAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Damage_Types";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Damage Type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button DamageTypeAccept;
        public System.Windows.Forms.TextBox[] DamageTypeText;
        public System.Windows.Forms.RadioButton[] DamageTypeRadio;
    }
}