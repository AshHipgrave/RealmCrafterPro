namespace RealmCrafter
{
    partial class LightFunctionDialog
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
            this.EventsList = new System.Windows.Forms.DataGridView();
            this.TimeCollumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InterpolateCollumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColorCollumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RadiusCollumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventProperties = new System.Windows.Forms.PropertyGrid();
            this.CnlButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.EventsList)).BeginInit();
            this.SuspendLayout();
            // 
            // EventsList
            // 
            this.EventsList.AllowUserToAddRows = false;
            this.EventsList.AllowUserToResizeColumns = false;
            this.EventsList.AllowUserToResizeRows = false;
            this.EventsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EventsList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TimeCollumn,
            this.InterpolateCollumn,
            this.ColorCollumn,
            this.RadiusCollumn});
            this.EventsList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.EventsList.Location = new System.Drawing.Point(236, 12);
            this.EventsList.MultiSelect = false;
            this.EventsList.Name = "EventsList";
            this.EventsList.RowHeadersVisible = false;
            this.EventsList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.EventsList.ShowCellErrors = false;
            this.EventsList.Size = new System.Drawing.Size(266, 232);
            this.EventsList.TabIndex = 0;
            this.EventsList.SelectionChanged += new System.EventHandler(this.EventsList_SelectionChanged);
            // 
            // TimeCollumn
            // 
            this.TimeCollumn.HeaderText = "Time";
            this.TimeCollumn.Name = "TimeCollumn";
            this.TimeCollumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TimeCollumn.Width = 50;
            // 
            // InterpolateCollumn
            // 
            this.InterpolateCollumn.HeaderText = "Interpolate";
            this.InterpolateCollumn.Name = "InterpolateCollumn";
            this.InterpolateCollumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.InterpolateCollumn.Width = 70;
            // 
            // ColorCollumn
            // 
            this.ColorCollumn.HeaderText = "Color";
            this.ColorCollumn.Name = "ColorCollumn";
            this.ColorCollumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColorCollumn.Width = 50;
            // 
            // RadiusCollumn
            // 
            this.RadiusCollumn.HeaderText = "Radius";
            this.RadiusCollumn.Name = "RadiusCollumn";
            this.RadiusCollumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.RadiusCollumn.Width = 60;
            // 
            // EventProperties
            // 
            this.EventProperties.Location = new System.Drawing.Point(12, 12);
            this.EventProperties.Name = "EventProperties";
            this.EventProperties.Size = new System.Drawing.Size(218, 232);
            this.EventProperties.TabIndex = 1;
            // 
            // CnlButton
            // 
            this.CnlButton.Location = new System.Drawing.Point(427, 250);
            this.CnlButton.Name = "CnlButton";
            this.CnlButton.Size = new System.Drawing.Size(75, 23);
            this.CnlButton.TabIndex = 2;
            this.CnlButton.Text = "Cancel";
            this.CnlButton.UseVisualStyleBackColor = true;
            this.CnlButton.Click += new System.EventHandler(this.CnlButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(346, 250);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 3;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(174, 250);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 4;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Location = new System.Drawing.Point(255, 250);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(75, 23);
            this.RemoveButton.TabIndex = 5;
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // LightFunctionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 278);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CnlButton);
            this.Controls.Add(this.EventProperties);
            this.Controls.Add(this.EventsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LightFunctionDialog";
            this.Text = "Edit Light Function";
            this.Load += new System.EventHandler(this.LightFunctionDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EventsList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView EventsList;
        private System.Windows.Forms.PropertyGrid EventProperties;
        private System.Windows.Forms.Button CnlButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeCollumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn InterpolateCollumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColorCollumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RadiusCollumn;
        private System.Windows.Forms.Button RemoveButton;

    }
}

