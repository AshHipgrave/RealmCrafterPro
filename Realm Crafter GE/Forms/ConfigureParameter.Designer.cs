namespace RealmCrafter_GE
{
    partial class ConfigureParameter
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
            this.SaveParams = new System.Windows.Forms.Button();
            this.CancelParams = new System.Windows.Forms.Button();
            this.ParameterGrid = new System.Windows.Forms.PropertyGrid();
            this.ApplyParams = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SaveParams
            // 
            this.SaveParams.Location = new System.Drawing.Point(57, 281);
            this.SaveParams.Name = "SaveParams";
            this.SaveParams.Size = new System.Drawing.Size(75, 23);
            this.SaveParams.TabIndex = 0;
            this.SaveParams.Text = "OK";
            this.SaveParams.UseVisualStyleBackColor = true;
            this.SaveParams.Click += new System.EventHandler(this.SaveParams_Click);
            // 
            // CancelParams
            // 
            this.CancelParams.Location = new System.Drawing.Point(138, 281);
            this.CancelParams.Name = "CancelParams";
            this.CancelParams.Size = new System.Drawing.Size(75, 23);
            this.CancelParams.TabIndex = 1;
            this.CancelParams.Text = "Cancel";
            this.CancelParams.UseVisualStyleBackColor = true;
            this.CancelParams.Click += new System.EventHandler(this.CancelParams_Click);
            // 
            // ParameterGrid
            // 
            this.ParameterGrid.Location = new System.Drawing.Point(12, 12);
            this.ParameterGrid.Name = "ParameterGrid";
            this.ParameterGrid.Size = new System.Drawing.Size(282, 263);
            this.ParameterGrid.TabIndex = 2;
            this.ParameterGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ParameterGrid_PropertyValueChanged);
            // 
            // ApplyParams
            // 
            this.ApplyParams.Location = new System.Drawing.Point(219, 281);
            this.ApplyParams.Name = "ApplyParams";
            this.ApplyParams.Size = new System.Drawing.Size(75, 23);
            this.ApplyParams.TabIndex = 3;
            this.ApplyParams.Text = "Apply";
            this.ApplyParams.UseVisualStyleBackColor = true;
            this.ApplyParams.Click += new System.EventHandler(this.ApplyParams_Click);
            // 
            // ConfigureParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 316);
            this.Controls.Add(this.ApplyParams);
            this.Controls.Add(this.CancelParams);
            this.Controls.Add(this.SaveParams);
            this.Controls.Add(this.ParameterGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureParameter";
            this.Text = "Configure Parameters";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SaveParams;
        private System.Windows.Forms.Button CancelParams;
        private System.Windows.Forms.PropertyGrid ParameterGrid;
        private System.Windows.Forms.Button ApplyParams;
    }
}