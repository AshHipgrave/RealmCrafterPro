namespace RealmCrafter_GE.PostProcess
{
    public partial class PostProcessWindows
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
            this.chbxEnable = new System.Windows.Forms.CheckBox();
            this.gbUserPP = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btRename = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btNew = new System.Windows.Forms.Button();
            this.cbUserPP = new System.Windows.Forms.ComboBox();
            this.gbPrePP = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btApply = new System.Windows.Forms.Button();
            this.cbPrePP = new System.Windows.Forms.ComboBox();
            this.gbAvaibleEffects = new System.Windows.Forms.GroupBox();
            this.lbxEffects = new System.Windows.Forms.ListBox();
            this.gbActiveEffects = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btEffectDelete = new System.Windows.Forms.Button();
            this.btMoveDown = new System.Windows.Forms.Button();
            this.btMoveUp = new System.Windows.Forms.Button();
            this.lbxActiveEffects = new System.Windows.Forms.ListBox();
            this.gbParam = new System.Windows.Forms.Panel();
            this.gbUserPP.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbPrePP.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.gbAvaibleEffects.SuspendLayout();
            this.gbActiveEffects.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbxEnable
            // 
            this.chbxEnable.AutoSize = true;
            this.chbxEnable.Checked = true;
            this.chbxEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxEnable.Dock = System.Windows.Forms.DockStyle.Top;
            this.chbxEnable.Location = new System.Drawing.Point(0, 0);
            this.chbxEnable.Name = "chbxEnable";
            this.chbxEnable.Size = new System.Drawing.Size(332, 21);
            this.chbxEnable.TabIndex = 6;
            this.chbxEnable.Text = "Enable post-processing.";
            this.chbxEnable.UseVisualStyleBackColor = true;
            this.chbxEnable.CheckedChanged += new System.EventHandler(this.chbxEnable_CheckedChanged);
            // 
            // gbUserPP
            // 
            this.gbUserPP.Controls.Add(this.tableLayoutPanel1);
            this.gbUserPP.Controls.Add(this.cbUserPP);
            this.gbUserPP.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbUserPP.Location = new System.Drawing.Point(0, 21);
            this.gbUserPP.Name = "gbUserPP";
            this.gbUserPP.Size = new System.Drawing.Size(332, 77);
            this.gbUserPP.TabIndex = 8;
            this.gbUserPP.TabStop = false;
            this.gbUserPP.Text = "User defined post-process:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Controls.Add(this.btRename, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btDelete, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btNew, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 42);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(326, 32);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // btRename
            // 
            this.btRename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btRename.Location = new System.Drawing.Point(197, 3);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(126, 26);
            this.btRename.TabIndex = 9;
            this.btRename.Text = "Rename";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // btDelete
            // 
            this.btDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btDelete.Location = new System.Drawing.Point(100, 3);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(91, 26);
            this.btDelete.TabIndex = 8;
            this.btDelete.Text = "Delete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btNew
            // 
            this.btNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btNew.Location = new System.Drawing.Point(3, 3);
            this.btNew.Name = "btNew";
            this.btNew.Size = new System.Drawing.Size(91, 26);
            this.btNew.TabIndex = 7;
            this.btNew.Text = "New";
            this.btNew.UseVisualStyleBackColor = true;
            this.btNew.Click += new System.EventHandler(this.btNew_Click);
            // 
            // cbUserPP
            // 
            this.cbUserPP.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbUserPP.FormattingEnabled = true;
            this.cbUserPP.Location = new System.Drawing.Point(3, 18);
            this.cbUserPP.Name = "cbUserPP";
            this.cbUserPP.Size = new System.Drawing.Size(326, 24);
            this.cbUserPP.TabIndex = 14;
            this.cbUserPP.SelectedIndexChanged += new System.EventHandler(this.cbUserPP_SelectedIndexChanged);
            // 
            // gbPrePP
            // 
            this.gbPrePP.Controls.Add(this.tableLayoutPanel3);
            this.gbPrePP.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPrePP.Location = new System.Drawing.Point(0, 397);
            this.gbPrePP.Name = "gbPrePP";
            this.gbPrePP.Size = new System.Drawing.Size(332, 55);
            this.gbPrePP.TabIndex = 1;
            this.gbPrePP.TabStop = false;
            this.gbPrePP.Text = "Predefined post-process combinations:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.btApply, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cbPrePP, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(326, 34);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // btApply
            // 
            this.btApply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btApply.Location = new System.Drawing.Point(247, 3);
            this.btApply.Name = "btApply";
            this.btApply.Size = new System.Drawing.Size(76, 28);
            this.btApply.TabIndex = 2;
            this.btApply.Text = "Apply";
            this.btApply.UseVisualStyleBackColor = true;
            this.btApply.Click += new System.EventHandler(this.btApply_Click);
            // 
            // cbPrePP
            // 
            this.cbPrePP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPrePP.FormattingEnabled = true;
            this.cbPrePP.Location = new System.Drawing.Point(3, 3);
            this.cbPrePP.Name = "cbPrePP";
            this.cbPrePP.Size = new System.Drawing.Size(238, 24);
            this.cbPrePP.TabIndex = 1;
            // 
            // gbAvaibleEffects
            // 
            this.gbAvaibleEffects.Controls.Add(this.lbxEffects);
            this.gbAvaibleEffects.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbAvaibleEffects.Location = new System.Drawing.Point(0, 98);
            this.gbAvaibleEffects.Name = "gbAvaibleEffects";
            this.gbAvaibleEffects.Size = new System.Drawing.Size(332, 133);
            this.gbAvaibleEffects.TabIndex = 8;
            this.gbAvaibleEffects.TabStop = false;
            this.gbAvaibleEffects.Text = "Available effects (Double click inserts effect)";
            // 
            // lbxEffects
            // 
            this.lbxEffects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxEffects.FormattingEnabled = true;
            this.lbxEffects.ItemHeight = 16;
            this.lbxEffects.Location = new System.Drawing.Point(3, 18);
            this.lbxEffects.Name = "lbxEffects";
            this.lbxEffects.Size = new System.Drawing.Size(326, 100);
            this.lbxEffects.TabIndex = 4;
            this.lbxEffects.DoubleClick += new System.EventHandler(this.lbxEffects_DoubleClick);
            // 
            // gbActiveEffects
            // 
            this.gbActiveEffects.Controls.Add(this.panel1);
            this.gbActiveEffects.Controls.Add(this.lbxActiveEffects);
            this.gbActiveEffects.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbActiveEffects.Location = new System.Drawing.Point(0, 231);
            this.gbActiveEffects.Name = "gbActiveEffects";
            this.gbActiveEffects.Size = new System.Drawing.Size(332, 166);
            this.gbActiveEffects.TabIndex = 12;
            this.gbActiveEffects.TabStop = false;
            this.gbActiveEffects.Text = "Active effects";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(326, 40);
            this.panel1.TabIndex = 15;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.Controls.Add(this.btEffectDelete, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btMoveDown, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btMoveUp, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(326, 40);
            this.tableLayoutPanel2.TabIndex = 15;
            // 
            // btEffectDelete
            // 
            this.btEffectDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btEffectDelete.Location = new System.Drawing.Point(231, 3);
            this.btEffectDelete.Name = "btEffectDelete";
            this.btEffectDelete.Size = new System.Drawing.Size(92, 34);
            this.btEffectDelete.TabIndex = 9;
            this.btEffectDelete.Text = "Delete";
            this.btEffectDelete.UseVisualStyleBackColor = true;
            this.btEffectDelete.Click += new System.EventHandler(this.btEffectDelete_Click);
            // 
            // btMoveDown
            // 
            this.btMoveDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btMoveDown.Location = new System.Drawing.Point(117, 3);
            this.btMoveDown.Name = "btMoveDown";
            this.btMoveDown.Size = new System.Drawing.Size(108, 34);
            this.btMoveDown.TabIndex = 8;
            this.btMoveDown.Text = "Move Down";
            this.btMoveDown.UseVisualStyleBackColor = true;
            this.btMoveDown.Click += new System.EventHandler(this.btMoveDown_Click);
            // 
            // btMoveUp
            // 
            this.btMoveUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btMoveUp.Location = new System.Drawing.Point(3, 3);
            this.btMoveUp.Name = "btMoveUp";
            this.btMoveUp.Size = new System.Drawing.Size(108, 34);
            this.btMoveUp.TabIndex = 7;
            this.btMoveUp.Text = "Move Up";
            this.btMoveUp.UseVisualStyleBackColor = true;
            this.btMoveUp.Click += new System.EventHandler(this.btMoveUp_Click);
            // 
            // lbxActiveEffects
            // 
            this.lbxActiveEffects.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbxActiveEffects.FormattingEnabled = true;
            this.lbxActiveEffects.ItemHeight = 16;
            this.lbxActiveEffects.Location = new System.Drawing.Point(3, 18);
            this.lbxActiveEffects.Name = "lbxActiveEffects";
            this.lbxActiveEffects.Size = new System.Drawing.Size(326, 100);
            this.lbxActiveEffects.TabIndex = 13;
            this.lbxActiveEffects.SelectedIndexChanged += new System.EventHandler(this.lbxActiveEffects_SelectedIndexChanged);
            // 
            // gbParam
            // 
            this.gbParam.AutoScroll = true;
            this.gbParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbParam.Location = new System.Drawing.Point(0, 452);
            this.gbParam.Name = "gbParam";
            this.gbParam.Size = new System.Drawing.Size(332, 156);
            this.gbParam.TabIndex = 13;
            // 
            // PostProcessWindows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 608);
            this.Controls.Add(this.gbParam);
            this.Controls.Add(this.gbPrePP);
            this.Controls.Add(this.gbActiveEffects);
            this.Controls.Add(this.gbAvaibleEffects);
            this.Controls.Add(this.gbUserPP);
            this.Controls.Add(this.chbxEnable);
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PostProcessWindows";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.ShowInTaskbar = false;
            this.TabText = "PostProcess Manager";
            this.Text = "PostProcess Manager";
            this.Load += new System.EventHandler(this.PostProcessWindows_Load);
            this.Click += new System.EventHandler(this.PostProcessWindows_Load);
            this.gbUserPP.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gbPrePP.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.gbAvaibleEffects.ResumeLayout(false);
            this.gbActiveEffects.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox chbxEnable;
        private System.Windows.Forms.GroupBox gbUserPP;
        private System.Windows.Forms.GroupBox gbAvaibleEffects;
        private System.Windows.Forms.GroupBox gbActiveEffects;
        private System.Windows.Forms.GroupBox gbPrePP;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btRename;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btNew;
        private System.Windows.Forms.ComboBox cbUserPP;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btApply;
        private System.Windows.Forms.ComboBox cbPrePP;
        private System.Windows.Forms.ListBox lbxEffects;
        private System.Windows.Forms.ListBox lbxActiveEffects;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btEffectDelete;
        private System.Windows.Forms.Button btMoveDown;
        private System.Windows.Forms.Button btMoveUp;
        private System.Windows.Forms.Panel gbParam;
    }
}