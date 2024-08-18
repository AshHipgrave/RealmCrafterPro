namespace RealmCrafter_GE
{
    public partial class WorldZoneList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorldZoneList));
            this.panel1 = new System.Windows.Forms.Panel();
            this.WorldZonesTree = new System.Windows.Forms.TreeView();
            this.ObjectImageList = new System.Windows.Forms.ImageList(this.components);
            this.ObjectMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goToObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.ObjectMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.WorldZonesTree);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 410);
            this.panel1.TabIndex = 0;
            // 
            // WorldZonesTree
            // 
            this.WorldZonesTree.CheckBoxes = true;
            this.WorldZonesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorldZonesTree.FullRowSelect = true;
            this.WorldZonesTree.HotTracking = true;
            this.WorldZonesTree.Location = new System.Drawing.Point(0, 0);
            this.WorldZonesTree.Name = "WorldZonesTree";
            this.WorldZonesTree.Size = new System.Drawing.Size(229, 410);
            this.WorldZonesTree.TabIndex = 0;
            this.WorldZonesTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.WorldZonesTree_AfterCheck);
            this.WorldZonesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.WorldZonesTree_AfterSelect);
            this.WorldZonesTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.WorldZonesTree_MouseClick);
            // 
            // ObjectImageList
            // 
            this.ObjectImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ObjectImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.ObjectImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ObjectMenuStrip
            // 
            this.ObjectMenuStrip.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToObjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectObjectToolStripMenuItem,
            this.duplicateObjectToolStripMenuItem,
            this.deleteObjectToolStripMenuItem});
            this.ObjectMenuStrip.Name = "contextMenuStrip1";
            this.ObjectMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ObjectMenuStrip.ShowImageMargin = false;
            this.ObjectMenuStrip.Size = new System.Drawing.Size(140, 98);
            // 
            // goToObjectToolStripMenuItem
            // 
            this.goToObjectToolStripMenuItem.Name = "goToObjectToolStripMenuItem";
            this.goToObjectToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.goToObjectToolStripMenuItem.Text = "Go To Object";
            this.goToObjectToolStripMenuItem.Click += new System.EventHandler(this.goToObjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(136, 6);
            // 
            // selectObjectToolStripMenuItem
            // 
            this.selectObjectToolStripMenuItem.Name = "selectObjectToolStripMenuItem";
            this.selectObjectToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.selectObjectToolStripMenuItem.Text = "Select Object";
            this.selectObjectToolStripMenuItem.Click += new System.EventHandler(this.selectObjectToolStripMenuItem_Click);
            // 
            // duplicateObjectToolStripMenuItem
            // 
            this.duplicateObjectToolStripMenuItem.Name = "duplicateObjectToolStripMenuItem";
            this.duplicateObjectToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.duplicateObjectToolStripMenuItem.Text = "Duplicate Object";
            this.duplicateObjectToolStripMenuItem.Click += new System.EventHandler(this.duplicateObjectToolStripMenuItem_Click);
            // 
            // deleteObjectToolStripMenuItem
            // 
            this.deleteObjectToolStripMenuItem.Name = "deleteObjectToolStripMenuItem";
            this.deleteObjectToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.deleteObjectToolStripMenuItem.Text = "Delete Object";
            this.deleteObjectToolStripMenuItem.Click += new System.EventHandler(this.deleteObjectToolStripMenuItem_Click);
            // 
            // WorldZoneList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 410);
            this.Controls.Add(this.panel1);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WorldZoneList";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.TabText = "Objects";
            this.Text = "Objects";
            this.Load += new System.EventHandler(this.WorldZoneList_Load);
            this.panel1.ResumeLayout(false);
            this.ObjectMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.TreeView WorldZonesTree;
        private System.Windows.Forms.ImageList ObjectImageList;
        private System.Windows.Forms.ContextMenuStrip ObjectMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem goToObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}