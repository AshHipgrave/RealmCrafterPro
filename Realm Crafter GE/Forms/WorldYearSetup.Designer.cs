namespace RealmCrafter_GE.Forms
{
    partial class WorldYearSetup
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
            System.Windows.Forms.Label WorldYearCurrentDayLabel;
            System.Windows.Forms.Label WorldYearCurrentYearLabel;
            System.Windows.Forms.Label WorldYearCompressionLabel;
            this.GroupYear = new System.Windows.Forms.GroupBox();
            this.WorldYearCurrentDay = new System.Windows.Forms.NumericUpDown();
            this.WorldYearCurrentYear = new System.Windows.Forms.NumericUpDown();
            this.WorldYearTimeCompression = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            WorldYearCurrentDayLabel = new System.Windows.Forms.Label();
            WorldYearCurrentYearLabel = new System.Windows.Forms.Label();
            WorldYearCompressionLabel = new System.Windows.Forms.Label();
            this.GroupYear.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorldYearCurrentDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorldYearCurrentYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorldYearTimeCompression)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // WorldYearCurrentDayLabel
            // 
            WorldYearCurrentDayLabel.AutoSize = true;
            WorldYearCurrentDayLabel.Location = new System.Drawing.Point(8, 73);
            WorldYearCurrentDayLabel.Name = "WorldYearCurrentDayLabel";
            WorldYearCurrentDayLabel.Size = new System.Drawing.Size(64, 13);
            WorldYearCurrentDayLabel.TabIndex = 6;
            WorldYearCurrentDayLabel.Text = "Current day:";
            // 
            // WorldYearCurrentYearLabel
            // 
            WorldYearCurrentYearLabel.AutoSize = true;
            WorldYearCurrentYearLabel.Location = new System.Drawing.Point(8, 47);
            WorldYearCurrentYearLabel.Name = "WorldYearCurrentYearLabel";
            WorldYearCurrentYearLabel.Size = new System.Drawing.Size(67, 13);
            WorldYearCurrentYearLabel.TabIndex = 5;
            WorldYearCurrentYearLabel.Text = "Current year:";
            // 
            // WorldYearCompressionLabel
            // 
            WorldYearCompressionLabel.AutoSize = true;
            WorldYearCompressionLabel.Location = new System.Drawing.Point(6, 21);
            WorldYearCompressionLabel.Name = "WorldYearCompressionLabel";
            WorldYearCompressionLabel.Size = new System.Drawing.Size(95, 13);
            WorldYearCompressionLabel.TabIndex = 3;
            WorldYearCompressionLabel.Text = "Time compression:";
            // 
            // GroupYear
            // 
            this.GroupYear.BackColor = System.Drawing.SystemColors.Control;
            this.GroupYear.Controls.Add(this.WorldYearCurrentDay);
            this.GroupYear.Controls.Add(this.WorldYearCurrentYear);
            this.GroupYear.Controls.Add(WorldYearCurrentDayLabel);
            this.GroupYear.Controls.Add(WorldYearCurrentYearLabel);
            this.GroupYear.Controls.Add(this.WorldYearTimeCompression);
            this.GroupYear.Controls.Add(WorldYearCompressionLabel);
            this.GroupYear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupYear.Location = new System.Drawing.Point(0, 0);
            this.GroupYear.Name = "GroupYear";
            this.GroupYear.Size = new System.Drawing.Size(212, 110);
            this.GroupYear.TabIndex = 28;
            this.GroupYear.TabStop = false;
            // 
            // WorldYearCurrentDay
            // 
            this.WorldYearCurrentDay.Location = new System.Drawing.Point(117, 71);
            this.WorldYearCurrentDay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.WorldYearCurrentDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorldYearCurrentDay.Name = "WorldYearCurrentDay";
            this.WorldYearCurrentDay.Size = new System.Drawing.Size(86, 20);
            this.WorldYearCurrentDay.TabIndex = 8;
            this.WorldYearCurrentDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorldYearCurrentDay.ValueChanged += new System.EventHandler(this.WorldYearCurrentDay_ValueChanged);
            // 
            // WorldYearCurrentYear
            // 
            this.WorldYearCurrentYear.Location = new System.Drawing.Point(117, 45);
            this.WorldYearCurrentYear.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.WorldYearCurrentYear.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.WorldYearCurrentYear.Name = "WorldYearCurrentYear";
            this.WorldYearCurrentYear.Size = new System.Drawing.Size(86, 20);
            this.WorldYearCurrentYear.TabIndex = 7;
            this.WorldYearCurrentYear.ValueChanged += new System.EventHandler(this.WorldYearCurrentYear_ValueChanged);
            // 
            // WorldYearTimeCompression
            // 
            this.WorldYearTimeCompression.Location = new System.Drawing.Point(117, 19);
            this.WorldYearTimeCompression.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WorldYearTimeCompression.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorldYearTimeCompression.Name = "WorldYearTimeCompression";
            this.WorldYearTimeCompression.Size = new System.Drawing.Size(86, 20);
            this.WorldYearTimeCompression.TabIndex = 4;
            this.WorldYearTimeCompression.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorldYearTimeCompression.ValueChanged += new System.EventHandler(this.WorldYearTimeCompression_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.GroupYear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(212, 110);
            this.panel1.TabIndex = 29;
            // 
            // WorldYearSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 110);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorldYearSetup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Year Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorldYearSetup_FormClosing);
            this.GroupYear.ResumeLayout(false);
            this.GroupYear.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorldYearCurrentDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorldYearCurrentYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorldYearTimeCompression)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupYear;
        private System.Windows.Forms.NumericUpDown WorldYearCurrentDay;
        private System.Windows.Forms.NumericUpDown WorldYearCurrentYear;
        private System.Windows.Forms.NumericUpDown WorldYearTimeCompression;
        private System.Windows.Forms.Panel panel1;
    }
}