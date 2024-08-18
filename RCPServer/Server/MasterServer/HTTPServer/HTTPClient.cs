using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace RCPServer
{
    public class HTTPClient
    {
        HTTPServer Host = null;
        Socket Client = null;
        Dictionary<string, string> Headers = new Dictionary<string, string>();

        public HTTPClient(HTTPServer server, Socket client)
        {
            Host = server;
            Client = client;
        }

        public void Process(object state)
        {
            byte[] Buffer = new byte[1024];
            byte[] ExtraBytes = new byte[0];
            bool ReadingHeaders = true;

            while (ReadingHeaders)
            {
                int BytesRead = 0;

                // This will have a tendency to fail if a dev has his browser open with the AJAX update
                // running (such as the 'Cluster Nodes' page). If the socket is closed, just fail quietly.
                try
                {
                     BytesRead = Client.Receive(Buffer);
                }
                catch (System.Exception ex)
                {
                    return;
                }
                

                byte[] ProcBuffer = new byte[BytesRead + ExtraBytes.Length];
                ExtraBytes.CopyTo(ProcBuffer, 0);
                for (int i = 0; i < BytesRead; ++i)
                {
                    ProcBuffer[i + ExtraBytes.Length] = Buffer[i];
                }

                int LastOffset = 0;
                for (int i = 0; i < ProcBuffer.Length; ++i)
                {
                    if (ProcBuffer[i] == '\r' || ProcBuffer[i] == '\n')
                    {
                        // End of Header
                        if((i - LastOffset == 0))
                        {
                            ReadingHeaders = false;
                            break;
                        }

                        string Ln = ASCIIEncoding.ASCII.GetString(ProcBuffer, LastOffset, i - LastOffset);

                        if (Ln.Length > 4
                            && (Ln.Substring(0, 3).Equals("GET", StringComparison.CurrentCultureIgnoreCase)
                                || Ln.Substring(0, 4).Equals("POST", StringComparison.CurrentCultureIgnoreCase)))
                        {
                            Headers.Add("Request", Ln);
                        }
                        else
                        {
                            int Colon = Ln.IndexOf(':');
                            if (Colon > -1)
                            {
                                string Key = Ln.Substring(0, Colon).Trim();
                                string Val = Ln.Substring(Colon + 1).Trim();

                                Headers.Add(Key, Val);
                            }
                            else
                            {
                                Headers.Add("Unknown", Ln);
                            }
                        }

                        if (ProcBuffer[i] == '\r' && i < ProcBuffer.Length - 1)
                            if (ProcBuffer[i + 1] == '\n')
                                ++i;
                        LastOffset = i + 1;
                    }
                }

                if (!ReadingHeaders)
                    break;
            } // Reading Headers

            // Check Authentication first!
            if (!Headers.ContainsKey("Authorization") || Headers["Authorization"].Length < 7 || !Headers["Authorization"].Substring(0, 6).Equals("basic ", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("Sending AUTH to client: " + Client.RemoteEndPoint.ToString());

                PostHTTPAuthorization();
                Client.Close();
                return;
            }
            else
            {
                string UserAndPass = Headers["Authorization"].Substring(6);
                UserAndPass = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(UserAndPass));

                int Colon = UserAndPass.IndexOf(':');
                if (Colon == -1)
                {
                    PostHTTPAuthorization();
                    Client.Close();
                    return;
                }

                string User = UserAndPass.Substring(0, Colon);
                string Pass = UserAndPass.Substring(Colon + 1);

                if (!Host.CheckAuth(User, Pass))
                {
                    PostHTTPAuthorization();
                    Client.Close();
                    return;
                }

                //Console.WriteLine("Client OK: " + Client.RemoteEndPoint.ToString());
            }

            if (!Headers.ContainsKey("Request"))
            {
                Client.Close();
                return;
            }

            if (Headers["Request"].Substring(0, 4).Equals("GET ", StringComparison.CurrentCultureIgnoreCase))
            {
                string U = Headers["Request"].Substring(4);
                if (!U.Substring(U.Length - 9).Equals(" HTTP/1.1"))
                {
                    Client.Close();
                    return;
                }

                Dictionary<string, string> Arguments = new Dictionary<string, string>();

                U = U.Substring(0, U.Length - 9);
                int Sep = U.IndexOf('?');
                string A = "";
                if (Sep > -1)
                {
                    A = U.Substring(Sep + 1);
                    U = U.Substring(0, Sep);

                    Regex Reg = new Regex("[&?]");
                    string[] Arg = Reg.Split(A);

                    foreach (string SA in Arg)
                    {
                        int Eq = SA.IndexOf('=');
                        if (Eq < 1)
                            continue;

                        string ArgName = SA.Substring(0, Eq);
                        string ArgVal = Uri.UnescapeDataString(SA.Substring(Eq + 1));

                        Arguments.Add(ArgName, ArgVal);
                    }
                }

                Client.Send(Host.ProcessModule(U, Arguments));
            }


            Client.Close();
            return;
        }

        public void PostHTTPAuthorization()
        {
            StringBuilder HTML = new StringBuilder();
            HTML.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/1999/REC-html401-19991224/loose.dtd\">");
            HTML.Append("<html><head><title>Error</title><meta http-equiv=\"Content-Type\" CONTENT=\"text/html; charset=ISO-8859-1\"></head><body><h1>401 Unauthorized.</h1></body></html>");
            
            StringBuilder Packet = new StringBuilder();
            Packet.Append("HTTP/1.1 401 Authorization Required\r\n");
            Packet.Append("Server: RCP/2.50\r\n");
            Packet.Append("WWW-Authenticate: Basic realm=\"Game Server\"\r\n");
            Packet.Append("Content-Type: text/html\r\n");
            Packet.Append("Content-Length: " + HTML.Length.ToString() + "\r\n");
            Packet.Append("\r\n");
            Packet.Append(HTML.ToString());
            
            Client.Send(ASCIIEncoding.ASCII.GetBytes(Packet.ToString()));

        }
    }
}
