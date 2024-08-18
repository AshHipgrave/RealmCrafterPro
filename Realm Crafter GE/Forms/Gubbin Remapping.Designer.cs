namespace RealmCrafter_GE
{
    partial class Gubbin_Remapping
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
            this.GubbinRemapAccept = new System.Windows.Forms.Button();
            this.GubbinRemapLabel1 = new System.Windows.Forms.Label();
            this.GubbinRemapLabel6 = new System.Windows.Forms.Label();
            this.GubbinRemapLabel4 = new System.Windows.Forms.Label();
            this.GubbinRemapLabel5 = new System.Windows.Forms.Label();
            this.GubbinRemapLabel3 = new System.Windows.Forms.Label();
            this.GubbinRemapLabel2 = new System.Windows.Forms.Label();
            this.GubbinRemapText = new System.Windows.Forms.TextBox[6];
            for (int i = 0; i < 6; ++i)
                this.GubbinRemapText[i] = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // GubbinRemapAccept
            // 
            this.GubbinRemapAccept.Location = new System.Drawing.Point(93, 160);
            this.GubbinRemapAccept.Name = "GubbinRemapAccept";
            this.GubbinRemapAccept.Size = new System.Drawing.Size(75, 23);
            this.GubbinRemapAccept.TabIndex = 6;
            this.GubbinRemapAccept.Text = "Accept";
            this.GubbinRemapAccept.UseVisualStyleBackColor = true;
            this.GubbinRemapAccept.Click += new System.EventHandler(this.GubbinRemapAccept_Click);
            // 
            // GubbinRemapLabel1
            // 
            this.GubbinRemapLabel1.AutoSize = true;
            this.GubbinRemapLabel1.Location = new System.Drawing.Point(12, 9);
            this.GubbinRemapLabel1.Name = "GubbinRemapLabel1";
            this.GubbinRemapLabel1.Size = new System.Drawing.Size(53, 13);
            this.GubbinRemapLabel1.TabIndex = 7;
            this.GubbinRemapLabel1.Text = "Gubbin 1:";
            // 
            // GubbinRemapLabel6
            // 
            this.GubbinRemapLabel6.AutoSize = true;
            this.GubbinRemapLabel6.Location = new System.Drawing.Point(12, 139);
            this.GubbinRemapLabel6.Name = "GubbinRemapLabel6";
            this.GubbinRemapLabel6.Size = new System.Drawing.Size(53, 13);
            this.GubbinRemapLabel6.TabIndex = 12;
            this.GubbinRemapLabel6.Text = "Gubbin 6:";
            // 
            // GubbinRemapLabel4
            // 
            this.GubbinRemapLabel4.AutoSize = true;
            this.GubbinRemapLabel4.Location = new System.Drawing.Point(12, 87);
            this.GubbinRemapLabel4.Name = "GubbinRemapLabel4";
            this.GubbinRemapLabel4.Size = new System.Drawing.Size(53, 13);
            this.GubbinRemapLabel4.TabIndex = 10;
            this.GubbinRemapLabel4.Text = "Gubbin 4:";
            // 
            // GubbinRemapLabel5
            // 
            this.GubbinRemapLabel5.AutoSize = true;
            this.GubbinRemapLabel5.Location = new System.Drawing.Point(12, 113);
            this.GubbinRemapLabel5.Name = "GubbinRemapLabel5";
            this.GubbinRemapLabel5.Size = new System.Drawing.Size(53, 13);
            this.GubbinRemapLabel5.TabIndex = 11;
            this.GubbinRemapLabel5.Text = "Gubbin 5:";
            // 
            // GubbinRemapLabel3
            // 
            this.GubbinRemapLabel3.AutoSize = true;
            this.GubbinRemapLabel3.Location = new System.Drawing.Point(12, 61);
            this.GubbinRemapLabel3.Name = "GubbinRemapLabel3";
            this.GubbinRemapLabel3.Size = new System.Drawing.Size(53, 13);
            this.GubbinRemapLabel3.TabIndex = 9;
            this.GubbinRemapLabel3.Text = "Gubbin 3:";
            // 
            // GubbinRemapLabel2
            // 
            this.GubbinRemapLabel2.AutoSize = true;
            this.GubbinRemapLabel2.Location = new System.Drawing.Point(12, 35);
            this.GubbinRemapLabel2.Name = "GubbinRemapLabel2";
            this.GubbinRemapLabel2.Size = new System.Drawing.Size(53, 13);
            this.GubbinRemapLabel2.TabIndex = 8;
            this.GubbinRemapLabel2.Text = "Gubbin 2:";
            // 
            // GubbinRemapText
            // 
            for (int i = 0; i < 6; ++i)
            {
                this.GubbinRemapText[i].Location = new System.Drawing.Point(71, 6 + (i * 26));
                this.GubbinRemapText[i].Name = "GubbinRemapText";
                this.GubbinRemapText[i].Size = new System.Drawing.Size(171, 20);
                this.GubbinRemapText[i].TabIndex = i;
            }
            // 
            // Gubbin_Remapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 195);
            for (int i = 0; i < 6; ++i)
                this.Controls.Add(this.GubbinRemapText[i]);
            this.Controls.Add(this.GubbinRemapLabel2);
            this.Controls.Add(this.GubbinRemapLabel3);
            this.Controls.Add(this.GubbinRemapLabel5);
            this.Controls.Add(this.GubbinRemapLabel4);
            this.Controls.Add(this.GubbinRemapLabel6);
            this.Controls.Add(this.GubbinRemapLabel1);
            this.Controls.Add(this.GubbinRemapAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Gubbin_Remapping";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gubbin Remapping";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GubbinRemapAccept;
        private System.Windows.Forms.Label GubbinRemapLabel1;
        private System.Windows.Forms.Label GubbinRemapLabel6;
        private System.Windows.Forms.Label GubbinRemapLabel4;
        private System.Windows.Forms.Label GubbinRemapLabel5;
        private System.Windows.Forms.Label GubbinRemapLabel3;
        private System.Windows.Forms.Label GubbinRemapLabel2;
        public System.Windows.Forms.TextBox[] GubbinRemapText;
    }
}