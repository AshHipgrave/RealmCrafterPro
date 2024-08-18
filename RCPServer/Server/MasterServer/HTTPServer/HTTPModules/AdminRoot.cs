using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RCPServer.HTTPModules
{
    public class AdminRoot : IHTTPModule
    {
        string PageHTML = "";

        public AdminRoot()
        {
            StreamReader Reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("RCPServer.EmbResource.root.html"));

            while (!Reader.EndOfStream)
            {
                PageHTML += Reader.ReadLine();
            }

            Reader.Close();

            PageHTML = PageHTML.Replace("$ServerVersion", MasterServer.ApplicationVersion);
        }

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/"))
                return true;
            return false;
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
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
