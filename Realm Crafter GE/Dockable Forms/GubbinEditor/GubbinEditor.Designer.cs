namespace RealmCrafter_GE.Dockable_Forms.GubbinEditor
{
    partial class GubbinEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ActorsRemoveButton = new System.Windows.Forms.Button();
            this.ActorsAddButton = new System.Windows.Forms.Button();
            this.TemplateActorsList = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TemplateRemoveButton = new System.Windows.Forms.Button();
            this.TemplateEditButton = new System.Windows.Forms.Button();
            this.TemplateNewButton = new System.Windows.Forms.Button();
            this.TemplatesList = new System.Windows.Forms.ComboBox();
            this.RenderPanel = new System.Windows.Forms.Panel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.RenderPanel);
            this.splitContainer1.Size = new System.Drawing.Size(823, 664);
            this.splitContainer1.SplitterDistance = 223;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ActorsRemoveButton);
            this.groupBox2.Controls.Add(this.ActorsAddButton);
            this.groupBox2.Controls.Add(this.TemplateActorsList);
            this.groupBox2.Location = new System.Drawing.Point(12, 101);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(206, 252);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Template Actors:";
            // 
            // ActorsRemoveButton
            // 
            this.ActorsRemoveButton.Location = new System.Drawing.Point(108, 224);
            this.ActorsRemoveButton.Name = "ActorsRemoveButton";
            this.ActorsRemoveButton.Size = new System.Drawing.Size(90, 23);
            this.ActorsRemoveButton.TabIndex = 2;
            this.ActorsRemoveButton.Text = "Remove";
            this.ActorsRemoveButton.UseVisualStyleBackColor = true;
            this.ActorsRemoveButton.Click += new System.EventHandler(this.ActorsRemoveButton_Click);
            // 
            // ActorsAddButton
            // 
            this.ActorsAddButton.Location = new System.Drawing.Point(6, 224);
            this.ActorsAddButton.Name = "ActorsAddButton";
            this.ActorsAddButton.Size = new System.Drawing.Size(90, 23);
            this.ActorsAddButton.TabIndex = 1;
            this.ActorsAddButton.Text = "Add";
            this.ActorsAddButton.UseVisualStyleBackColor = true;
            this.ActorsAddButton.Click += new System.EventHandler(this.ActorsAddButton_Click);
            // 
            // TemplateActorsList
            // 
            this.TemplateActorsList.FormattingEnabled = true;
            this.TemplateActorsList.Location = new System.Drawing.Point(6, 19);
            this.TemplateActorsList.Name = "TemplateActorsList";
            this.TemplateActorsList.Size = new System.Drawing.Size(192, 199);
            this.TemplateActorsList.TabIndex = 0;
            this.TemplateActorsList.SelectedIndexChanged += new System.EventHandler(this.TemplateActorsList_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TemplateRemoveButton);
            this.groupBox1.Controls.Add(this.TemplateEditButton);
            this.groupBox1.Controls.Add(this.TemplateNewButton);
            this.groupBox1.Controls.Add(this.TemplatesList);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 83);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gubbin Templates:";
            // 
            // TemplateRemoveButton
            // 
            this.TemplateRemoveButton.Location = new System.Drawing.Point(138, 46);
            this.TemplateRemoveButton.Name = "TemplateRemoveButton";
            this.TemplateRemoveButton.Size = new System.Drawing.Size(60, 23);
            this.TemplateRemoveButton.TabIndex = 3;
            this.TemplateRemoveButton.Text = "Remove";
            this.TemplateRemoveButton.UseVisualStyleBackColor = true;
            this.TemplateRemoveButton.Click += new System.EventHandler(this.TemplateRemoveButton_Click);
            // 
            // TemplateEditButton
            // 
            this.TemplateEditButton.Location = new System.Drawing.Point(72, 46);
            this.TemplateEditButton.Name = "TemplateEditButton";
            this.TemplateEditButton.Size = new System.Drawing.Size(60, 23);
            this.TemplateEditButton.TabIndex = 2;
            this.TemplateEditButton.Text = "Edit";
            this.TemplateEditButton.UseVisualStyleBackColor = true;
            this.TemplateEditButton.Click += new System.EventHandler(this.TemplateEditButton_Click);
            // 
            // TemplateNewButton
            // 
            this.TemplateNewButton.Location = new System.Drawing.Point(6, 46);
            this.TemplateNewButton.Name = "TemplateNewButton";
            this.TemplateNewButton.Size = new System.Drawing.Size(60, 23);
            this.TemplateNewButton.TabIndex = 1;
            this.TemplateNewButton.Text = "New";
            this.TemplateNewButton.UseVisualStyleBackColor = true;
            this.TemplateNewButton.Click += new System.EventHandler(this.TemplateNewButton_Click);
            // 
            // TemplatesList
            // 
            this.TemplatesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TemplatesList.FormattingEnabled = true;
            this.TemplatesList.Location = new System.Drawing.Point(6, 19);
            this.TemplatesList.Name = "TemplatesList";
            this.TemplatesList.Size = new System.Drawing.Size(192, 21);
            this.TemplatesList.TabIndex = 0;
            this.TemplatesList.SelectedIndexChanged += new System.EventHandler(this.TemplatesList_SelectedIndexChanged);
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(596, 664);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.RenderPanel_PreviewKeyDown);
            this.RenderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderPanel_MouseMove);
            this.RenderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderPanel_MouseDown);
            this.RenderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderPanel_MouseUp);
            this.RenderPanel.SizeChanged += new System.EventHandler(this.RenderPanel_SizeChanged);
            // 
            // GubbinEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 664);
            this.Controls.Add(this.splitContainer1);
            this.HideOnClose = true;
            this.Name = "GubbinEditor";
            this.TabText = "Gubbin Editor";
            this.Text = "Gubbin Editor";
            this.Load += new System.EventHandler(this.GubbinEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button TemplateRemoveButton;
        private System.Windows.Forms.Button TemplateEditButton;
        private System.Windows.Forms.Button TemplateNewButton;
        private System.Windows.Forms.Button ActorsAddButton;
        private System.Windows.Forms.Button ActorsRemoveButton;
        public System.Windows.Forms.ComboBox TemplatesList;
        public System.Windows.Forms.ListBox TemplateActorsList;
        private System.Windows.Forms.Panel RenderPanel;
        public System.Windows.Forms.SplitContainer splitContainer1;
    }
}