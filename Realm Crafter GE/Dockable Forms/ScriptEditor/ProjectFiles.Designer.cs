namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    partial class ProjectFiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectFiles));
            this.ProjectTree = new System.Windows.Forms.TreeView();
            this.ProjectImages = new System.Windows.Forms.ImageList(this.components);
            this.ProjectToolStrip = new System.Windows.Forms.ToolStrip();
            this.ProjectRefreshScripts = new System.Windows.Forms.ToolStripButton();
            this.ProjectPanel = new System.Windows.Forms.Panel();
            this.ScriptContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDesignerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjectContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.scriptFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllOpenScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjectToolStrip.SuspendLayout();
            this.ProjectPanel.SuspendLayout();
            this.ScriptContextMenu.SuspendLayout();
            this.ProjectContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProjectTree
            // 
            this.ProjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectTree.ImageIndex = 1;
            this.ProjectTree.ImageList = this.ProjectImages;
            this.ProjectTree.LabelEdit = true;
            this.ProjectTree.Location = new System.Drawing.Point(0, 0);
            this.ProjectTree.Name = "ProjectTree";
            this.ProjectTree.SelectedImageIndex = 0;
            this.ProjectTree.Size = new System.Drawing.Size(226, 294);
            this.ProjectTree.TabIndex = 0;
            this.ProjectTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ProjectTree_AfterLabelEdit);
            this.ProjectTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProjectTree_KeyDown);
            this.ProjectTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ProjectTree_MouseClick);
            this.ProjectTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ProjectTree_MouseDoubleClick);
            // 
            // ProjectImages
            // 
            this.ProjectImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ProjectImages.ImageStream")));
            this.ProjectImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ProjectImages.Images.SetKeyName(0, "612-home.png");
            this.ProjectImages.Images.SetKeyName(1, "CSFile.png");
            this.ProjectImages.Images.SetKeyName(2, "CSForm.png");
            // 
            // ProjectToolStrip
            // 
            this.ProjectToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ProjectToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProjectRefreshScripts});
            this.ProjectToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ProjectToolStrip.Name = "ProjectToolStrip";
            this.ProjectToolStrip.Size = new System.Drawing.Size(226, 25);
            this.ProjectToolStrip.TabIndex = 1;
            this.ProjectToolStrip.Text = "ProjectToolStrip";
            // 
            // ProjectRefreshScripts
            // 
            this.ProjectRefreshScripts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ProjectRefreshScripts.Image = ((System.Drawing.Image)(resources.GetObject("ProjectRefreshScripts.Image")));
            this.ProjectRefreshScripts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ProjectRefreshScripts.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.ProjectRefreshScripts.Name = "ProjectRefreshScripts";
            this.ProjectRefreshScripts.Size = new System.Drawing.Size(23, 22);
            this.ProjectRefreshScripts.Text = "Refresh Project";
            this.ProjectRefreshScripts.Click += new System.EventHandler(this.ProjectRefreshScripts_Click);
            // 
            // ProjectPanel
            // 
            this.ProjectPanel.Controls.Add(this.ProjectTree);
            this.ProjectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectPanel.Location = new System.Drawing.Point(0, 25);
            this.ProjectPanel.Name = "ProjectPanel";
            this.ProjectPanel.Size = new System.Drawing.Size(226, 294);
            this.ProjectPanel.TabIndex = 2;
            // 
            // ScriptContextMenu
            // 
            this.ScriptContextMenu.BackColor = System.Drawing.Color.White;
            this.ScriptContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.viewDesignerToolStripMenuItem,
            this.toolStripSeparator1,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.ScriptContextMenu.Name = "ScriptContextMenu";
            this.ScriptContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ScriptContextMenu.Size = new System.Drawing.Size(149, 96);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.AutoSize = false;
            this.openToolStripMenuItem.Image = global::RealmCrafter_GE.Properties.Resources.ViewSource;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0);
            this.openToolStripMenuItem.Size = new System.Drawing.Size(151, 20);
            this.openToolStripMenuItem.Text = "View Source";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // viewDesignerToolStripMenuItem
            // 
            this.viewDesignerToolStripMenuItem.Image = global::RealmCrafter_GE.Properties.Resources.ViewDesigner;
            this.viewDesignerToolStripMenuItem.Name = "viewDesignerToolStripMenuItem";
            this.viewDesignerToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.viewDesignerToolStripMenuItem.Text = "View Designer";
            this.viewDesignerToolStripMenuItem.Click += new System.EventHandler(this.viewDesignerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.AutoSize = false;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.AutoSize = false;
            this.deleteToolStripMenuItem.Image = global::RealmCrafter_GE.Properties.Resources.deleteicon;
            this.deleteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // ProjectContextMenu
            // 
            this.ProjectContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveAllToolStripMenuItem,
            this.closeAllOpenScriptsToolStripMenuItem});
            this.ProjectContextMenu.Name = "ProjectContextMenu";
            this.ProjectContextMenu.Size = new System.Drawing.Size(186, 76);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newScriptToolStripMenuItem,
            this.toolStripSeparator3,
            this.scriptFromFileToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // newScriptToolStripMenuItem
            // 
            this.newScriptToolStripMenuItem.Name = "newScriptToolStripMenuItem";
            this.newScriptToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newScriptToolStripMenuItem.Text = "New Script";
            this.newScriptToolStripMenuItem.Click += new System.EventHandler(this.newScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // scriptFromFileToolStripMenuItem
            // 
            this.scriptFromFileToolStripMenuItem.Name = "scriptFromFileToolStripMenuItem";
            this.scriptFromFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.scriptFromFileToolStripMenuItem.Text = "Script from file";
            this.scriptFromFileToolStripMenuItem.Click += new System.EventHandler(this.scriptFromFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveAllToolStripMenuItem.Text = "Save all";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // closeAllOpenScriptsToolStripMenuItem
            // 
            this.closeAllOpenScriptsToolStripMenuItem.Name = "closeAllOpenScriptsToolStripMenuItem";
            this.closeAllOpenScriptsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.closeAllOpenScriptsToolStripMenuItem.Text = "Close all open scripts";
            this.closeAllOpenScriptsToolStripMenuItem.Click += new System.EventHandler(this.closeAllOpenScriptsToolStripMenuItem_Click);
            // 
            // ProjectFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 319);
            this.Controls.Add(this.ProjectPanel);
            this.Controls.Add(this.ProjectToolStrip);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProjectFiles";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.TabText = "Project Explorer";
            this.Text = "Project Explorer";
            this.ToolTipText = "Project Explorer";
            this.Load += new System.EventHandler(this.ProjectFiles_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProjectFiles_KeyDown);
            this.ProjectToolStrip.ResumeLayout(false);
            this.ProjectToolStrip.PerformLayout();
            this.ProjectPanel.ResumeLayout(false);
            this.ScriptContextMenu.ResumeLayout(false);
            this.ProjectContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView ProjectTree;
        private System.Windows.Forms.ImageList ProjectImages;
        private System.Windows.Forms.ToolStrip ProjectToolStrip;
        private System.Windows.Forms.ToolStripButton ProjectRefreshScripts;
        private System.Windows.Forms.Panel ProjectPanel;
        private System.Windows.Forms.ContextMenuStrip ScriptContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ProjectContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllOpenScriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem scriptFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem viewDesignerToolStripMenuItem;

    }
}