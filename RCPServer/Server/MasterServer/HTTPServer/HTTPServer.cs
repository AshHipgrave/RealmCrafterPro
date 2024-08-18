using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace RCPServer
{
    public class HTTPServer
    {
        Socket ServerSocket = null;
        Thread ListenThread = null;
        List<HTTPModules.IHTTPModule> Modules = new List<HTTPModules.IHTTPModule>();

        public HTTPServer(string bindIP, int bindPort)
        {
            Modules.Add(new HTTPModules.AdminRoot());
            Modules.Add(new HTTPModules.AdminImage());
            Modules.Add(new HTTPModules.AdminGlobal());
            Modules.Add(new HTTPModules.AdminServers());
            Modules.Add(new HTTPModules.AdminZones());
            Modules.Add(new HTTPModules.LogPlayers());
            Modules.Add(new HTTPModules.LogBandwidth());
            Modules.Add(new HTTPModules.ScriptExceptions());
            Modules.Add(new HTTPModules.LogView());

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            IPEndPoint EP = null;
            if (bindIP.Equals("0.0.0.0"))
            {
                EP = new IPEndPoint(IPAddress.Any, bindPort);
            }
            else
            {
                EP = new IPEndPoint(IPAddress.Parse(bindIP), bindPort);
            }

            ServerSocket.Bind(EP);
            ServerSocket.Listen(1024);

            ListenThread = new Thread(new ThreadStart(ListenUpdate));
            ListenThread.Start();
        }

        public byte[] ProcessModule(string u, Dictionary<string, string> arguments)
        {
            lock (this)
            {
                foreach (HTTPModules.IHTTPModule M in Modules)
                {
                    if(M.CheckRequestURL(u))
                        return M.Process(u, arguments);
                }

                string Mod = "Requested Module '" + u + "' Not Found!";
                StringBuilder Packet = new StringBuilder();
                Packet.Append("HTTP/1.1 200 OK\r\n");
                Packet.Append("Server: RCP/2.50\r\n");
                Packet.Append("Content-Type: text/html\r\n");
                Packet.Append("Cache-Control: no-store, no-cache, must-revalidate\r\n");
                Packet.Append("Pragma: no-cache\r\n");
                Packet.Append("Content-Length: " + Mod.Length.ToString() + "\r\n");
                Packet.Append("\r\n");
                Packet.Append(Mod);

                return ASCIIEncoding.ASCII.GetBytes(Packet.ToString());
            }
        }

        public bool CheckAuth(string user, string pass)
        {
            bool Valid = false;

            lock(this)
            {
                Valid = user.Equals(Server.AdminUsername) && pass.Equals(Server.AdminPassword);
            }

            return Valid;
        }

        void ListenUpdate()
        {
            if (ServerSocket == null)
                return;

            while (true)
            {
                Socket Client = ServerSocket.Accept();
                if (Client == null)
                    continue;

                HTTPClient C = new HTTPClient(this, Client);
                ThreadPool.QueueUserWorkItem(new WaitCallback(C.Process));

                Thread.Sleep(10);
            }
        }

    }
}
