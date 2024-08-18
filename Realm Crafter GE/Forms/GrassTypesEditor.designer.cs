namespace RCTTest
{
    partial class GrassTypesEditor
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
            this.TypesList = new System.Windows.Forms.ListBox();
            this.CnlButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.TypesGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // TypesList
            // 
            this.TypesList.FormattingEnabled = true;
            this.TypesList.Location = new System.Drawing.Point(12, 12);
            this.TypesList.Name = "TypesList";
            this.TypesList.Size = new System.Drawing.Size(222, 199);
            this.TypesList.TabIndex = 0;
            this.TypesList.SelectedIndexChanged += new System.EventHandler(this.TypesList_SelectedIndexChanged);
            // 
            // CnlButton
            // 
            this.CnlButton.Location = new System.Drawing.Point(387, 246);
            this.CnlButton.Name = "CnlButton";
            this.CnlButton.Size = new System.Drawing.Size(75, 23);
            this.CnlButton.TabIndex = 1;
            this.CnlButton.Text = "Cancel";
            this.CnlButton.UseVisualStyleBackColor = true;
            this.CnlButton.Click += new System.EventHandler(this.CnlButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(306, 246);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 2;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(78, 217);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 3;
            this.AddButton.Text = "Add New";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(159, 217);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteButton.TabIndex = 4;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // TypesGrid
            // 
            this.TypesGrid.Location = new System.Drawing.Point(240, 12);
            this.TypesGrid.Name = "TypesGrid";
            this.TypesGrid.Size = new System.Drawing.Size(222, 228);
            this.TypesGrid.TabIndex = 5;
            // 
            // GrassTypesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 279);
            this.ControlBox = false;
            this.Controls.Add(this.TypesGrid);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.CnlButton);
            this.Controls.Add(this.TypesList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GrassTypesEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Edit Grass Types...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox TypesList;
        private System.Windows.Forms.Button CnlButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.PropertyGrid TypesGrid;
    }
}