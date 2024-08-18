using System;
using System.Collections.Generic;
using System.Text;
using ScriptManager = Scripting.ScriptManager;

using System.Reflection;
using System.Reflection.Emit;
using Scripting;
using ServerShared;

namespace RCPServer
{
    public class MasterServer
    {
        public static string ApplicationName = "Realm Crafter Professional Server";
        public static string ApplicationVersion = "2.53 (Release)";
        public static string ApplicationBuildType = "Master Server";

        public static HTTPServer WebServer;

        public const bool UseDebugger = false;
        public static int Debugging = 0;
        public static string LastDebug = "";
        public static float FPS = 0;

        public static uint DisconnectTime = 0;
        public static int DisconnectState = 0; // 0 - ZS, 1 = Proxy+Accnt

        protected static int[] LastFrameMS = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected static uint LastFrameTick = 0;
        public static int AverageFT = 0;
        protected static int Frames = 0;

        public static string ServerPassword;

        public static void Main(string[] args)
        {
            ServerPassword = SecretString.LoadSecretString();

            #region Server Startup

            if (args.Length > 0 && args[0].Equals("-testingServer", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Title = "MASTER";
                Debugging = RCEnet.Connect("localhost", 24999);


                Log.StartLog("MasterLog.txt", true);
            }
            else
            {
                Log.StartLog("MasterLog.txt", true);
                Console.Title = ApplicationName;
            }

            // Setup console for notification output
            Console.CursorVisible = false;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            

            Log.Write("Starting ");
            Log.WriteLine(ApplicationName, ConsoleColor.Yellow);
            Log.Write("Version: ");
            Log.WriteLine(ApplicationVersion, ConsoleColor.Yellow);
            Log.Write("Build Type: ");
            Log.WriteLine(ApplicationBuildType, ConsoleColor.Yellow);
            Log.WriteLine("");

            try
            {
                // Load node information (0 = proxy, 1 = master, 2 = account, 3 = zone
                Server.LoadClusterInfo("MasterServer.xml");

                Log.Write("Loading Areas                  ");
                int AreaCount = WorldZone.Load("Data/Server Data/Areas/");
                if (AreaCount == 0)
                    throw new Exception("At least one zone is required");
                Log.WriteOK("(" + AreaCount.ToString() + " Instances)");

                Log.Write("Starting Network               ");

                Server.Host = RCEnet.StartHost(Server.PrivatePort, 5000);
                if (Server.Host == 0)
                    throw new Exception("Could not open network!");
                NetworkImpl.Init();
                Log.WriteOK();

                Log.Write("Starting HTTP Server           ");

                WebServer = new HTTPServer("0.0.0.0", 400);

                Log.WriteOK();
            }
            catch (Exception e)
            {

                Log.WriteFail(e.Message);

                if (Debugging != 0)
                {
                    string HTMLLog = Log.HTMLLog();

                    PacketWriter DBGPkt = new PacketWriter();
                    DBGPkt.Write((byte)'M');
                    DBGPkt.Write(HTMLLog, true);
                    LastDebug = HTMLLog;

                    RCEnet.Send(Debugging, MessageTypes.P_ServerStat, DBGPkt.ToArray(), true);

                    // Wait a few seconds
                    uint StartQuit = Server.MilliSecs();
                    while (StartQuit + 5000 > Server.MilliSecs())
                        RCEnet.Update();
                }

                Log.StopLog();
                System.Environment.Exit(0);

                return;
            }

            Log.WriteLine("");
            Log.WriteLine("Server Started");
            #endregion

            // Server loaded, main loop now
            while (true)
            {
                if (DisconnectTime != 0)
                {
                    if (Server.MilliSecs() - DisconnectTime > 5000 && DisconnectState == 0)
                    {
                        foreach (AccountServer As in AccountServer.AccountServers)
                        {
                            if (As.LinkedServer != null)
                            {
                                lock (As.LinkedServer)
                                {
                                    As.LinkedServer.DropServer = true;
                                }
                            }
                        }


                        foreach (ProxyServer Ps in ProxyServer.ProxyServers)
                        {
                            if (Ps.LinkedServer != null)
                            {
                                lock (Ps.LinkedServer)
                                {
                                    Ps.LinkedServer.DropServer = true;
                                }
                            }
                        }

                        DisconnectState = 1;
                        DisconnectTime = Server.MilliSecs();
                    }
                    else if (Server.MilliSecs() - DisconnectTime > 6000 && DisconnectState == 1)
                    {
                        break;
                    }
                }


                //GC.Collect();
                RCEnet.Update();
                NetworkImpl.UpdateNetwork();

                // Update info requests
                MasterInfoRequest.Update();

                // Sync real zone lists with HTTP Server
                lock (HTTPModules.AdminServers.WorldServers)
                {
                    // Reset zone count for zone servers
                    for (int s = 0; s < HTTPModules.AdminServers.WorldServers.Count; ++s)
                    {
                        HTTPModules.AdminServers.WorldServers[s].ZoneCount = 0;
                    }

                    foreach (ZoneServer Zi in ZoneServer.ZoneServers)
                    {
                        if(Zi.LinkedServer != null)
                        {
                            lock (Zi.LinkedServer)
                            {
                                Zi.LinkedServer.AvgUpdate = Zi.AvgUpdate;
                                Zi.LinkedServer.ClientCount = Zi.ClientCount;
                            }
                        }
                    }

                    // Match each zone instance with a real instance
                    lock (HTTPModules.AdminServers.ZoneInstance.Instances)
                    {
                        for (int i = 0; i < HTTPModules.AdminServers.ZoneInstance.Instances.Count; ++i)
                        {
                            HTTPModules.AdminServers.ZoneInstance Zi = HTTPModules.AdminServers.ZoneInstance.Instances[i];

                            foreach (ZoneInstance RealZi in ZoneInstance.ZoneInstances)
                            {
                                if (Zi.ZoneName.Equals(RealZi.InstanceOf.Name) && Zi.InstanceID == RealZi.ID)
                                {
                                    // Copy client count
                                    Zi.ClientCount = RealZi.ConnectedClients;

                                    // Find server
                                    if (RealZi.ServerHost != null && RealZi.ServerHost.LinkedServer != null)
                                    {
                                        Zi.Server = RealZi.ServerHost.LinkedServer;
                                        ++RealZi.ServerHost.LinkedServer.ZoneCount;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                } // Lock WorldServers



                lock (typeof(HTTPModules.AdminGlobal))
                {
                    HTTPModules.AdminGlobal.ServerCount = ZoneServer.ZoneServers.Count + AccountServer.AccountServers.Count + ProxyServer.ProxyServers.Count;
                    HTTPModules.AdminGlobal.ZoneCount = ZoneInstance.ZoneInstances.Count;
                    HTTPModules.AdminGlobal.PlayerCount = 0;
                    HTTPModules.AdminGlobal.LoadLevel = 0;
                    HTTPModules.AdminGlobal.Warnings = 0;
                    string HTMLLog = Log.HTMLLog();
                    HTTPModules.AdminGlobal.HTMLLog = HTMLLog;
                    if(Debugging >= 0 && !HTMLLog.Equals(LastDebug))
                    {
                        PacketWriter DBGPkt = new PacketWriter();
                        DBGPkt.Write((byte)'M');
                        DBGPkt.Write(HTMLLog, true);
                        LastDebug = HTMLLog;

                        RCEnet.Send(Debugging, MessageTypes.P_ServerStat, DBGPkt.ToArray(), true);
                    }


                    List<ProxyServer> PsRemovals = new List<ProxyServer>();
                    foreach (ProxyServer Ps in ProxyServer.ProxyServers)
                    {
                        if (Ps.AvgUpdate > 50)
                        {
                            HTTPModules.AdminGlobal.LoadLevel = 2;
                            HTTPModules.AdminGlobal.Warnings++;
                        }
                        else if (Ps.AvgUpdate > 30 && HTTPModules.AdminGlobal.LoadLevel < 1)
                        {
                            HTTPModules.AdminGlobal.LoadLevel = 1;
                            HTTPModules.AdminGlobal.Warnings++;
                        }

                        HTTPModules.AdminGlobal.PlayerCount += Ps.ClientCount;

                        if (Ps.LinkedServer != null)
                        {
                            lock (Ps.LinkedServer)
                            {
                                if (Ps.LinkedServer.DropServer)
                                {
                                    // TODO: Event Log
                                    Log.WriteLine("Administrator dropped proxy server: "
                                        + Ps.ExternalAddress.ToString() + ": " + Ps.ExternalHostPort.ToString() + "/" + Ps.MachineHostName);

                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write((byte)'S');

                                    if (Ps.ServerConnection != 0)
                                        RCEnet.Send(Ps.ServerConnection, MessageTypes.P_ServerList, Pa.ToArray(), true);

                                    PsRemovals.Add(Ps);

                                    lock (HTTPModules.AdminServers.ProxyServers)
                                    {
                                        HTTPModules.AdminServers.ProxyServers.Remove(Ps.LinkedServer);
                                    }

                                    Ps.LinkedServer = null;
                                    if (ProxyServer.MostFree == Ps)
                                        ProxyServer.MostFree = null;
                                    if (ProxyServer.MasterProxy == Ps)
                                        ProxyServer.MasterProxy = null;
                                }
                                else
                                {

                                    Ps.LinkedServer.AvgUpdate = Ps.AvgUpdate;
                                    Ps.LinkedServer.ClientCount = Ps.ClientCount;
                                    Ps.HourlySetsTX.CopyTo(Ps.LinkedServer.HourlySetsTX, 0);
                                    Ps.HourlySetsRX.CopyTo(Ps.LinkedServer.HourlySetsRX, 0);
                                    Ps.HourlyClients.CopyTo(Ps.LinkedServer.HourlyClients, 0);
                                }
                            }
                        }
                    }
                    if (PsRemovals.Count > 0)
                    {
                        foreach (ProxyServer Ps in PsRemovals)
                        {
                            ProxyServer.ProxyServers.Remove(Ps);
                        }

                        if (ProxyServer.MostFree == null)
                            ProxyServer.MostFree = (ProxyServer.ProxyServers.Count > 0) ? ProxyServer.ProxyServers.First.Value : null;
                    }

                    // Moved account servers inside 'global' lock so get load levels
                    List<AccountServer> AccRemovals = new List<AccountServer>();
                    foreach (AccountServer Si in AccountServer.AccountServers)
                    {
                        if (Si.AvgUpdate > 50)
                        {
                            HTTPModules.AdminGlobal.LoadLevel = 2;
                            HTTPModules.AdminGlobal.Warnings++;
                        }
                        else if (Si.AvgUpdate > 30 && HTTPModules.AdminGlobal.LoadLevel < 1)
                        {
                            HTTPModules.AdminGlobal.LoadLevel = 1;
                            HTTPModules.AdminGlobal.Warnings++;
                        }

                        if (Si.LinkedServer != null)
                        {
                            lock (Si.LinkedServer)
                            {
                                if (Si.LinkedServer.DropServer)
                                {
                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write((byte)'D');
                                    Pa.Write((byte)'C');
                                    Pa.Write(Si.Address.GetAddressBytes(), 0, 4);
                                    Pa.Write(Si.HostPort);
                                    byte[] PaA = Pa.ToArray();

                                    foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                    {
                                        if (Pr.ServerConnection != 0)
                                            RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                    }

                                    // TODO: Event Log
                                    Log.WriteLine("Administrator dropped account server: "
                                        + Si.Address.ToString() + ": " + Si.HostPort.ToString() + "/" + Si.MachineHostName);

                                    Pa = new PacketWriter();
                                    Pa.Write((byte)'S');

                                    if(Si.ServerConnection != 0)
                                        RCEnet.Send(Si.ServerConnection, MessageTypes.P_ServerList, Pa.ToArray(), true);

                                    AccRemovals.Add(Si);

                                    lock(HTTPModules.AdminServers.AccountServers)
                                    {
                                        HTTPModules.AdminServers.AccountServers.Remove(Si.LinkedServer);
                                    }

                                    Si.LinkedServer = null;
                                    if (AccountServer.MostFree == Si)
                                        AccountServer.MostFree = null;
                                }
                                else
                                {
                                    Si.LinkedServer.AvgUpdate = Si.AvgUpdate;
                                }
                            }
                        }
                    }

                    if (AccRemovals.Count > 0)
                    {
                        foreach (AccountServer Si in AccRemovals)
                        {
                            AccountServer.AccountServers.Remove(Si);
                        }

                        if (AccountServer.MostFree == null)
                            AccountServer.MostFree = (AccountServer.AccountServers.Count > 0) ? AccountServer.AccountServers.First.Value : null;
                    }

                    // Zone Servers
                    List<ZoneServer> ZnRemovals = new List<ZoneServer>();
                    foreach (ZoneServer Zs in ZoneServer.ZoneServers)
                    {
                        if (Zs.AvgUpdate > 50)
                        {
                            HTTPModules.AdminGlobal.LoadLevel = 2;
                            HTTPModules.AdminGlobal.Warnings++;
                        }
                        else if (Zs.AvgUpdate > 30 && HTTPModules.AdminGlobal.LoadLevel < 1)
                        {
                            HTTPModules.AdminGlobal.LoadLevel = 1;
                            HTTPModules.AdminGlobal.Warnings++;
                        }

                        if (Zs.LinkedServer != null)
                        {
                            lock (Zs.LinkedServer)
                            {
                                if (Zs.LinkedServer.DropServer)
                                {
                                    ZoneServer NewServer = null;
                                    if (ZoneServer.MostFree == Zs)
                                        ZoneServer.MostFree = null;

                                    if (ZoneServer.MostFree != null && ZoneServer.MostFree.LinkedServer.DropServer == false)
                                    {
                                        NewServer = ZoneServer.MostFree;
                                    }
                                    else
                                    {
                                        foreach (ZoneServer Zs2 in ZoneServer.ZoneServers)
                                        {
                                            if (Zs2 != Zs && Zs2.LinkedServer.DropServer == false)
                                            {
                                                NewServer = Zs2;
                                                break;
                                            }
                                        }
                                    }

                                    System.Net.IPAddress NewAddress = System.Net.IPAddress.None;
                                    ushort NewPort = 0;

                                    if (NewServer != null)
                                    {
                                        NewAddress = NewServer.Address;
                                        NewPort = NewServer.HostPort;
                                    }

                                    PacketWriter ZoneWriter = new PacketWriter();
                                    PacketWriter NewZoneWriter = new PacketWriter();
                                    PacketWriter ProxyWriter = new PacketWriter();

                                    ProxyWriter.Write((byte)'D');
                                    ProxyWriter.Write((byte)(NewServer != null ? 'S' : 'Z'));
                                    ProxyWriter.Write(Zs.Address.GetAddressBytes(), 0, 4);
                                    ProxyWriter.Write(Zs.HostPort);

                                    foreach (ZoneInstance Zi in ZoneInstance.ZoneInstances)
                                    {
                                        if (Zi.ServerHost == Zs)
                                        {
                                            ZoneWriter.Write((byte)'U');
                                            ZoneWriter.Write((byte)Zi.InstanceOf.Name.Length);
                                            ZoneWriter.Write(Zi.InstanceOf.Name, false);
                                            ZoneWriter.Write((ushort)Zi.ID);
                                            ZoneWriter.Write(-1);

                                            NewZoneWriter.Write((byte)'A');
                                            NewZoneWriter.Write((byte)Zi.InstanceOf.Name.Length);
                                            NewZoneWriter.Write(Zi.InstanceOf.Name, false);
                                            NewZoneWriter.Write((ushort)Zi.ID);
                                            NewZoneWriter.Write(0);

                                            ProxyWriter.Write((byte)'U');
                                            ProxyWriter.Write((byte)Zi.InstanceOf.Name.Length);
                                            ProxyWriter.Write(Zi.InstanceOf.Name, false);
                                            ProxyWriter.Write((ushort)Zi.ID);

                                            ProxyWriter.Write(NewAddress.GetAddressBytes(), 0);
                                            ProxyWriter.Write((ushort)NewPort);

                                            Zi.ServerHost = NewServer;
                                        }
                                    } // foreach

                                    // Send redirections to proxy servers
                                    byte[] ProxyWriterA = ProxyWriter.ToArray();

                                    foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                    {
                                        if (Pr.ServerConnection != 0)
                                            RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, ProxyWriterA, true);
                                    }

                                    // TODO: Event Log
                                    Log.WriteLine("Administrator dropped zone server: "
                                        + Zs.Address.ToString() + ": " + Zs.HostPort.ToString() + "/" + Zs.MachineHostName);

                                    // Send new zone server the 'activate' packet
                                    if (NewServer != null && NewServer.ServerConnection != 0)
                                        RCEnet.Send(NewServer.ServerConnection, MessageTypes.P_ServerList, NewZoneWriter.ToArray(), true);

                                    // Old server packets
                                    if (Zs.ServerConnection != 0)
                                    {
                                        // Shutdown message
                                        ZoneWriter.Write((byte)'S');

                                        // Wait for about five seconds.. enough time to inform the proxy servers
                                        NetworkImpl.SendDeferred(Zs.ServerConnection, MessageTypes.P_ServerList, ZoneWriter.ToArray(), true, 5000);
                                    }

                                    ZnRemovals.Add(Zs);

                                    lock (HTTPModules.AdminServers.WorldServers)
                                    {
                                        HTTPModules.AdminServers.WorldServers.Remove(Zs.LinkedServer);
                                    }

                                    Zs.LinkedServer = null;

                                }
                            }
                        }
                    } // Zone Servers

                    if (ZnRemovals.Count > 0)
                    {
                        foreach (ZoneServer Zs in ZnRemovals)
                        {
                            ZoneServer.ZoneServers.Remove(Zs);
                        }

                        if (ZoneServer.MostFree == null)
                            ZoneServer.MostFree = (ZoneServer.ZoneServers.Count > 0) ? ZoneServer.ZoneServers.First.Value : null;
                    }


                    // Rebalance zones
                    if (HTTPModules.AdminGlobal.RebalanceRequest)
                    {
                        HTTPModules.AdminGlobal.RebalanceRequest = false;
                        ZoneInstance.MasterReBalance();
                    }

                } // Lock

                #region Frame Counter
                // Update server frametime
                // This is used to estimate the servers load
                int FrameTime = (int)(Server.MilliSecs() - LastFrameTick);
                LastFrameTick = Server.MilliSecs();
                int MSTotal = 0;
                for (int i = 9; i > 0; --i)
                {
                    LastFrameMS[i - 1] = LastFrameMS[i];
                    MSTotal += LastFrameMS[i - 1];
                }
                LastFrameMS[9] = FrameTime;
                MSTotal += FrameTime;

                AverageFT = MSTotal / 10;
                //Console.Title = ApplicationBuildType + ": FT=" + AverageFT.ToString();

                ++Frames;

                // Frame limiting.. for testing only!
                if (Debugging > 0 && AverageFT < 10000 && AverageFT >= 0) // Ten second cap is to stop dangerous values
                    System.Threading.Thread.Sleep((30 - AverageFT < 0 ? 0 : 30 - AverageFT));
                #endregion
            }


            Log.StopLog();
            System.Environment.Exit(0);
        }
    }
}
