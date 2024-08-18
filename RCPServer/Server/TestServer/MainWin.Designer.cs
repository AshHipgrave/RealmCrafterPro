namespace TestServer
{
    partial class MainWin
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStartButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBrowserOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolExitButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.MasterTab = new System.Windows.Forms.TabPage();
            this.MasterHTML = new System.Windows.Forms.WebBrowser();
            this.AccountTab = new System.Windows.Forms.TabPage();
            this.AccountHTML = new System.Windows.Forms.WebBrowser();
            this.ProxyTab = new System.Windows.Forms.TabPage();
            this.ProxyHTML = new System.Windows.Forms.WebBrowser();
            this.ZoneTab = new System.Windows.Forms.TabPage();
            this.ZoneHTML = new System.Windows.Forms.WebBrowser();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.ShutdownTimer = new System.Windows.Forms.Timer(this.components);
            this.stopSoftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.softToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayMenu.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.MasterTab.SuspendLayout();
            this.AccountTab.SuspendLayout();
            this.ProxyTab.SuspendLayout();
            this.ZoneTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayMenu;
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "Realm Crafter Professional Testing Server";
            this.TrayIcon.Visible = true;
            this.TrayIcon.DoubleClick += new System.EventHandler(this.TrayIcon_DoubleClick);
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopSoftToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripSeparator1,
            this.openConsoleToolStripMenuItem,
            this.openBrowserToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(153, 170);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopToolStripMenuItem.Text = "Stop (Hard)";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // openConsoleToolStripMenuItem
            // 
            this.openConsoleToolStripMenuItem.Name = "openConsoleToolStripMenuItem";
            this.openConsoleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openConsoleToolStripMenuItem.Text = "Open Console";
            this.openConsoleToolStripMenuItem.Click += new System.EventHandler(this.openConsoleToolStripMenuItem_Click);
            // 
            // openBrowserToolStripMenuItem
            // 
            this.openBrowserToolStripMenuItem.Name = "openBrowserToolStripMenuItem";
            this.openBrowserToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openBrowserToolStripMenuItem.Text = "Open Browser";
            this.openBrowserToolStripMenuItem.Click += new System.EventHandler(this.openBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStartButton,
            this.toolStripDropDownButton1,
            this.toolStripSeparator3,
            this.toolBrowserOpen,
            this.toolStripSeparator4,
            this.toolExitButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(606, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStartButton
            // 
            this.toolStartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStartButton.Image = global::TestServer.Properties.Resources.PlayIcon;
            this.toolStartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStartButton.Name = "toolStartButton";
            this.toolStartButton.Size = new System.Drawing.Size(23, 22);
            this.toolStartButton.Text = "Start";
            this.toolStartButton.Click += new System.EventHandler(this.toolStartButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBrowserOpen
            // 
            this.toolBrowserOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBrowserOpen.Image = global::TestServer.Properties.Resources.BrowseIcon;
            this.toolBrowserOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBrowserOpen.Name = "toolBrowserOpen";
            this.toolBrowserOpen.Size = new System.Drawing.Size(23, 22);
            this.toolBrowserOpen.Text = "Open Browser";
            this.toolBrowserOpen.Click += new System.EventHandler(this.toolBrowserOpen_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolExitButton
            // 
            this.toolExitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolExitButton.Image = ((System.Drawing.Image)(resources.GetObject("toolExitButton.Image")));
            this.toolExitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolExitButton.Name = "toolExitButton";
            this.toolExitButton.Size = new System.Drawing.Size(29, 22);
            this.toolExitButton.Text = "Exit";
            this.toolExitButton.Click += new System.EventHandler(this.toolExitButton_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.MasterTab);
            this.tabControl.Controls.Add(this.AccountTab);
            this.tabControl.Controls.Add(this.ProxyTab);
            this.tabControl.Controls.Add(this.ZoneTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 25);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(606, 314);
            this.tabControl.TabIndex = 2;
            // 
            // MasterTab
            // 
            this.MasterTab.Controls.Add(this.MasterHTML);
            this.MasterTab.Location = new System.Drawing.Point(4, 22);
            this.MasterTab.Name = "MasterTab";
            this.MasterTab.Padding = new System.Windows.Forms.Padding(3);
            this.MasterTab.Size = new System.Drawing.Size(598, 288);
            this.MasterTab.TabIndex = 0;
            this.MasterTab.Text = "Master Server";
            this.MasterTab.UseVisualStyleBackColor = true;
            // 
            // MasterHTML
            // 
            this.MasterHTML.AllowWebBrowserDrop = false;
            this.MasterHTML.IsWebBrowserContextMenuEnabled = false;
            this.MasterHTML.Location = new System.Drawing.Point(0, 0);
            this.MasterHTML.Name = "MasterHTML";
            this.MasterHTML.Size = new System.Drawing.Size(598, 290);
            this.MasterHTML.TabIndex = 1;
            // 
            // AccountTab
            // 
            this.AccountTab.Controls.Add(this.AccountHTML);
            this.AccountTab.Location = new System.Drawing.Point(4, 22);
            this.AccountTab.Name = "AccountTab";
            this.AccountTab.Padding = new System.Windows.Forms.Padding(3);
            this.AccountTab.Size = new System.Drawing.Size(598, 288);
            this.AccountTab.TabIndex = 1;
            this.AccountTab.Text = "Account Server";
            this.AccountTab.UseVisualStyleBackColor = true;
            // 
            // AccountHTML
            // 
            this.AccountHTML.AllowWebBrowserDrop = false;
            this.AccountHTML.IsWebBrowserContextMenuEnabled = false;
            this.AccountHTML.Location = new System.Drawing.Point(0, 0);
            this.AccountHTML.Name = "AccountHTML";
            this.AccountHTML.Size = new System.Drawing.Size(598, 290);
            this.AccountHTML.TabIndex = 1;
            // 
            // ProxyTab
            // 
            this.ProxyTab.Controls.Add(this.ProxyHTML);
            this.ProxyTab.Location = new System.Drawing.Point(4, 22);
            this.ProxyTab.Name = "ProxyTab";
            this.ProxyTab.Padding = new System.Windows.Forms.Padding(3);
            this.ProxyTab.Size = new System.Drawing.Size(598, 288);
            this.ProxyTab.TabIndex = 2;
            this.ProxyTab.Text = "Proxy Server";
            this.ProxyTab.UseVisualStyleBackColor = true;
            // 
            // ProxyHTML
            // 
            this.ProxyHTML.AllowWebBrowserDrop = false;
            this.ProxyHTML.IsWebBrowserContextMenuEnabled = false;
            this.ProxyHTML.Location = new System.Drawing.Point(0, 0);
            this.ProxyHTML.Name = "ProxyHTML";
            this.ProxyHTML.Size = new System.Drawing.Size(598, 290);
            this.ProxyHTML.TabIndex = 1;
            // 
            // ZoneTab
            // 
            this.ZoneTab.Controls.Add(this.ZoneHTML);
            this.ZoneTab.Location = new System.Drawing.Point(4, 22);
            this.ZoneTab.Name = "ZoneTab";
            this.ZoneTab.Padding = new System.Windows.Forms.Padding(3);
            this.ZoneTab.Size = new System.Drawing.Size(598, 288);
            this.ZoneTab.TabIndex = 3;
            this.ZoneTab.Text = "Zone Server";
            this.ZoneTab.UseVisualStyleBackColor = true;
            // 
            // ZoneHTML
            // 
            this.ZoneHTML.AllowWebBrowserDrop = false;
            this.ZoneHTML.IsWebBrowserContextMenuEnabled = false;
            this.ZoneHTML.Location = new System.Drawing.Point(0, 0);
            this.ZoneHTML.Name = "ZoneHTML";
            this.ZoneHTML.Size = new System.Drawing.Size(598, 290);
            this.ZoneHTML.TabIndex = 0;
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // ShutdownTimer
            // 
            this.ShutdownTimer.Interval = 11000;
            this.ShutdownTimer.Tick += new System.EventHandler(this.ShutdownTimer_Tick);
            // 
            // stopSoftToolStripMenuItem
            // 
            this.stopSoftToolStripMenuItem.Name = "stopSoftToolStripMenuItem";
            this.stopSoftToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopSoftToolStripMenuItem.Text = "Stop (Soft)";
            this.stopSoftToolStripMenuItem.Click += new System.EventHandler(this.stopSoftToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.softToolStripMenuItem,
            this.hardToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::TestServer.Properties.Resources.StopIcon;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // softToolStripMenuItem
            // 
            this.softToolStripMenuItem.Name = "softToolStripMenuItem";
            this.softToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.softToolStripMenuItem.Text = "Soft";
            this.softToolStripMenuItem.Click += new System.EventHandler(this.softToolStripMenuItem_Click);
            // 
            // hardToolStripMenuItem
            // 
            this.hardToolStripMenuItem.Name = "hardToolStripMenuItem";
            this.hardToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hardToolStripMenuItem.Text = "Hard";
            this.hardToolStripMenuItem.Click += new System.EventHandler(this.hardToolStripMenuItem_Click);
            // 
            // MainWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 339);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWin";
            this.Text = "Realm Crafter Professional Testing Server";
            this.Load += new System.EventHandler(this.MainWin_Load);
            this.Resize += new System.EventHandler(this.MainWin_Resize);
            this.TrayMenu.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.MasterTab.ResumeLayout(false);
            this.AccountTab.ResumeLayout(false);
            this.ProxyTab.ResumeLayout(false);
            this.ZoneTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage MasterTab;
        private System.Windows.Forms.TabPage AccountTab;
        private System.Windows.Forms.TabPage ProxyTab;
        private System.Windows.Forms.TabPage ZoneTab;
        private System.Windows.Forms.WebBrowser ZoneHTML;
        private System.Windows.Forms.WebBrowser MasterHTML;
        private System.Windows.Forms.WebBrowser AccountHTML;
        private System.Windows.Forms.WebBrowser ProxyHTML;
        private System.Windows.Forms.ToolStripButton toolStartButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolBrowserOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolExitButton;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Timer ShutdownTimer;
        private System.Windows.Forms.ToolStripMenuItem stopSoftToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem softToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardToolStripMenuItem;
    }
}

