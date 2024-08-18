using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using RCPServer;
using Scripting;

namespace TestServer
{
    public partial class MainWin : Form
    {
        string HTMLHeader = "<html><body onload=\"window.scrollBy(0, 100000);\" style=\"padding: 0px; margin: 0px; background-color: #000000; color: #ffffff; font-family: 'courier new'; font-size: 12px;\">";
        string HTMLFooter = "</body></html>";
        int Connection = 0;
        Process MasterServer, AccountServer, ProxyServer, ZoneServer;
        bool running = false;
        int MasterConnection = 0;

        string TitleText = "Realm Crafter Professional Testing Server";

        public MainWin()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            InitializeComponent();
            Running = false;

            // Process information
            MasterServer = new Process();
            MasterServer.StartInfo.FileName = Environment.CurrentDirectory + "\\MasterServer.exe";
            MasterServer.StartInfo.Arguments = "-testingServer";

            AccountServer = new Process();
            AccountServer.StartInfo.FileName = Environment.CurrentDirectory + "\\AccountServer.exe";
            AccountServer.StartInfo.Arguments = "-testingServer";

            ProxyServer = new Process();
            ProxyServer.StartInfo.FileName = Environment.CurrentDirectory + "\\ProxyServer.exe";
            ProxyServer.StartInfo.Arguments = "-testingServer";

            ZoneServer = new Process();
            ZoneServer.StartInfo.FileName = Environment.CurrentDirectory + "\\ZoneServer.exe";
            ZoneServer.StartInfo.Arguments = "-testingServer";

            // Recolor default HTMLViews to 'black'
            MasterHTML.DocumentText = HTMLHeader + HTMLFooter;
            AccountHTML.DocumentText = HTMLHeader + HTMLFooter;
            ProxyHTML.DocumentText = HTMLHeader + HTMLFooter;
            ZoneHTML.DocumentText = HTMLHeader + HTMLFooter;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Running && ShutdownTimer.Enabled)
            {
                MessageBox.Show("Cannot close the server, it is already shutting down!");
                e.Cancel = true;
                return;
            }

            // Give cancel option if servers are running
            if (Running && MessageBox.Show("The servers are still running, are you sure you want to exit?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            // If the application must close for any reason, force the servers to stop since they are invisible
            StopServers(true);
        }

        private void MainWin_Resize(object sender, EventArgs e)
        {
            // Minimized? Hide taskbar icon
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                this.ShowInTaskbar = true;
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            // Tray double clicked, open
            this.ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void MainWin_Load(object sender, EventArgs e)
        {
            // Setup debugging connection (just to get log messaged out of the server)
            Connection = RCEnet.StartHost(24999, 10);
            if (Connection == 0)
            {
                MessageBox.Show("Error: Could not start testing server (maybe it is already open?)");
                Application.Exit();
            }
            else
                UpdateTimer.Start();

//             // Minimize ourselves
//             WindowState = FormWindowState.Minimized;
//             this.ShowInTaskbar = false;

            // Alert our presence
            //TrayIcon.ShowBalloonTip(4000, "Realm Crafter Testing Server", "Server is offline, right click this tray icon for options.", ToolTipIcon.Info);
        }

        public void FindAndHide(string name)
        {
            // Use external API to find and hide window
            IntPtr Hwnd = Program.FindWindow(null, name);
            if (Hwnd != IntPtr.Zero)
                Program.ShowWindow(Hwnd, 0);
        }

        public void StartServers()
        {
            // Cancel if already running
            if (Running == true)
                return;

            // Start applications
            if (MasterServer != null)
                MasterServer.Start();
            System.Threading.Thread.Sleep(2000);
            if (AccountServer != null)
                AccountServer.Start();
            if (ZoneServer != null)
                ZoneServer.Start(); 
            if (ProxyServer != null)
                ProxyServer.Start();

            // Wait a second so they can rename themselves
            System.Threading.Thread.Sleep(2000);

            // Hide windows
            FindAndHide("MASTER");
            FindAndHide("ACCOUNT");
            FindAndHide("ZONE");
            FindAndHide("PROXY");

            // This updates the GUI as well
            Running = true;

            // Infos
            //TrayIcon.ShowBalloonTip(2000, "Realm Crafter Testing Server", "Server Started\r\nAdmin Console: 127.0.0.1 (admin/password)", ToolTipIcon.Info);
        }

        public void StopServers(bool hard)
        {
            // Don't bother if they are closed
            if (Running == false)
                return;

            if (hard)
            {
                ShutdownTimer_Tick(null, null);
                return;
            }

            // Send message
            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)'S');

            RCEnet.Send(MasterConnection, MessageTypes.P_DebugMessage, Pa.ToArray(), true);

            Text = TitleText + " [Stopping Server]";
            stopToolStripMenuItem.Enabled = false;
            stopSoftToolStripMenuItem.Enabled = false;
            softToolStripMenuItem.Enabled = false;
            hardToolStripMenuItem.Enabled = false;
            openBrowserToolStripMenuItem.Enabled = false;
            toolBrowserOpen.Enabled = false;
            ShutdownTimer.Start();
        }

        public bool Running
        {
            get { return running; }

            set
            {
                running = value;

                // Update GUI
                startToolStripMenuItem.Enabled = !running;
                stopToolStripMenuItem.Enabled = running;
                stopSoftToolStripMenuItem.Enabled = running;
                softToolStripMenuItem.Enabled = running;
                hardToolStripMenuItem.Enabled = running;
                openBrowserToolStripMenuItem.Enabled = running;
                toolStartButton.Enabled = !running;
                toolBrowserOpen.Enabled = running;
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Start
            StartServers();
        }



        private void openConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open
            TrayIcon_DoubleClick(sender, e);
        }

        private void openBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open interwebz
            Process.Start("http://127.0.0.1:400/");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check first
            if (Running && MessageBox.Show("The servers are still running, are you sure you want to exit?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // Nope, exit!
            Application.Exit();
        }

        private void toolStartButton_Click(object sender, EventArgs e)
        {
            startToolStripMenuItem_Click(sender, e);
        }

//         private void toolStopButton_Click(object sender, EventArgs e)
//         {
//             stopToolStripMenuItem_Click(sender, e);
//         }

        private void toolBrowserOpen_Click(object sender, EventArgs e)
        {
            openBrowserToolStripMenuItem_Click(sender, e);
        }

        private void toolExitButton_Click(object sender, EventArgs e)
        {
            exitToolStripMenuItem_Click(sender, e);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Connection != 0)
            {
                RCEnet.Update();

                // Incoming messages
                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    if (M.MessageData != null)
                    {
                        switch (M.MessageType)
                        {
                            case MessageTypes.P_ServerStat:
                                {
                                    // Get text
                                    byte ServerType = M.MessageData.ReadByte();
                                    string HTMLText = M.MessageData.ReadString();

                                    // Apply HTML
                                    if (ServerType == (byte)'M')
                                    {
                                        MasterHTML.DocumentText = HTMLHeader + HTMLText + HTMLFooter;
                                        MasterConnection = M.FromID;
                                    }
                                    if (ServerType == (byte)'A')
                                        AccountHTML.DocumentText = HTMLHeader + HTMLText + HTMLFooter;
                                    if (ServerType == (byte)'P')
                                        ProxyHTML.DocumentText = HTMLHeader + HTMLText + HTMLFooter;
                                    if (ServerType == (byte)'Z')
                                        ZoneHTML.DocumentText = HTMLHeader + HTMLText + HTMLFooter;
                                    break;
                                }
                        }
                    }

                    RCE_Message.Delete(M);
                }
                RCE_Message.Clean();
            } // Connection != 0

        }

        private void ShutdownTimer_Tick(object sender, EventArgs e)
        {
            ShutdownTimer.Stop();

            // This won't catch anything, but helps if one server shutdown due to an exception
            try
            {
                if (MasterServer != null)
                    MasterServer.Kill();
            }
            catch (System.Exception ex)
            {

            }

            try
            {
                if (AccountServer != null)
                    AccountServer.Kill();
            }
            catch (System.Exception ex)
            {

            }

            try
            {
                if (ZoneServer != null)
                    ZoneServer.Kill();
            }
            catch (System.Exception ex)
            {

            }

            try
            {
                if (ProxyServer != null)
                    ProxyServer.Kill();
            }
            catch (System.Exception ex)
            {

            }

            // Wait and update GUI
            System.Threading.Thread.Sleep(1000);
            //TrayIcon.ShowBalloonTip(2000, "Realm Crafter Testing Server", "Server Stopped", ToolTipIcon.Info);
            Running = false;
            Text = TitleText;
        }

        private void softToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopSoftToolStripMenuItem_Click(sender, e);
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopToolStripMenuItem_Click(sender, e);
        }

        private void stopSoftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stop soft
            StopServers(false);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stop
            StopServers(true);
        }
    }
}