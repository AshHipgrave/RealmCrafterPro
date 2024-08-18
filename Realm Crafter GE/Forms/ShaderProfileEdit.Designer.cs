namespace RealmCrafter_GE
{
    partial class ShaderProfileEdit
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.HNCombo = new System.Windows.Forms.ComboBox();
            this.MNCombo = new System.Windows.Forms.ComboBox();
            this.LNCombo = new System.Windows.Forms.ComboBox();
            this.LFCombo = new System.Windows.Forms.ComboBox();
            this.MFCombo = new System.Windows.Forms.ComboBox();
            this.HFCombo = new System.Windows.Forms.ComboBox();
            this.RangeBox = new System.Windows.Forms.NumericUpDown();
            this.SaveButton = new System.Windows.Forms.Button();
            this.DCancelButton = new System.Windows.Forms.Button();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.DefaultStaticBox = new System.Windows.Forms.CheckBox();
            this.DefaultAnimatedBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.RangeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Near Shader:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Far Shader:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "High Detail:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Medium Detail:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Low Detail:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Distance Scale:";
            // 
            // HNCombo
            // 
            this.HNCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HNCombo.FormattingEnabled = true;
            this.HNCombo.Location = new System.Drawing.Point(100, 25);
            this.HNCombo.Name = "HNCombo";
            this.HNCombo.Size = new System.Drawing.Size(121, 21);
            this.HNCombo.TabIndex = 6;
            this.HNCombo.SelectedIndexChanged += new System.EventHandler(this.HNCombo_SelectedIndexChanged);
            // 
            // MNCombo
            // 
            this.MNCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MNCombo.FormattingEnabled = true;
            this.MNCombo.Location = new System.Drawing.Point(100, 52);
            this.MNCombo.Name = "MNCombo";
            this.MNCombo.Size = new System.Drawing.Size(121, 21);
            this.MNCombo.TabIndex = 7;
            this.MNCombo.SelectedIndexChanged += new System.EventHandler(this.MNCombo_SelectedIndexChanged);
            // 
            // LNCombo
            // 
            this.LNCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LNCombo.FormattingEnabled = true;
            this.LNCombo.Location = new System.Drawing.Point(100, 79);
            this.LNCombo.Name = "LNCombo";
            this.LNCombo.Size = new System.Drawing.Size(121, 21);
            this.LNCombo.TabIndex = 11;
            this.LNCombo.SelectedIndexChanged += new System.EventHandler(this.LNCombo_SelectedIndexChanged);
            // 
            // LFCombo
            // 
            this.LFCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LFCombo.FormattingEnabled = true;
            this.LFCombo.Location = new System.Drawing.Point(227, 79);
            this.LFCombo.Name = "LFCombo";
            this.LFCombo.Size = new System.Drawing.Size(121, 21);
            this.LFCombo.TabIndex = 12;
            this.LFCombo.SelectedIndexChanged += new System.EventHandler(this.LFCombo_SelectedIndexChanged);
            // 
            // MFCombo
            // 
            this.MFCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MFCombo.FormattingEnabled = true;
            this.MFCombo.Location = new System.Drawing.Point(227, 52);
            this.MFCombo.Name = "MFCombo";
            this.MFCombo.Size = new System.Drawing.Size(121, 21);
            this.MFCombo.TabIndex = 13;
            this.MFCombo.SelectedIndexChanged += new System.EventHandler(this.MFCombo_SelectedIndexChanged);
            // 
            // HFCombo
            // 
            this.HFCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HFCombo.FormattingEnabled = true;
            this.HFCombo.Location = new System.Drawing.Point(227, 25);
            this.HFCombo.Name = "HFCombo";
            this.HFCombo.Size = new System.Drawing.Size(121, 21);
            this.HFCombo.TabIndex = 14;
            this.HFCombo.SelectedIndexChanged += new System.EventHandler(this.HFCombo_SelectedIndexChanged);
            // 
            // RangeBox
            // 
            this.RangeBox.DecimalPlaces = 2;
            this.RangeBox.Location = new System.Drawing.Point(100, 106);
            this.RangeBox.Name = "RangeBox";
            this.RangeBox.Size = new System.Drawing.Size(121, 20);
            this.RangeBox.TabIndex = 15;
            this.RangeBox.ValueChanged += new System.EventHandler(this.RangeBox_ValueChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(192, 159);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 16;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DCancelButton
            // 
            this.DCancelButton.Location = new System.Drawing.Point(273, 159);
            this.DCancelButton.Name = "DCancelButton";
            this.DCancelButton.Size = new System.Drawing.Size(75, 23);
            this.DCancelButton.TabIndex = 17;
            this.DCancelButton.Text = "Cancel";
            this.DCancelButton.UseVisualStyleBackColor = true;
            this.DCancelButton.Click += new System.EventHandler(this.DCancelButton_Click);
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(100, 133);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(121, 20);
            this.NameBox.TabIndex = 18;
            this.NameBox.TextChanged += new System.EventHandler(this.NameBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Profile Name:";
            // 
            // DefaultStaticBox
            // 
            this.DefaultStaticBox.AutoSize = true;
            this.DefaultStaticBox.Location = new System.Drawing.Point(227, 107);
            this.DefaultStaticBox.Name = "DefaultStaticBox";
            this.DefaultStaticBox.Size = new System.Drawing.Size(96, 17);
            this.DefaultStaticBox.TabIndex = 20;
            this.DefaultStaticBox.Text = "Default (Static)";
            this.DefaultStaticBox.UseVisualStyleBackColor = true;
            this.DefaultStaticBox.CheckedChanged += new System.EventHandler(this.DefaultStaticBox_CheckedChanged);
            // 
            // DefaultAnimatedBox
            // 
            this.DefaultAnimatedBox.AutoSize = true;
            this.DefaultAnimatedBox.Location = new System.Drawing.Point(227, 135);
            this.DefaultAnimatedBox.Name = "DefaultAnimatedBox";
            this.DefaultAnimatedBox.Size = new System.Drawing.Size(113, 17);
            this.DefaultAnimatedBox.TabIndex = 21;
            this.DefaultAnimatedBox.Text = "Default (Animated)";
            this.DefaultAnimatedBox.UseVisualStyleBackColor = true;
            this.DefaultAnimatedBox.CheckedChanged += new System.EventHandler(this.DefaultAnimatedBox_CheckedChanged);
            // 
            // ShaderProfileEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 191);
            this.Controls.Add(this.DefaultAnimatedBox);
            this.Controls.Add(this.DefaultStaticBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.DCancelButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.RangeBox);
            this.Controls.Add(this.HFCombo);
            this.Controls.Add(this.MFCombo);
            this.Controls.Add(this.LFCombo);
            this.Controls.Add(this.LNCombo);
            this.Controls.Add(this.MNCombo);
            this.Controls.Add(this.HNCombo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ShaderProfileEdit";
            this.Text = "ShaderProfileEdit";
            ((System.ComponentModel.ISupportInitialize)(this.RangeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox HNCombo;
        private System.Windows.Forms.ComboBox MNCombo;
        private System.Windows.Forms.ComboBox LNCombo;
        private System.Windows.Forms.ComboBox LFCombo;
        private System.Windows.Forms.ComboBox MFCombo;
        private System.Windows.Forms.ComboBox HFCombo;
        private System.Windows.Forms.NumericUpDown RangeBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button DCancelButton;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox DefaultStaticBox;
        private System.Windows.Forms.CheckBox DefaultAnimatedBox;
    }
}