using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RCPServer.HTTPModules
{
    public class AdminZones : IHTTPModule
    {

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/zones"))
                return true;
            return false;
        }

        private void DumpList(AdminServers.WorldServerInstance wsi, bool dumpAll, ref int Idx, ref string HTML)
        {
            lock (AdminServers.ZoneInstance.Instances)
            {
                foreach (AdminServers.ZoneInstance W in AdminServers.ZoneInstance.Instances)
                {
                    if (dumpAll || W.Server == wsi)
                    {
                        string ServerName = "Unknown";
                        if (W.Server != null)
                            ServerName = W.Server.HostName;

                        HTML += string.Format("<tr class=\"onlineRow\" id=\"d{0}_r{1}\"><td onclick=\"SelectRow({0}, {1});\">{2}</td><td onclick=\"SelectRow({0}, {1});\">{3}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}\">{4}</td><td onclick=\"SelectRow({0}, {1});\">{5}</td><td onclick=\"SelectRow({0}, {1});\">&nbsp;</td><td onclick=\"SelectRow({0}, {1});\">&nbsp;</td></tr>",
                            new string[] { "3", Idx.ToString(), W.ZoneName,
                        W.InstanceID.ToString(), ServerName, W.ClientCount.ToString() });
                        ++Idx;
                    }
                }
            }
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
            string HTML = "err";
            AdminServers.WorldServerInstance SelZone = null;

            if (args.ContainsKey("rebal"))
            {
                HTML = "JScontextSwitch('global');|Loading...";

                // Rebalance
                lock (typeof(HTTPModules.AdminGlobal))
                {
                    HTTPModules.AdminGlobal.RebalanceRequest = true;
                }
            }
            else
            {
                int SID = 0;

                if (args.ContainsKey("sid"))
                {
                    SID = Convert.ToInt32(args["sid"]);

                    lock (AdminServers.WorldServers)
                    {
                        if (SID >= 0 && SID < AdminServers.WorldServers.Count)
                        {
                            SelZone = AdminServers.WorldServers[SID];
                        }
                    }

                }


                HTML = "";

                if (SelZone != null)
                    HTML += "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" valign=\"top\" style=\"padding-left: 50px;\"><br /><br /><br /><br />Zone Instances for " + SelZone.HostName + ":&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"button\" value=\"Return to Properties\" onclick=\"contextSwitch('/servers?s=w&propid=" + SID + "')\" /></br>";
                else
                    HTML += "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" valign=\"top\" style=\"padding-left: 50px;\"><br /><br /><br /><br />Zone Instances:&nbsp;&nbsp;&nbsp;</br>";

                HTML += "<div id=\"tableContainer\" class=\"tableContainerLG\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"scrollTable\"><thead class=\"fixedHeader\"><tr><th><a href=\"#\">Zone Name</a></th><th><a href=\"#\">Instance ID</a></th><th><a href=\"#\">Host Server</a></th><th><a href=\"#\">Clients</a></th><th><a href=\"#\">&nbsp;</a></th><th>&nbsp;</th></tr></thead><tbody id=\"tableContainer_scrollContent\" class=\"scrollContentLG\">";

                int Idx = 0;

                if (SelZone != null)
                {
                    DumpList(SelZone, false, ref Idx, ref HTML);
                }
                else
                {
                    DumpList(null, true, ref Idx, ref HTML);
                }

                HTML += "</tbody></table></div></td></tr></table><br/><br/><script language=\"javascript\"></script>";
            }

            StringBuilder Packet = new StringBuilder();
            Packet.Append("HTTP/1.1 200 OK\r\n");
            Packet.Append("Server: RCP/2.50\r\n");
            Packet.Append("Content-Type: text/html\r\n");
            Packet.Append("Content-Length: " + HTML.Length.ToString() + "\r\n");
            Packet.Append("\r\n");
            Packet.Append(HTML);

            return ASCIIEncoding.ASCII.GetBytes(Packet.ToString());
        }
    }
}
