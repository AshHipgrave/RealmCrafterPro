using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RCPServer.HTTPModules
{
    public class AdminServers : IHTTPModule
    {
        public class ZoneInstance
        {
            public static List<ZoneInstance> Instances = new List<ZoneInstance>();

            public string ZoneName;
            public int InstanceID;
            public int ClientCount;
            public bool IsUnique;
            public WorldServerInstance Server;

            public ZoneInstance(string zoneName, int instanceID, int clientCount, bool isUnique)
            {
                ZoneName = zoneName;
                InstanceID = instanceID;
                ClientCount = clientCount;
                IsUnique = isUnique;
            }
        }

        public class WorldServerInstance
        {
            public string HostName;
            public string PublicIP;
            public string PrivateIP;
            public int PublicPort;
            public int PrivatePort;
            public int AvgUpdate;
            public int ZoneCount;
            public int ClientCount;
            public bool Master = false;
            public bool DropServer = false;
            public int[] HourlySetsTX = new int[24];
            public int[] HourlySetsRX = new int[24];
            public int[] HourlyClients = new int[24];

            public WorldServerInstance(string name, string pip, int ppo, string vip, int vpo, int fps, int zs, int cc)
            {
                HostName = name;
                PublicIP = pip;
                PrivateIP = vip;
                PublicPort = ppo;
                PrivatePort = vpo;
                AvgUpdate = fps;
                ZoneCount = zs;
                ClientCount = cc;

//                 Random R = new Random();
//                 Instances.Add(new ZoneInstance("City", 0, R.Next(0, 10), true));
//                 Instances.Add(new ZoneInstance("DefaultZone", 0, R.Next(0, 10), false));
//                 Instances.Add(new ZoneInstance("Mine_2_Inst", 0, 0, false));
//                 Instances.Add(new ZoneInstance("Mine_2_Inst", 1, R.Next(0, 10), false));
//                 Instances.Add(new ZoneInstance("Mine_2_Inst", 2, R.Next(0, 10), false));
//                 Instances.Add(new ZoneInstance("Mine_2_Inst", 3, R.Next(0, 10), false));
//                 Instances.Add(new ZoneInstance("Mine_2_Inst", 4, R.Next(0, 10), false));
            }
        };

        public static List<WorldServerInstance> WorldServers = new List<WorldServerInstance>();
        public static List<WorldServerInstance> ProxyServers = new List<WorldServerInstance>();
        public static List<WorldServerInstance> AccountServers = new List<WorldServerInstance>();

        public AdminServers()
        {
//             Random R = new Random();
//             AccountServers.Add(new WorldServerInstance("Lrrr", "", 0, "192.168.0.72", 27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
//             AccountServers.Add(new WorldServerInstance("Zapp", "", 0, "192.168.0.73", 27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
// 
//             WorldServers.Add(new WorldServerInstance("Fry", "", 0, "192.168.0.3", 27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
//             WorldServers.Add(new WorldServerInstance("Leela", "", 0, "192.168.0.4", 27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
//             WorldServers.Add(new WorldServerInstance("Bender",   "",  0, "192.168.0.5",  27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
//             WorldServers.Add(new WorldServerInstance("Zoidberg", "",  0, "192.168.0.6",  27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
//             WorldServers.Add(new WorldServerInstance("Hermes",   "",  0, "192.168.0.7",  27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
//             WorldServers.Add(new WorldServerInstance("Amy",      "",  0, "192.168.0.8",  27000, R.Next(1, 4000), R.Next(1, 10), R.Next(10, 60)));
// 
//             ProxyServers.Add(new WorldServerInstance("Calculon", "10.62.62.11", 25000, "192.168.0.11", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Kif", "10.62.62.12", 25000, "192.168.0.12", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Nibbler", "10.62.62.13", 25000, "192.168.0.13", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Nixon", "10.62.62.14", 25000, "192.168.0.14", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Hypnotoad", "10.62.62.16", 25000, "192.168.0.16", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("BrainSlug", "10.62.62.17", 25000, "192.168.0.17", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Flexo", "10.62.62.18", 25000, "192.168.0.18", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Crushinator", "10.62.62.19", 25000, "192.168.0.19", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Morbo", "10.62.62.20", 25000, "192.168.0.20", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("DonBot", "10.62.62.21", 25000, "192.168.0.21", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
//             ProxyServers.Add(new WorldServerInstance("Elzar", "10.62.62.22", 25000, "192.168.0.22", 27000, R.Next(1, 4000), R.Next(200, 2000), R.Next(10, 60)));
        }

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/servers"))
                return true;
            return false;
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
            string HTML = "err";

            if (args.ContainsKey("cliid"))
            {
                int PropID = Convert.ToInt32(args["cliid"]);
                if (PropID == -1)
                    args["s"] = "u";

                HTML = "Implement Client List! (" + PropID.ToString() + ")";

            }
            else if (args.ContainsKey("disid"))
            {
                int PropID = Convert.ToInt32(args["disid"]);
                if (PropID == -1)
                    args["s"] = "u";

                string TypeName = args["s"];
                if (TypeName == "a")
                    TypeName = "Account Server";
                else if (TypeName == "p")
                    TypeName = "Proxy Server";
                else if (TypeName == "w")
                    TypeName = "Zone Server";
                else
                    TypeName = "Unknown";
                
                if (TypeName == "Account Server")
                {
                    lock (AccountServers)
                    {
                        if (PropID < AccountServers.Count)
                        {
                            WorldServerInstance Ac = AccountServers[PropID];

                            // Since we're in another thread, we can't just search for another server
                            // without risking an issue popping up. Instead, set a 'drop' flag which
                            // the main thread will capture and process.
                            Ac.DropServer = true;

                            HTML = "JScontextSwitch('/servers');|Redirecting...";

                        }
                    }
                }
                else if (TypeName == "Proxy Server")
                {
                    lock (ProxyServers)
                    {
                        if (PropID < ProxyServers.Count)
                        {
                            WorldServerInstance Pr = ProxyServers[PropID];

                            // Since we're in another thread, we can't just search for another server
                            // without risking an issue popping up. Instead, set a 'drop' flag which
                            // the main thread will capture and process.
                            Pr.DropServer = true;

                            HTML = "JScontextSwitch('/servers');|Redirecting...";
                        }
                    }
                }
                else if (TypeName == "Zone Server")
                {
                    lock (WorldServers)
                    {
                        if (PropID < WorldServers.Count)
                        {
                            WorldServerInstance Pr = WorldServers[PropID];

                            // Since we're in another thread, we can't just search for another server
                            // without risking an issue popping up. Instead, set a 'drop' flag which
                            // the main thread will capture and process.
                            Pr.DropServer = true;

                            HTML = "JScontextSwitch('/servers');|Redirecting...";
                        }
                    }
                }

            }
            else if (args.ContainsKey("propid"))
            {
                int PropID = Convert.ToInt32(args["propid"]);
                if(PropID == -1)
                    args["s"] = "u";

                string TypeName = args["s"];
                if (TypeName == "a")
                    TypeName = "Account Server";
                else if (TypeName == "p")
                    TypeName = "Proxy Server";
                else if (TypeName == "w")
                    TypeName = "Zone Server";
                else
                    TypeName = "Unknown";
                
                HTML = "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" style=\"padding-left: 50px;\" valign=\"top\"><br /><br /><br /><br /><table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 450px;\"><tr><td style=\"background-image: url('/img/menugrad.jpg'); height: 21px; padding-left: 10px; border-top: 2px solid #DC1300; border-left: 1px solid #DC1300; border-bottom: 1px solid #390909; border-right: 1px solid #390909;\" valign=\"middle\" class=\"VertMenuTitle\"><b>Server Properies</b></td></tr><tr><td style=\"border: 1px solid #000000; padding: 5px;\">";
                HTML += "<table style=\"width: 100%; border: 0px; \">";

                HTML += "<tr><td>Server Type:</td><td> " + TypeName + "</td></tr>";

                if (TypeName == "Account Server")
                {
                    lock (AccountServers)
                    {
                        if (PropID < AccountServers.Count)
                        {


                            WorldServerInstance Ac = AccountServers[PropID];
                            HTML += "<tr><td>Machine HostName:</td><td> " + Ac.HostName + "</td></tr>";
                            HTML += "<tr><td>Address:</td><td> " + Ac.PrivateIP + ":" + Ac.PrivatePort.ToString() + "</td></tr>";
                            HTML += "<tr><td>Update Time:</td><td> " + Ac.AvgUpdate.ToString() + "</td></tr>";

                            HTML += "<tr><td></td><td></td></tr>";
                            HTML += "<tr><td></td><td><input type=\"button\" value=\"Drop Server\" onclick=\"if( confirm('Are you sure you want to drop this server? All players connected will be dropped and once offline it cannot reconnect by itself!') ) { contextSwitch('/servers?disid=" + PropID.ToString() + "&s=a'); } \" />&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>";


                        }
                        else
                        {
                            HTML += "Stopped - Selected Index not found!";
                        }
                    }
                }
                else if (TypeName == "Proxy Server")
                {
                    lock (ProxyServers)
                    {
                        if (PropID < ProxyServers.Count)
                        {
                            WorldServerInstance Pr = ProxyServers[PropID];

                            string BWAsStr1 = "";
                            int Div = 0;
                            double BW = (double)Pr.HourlySetsTX[0];

                            while (BW > 1024.0)
                            {
                                BW /= 1024.0;
                                ++Div;
                            }

                            BWAsStr1 = BW.ToString("F");

                            if (Div == 0)
                                BWAsStr1 += " B/s";
                            else if (Div == 1)
                                BWAsStr1 += " KB/s";
                            else if (Div == 2)
                                BWAsStr1 += " MB/s";

                            string BWAsStr2 = "";
                            Div = 0;
                            BW = (double)Pr.HourlySetsRX[0];

                            while (BW > 1024.0)
                            {
                                BW /= 1024.0;
                                ++Div;
                            }

                            BWAsStr2 = BW.ToString("F");

                            if (Div == 0)
                                BWAsStr2 += " B/s";
                            else if (Div == 1)
                                BWAsStr2 += " KB/s";
                            else if (Div == 2)
                                BWAsStr2 += " MB/s";

                            if (args.ContainsKey("dir"))
                            {
                                if(args["dir"] == "p")
                                    HTML = "JScontextSwitch('/24hplayers?sa=" + Pr.PrivateIP + "&sp=" + Pr.PrivatePort + "&propid=" + PropID.ToString() + "&s=p');|";
                                else if(args["dir"] == "b")
                                    HTML = "JScontextSwitch('/24hbandwidth?sa=" + Pr.PrivateIP + "&sp=" + Pr.PrivatePort + "&propid=" + PropID.ToString() + "&s=p');|";
                            }


                            HTML += "<tr><td>Machine HostName:</td><td> " + Pr.HostName + "</td></tr>";
                            HTML += "<tr><td>Master:</td><td>" + (Pr.Master ? "Yes" : "No") + "</td></tr>";
                            HTML += "<tr><td>Private Address:</td><td> " + Pr.PrivateIP + ":" + Pr.PrivatePort.ToString() + "</td></tr>";
                            HTML += "<tr><td>Public Address:</td><td> " + Pr.PublicIP + ":" + Pr.PublicPort.ToString() + "</td></tr>";
                            HTML += "<tr><td>Update Time:</td><td> " + Pr.AvgUpdate.ToString() + "</td></tr>";
                            HTML += "<tr><td>Clients:</td><td> " + Pr.ClientCount.ToString() + "</td></tr>";
                            HTML += "<tr><td>Avg TX:</td><td> " + BWAsStr1 + "</td></tr>";
                            HTML += "<tr><td>Avg RX:</td><td> " + BWAsStr2 + "</td></tr>";

                            HTML += "<tr><td></td><td></td></tr>";
                            HTML += "<tr><td></td><td><input type=\"button\" value=\"Player Graph\" onclick=\"contextSwitch('/24hplayers?sa=" + Pr.PrivateIP + "&sp=" + Pr.PrivatePort + "&propid=" + PropID.ToString() + "&s=p');\" />&nbsp;&nbsp;";
                            HTML += "<input type=\"button\" value=\"Bandwidth Graph\" onclick=\"contextSwitch('/24hbandwidth?sa=" + Pr.PrivateIP + "&sp=" + Pr.PrivatePort + "&propid=" + PropID.ToString() + "&s=p');\" /></td></tr>";
                            HTML += "<tr><td></td><td><input type=\"button\" value=\"Drop Server\" onclick=\"if( confirm('Are you sure you want to drop this server? Once offline it cannot reconnect by itself!') ) { contextSwitch('/servers?disid=" + PropID.ToString() + "&s=p'); } \" />&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>";
                            //HTML += "<input type=\"button\" value=\"View Client List\" onclick=\"contextSwitch('/servers?cliid=" + PropID.ToString() + "&s=p');\" />&nbsp;&nbsp;&nbsp;&nbsp;";
                        }
                        else
                        {
                            HTML += "Stopped - Selected Index not found!";
                        }
                    }
                }
                else if (TypeName == "Zone Server")
                {
                    lock (WorldServers)
                    {
                        if (PropID < WorldServers.Count)
                        {
                            WorldServerInstance Pr = WorldServers[PropID];

                            if (args.ContainsKey("dir"))
                            {
                                if (args["dir"] == "z")
                                    HTML = "JScontextSwitch('/zones?sid=" + PropID.ToString() + "&s=w');|";
                            }


                            HTML += "<tr><td>Machine HostName:</td><td> " + Pr.HostName + "</td></tr>";
                            HTML += "<tr><td>Private Address:</td><td> " + Pr.PrivateIP + ":" + Pr.PrivatePort.ToString() + "</td></tr>";
                            HTML += "<tr><td>Update Time:</td><td> " + Pr.AvgUpdate.ToString() + "</td></tr>";
                            HTML += "<tr><td>Zone Instances:</td><td> " + Pr.ZoneCount.ToString() + "</td></tr>";
                            HTML += "<tr><td>Clients:</td><td> " + Pr.ClientCount.ToString() + "</td></tr>";

                            HTML += "<tr><td></td><td></td></tr>";
                            HTML += "<tr><td></td><td><input type=\"button\" value=\"Instance List\" onclick=\"contextSwitch('/zones?sid=" + PropID.ToString() + "&s=w');\" /></td></tr>";
                            HTML += "<tr><td></td><td><input type=\"button\" value=\"Drop Server\" onclick=\"if( confirm('Are you sure you want to drop this server? Once offline it cannot reconnect by itself!') ) { contextSwitch('/servers?disid=" + PropID.ToString() + "&s=w'); } \" /></td></tr>";
                            //HTML += "<input type=\"button\" value=\"View Client List\" onclick=\"contextSwitch('/servers?cliid=" + PropID.ToString() + "&s=w');\" />&nbsp;&nbsp;&nbsp;&nbsp;";
                        }
                        else
                        {
                            HTML += "Stopped - Selected Index not found!";
                        }
                    }
                }
                else
                {
                    HTML += "Unidentified server!";
                }



                HTML += "</table>";
                HTML += "</td></tr></table></td></tr></table>";
            }// Check for 'get update' check
            else if (args.ContainsKey("UpdateFPS"))
            {
                HTML = "";

                lock (WorldServers)
                {
                    foreach (WorldServerInstance W in WorldServers)
                    {
                        string ClassName = "#B5B5B5";
                        if (W.AvgUpdate > 30)
                            ClassName = "#FFA53F";
                        if (W.AvgUpdate > 50)
                            ClassName = "#FF0000";

                        HTML += "d0_" + W.HostName + "|" + ClassName + "|" + W.AvgUpdate.ToString() + "|";
                        HTML += "d0_" + W.HostName + "c|#B5B5B5|" + W.ClientCount.ToString() + "|";
                    }
                }

                lock (ProxyServers)
                {
                    foreach (WorldServerInstance W in ProxyServers)
                    {
                        string ClassName = "#B5B5B5";
                        if (W.AvgUpdate > 30)
                            ClassName = "#FFA53F";
                        if (W.AvgUpdate > 50)
                            ClassName = "#FF0000";

                        string BWAsStr = "";
                        int Div = 0;
                        double BW = (double)W.HourlySetsRX[0] + W.HourlySetsTX[0];

                        while (BW > 1024.0)
                        {
                            BW /= 1024.0;
                            ++Div;
                        }

                        BWAsStr = BW.ToString("F");

                        if (Div == 0)
                            BWAsStr += " B/s";
                        else if (Div == 1)
                            BWAsStr += " KB/s";
                        else if (Div == 2)
                            BWAsStr += " MB/s";

                        HTML += "d1_" + W.HostName + "|" + ClassName + "|" + W.AvgUpdate.ToString() + "|";
                        HTML += "d1_" + W.HostName + "b|#B5B5B5|" + BWAsStr + "|";
                        HTML += "d1_" + W.HostName + "c|#B5B5B5|" + W.ClientCount.ToString() + "|";
                    }
                }

                lock (AccountServers)
                {
                    foreach (WorldServerInstance W in AccountServers)
                    {
                        string ClassName = "#B5B5B5";
                        if (W.AvgUpdate > 30)
                            ClassName = "#FFA53F";
                        if (W.AvgUpdate > 50)
                            ClassName = "#FF0000";

                        HTML += "d2_" + W.HostName + "|" + ClassName + "|" + W.AvgUpdate.ToString() + "|";
                    }
                }
            }
            else
            {
                HTML = "";
                HTML += "JSStartChecker('/servers?UpdateFPS=0', 5000);|<table style=\"width: 850px; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" valign=\"top\" style=\"padding-left: 50px;\"><br /><br /><br /><br />Account Servers:&nbsp;&nbsp;&nbsp;<input type=\"button\" value=\"Properties\" onclick=\"if(AccountServerSelected >= 0) { contextSwitch('/servers?propid=' + AccountServerSelected + '&s=a'); }\"/></br><div id=\"tableContainer\" class=\"tableContainer\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"800\" class=\"scrollTable\"><thead class=\"fixedHeader\"><tr><th><a href=\"#\">Hostname</a></th><th><a href=\"#\">Private IP</a></th><th><a href=\"#\">FT (ms)</a></th><th><a href=\"#\"></a></th><th><a href=\"#\">&nbsp;</a></th><th>&nbsp;</th></tr></thead><tbody id=\"tableContainer_scrollContent\" class=\"scrollContent\">";

                int Idx = 0;
                lock (AccountServers)
                {
                    foreach (WorldServerInstance W in AccountServers)
                    {
                        HTML += string.Format("<tr class=\"onlineRow\" id=\"d{0}_r{1}\"><td onclick=\"SelectRow({0}, {1});\">{2}</td><td onclick=\"SelectRow({0}, {1});\">{4}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}\">{5}</td><td onclick=\"SelectRow({0}, {1});\">{6}</td><td onclick=\"SelectRow({0}, {1});\">&nbsp;</td><td onclick=\"SelectRow({0}, {1});\">&nbsp;</td></tr>",
                            new string[] { "2", Idx.ToString(), W.HostName,
                    W.PublicIP + ":" + W.PublicPort.ToString(), W.PrivateIP + ":" + W.PrivatePort.ToString(),
                    W.AvgUpdate.ToString(), ""});
                        ++Idx;
                    }
                }

                HTML += "</tbody></table></div><br/><br/>";

                HTML += "World Servers:&nbsp;&nbsp;&nbsp;<input type=\"button\" value=\"Properties\" onclick=\"if(WorldServerSelected >= 0) { contextSwitch('/servers?propid=' + WorldServerSelected + '&s=w'); }\" /><input type=\"button\" value=\"Instance List\" onclick=\"if(WorldServerSelected >= 0) { contextSwitch('/servers?propid=' + WorldServerSelected + '&s=w&dir=z'); }\" /></br><div id=\"tableContainer\" class=\"tableContainer\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"scrollTable\"><thead class=\"fixedHeader\"><tr><th><a href=\"#\">Hostname</a></th><th><a href=\"#\">Private IP</a></th><th><a href=\"#\">FT (ms)</a></th><th><a href=\"#\">Zones</a></th><th><a href=\"#\">Clients</a></th><th>&nbsp;</th></tr></thead><tbody id=\"tableContainer_scrollContent\" class=\"scrollContent\">";

                Idx = 0;
                lock (WorldServers)
                {
                    foreach (WorldServerInstance W in WorldServers)
                    {
                        HTML += string.Format("<tr class=\"onlineRow\" id=\"d{0}_r{1}\"><td onclick=\"SelectRow({0}, {1});\">{2}</td><td onclick=\"SelectRow({0}, {1});\">{4}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}\">{5}</td><td onclick=\"SelectRow({0}, {1});\">{6}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}c\">{7}</td><td onclick=\"SelectRow({0}, {1});\">&nbsp;</td></tr>",
                            new string[] { "0", Idx.ToString(), W.HostName,
                    W.PublicIP + ":" + W.PublicPort.ToString(), W.PrivateIP + ":" + W.PrivatePort.ToString(),
                    W.AvgUpdate.ToString(), W.ZoneCount.ToString(), W.ClientCount.ToString()});
                        ++Idx;
                    }
                }

                HTML += "</tbody></table></div><br/><br/>";

                HTML += "Proxy Servers:&nbsp;&nbsp;&nbsp;<input type=\"button\" value=\"Properties\" onclick=\"if(ProxyServerSelected >= 0) { contextSwitch('/servers?propid=' + ProxyServerSelected + '&s=p'); }\" /><input type=\"button\" value=\"Player Graph\" onclick=\"if(ProxyServerSelected >= 0) { contextSwitch('/servers?propid=' + ProxyServerSelected + '&s=p&dir=p'); }\" /><input type=\"button\" value=\"Bandwidth Graph\" onclick=\"if(ProxyServerSelected >= 0) { contextSwitch('/servers?propid=' + ProxyServerSelected + '&s=p&dir=b'); }\" /><br /><div id=\"tableContainer\" class=\"tableContainer\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"scrollTable\"><thead class=\"fixedHeader\"><tr><th><a href=\"#\">Hostname</a></th><th><a href=\"#\">Public IP</a></th><th><a href=\"#\">Private IP</a></th><th><a href=\"#\">FT (ms)</a></th><th><a href=\"#\">Clients</a></th><th><a href=\"#\">Avg. Bw.</a></th></tr></thead><tbody id=\"tableContainer_scrollContent\" class=\"scrollContent\">";

                Idx = 0;
                lock (ProxyServers)
                {
                    foreach (WorldServerInstance W in ProxyServers)
                    {
                        string BWAsStr = "";
                        int Div = 0;
                        double BW = (double)W.HourlySetsRX[0] + W.HourlySetsTX[0];

                        while (BW > 1024.0)
                        {
                            BW /= 1024.0;
                            ++Div;
                        }

                        BWAsStr = BW.ToString("F");

                        if (Div == 0)
                            BWAsStr += " B/s";
                        else if (Div == 1)
                            BWAsStr += " KB/s";
                        else if (Div == 2)
                            BWAsStr += " MB/s";


                        HTML += string.Format("<tr class=\"onlineRow\" id=\"d{0}_r{1}\"><td onclick=\"SelectRow({0}, {1});\">{2}</td><td onclick=\"SelectRow({0}, {1});\">{3}</td><td onclick=\"SelectRow({0}, {1});\">{4}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}\">{5}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}c\">{6}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}b\">{7}</td></tr>",
                            new string[] { "1", Idx.ToString(), W.HostName,
                    W.PublicIP + ":" + W.PublicPort.ToString(), W.PrivateIP + ":" + W.PrivatePort.ToString(),
                    W.AvgUpdate.ToString(), W.ClientCount.ToString(), BWAsStr});
                        ++Idx;
                    }
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
