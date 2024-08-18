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
    class ProxyServer
    {
        public static string ApplicationName = "Realm Crafter Professional Server";
        public static string ApplicationVersion = "2.53 (Release)";
        public static string ApplicationBuildType = "Proxy Server";

        public const bool UseDebugger = false;
        public static int Debugging = 0;
        public static string LastDebug = "";


        protected static int[] LastFrameMS = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected static uint LastFrameTick = 0;
        public static int AverageFT = 0;
        protected static int Frames = 0;

        public static uint DisconnectTime = 0;

        static string ServerPassword = "test";

        static void Main(string[] args)
        {
            #region Server Startup
            ServerPassword = SecretString.LoadSecretString();

            if (args.Length > 0 && args[0].Equals("-testingServer", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Title = "PROXY";

                Debugging = RCEnet.Connect("localhost", 24999);

                Log.StartLog("ProxyLog.txt", true);
                Log.WriteLine("Starting " + ApplicationName + " (" + ApplicationVersion + ") (" + ApplicationBuildType + ")");
            }
            else
            {
                Log.StartLog("ProxyLog.txt", false);
                Log.WriteLine("Starting " + ApplicationName + " (" + ApplicationVersion + ") (" + ApplicationBuildType + ")");

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
                Server.LoadClusterInfo("ProxyServer.xml");

                Log.Write("Loading Language               ");
                Language.Load("Data/Server Data/Language.xml");
                Log.WriteOK();

                // Areas list is never fully processed; just stored for portal and direction info.
                Log.Write("Loading Areas                  ");
                int AreaCount = Area.Load("Data/Server Data/Areas/");
                if (AreaCount == 0)
                    throw new Exception("At least one zone is required");
                Log.WriteOK("(" + AreaCount.ToString() + " Areas)");

                Log.Write("Starting Network               ");
                Server.Host = RCEnet.StartHost(Server.PublicPort, 5000);
                if (Server.Host == 0)
                    throw new Exception("Could not open network!");
                NetworkImpl.Init();
                Log.WriteOK();

                // Write a whole line because the the master response arrives too fast on a local network ;)
                Log.WriteLine("Connecting to MasterServer     ");

                // Connect
                Server.MasterConnection = RCEnet.Connect(Server.MasterAddr.ToString(), Server.MasterPort);
                if (Server.MasterConnection <= 0 && Server.MasterConnection > -6)
                    throw new Exception(string.Format("Could not connect to master server ({0}:{1}): '{2}')", new string[] { Server.MasterAddr.ToString(), Server.MasterPort.ToString(), RCEnet.ConnectionErrors[Math.Abs(Server.MasterConnection)] }));

                // Send details
                PacketWriter Mas = new PacketWriter();
                Mas.Write((byte)2);
                Mas.Write((byte)0);
                Mas.Write(ServerPassword, true);
                Mas.Write(Server.PublicAddr.GetAddressBytes(), 0, 4);
                Mas.Write((ushort)Server.PublicPort);
                Mas.Write(Server.PrivateAddr.GetAddressBytes(), 0, 4);
                Mas.Write((ushort)Server.PrivatePort);
                Mas.Write(Server.HostName, true);

                RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerConnected, Mas.ToArray(), true);

                // Give the master server ten seconds to respond with an acknowledge
                Server.MasterAckTime = Server.MilliSecs();

                while (Server.MasterAckTime + 10000 > Server.MilliSecs() && Server.MasterAck == false)
                {
                    RCEnet.Update();
                    NetworkImpl.UpdateNetwork();
                }

                if (Server.MasterAck == false)
                    throw new Exception("MasterServer did not respond to request");
                Server.MasterLogAck = true;

                //Log.WriteOK();
            }
            catch (Exception e)
            {

                Log.WriteFail(e.Message);

                if (Debugging != 0)
                {
                    string HTMLLog = Log.HTMLLog();

                    PacketWriter DBGPkt = new PacketWriter();
                    DBGPkt.Write((byte)'P');
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

            LastFrameTick = Server.MilliSecs();

            // Server loaded, main loop now
            while (Server.IsRunning || Server.MilliSecs() - DisconnectTime < 5000)
            {
                //GC.Collect();
                RCEnet.Update();
                NetworkImpl.UpdateNetwork();
                ClientInitList.UpdateClientInitList();
                ClusterServerNode.UpdateNodes();

                if (Debugging >= 0)
                {
                    string HTMLLog = Log.HTMLLog();
                    if (!HTMLLog.Equals(LastDebug))
                    {
                        PacketWriter DBGPkt = new PacketWriter();
                        DBGPkt.Write((byte)'P');
                        DBGPkt.Write(HTMLLog, true);
                        LastDebug = HTMLLog;

                        RCEnet.Send(Debugging, MessageTypes.P_ServerStat, DBGPkt.ToArray(), true);
                    }
                }

                if (Server.MasterConnection == 0 && Server.MasterReconnectTime + 10000 < Server.MilliSecs())
                {
                    // Connect
                    Server.MasterConnection = RCEnet.Connect(Server.MasterAddr.ToString(), Server.MasterPort);
                    if (Server.MasterConnection <= 0 && Server.MasterConnection > -6)
                    {
                        Log.WriteLine(string.Format("Could not reconnect to master server ({0}:{1}): '{2}')", new string[] { Server.MasterAddr.ToString(), Server.MasterPort.ToString(), RCEnet.ConnectionErrors[Math.Abs(Server.MasterConnection)] }), ConsoleColor.Yellow);
                        Server.MasterConnection = 0;
                        Server.MasterReconnectTime = Server.MilliSecs();
                    }
                    else
                    {
                        // Send details
                        PacketWriter Mas2 = new PacketWriter();
                        Mas2.Write((byte)2);
                        Mas2.Write((byte)1);
                        Mas2.Write(ServerPassword, true);
                        Mas2.Write(Server.PublicAddr.GetAddressBytes(), 0, 4);
                        Mas2.Write((ushort)Server.PublicPort);
                        Mas2.Write(Server.PrivateAddr.GetAddressBytes(), 0, 4);
                        Mas2.Write((ushort)Server.PrivatePort);
                        Mas2.Write(Server.HostName, true);

                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerConnected, Mas2.ToArray(), true);

                        // Give the master server ten seconds to respond with an acknowledge
                        Server.MasterAckTime = Server.MilliSecs();
                        Server.MasterAck = false;
                    }
                }
                // Reconnected, but not ack'd
                else if (Server.MasterConnection != 0 && Server.MasterAck == false && Server.MasterAckTime + 10000 < Server.MilliSecs())
                {
                    Log.WriteLine("MasterServer did not respond to request... retrying", ConsoleColor.Yellow);
                    RCEnet.Disconnect(Server.MasterConnection);
                    Server.MasterConnection = 0;
                    Server.MasterReconnectTime = Server.MilliSecs();
                }

                // Update scripts
                Scripting.Timer.Update();
                ScriptManager.Update();

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

            // Shutdown
            if (Server.MasterConnection != 0)
                RCEnet.Disconnect(Server.MasterConnection);

            // This will prevent invalid messages from being sent and also will prevent
            // a reconnect for 10 seconds (we'll have exited by then).
            Server.MasterConnection = 0;
            Server.MasterReconnectTime = Server.MilliSecs();

            Log.StopLog();
            System.Environment.Exit(0);
        }
    }
}
