namespace ScriptErrorLog
{
    partial class MainWin
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ExceptionStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ScriptText = new System.Windows.Forms.RichTextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExceptionStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 597);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(680, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ExceptionStatus
            // 
            this.ExceptionStatus.Name = "ExceptionStatus";
            this.ExceptionStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // ScriptText
            // 
            this.ScriptText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScriptText.Location = new System.Drawing.Point(0, 0);
            this.ScriptText.Multiline = true;
            this.ScriptText.Name = "ScriptText";
            this.ScriptText.Size = new System.Drawing.Size(680, 597);
            this.ScriptText.TabIndex = 1;
            // 
            // MainWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 619);
            this.Controls.Add(this.ScriptText);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainWin";
            this.Text = "Script Exception";
            this.Load += new System.EventHandler(this.MainWin_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ExceptionStatus;
        private System.Windows.Forms.RichTextBox ScriptText;
    }
}

