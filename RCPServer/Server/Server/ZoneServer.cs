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
    class ZoneServer
    {
        public static string ApplicationName = "Realm Crafter Professional Server";
        public static string ApplicationVersion = "2.53 (Release)";
        public static string ApplicationBuildType = "Zone Server";

        public const bool UseDebugger = false;
        public static int Debugging = 0;
        public static string LastDebug = "";

        protected static uint StatsTimer = 0;
        protected static int[] LastFrameMS = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected static uint LastFrameTick = 0;
        public static int AverageFT = 0;
        protected static int Frames = 0;

        public static uint DisconnectTime = 0;

        static string ServerPassword = "testsds";

        static void Main(string[] args)
        {
#region Server Startup
            ServerPassword = SecretString.LoadSecretString();

            if (args.Length > 0 && args[0].Equals("-testingServer", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Title = "ZONE";

                Debugging = RCEnet.Connect("localhost", 24999);

                Log.StartLog("ZoneLog.txt", true);
                Log.WriteLine("Starting " + ApplicationName + " (" + ApplicationVersion + ") (" + ApplicationBuildType + ")");
            }
            else
            {
                Log.StartLog("ZoneLog.txt", false);
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
                Server.LoadClusterInfo("ZoneServer.xml");

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

                Log.Write("Loading SuperGlobals           ");
                SuperGlobals.Load("Data/Server Data/Superglobals.dat");
                Log.WriteOK();

                Log.Write("Loading Areas                  ");
                int AreaCount = Area.Load("Data/Server Data/Areas/");
                if (AreaCount == 0)
                    throw new Exception("At least one zone is required");
                Log.WriteOK("(" + AreaCount.ToString() + " Areas)");
                Server.UpdateArea = Area.AreaList.First;

                Log.Write("Loading Misc Data              ");
                Server.LoadData();
                Log.WriteOK();

                Log.Write("Loading Environment            ");
                WorldEnvironment.Load("Data/Server Data/Environment.dat");
                Log.WriteOK();

                Log.WriteLine("Loading Scripts...             ");
                ScriptingCoreCommands.Initialize();
                int ScriptCount = ScriptManager.Load("Data/Server Data/Scripts/", "zone:");
                Log.Write("Loading Scripts                ");
                if (ScriptCount == -1)
                    throw new Exception("Some scripts did not compile");
                ScriptManager.ExceptionCaught += new ScriptExceptionHandler(ScriptManager_ExceptionCaught);
                Log.WriteOK("(" + ScriptCount.ToString() + " Scripts)");

                Log.Write("Loading ChatProcessor          ");
                Server.ChatProcessor = ScriptManager.InstantiateSpecialScriptObject(typeof(Scripting.IChatProcessor));
                if (Server.ChatProcessor == null)
                    throw new Exception("Could not find an IChatProcessor object!");
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
                if (Server.MasterConnection <= 0 && Server.MasterConnection > -6)
                    throw new Exception(string.Format("Could not connect to master server ({0}:{1}): '{2}')", new string[] { Server.MasterAddr.ToString(), Server.MasterPort.ToString(), RCEnet.ConnectionErrors[Math.Abs(Server.MasterConnection)] }));

                // Send details
                PacketWriter Mas = new PacketWriter();
                Mas.Write((byte)3);
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
                    DBGPkt.Write((byte)'Z');
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
            if (!ScriptManager.Execute("ScriptingGlobal", "OnStartup", null, null, null))
            {
                Log.WriteLine("ScriptingGlobal.OnStartup() not found.. continuing...");
            }
#endregion

//             ActorInstance AI = new ActorInstance();
//             ScriptManager.Execute("TestScript", "Main", null, AI, null);
// 
//             Scripting.PacketWriter Pw = new Scripting.PacketWriter();
//             Scripting.ScriptBase.SavePersistentInstances(ref Pw, AI);
// 
//             Scripting.PacketReader Pr = new Scripting.PacketReader(Pw.ToArray());
//             Scripting.ScriptBase.LoadPersistentInstance(ref Pr, AI);
//                 

            LastFrameTick = Server.MilliSecs();
            StatsTimer = Server.MilliSecs();

            // Server loaded, main loop now
            while (Server.IsRunning || (DisconnectTime > 0 && Server.MilliSecs() - DisconnectTime < 5000))
            {
                ActorInstance.Clean();
                //GC.Collect();
                RCEnet.Update();
                NetworkImpl.UpdateNetwork();

                // Update zone instance requests
                ActorInfo.Update();

                if (Debugging >= 0)
                {
                    string HTMLLog = Log.HTMLLog();
                    if (!HTMLLog.Equals(LastDebug))
                    {
                        PacketWriter DBGPkt = new PacketWriter();
                        DBGPkt.Write((byte)'Z');
                        DBGPkt.Write(HTMLLog, true);
                        LastDebug = HTMLLog;

                        RCEnet.Send(Debugging, MessageTypes.P_ServerStat, DBGPkt.ToArray(), true);
                    }
                }

                #region Master Update
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
                        Mas.Write((byte)3);
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
                #endregion

                #region Zone Update
                // Update one zone per loop to save CPU time
                LinkedListNode<Area> Current = Server.UpdateArea;
                while (true)
                {
                    if(Current != null)
                        Current = Current.Next;
                    if (Current == null)
                        Current = Area.AreaList.First;

                    break;
                }
                Server.UpdateArea = Server.UpdateArea.Next;
                if (Server.UpdateArea == null)
                    Server.UpdateArea = Area.AreaList.First;
                Area UpdateArea = Server.UpdateArea.Value;

                foreach (KeyValuePair<int, Area.AreaInstance> KvInstance in UpdateArea.Instances)
                {
                    Area.AreaInstance Instance = KvInstance.Value;

                    if (Instance != null && Instance.Active)
                    {
                        // Weather
                        //Instance.UpdateWeather();

                        int PlayersInInstance = 0;

                        foreach (Area.InstanceSector Sector in Instance.Sectors)
                        {
                            if (Sector.Active == true)
                            {
                                PlayersInInstance += Sector.Players.Count;

                                // Spawn points
                                for (int i = 0; i < Sector.Spawned.Length; ++i)
                                {
                                    if (Sector.Spawned[i] < 0)
                                        Sector.Spawned[i] = 0;

                                    if (Sector.Spawned[i] < Sector.Sector.Spawns[i].Max)
                                    {
                                        if (Sector.SpawnLast[i] == 0)
                                        {
                                            Sector.SpawnLast[i] = Server.MilliSecs();
                                        }
                                        else if (Server.MilliSecs() - Sector.SpawnLast[i] > Sector.Sector.Spawns[i].Frequency * 1000)
                                        {
                                            Actor ListActor = Actor.Actors[Sector.Sector.Spawns[i].ActorID];
                                            if (ListActor == null)
                                                continue;

                                            // Get the current sector
                                            Scripting.Math.SectorVector Pos = Sector.Sector.Spawns[i].Waypoint.Position;
                                            int SectorX = Pos.SectorX;
                                            int SectorZ = Pos.SectorZ;
                                            if (SectorX >= Instance.Sectors.GetLength(0))
                                                SectorX = Instance.Sectors.GetLength(0) - 1;
                                            if (SectorZ >= Instance.Sectors.GetLength(1))
                                                SectorZ = Instance.Sectors.GetLength(1) - 1;

                                            Area.InstanceSector CurrentSector = Instance.Sectors[SectorX, SectorZ];

                                            // JB: Commented line is for debugging, no one really cares who was spawned.
                                            //Log.WriteLine("Spawning AI actor: " + ListActor.Race + " in zone: " + UpdateArea.Name);
                                            //Log.WriteLine("WPID: " + Sector.Sector.Spawns[i].Waypoint.ID);
                                            ActorInstance AI = ActorInstance.CreateActorInstance(ListActor);
                                            AI.SpawnedSector = CurrentSector;
                                            AI.RNID = -1;
                                            AI.AssignRuntimeID();
                                            AI.SetArea(UpdateArea, KvInstance.Key, Sector.Sector.Spawns[i].Waypoint.ID);
                                            AI.Position.X += Server.Rnd(Sector.Sector.Spawns[i].Size / -2.0f, Sector.Sector.Spawns[i].Size / 2.0f);
                                            AI.Position.Z += Server.Rnd(Sector.Sector.Spawns[i].Size / -2.0f, Sector.Sector.Spawns[i].Size / 2.0f);
                                            ++Sector.Spawned[i];
                                            Sector.SpawnLast[i] = Server.MilliSecs();
                                            AI.SourceSP = i;
                                            AI.AIMode = Scripting.AIModes.AI_Patrol;
                                            AI.CurrentWaypoint = Sector.Sector.Spawns[i].Waypoint.ID;
                                            AI.Script = Sector.Sector.Spawns[i].Script;                                       
                                            AI.DeathScript = Sector.Sector.Spawns[i].Script;
                                            // If no spawn script is set use Global spawn
                                            if (Sector.Sector.Spawns[i].Script.Length > 0)
                                                ScriptManager.Execute(Sector.Sector.Spawns[i].Script, "OnSpawn", new object[] { AI }, AI, null);
                                            else
                                            {
                                                ScriptManager.Execute("Spawn", "OnSpawn", new object[] { AI }, AI, null);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Sector.SpawnLast[i] = 0;
                                    }
                                }

                                foreach (ActorInstance AI in ActorInstance.ActorInstanceList)
                                {
                                    if (AI.RuntimeID > -1 && AI.RNID > 0)
                                    {
                                        if (AI.ServerArea.Area == UpdateArea)
                                        {
                                            // Portals
                                            for (int i = 0; i < Sector.Sector.Portals.Length; ++i)
                                            {
                                                if (Sector.Sector.Portals[i].Name.Length > 0)
                                                {
                                                    if (Sector.Sector.Portals[i].LinkArea.Length > 0)
                                                    {
                                                        // Calculate distance
                                                        bool InPortal = false;

                                                        if (Sector.Sector.Portals[i].IsSquare)
                                                        {
                                                            InPortal = AI.Position.WithinSectorDimension(Sector.Sector.Portals[i].Position, Sector.Sector.Portals[i].Width, Sector.Sector.Portals[i].Depth)
                                                                && AI.Position.Y > Sector.Sector.Portals[i].Position.Y && AI.Position.Y < Sector.Sector.Portals[i].Position.Y + Sector.Sector.Portals[i].Height;
                                                        }
                                                        else
                                                        {
                                                            float Size = Sector.Sector.Portals[i].Width * Sector.Sector.Portals[i].Width;
                                                            Scripting.Math.SectorVector DistVec = AI.Position - Sector.Sector.Portals[i].Position;
                                                            float DistX = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                                            float DistZ = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                                            float DistY = Math.Abs(DistVec.Y);
                                                            float Dist = (DistX * DistX) + (DistY * DistY) + (DistZ * DistZ);

                                                            InPortal = Dist < Size;
                                                        }

                                                        // Inside portal area
                                                        if (InPortal && (AI.LastPortal != i || (Server.MilliSecs() - AI.LastPortalTime > Server.PortalLockTime)))
                                                        {
                                                            InPortal = false;

                                                            string Name = Sector.Sector.Portals[i].LinkArea;

                                                            foreach (Area Ar in Area.AreaList)
                                                            {
                                                                if (Ar.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
                                                                {
                                                                    Name = Sector.Sector.Portals[i].LinkName;
                                                                    int Port = 0;

                                                                    if (Name.Length > 0)
                                                                    {
                                                                        for (int pj = 0; pj < Ar.Portals.Length; ++pj)
                                                                        {
                                                                            if (Ar.Portals[pj].Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
                                                                            {
                                                                                Port = pj;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    AI.SetArea(Ar, 0, -1, Port);
                                                                    InPortal = true;
                                                                    break;
                                                                }
                                                            }
                                                            if (InPortal)
                                                                break;
                                                        }
                                                    }
                                                }
                                            }

                                            // Triggers
                                            for (int i = 0; i < Sector.Sector.Triggers.Length; ++i)
                                            {
                                                RCPServer.Area.Trigger Tr = Sector.Sector.Triggers[i];

                                                if (Tr.ScriptName.Length > 0)
                                                {
                                                    bool InTrigger = false;

                                                    if (Tr.IsSquare)
                                                    {
                                                        InTrigger = AI.Position.WithinSectorDimension(Tr.Position, Tr.Width, Tr.Depth)
                                                                && AI.Position.Y > Tr.Position.Y && AI.Position.Y < Tr.Position.Y + Tr.Height;
                                                    }
                                                    else
                                                    {
                                                        // Calculate distance
                                                        float Size = Tr.Width * Tr.Width;
                                                        Scripting.Math.SectorVector DistVec = AI.Position - Tr.Position;
                                                        float DistX = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                                        float DistZ = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                                        float DistY = Math.Abs(DistVec.Y);
                                                        float Dist = (DistX * DistX) + (DistY * DistY) + (DistZ * DistZ);

                                                        InTrigger = Dist < Size;
                                                    }

                                                    // Check if we are already inside the trigger
                                                    if (AI.InTriggers.Contains(Tr))
                                                    {
                                                        // Have we left it?
                                                        if (!InTrigger)
                                                        {
                                                            // Check if the script is already running (after OnEnter)
                                                            ScriptBase OpenScript = ScriptBase.FindScriptFromTags(AI, Tr);

                                                            if (OpenScript != null)
                                                                ScriptManager.ExecuteSpecialScriptObject(OpenScript, "OnExit", AI);
                                                            else
                                                                ScriptManager.Execute(Tr.ScriptName, "OnExit", AI, AI, Tr);

                                                            //Log.WriteLine("Left: " + i);

                                                            // Remove from list
                                                            AI.InTriggers.Remove(Tr);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // Are we inside it though?
                                                        if (InTrigger)
                                                        {
                                                            // Add to list
                                                            AI.InTriggers.Add(Tr);

                                                            // Run script
                                                            ScriptManager.Execute(Tr.ScriptName, "OnEnter", AI, AI, Tr);

                                                            //Log.WriteLine("Entered: " + i);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                } // ActorInstance loop
                            }
                        } // SectorLoop

                        if (PlayersInInstance > 0)
                            Instance.FirstEmpty = Server.MilliSecs();

                        // TODO: Also check if its 'unique' or sectorized?
                        // If its a spawned instance and its been empty for some time, then kill it
                        if (Instance.ID > 0 && PlayersInInstance == 0)
                        {
                            if (Instance.FirstEmpty == 0)
                            {
                                Instance.FirstEmpty = Server.MilliSecs();
                            }
                            else if (Server.MilliSecs() - Instance.FirstEmpty > 20000)
                            {
                                Instance.Area.RemoveInstance(Instance.ID, true);
                            }
                        }
                    }
                } // Area update
                #endregion

                // Update actor instances
		        if(Server.MilliSecs() - Server.LastWorldUpdate > Server.WorldUpdateMS)
                {
			        // Update actors with network broadcast
			        if(Server.MilliSecs() - Server.LastBroadcast > Server.BroadcastMS)
                    {
                        ActorInstance.UpdateActorInstances(true);
                        Server.LastBroadcast = Server.MilliSecs();
			        }else // Update actors without network broadcast
                    {
                        ActorInstance.UpdateActorInstances(false);
                    }
			        Server.LastWorldUpdate = Server.MilliSecs();
		        }

                // Perform world environment update
                WorldEnvironment.Update();

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
                    Pa.Write((byte)'Z');
                    Pa.Write((byte)'S');
                    Pa.Write(Server.PrivateAddr.GetAddressBytes(), 0);
                    Pa.Write((ushort)Server.PrivatePort);
                    Pa.Write(AverageFT);

                    LinkedList<Area.AreaInstance> OpenInstances = new LinkedList<Area.AreaInstance>();
                    foreach (Area Ar in Area.AreaList)
                    {
                        foreach (KeyValuePair<int, Area.AreaInstance> Inst in Ar.Instances)
                        {
                            if (Inst.Value.Active)
                                OpenInstances.AddLast(Inst.Value);
                        }
                    }

                    Pa.Write((ushort)OpenInstances.Count);
                    foreach (Area.AreaInstance AIn in OpenInstances)
                    {
                        Pa.Write((byte)AIn.Area.Name.Length);
                        Pa.Write(AIn.Area.Name, false);
                        Pa.Write((ushort)AIn.ID);

                        int PlayerCount = 0;

                        // This could be optimized in the future
                        foreach (Area.InstanceSector Sector in AIn.Sectors)
                        {
                            if (Sector.Active)
                                PlayerCount += Sector.Players.Count;
                        }

                        Pa.Write((ushort)PlayerCount);
                    }

                    if (Server.MasterConnection != 0)
                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerStat, Pa.ToArray(), true);
                }
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

        /// <summary>
        /// Capture script events
        /// </summary>
        /// <param name="e"></param>
        static void ScriptManager_ExceptionCaught(ScriptExceptionArgs e)
        {
            // Create a fail packet
            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)'Z');
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
            if(Server.MasterConnection != 0)
                RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerStat, Pa.ToArray(), true);
        }
    }
}
