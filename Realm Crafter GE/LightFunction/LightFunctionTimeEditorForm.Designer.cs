namespace RealmCrafter
{
    partial class LightFunctionTimeEditorForm
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
            this.WaitTimerOption = new System.Windows.Forms.RadioButton();
            this.GameTimerOption = new System.Windows.Forms.RadioButton();
            this.WaitTimeMS = new System.Windows.Forms.NumericUpDown();
            this.GameTimeHour = new System.Windows.Forms.NumericUpDown();
            this.GameTimeMinute = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.WaitTimeMS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameTimeHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameTimeMinute)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wait Timer:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Game Time:";
            // 
            // WaitTimerOption
            // 
            this.WaitTimerOption.AutoSize = true;
            this.WaitTimerOption.Location = new System.Drawing.Point(15, 27);
            this.WaitTimerOption.Name = "WaitTimerOption";
            this.WaitTimerOption.Size = new System.Drawing.Size(14, 13);
            this.WaitTimerOption.TabIndex = 2;
            this.WaitTimerOption.TabStop = true;
            this.WaitTimerOption.UseVisualStyleBackColor = true;
            // 
            // GameTimerOption
            // 
            this.GameTimerOption.AutoSize = true;
            this.GameTimerOption.Location = new System.Drawing.Point(17, 66);
            this.GameTimerOption.Name = "GameTimerOption";
            this.GameTimerOption.Size = new System.Drawing.Size(14, 13);
            this.GameTimerOption.TabIndex = 3;
            this.GameTimerOption.TabStop = true;
            this.GameTimerOption.UseVisualStyleBackColor = true;
            // 
            // WaitTimeMS
            // 
            this.WaitTimeMS.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.WaitTimeMS.Location = new System.Drawing.Point(37, 25);
            this.WaitTimeMS.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.WaitTimeMS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WaitTimeMS.Name = "WaitTimeMS";
            this.WaitTimeMS.Size = new System.Drawing.Size(78, 20);
            this.WaitTimeMS.TabIndex = 4;
            this.WaitTimeMS.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // GameTimeHour
            // 
            this.GameTimeHour.Location = new System.Drawing.Point(37, 64);
            this.GameTimeHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.GameTimeHour.Name = "GameTimeHour";
            this.GameTimeHour.Size = new System.Drawing.Size(36, 20);
            this.GameTimeHour.TabIndex = 5;
            // 
            // GameTimeMinute
            // 
            this.GameTimeMinute.Location = new System.Drawing.Point(79, 64);
            this.GameTimeMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.GameTimeMinute.Name = "GameTimeMinute";
            this.GameTimeMinute.Size = new System.Drawing.Size(36, 20);
            this.GameTimeMinute.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(121, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "ms";
            // 
            // LightFunctionTimeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 96);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GameTimeMinute);
            this.Controls.Add(this.GameTimeHour);
            this.Controls.Add(this.WaitTimeMS);
            this.Controls.Add(this.GameTimerOption);
            this.Controls.Add(this.WaitTimerOption);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LightFunctionTimeEditorForm";
            this.Text = "LightFunctionTimeEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.WaitTimeMS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameTimeHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameTimeMinute)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton WaitTimerOption;
        private System.Windows.Forms.RadioButton GameTimerOption;
        private System.Windows.Forms.NumericUpDown WaitTimeMS;
        private System.Windows.Forms.NumericUpDown GameTimeHour;
        private System.Windows.Forms.NumericUpDown GameTimeMinute;
        private System.Windows.Forms.Label label3;
    }
}