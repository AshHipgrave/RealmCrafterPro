using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace RCPServer.HTTPModules
{
    public class AdminImage : IHTTPModule
    {
        
        public AdminImage()
        {
        }

        public bool CheckRequestURL(string c)
        {
            if (c.Length > 5 && c.Substring(0, 5).Equals("/img/"))
                return true;
            return false;
        }

        private byte[] Return404()
        {
            StringBuilder Pkt = new StringBuilder();
            Pkt.Append("HTTP/1.1 504 Not Found\r\n");
            Pkt.Append("Server: RCP/2.50\r\n");
            Pkt.Append("Content-Type: text/html\r\n");
            Pkt.Append("Content-Length: 9\r\n");
            Pkt.Append("\r\n");
            Pkt.Append("Not Found");

            return ASCIIEncoding.ASCII.GetBytes(Pkt.ToString());
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
            u = u.Substring(5);

            Stream S = Assembly.GetExecutingAssembly().GetManifestResourceStream("RCPServer.EmbResource." + u);
            if (S == null)
                return Return404();

            StringBuilder Packet = new StringBuilder();
            Packet.Append("HTTP/1.0 200 OK\r\n");
            Packet.Append("Server: RCP/2.50\r\n");
            if (Path.GetExtension(u).Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                Packet.Append("Content-Type: image/png\r\n");
            if (Path.GetExtension(u).Equals(".gif", StringComparison.CurrentCultureIgnoreCase))
                Packet.Append("Content-Type: image/gif\r\n");
            if (Path.GetExtension(u).Equals(".jpg", StringComparison.CurrentCultureIgnoreCase))
                Packet.Append("Content-Type: image/jpeg\r\n");
            Packet.Append("Connection: keep-alive\r\n");
            Packet.Append("Content-Length: " + S.Length.ToString() + "\r\n");
            Packet.Append("\r\n");

            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(Packet.ToString());
            byte[] FullStr = new byte[S.Length + Buffer.Length];
            Buffer.CopyTo(FullStr, 0);

            S.Read(FullStr, Buffer.Length, (int)S.Length);

            return FullStr;
        }
    }
}
