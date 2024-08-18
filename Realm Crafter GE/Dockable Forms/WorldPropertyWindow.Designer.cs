namespace RealmCrafter_GE
{
    public partial class WorldPropertyWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorldPropertyWindow));
            this.ObjectProperties = new System.Windows.Forms.PropertyGrid();
            this.ButtonRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ObjectProperties
            // 
            this.ObjectProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjectProperties.Location = new System.Drawing.Point(0, 0);
            this.ObjectProperties.Name = "ObjectProperties";
            this.ObjectProperties.Size = new System.Drawing.Size(226, 487);
            this.ObjectProperties.TabIndex = 0;
            this.ObjectProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ObjectProperties_PropertyValueChanged);
            // 
            // ButtonRefresh
            // 
            this.ButtonRefresh.Location = new System.Drawing.Point(153, 0);
            this.ButtonRefresh.Name = "ButtonRefresh";
            this.ButtonRefresh.Size = new System.Drawing.Size(61, 23);
            this.ButtonRefresh.TabIndex = 1;
            this.ButtonRefresh.Text = "Refresh";
            this.ButtonRefresh.UseVisualStyleBackColor = true;
            this.ButtonRefresh.Visible = false;
            this.ButtonRefresh.Click += new System.EventHandler(this.ButtonRefresh_Click);
            // 
            // WorldPropertyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 487);
            this.Controls.Add(this.ButtonRefresh);
            this.Controls.Add(this.ObjectProperties);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WorldPropertyWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.ShowInTaskbar = false;
            this.TabText = "Properties";
            this.Text = "Properties";
            this.Load += new System.EventHandler(this.WorldPropertyWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid ObjectProperties;
        public System.Windows.Forms.Button ButtonRefresh;
    }
}