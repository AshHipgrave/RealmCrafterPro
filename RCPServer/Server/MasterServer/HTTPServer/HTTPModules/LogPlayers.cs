using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using Graph;

namespace RCPServer.HTTPModules
{
    public class LogPlayers : IHTTPModule
    {

        public LogPlayers()
        {
            
        }

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/24hplayers"))
                return true;
            return false;
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {


            if (args.ContainsKey("g"))
            {
                int[] DayTotals = new int[24];

                bool Found = false;
                if (args.ContainsKey("sa") && args.ContainsKey("sp"))
                {
                    string Address = args["sa"];
                    int Port = Convert.ToInt32(args["sp"]);

                    lock (AdminServers.ProxyServers)
                    {
                        foreach (AdminServers.WorldServerInstance WSI in AdminServers.ProxyServers)
                        {
                            if (WSI.PrivatePort == Port && WSI.PrivateIP.Equals(Address, StringComparison.CurrentCultureIgnoreCase))
                            {
                                DayTotals = WSI.HourlyClients;

                                Found = true;
                                break;
                            }
                        }
                    }
                }

                if (!Found)
                {
                    lock (AdminServers.ProxyServers)
                    {
                        foreach (AdminServers.WorldServerInstance WSI in AdminServers.ProxyServers)
                        {
                            lock (WSI)
                            {
                                for (int i = 0; i < 24; ++i)
                                    DayTotals[i] += WSI.HourlyClients[i];
                            }
                        }
                    }
                }


                LineGraph G = new LineGraph(640, 480);
                G.XAxisLabel = "Time";
                G.YAxisLabel = "Players";
                G.YDifference = 10;
                G.DrawCurve = false;
                G.Title = "Player Count over 24 Hours";

                List<LineGraph.PlotPoint> P = new List<LineGraph.PlotPoint>();

                for (int i = 23, t = 0; i >= 0; --i, ++t)
                {
                    P.Add(new LineGraph.PlotPoint(-23 + t, DayTotals[i]));
                }



                //G.SetLabel(0, "Blue Line");
                //G.SetLabel(1, "Red Line");
                //G.SetLabel(2, "Green Line");
                //G.SetLabel(3, "Pink Line");
                //G.SetLabel(4, "Purple Line");

                G.SetData(0, P);

                Image I = G.Draw();

                MemoryStream ImgMem = new MemoryStream();
                MemoryStream PacketMem = new MemoryStream();

                I.Save(ImgMem, System.Drawing.Imaging.ImageFormat.Gif);
                
                StringBuilder Packet = new StringBuilder();
                Packet.Append("HTTP/1.1 200 OK\r\n");
                Packet.Append("Server: RCP/2.50\r\n");
                Packet.Append("Content-Type: image/gif\r\n");
                Packet.Append("Content-Length: " + ImgMem.Length.ToString() + "\r\n");
                Packet.Append("\r\n");

                byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(Packet.ToString());
                PacketMem.Write(Buffer, 0, Buffer.Length);
                Buffer = ImgMem.GetBuffer();
                PacketMem.Write(Buffer, 0, Buffer.Length);

                return PacketMem.GetBuffer();
            }
            else
            {
                string PageHTML = "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" valign=\"top\" style=\"padding-left: 50px;\"><br /><br /><br /><br /><table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 300px;\"><tr><td style=\"background-image: url('/img/menugrad.jpg'); height: 21px; padding-left: 10px; border-top: 2px solid #DC1300; border-left: 1px solid #DC1300; border-bottom: 1px solid #390909; border-right: 1px solid #390909;\" valign=\"middle\" class=\"VertMenuTitle\"><b>View Graph</b></td></tr><tr><td style=\"border: 1px solid #000000; padding: 5px;\">";

                if (args.ContainsKey("sa") && args.ContainsKey("sp"))
                {
                    string PropID = "-1";
                    string SType = "u";

                    if (args.ContainsKey("propid"))
                        PropID = args["propid"];
                    if (args.ContainsKey("s"))
                        SType = args["s"];

                    PageHTML += "<input type=\"button\" value=\"Return to Properties\" onclick=\"contextSwitch('/servers?propid=" + PropID + "&s=" + SType + "&ctime=' + new Date().getTime());\"/><br/>";
                    PageHTML += "<img src=\"/24hplayers?g=1&sa=" + args["sa"] + "&sp=" + args["sp"] + "&t=" + Environment.TickCount + "\" style=\"width: 640px; height: 480px;\" />";
                }
                else
                {
                    PageHTML += "<img src=\"/24hplayers?g=1&t=" + Environment.TickCount + "\" style=\"width: 640px; height: 480px;\" />";
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
}
