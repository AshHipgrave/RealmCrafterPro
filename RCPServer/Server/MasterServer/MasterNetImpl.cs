using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Scripting;
using Environment = System.Environment;

namespace RCPServer
{
    public class NetworkImpl : Network
    {
        public static void Init()
        {
            sendQueued = new SendQueuedDelegate(SendQueuedImpl);
            sendPartyUpdate = new SendPartyUpdateDelegate(SendPartyUpdateImpl);
        }

        protected class QueuedPacket
        {
            public bool IsDeferred = false;
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
        static LinkedList<QueuedPacket> DeferredPackets = new LinkedList<QueuedPacket>();

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

        public static void SendDeferred(int destination, int packetType, byte[] packet, bool reliable, uint delay)
        {
            // Create packet
            QueuedPacket Q = new QueuedPacket();
            Q.Destination = destination;
            Q.PacketType = packetType;
            Q.Pa = packet;
            Q.ReliableFlag = reliable;
            Q.PreviousSentTime = Server.MilliSecs() + delay;
            Q.IsDeferred = true;

            DeferredPackets.AddLast(Q);
        }

        public static void SendPartyUpdateImpl(ActorInstance ai)
        {
            /* - New party stuff means client side for this won't work - Ben
            Party P = ai.PartyID;
            if (P != null)
            {
                for (int i = 0; i < P.Players.Count; ++i)
                {
                    if (P.Players[i] != null)
                    {
                        PacketWriter Pa = new PacketWriter();
                        for (int j = 0; j < P.Players.Count; ++j)
                        {
                            if (P.Players[j] != null)
                            {
                             
                                Pa.Write((byte)(P.Players[j].Name.Length));
                                Pa.Write(P.Players[j].Name, false);
                            }
                        }
                        RCEnet.Send(P.Players[i].RNID, MessageTypes.P_PartyUpdate, Pa.ToArray(), true);
                    }
                }
            }
             */
        }

        public static void UpdateNetwork()
        {
            // Send off any queued messages
            foreach (QueuedPacket Q in QueuedPacket.QueuedList)
            {
                if (Q.PreviousInQueue == null && !Q.IsDeferred)
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

            LinkedListNode<QueuedPacket> QPNode = DeferredPackets.First;
            while (QPNode != null)
            {
                LinkedListNode<QueuedPacket> QPDelete = null;

                QueuedPacket Q = QPNode.Value;
                if (Server.MilliSecs() > Q.PreviousSentTime)
                {
                    // Send it
                    RCEnet.Send(Q.Destination, Q.PacketType, Q.Pa, Q.ReliableFlag);
                    QueuedPacket.Delete(Q);
                    QPDelete = QPNode;
                }


                QPNode = QPNode.Next;
                if (QPDelete != null)
                    DeferredPackets.Remove(QPDelete);
            }

            QueuedPacket.Clean();

            // Incoming messages
#if !DEBUG
            try
            {
#endif
                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    switch (M.MessageType)
                    {
                        case MessageTypes.P_DebugMessage:
                            {
                                if (MasterServer.Debugging <= 0 | M.FromID != MasterServer.Debugging)
                                    break;

                                byte Cmd = M.MessageData.ReadByte();

                                // Shutdown
                                if (Cmd == (byte)'S')
                                {
                                    foreach (ZoneServer Zs in ZoneServer.ZoneServers)
                                    {
                                        if (Zs.LinkedServer != null)
                                        {
                                            lock (Zs.LinkedServer)
                                            {
                                                Zs.LinkedServer.DropServer = true;
                                            }
                                        }
                                    }

                                    MasterServer.DisconnectTime = Server.MilliSecs();
                                    MasterServer.DisconnectState = 0;
                                }

                                break;
                            }
                        case MessageTypes.P_ActorInfoRequest:
                            {
                                byte RequestType = M.MessageData.ReadByte();

                                // New
                                if (RequestType == (byte)'N')
                                {
                                    uint ZSAllocID = M.MessageData.ReadUInt32();
                                    string ActorName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                    Scripting.ActorInfoRequestState State = (Scripting.ActorInfoRequestState)M.MessageData.ReadByte();

                                    // Setup request
                                    MasterInfoRequest Request = new MasterInfoRequest();
                                    Request.ActorName = ActorName;
                                    Request.PostTime = Server.MilliSecs();
                                    Request.State = State;
                                    Request.ZSAllocID = ZSAllocID;

                                    MasterInfoRequest.Requests.AddLast(Request);

                                    // Send the request to the correct servers, check 'online' first
                                    if (State == ActorInfoRequestState.Online || State == ActorInfoRequestState.All)
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'F');
                                        Pa.Write(Request.AllocID);
                                        Pa.Write((byte)ActorName.Length);
                                        Pa.Write(ActorName, false);
                                        byte[] PaA = Pa.ToArray();

                                        foreach (ZoneServer Zs in ZoneServer.ZoneServers)
                                        {
                                            if (Zs.ServerConnection == M.FromID)
                                            {
                                                Request.Server = Zs;
                                            }

                                            if (Zs.ServerConnection != 0)
                                            {
                                                RCEnet.Send(Zs.ServerConnection, MessageTypes.P_ActorInfoRequest, PaA, true);
                                                ++Request.WaitingForServers;
                                            }
                                        }
                                    }

                                    // AccountServer request
                                    if (State == ActorInfoRequestState.Offline || State == ActorInfoRequestState.All)
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'F');
                                        Pa.Write(Request.AllocID);
                                        Pa.Write((byte)ActorName.Length);
                                        Pa.Write(ActorName, false);
                                        byte[] PaA = Pa.ToArray();

                                        if (AccountServer.MostFree != null && AccountServer.MostFree.ServerConnection != 0)
                                        {
                                            RCEnet.Send(AccountServer.MostFree.ServerConnection, MessageTypes.P_ActorInfoRequest, PaA, true);
                                            ++Request.WaitingForServers;
                                        }
                                    }
                                }
                                // Reply from ZoneServer
                                else if (RequestType == (byte)'Z')
                                {
                                    byte Acceptance = M.MessageData.ReadByte();
                                    uint AllocID = M.MessageData.ReadUInt32();

                                    MasterInfoRequest Request = MasterInfoRequest.FromAllocID(AllocID);

                                    if (Request != null)
                                    {
                                        if (Acceptance == (byte)'Y')
                                        {
                                            byte[] IPData = M.MessageData.ReadBytes(6);
                                            Request.InfoData = M.MessageData.ReadBytes(M.MessageData.ReadUInt16());

                                            // 'Online' is the best possible response, so write back to originating server
                                            if (Request.Server != null && Request.Server.ServerConnection != 0)
                                            {
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write((byte)'R');
                                                Pa.Write(Request.ZSAllocID);
                                                Pa.Write((byte)Scripting.ActorInfoState.Online); // The ZS *can* post an offline, but it wont
                                                Pa.Write(IPData);
                                                Pa.Write((ushort)Request.InfoData.Length);
                                                Pa.Write(Request.InfoData);

                                                RCEnet.Send(Request.Server.ServerConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);
                                            }

                                            // Remove it, its dealt with
                                            MasterInfoRequest.Requests.Remove(Request);
                                        }
                                        else
                                        {
                                            // ZS said no, so count is as replied
                                            --Request.WaitingForServers;

                                            // Received all possible responses? its a NotFound then
                                            if (Request.WaitingForServers == 0)
                                            {
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write((byte)'R');
                                                Pa.Write(Request.ZSAllocID);
                                                if (Request.InfoData != null)
                                                {
                                                    Pa.Write((byte)Scripting.ActorInfoState.Offline);
                                                    Pa.Write((uint)0); Pa.Write((ushort)0);
                                                    Pa.Write((ushort)Request.InfoData.Length);
                                                    Pa.Write(Request.InfoData);
                                                }
                                                else
                                                {
                                                    Pa.Write((byte)Scripting.ActorInfoState.NotFound);
                                                }

                                                if (Request.Server != null && Request.Server.ServerConnection != 0)
                                                    RCEnet.Send(Request.Server.ServerConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);

                                                MasterInfoRequest.Requests.Remove(Request);
                                            }
                                        }


                                    }
                                }
                                // Reply from AccountServer
                                else if (RequestType == (byte)'A')
                                {
                                    byte Acceptance = M.MessageData.ReadByte();
                                    uint AllocID = M.MessageData.ReadUInt32();

                                    MasterInfoRequest Request = MasterInfoRequest.FromAllocID(AllocID);

                                    if (Request != null)
                                    {
                                        // Store intermediate data if the account is offline. If a ZoneServer returns 'yes' then this
                                        // data is overridden, if the player really is offline though, then we have basic information to pass
                                        if (Acceptance == (byte)'Y')
                                        {
                                            Request.InfoData = M.MessageData.ReadBytes(M.MessageData.ReadUInt16());
                                        }



                                        // Regardless of the response, we should register our completion.
                                        --Request.WaitingForServers;

                                        // Received all possible responses? its a NotFound then
                                        if (Request.WaitingForServers == 0)
                                        {
                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write((byte)'R');
                                            Pa.Write(Request.ZSAllocID);

                                            if (Request.InfoData != null)
                                            {
                                                Pa.Write((byte)Scripting.ActorInfoState.Offline);
                                                Pa.Write((uint)0); Pa.Write((ushort)0);
                                                Pa.Write((ushort)Request.InfoData.Length);
                                                Pa.Write(Request.InfoData);
                                            }
                                            else
                                            {
                                                Pa.Write((byte)Scripting.ActorInfoState.NotFound);
                                            }

                                            if (Request.Server != null && Request.Server.ServerConnection != 0)
                                                RCEnet.Send(Request.Server.ServerConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);

                                            MasterInfoRequest.Requests.Remove(Request);
                                        }



                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_RemoveInstance:
                            {
                                // ZoneServer had an empty instance, so its removing it
                                string Name = M.MessageData.ReadString(M.MessageData.ReadByte());
                                ushort InstanceID = M.MessageData.ReadUInt16();

                                // Find instance
                                ZoneInstance Inst = ZoneInstance.Find(Name, InstanceID);

                                ZoneServer Zs = ZoneServer.FromConnection(M.FromID);

                                // If this is a request coming from a different server (zone was MOVED)
                                if (Inst.ServerHost != Zs)
                                {
                                    PacketWriter NewZoneWriter = new PacketWriter();

                                    NewZoneWriter.Write((byte)'A');
                                    NewZoneWriter.Write((byte)Inst.InstanceOf.Name.Length);
                                    NewZoneWriter.Write(Inst.InstanceOf.Name, false);
                                    NewZoneWriter.Write((ushort)Inst.ID);

                                    int DataLeft = M.MessageData.Length - M.MessageData.Location;

                                    if (DataLeft > 1)
                                    {
                                        byte[] ZoneData = M.MessageData.ReadBytes(DataLeft);

                                        NewZoneWriter.Write(ZoneData, 0);

                                        // Also save backup
                                        WorldZone.Save(Inst, ZoneData);
                                    }
                                    else
                                        NewZoneWriter.Write(0);

                                    if (Zs != null && Zs.ServerConnection != 0)
                                        RCEnet.Send(Zs.ServerConnection, MessageTypes.P_ServerList, NewZoneWriter.ToArray(), true);


                                }
                                else // Its just an empty instance shutdown
                                {

                                    // Close it on child servers
                                    PacketWriter ProxyWriter = new PacketWriter();

                                    ProxyWriter.Write((byte)'R');

                                    ProxyWriter.Write((byte)Inst.InstanceOf.Name.Length);
                                    ProxyWriter.Write(Inst.InstanceOf.Name, false);
                                    ProxyWriter.Write((ushort)Inst.ID);

                                    ProxyWriter.Write(System.Net.IPAddress.None.GetAddressBytes(), 0);
                                    ProxyWriter.Write((ushort)65535);

                                    PacketWriter ZoneWriter = new PacketWriter();

                                    ZoneWriter.Write((byte)'R');
                                    ZoneWriter.Write((byte)Name.Length);
                                    ZoneWriter.Write(Name, false);
                                    ZoneWriter.Write((ushort)InstanceID);

                                    byte[] ProxyWriterA = ProxyWriter.ToArray();
                                    foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                    {
                                        if (Pr.ServerConnection != 0)
                                            RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, ProxyWriterA, true);
                                    }

                                    if (Inst.ServerHost != null && Inst.ServerHost.ServerConnection != 0)
                                        RCEnet.Send(Inst.ServerHost.ServerConnection, MessageTypes.P_ServerList, ZoneWriter.ToArray(), true);

                                    // Save data, if necessary
                                    int DataLeft = M.MessageData.Length - M.MessageData.Location;

                                    if (DataLeft > 1)
                                        WorldZone.Save(Inst, M.MessageData.ReadBytes(DataLeft));
                                    ZoneInstance.ZoneInstances.Remove(Inst);

                                    // Server Log
                                    Log.WriteLine(string.Format("Removed Instance: {0}:{1}", Name, InstanceID));
                                }

                                break;
                            }
                        case MessageTypes.P_ServerStat:
                            {
                                byte ServerType = M.MessageData.ReadByte();
                                byte UpdateType = M.MessageData.ReadByte();

                                System.Net.IPAddress ServerAddr = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                ushort ServerPort = M.MessageData.ReadUInt16();

                                if (ServerType == (byte)'P') // Proxy Server
                                {
                                    // Find Server
                                    ProxyServer Pr = ProxyServer.FromAddress(ServerAddr, ServerPort);

                                    if (Pr != null)
                                    {
                                        // Standard update, current BW, Clients and FT
                                        if (UpdateType == (byte)'S')
                                        {
                                            Pr.HourlySetsTX[0] = M.MessageData.ReadInt32();
                                            Pr.HourlySetsRX[0] = M.MessageData.ReadInt32();
                                            Pr.ClientCount = M.MessageData.ReadInt32();
                                            Pr.HourlyClients[0] = Pr.ClientCount;
                                            Pr.AvgUpdate = M.MessageData.ReadInt32();

                                            if (ProxyServer.MostFree == null || Pr.AvgUpdate < ProxyServer.MostFree.AvgUpdate)
                                                ProxyServer.MostFree = Pr;
                                        }
                                        else if (UpdateType == (byte)'H') // Hourly update!
                                        {
                                            for (int i = 0; i < 24; ++i)
                                            {
                                                Pr.HourlySetsTX[i] = M.MessageData.ReadInt32();
                                                Pr.HourlySetsRX[i] = M.MessageData.ReadInt32();
                                                Pr.HourlyClients[i] = M.MessageData.ReadInt32();
                                            }
                                        }
                                    }
                                }
                                else if (ServerType == (byte)'A') // Account Server
                                {
                                    // Find Server
                                    AccountServer Ac = AccountServer.FromAddress(ServerAddr, ServerPort);

                                    if (Ac != null)
                                    {
                                        // Standard update
                                        if (UpdateType == (byte)'S')
                                        {
                                            Ac.AvgUpdate = M.MessageData.ReadInt32();

                                            if (AccountServer.MostFree == null || Ac.AvgUpdate < AccountServer.MostFree.AvgUpdate)
                                                AccountServer.MostFree = Ac;
                                        }
                                        else if (UpdateType == (byte)'E') // Exception Caught
                                        {
                                            string FileName = M.MessageData.ReadString();
                                            int LineNumber = M.MessageData.ReadInt32();
                                            int Collumn = M.MessageData.ReadInt32();
                                            string Message = M.MessageData.ReadString();
                                            string ActorName = M.MessageData.ReadString();

                                            lock (HTTPModules.ScriptExceptions.Instances)
                                            {
                                                HTTPModules.ScriptExceptions.Instances.Add(
                                                    new HTTPModules.ScriptExceptions.ExceptionInstance(Ac.MachineHostName,
                                                    ActorName, FileName, LineNumber, Collumn, Message));
                                            }

                                        }
                                    }
                                }
                                else if (ServerType == (byte)'Z') // Zone Server
                                {
                                    // Find Server
                                    ZoneServer Zs = ZoneServer.FromAddress(ServerAddr, ServerPort);

                                    if (Zs != null)
                                    {
                                        // Standard update
                                        if (UpdateType == (byte)'S')
                                        {
                                            Zs.AvgUpdate = M.MessageData.ReadInt32();
                                            Zs.ClientCount = 0;

                                            if (ZoneServer.MostFree == null || Zs.AvgUpdate < ZoneServer.MostFree.AvgUpdate)
                                                ZoneServer.MostFree = Zs;

                                            ushort InstanceCount = M.MessageData.ReadUInt16();
                                            for (int i = 0; i < InstanceCount; ++i)
                                            {
                                                string AreaName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                                ushort InstanceID = M.MessageData.ReadUInt16();
                                                ushort PlayerCount = M.MessageData.ReadUInt16();
                                                Zs.ClientCount += PlayerCount;

                                                ZoneInstance Zi = ZoneInstance.Find(AreaName, InstanceID);
                                                if (Zi != null)
                                                    Zi.ConnectedClients = PlayerCount;
                                            }
                                        }
                                        else if (UpdateType == (byte)'E') // Exception Caught
                                        {
                                            string FileName = M.MessageData.ReadString();
                                            int LineNumber = M.MessageData.ReadInt32();
                                            int Collumn = M.MessageData.ReadInt32();
                                            string Message = M.MessageData.ReadString();
                                            string ActorName = M.MessageData.ReadString();

                                            lock (HTTPModules.ScriptExceptions.Instances)
                                            {
                                                HTTPModules.ScriptExceptions.Instances.Add(
                                                    new HTTPModules.ScriptExceptions.ExceptionInstance(Zs.MachineHostName,
                                                    ActorName, FileName, LineNumber, Collumn, Message));
                                            }

                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_Instance:
                            {
                                byte Command = M.MessageData.ReadByte();

                                if (Command == (byte)'C')
                                {
                                    string Name = M.MessageData.ReadString(M.MessageData.ReadByte());
                                    ushort RequestedID = M.MessageData.ReadUInt16();

                                    ZoneInstance Inst = ZoneInstance.FindOrCreate(Name, RequestedID);

                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write((byte)'R');
                                    Pa.Write((byte)Name.Length);
                                    Pa.Write(Name, false);
                                    Pa.Write(RequestedID);

                                    if (Inst != null)
                                        Pa.Write((ushort)Inst.ID);
                                    else
                                        Pa.Write((ushort)65535);

                                    SendQueued(M.FromID, MessageTypes.P_Instance, Pa.ToArray(), true);

                                    Log.WriteLine("Created Instance: (" + Name + ":" + Inst.ID + ")");
                                }

                                break;
                            }
                        case MessageTypes.P_ConnectInit:
                            {
                                // Player connected, load balance proxies and direct user to an accounts server.

                                // JoinID contains a client ID that we use to identify him. This is a loose ID
                                // generated from a time value, its extremely unlikely to match another connecting
                                // client. If it does match one, then an incorrect client will be redirected,
                                // there cannot be a catastrophic fail.
                                int JoinID = M.MessageData.ReadInt32();

                                ProxyServer TargetProxy = ProxyServer.MostFree;
                                AccountServer TargetAccount = AccountServer.MostFree;
                          
                                if (TargetProxy == null || TargetAccount == null)
                                {
                                    PacketWriter Err = new PacketWriter();
                                    Err.Write(JoinID);
                                    Err.Write((byte)'N');
                                    Err.Write((byte)'S');
                                    RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Err.ToArray(), true);

                                    Log.WriteLine(string.Format("Denied client request! (Proxy = {0}, Account = {1})",
                                        TargetProxy == null ? "Invalid" : "OK",
                                        TargetAccount == null ? "Invalid" : "OK"));

                                    break;
                                }

                                // Its a confirmation, so also make a unique identifier
                                PacketWriter Conf = new PacketWriter();
                                Conf.Write(JoinID);

                                // User is connected to master already!
                                if (TargetProxy == ProxyServer.MasterProxy)
                                {
                                    Conf.Write((byte)'Y');
                                    Conf.Write(Server.AllocateGlobalClientID());
                                    Conf.Write(TargetAccount.Address.GetAddressBytes(), 0, 4);
                                    Conf.Write((ushort)TargetAccount.HostPort);

                                    RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Conf.ToArray(), true);
                                }
                                else
                                {
                                    Conf.Write((byte)'R');
                                    Conf.Write(TargetProxy.ExternalAddress.GetAddressBytes(), 0, 4);
                                    Conf.Write((ushort)TargetProxy.ExternalHostPort);

                                    RCEnet.Send(M.FromID, MessageTypes.P_ConnectInit, Conf.ToArray(), true);

                                    // Send direction to a waiting proxy
                                    PacketWriter Prep = new PacketWriter();
                                    Prep.Write(JoinID);
                                    Prep.Write((byte)'Y');
                                    Prep.Write(Server.AllocateGlobalClientID());
                                    Prep.Write(TargetAccount.Address.GetAddressBytes(), 0, 4);
                                    Prep.Write((ushort)TargetAccount.HostPort);

                                    RCEnet.Send(TargetProxy.ServerConnection, MessageTypes.P_ConnectInit, Prep.ToArray(), true);
                                }


                                break;
                            }

                        case MessageTypes.P_ServerConnected:
                            {
                                byte ServerType = M.MessageData.ReadByte();
                                byte ConnType = M.MessageData.ReadByte();
                                string password = M.MessageData.ReadString();

                                if (password != MasterServer.ServerPassword)
                                {
                                    Log.WriteLine("Warning! Server attempting to connect to Master Server that does not have the correct password!");
                                    break;
                                }
                               

                                // Account Server
                                if (ServerType == 1)
                                {
                                    System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                    ushort Port = M.MessageData.ReadUInt16();
                                    string HostName = M.MessageData.ReadString();

                                    if (ConnType == 1)
                                    {
                                        LinkedListNode<AccountServer> SNode = AccountServer.AccountServers.First;
                                        bool Found = false;

                                        while (SNode != null)
                                        {
                                            // Handled reconnect
                                            if (SNode.Value.Address.Equals(Address) && SNode.Value.HostPort == Port)
                                            {
                                                SNode.Value.ServerConnection = M.FromID;

                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write((byte)108);
                                                RCEnet.Send(M.FromID, MessageTypes.P_ServerConnected, Pa.ToArray(), true);

                                                Log.WriteLine(string.Format("AccountServer ({0}:{1}/{2}) Reconnected",
                                                    new string[] { Address.ToString(), Port.ToString(), HostName }), ConsoleColor.Green);
                                                Found = true;

                                                break;
                                            }

                                            SNode = SNode.Next;
                                        }

                                        if (!Found)
                                            ConnType = 0;
                                    }

                                    if (ConnType == 0)
                                    {
                                        AccountServer S = new AccountServer();
                                        S.MachineHostName = HostName;
                                        S.Address = Address;
                                        S.HostPort = Port;
                                        S.ServerConnection = M.FromID;                                  
                                        AccountServer.AccountServers.AddLast(S);

                                        lock (HTTPModules.AdminServers.AccountServers)
                                        {
                                            bool Found = false;

                                            foreach (HTTPModules.AdminServers.WorldServerInstance Wi in HTTPModules.AdminServers.AccountServers)
                                            {
                                                if (Wi.PrivateIP.Equals(S.Address.ToString()) && Wi.PrivatePort == S.HostPort)
                                                {
                                                    Found = true;
                                                    break;
                                                }
                                            }

                                            if (!Found)
                                            {
                                                RCPServer.HTTPModules.AdminServers.WorldServerInstance Inst = new RCPServer.HTTPModules.AdminServers.WorldServerInstance(
                                                    HostName, Address.ToString(), Port, Address.ToString(), Port, 0, 0, 0);

                                                HTTPModules.AdminServers.AccountServers.Add(Inst);
                                                S.LinkedServer = Inst;
                                            }
                                        }

                                        if (AccountServer.MostFree == null)
                                            AccountServer.MostFree = S;

                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)108);
                                        RCEnet.Send(S.ServerConnection, MessageTypes.P_ServerConnected, Pa.ToArray(), true);

                                        // Dispatch a List-Add to all proxy servers to maintain a connection to this account server.
                                        Pa = new PacketWriter();
                                        Pa.Write((byte)'A');
                                        Pa.Write((byte)'C');
                                        Pa.Write(Address.GetAddressBytes(), 0, 4);
                                        Pa.Write(Port);
                                        Pa.Write(HostName, true);
                                        byte[] PaA = Pa.ToArray();

                                        foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                        {
                                            if (Pr.ServerConnection != 0)
                                                RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                        }


                                        Log.WriteLine(string.Format("AccountServer ({0}:{1}/{2}) Connected",
                                            new string[] { Address.ToString(), Port.ToString(), HostName }), ConsoleColor.Green);
                                    }

                                }
                                else if (ServerType == 2)
                                {

                                    System.Net.IPAddress ExtAddress = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                    ushort ExtPort = M.MessageData.ReadUInt16();
                                    System.Net.IPAddress IntAddress = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                    ushort IntPort = M.MessageData.ReadUInt16();
                                    string HostName = M.MessageData.ReadString();

                                    if (ConnType == 1)
                                    {
                                        LinkedListNode<ProxyServer> SNode = ProxyServer.ProxyServers.First;
                                        bool Found = false;

                                        while (SNode != null)
                                        {
                                            // Handled reconnect
                                            if (SNode.Value.InternalAddress.Equals(IntAddress) && SNode.Value.InternalHostPort == IntPort)
                                            {
                                                SNode.Value.ServerConnection = M.FromID;

                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write((byte)108);
                                                RCEnet.Send(M.FromID, MessageTypes.P_ServerConnected, Pa.ToArray(), true);

                                                Log.WriteLine(string.Format("ProxyServer ({0}:{1}/{2}:{3}/{4}) Reconnected",
                                                    new string[] { IntAddress.ToString(), IntPort.ToString(), ExtAddress.ToString(), ExtPort.ToString(), HostName }), ConsoleColor.Green);
                                                Found = true;

                                                break;
                                            }

                                            SNode = SNode.Next;
                                        }

                                        if (!Found)
                                            ConnType = 0;
                                    }

                                    if (ConnType == 0)
                                    {
                                        ProxyServer S = new ProxyServer();
                                        S.MachineHostName = HostName;
                                        S.InternalAddress = IntAddress;
                                        S.InternalHostPort = IntPort;
                                        S.ExternalAddress = ExtAddress;
                                        S.ExternalHostPort = ExtPort;
                                        S.ServerConnection = M.FromID;
                                        ProxyServer.ProxyServers.AddLast(S);

                                        if (S.ExternalAddress.Equals(Server.MasterProxyAddr) && S.ExternalHostPort == Server.MasterProxyPort)
                                        {
                                            ProxyServer.MasterProxy = S;
                                        }

                                        lock (HTTPModules.AdminServers.ProxyServers)
                                        {
                                            bool Found = false;

                                            foreach (HTTPModules.AdminServers.WorldServerInstance Wi in HTTPModules.AdminServers.ProxyServers)
                                            {
                                                if (Wi.PrivateIP.Equals(IntAddress.ToString()) && Wi.PrivatePort == IntPort)
                                                {
                                                    Found = true;
                                                    break;
                                                }
                                            }

                                            if (!Found)
                                            {
                                                RCPServer.HTTPModules.AdminServers.WorldServerInstance Inst = new RCPServer.HTTPModules.AdminServers.WorldServerInstance(
                                                    HostName, ExtAddress.ToString(), ExtPort, IntAddress.ToString(), IntPort, 0, 0, 0);
                                                if (ProxyServer.MasterProxy == S)
                                                    Inst.Master = true;

                                                HTTPModules.AdminServers.ProxyServers.Add(Inst);
                                                S.LinkedServer = Inst;
                                            }
                                        }


                                        //if (ProxyServer.MostFree == null)
                                        ProxyServer.MostFree = S;

                                        PacketWriter Pa = new PacketWriter();
                                        foreach (AccountServer Ac in AccountServer.AccountServers)
                                        {
                                            // Dispatch a List-Add to all proxy servers to maintain a connection to this account server.
                                            Pa.Write((byte)'A');
                                            Pa.Write((byte)'C');
                                            Pa.Write(Ac.Address.GetAddressBytes(), 0, 4);
                                            Pa.Write(Ac.HostPort);
                                            Pa.Write(Ac.MachineHostName, true);

                                        }
                                        foreach (ZoneServer Zs in ZoneServer.ZoneServers)
                                        {
                                            // Dispatch a List-Add to all proxy servers to maintain a connection to this account server.
                                            Pa.Write((byte)'A');
                                            Pa.Write((byte)'Z');
                                            Pa.Write(Zs.Address.GetAddressBytes(), 0, 4);
                                            Pa.Write(Zs.HostPort);
                                            Pa.Write(Zs.MachineHostName, true);

                                        }
                                        byte[] PaA = Pa.ToArray();
                                        RCEnet.Send(S.ServerConnection, MessageTypes.P_ServerList, PaA, true);

                                        Pa = new PacketWriter();

                                        foreach (ZoneInstance Zi in ZoneInstance.ZoneInstances)
                                        {
                                            Pa.Write((byte)'U');
                                            Pa.Write((byte)Zi.InstanceOf.Name.Length);
                                            Pa.Write(Zi.InstanceOf.Name, false);
                                            Pa.Write((ushort)Zi.ID);

                                            if (Zi.ServerHost != null)
                                            {
                                                Pa.Write(Zi.ServerHost.Address.GetAddressBytes(), 0, 4);
                                                Pa.Write(Zi.ServerHost.HostPort);
                                            }
                                            else
                                            {
                                                Pa.Write(System.Net.IPAddress.None.GetAddressBytes(), 0, 4);
                                                Pa.Write((ushort)0);
                                            }
                                        }
                                        PaA = Pa.ToArray();

                                        foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                        {
                                            if (Pr.ServerConnection != 0)
                                                RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                        }

                                        Pa = new PacketWriter();
                                        Pa.Write((byte)108);
                                        RCEnet.Send(S.ServerConnection, MessageTypes.P_ServerConnected, Pa.ToArray(), true);

                                        Log.WriteLine(string.Format("ProxyServer ({0}:{1}/{2}:{3}/{4}) Connected",
                                            new string[] { IntAddress.ToString(), IntPort.ToString(), ExtAddress.ToString(), ExtPort.ToString(), HostName }), ConsoleColor.Green);
                                    }

                                }
                                else if (ServerType == 3) // Zone Server
                                {
                                    System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                    ushort Port = M.MessageData.ReadUInt16();
                                    string HostName = M.MessageData.ReadString();

                                    if (ConnType == 1)
                                    {
                                        LinkedListNode<ZoneServer> SNode = ZoneServer.ZoneServers.First;
                                        bool Found = false;

                                        while (SNode != null)
                                        {
                                            // Handled reconnect
                                            if (SNode.Value.Address.Equals(Address) && SNode.Value.HostPort == Port)
                                            {
                                                SNode.Value.ServerConnection = M.FromID;

                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write((byte)108);
                                                RCEnet.Send(M.FromID, MessageTypes.P_ServerConnected, Pa.ToArray(), true);

                                                Log.WriteLine(string.Format("ZoneServer ({0}:{1}/{2}) Reconnected",
                                                    new string[] { Address.ToString(), Port.ToString(), HostName }), ConsoleColor.Green);
                                                Found = true;

                                                if (ZoneServer.MostFree == null)
                                                    ZoneServer.MostFree = SNode.Value;

                                                break;
                                            }

                                            SNode = SNode.Next;
                                        }

                                        if (!Found)
                                            ConnType = 0;
                                    }

                                    if (ConnType == 0)
                                    {
                                        ZoneServer S = new ZoneServer();
                                        S.MachineHostName = HostName;
                                        S.Address = Address;
                                        S.HostPort = Port;
                                        S.ServerConnection = M.FromID;
                                        ZoneServer.ZoneServers.AddLast(S);

                                        lock (HTTPModules.AdminServers.WorldServers)
                                        {
                                            bool Found = false;

                                            foreach (HTTPModules.AdminServers.WorldServerInstance Wi in HTTPModules.AdminServers.WorldServers)
                                            {
                                                if (Wi.PrivateIP.Equals(S.Address.ToString()) && Wi.PrivatePort == S.HostPort)
                                                {
                                                    Found = true;
                                                    break;
                                                }
                                            }

                                            if (!Found)
                                            {
                                                RCPServer.HTTPModules.AdminServers.WorldServerInstance WSI = new RCPServer.HTTPModules.AdminServers.WorldServerInstance(
                                                    HostName, Address.ToString(), Port, Address.ToString(), Port, 0, 0, 0);
                                                S.LinkedServer = WSI;
                                                HTTPModules.AdminServers.WorldServers.Add(WSI);
                                            }
                                        }

                                        if (ZoneServer.MostFree == null)
                                            ZoneServer.MostFree = S;

                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)108);
                                        RCEnet.Send(S.ServerConnection, MessageTypes.P_ServerConnected, Pa.ToArray(), true);

                                        // Dispatch a List-Add to all proxy servers to maintain a connection to this account server.
                                        Pa = new PacketWriter();
                                        Pa.Write((byte)'A');
                                        Pa.Write((byte)'Z');
                                        Pa.Write(Address.GetAddressBytes(), 0, 4);
                                        Pa.Write(Port);
                                        Pa.Write(HostName, true);
                                        byte[] PaA = Pa.ToArray();

                                        foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                        {
                                            if (Pr.ServerConnection != 0)
                                                RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                        }

                                        // First zone server.. assign it ALL zones, and fresh instance data
                                        if (ZoneServer.ZoneServers.Count == 1)
                                        {
                                            // Proxy Assignments
                                            Pa = new PacketWriter();

                                            foreach (ZoneInstance Zi in ZoneInstance.ZoneInstances)
                                            {
                                                Zi.ServerHost = S;

                                                Pa.Write((byte)'U');
                                                Pa.Write((byte)Zi.InstanceOf.Name.Length);
                                                Pa.Write(Zi.InstanceOf.Name, false);
                                                Pa.Write((ushort)Zi.ID);
                                                Pa.Write(Address.GetAddressBytes(), 0, 4);
                                                Pa.Write(Port);
                                            }
                                            PaA = Pa.ToArray();

                                            foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                            {
                                                if (Pr.ServerConnection != 0)
                                                    RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                            }

                                            // ZoneServer Assignments
                                            Pa = new PacketWriter();

                                            foreach (ZoneInstance Zi in ZoneInstance.ZoneInstances)
                                            {
                                                Zi.ServerHost = S;

                                                Pa.Write((byte)'A');
                                                Pa.Write((byte)Zi.InstanceOf.Name.Length);
                                                Pa.Write(Zi.InstanceOf.Name, false);
                                                Pa.Write((ushort)Zi.ID);
                                                Pa.Write(WorldZone.Load(Zi), 0);

                                            }

                                            RCEnet.Send(S.ServerConnection, MessageTypes.P_ServerList, Pa.ToArray(), true);
                                        }

                                        Log.WriteLine(string.Format("ZoneServer ({0}:{1}/{2}) Connected",
                                            new string[] { Address.ToString(), Port.ToString(), HostName }), ConsoleColor.Green);
                                    }

                                }
                                else
                                {
                                    Log.WriteLine(string.Format("Unknown Server attempted to connect ({0})", ServerType.ToString()));
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

                                AccountServer LostAcc = null;
                                foreach (AccountServer S in AccountServer.AccountServers)
                                {
                                    if (S.ServerConnection == M.FromID)
                                    {
                                        Log.WriteLine(string.Format("AccountServer ({0}:{1}/{2}) Disconnected",
                                            new string[] { S.Address.ToString(), S.HostPort.ToString(), S.MachineHostName }), ConsoleColor.Red);

                                        LostAcc = S;
                                        break;
                                    }
                                }

                                if (LostAcc != null)
                                {
                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write((byte)'D');
                                    Pa.Write((byte)'C');
                                    Pa.Write(LostAcc.Address.GetAddressBytes(), 0, 4);
                                    Pa.Write(LostAcc.HostPort);
                                    byte[] PaA = Pa.ToArray();

                                    foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                    {
                                        if (Pr.ServerConnection != 0)
                                            RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                    }

                                    AccountServer.AccountServers.Remove(LostAcc);

                                    // Update HTTP Server
                                    lock (HTTPModules.AdminServers.AccountServers)
                                    {
                                        HTTPModules.AdminServers.WorldServerInstance WSIRemove = LostAcc.LinkedServer;

                                        if (WSIRemove != null)
                                        {
                                            HTTPModules.AdminServers.AccountServers.Remove(WSIRemove);
                                            LostAcc.LinkedServer = null;
                                        }
                                    }
                                }

                                ZoneServer LostZn = null;
                                foreach (ZoneServer Z in ZoneServer.ZoneServers)
                                {
                                    if (Z.ServerConnection == M.FromID)
                                    {
                                        Log.WriteLine(string.Format("ZoneServer ({0}:{1}/{2}) Disconnected",
                                            new string[] { Z.Address.ToString(), Z.HostPort.ToString(), Z.MachineHostName }), ConsoleColor.Red);

                                        LostZn = Z;
                                        break;
                                    }
                                }

                                if (LostZn != null)
                                {
                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write((byte)'D');
                                    Pa.Write((byte)'Z');
                                    Pa.Write(LostZn.Address.GetAddressBytes(), 0, 4);
                                    Pa.Write(LostZn.HostPort);
                                    byte[] PaA = Pa.ToArray();

                                    foreach (ProxyServer Pr in ProxyServer.ProxyServers)
                                    {
                                        if (Pr.ServerConnection != 0)
                                            RCEnet.Send(Pr.ServerConnection, MessageTypes.P_ServerList, PaA, true);
                                    }

                                    ZoneServer.ZoneServers.Remove(LostZn);

                                    // Update HTTP Server
                                    lock (HTTPModules.AdminServers.WorldServers)
                                    {
                                        HTTPModules.AdminServers.WorldServerInstance WSIRemove = LostZn.LinkedServer;

                                        if (WSIRemove != null)
                                        {
                                            HTTPModules.AdminServers.WorldServers.Remove(WSIRemove);
                                            LostZn.LinkedServer = null;
                                        }
                                    }
                                }

                                ProxyServer LostProx = null;
                                foreach (ProxyServer P in ProxyServer.ProxyServers)
                                {
                                    if (P.ServerConnection == M.FromID)
                                    {
                                        Log.WriteLine(string.Format("ProxyServer ({0}:{1}/{2}) Disconnected",
                                            new string[] { P.InternalAddress.ToString(), P.InternalHostPort.ToString(), P.MachineHostName }), ConsoleColor.Red);

                                        LostProx = P;
                                        break;
                                    }
                                }

                                if (LostProx != null)
                                {
                                    ProxyServer.ProxyServers.Remove(LostProx);

                                    // Update HTTP Server
                                    lock (HTTPModules.AdminServers.ProxyServers)
                                    {
                                        HTTPModules.AdminServers.WorldServerInstance WSIRemove = LostProx.LinkedServer;

                                        if (WSIRemove != null)
                                        {
                                            HTTPModules.AdminServers.ProxyServers.Remove(WSIRemove);
                                            LostProx.LinkedServer = null;
                                        }
                                    }
                                }



                                break;
                            }
                        case MessageTypes.P_ClientDrop:
                            {
                                byte CommandType = M.MessageData.ReadByte();

                                if (CommandType == (byte)'S') // Zone server wants the player saved
                                {
                                    // Packet is redirects to 'most free' account server
                                    RCEnet.Send(AccountServer.MostFree.ServerConnection, MessageTypes.P_ClientDrop, M.MessageData.GetData(), true);
                                }

                                break;
                            }
                        case MessageTypes.P_ClientInfo:
                            {
                                byte Command = M.MessageData.PeekByte();

                                if (Command == (byte)'O') // Valid account
                                {
                                    M.MessageData.ReadByte();
                                    string Username = M.MessageData.ReadString(M.MessageData.ReadByte());
                                    uint GCLID = M.MessageData.ReadUInt32();

                                    System.Net.IPAddress Address = System.Net.IPAddress.None;
                                    ushort Port = 0;
                                    string ZoneName = "None";
                                    ushort InstanceID = 0;

                                    if (M.MessageData.Length > M.MessageData.Location + 1)
                                    {
                                        Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                        Port = M.MessageData.ReadUInt16();
                                        ZoneName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                        InstanceID = M.MessageData.ReadUInt16();
                                    }

                                    ZoneServer Found = null;
                                    foreach (ZoneServer S in ZoneServer.ZoneServers)
                                    {
                                        if (S.Address.Equals(Address) && S.HostPort == Port)
                                        {
                                            Found = S;
                                            break;
                                        }
                                    }

                                    string SName = "Unknown";
                                    if (Found != null)
                                        SName = Found.MachineHostName;

                                    Log.WriteLine(string.Format("{0} Stats:\nGCLID: {1}\nServer: {2}\nZone: {3}:{4}", new object[] {
                                    Username,
                                    GCLID,
                                    SName,
                                    ZoneName,
                                    InstanceID }));


                                }
                                else if (Command == (byte)'N') // Invalid account
                                {
                                    M.MessageData.ReadByte();
                                    string Username = M.MessageData.ReadString(M.MessageData.ReadByte());

                                    Log.WriteLine("Account not found: " + Username);
                                }
                                else // Redirect
                                {

                                    foreach (AccountServer As in AccountServer.AccountServers)
                                    {
                                        RCEnet.Send(As.ServerConnection, MessageTypes.P_ClientInfo, M.MessageData.GetData(), true);
                                    }
                                }

                                break;
                            }
                    }

                    RCE_Message.Delete(M);
                }
                RCE_Message.Clean();
#if !DEBUG
            }
            catch (Exception E)
            {
                Log.WriteLine("Invalid packet(s) received: ");
                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    Log.WriteLine(M.ToString());
                    RCE_Message.Delete(M);
                }
                RCE_Message.Clean();
            }
#endif
        }
    }
}
