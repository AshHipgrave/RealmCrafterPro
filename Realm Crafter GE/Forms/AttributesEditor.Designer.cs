namespace RealmCrafter_GE
{
    partial class AttributesEditor
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
            System.Windows.Forms.Label AttributeRenameLabel;
            System.Windows.Forms.Label AttributeBelowLabel;
            System.Windows.Forms.Label AttributeAssignLabel;
            System.Windows.Forms.GroupBox AttributeFixedGroup;
            System.Windows.Forms.Label AttributeFixedEnergyLabel;
            System.Windows.Forms.Label AttributeFixedBreathLabel;
            System.Windows.Forms.Label AttributeFixedToughnessLabel;
            System.Windows.Forms.Label AttributeFixedStrengthLabel;
            System.Windows.Forms.Label AttributeFixedSpeedLabel;
            System.Windows.Forms.Label AttributeFixedHealthLabel;
            this.AttributeFixedEnergyCombo = new System.Windows.Forms.ComboBox();
            this.AttributeFixedBreathCombo = new System.Windows.Forms.ComboBox();
            this.AttributeFixedToughnessCombo = new System.Windows.Forms.ComboBox();
            this.AttributeFixedStrengthCombo = new System.Windows.Forms.ComboBox();
            this.AttributeFixedSpeedCombo = new System.Windows.Forms.ComboBox();
            this.AttributeFixedHealthCombo = new System.Windows.Forms.ComboBox();
            this.AttributesSave = new System.Windows.Forms.Button();
            this.AttributesCancel = new System.Windows.Forms.Button();
            this.AttributesList = new System.Windows.Forms.ListBox();
            this.AttributeNameText = new System.Windows.Forms.TextBox();
            this.AttributeSkillCheck = new System.Windows.Forms.CheckBox();
            this.AttributeHideCheck = new System.Windows.Forms.CheckBox();
            this.AttributeAdd = new System.Windows.Forms.Button();
            this.AttributeRemove = new System.Windows.Forms.Button();
            this.AttributeAssignSpinner = new System.Windows.Forms.NumericUpDown();
            AttributeRenameLabel = new System.Windows.Forms.Label();
            AttributeBelowLabel = new System.Windows.Forms.Label();
            AttributeAssignLabel = new System.Windows.Forms.Label();
            AttributeFixedGroup = new System.Windows.Forms.GroupBox();
            AttributeFixedEnergyLabel = new System.Windows.Forms.Label();
            AttributeFixedBreathLabel = new System.Windows.Forms.Label();
            AttributeFixedToughnessLabel = new System.Windows.Forms.Label();
            AttributeFixedStrengthLabel = new System.Windows.Forms.Label();
            AttributeFixedSpeedLabel = new System.Windows.Forms.Label();
            AttributeFixedHealthLabel = new System.Windows.Forms.Label();
            AttributeFixedGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AttributeAssignSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // AttributeRenameLabel
            // 
            AttributeRenameLabel.AutoSize = true;
            AttributeRenameLabel.Location = new System.Drawing.Point(212, 63);
            AttributeRenameLabel.Name = "AttributeRenameLabel";
            AttributeRenameLabel.Size = new System.Drawing.Size(91, 13);
            AttributeRenameLabel.TabIndex = 3;
            AttributeRenameLabel.Text = "Rename attribute:";
            // 
            // AttributeBelowLabel
            // 
            AttributeBelowLabel.AutoSize = true;
            AttributeBelowLabel.Location = new System.Drawing.Point(12, 9);
            AttributeBelowLabel.Name = "AttributeBelowLabel";
            AttributeBelowLabel.Size = new System.Drawing.Size(191, 13);
            AttributeBelowLabel.TabIndex = 9;
            AttributeBelowLabel.Text = "You may add up to 40 attributes below:";
            // 
            // AttributeAssignLabel
            // 
            AttributeAssignLabel.AutoSize = true;
            AttributeAssignLabel.Location = new System.Drawing.Point(12, 477);
            AttributeAssignLabel.Name = "AttributeAssignLabel";
            AttributeAssignLabel.Size = new System.Drawing.Size(278, 13);
            AttributeAssignLabel.TabIndex = 10;
            AttributeAssignLabel.Text = "Assignable attribute points available in character creation:";
            // 
            // AttributeFixedGroup
            // 
            AttributeFixedGroup.Controls.Add(this.AttributeFixedEnergyCombo);
            AttributeFixedGroup.Controls.Add(this.AttributeFixedBreathCombo);
            AttributeFixedGroup.Controls.Add(this.AttributeFixedToughnessCombo);
            AttributeFixedGroup.Controls.Add(this.AttributeFixedStrengthCombo);
            AttributeFixedGroup.Controls.Add(this.AttributeFixedSpeedCombo);
            AttributeFixedGroup.Controls.Add(this.AttributeFixedHealthCombo);
            AttributeFixedGroup.Controls.Add(AttributeFixedEnergyLabel);
            AttributeFixedGroup.Controls.Add(AttributeFixedBreathLabel);
            AttributeFixedGroup.Controls.Add(AttributeFixedToughnessLabel);
            AttributeFixedGroup.Controls.Add(AttributeFixedStrengthLabel);
            AttributeFixedGroup.Controls.Add(AttributeFixedSpeedLabel);
            AttributeFixedGroup.Controls.Add(AttributeFixedHealthLabel);
            AttributeFixedGroup.Location = new System.Drawing.Point(444, 12);
            AttributeFixedGroup.Name = "AttributeFixedGroup";
            AttributeFixedGroup.Size = new System.Drawing.Size(254, 187);
            AttributeFixedGroup.TabIndex = 12;
            AttributeFixedGroup.TabStop = false;
            AttributeFixedGroup.Text = "Fixed attributes";
            // 
            // AttributeFixedEnergyCombo
            // 
            this.AttributeFixedEnergyCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttributeFixedEnergyCombo.FormattingEnabled = true;
            this.AttributeFixedEnergyCombo.Location = new System.Drawing.Point(74, 46);
            this.AttributeFixedEnergyCombo.Name = "AttributeFixedEnergyCombo";
            this.AttributeFixedEnergyCombo.Size = new System.Drawing.Size(174, 21);
            this.AttributeFixedEnergyCombo.TabIndex = 11;
            this.AttributeFixedEnergyCombo.SelectedIndexChanged += new System.EventHandler(this.AttributeFixedEnergyCombo_SelectedIndexChanged);
            // 
            // AttributeFixedBreathCombo
            // 
            this.AttributeFixedBreathCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttributeFixedBreathCombo.FormattingEnabled = true;
            this.AttributeFixedBreathCombo.Location = new System.Drawing.Point(74, 73);
            this.AttributeFixedBreathCombo.Name = "AttributeFixedBreathCombo";
            this.AttributeFixedBreathCombo.Size = new System.Drawing.Size(174, 21);
            this.AttributeFixedBreathCombo.TabIndex = 10;
            this.AttributeFixedBreathCombo.SelectedIndexChanged += new System.EventHandler(this.AttributeFixedBreathCombo_SelectedIndexChanged);
            // 
            // AttributeFixedToughnessCombo
            // 
            this.AttributeFixedToughnessCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttributeFixedToughnessCombo.FormattingEnabled = true;
            this.AttributeFixedToughnessCombo.Location = new System.Drawing.Point(74, 100);
            this.AttributeFixedToughnessCombo.Name = "AttributeFixedToughnessCombo";
            this.AttributeFixedToughnessCombo.Size = new System.Drawing.Size(174, 21);
            this.AttributeFixedToughnessCombo.TabIndex = 9;
            this.AttributeFixedToughnessCombo.SelectedIndexChanged += new System.EventHandler(this.AttributeFixedToughnessCombo_SelectedIndexChanged);
            // 
            // AttributeFixedStrengthCombo
            // 
            this.AttributeFixedStrengthCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttributeFixedStrengthCombo.FormattingEnabled = true;
            this.AttributeFixedStrengthCombo.Location = new System.Drawing.Point(74, 127);
            this.AttributeFixedStrengthCombo.Name = "AttributeFixedStrengthCombo";
            this.AttributeFixedStrengthCombo.Size = new System.Drawing.Size(174, 21);
            this.AttributeFixedStrengthCombo.TabIndex = 8;
            this.AttributeFixedStrengthCombo.SelectedIndexChanged += new System.EventHandler(this.AttributeFixedStrengthCombo_SelectedIndexChanged);
            // 
            // AttributeFixedSpeedCombo
            // 
            this.AttributeFixedSpeedCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttributeFixedSpeedCombo.FormattingEnabled = true;
            this.AttributeFixedSpeedCombo.Location = new System.Drawing.Point(74, 154);
            this.AttributeFixedSpeedCombo.Name = "AttributeFixedSpeedCombo";
            this.AttributeFixedSpeedCombo.Size = new System.Drawing.Size(174, 21);
            this.AttributeFixedSpeedCombo.TabIndex = 7;
            this.AttributeFixedSpeedCombo.SelectedIndexChanged += new System.EventHandler(this.AttributeFixedSpeedCombo_SelectedIndexChanged);
            // 
            // AttributeFixedHealthCombo
            // 
            this.AttributeFixedHealthCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttributeFixedHealthCombo.FormattingEnabled = true;
            this.AttributeFixedHealthCombo.Location = new System.Drawing.Point(74, 19);
            this.AttributeFixedHealthCombo.Name = "AttributeFixedHealthCombo";
            this.AttributeFixedHealthCombo.Size = new System.Drawing.Size(174, 21);
            this.AttributeFixedHealthCombo.TabIndex = 6;
            this.AttributeFixedHealthCombo.SelectedIndexChanged += new System.EventHandler(this.AttributeFixedHealthCombo_SelectedIndexChanged);
            // 
            // AttributeFixedEnergyLabel
            // 
            AttributeFixedEnergyLabel.AutoSize = true;
            AttributeFixedEnergyLabel.Location = new System.Drawing.Point(6, 49);
            AttributeFixedEnergyLabel.Name = "AttributeFixedEnergyLabel";
            AttributeFixedEnergyLabel.Size = new System.Drawing.Size(43, 13);
            AttributeFixedEnergyLabel.TabIndex = 5;
            AttributeFixedEnergyLabel.Text = "Energy:";
            // 
            // AttributeFixedBreathLabel
            // 
            AttributeFixedBreathLabel.AutoSize = true;
            AttributeFixedBreathLabel.Location = new System.Drawing.Point(6, 76);
            AttributeFixedBreathLabel.Name = "AttributeFixedBreathLabel";
            AttributeFixedBreathLabel.Size = new System.Drawing.Size(41, 13);
            AttributeFixedBreathLabel.TabIndex = 4;
            AttributeFixedBreathLabel.Text = "Breath:";
            // 
            // AttributeFixedToughnessLabel
            // 
            AttributeFixedToughnessLabel.AutoSize = true;
            AttributeFixedToughnessLabel.Location = new System.Drawing.Point(6, 103);
            AttributeFixedToughnessLabel.Name = "AttributeFixedToughnessLabel";
            AttributeFixedToughnessLabel.Size = new System.Drawing.Size(63, 13);
            AttributeFixedToughnessLabel.TabIndex = 3;
            AttributeFixedToughnessLabel.Text = "Toughness:";
            // 
            // AttributeFixedStrengthLabel
            // 
            AttributeFixedStrengthLabel.AutoSize = true;
            AttributeFixedStrengthLabel.Location = new System.Drawing.Point(6, 130);
            AttributeFixedStrengthLabel.Name = "AttributeFixedStrengthLabel";
            AttributeFixedStrengthLabel.Size = new System.Drawing.Size(50, 13);
            AttributeFixedStrengthLabel.TabIndex = 2;
            AttributeFixedStrengthLabel.Text = "Strength:";
            // 
            // AttributeFixedSpeedLabel
            // 
            AttributeFixedSpeedLabel.AutoSize = true;
            AttributeFixedSpeedLabel.Location = new System.Drawing.Point(6, 157);
            AttributeFixedSpeedLabel.Name = "AttributeFixedSpeedLabel";
            AttributeFixedSpeedLabel.Size = new System.Drawing.Size(41, 13);
            AttributeFixedSpeedLabel.TabIndex = 1;
            AttributeFixedSpeedLabel.Text = "Speed:";
            // 
            // AttributeFixedHealthLabel
            // 
            AttributeFixedHealthLabel.AutoSize = true;
            AttributeFixedHealthLabel.Location = new System.Drawing.Point(6, 22);
            AttributeFixedHealthLabel.Name = "AttributeFixedHealthLabel";
            AttributeFixedHealthLabel.Size = new System.Drawing.Size(41, 13);
            AttributeFixedHealthLabel.TabIndex = 0;
            AttributeFixedHealthLabel.Text = "Health:";
            // 
            // AttributesSave
            // 
            this.AttributesSave.Location = new System.Drawing.Point(600, 472);
            this.AttributesSave.Name = "AttributesSave";
            this.AttributesSave.Size = new System.Drawing.Size(99, 23);
            this.AttributesSave.TabIndex = 0;
            this.AttributesSave.Text = "Save attributes";
            this.AttributesSave.UseVisualStyleBackColor = true;
            this.AttributesSave.Click += new System.EventHandler(this.AttributesSave_Click);
            // 
            // AttributesCancel
            // 
            this.AttributesCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AttributesCancel.Location = new System.Drawing.Point(519, 472);
            this.AttributesCancel.Name = "AttributesCancel";
            this.AttributesCancel.Size = new System.Drawing.Size(75, 23);
            this.AttributesCancel.TabIndex = 1;
            this.AttributesCancel.Text = "Cancel";
            this.AttributesCancel.UseVisualStyleBackColor = true;
            this.AttributesCancel.Click += new System.EventHandler(this.AttributesCancel_Click);
            // 
            // AttributesList
            // 
            this.AttributesList.FormattingEnabled = true;
            this.AttributesList.Location = new System.Drawing.Point(15, 63);
            this.AttributesList.Name = "AttributesList";
            this.AttributesList.Size = new System.Drawing.Size(191, 394);
            this.AttributesList.TabIndex = 2;
            this.AttributesList.SelectedIndexChanged += new System.EventHandler(this.AttributesList_SelectedIndexChanged);
            // 
            // AttributeNameText
            // 
            this.AttributeNameText.Location = new System.Drawing.Point(233, 79);
            this.AttributeNameText.MaxLength = 200;
            this.AttributeNameText.Name = "AttributeNameText";
            this.AttributeNameText.Size = new System.Drawing.Size(165, 20);
            this.AttributeNameText.TabIndex = 4;
            this.AttributeNameText.TextChanged += new System.EventHandler(this.AttributeNameText_TextChanged);
            // 
            // AttributeSkillCheck
            // 
            this.AttributeSkillCheck.AutoSize = true;
            this.AttributeSkillCheck.Location = new System.Drawing.Point(215, 115);
            this.AttributeSkillCheck.Name = "AttributeSkillCheck";
            this.AttributeSkillCheck.Size = new System.Drawing.Size(104, 17);
            this.AttributeSkillCheck.TabIndex = 5;
            this.AttributeSkillCheck.Text = "Attribute is a skill";
            this.AttributeSkillCheck.UseVisualStyleBackColor = true;
            this.AttributeSkillCheck.CheckedChanged += new System.EventHandler(this.AttributeSkillCheck_CheckedChanged);
            // 
            // AttributeHideCheck
            // 
            this.AttributeHideCheck.AutoSize = true;
            this.AttributeHideCheck.Location = new System.Drawing.Point(215, 138);
            this.AttributeHideCheck.Name = "AttributeHideCheck";
            this.AttributeHideCheck.Size = new System.Drawing.Size(148, 17);
            this.AttributeHideCheck.TabIndex = 6;
            this.AttributeHideCheck.Text = "Hide attribute from players";
            this.AttributeHideCheck.UseVisualStyleBackColor = true;
            this.AttributeHideCheck.CheckedChanged += new System.EventHandler(this.AttributeHideCheck_CheckedChanged);
            // 
            // AttributeAdd
            // 
            this.AttributeAdd.Location = new System.Drawing.Point(15, 34);
            this.AttributeAdd.Name = "AttributeAdd";
            this.AttributeAdd.Size = new System.Drawing.Size(88, 23);
            this.AttributeAdd.TabIndex = 7;
            this.AttributeAdd.Text = "Add attribute";
            this.AttributeAdd.UseVisualStyleBackColor = true;
            this.AttributeAdd.Click += new System.EventHandler(this.AttributeAdd_Click);
            // 
            // AttributeRemove
            // 
            this.AttributeRemove.Location = new System.Drawing.Point(109, 34);
            this.AttributeRemove.Name = "AttributeRemove";
            this.AttributeRemove.Size = new System.Drawing.Size(97, 23);
            this.AttributeRemove.TabIndex = 8;
            this.AttributeRemove.Text = "Remove attribute";
            this.AttributeRemove.UseVisualStyleBackColor = true;
            this.AttributeRemove.Click += new System.EventHandler(this.AttributeRemove_Click);
            // 
            // AttributeAssignSpinner
            // 
            this.AttributeAssignSpinner.Location = new System.Drawing.Point(296, 475);
            this.AttributeAssignSpinner.Name = "AttributeAssignSpinner";
            this.AttributeAssignSpinner.Size = new System.Drawing.Size(80, 20);
            this.AttributeAssignSpinner.TabIndex = 11;
            // 
            // AttributesEditor
            // 
            this.AcceptButton = this.AttributesSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.AttributesCancel;
            this.ClientSize = new System.Drawing.Size(710, 506);
            this.Controls.Add(AttributeFixedGroup);
            this.Controls.Add(this.AttributeAssignSpinner);
            this.Controls.Add(AttributeAssignLabel);
            this.Controls.Add(AttributeBelowLabel);
            this.Controls.Add(this.AttributeRemove);
            this.Controls.Add(this.AttributeAdd);
            this.Controls.Add(this.AttributeHideCheck);
            this.Controls.Add(this.AttributeSkillCheck);
            this.Controls.Add(this.AttributeNameText);
            this.Controls.Add(AttributeRenameLabel);
            this.Controls.Add(this.AttributesList);
            this.Controls.Add(this.AttributesCancel);
            this.Controls.Add(this.AttributesSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttributesEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Attributes";
            this.Load += new System.EventHandler(this.AttributesEditor_Load);
            AttributeFixedGroup.ResumeLayout(false);
            AttributeFixedGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AttributeAssignSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AttributesSave;
        private System.Windows.Forms.Button AttributesCancel;
        private System.Windows.Forms.TextBox AttributeNameText;
        private System.Windows.Forms.CheckBox AttributeSkillCheck;
        private System.Windows.Forms.CheckBox AttributeHideCheck;
        private System.Windows.Forms.Button AttributeAdd;
        private System.Windows.Forms.Button AttributeRemove;
        private System.Windows.Forms.NumericUpDown AttributeAssignSpinner;
        private System.Windows.Forms.ListBox AttributesList;
        private System.Windows.Forms.ComboBox AttributeFixedEnergyCombo;
        private System.Windows.Forms.ComboBox AttributeFixedBreathCombo;
        private System.Windows.Forms.ComboBox AttributeFixedToughnessCombo;
        private System.Windows.Forms.ComboBox AttributeFixedStrengthCombo;
        private System.Windows.Forms.ComboBox AttributeFixedSpeedCombo;
        private System.Windows.Forms.ComboBox AttributeFixedHealthCombo;
    }
}