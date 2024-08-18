namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    partial class Toolbox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toolbox));
            this._ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PaintTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // PaintTimer
            // 
            this.PaintTimer.Enabled = true;
            this.PaintTimer.Tick += new System.EventHandler(this.PaintTimer_Tick);
            // 
            // Toolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(284, 523);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Toolbox";
            this.TabText = "Toolbox";
            this.Text = "Toolbox";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip _ToolTip;
        private System.Windows.Forms.Timer PaintTimer;



    }
}