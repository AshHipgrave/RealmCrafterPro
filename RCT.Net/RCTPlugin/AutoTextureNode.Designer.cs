namespace RCTPlugin
{
    partial class AutoTextureNode
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoTextureNode));
            this.MainGroup = new System.Windows.Forms.GroupBox();
            this.HeightMaxTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.HeightMinTextbox = new System.Windows.Forms.TextBox();
            this.SlopeMaxTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SlopeMinTextbox = new System.Windows.Forms.TextBox();
            this.TexturePathButton = new System.Windows.Forms.Button();
            this.TexturePanel = new System.Windows.Forms.Panel();
            this.TexturePathLabel = new System.Windows.Forms.Label();
            this.OpenTextureDialog = new System.Windows.Forms.OpenFileDialog();
            this.MainGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainGroup
            // 
            this.MainGroup.BackColor = System.Drawing.SystemColors.Control;
            this.MainGroup.Controls.Add(this.HeightMaxTextbox);
            this.MainGroup.Controls.Add(this.label3);
            this.MainGroup.Controls.Add(this.HeightMinTextbox);
            this.MainGroup.Controls.Add(this.SlopeMaxTextbox);
            this.MainGroup.Controls.Add(this.label2);
            this.MainGroup.Controls.Add(this.SlopeMinTextbox);
            this.MainGroup.Controls.Add(this.TexturePathButton);
            this.MainGroup.Controls.Add(this.TexturePanel);
            this.MainGroup.Controls.Add(this.TexturePathLabel);
            this.MainGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainGroup.Location = new System.Drawing.Point(0, 0);
            this.MainGroup.Name = "MainGroup";
            this.MainGroup.Size = new System.Drawing.Size(490, 96);
            this.MainGroup.TabIndex = 0;
            this.MainGroup.TabStop = false;
            this.MainGroup.Text = "TerrainType";
            this.MainGroup.Paint += new System.Windows.Forms.PaintEventHandler(this.MainGroup_Paint);
            // 
            // HeightMaxTextbox
            // 
            this.HeightMaxTextbox.Location = new System.Drawing.Point(430, 63);
            this.HeightMaxTextbox.Name = "HeightMaxTextbox";
            this.HeightMaxTextbox.Size = new System.Drawing.Size(50, 20);
            this.HeightMaxTextbox.TabIndex = 8;
            this.HeightMaxTextbox.Text = "0.00";
            this.HeightMaxTextbox.TextChanged += new System.EventHandler(this.HeightMaxTextbox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(286, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Height Min/Max:";
            // 
            // HeightMinTextbox
            // 
            this.HeightMinTextbox.Location = new System.Drawing.Point(374, 63);
            this.HeightMinTextbox.Name = "HeightMinTextbox";
            this.HeightMinTextbox.Size = new System.Drawing.Size(50, 20);
            this.HeightMinTextbox.TabIndex = 6;
            this.HeightMinTextbox.Text = "0.00";
            this.HeightMinTextbox.TextChanged += new System.EventHandler(this.HeightMinTextbox_TextChanged);
            // 
            // SlopeMaxTextbox
            // 
            this.SlopeMaxTextbox.Location = new System.Drawing.Point(220, 63);
            this.SlopeMaxTextbox.Name = "SlopeMaxTextbox";
            this.SlopeMaxTextbox.Size = new System.Drawing.Size(50, 20);
            this.SlopeMaxTextbox.TabIndex = 5;
            this.SlopeMaxTextbox.Text = "0.00";
            this.SlopeMaxTextbox.TextChanged += new System.EventHandler(this.SlopeMaxTextbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Slope Min/Max:";
            // 
            // SlopeMinTextbox
            // 
            this.SlopeMinTextbox.Location = new System.Drawing.Point(164, 63);
            this.SlopeMinTextbox.Name = "SlopeMinTextbox";
            this.SlopeMinTextbox.Size = new System.Drawing.Size(50, 20);
            this.SlopeMinTextbox.TabIndex = 2;
            this.SlopeMinTextbox.Text = "0.00";
            this.SlopeMinTextbox.TextChanged += new System.EventHandler(this.SlopeMinTextbox_TextChanged);
            // 
            // TexturePathButton
            // 
            this.TexturePathButton.Location = new System.Drawing.Point(452, 19);
            this.TexturePathButton.Name = "TexturePathButton";
            this.TexturePathButton.Size = new System.Drawing.Size(28, 20);
            this.TexturePathButton.TabIndex = 1;
            this.TexturePathButton.Text = "...";
            this.TexturePathButton.UseVisualStyleBackColor = true;
            this.TexturePathButton.Click += new System.EventHandler(this.TexturePathButton_Click);
            // 
            // TexturePanel
            // 
            this.TexturePanel.Location = new System.Drawing.Point(6, 19);
            this.TexturePanel.Name = "TexturePanel";
            this.TexturePanel.Size = new System.Drawing.Size(64, 64);
            this.TexturePanel.TabIndex = 0;
            // 
            // TexturePathLabel
            // 
            this.TexturePathLabel.AutoSize = true;
            this.TexturePathLabel.Location = new System.Drawing.Point(76, 23);
            this.TexturePathLabel.Name = "TexturePathLabel";
            this.TexturePathLabel.Size = new System.Drawing.Size(74, 13);
            this.TexturePathLabel.TabIndex = 3;
            this.TexturePathLabel.Text = "Texture Path: ";
            // 
            // OpenTextureDialog
            // 
            this.OpenTextureDialog.Filter = resources.GetString("OpenTextureDialog.Filter");
            // 
            // AutoTextureNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainGroup);
            this.DoubleBuffered = true;
            this.Name = "AutoTextureNode";
            this.Size = new System.Drawing.Size(490, 96);
            this.MainGroup.ResumeLayout(false);
            this.MainGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox MainGroup;
        private System.Windows.Forms.TextBox SlopeMinTextbox;
        private System.Windows.Forms.Button TexturePathButton;
        private System.Windows.Forms.TextBox HeightMaxTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox HeightMinTextbox;
        private System.Windows.Forms.TextBox SlopeMaxTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label TexturePathLabel;
        public System.Windows.Forms.Panel TexturePanel;
        private System.Windows.Forms.OpenFileDialog OpenTextureDialog;
    }
}
