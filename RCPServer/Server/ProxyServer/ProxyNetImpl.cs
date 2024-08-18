using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Scripting;
using Environment = System.Environment;
using System.Diagnostics;

namespace RCPServer
{
    public class NetworkImpl : Network
    {
        public static long HourlyTotalTX = 0;
        public static long HourlyTotalRX = 0;
        public static long HourlyPlayersTotal = 0;
        public static long HourlySnaps = 0;
        public static int StatsUpdateSnaps = 0;
        public static int[] HourlySetsTX = new int[24];
        public static int[] HourlySetsRX = new int[24];
        public static int[] HourlyPlayers = new int[24];
        public static uint LastSnap = 0;
        public static DateTime LastHour;
        public static Stopwatch PingInterval;

        public static void Init()
        {
            sendQueued = new SendQueuedDelegate(SendQueuedImpl);
            sendPartyUpdate = new SendPartyUpdateDelegate(SendPartyUpdateImpl);

            for (int i = 0; i < 24; ++i)
            {
                HourlySetsRX[i] = 0;
                HourlySetsTX[i] = 0;
                HourlyPlayers[i] = 0;
            }

            LastSnap = Server.MilliSecs();
            DateTime Now = DateTime.Now;
            LastHour = new DateTime(Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, 0);

            PingInterval = new Stopwatch();
            PingInterval.Start();
            
        }

        protected class QueuedPacket
        {
            public int Connection = 0;
            public int Destination = 0;
            public int PacketType = 0;
            public byte[] Pa = null;
            public bool ReliableFlag = false;
            public int PlayerFrom = 0;
            public QueuedPacket NextInQueue = null;
            public QueuedPacket PreviousInQueue = null;
            public uint PreviousSentTime = 0;

            public QueuedPacket()
            {
                QueuedList.AddLast(this);
            }

            public static LinkedList<QueuedPacket> QueuedList = new LinkedList<QueuedPacket>();
            public static LinkedList<QueuedPacket> QueuedDelete = new LinkedList<QueuedPacket>();

            public static void Delete(QueuedPacket item)
            {
                QueuedDelete.AddLast(item);
            }

            public static void Clean()
            {
                foreach (QueuedPacket item in QueuedDelete)
                {
                    QueuedList.Remove(item);
                }
                QueuedDelete.Clear();
            }
        }

        static Regex CommaSplit = new Regex("[,]");

        public static void SendQueuedImpl(int destination, int packetType, byte[] packet, bool reliable)
        {
            // Create packet
            QueuedPacket Q = new QueuedPacket();
            Q.Destination = destination;
            Q.PacketType = packetType;
            Q.Pa = packet;
            Q.ReliableFlag = reliable;
            Q.PreviousSentTime = Server.MilliSecs() - 8;

            // Attempt to find previous packet in queue
            foreach (QueuedPacket Q2 in QueuedPacket.QueuedList)
            {
                if (Q2.NextInQueue == null && Q2.Destination == destination)
                {
                    if (Q2 != Q)
                    {
                        Q2.NextInQueue = Q;
                        Q.PreviousInQueue = Q2;
                        break;
                    }
                }
            }
        }

        public static void SendPartyUpdateImpl(ActorInstance ai)
        {
            Party P = ai.PartyID;
            if (P != null)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (P.Players[i] != null)
                    {
                        PacketWriter Pa = new PacketWriter();
                        for (int j = 0; j < 8; ++j)
                        {
                            if (P.Players[j] != null && j != i)
                            {
                                Pa.Write((byte)(P.Players[j].Name.Length));
                                Pa.Write(P.Players[j].Name, false);
                            }
                        }
                        RCEnet.Send(P.Players[i].RNID, MessageTypes.P_PartyUpdate, Pa.ToArray(), true);
                    }
                }
            }
        }

        private static void HandleDefaultProxy(int messageID, RCE_Message M)
        {
            // Check clients
            Client Cl = Client.FromPeer(M.FromID);
            if (Cl == null)
            {
                // Could still be from a cluster node
                ClusterServerNode No = ClusterServerNode.FromPeer(M.FromID);
                if (No == null || M.MessageData == null)
                    return;

                // ID
                uint GCLID = M.MessageData.ReadUInt32();
                int ConnectionID = 0;

                if (GCLID > 0)
                {
                    Cl = Client.FromGCLID(GCLID);
                    if (Cl == null)
                    {
                        Log.WriteLine("Del: NO GCLID: " + M.MessageType.ToString());
                        return;
                    }

                    ConnectionID = Cl.Connection;
                }

                PacketWriter Pa = new PacketWriter();
                if(M.MessageData.Length > 4)
                    Pa.Write(M.MessageData.ReadBytes(M.MessageData.Length - 4), 0);

                // Server->Client (TX)
                HourlyTotalTX += M.MessageData.Length;

                RCEnet.Send(ConnectionID, messageID, Pa.ToArray(), true);

                return;
            }


            if (Cl.CurrentServer == null)
                return;

            PacketWriter R = new PacketWriter();
            R.Write(Cl.GCLID);
            if(M.MessageData != null && M.MessageData.Length > 0)
                R.Write(M.MessageData.ReadBytes(M.MessageData.Length), 0);

            // Client->Server (RX)
            if(M != null && M.MessageData != null)
                HourlyTotalRX += M.MessageData.Length;

            RCEnet.Send(Cl.CurrentServer.Connection, messageID, R.ToArray(), true);
        }

        public static void UpdateNetwork()
        {
            // DV: Removed, because it doesn't look like there is actually a client side version of this!
            //UpdatePing();

            // One second passed, so take a snapshot
            if (Server.MilliSecs() - LastSnap > 1000)
            {
                ++HourlySnaps;
                ++StatsUpdateSnaps;
                HourlyPlayersTotal += Client.Clients.Count;
                LastSnap = Server.MilliSecs();

                long HourAveragePL = HourlyPlayersTotal / HourlySnaps;
                long HourAverageTX = HourlyTotalTX / HourlySnaps;
                long HourAverageRX = HourlyTotalRX / HourlySnaps;

                HourlySetsTX[0] = (int)HourAverageTX;
                HourlySetsRX[0] = (int)HourAverageRX;
                HourlyPlayers[0] = (int)HourAveragePL;

                // Roughly 10 seconds has passed
                if (StatsUpdateSnaps == 10)
                {
                    StatsUpdateSnaps = 0;

                    // Create Stats packet
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write((byte)'P');
                    Pa.Write((byte)'S'); // S means 'Standard' meaning Current Average and Client Count and FrameTime
                    Pa.Write(Server.PrivateAddr.GetAddressBytes(), 0);
                    Pa.Write((ushort)Server.PrivatePort);
                    Pa.Write(HourlySetsTX[0]);
                    Pa.Write(HourlySetsRX[0]);
                    Pa.Write(Client.Clients.Count);
                    Pa.Write(ProxyServer.AverageFT);

                    // Send stats
                    if (Server.MasterConnection != 0)
                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerStat, Pa.ToArray(), true);

                }
            }

            // An hour passed, take an hourly snapshot
            TimeSpan HourPassed = DateTime.Now - LastHour;
            if (HourPassed.Hours >= 1)
            {
                for (int i = 23; i > 0; --i)
                {
                    HourlyPlayers[i] = HourlyPlayers[i - 1];
                    HourlySetsTX[i] = HourlySetsTX[i - 1];
                    HourlySetsRX[i] = HourlySetsRX[i - 1];
                }

                HourlyPlayers[0] = 0;
                HourlySetsTX[0] = 0;
                HourlySetsRX[0] = 0;
                HourlyPlayersTotal = 0;
                HourlyTotalTX = 0;
                HourlyTotalRX = 0;
                HourlySnaps = 0;

                // Create Stats packet
                PacketWriter Pa = new PacketWriter();
                Pa.Write((byte)'P');
                Pa.Write((byte)'H'); // H means 'hourly update
                Pa.Write(Server.PrivateAddr.GetAddressBytes(), 0);
                Pa.Write((ushort)Server.PrivatePort);
                for (int i = 0; i < 24; ++i)
                {
                    Pa.Write(HourlySetsTX[i]);
                    Pa.Write(HourlySetsRX[i]);
                    Pa.Write(HourlyPlayers[i]);
                }

                // Send stats
                if (Server.MasterConnection != 0)
                    RCEnet.Send(Server.MasterConnection, MessageTypes.P_ServerStat, Pa.ToArray(), true);

                DateTime Now = DateTime.Now;
                LastHour = new DateTime(Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, 0);
            }

            // Send off any queued messages
#if !DEBUG
            try
            {
#endif
                foreach (QueuedPacket Q in QueuedPacket.QueuedList)
                {
                    if (Q.PreviousInQueue == null)
                    {
                        if (Server.MilliSecs() - Q.PreviousSentTime >= 12)
                        {
                            // Send it
                            RCEnet.Send(Q.Destination, Q.PacketType, Q.Pa, Q.ReliableFlag);

                            // Tell next in queue when this one was sent
                            if (Q.NextInQueue != null)
                            {
                                Q.NextInQueue.PreviousSentTime = Server.MilliSecs();
                                Q.NextInQueue.PreviousInQueue = null;
                            }

                            QueuedPacket.Delete(Q);
                        }
                    }
                }
                QueuedPacket.Clean();

                // Incoming messages
                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    switch (M.MessageType)
                    {
                        case MessageTypes.P_StartGame:
                        case MessageTypes.P_FetchActors:
                        case MessageTypes.P_CreateAccount:
                        case MessageTypes.P_VerifyAccount:
                        case MessageTypes.P_FetchCharacter:
                        case MessageTypes.P_CreateCharacter:
                        case MessageTypes.P_DeleteCharacter:
                            {
                                HandleDefaultProxy(M.MessageType, M);

                                break;
                            }
                        case MessageTypes.P_FetchItems:
                        case MessageTypes.P_ChangeArea:
                        case MessageTypes.P_FetchUpdateFiles:
                        case MessageTypes.P_NewActor:
                        case MessageTypes.P_ActorGone:
                        case MessageTypes.P_StandardUpdate:
                        case MessageTypes.P_InventoryUpdate:
                        case MessageTypes.P_ChatMessage:
                        case MessageTypes.P_WeatherChange:
                        case MessageTypes.P_AttackActor:
                        case MessageTypes.P_ActorDead:
                        case MessageTypes.P_RightClick:
                        case MessageTypes.P_Dialog:
                        case MessageTypes.P_StatUpdate:
                        case MessageTypes.P_QuestLog:
                        case MessageTypes.P_GoldChange:
                        case MessageTypes.P_NameChange:
                        case MessageTypes.P_KnownSpellUpdate:
                        case MessageTypes.P_SpellUpdate:
                        case MessageTypes.P_CreateEmitter:
                        case MessageTypes.P_Sound:
                        case MessageTypes.P_AnimateActor:
                        case MessageTypes.P_ActionBarUpdate:
                        case MessageTypes.P_XPUpdate:
                        case MessageTypes.P_ScreenFlash:
                        case MessageTypes.P_Music:
                        case MessageTypes.P_ActorEffect:
                        case MessageTypes.P_Projectile:
                        case MessageTypes.P_PartyUpdate:
                        case MessageTypes.P_AppearanceUpdate:
                        case MessageTypes.P_SelectScenery:
                        case MessageTypes.P_ItemScript:
                        case MessageTypes.P_EatItem:
                        case MessageTypes.P_ItemHealth:
                        case MessageTypes.P_Jump:
                        case MessageTypes.P_Dismount:
                        case MessageTypes.P_FloatingNumber:
                        case MessageTypes.P_RepositionActor:
                        case MessageTypes.P_Speech:
                        case MessageTypes.P_ProgressBar:
                        case MessageTypes.P_BubbleMessage:
                        case MessageTypes.P_ScriptInput:
                        case MessageTypes.P_KickedPlayer:
                        case MessageTypes.P_ReAllocateIDs:
                        case MessageTypes.P_ChatTab:
                        case MessageTypes.P_SceneryInteract:
                        case MessageTypes.P_ShaderConstant:
                            {
                                HandleDefaultProxy(M.MessageType, M);

                                break;
                            }
                        case MessageTypes.P_OpenForm:
                        case MessageTypes.P_PropertiesUpdate:
                        case MessageTypes.P_GUIEvent:
                        case MessageTypes.P_CloseForm:
                            {
                                HandleDefaultProxy(M.MessageType, M);

                                break;
                            }
                        case MessageTypes.P_ActorInfoCommand:
                            {
                                // Verify origin (so that clients can't feed in packets)
                                ClusterServerNode Node = ClusterServerNode.FromPeer(M.FromID);
                                if (Node != null && Node.IsZoneServer)
                                {
                                    // Redirect
                                    uint GCLID = M.MessageData.PeekUInt32();
                                    Client Cl = Client.FromGCLID(GCLID);
                                    if (Cl != null)
                                    {
                                        if (Cl.CurrentServer != null && Cl.CurrentServer.Connection != 0)
                                        {
                                            RCEnet.Send(Cl.CurrentServer.Connection, MessageTypes.P_ActorInfoCommand, M.MessageData.GetData(), true);
                                        }
                                    }
                                }


                                break;
                            }
                        case MessageTypes.P_ChangeAreaRequest:
                            {
                                // Server is telling us to connect to a new zone server
                                M.MessageData.Location = 1;
                                uint GCLID = M.MessageData.ReadUInt32();

                                string AreaName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                ushort InstanceID = M.MessageData.ReadUInt16();

                                ProxyZoneInstance Zn = ProxyZoneInstance.Find(AreaName, InstanceID);
                                if (Zn != null)
                                {
                                    Client Cl = Client.FromGCLID(GCLID);
                                    if (Cl != null)
                                    {
                                        Cl.CurrentServer = Zn.ServerHost;

                                        if (Cl.CurrentServer.Connection != 0)
                                        {
                                            RCEnet.Send(Cl.CurrentServer.Connection, MessageTypes.P_ChangeAreaRequest, M.MessageData.GetData(), true);
                                        }
                                        else
                                        {
                                            //TODO: Add client disconnect and account save
                                            Log.WriteLine(string.Format("Couldn't direct client '{0}' as zone server is offline!", Cl.GCLID));
                                        }
                                    }
                                    else
                                    {
                                        Log.WriteLine(string.Format("Couldn't direct client '{0}' to '{1}:{2}'", new object[] { GCLID, AreaName, InstanceID }));
                                    }
                                }
                                else
                                {
                                    Log.WriteLine("Critical Error: Could not locate zone: '" + AreaName + ":" + InstanceID.ToString() + "'");

                                    //TODO: Pass critical error to master server for logging!

                                    // Save player, at least
                                    M.MessageData.ReadInt32(); // Portal

                                    PacketWriter SaveRequest = new PacketWriter();
                                    SaveRequest.Write((byte)'S');
                                    SaveRequest.Write(GCLID);
                                    SaveRequest.Write(M.MessageData.ReadBytes(M.MessageData.Length - M.MessageData.Location), 0);

                                    RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientDrop, SaveRequest.ToArray(), true);

                                    Client Cl = Client.FromGCLID(GCLID);
                                    if (Cl != null)
                                    {
                                        if (Cl.Connection != 0)
                                            RCEnet.Disconnect(Cl.Connection);

                                        Cl.Connection = 0;
                                        Client.Clients.Remove(Cl);
                                    }
                                }

                                break;
                            }

                        case MessageTypes.P_ServerList:
                            {
                                if (M.FromID != Server.MasterConnection)
                                {
                                    Log.WriteLine("Received P_ServerList from non-master connection. Ignoring");
                                    break;
                                }

                                while (M.MessageData != null && M.MessageData.Location <= M.MessageData.Length - 1)
                                {
                                    byte Cmd = M.MessageData.ReadByte();

                                    if (Cmd == (byte)'A')
                                    {
                                        byte SType = M.MessageData.ReadByte();
                                        bool IsZoneServer = (SType == ((byte)'Z'));
                                        System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                        ushort Port = M.MessageData.ReadUInt16();
                                        string MachineName = M.MessageData.ReadString();


                                        ClusterServerNode Node = ClusterServerNode.Find(Address, Port);
                                        bool Found = false;

                                        if (Node == null)
                                        {
                                            Node = new ClusterServerNode();
                                            Node.IsZoneServer = (SType == ((byte)'Z'));
                                            Node.Address = Address;
                                            Node.Port = Port;
                                            Node.MachineName = MachineName;
                                        }
                                        else
                                        {
                                            // Reconnect if necessary
                                            if (Node.Connection != 0)
                                                RCEnet.Disconnect(Node.Connection);

                                            Node.Connection = 0;
                                            Found = true;
                                        }

                                        // Attempt to connect
                                        Log.Write("Attempting to connect to " + (Node.IsZoneServer ? "Zone Server" : "Account Server") + ": " + Node.Address.ToString() + ":" + Node.Port.ToString() + "/" + Node.MachineName + " ");

                                        Node.Connection = RCEnet.Connect(Node.Address.ToString(), Node.Port);
                                        if (Node.Connection <= 0 && Node.Connection > -6)
                                        {
                                            Log.WriteLine(string.Format("Could not connect: '{0}'", new string[] { RCEnet.ConnectionErrors[Math.Abs(Node.Connection)] }), ConsoleColor.Red);
                                            Node.Connection = 0;
                                            Node.LastDisconnect = Server.MilliSecs();
                                        }
                                        else
                                        {
                                            Log.WriteOK();

                                            // Send message to ZS so it can add to an internal list for remote directed packets (ie ActorInfo.Output)
                                            if (Node.IsZoneServer)
                                            {
                                                PacketWriter ZSCon = new PacketWriter();
                                                ZSCon.Write(Server.PrivateAddr.GetAddressBytes(), 0, 4);
                                                ZSCon.Write((ushort)Server.PrivatePort);
                                                ZSCon.Write(Server.HostName, true);

                                                RCEnet.Send(Node.Connection, MessageTypes.P_ProxyConnected, ZSCon.ToArray(), true);
                                            }


                                            if (!Found)
                                                ClusterServerNode.Nodes.AddLast(Node);
                                        }

                                    }
                                    else if (Cmd == (byte)'D')
                                    {
                                        byte SType = M.MessageData.ReadByte();
                                        bool IsZoneServer = (SType == ((byte)'Z')) || (SType == ((byte)'S'));
                                        System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                        ushort Port = M.MessageData.ReadUInt16();

                                        LinkedListNode<ClusterServerNode> Node = ClusterServerNode.Nodes.First;
                                        while (Node != null)
                                        {
                                            if (Node.Value.Address.Equals(Address) && Node.Value.Port == Port && Node.Value.IsZoneServer == IsZoneServer)
                                            {

                                                if (SType == (byte)'Z')
                                                {
                                                    Log.WriteLine("Master: Disconnect from: " + Address.ToString() + ": " + Port.ToString() + "/" + Node.Value.MachineName, ConsoleColor.Yellow);

                                                    RCEnet.Disconnect(Node.Value.Connection);
                                                    Node.Value.Connection = 0;

                                                    ClusterServerNode.Nodes.Remove(Node);

                                                    Client.RemoveByServer(Node.Value);
                                                }
                                                else
                                                {
                                                    Node.Value.LastDisconnect = Server.MilliSecs();
                                                    Node.Value.Shutdown = true;
                                                }

                                                break;
                                            }

                                            Node = Node.Next;
                                        }
                                    }
                                    else if (Cmd == (byte)'U')
                                    {
                                        byte StrLen = M.MessageData.ReadByte();
                                        string ZoneName = M.MessageData.ReadString(StrLen);
                                        ushort InstanceID = M.MessageData.ReadUInt16();

                                        System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                        ushort Port = M.MessageData.ReadUInt16();

                                        // Find the server
                                        ClusterServerNode Node = ClusterServerNode.Find(Address, Port);
                                        if (Node == null)
                                        {
                                            ProxyZoneInstance Zi = ProxyZoneInstance.Find(ZoneName, InstanceID);

                                            if (Address.Equals(System.Net.IPAddress.None))
                                            {
                                                if (Zi != null)
                                                    Zi.ServerHost = null;

                                                Log.WriteLine(string.Format("Zone ({0}:{1}) is now on {2}", new string[] { ZoneName, InstanceID.ToString(), System.Net.IPAddress.None.ToString() }));
                                            }
                                            else
                                            {
                                                Log.WriteLine(string.Format("Error: Couldn't find server '{0}:{1}' for zone '{2}'", new string[] { Address.ToString(), Port.ToString(), ZoneName }));
                                            }
                                        }
                                        else
                                        {
                                            ProxyZoneInstance Zi = ProxyZoneInstance.FindOrCreate(ZoneName, InstanceID);
                                            Zi.ServerHost = Node;

                                            Log.WriteLine(string.Format("Zone ({0}:{1}) is now on {2}", new string[] { ZoneName, InstanceID.ToString(), Node.MachineName }));
                                        }

                                    }
                                    else if (Cmd == (byte)'R')
                                    {
                                        byte StrLen = M.MessageData.ReadByte();
                                        string ZoneName = M.MessageData.ReadString(StrLen);
                                        ushort InstanceID = M.MessageData.ReadUInt16();

                                        System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                        ushort Port = M.MessageData.ReadUInt16();

                                        // Find the server
                                        ProxyZoneInstance Zi = ProxyZoneInstance.Find(ZoneName, InstanceID);
                                        if (Zi != null)
                                        {
                                            ProxyZoneInstance.ZoneInstances.Remove(Zi);
                                            Zi.ServerHost = null;
                                        }

                                        Log.WriteLine(string.Format("Zone ({0}:{1}) was removed", new string[] { ZoneName, InstanceID.ToString() }));
                                    }
                                    else if (Cmd == (byte)'S')
                                    {
                                        foreach (Client Cl in Client.Clients)
                                        {
                                            // Server Disconnect (with time to save)
                                            if (Cl.CurrentServer != null && Cl.CurrentServer.Connection != 0)
                                            {
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write((byte)'T');
                                                Pa.Write(Cl.GCLID);
                                                RCEnet.Send(Cl.CurrentServer.Connection, MessageTypes.P_ClientDrop, Pa.ToArray(), true);
                                            }

                                            // Client Disconnect
                                            if (Cl.Connection != 0)
                                            {
                                                RCEnet.Send(Cl.Connection, MessageTypes.P_KickedPlayer, new byte[] { }, true);
                                            }
                                        }

                                        // Disconnect timer
                                        ProxyServer.DisconnectTime = Server.MilliSecs();

                                        // Bye
                                        Server.IsRunning = false;
                                        Log.WriteLine("Shutdown by MasterServer request.");
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_ConnectInit:
                            {
                                if (M.Connection == Server.Host)
                                {
                                    // A new client connected and needs to negotiate a proxy connection
                                    // If I'm the master proxy, I need to as the master server what to do
                                    if (Server.IAmMasterProxy)
                                    {
                                        // No Master?
                                        if (Server.MasterConnection == 0)
                                        {
                                            PacketWriter Err = new PacketWriter();
                                            Err.Write((byte)'N');
                                            Err.Write((byte)'M');
                                            RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Err.ToArray(), true);
                                        }
                                        else
                                        {

                                            ClientInitList Li = new ClientInitList();
                                            Li.ConnectionID = M.FromID;
                                            Li.JoinID = M.MessageData.ReadInt32();
                                            Li.JoinTime = Server.MilliSecs();
                                            ClientInitList.WaitingClients.AddLast(Li);

                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(Li.JoinID);

                                            RCEnet.Send(Server.MasterConnection, MessageTypes.P_ConnectInit, Pa.ToArray(), true);
                                        }
                                    }
                                    else // I am a slave proxy
                                    {
                                        int JoinID = M.MessageData.ReadInt32();

                                        ClientInitList Li = ClientInitList.FromJoinID(JoinID);

                                        // Send 'accept' to client
                                        if (Li != null)
                                        {
                                            ClientInitList.WaitingClients.Remove(Li);

                                            ClusterServerNode Ac = ClusterServerNode.Find(Li.AcAddress, Li.AcPort);
                                            if (Ac == null)
                                            {
                                                Log.WriteLine("Dropped Client: Account Server not found! (" + Li.AcAddress.ToString() + ":" + Li.AcPort.ToString() + ")");
                                                PacketWriter Err = new PacketWriter();
                                                Err.Write((byte)'N');
                                                Err.Write((byte)'S');
                                                RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Err.ToArray(), true);

                                                break;
                                            }

                                            // Add new client
                                            Client Cl = new Client();
                                            Cl.Connection = Li.ConnectionID;
                                            Cl.GCLID = Li.GCLID;
                                            Cl.CurrentServer = Ac;
                                            Client.Clients.AddLast(Cl);

                                            PacketWriter Conf = new PacketWriter();
                                            Conf.Write((byte)'Y');
                                            RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Conf.ToArray(), true);


                                            Log.WriteLine("OK");

                                        }
                                        else // Wait for control packet from master
                                        {
                                            Li = new ClientInitList();
                                            Li.ConnectionID = M.FromID;
                                            Li.JoinID = JoinID;
                                            Li.JoinTime = Server.MilliSecs();
                                            ClientInitList.WaitingClients.AddLast(Li);

                                            Log.WriteLine("Waiting");
                                        }

                                    }
                                }
                                else if (M.FromID == Server.MasterConnection)
                                {
                                    PacketWriter Redirected = new PacketWriter();

                                    int JoinID = M.MessageData.ReadInt32();
                                    byte AckType = M.MessageData.ReadByte();

                                    ClientInitList Li = ClientInitList.FromJoinID(JoinID);

                                    if (AckType == (byte)'N')
                                    {
                                        Redirected.Write(AckType);
                                        Redirected.Write(M.MessageData.ReadByte());

                                        Log.WriteLine("DEL: Server Error");

                                        ClientInitList.WaitingClients.Remove(Li);
                                    }
                                    else if (AckType == (byte)'Y')
                                    {
                                        Redirected.Write(AckType);

                                        uint GCLID = M.MessageData.ReadUInt32();
                                        System.Net.IPAddress AcAddress = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                        ushort AcPort = M.MessageData.ReadUInt16();

                                        // Expect a client connection instead
                                        if (Li == null)
                                        {
                                            Li = new ClientInitList();
                                            Li.ConnectionID = 0;
                                            Li.JoinID = JoinID;
                                            Li.JoinTime = Server.MilliSecs();
                                            Li.GCLID = GCLID;
                                            Li.AcAddress = AcAddress;
                                            Li.AcPort = AcPort;
                                            ClientInitList.WaitingClients.AddLast(Li);

                                            Log.WriteLine("Del: Expect Connections");

                                            break;
                                        }
                                        else
                                        {
                                            ClientInitList.WaitingClients.Remove(Li);

                                            ClusterServerNode Ac = ClusterServerNode.Find(AcAddress, AcPort);
                                            if (Ac == null)
                                            {
                                                Log.WriteLine("Dropped Client: Account Server not found! (" + AcAddress.ToString() + ":" + AcPort.ToString() + ")");
                                                PacketWriter Conf = new PacketWriter();
                                                Conf.Write((byte)'N');
                                                Conf.Write((byte)'S');
                                                RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Conf.ToArray(), true);
                                                break;
                                            }

                                            // Add new client
                                            Client Cl = new Client();
                                            Cl.Connection = Li.ConnectionID;
                                            Cl.GCLID = GCLID;
                                            Cl.CurrentServer = Ac;
                                            Client.Clients.AddLast(Cl);
                                        }
                                    }
                                    else if (AckType == (byte)'R')
                                    {
                                        // Skip to cause the proxy to timeout
                                        Redirected.Write(AckType);
                                        Redirected.Write(M.MessageData.ReadBytes(4), 0, 4);
                                        Redirected.Write(M.MessageData.ReadUInt16());

                                        ClientInitList.WaitingClients.Remove(Li);
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    if (Li != null)
                                        RCEnet.Send(Li.ConnectionID, MessageTypes.P_ConnectInit, Redirected.ToArray(), true);
                                    else
                                        Log.WriteLine("Attempted to redirect connection message to missing client");
                                }

                                break;
                            }
                        case MessageTypes.P_ServerConnected:
                            {
                                byte AckType = M.MessageData.ReadByte();

                                // Letting us know we're registered
                                if (AckType == 108)
                                {
                                    Server.MasterAck = true;

                                    // This Ack was a reconnection
                                    if (Server.MasterLogAck)
                                    {
                                        Log.WriteLine("Reconnected to MasterServer", ConsoleColor.Green);
                                    }
                                }


                                break;
                            }
                        case RCEnet.PlayerTimedOut:
                        case RCEnet.PlayerHasLeft:
                        case RCEnet.PlayerKicked:
                            {
                                //                             if (M.MessageType == RCEnet.PlayerKicked)
                                //                             {
                                M.FromID = M.MessageData.ReadInt32();
                                //                             }
                                //                             else
                                //                             {
                                //                                 M.FromID = RCEnet.LastDisconnectedPeer();
                                //                             }

                                // Lost the master server, this is a catastropic failure!
                                if (M.FromID == Server.MasterConnection)
                                {
                                    Log.WriteLine("Warning: MasterServer lost... reconnecting...", ConsoleColor.Yellow);
                                    Server.MasterConnection = 0;
                                    break;
                                }

                                // Was it a client?
                                Client Cl = Client.FromPeer(M.FromID);
                                if (Cl != null)
                                {
                                    Cl.Connection = 0;

                                    if (Cl.CurrentServer != null && Cl.CurrentServer.Connection != 0)
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'T');
                                        Pa.Write(Cl.GCLID);
                                        RCEnet.Send(Cl.CurrentServer.Connection, MessageTypes.P_ClientDrop, Pa.ToArray(), true);
                                    }
                                }

                                LinkedListNode<ClusterServerNode> Node = ClusterServerNode.Nodes.First;
                                while (Node != null)
                                {
                                    if (Node.Value.Connection == M.FromID)
                                    {
                                        Log.WriteLine("Disconnected from: " + Node.Value.Address.ToString() + ":" + Node.Value.Port.ToString() + "/" + Node.Value.MachineName, ConsoleColor.Red);

                                        Node.Value.Connection = 0;
                                        Node.Value.LastDisconnect = Server.MilliSecs();

                                        break;
                                    }

                                    Node = Node.Next;
                                }


                                break;
                            }
                        case MessageTypes.P_ClientDrop:
                            {
                                byte CommandType = M.MessageData.ReadByte();

                                // Server didn't understand the actor, so it can't save
                                if (CommandType == (byte)'N')
                                {
                                    uint GCLID = M.MessageData.ReadUInt32();

                                    Client Cl = Client.FromGCLID(GCLID);
                                    if (Cl != null)
                                    {
                                        if (Cl.Connection != 0)
                                            RCEnet.Disconnect(Cl.Connection);

                                        Cl.Connection = 0;
                                        Client.Clients.Remove(Cl);
                                    }
                                }
                                else if (CommandType == (byte)'S') // Zone server wants the player saved
                                {
                                    uint GCLID = M.MessageData.ReadUInt32();

                                    // Save packet is just redirected to master server because only the master server
                                    // knows which account server to use.
                                    if (Server.MasterConnection == 0)
                                    {
                                        Log.WriteLine("Master Server offline, cannot save actor!");
                                    }
                                    else
                                    {
                                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientDrop, M.MessageData.GetData(), true);
                                    }

                                    // Remove client
                                    Client Cl = Client.FromGCLID(GCLID);
                                    if (Cl != null)
                                    {
                                        if (Cl.Connection != 0)
                                            RCEnet.Disconnect(Cl.Connection);

                                        Cl.Connection = 0;
                                        Client.Clients.Remove(Cl);
                                    }
                                }

                                break;
                            }

                        case MessageTypes.P_Ping:
                            {
                                Client Cl = Client.FromPeer(M.FromID);
                                if (Cl != null)
                                {
                                    Cl.ResetPing();
                                }
                            }
                            break;
                    }

                    RCE_Message.Delete(M);
                }
                RCE_Message.Clean();
#if !DEBUG

            }
            catch (Exception E)
            {
                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    Log.WriteLine("Packet dropped: " + M.MessageType.ToString() + " " + M.Connection.ToString() + " " + M.FromID.ToString());
                    RCE_Message.Delete(M);
                }
                RCE_Message.Clean();

            }      
#endif


        }

        private static void UpdatePing()
        {
            if (PingInterval.ElapsedMilliseconds > 5000)
            {
                List<Client> removeClients = new List<Client>();
                foreach (Client cl in Client.Clients)
                {
                    if (cl.IsTimedOut)
                    {
                        // Drop client
                        removeClients.Add(cl);
                        cl.Connection = 0;

                        if (cl.CurrentServer != null && cl.CurrentServer.Connection != 0)
                        {
                            PacketWriter Pa = new PacketWriter();
                            Pa.Write((byte)'T');
                            Pa.Write(cl.GCLID);
                            RCEnet.Send(cl.CurrentServer.Connection, MessageTypes.P_ClientDrop, Pa.ToArray(), true);
                        }
                        Log.WriteLine("Client Timed Out: " + cl.GCLID);
                        continue;
                    }

                    if (!cl.IsWaitingForPong)
                    {
                        cl.ResetPing();
                        cl.IsWaitingForPong = true;
                        PacketWriter pa = new PacketWriter();
                        RCEnet.Send(cl.Connection, MessageTypes.P_Ping, pa.ToArray(), true);
                    }
                }

                foreach (Client cl in removeClients)
                {
                    if (cl.Connection != 0)
                        RCEnet.Disconnect(cl.Connection);

                    cl.Connection = 0;
                    Client.Clients.Remove(cl);
                }

                PingInterval.Reset();
                PingInterval.Start();
            }

        }
    }
}
