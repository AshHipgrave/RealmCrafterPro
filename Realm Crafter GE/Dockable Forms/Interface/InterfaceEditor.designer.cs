namespace RealmCrafter_GE.Dockable_Forms
{
    partial class InterfaceEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InterfaceEditor));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.InterfacePanelRender = new System.Windows.Forms.Panel();
            this.InterfaceDock = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.InterfacePanelRender);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.InterfaceDock);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(805, 636);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(805, 661);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // InterfacePanelRender
            // 
            this.InterfacePanelRender.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InterfacePanelRender.Location = new System.Drawing.Point(0, 0);
            this.InterfacePanelRender.Margin = new System.Windows.Forms.Padding(2);
            this.InterfacePanelRender.MaximumSize = new System.Drawing.Size(805, 636);
            this.InterfacePanelRender.MinimumSize = new System.Drawing.Size(805, 636);
            this.InterfacePanelRender.Name = "InterfacePanelRender";
            this.InterfacePanelRender.Size = new System.Drawing.Size(805, 636);
            this.InterfacePanelRender.TabIndex = 1;
            // 
            // InterfaceDock
            // 
            this.InterfaceDock.ActiveAutoHideContent = null;
            this.InterfaceDock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.InterfaceDock.DockBottomPortion = 0.2D;
            this.InterfaceDock.DockLeftPortion = 0.2D;
            this.InterfaceDock.DockRightPortion = 0.2D;
            this.InterfaceDock.DockTopPortion = 0.2D;
            this.InterfaceDock.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.InterfaceDock.Location = new System.Drawing.Point(0, 0);
            this.InterfaceDock.Name = "InterfaceDock";
            this.InterfaceDock.Size = new System.Drawing.Size(805, 625);
            this.InterfaceDock.TabIndex = 0;
            // 
            // InterfaceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 661);
            this.Controls.Add(this.toolStripContainer1);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InterfaceEditor";
            this.TabText = "Interface Editor";
            this.Text = "Interface Editor";
            this.Load += new System.EventHandler(this.InterfaceEditor_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        public WeifenLuo.WinFormsUI.Docking.DockPanel InterfaceDock;
        public System.Windows.Forms.Panel InterfacePanelRender;
    }
}