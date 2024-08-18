namespace RealmCrafter
{
    partial class LightFunctionListEditorForm
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
            this.FunctionsList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // FunctionsList
            // 
            this.FunctionsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FunctionsList.FormattingEnabled = true;
            this.FunctionsList.Location = new System.Drawing.Point(0, 0);
            this.FunctionsList.Name = "FunctionsList";
            this.FunctionsList.Size = new System.Drawing.Size(220, 199);
            this.FunctionsList.TabIndex = 0;
            this.FunctionsList.DoubleClick += new System.EventHandler(this.FunctionsList_DoubleClick);
            // 
            // LightFunctionListEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 201);
            this.Controls.Add(this.FunctionsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LightFunctionListEditorForm";
            this.Text = "LightFunctionListEditorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox FunctionsList;
    }
}