namespace RealmCrafter_GE
{
    partial class InterfaceGuiElements
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
            this.interfaceGuiElementsFormGroupBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.guiElementsTexturesGuiButton = new System.Windows.Forms.Button();
            this.guiElementsTexturesButton = new System.Windows.Forms.Button();
            this.interfaceGuiElementsFormGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // interfaceGuiElementsFormGroupBox
            // 
            this.interfaceGuiElementsFormGroupBox.Controls.Add(this.label3);
            this.interfaceGuiElementsFormGroupBox.Controls.Add(this.label2);
            this.interfaceGuiElementsFormGroupBox.Controls.Add(this.guiElementsTexturesGuiButton);
            this.interfaceGuiElementsFormGroupBox.Controls.Add(this.guiElementsTexturesButton);
            this.interfaceGuiElementsFormGroupBox.Location = new System.Drawing.Point(3, 3);
            this.interfaceGuiElementsFormGroupBox.Name = "interfaceGuiElementsFormGroupBox";
            this.interfaceGuiElementsFormGroupBox.Size = new System.Drawing.Size(287, 76);
            this.interfaceGuiElementsFormGroupBox.TabIndex = 0;
            this.interfaceGuiElementsFormGroupBox.TabStop = false;
            this.interfaceGuiElementsFormGroupBox.Text = "View GUI Element Files";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(204, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Game Window: Actionbar, Inventory Slots";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Chat bubble, Compass, Compass Overlay";
            // 
            // guiElementsTexturesGuiButton
            // 
            this.guiElementsTexturesGuiButton.Location = new System.Drawing.Point(245, 48);
            this.guiElementsTexturesGuiButton.Name = "guiElementsTexturesGuiButton";
            this.guiElementsTexturesGuiButton.Size = new System.Drawing.Size(32, 23);
            this.guiElementsTexturesGuiButton.TabIndex = 2;
            this.guiElementsTexturesGuiButton.Text = "...";
            this.guiElementsTexturesGuiButton.UseVisualStyleBackColor = true;
            this.guiElementsTexturesGuiButton.Click += new System.EventHandler(this.guiElementsTexturesGuiButton_Click);
            // 
            // guiElementsTexturesButton
            // 
            this.guiElementsTexturesButton.Location = new System.Drawing.Point(245, 19);
            this.guiElementsTexturesButton.Name = "guiElementsTexturesButton";
            this.guiElementsTexturesButton.Size = new System.Drawing.Size(32, 23);
            this.guiElementsTexturesButton.TabIndex = 1;
            this.guiElementsTexturesButton.Text = "...";
            this.guiElementsTexturesButton.UseVisualStyleBackColor = true;
            this.guiElementsTexturesButton.Click += new System.EventHandler(this.guiElementsTexturesButton_Click);
            // 
            // InterfaceGuiElements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 82);
            this.Controls.Add(this.interfaceGuiElementsFormGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InterfaceGuiElements";
            this.Text = "GUI Elements";
            this.interfaceGuiElementsFormGroupBox.ResumeLayout(false);
            this.interfaceGuiElementsFormGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox interfaceGuiElementsFormGroupBox;
        private System.Windows.Forms.Button guiElementsTexturesGuiButton;
        private System.Windows.Forms.Button guiElementsTexturesButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}