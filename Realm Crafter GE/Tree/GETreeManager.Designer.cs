namespace RealmCrafter_GE
{
    partial class GETreeManager
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
            this.TreesList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // TreesList
            // 
            this.TreesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreesList.Location = new System.Drawing.Point(0, 0);
            this.TreesList.Name = "TreesList";
            this.TreesList.Size = new System.Drawing.Size(284, 264);
            this.TreesList.TabIndex = 0;
            this.TreesList.DoubleClick += new System.EventHandler(this.TreesList_DoubleClick);
            // 
            // GETreeManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.TreesList);
            this.Name = "GETreeManager";
            this.Text = "GETreeManager";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView TreesList;



    }
}