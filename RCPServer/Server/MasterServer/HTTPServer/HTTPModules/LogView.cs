using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using Graph;

namespace RCPServer.HTTPModules
{
    public class LogView : IHTTPModule
    {

        public LogView()
        {

        }

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/logview"))
                return true;
            return false;
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
            string PageHTML = "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" valign=\"top\" style=\"padding-left: 50px;\"><br /><br /><br /><br /><table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 700px;\"><tr><td style=\"background-image: url('/img/menugrad.jpg'); height: 21px; padding-left: 10px; border-top: 2px solid #DC1300; border-left: 1px solid #DC1300; border-bottom: 1px solid #390909; border-right: 1px solid #390909;\" valign=\"middle\" class=\"VertMenuTitle\"><b>Event Log</b></td></tr>";
            PageHTML += "<tr><td style=\"border: 1px solid #000000; padding: 5px; font-family: 'courier new'; background-color: #000000; color: #ffffff;\">";

            lock (typeof(AdminGlobal))
            {
                PageHTML += AdminGlobal.HTMLLog;
            }

            PageHTML += "</td></tr></table></td></tr></table>";

            StringBuilder Packet = new StringBuilder();
            Packet.Append("HTTP/1.1 200 OK\r\n");
            Packet.Append("Server: RCP/2.50\r\n");
            Packet.Append("Content-Type: text/html\r\n");
            Packet.Append("Content-Length: " + PageHTML.Length.ToString() + "\r\n");
            Packet.Append("\r\n");
            Packet.Append(PageHTML);

            return ASCIIEncoding.ASCII.GetBytes(Packet.ToString());
        }
    }
}
