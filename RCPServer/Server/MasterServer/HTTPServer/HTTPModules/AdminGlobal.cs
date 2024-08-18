using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RCPServer.HTTPModules
{
    public class AdminGlobal : IHTTPModule
    {
        string PageHTML = "";
        DateTime StartTime;
        public static int ServerCount = 0;
        public static int PlayerCount = 0;
        public static int ZoneCount = 0;
        public static int LoadLevel = 0;
        public static int Warnings = 0;
        public static bool RebalanceRequest = false;
        public static string HTMLLog = "";

        public AdminGlobal()
        {
            StreamReader Reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("RCPServer.EmbResource.global.html"));

            while (!Reader.EndOfStream)
            {
                PageHTML += Reader.ReadLine();
            }

            Reader.Close();

            StartTime = DateTime.Now;
        }

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/global"))
                return true;
            return false;
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
            string HTML = PageHTML;
            TimeSpan TimeDiff = (DateTime.Now - StartTime);
            HTML = HTML.Replace("$Uptime", string.Format("{0} Days {1:D2}:{2:D2}:{3:D2}", new object[] { 
                ((int)TimeDiff.TotalDays).ToString(), TimeDiff.Hours, TimeDiff.Minutes, TimeDiff.Seconds }));

            string LoadStr = "<span class=\"LowLoad\">Low</span>"; 

            lock (typeof(AdminGlobal))
            {
                HTML = HTML.Replace("$Servers", ServerCount.ToString());
                HTML = HTML.Replace("$Players", PlayerCount.ToString());
                HTML = HTML.Replace("$Zones", ZoneCount.ToString());


                
                if (LoadLevel == 1)
                    LoadStr = "<span class=\"MidLoad\">Medium</span>";
                else if (LoadLevel == 2)
                    LoadStr = "<span class=\"HighLoad\">High</span>";

                if (Warnings > 0)
                    LoadStr += "&nbsp&nbsp<span class=\"Warnings\">" + Warnings.ToString() + " Warnings</span>";

            }

            HTML = HTML.Replace("$Load", LoadStr);

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
