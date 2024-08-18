namespace RealmCrafter_GE
{
    partial class TreeCollectionEditorList
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
            this.FullTreeList = new System.Windows.Forms.ListBox();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FullTreeList
            // 
            this.FullTreeList.FormattingEnabled = true;
            this.FullTreeList.Location = new System.Drawing.Point(12, 12);
            this.FullTreeList.Name = "FullTreeList";
            this.FullTreeList.Size = new System.Drawing.Size(202, 212);
            this.FullTreeList.TabIndex = 1;
            this.FullTreeList.DoubleClick += new System.EventHandler(this.FullTreeList_DoubleClick);
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(139, 230);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 2;
            this.ButtonOK.Text = "Done";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // TreeCollectionEditorList
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 265);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.FullTreeList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TreeCollectionEditorList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Trees";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonOK;
        public System.Windows.Forms.ListBox FullTreeList;
    }
}