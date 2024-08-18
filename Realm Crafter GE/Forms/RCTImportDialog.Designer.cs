namespace RCTTest
{
    partial class RCTImportDialog
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
            this.PluginGrid = new System.Windows.Forms.DataGridView();
            this.TypeNameHeading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeDescriptionHeading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PluginGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // PluginGrid
            // 
            this.PluginGrid.AllowUserToAddRows = false;
            this.PluginGrid.AllowUserToDeleteRows = false;
            this.PluginGrid.AllowUserToResizeRows = false;
            this.PluginGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.PluginGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PluginGrid.ColumnHeadersVisible = false;
            this.PluginGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TypeNameHeading,
            this.TypeDescriptionHeading});
            this.PluginGrid.Location = new System.Drawing.Point(12, 12);
            this.PluginGrid.MultiSelect = false;
            this.PluginGrid.Name = "PluginGrid";
            this.PluginGrid.ReadOnly = true;
            this.PluginGrid.RowHeadersVisible = false;
            this.PluginGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.PluginGrid.ShowCellErrors = false;
            this.PluginGrid.ShowCellToolTips = false;
            this.PluginGrid.ShowEditingIcon = false;
            this.PluginGrid.ShowRowErrors = false;
            this.PluginGrid.Size = new System.Drawing.Size(431, 178);
            this.PluginGrid.TabIndex = 4;
            this.PluginGrid.SelectionChanged += new System.EventHandler(this.PluginGrid_SelectionChanged);
            // 
            // TypeNameHeading
            // 
            this.TypeNameHeading.HeaderText = "Type";
            this.TypeNameHeading.Name = "TypeNameHeading";
            this.TypeNameHeading.ReadOnly = true;
            this.TypeNameHeading.Width = 200;
            // 
            // TypeDescriptionHeading
            // 
            this.TypeDescriptionHeading.HeaderText = "Description";
            this.TypeDescriptionHeading.Name = "TypeDescriptionHeading";
            this.TypeDescriptionHeading.ReadOnly = true;
            this.TypeDescriptionHeading.Width = 1000;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(368, 196);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 9;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(287, 196);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 8;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // RCTImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 223);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.PluginGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RCTImportDialog";
            this.ShowIcon = false;
            this.Text = "Choose Import Plugin...";
            this.Load += new System.EventHandler(this.RCTImportDialog_Load);
            this.VisibleChanged += new System.EventHandler(this.RCTImportDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.PluginGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView PluginGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeNameHeading;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeDescriptionHeading;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
    }
}