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
    class Program
    {
        public static string ApplicationName = "Realm Crafter Professional Server";
        public static string ApplicationVersion = "2.53 (Release)";
        public static string ApplicationBuildType = "Account Server";

        public const bool UseDebugger = false;
        public static int Debugging = 0;
        public static string LastDebug = "";

        protected static uint StatsTimer = 0;
        protected static int[] LastFrameMS = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected static uint LastFrameTick = 0;
        public static int AverageFT = 0;
        protected static int Frames = 0;

        static string ServerPassword = "test";

        static void Main(string[] args)
        {
            ServerPassword = SecretString.LoadSecretString();

            #region Server Startup
            if (args.Length > 0 && args[0].Equals("-testingServer", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Title = "ACCOUNT";

                Debugging = RCEnet.Connect("localhost", 24999);

                Log.StartLog("AccountLog.txt", true);
                Log.WriteLine("Starting " + ApplicationName + " (" + ApplicationVersion + ") (" + ApplicationBuildType + ")");
            }
            else
            {
                Log.StartLog("AccountLog.txt", false);
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
                // Load node information (0 = proxy, 1 = master, 2 = account, 3 = zone)
                Server.LoadClusterInfo("AccountServer.xml");

                Log.Write("Loading Language               ");
                Language.Load("Data/Server Data/Language.xml");
                Log.WriteOK();

                Log.Write("Loading Attributes             ");
                Attribute.Load("Data/Server Data/Attributes.dat");
                Log.WriteOK();

                Log.Write("Loading Damage Types           ");
                DamageType.Load("Data/Server Data/Damage.dat");
                Log.WriteOK();

                Log.Write("Loading Spells                 ");
                int SpellCount = Spell.Load(Spell.SpellsDatabase);
                Log.WriteOK("(" + SpellCount.ToString() + " Spells)");

                Log.Write("Loading Factions               ");
                int FactionCount = Faction.Load(Faction.FactionDatabase);
                Log.WriteOK("(" + FactionCount.ToString() + " Factions)");

                Log.Write("Loading Actors                 ");
                int ActorCount = Actor.Load(Actor.ActorsDatabase);
                Log.WriteOK("(" + ActorCount.ToString() + " Actors)");

                Log.Write("Loading Items                  ");
                int ItemCount = Item.Load(Item.FileDirectory);
                Log.WriteOK("(" + ItemCount.ToString() + " Items)");

                Log.Write("Loading Projectiles            ");
                int ProjectileCount = Projectile.Load("Data/Server Data/Projectiles.dat");
                Log.WriteOK("(" + ProjectileCount.ToString() + " Projectiles)");

                // Areas list is never fully processed; just stored for portal and direction info.
                Log.Write("Loading Areas                  ");
                int AreaCount = Area.Load("Data/Server Data/Areas/");
                if (AreaCount == 0)
                    throw new Exception("At least one zone is required");
                Log.WriteOK("(" + AreaCount.ToString() + " Areas)");

                Log.Write("Loading Environment            ");
                WorldEnvironment.Load("Data/Server Data/Environment.dat");
                Log.WriteOK();

                Log.Write("Loading Misc Data              ");
                Server.LoadData();
                Log.WriteOK();

                Log.WriteLine("Loading Scripts...             ");
                ScriptingCoreCommands.Initialize();
                int ScriptCount = ScriptManager.Load("Data/Server Data/Scripts/", "acct:");
                Log.Write("Loading Scripts                ");
                if (ScriptCount == -1)
                    throw new Exception("Some scripts did not compile");
                ScriptManager.ExceptionCaught += new ScriptExceptionHandler(ScriptManager_ExceptionCaught);
                Log.WriteOK("(" + ScriptCount.ToString() + " Scripts)");

                Log.Write("Loading AccountDatabase        ");
                Server.AccountDatabase = ScriptManager.InstantiateSpecialScriptObject(typeof(Scripting.IAccountDatabase));
                if (Server.AccountDatabase == null)
                    throw new Exception("Could not find an IAccountDatabase object!");
                ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Initialize", new AccountAddEventHandler(Account.AddAccountCallback));
                Log.WriteOK();

                Log.Write("Starting Network               ");
                Server.Host = RCEnet.StartHost(Server.PrivatePort, 5000);
                if (Server.Host == 0)
                    throw new Exception("Could not open network!");
                NetworkImpl.Init();
                Log.WriteOK();

                Log.Write("Connecting to MasterServer     ");

                // Connect
                Server.MasterConnection = RCEnet.Connect(Server.MasterAddr.ToString(), Server.MasterPort);
                if(Server.MasterConnection <= 0 && Server.MasterConnection > -6)
                    throw new Exception(string.Format("Could not connect to master server ({0}:{1}): '{2}')", new string[] { Server.MasterAddr.ToString(), Server.MasterPort.ToString(), RCEnet.ConnectionErrors[Math.Abs(Server.MasterConnection)] }));

                // Send details
                PacketWriter Mas = new PacketWriter();
                Mas.Write((byte)1);
                Mas.Write((byte)0);
                Mas.Write(ServerPassword, true);
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

                Log.WriteOK();
            }
            catch (Exception e)
            {

                Log.WriteFail(e.Message);

                if (Debugging != 0)
                {
                    string HTMLLog = Log.HTMLLog();

                    PacketWriter DBGPkt = new PacketWriter();
                    DBGPkt.Write((byte)'A');
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
            StatsTimer = Server.MilliSecs();

            // Server loaded, main loop now
            while (Server.IsRunning)
            {
                ActorInstance.Clean();
                //GC.Collect();
                RCEnet.Update();
                NetworkImpl.UpdateNetwork();
                ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Update", new object[] { });
                Account.WaitInstance.Update();

                if (Debugging >= 0)
                {
                    string HTMLLog = Log.HTMLLog();
                    if (!HTMLLog.Equals(LastDebug))
                    {
                        PacketWriter DBGPkt = new PacketWriter();
                        DBGPkt.Write((byte)'A');
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
                        PacketWriter Mas = new PacketWriter();
                        Mas.Write((byte)1);
                        Mas.Write((byte)1);
                        Mas.Write(ServerPassword, true);
                        Mas.Write(Server.PrivateAddr.GetAddressBytes(), 0, 4);
                        Mas.Write((ushort)Server.PrivatePort);
                        Mas.Write(Server.HostName, true);

                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerConnected, Mas.ToArray(), true);

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

                #region Master Server Stats
                if (Server.MilliSecs() - StatsTimer > 10000)
                {
                    StatsTimer = Server.MilliSecs();

                    PacketWriter Pa = new PacketWriter();
                    Pa.Write((byte)'A');
                    Pa.Write((byte)'S');
                    Pa.Write(Server.PrivateAddr.GetAddressBytes(), 0);
                    Pa.Write((ushort)Server.PrivatePort);
                    Pa.Write(AverageFT);

                    if(Server.MasterConnection != 0)
                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerStat, Pa.ToArray(), true);
                }
                #endregion
            }

            Log.StopLog();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Capture script events
        /// </summary>
        /// <param name="e"></param>
        static void ScriptManager_ExceptionCaught(ScriptExceptionArgs e)
        {
            // Create a fail packet
            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)'A');
            Pa.Write((byte)'E');
            Pa.Write(Server.PrivateAddr.GetAddressBytes(), 0);
            Pa.Write((ushort)Server.PrivatePort);

            Pa.Write(e.Filename, true);
            Pa.Write(e.Line);
            Pa.Write(e.Collumn);
            Pa.Write(e.Message, true);

            string ActorName = "None";
            if (e.Tag1 != null && e.Tag1 is ActorInstance)
                ActorName = (e.Tag1 as ActorInstance).name;

            Pa.Write(ActorName, true);

            // Send
            if (Server.MasterConnection != 0)
                RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerStat, Pa.ToArray(), true);
        }
    }
}
