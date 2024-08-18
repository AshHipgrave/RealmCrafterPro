namespace RealmCrafter_GE
{
    public partial class InterfaceHierarchyWindows
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InterfaceHierarchyWindows));
            this.ControlsHierarchy = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // ControlsHierarchy
            // 
            this.ControlsHierarchy.CheckBoxes = true;
            this.ControlsHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ControlsHierarchy.Location = new System.Drawing.Point(0, 0);
            this.ControlsHierarchy.Name = "ControlsHierarchy";
            this.ControlsHierarchy.Size = new System.Drawing.Size(226, 487);
            this.ControlsHierarchy.TabIndex = 4;
            this.ControlsHierarchy.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ControlsHierarchy_AfterCheck);
            this.ControlsHierarchy.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ControlsHierarchy_AfterSelect);
            // 
            // InterfaceHierarchyWindows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 487);
            this.Controls.Add(this.ControlsHierarchy);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InterfaceHierarchyWindows";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.ShowInTaskbar = false;
            this.TabText = "Interface Hierarchy";
            this.Text = "Interface Hierarchy";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView ControlsHierarchy;

    }
}