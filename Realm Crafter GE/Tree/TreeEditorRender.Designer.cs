namespace RealmCrafter_GE
{
    partial class TreeEditorRender
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
            this.RenderPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.RenderPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(284, 264);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.RenderPanel_PreviewKeyDown);
            this.RenderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderPanel_MouseMove);
            this.RenderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderPanel_MouseDown);
            this.RenderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderPanel_MouseUp);
            this.RenderPanel.SizeChanged += new System.EventHandler(this.RenderPanel_SizeChanged);
            // 
            // TreeEditorRender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.RenderPanel);
            this.Name = "TreeEditorRender";
            this.TabText = "TreeEditorRender";
            this.Text = "TreeEditorRender";
            this.Load += new System.EventHandler(this.TreeEditorRender_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeEditorRender_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel RenderPanel;

    }
}