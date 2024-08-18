namespace RealmCrafter_GE
{
    partial class FontEditor
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
            this.FontPreview = new System.Windows.Forms.PictureBox();
            this.DoneButton = new System.Windows.Forms.Button();
            this.ChooseFontButton = new System.Windows.Forms.Button();
            this.ChooseColourButton = new System.Windows.Forms.Button();
            this.ChooseShadowColourButton = new System.Windows.Forms.Button();
            this.UseShadowCheck = new System.Windows.Forms.CheckBox();
            this.SaveFontButton = new System.Windows.Forms.Button();
            this.FontEditColourDialog = new System.Windows.Forms.ColorDialog();
            this.FontEditFontDialog = new System.Windows.Forms.FontDialog();
            ((System.ComponentModel.ISupportInitialize)(this.FontPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // FontPreview
            // 
            this.FontPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FontPreview.Location = new System.Drawing.Point(12, 12);
            this.FontPreview.Name = "FontPreview";
            this.FontPreview.Size = new System.Drawing.Size(512, 512);
            this.FontPreview.TabIndex = 0;
            this.FontPreview.TabStop = false;
            // 
            // DoneButton
            // 
            this.DoneButton.Location = new System.Drawing.Point(530, 501);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(108, 23);
            this.DoneButton.TabIndex = 1;
            this.DoneButton.Text = "Close";
            this.DoneButton.UseVisualStyleBackColor = true;
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // ChooseFontButton
            // 
            this.ChooseFontButton.Location = new System.Drawing.Point(530, 12);
            this.ChooseFontButton.Name = "ChooseFontButton";
            this.ChooseFontButton.Size = new System.Drawing.Size(108, 23);
            this.ChooseFontButton.TabIndex = 2;
            this.ChooseFontButton.Text = "Choose Font";
            this.ChooseFontButton.UseVisualStyleBackColor = true;
            this.ChooseFontButton.Click += new System.EventHandler(this.ChooseFontButton_Click);
            // 
            // ChooseColourButton
            // 
            this.ChooseColourButton.Location = new System.Drawing.Point(530, 41);
            this.ChooseColourButton.Name = "ChooseColourButton";
            this.ChooseColourButton.Size = new System.Drawing.Size(108, 23);
            this.ChooseColourButton.TabIndex = 3;
            this.ChooseColourButton.Text = "Text Colour";
            this.ChooseColourButton.UseVisualStyleBackColor = true;
            this.ChooseColourButton.Click += new System.EventHandler(this.ChooseColourButton_Click);
            // 
            // ChooseShadowColourButton
            // 
            this.ChooseShadowColourButton.Location = new System.Drawing.Point(530, 70);
            this.ChooseShadowColourButton.Name = "ChooseShadowColourButton";
            this.ChooseShadowColourButton.Size = new System.Drawing.Size(108, 23);
            this.ChooseShadowColourButton.TabIndex = 4;
            this.ChooseShadowColourButton.Text = "Shadow Colour";
            this.ChooseShadowColourButton.UseVisualStyleBackColor = true;
            this.ChooseShadowColourButton.Click += new System.EventHandler(this.ChooseShadowColourButton_Click);
            // 
            // UseShadowCheck
            // 
            this.UseShadowCheck.AutoSize = true;
            this.UseShadowCheck.Checked = true;
            this.UseShadowCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseShadowCheck.Location = new System.Drawing.Point(530, 99);
            this.UseShadowCheck.Name = "UseShadowCheck";
            this.UseShadowCheck.Size = new System.Drawing.Size(85, 17);
            this.UseShadowCheck.TabIndex = 5;
            this.UseShadowCheck.Text = "Use shadow";
            this.UseShadowCheck.UseVisualStyleBackColor = true;
            this.UseShadowCheck.CheckedChanged += new System.EventHandler(this.UseShadowCheck_CheckedChanged);
            // 
            // SaveFontButton
            // 
            this.SaveFontButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveFontButton.Location = new System.Drawing.Point(530, 132);
            this.SaveFontButton.Name = "SaveFontButton";
            this.SaveFontButton.Size = new System.Drawing.Size(108, 23);
            this.SaveFontButton.TabIndex = 6;
            this.SaveFontButton.Text = "Save Font";
            this.SaveFontButton.UseVisualStyleBackColor = true;
            this.SaveFontButton.Click += new System.EventHandler(this.SaveFontButton_Click);
            // 
            // FontEditColourDialog
            // 
            this.FontEditColourDialog.AnyColor = true;
            this.FontEditColourDialog.Color = System.Drawing.Color.White;
            this.FontEditColourDialog.FullOpen = true;
            this.FontEditColourDialog.SolidColorOnly = true;
            // 
            // FontEditFontDialog
            // 
            this.FontEditFontDialog.Font = new System.Drawing.Font("Arial", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontEditFontDialog.FontMustExist = true;
            this.FontEditFontDialog.MaxSize = 64;
            this.FontEditFontDialog.MinSize = 8;
            // 
            // FontEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 538);
            this.Controls.Add(this.SaveFontButton);
            this.Controls.Add(this.UseShadowCheck);
            this.Controls.Add(this.ChooseShadowColourButton);
            this.Controls.Add(this.ChooseColourButton);
            this.Controls.Add(this.ChooseFontButton);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.FontPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Font Editor";
            ((System.ComponentModel.ISupportInitialize)(this.FontPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox FontPreview;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.Button ChooseFontButton;
        private System.Windows.Forms.Button ChooseColourButton;
        private System.Windows.Forms.Button ChooseShadowColourButton;
        private System.Windows.Forms.CheckBox UseShadowCheck;
        private System.Windows.Forms.Button SaveFontButton;
        private System.Windows.Forms.ColorDialog FontEditColourDialog;
        private System.Windows.Forms.FontDialog FontEditFontDialog;
    }
}