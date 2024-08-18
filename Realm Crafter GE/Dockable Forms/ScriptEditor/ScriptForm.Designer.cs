namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    partial class ScriptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptForm));
            this.scriptText = new ScintillaNet.Scintilla();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.TabMenuContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllButThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openScriptsFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.scriptText)).BeginInit();
            this.panel1.SuspendLayout();
            this.TabMenuContextStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScriptText
            // 
            this.scriptText.AllowDrop = true;
            this.scriptText.AutoComplete.IsCaseSensitive = false;
            this.scriptText.AutoComplete.ListString = "";
            this.scriptText.AutoComplete.SingleLineAccept = true;
            this.scriptText.Caret.Width = 2;
            this.scriptText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptText.Folding.Flags = ScintillaNet.FoldFlag.LineAfterContracted;
            this.scriptText.IsBraceMatching = true;
            this.scriptText.Location = new System.Drawing.Point(0, 0);
            this.scriptText.Margin = new System.Windows.Forms.Padding(0);
            this.scriptText.Margins.Left = 0;
            this.scriptText.Margins.Margin0.IsClickable = true;
            this.scriptText.Margins.Margin0.Width = 40;
            this.scriptText.Margins.Margin1.Width = 12;
            this.scriptText.Name = "ScriptText";
            this.scriptText.Size = new System.Drawing.Size(662, 570);
            this.scriptText.TabIndex = 0;
            this.scriptText.TextDeleted += new System.EventHandler<ScintillaNet.TextModifiedEventArgs>(this.ScriptText_TextDeleted);
            this.scriptText.SelectionChanged += new System.EventHandler(this.ScriptText_SelectionChanged);
            this.scriptText.TextInserted += new System.EventHandler<ScintillaNet.TextModifiedEventArgs>(this.ScriptText_TextInserted);
            this.scriptText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ScriptText_KeyUp);
            this.scriptText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScriptText_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.scriptText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(662, 570);
            this.panel1.TabIndex = 2;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "624-business_object.png");
            // 
            // TabMenuContextStrip
            // 
            this.TabMenuContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem,
            this.closeAllButThisToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.openScriptsFolderToolStripMenuItem});
            this.TabMenuContextStrip.Name = "TabMenuContextStrip";
            this.TabMenuContextStrip.Size = new System.Drawing.Size(177, 148);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::RealmCrafter_GE.Properties.Resources._039_save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Image = global::RealmCrafter_GE.Properties.Resources.Saveall;
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveAllToolStripMenuItem.Text = "Save All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeAllButThisToolStripMenuItem
            // 
            this.closeAllButThisToolStripMenuItem.Name = "closeAllButThisToolStripMenuItem";
            this.closeAllButThisToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.closeAllButThisToolStripMenuItem.Text = "Close All But This";
            this.closeAllButThisToolStripMenuItem.Click += new System.EventHandler(this.closeAllButThisToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.closeAllToolStripMenuItem.Text = "Close All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // openScriptsFolderToolStripMenuItem
            // 
            this.openScriptsFolderToolStripMenuItem.Name = "openScriptsFolderToolStripMenuItem";
            this.openScriptsFolderToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.openScriptsFolderToolStripMenuItem.Text = "Open scripts folder";
            this.openScriptsFolderToolStripMenuItem.Click += new System.EventHandler(this.openScriptsFolderToolStripMenuItem_Click);
            // 
            // ScriptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 570);
            this.Controls.Add(this.panel1);
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ScriptForm";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.TabPageContextMenuStrip = this.TabMenuContextStrip;
            this.TabText = "ScriptForm";
            this.Text = "ScriptForm";
            this.Load += new System.EventHandler(this.ScriptForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.scriptText)).EndInit();
            this.panel1.ResumeLayout(false);
            this.TabMenuContextStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public ScintillaNet.Scintilla scriptText;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip TabMenuContextStrip;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllButThisToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem openScriptsFolderToolStripMenuItem;

    }
}