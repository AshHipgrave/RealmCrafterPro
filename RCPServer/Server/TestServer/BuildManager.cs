using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace TestServer
{
    public partial class BuildManager : Form
    {
        public BuildManager()
        {
            InitializeComponent();

            try
            {
                string MasterProxyName = "";
                XmlTextReader X = new XmlTextReader("Servers.xml");

                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("master", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Address = X.GetAttribute("address");
                        string Port = X.GetAttribute("port");
                        MasterProxyName = X.GetAttribute("masterproxy");
                        string Username = X.GetAttribute("username");
                        string Password = X.GetAttribute("password");

                        MasterAddress.Text = Address;
                        MasterPort.Text = Port;
                        MasterUser.Text = Username;
                        MasterPass.Text = Password;
                    }

                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("proxy", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string PublicIP = X.GetAttribute("publicaddress");
                        string PublicPort = X.GetAttribute("publicport");
                        string PrivateIP = X.GetAttribute("privateaddress");
                        string PrivatePort = X.GetAttribute("privateport");

                        ProxiesGrid.Rows.Add(new object[] { Name, PublicIP, PublicPort, PrivateIP, PrivatePort });
                    }

                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("zone", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string PrivateIP = X.GetAttribute("privateaddress");
                        string PrivatePort = X.GetAttribute("privateport");

                        ZonesGrid.Rows.Add(new object[] { Name, PrivateIP, PrivatePort });
                    }

                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("account", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string PrivateIP = X.GetAttribute("privateaddress");
                        string PrivatePort = X.GetAttribute("privateport");

                        AccountsGrid.Rows.Add(new object[] { Name, PrivateIP, PrivatePort });
                    }

                }

                X.Close();

                MasterProxy.Items.Clear();
                int Index = -1;
                foreach (DataGridViewRow R in ProxiesGrid.Rows)
                {
                    if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                    {
                        if (R.Cells[0].Value.ToString().Equals(MasterProxyName, StringComparison.CurrentCultureIgnoreCase))
                            Index = MasterProxy.Items.Count;
                        MasterProxy.Items.Add(R.Cells[0].Value.ToString());
                    }
                }

                MasterProxy.SelectedIndex = Index;

            }
            catch (System.Exception ex)
            {

            }

            
        }

        private void MasterProxy_Click(object sender, EventArgs e)
        {
            int Index = MasterProxy.SelectedIndex;
            MasterProxy.Items.Clear();

            foreach (DataGridViewRow R in ProxiesGrid.Rows)
            {
                if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                {
                    MasterProxy.Items.Add(R.Cells[0].Value.ToString());
                }
            }

            if (Index < MasterProxy.Items.Count)
                MasterProxy.SelectedIndex = Index;
        }

        string CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Program.CreateDirectory(path);
                return path;
            }

            for (int i = 0; i < 100; ++i)
            {
                if (!Directory.Exists(path + "." + i.ToString()))
                {
                    Program.CreateDirectory(path + "." + i.ToString());
                    return path + "." + i.ToString();
                }
            }

            return "ERROR";
        }

        void WriteConfig(XmlTextWriter x, string name, string val)
        {
            if (val == null)
                val = "";

            x.WriteStartElement("config");
            x.WriteAttributeString("name", name);
            x.WriteAttributeString("value", val);
            x.WriteEndElement();
        }

        // Copy a folder and all its subfolders (RECURSIVE)
        private void CopyTree(string Dir, string DestinationDir)
        {
            // Create destination folder if required
            if (!Directory.Exists(DestinationDir))
            {
                Directory.CreateDirectory(DestinationDir);
            }

            // Copy each file in this folder
            string[] Files = Directory.GetFiles(Dir);
            foreach (string S in Files)
            {
                try
                {
                    File.Copy(S, DestinationDir + @"\" + Path.GetFileName(S), true);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Exception thrown when copying file: " + S + ".\nThis may cause problems when running the built project\n\nException Info:\n" + ex.Message);
                }

            }

            // Recursively copy each subfolder
            string[] Folders = Directory.GetDirectories(Dir);
            foreach (string S in Folders)
            {
                if (S != "." && S != "..")
                {
                    string SubDir = Path.GetFileName(S);
                    CopyTree(Dir + @"\" + SubDir, DestinationDir + @"\" + SubDir);
                }
            }
        }

        void GenerateServer(string path, string exeFilename, string xmlFilename, string machineName, string privateIP, string privatePort, string publicIP, string publicPort, string masterProxyIP, string masterProxyPort, string adminName, string adminPass
            , string masterIP, string masterPort)
        {
            // Create dir
            path = CreateDirectory(Path.Combine(path, machineName));

            // Copy content
            try
            {
                File.Copy(exeFilename, Path.Combine(path, exeFilename));
                File.Copy("ServerShared.dll", Path.Combine(path, "ServerShared.dll"));
                File.Copy("Scripting.dll", Path.Combine(path, "Scripting.dll"));
                File.Copy("RCEnet.dll", Path.Combine(path, "RCENet.dll"));
                File.Copy("librcenet.so", Path.Combine(path, "librcenet.so"));

                Program.CreateDirectory(Path.Combine(path, "Data"));
                Program.CreateDirectory(Path.Combine(Path.Combine(path, "Data"), "Server Data"));

                CopyTree("Data\\Server Data", Path.Combine(Path.Combine(path, "Data"), "Server Data"));   
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            // Generate Xml
            try
            {
                XmlTextWriter X = new XmlTextWriter(Path.Combine(path, xmlFilename), Encoding.ASCII);

                X.Formatting = Formatting.Indented;
                X.WriteStartDocument();
                X.WriteStartElement("configuration");

                WriteConfig(X, "privateaddress", privateIP);
                WriteConfig(X, "privateport", privatePort);
                WriteConfig(X, "publicaddress", publicIP);
                WriteConfig(X, "publicport", publicPort);
                WriteConfig(X, "masteraddress", masterIP);
                WriteConfig(X, "masterport", masterPort);
                WriteConfig(X, "masterproxyaddress", masterProxyIP);
                WriteConfig(X, "masterproxyport", masterProxyPort);
                WriteConfig(X, "adminusername", adminName);
                WriteConfig(X, "adminpassword", adminPass);


                X.WriteEndElement();
                X.WriteEndDocument();
                X.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BuildBases(string path)
        {
            string[] SubDir = Directory.GetDirectories(path);

            try
            {
                foreach (string Dir in SubDir)
                    Directory.Delete(Dir, true);
            }
            catch (System.Exception ex)
            {

            }

            string MasterProxyIP = "";
            string MasterProxyPort = "";
            string MasterProxyName = (MasterProxy.SelectedItem != null) ? MasterProxy.SelectedItem.ToString() : "";

            // Get Master Proxy details
            foreach (DataGridViewRow R in ProxiesGrid.Rows)
            {
                if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                {
                    if (R.Cells[0].Value.ToString().Equals(MasterProxyName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        MasterProxyIP = R.Cells[1].Value.ToString();
                        MasterProxyPort = R.Cells[2].Value.ToString();

                        break;
                    }
                }
            }

            // No master? throw error
            if(MasterProxyIP.Length == 0 || MasterProxyPort.Length == 0)
                throw new Exception("Master Proxy not set or not found! Cannot build cluster.");

            GenerateServer(path, "MasterServer.exe", "MasterServer.xml", "MasterServer", MasterAddress.Text, MasterPort.Text,
                "", "", MasterProxyIP, MasterProxyPort, MasterUser.Text, MasterPass.Text, MasterAddress.Text, MasterPort.Text);

            foreach (DataGridViewRow R in ProxiesGrid.Rows)
            {
                if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                {
                    GenerateServer(path, "ProxyServer.exe", "ProxyServer.xml", R.Cells[0].Value.ToString(), R.Cells[3].Value.ToString(), R.Cells[4].Value.ToString(),
                        R.Cells[1].Value.ToString(), R.Cells[2].Value.ToString(), MasterProxyIP, MasterProxyPort, "", "", MasterAddress.Text, MasterPort.Text);
                }
            }

            foreach (DataGridViewRow R in ZonesGrid.Rows)
            {
                if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                {
                    GenerateServer(path, "ZoneServer.exe", "ZoneServer.xml", R.Cells[0].Value.ToString(), R.Cells[1].Value.ToString(), R.Cells[2].Value.ToString(),
                        "", "", "", "", "", "", MasterAddress.Text, MasterPort.Text);
                }
            }

            foreach (DataGridViewRow R in AccountsGrid.Rows)
            {
                if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                {
                    GenerateServer(path, "AccountServer.exe", "AccountServer.xml", R.Cells[0].Value.ToString(), R.Cells[1].Value.ToString(), R.Cells[2].Value.ToString(),
                        "", "", "", "", "", "", MasterAddress.Text, MasterPort.Text);
                }
            }


        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {

                XmlTextWriter X = new XmlTextWriter("Servers.xml", Encoding.ASCII);

                X.Formatting = Formatting.Indented;
                X.WriteStartDocument();
                X.WriteStartElement("servers");

                X.WriteStartElement("master");
                X.WriteAttributeString("address", MasterAddress.Text);
                X.WriteAttributeString("port", MasterPort.Text);
                X.WriteAttributeString("masterproxy", MasterProxy.SelectedItem.ToString());
                X.WriteAttributeString("username", MasterUser.Text);
                X.WriteAttributeString("password", MasterPass.Text);
                X.WriteEndElement();

                foreach (DataGridViewRow R in ProxiesGrid.Rows)
                {
                    if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                    {
                        X.WriteStartElement("proxy");
                        X.WriteAttributeString("name", R.Cells[0].Value.ToString());
                        X.WriteAttributeString("publicaddress", R.Cells[1].Value.ToString());
                        X.WriteAttributeString("publicport", R.Cells[2].Value.ToString());
                        X.WriteAttributeString("privateaddress", R.Cells[3].Value.ToString());
                        X.WriteAttributeString("privateport", R.Cells[4].Value.ToString());
                        X.WriteEndElement();
                    }
                }

                foreach (DataGridViewRow R in ZonesGrid.Rows)
                {
                    if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                    {
                        X.WriteStartElement("zone");
                        X.WriteAttributeString("name", R.Cells[0].Value.ToString());
                        X.WriteAttributeString("privateaddress", R.Cells[1].Value.ToString());
                        X.WriteAttributeString("privateport", R.Cells[2].Value.ToString());
                        X.WriteEndElement();
                    }
                }

                foreach (DataGridViewRow R in AccountsGrid.Rows)
                {
                    if (R.Cells[0].Value != null && (R.Cells[0].Value as string).Length > 0)
                    {
                        X.WriteStartElement("account");
                        X.WriteAttributeString("name", R.Cells[0].Value.ToString());
                        X.WriteAttributeString("privateaddress", R.Cells[1].Value.ToString());
                        X.WriteAttributeString("privateport", R.Cells[2].Value.ToString());
                        X.WriteEndElement();
                    }
                }


                X.WriteEndElement();
                X.WriteEndDocument();
                X.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            DialogResult = DialogResult.OK;
        }



        private void BuildManager_Load(object sender, EventArgs e)
        {

        }
    }
}