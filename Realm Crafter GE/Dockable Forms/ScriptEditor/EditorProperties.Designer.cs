namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    partial class EditorProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorProperties));
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.ControlsList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.Location = new System.Drawing.Point(0, 21);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(284, 243);
            this.PropertyGrid.TabIndex = 0;
            // 
            // ControlsList
            // 
            this.ControlsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.ControlsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ControlsList.FormattingEnabled = true;
            this.ControlsList.Location = new System.Drawing.Point(0, 0);
            this.ControlsList.Name = "ControlsList";
            this.ControlsList.Size = new System.Drawing.Size(284, 21);
            this.ControlsList.TabIndex = 1;
            this.ControlsList.SelectedIndexChanged += new System.EventHandler(this.ControlsList_SelectedIndexChanged);
            // 
            // EditorProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.CloseButton = false;
            this.Controls.Add(this.PropertyGrid);
            this.Controls.Add(this.ControlsList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditorProperties";
            this.TabText = "Editor Properties";
            this.Text = "Editor Properties";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid PropertyGrid;
        public System.Windows.Forms.ComboBox ControlsList;

    }
}