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

        public static void UpdateNetwork()
        {

            // Send off any queued messages
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

#if !DEBUG

            try
            {
#endif
                // Incoming messages
                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    switch (M.MessageType)
                    {
                        case MessageTypes.P_ActorInfoRequest:
                            {
                                byte MsgType = M.MessageData.ReadByte();

                                if (MsgType == (byte)'F') // Find request
                                {
                                    uint AllocID = M.MessageData.ReadUInt32();
                                    string ActorName = M.MessageData.ReadString(M.MessageData.ReadByte());

                                    // Create an internal request to send into the account server
                                    InternalActorInfoRequest Request = new InternalActorInfoRequest(ActorName, AllocID);

                                    // Call account database script
                                    ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "OnActorInfoRequest",
                                                new object[] { Request, new AccountActorInfoRequestHandler(Account.OnActorInfoRequestCallback) });
                                }

                                break;
                            }
                        case MessageTypes.P_ServerList:
                            {
                                // Verify its from the master
                                if (M.FromID != Server.MasterConnection)
                                {
                                    Log.WriteLine("Invalid P_ServerList packet received: Not from MasterConnection!");
                                    break;
                                }

                                // We'll only ever receive a shutdown message
                                if (M.MessageData != null && M.MessageData.ReadByte() == (byte)'S')
                                {
                                    // Yep, shutdown
                                    if (Server.MasterConnection != 0)
                                        RCEnet.Disconnect(Server.MasterConnection);

                                    // This will prevent invalid messages from being sent and also will prevent
                                    // a reconnect for 10 seconds (we'll have exited by then).
                                    Server.MasterConnection = 0;
                                    Server.MasterReconnectTime = Server.MilliSecs();

                                    // Bye
                                    Server.IsRunning = false;
                                    Log.WriteLine("Shutdown by MasterServer request.");
                                }
                                else
                                {
                                    Log.WriteLine("Invalid ServerList packet received. Ignoring.");
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
                                //if (M.MessageType == RCEnet.PlayerKicked)
                                //{
                                M.FromID = M.MessageData.ReadInt32();
                                //}
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

                                break;
                            }
                        case MessageTypes.P_ClientDrop:
                            {
                                byte CommandType = M.MessageData.ReadByte();

                                if (CommandType == (byte)'S') // Zone server wants the player saved
                                {
                                    uint GCLID = M.MessageData.ReadUInt32();

                                    int ActorLen = M.MessageData.ReadInt32();
                                    byte[] ActorBytes = M.MessageData.ReadBytes(ActorLen);
                                    PacketWriter ReAllocator = new PacketWriter();
                                    ActorInstance SaveAI = ActorInstance.CreateFromPacketReader(new PacketReader(ActorBytes), ReAllocator);

                                    int ScriptsLen = M.MessageData.ReadInt32();
                                    byte[] ScriptData = M.MessageData.ReadBytes(ScriptsLen);

                                    bool Found = false;
                                    foreach (Account A in Account.Accounts)
                                    {
                                        if (A.Username.Equals(SaveAI.accountName))
                                        {
                                            A.AccountBase.IsGM = SaveAI.accountDM;
                                            A.AccountBase.IsBanned = SaveAI.accountBanned;

                                            Log.WriteLine(A.Username + ": Saving");
                                            ActorInstanceData Data = new ActorInstanceData(ActorBytes, ScriptData);

                                            ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Save",
                                                new object[] { A.AccountBase, SaveAI.accountIndex, Data });

                                            // We need to send an account update through the stats line. This will propagate a 
                                            // clearing packet to other servers, to log this account as 'offline'.
                                            PacketWriter StatsActor = new PacketWriter();
                                            StatsActor.Write((byte)'A');
                                            StatsActor.Write((byte)A.Username.Length);
                                            StatsActor.Write(A.Username, false);
                                            StatsActor.Write(0);

                                            RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientInfo, StatsActor.ToArray(), true);


                                            Found = true;
                                            break;
                                        }
                                    }

                                    if (!Found)
                                        Log.WriteLine(SaveAI.accountName + ": Not saved, account not found");
                                }

                                break;
                            }
                        case MessageTypes.P_StartGame:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();

                                byte UsernameLen = M.MessageData.ReadByte();
                                string Username = M.MessageData.ReadString((int)(UsernameLen));

                                PacketWriter Pa78 = new PacketWriter();
                                Pa78.Write(GCLID);
                                Pa78.Write((byte)78);

                                // Find account
                                Account A = Account.Find(Username);
                                if (A == null)
                                {
                                    RCEnet.Send(M.FromID, MessageTypes.P_StartGame, Pa78.ToArray(), true);
                                }
                                else
                                {
                                    byte PassLen = M.MessageData.ReadByte();
                                    string Password = M.MessageData.ReadString(PassLen);

                                    if (A.Password.Equals(Password))
                                    {
                                        // Ingame check
                                        if (A.LoggedGCLID == 0)
                                        {
                                            int Number = (int)(M.MessageData.ReadSByte());

                                            if (Number > -1 && Number < 10)
                                            {
                                                // OK To start, change area!
                                                ActorInstance AI = A.Character[Number];

                                                if (AI != null)
                                                {
                                                    Area Ar = Area.Find(AI.Area);

                                                    if (Ar != null)
                                                    {
                                                        // ChangeArea Packet Construction
                                                        // This is a CA 'Request' as this server cannot ever host a zone.

                                                        // Copy in account data each time
                                                        AI.accountName = A.Username;
                                                        AI.accountIndex = Number;
                                                        AI.accountEmail = A.Email;
                                                        AI.accountDM = A.IsGM;
                                                        AI.accountBanned = A.IsBanned;
                                                        A.LoggedGCLID = GCLID;

                                                        // We need to send an account update through the stats line. This will propagate a 
                                                        // basic 'location' packet so that the master server and other account servers can
                                                        // track an in game client.
                                                        PacketWriter StatsActor = new PacketWriter();
                                                        StatsActor.Write((byte)'A');
                                                        StatsActor.Write((byte)A.Username.Length);
                                                        StatsActor.Write(A.Username, false);
                                                        StatsActor.Write(GCLID);

                                                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientInfo, StatsActor.ToArray(), true);

                                                        AI.GCLID = GCLID;
                                                        byte[] SerializedActor = AI.Serialize().ToArray();

                                                        PacketWriter CARequest = new PacketWriter();

                                                        // Main Header
                                                        CARequest.Write((byte)'S');
                                                        CARequest.Write(GCLID);
                                                        CARequest.Write((byte)Ar.Name.Length);
                                                        CARequest.Write(Ar.Name, false);
                                                        CARequest.Write((ushort)0); // Instance
                                                        CARequest.Write(-1); // Portal ID

                                                        // Actor
                                                        CARequest.Write(SerializedActor.Length);
                                                        CARequest.Write(SerializedActor, 0);

                                                        // Scripts
                                                        if (A.Scripts[Number] != null)
                                                        {
                                                            CARequest.Write(A.Scripts[Number].Length);
                                                            CARequest.Write(A.Scripts[Number].GetData(), 0);
                                                        }
                                                        else
                                                        {
                                                            CARequest.Write(0);
                                                        }

                                                        // Forms
                                                        CARequest.Write((ushort)0);


                                                        // ReDirect
                                                        Log.WriteLine(Username + ": Directed to zone '" + Ar.Name + "'");
                                                        RCEnet.Send(M.FromID, MessageTypes.P_ChangeAreaRequest, CARequest.ToArray(), true);

                                                        // Unload actor data
                                                        for (int i = 0; i < A.Character.Length; ++i)
                                                        {
                                                            if (A.Character[i] != null)
                                                            {
                                                                ActorInstance.Delete(A.Character[i]);
                                                                A.Scripts[i] = null;
                                                            }

                                                            A.Character[i] = null;
                                                        }
                                                        ActorInstance.Clean();

                                                        break;
                                                    }
                                                    else
                                                        Log.WriteLine("StartGame Denied: Area Handle is null");
                                                }
                                                else
                                                    Log.WriteLine("StartGame Denied: Actor Instance is null");
                                            }
                                            else
                                                Log.WriteLine("StartGame Denied: Number is incorrect");
                                        }
                                        else
                                            Log.WriteLine("StartGame Denied: Already in game");
                                    }
                                    else
                                        Log.WriteLine("StartGame Denied: Password is incorrect");

                                    RCEnet.Send(M.FromID, MessageTypes.P_StartGame, Pa78.ToArray(), true);

                                    break;
                                }

                                break;
                            }

                        case MessageTypes.P_FetchActors:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();

                                // Attributes block
                                PacketWriter Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("A", false);
                                Pa.Write((byte)(Attribute.AttributeAssignment));
                                for (int i = 0; i < 40; ++i)
                                {
                                    Pa.Write((byte)(Attribute.Attributes[i].IsSkill ? 1 : 0));
                                    Pa.Write((byte)(Attribute.Attributes[i].IsHidden ? 1 : 0));
                                    Pa.Write((byte)(Attribute.Attributes[i].Name.Length));
                                    Pa.Write(Attribute.Attributes[i].Name, false);
                                }
                                SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);

                                // Damage types
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("D", false);
                                for (int i = 0; i < 20; ++i)
                                {
                                    Pa.Write((byte)(DamageType.DamageTypes[i].Type.Length));
                                    Pa.Write(DamageType.DamageTypes[i].Type, false);
                                }
                                SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);

                                // Environment block
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("E", false);
                                Pa.Write(0);
                                Pa.Write((ushort)(WorldEnvironment.Day));
                                Pa.Write((byte)(WorldEnvironment.TimeH));
                                Pa.Write((byte)(WorldEnvironment.TimeM));
                                Pa.Write((byte)(WorldEnvironment.TimeFactor));

                                SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);

                                // Item blocks
                                int ItemsSent = 0;
                                int TotalItems = 0;
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("IN", false);

                               

                                foreach (Item It in Item.Items.Values)
                                {
                                    if (It == null)
                                        continue;

                                    ++TotalItems;
                                    ++ItemsSent;

                                    Pa.Write((ushort)(It.ID));
                                    Pa.Write((byte)(It.ItemType));
                                    Pa.Write((byte)(It.TakesDamage ? 1 : 0));
                                    Pa.Write(It.Value);
                                    Pa.Write((ushort)(It.Mass));
                                    Pa.Write((ushort)(It.ThumbnailTexID));

                                    Pa.Write((byte)It.GubbinTemplates.Length);
                                    for (int i = 0; i < It.GubbinTemplates.Length; ++i)
                                        Pa.Write((ushort)It.GubbinTemplates[i]);

                                    Pa.Write((ushort)(It.SlotType));
                                    Pa.Write((byte)(It.Stackable ? 1 : 0));

                                    for (int i = 0; i < 40; ++i)
                                        Pa.Write((ushort)(It.Attributes.Value[i] + 5000));


                                    Pa.Write((byte)(It.Name.Length));
                                    Pa.Write(It.Name, false);
                                    Pa.Write((byte)(It.ExclusiveRace.Length));
                                    Pa.Write(It.ExclusiveRace, false);
                                    Pa.Write((byte)(It.ExclusiveClass.Length));
                                    Pa.Write(It.ExclusiveClass, false);

                                    switch (It.ItemType)
                                    {
                                        case ItemTypes.I_Weapon:
                                            {
                                                Pa.Write((ushort)(It.WeaponDamage));
                                                Pa.Write((ushort)(It.WeaponDamageType));
                                                Pa.Write((ushort)(It.WeaponType));
                                                Pa.Write(It.Range);
                                                break;
                                            }
                                        case ItemTypes.I_Armour:
                                            {
                                                Pa.Write((ushort)(It.ArmourLevel));
                                                break;
                                            }
                                        case ItemTypes.I_Potion:
                                        case ItemTypes.I_Ingredient:
                                            {
                                                Pa.Write((ushort)(It.EatEffectsLength));
                                                break;
                                            }
                                        case ItemTypes.I_Image:
                                            {
                                                Pa.Write((ushort)(It.ImageID));
                                                break;
                                            }
                                        case ItemTypes.I_Other:
                                            {
                                                Pa.Write((byte)(It.MiscData.Length));
                                                Pa.Write(It.MiscData, false);
                                                break;
                                            }
                                    }

                                    if (ItemsSent >= 6 && TotalItems != Item.Items.Values.Count)
                                    {
                                        SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);
                                        ItemsSent = 0;
                                        Pa = new PacketWriter();
                                        Pa.Write(GCLID);
                                        Pa.Write("IN", false);
                                    }
                                }
                                PacketWriter PaI = Pa;
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("IY", false);
                                Pa.Write((ushort)(TotalItems));
                                Pa.Write(PaI.ToArray(), 6);

                                SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);

                                // Faction blocks
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("F", false);
                                for (int i = 0; i < 100; ++i)
                                {
                                    Pa.Write((byte)(Faction.Factions[i].Name.Length));
                                    Pa.Write((byte)(i));
                                    Pa.Write(Faction.Factions[i].Name, false);

                                    if (Pa.Length >= 800)
                                    {
                                        SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);
                                        Pa = new PacketWriter();
                                        Pa.Write(GCLID);
                                        Pa.Write("F", false);
                                    }
                                }
                                if (Pa.Length > 0)
                                    SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);

                                // Actor blocks
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("N", false);
                                int ActorsSent = 0;
                                int TotalActors = 0;

                                Actor LastActor = null;
                                for (uint i = (uint)(Actor.Actors.Count - 1); i >= 0; --i)
                                {
                                    if (Actor.Actors[i] != null)
                                    {
                                        LastActor = Actor.Actors[i];
                                        break;
                                    }
                                }

                                foreach (Actor Ac in Actor.Actors.Values)
                                {
                                    if (Ac == null)
                                        continue;

                                    ++TotalActors;
                                    ++ActorsSent;
                                    Pa.Write((ushort)(Ac.ID));
                                    Pa.Write((byte)(Ac.Playable ? 1 : 0));
                                    Pa.Write((byte)(Ac.PolyCollision ? 1 : 0));

                                    Pa.Write((ushort)Ac.MaleMesh);
                                    Pa.Write((ushort)Ac.FemaleMesh);

                                    Pa.Write((byte)Ac.DefaultGubbins.Length);
                                    for (int bid = 0; bid < Ac.DefaultGubbins.Length; ++bid)
                                    {
                                        Pa.Write((ushort)Ac.DefaultGubbins[bid]);
                                    }

                                    Pa.Write((byte)Ac.Beards.Count);
                                    for (int bid = 0; bid < Ac.Beards.Count; ++bid)
                                    {
                                        Pa.Write((byte)Ac.Beards[bid].Length);
                                        for (int i = 0; i < Ac.Beards[bid].Length; ++i)
                                            Pa.Write((ushort)Ac.Beards[bid][i]);
                                    }

                                    Pa.Write((byte)Ac.MaleHairs.Count);
                                    for (int bid = 0; bid < Ac.MaleHairs.Count; ++bid)
                                    {
                                        Pa.Write((byte)Ac.MaleHairs[bid].Length);
                                        for (int i = 0; i < Ac.MaleHairs[bid].Length; ++i)
                                            Pa.Write((ushort)Ac.MaleHairs[bid][i]);
                                    }

                                    Pa.Write((byte)Ac.FemaleHairs.Count);
                                    for (int bid = 0; bid < Ac.FemaleHairs.Count; ++bid)
                                    {
                                        Pa.Write((byte)Ac.FemaleHairs[bid].Length);
                                        for (int i = 0; i < Ac.FemaleHairs[bid].Length; ++i)
                                            Pa.Write((ushort)Ac.FemaleHairs[bid][i]);
                                    }

                                    Pa.Write((byte)Ac.MaleFaceIDs.Length);
                                    for (int i = 0; i < Ac.MaleFaceIDs.Length; ++i)
                                    {
                                        Pa.Write((ushort)(Ac.MaleFaceIDs[i].Tex0));
                                        Pa.Write((ushort)(Ac.MaleFaceIDs[i].Tex1));
                                        Pa.Write((ushort)(Ac.MaleFaceIDs[i].Tex2));
                                        Pa.Write((ushort)(Ac.MaleFaceIDs[i].Tex3));
                                    }

                                    Pa.Write((byte)Ac.FemaleFaceIDs.Length);
                                    for (int i = 0; i < Ac.FemaleFaceIDs.Length; ++i)
                                    {
                                        Pa.Write((ushort)(Ac.FemaleFaceIDs[i].Tex0));
                                        Pa.Write((ushort)(Ac.FemaleFaceIDs[i].Tex1));
                                        Pa.Write((ushort)(Ac.FemaleFaceIDs[i].Tex2));
                                        Pa.Write((ushort)(Ac.FemaleFaceIDs[i].Tex3));
                                    }

                                    Pa.Write((byte)Ac.MaleBodyIDs.Length);
                                    for (int i = 0; i < Ac.MaleBodyIDs.Length; ++i)
                                    {
                                        Pa.Write((ushort)(Ac.MaleBodyIDs[i].Tex0));
                                        Pa.Write((ushort)(Ac.MaleBodyIDs[i].Tex1));
                                        Pa.Write((ushort)(Ac.MaleBodyIDs[i].Tex2));
                                        Pa.Write((ushort)(Ac.MaleBodyIDs[i].Tex3));
                                    }

                                    Pa.Write((byte)Ac.FemaleBodyIDs.Length);
                                    for (int i = 0; i < Ac.FemaleBodyIDs.Length; ++i)
                                    {
                                        Pa.Write((ushort)(Ac.FemaleBodyIDs[i].Tex0));
                                        Pa.Write((ushort)(Ac.FemaleBodyIDs[i].Tex1));
                                        Pa.Write((ushort)(Ac.FemaleBodyIDs[i].Tex2));
                                        Pa.Write((ushort)(Ac.FemaleBodyIDs[i].Tex3));
                                    }

                                    for (int i = 0; i < 16; ++i)
                                        Pa.Write((ushort)(Ac.MSpeechIDs[i]));
                                    for (int i = 0; i < 16; ++i)
                                        Pa.Write((ushort)(Ac.FSpeechIDs[i]));

                                    Pa.Write((byte)(Ac.Rideable ? 1 : 0));
                                    Pa.Write((byte)(Ac.TradeMode));
                                    Pa.Write((ushort)(Ac.BloodTexID));
                                    Pa.Write((byte)(Ac.Aggressiveness));
                                    Pa.Write((byte)(Ac.Genders));
                                    Pa.Write((byte)(Ac.Environment));
                                    Pa.Write((ushort)(Ac.InventorySlots));
                                    Pa.Write((ushort)(Ac.MAnimationSet));
                                    Pa.Write((ushort)(Ac.FAnimationSet));
                                    Pa.Write(Ac.Scale);
                                    Pa.Write((byte)(Ac.DefaultFaction));

                                    for (int i = 0; i < 40; ++i)
                                    {
                                        Pa.Write((ushort)(Ac.Attributes.Value[i]));
                                        Pa.Write((ushort)(Ac.Attributes.Maximum[i]));
                                    }

                                    Pa.Write((byte)(Ac.Race.Length));
                                    Pa.Write(Ac.Race, false);
                                    Pa.Write((byte)(Ac.Class.Length));
                                    Pa.Write(Ac.Class, false);
                                    Pa.Write((ushort)(Ac.Description.Length));
                                    Pa.Write(Ac.Description, false);

                                    if (ActorsSent >= 2 && Ac != LastActor)
                                    {
                                        SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);
                                        ActorsSent = 0;
                                        Pa = new PacketWriter();
                                        Pa.Write(GCLID);
                                        Pa.Write("N", false);
                                    }
                                }
                                PaI = Pa;
                                Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write("Y", false);
                                Pa.Write((ushort)(TotalActors));
                                Pa.Write(PaI.ToArray(), 5);
                                SendQueued(M.FromID, MessageTypes.P_FetchActors, Pa.ToArray(), true);

                                break;
                            }
                        case MessageTypes.P_CreateAccount:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                Log.WriteLine("Whoop: " + Server.AllowAccountCreation.ToString());
                                if (Server.AllowAccountCreation)
                                {
                                    int UsernameLen = (int)(M.MessageData.ReadByte());
                                    string Username = M.MessageData.ReadString(UsernameLen);

                                    // Check that username does not already exist
                                    Account Ta = Account.Find(Username);
                                    if (Ta == null)
                                    {
                                        int PwdLen = (int)(M.MessageData.ReadByte());
                                        string Password = M.MessageData.ReadString(PwdLen);
                                        int EmailLen = (int)(M.MessageData.ReadByte());
                                        string Email = M.MessageData.ReadString(EmailLen);

                                        // Check that all characters are valid
                                        if (Account.VerifyStrings(Username, Password, Email))
                                        {
                                            // Create base
                                            AccountBase Base = new AccountBase(Username, Password, Email, false, false);

                                            // Add a wait instance (since account addition can be asyncronous)
                                            new Account.WaitInstance(Base, MessageTypes.P_CreateAccount, M.FromID, GCLID);

                                            // Call the userscript which will create this account
                                            ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Add",
                                                new object[] { Base, new AccountAddEventHandler(Account.AddAccountCallback) });
                                        }
                                        else
                                        {
                                            PacketWriter Pa78 = new PacketWriter();
                                            Pa78.Write(GCLID);
                                            Pa78.Write((byte)78);

                                            RCEnet.Send(M.FromID, MessageTypes.P_CreateAccount, Pa78.ToArray(), true);
                                        }

                                    }
                                    else
                                    {
                                        PacketWriter Pa78 = new PacketWriter();
                                        Pa78.Write(GCLID);
                                        Pa78.Write((byte)78);

                                        RCEnet.Send(M.FromID, MessageTypes.P_CreateAccount, Pa78.ToArray(), true);
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_VerifyAccount:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();

                                int UsernameLen = (int)(M.MessageData.ReadByte());
                                string Username = M.MessageData.ReadString(UsernameLen);

                                // Find account
                                Account A = Account.Find(Username);
                                if (A != null)
                                {
                                    // Account is logged in
                                    if (A.LoggedGCLID != 0)
                                    {
                                        PacketWriter Pa76 = new PacketWriter();
                                        Pa76.Write(GCLID);
                                        Pa76.Write((byte)76);

                                        RCEnet.Send(M.FromID, MessageTypes.P_VerifyAccount, Pa76.ToArray(), true);
                                    }
                                    else
                                    {
                                        int PwdLen = (int)(M.MessageData.ReadByte());
                                        string Password = M.MessageData.ReadString(PwdLen);

                                        // If password is correct, send back character list
                                        if (A.Password.Equals(Password) && A.IsBanned == false)
                                        {

                                            // Cannot send back character list yet, it must first be loaded
                                            // Add a WaitInstance because loading can be asynchronous
                                            new Account.WaitInstance(A.AccountBase, MessageTypes.P_VerifyAccount, M.FromID, GCLID);

                                            // Call userscript
                                            ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Load",
                                                new object[] { A.AccountBase, new AccountLoadEventHandler(Account.LoadAccountCallback) });
                                        }
                                        else // Otherwise return pass failure
                                        {
                                            PacketWriter Pa80 = new PacketWriter();
                                            Pa80.Write(GCLID);
                                            Pa80.Write((byte)80);
                                            PacketWriter Pa66 = new PacketWriter();
                                            Pa66.Write(GCLID);
                                            Pa66.Write((byte)66);

                                            if (!A.IsBanned)
                                                RCEnet.Send(M.FromID, MessageTypes.P_VerifyAccount, Pa80.ToArray(), true);
                                            else
                                                RCEnet.Send(M.FromID, MessageTypes.P_VerifyAccount, Pa66.ToArray(), true);
                                        }
                                    }
                                }
                                else
                                {
                                    PacketWriter Pa78 = new PacketWriter();
                                    Pa78.Write(GCLID);
                                    Pa78.Write((byte)78);

                                    RCEnet.Send(M.FromID, MessageTypes.P_VerifyAccount, Pa78.ToArray(), true);
                                }

                                break;
                            }
                        case MessageTypes.P_FetchCharacter:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();

                                int UsernameLen = (int)(M.MessageData.ReadByte());
                                string Username = M.MessageData.ReadString(UsernameLen);

                                // Find account
                                Account A = Account.Find(Username);
                                if (A != null)
                                {
                                    int PwdLen = (int)(M.MessageData.ReadByte());
                                    string Password = M.MessageData.ReadString(PwdLen);

                                    // If password is correct
                                    if (A.Password.Equals(Password) && A.IsBanned == false)
                                    {
                                        int Number = (int)(M.MessageData.ReadByte());
                                        if (Number > -1 && Number < 10 && A.Character[Number] != null)
                                        {
                                            ActorInstance AI = A.Character[Number];

                                            // Send character data
                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(GCLID);
                                            Pa.Write("C1", false);
                                            Pa.Write(AI.Gold);
                                            Pa.Write((ushort)(AI.Reputation));
                                            Pa.Write((ushort)(AI.Level));
                                            Pa.Write(AI.XP);
                                            Pa.Write((byte)(AI.homeFaction));
                                            for (int i = 0; i < 40; ++i)
                                            {
                                                Pa.Write((ushort)(AI.Attributes.Value[i]));
                                                Pa.Write((ushort)(AI.Attributes.Maximum[i]));
                                            }
                                            SendQueued(M.FromID, MessageTypes.P_FetchCharacter, Pa.ToArray(), true);
                                            Pa = new PacketWriter();
                                            Pa.Write(GCLID);
                                            Pa.Write("C3", false);

                                            for (int i = 0; i < 50; ++i)
                                            {
                                                if (AI.Inventory.Items[i] != null)
                                                {
                                                    Pa.Write((byte)(i));
                                                    Pa.Write(AI.Inventory.Items[i].ToArray());
                                                    Pa.Write((ushort)(AI.Inventory.Amounts[i]));
                                                }
                                                else
                                                {
                                                    Pa.Write((byte)(99));
                                                }
                                                if (Pa.Length > 999 - ItemInstance.ItemInstanceStringLength())
                                                {
                                                    SendQueued(M.FromID, MessageTypes.P_FetchCharacter, Pa.ToArray(), true);
                                                    Pa = new PacketWriter();
                                                    Pa.Write(GCLID);
                                                    Pa.Write("C3", false);
                                                }
                                            }
                                            if (Pa.Length > 0)
                                                SendQueued(M.FromID, MessageTypes.P_FetchCharacter, Pa.ToArray(), true);

                                            // Send known spells
                                            Pa = new PacketWriter();
                                            PacketWriter OldPa = new PacketWriter();
                                            int SpellsDone = 0;
                                            for (int i = 0; i < 1000; ++i)
                                            {
                                                if (AI.SpellLevels[i] > 0)
                                                {
                                                    OldPa.Write(Pa.ToArray());
                                                    Pa = new PacketWriter();
                                                    Spell Sp = Spell.Spells[AI.KnownSpells[i]];
                                                    if (Sp != null)
                                                    {
                                                        Pa.Write((ushort)(AI.SpellLevels[i]));
                                                        Pa.Write((ushort)(Sp.ID));
                                                        Pa.Write((ushort)(Sp.ThumbnailTexID));
                                                        Pa.Write((ushort)(Sp.RechargeTime));
                                                        Pa.Write((ushort)(Sp.Name.Length));
                                                        Pa.Write(Sp.Name, false);
                                                        Pa.Write((ushort)(Sp.Description.Length));
                                                        Pa.Write(Sp.Description, false);

                                                        bool Memorised = false;
                                                        for (int j = 0; j < 10; ++j)
                                                        {
                                                            if (AI.MemorisedSpells[j] == i)
                                                            {
                                                                Memorised = true;
                                                                break;
                                                            }
                                                        }

                                                        Pa.Write((byte)(Memorised ? 1 : 0));
                                                        ++SpellsDone;
                                                    }
                                                    else
                                                    {
                                                        AI.SpellLevels[i] = 0;
                                                        AI.KnownSpells[i] = 0;
                                                    }

                                                    if (OldPa.Length + Pa.Length > 1000)
                                                    {
                                                        PacketWriter TPa = new PacketWriter();
                                                        TPa.Write(GCLID);
                                                        TPa.Write("S", false);
                                                        TPa.Write(OldPa.ToArray());
                                                        SendQueued(M.FromID, MessageTypes.P_FetchCharacter, TPa.ToArray(), true);
                                                        OldPa = new PacketWriter();
                                                    }
                                                }
                                            }
                                            OldPa.Write(Pa.ToArray());
                                            if (OldPa.Length > 0)
                                            {
                                                PacketWriter TPa = new PacketWriter();
                                                TPa.Write(GCLID);
                                                TPa.Write("S", false);
                                                TPa.Write(OldPa.ToArray());
                                                SendQueued(M.FromID, MessageTypes.P_FetchCharacter, TPa.ToArray(), true);
                                            }

                                            // Send quest log
                                            Pa = new PacketWriter();
                                            Pa.Write(GCLID);
                                            Pa.Write("Q", false);
                                            int Num = 0;

                                            for (int i = 0; i < 500; ++i)
                                            {
                                                if (A.Character[Number].QuestLog.EntryName[i].Length > 0)
                                                {
                                                    ++Num;
                                                    Pa.Write((byte)(A.Character[Number].QuestLog.EntryName[i].Length));
                                                    Pa.Write(A.Character[Number].QuestLog.EntryName[i], false);
                                                    Pa.Write((ushort)(A.Character[Number].QuestLog.EntryStatus[i].Length));
                                                    Pa.Write(A.Character[Number].QuestLog.EntryStatus[i]);

                                                    if (Pa.Length > 700)
                                                    {
                                                        SendQueued(M.FromID, MessageTypes.P_FetchCharacter, Pa.ToArray(), true);
                                                        Pa = new PacketWriter();
                                                        Pa.Write(GCLID);
                                                        Pa.Write("Q", false);
                                                    }
                                                }
                                            }

                                            if (Pa.Length > 0)
                                                SendQueued(M.FromID, MessageTypes.P_FetchCharacter, Pa.ToArray(), true);

                                            Pa = new PacketWriter();
                                            Pa.Write(GCLID);
                                            Pa.Write("F", false);
                                            Pa.Write((ushort)(Num));
                                            Pa.Write((ushort)(SpellsDone));
                                            SendQueued(M.FromID, MessageTypes.P_FetchCharacter, Pa.ToArray(), true);
                                        }
                                        else
                                        {
                                            PacketWriter Pa78 = new PacketWriter();
                                            Pa78.Write(GCLID);
                                            Pa78.Write((byte)78);

                                            RCEnet.Send(M.FromID, MessageTypes.P_FetchCharacter, Pa78.ToArray(), true);
                                        }
                                    }
                                    else // Otherwise return pass failure
                                    {
                                        PacketWriter Pa78 = new PacketWriter();
                                        Pa78.Write(GCLID);
                                        Pa78.Write((byte)78);

                                        RCEnet.Send(M.FromID, MessageTypes.P_FetchCharacter, Pa78.ToArray(), true);
                                    }
                                }
                                else
                                {
                                    PacketWriter Pa78 = new PacketWriter();
                                    Pa78.Write(GCLID);
                                    Pa78.Write((byte)78);

                                    RCEnet.Send(M.FromID, MessageTypes.P_FetchCharacter, Pa78.ToArray(), true);
                                }
                                break;
                            }
                        case MessageTypes.P_CreateCharacter:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();


                                int UsernameLen = (int)(M.MessageData.ReadByte());
                                string Username = M.MessageData.ReadString(UsernameLen);

                                // Find account
                                Account A = Account.Find(Username);
                                if (A != null)
                                {
                                    int PwdLen = (int)(M.MessageData.ReadByte());
                                    string Password = M.MessageData.ReadString(PwdLen);

                                    // If password is correct
                                    if (A.Password.Equals(Password) && A.IsBanned == false)
                                    {
                                        int Location = M.MessageData.Location;
                                        M.MessageData.Location = Location + 47;

                                        // Check character name is valid
                                        string Name = M.MessageData.ReadString(M.MessageData.Length - (Location + 47));
                                        bool NameValid = Server.IsNameValid(Name);

                                        // Check character name is not already in use
                                        if (Account.ActorNameTaken(Name))
                                            NameValid = false;

                                        M.MessageData.Location = Location;

                                        // Name was valid
                                        if (NameValid)
                                        {
                                            // Find free slot
                                            int FreeSlot = -1;
                                            int TotalChars = 0;
                                            for (int i = 0; i < 10; ++i)
                                            {
                                                if (A.Character[i] == null)
                                                {
                                                    if (FreeSlot == -1)
                                                        FreeSlot = i;
                                                }
                                                else
                                                {
                                                    ++TotalChars;
                                                }
                                            }

                                            // If we have a free slot and haven't exceeded the maximum allowed characters
                                            if (FreeSlot > -1 && TotalChars < Server.MaxAccountChars)
                                            {
                                                int ActorID = (int)(M.MessageData.ReadUInt16());
                                                A.Character[FreeSlot] = ActorInstance.CreateActorInstance(Actor.Actors[(uint)ActorID]);
                                                ActorInstance C = A.Character[FreeSlot];
                                                C.Account = A;
                                                C.accountName = A.Username;
                                                C.accountIndex = FreeSlot;
                                                C.accountEmail = A.Email;
                                                C.accountDM = A.IsGM;
                                                C.accountBanned = A.IsBanned;
                                                C.gender = M.MessageData.ReadByte();
                                                C.FaceTex = M.MessageData.ReadByte();
                                                C.hair = M.MessageData.ReadByte();
                                                C.beard = M.MessageData.ReadByte();
                                                C.BodyTex = M.MessageData.ReadByte();
                                                C.Area = C.Actor.StartArea;
                                                C.Gold = Server.StartGold;
                                                C.reputation = Server.StartReputation;

                                                Area Ar = Area.Find(C.Area);
                                                
                                                if (Ar == null)                                                                                         
                                                {
                                                    //Todo: Replace with a server error to client message.
                                                    PacketWriter Pa73 = new PacketWriter();
                                                    Pa73.Write(GCLID);
                                                    Pa73.Write((byte)73);
                                                    RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, Pa73.ToArray(), true);

                                                    Log.WriteLine("Error: Could not find starting zone for actor ID " +  C.Actor.ID + " (" + C.Area + ")");
                                                    break;
                                                }


                                                for (int i = 0; i < Ar.Portals.Length; ++i)
                                                {
                                                    if (Ar.Portals[i].Name.Equals(C.Actor.StartPortal, StringComparison.CurrentCultureIgnoreCase))
                                                    {
                                                        C.LastPortal = i;
                                                        C.Position = Ar.Portals[i].Position;
                                                        break;
                                                    }
                                                }

                                                if (Attribute.AttributeAssignment > 0)
                                                {

                                                    int TotalAmount = 0;

                                                    for (int i = 0; i < 40; ++i)
                                                    {
                                                        int Amount = M.MessageData.ReadByte();
                                                        TotalAmount += Amount;
                                                        C.Attributes.Value[i] = C.Attributes.Value[i] + Amount;
                                                    }

                                                    // Check for cheating

                                                    if (TotalAmount > Attribute.AttributeAssignment)
                                                    {
                                                        C.FreeActorInstance();

                                                        PacketWriter Pa78 = new PacketWriter();
                                                        Pa78.Write(GCLID);
                                                        Pa78.Write((byte)78);

                                                        RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, Pa78.ToArray(), true);
                                                        break;
                                                    }

                                                }

                                                C.Name = Name;

                                                // Have a temporary save
                                                ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Save",
                                                    new object[] { A.AccountBase, FreeSlot, new ActorInstanceData(C.Serialize().ToArray(), new byte[0]) });

                                                PacketWriter Pa89 = new PacketWriter();
                                                Pa89.Write(GCLID);
                                                Pa89.Write((byte)89);

                                                RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, Pa89.ToArray(), true);
                                            }
                                            else // No free slots, fail
                                            {
                                                PacketWriter Pa78 = new PacketWriter();
                                                Pa78.Write(GCLID);
                                                Pa78.Write((byte)78);

                                                RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, Pa78.ToArray(), true);
                                            }
                                        }
                                        else // Character name invalid
                                        {
                                            PacketWriter Pa73 = new PacketWriter();
                                            Pa73.Write(GCLID);
                                            Pa73.Write((byte)73);

                                            RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, Pa73.ToArray(), true);
                                        }
                                    }
                                    else // Wrong password
                                    {
                                        PacketWriter Pa78 = new PacketWriter();
                                        Pa78.Write(GCLID);
                                        Pa78.Write((byte)78);

                                        RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, Pa78.ToArray(), true);
                                    }
                                }
                                else // Wrong account name
                                {
                                    PacketWriter Pa78 = new PacketWriter();
                                    Pa78.Write(GCLID);
                                    Pa78.Write((byte)78);

                                    RCEnet.Send(M.FromID, MessageTypes.P_CreateCharacter, new byte[] { 78 }, true);
                                }

                                break;
                            }
                        case MessageTypes.P_DeleteCharacter:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();

                                int UsernameLen = (int)(M.MessageData.ReadByte());
                                string Username = M.MessageData.ReadString(UsernameLen);

                                // Find account
                                Account A = Account.Find(Username);
                                if (A != null)
                                {
                                    int PwdLen = (int)(M.MessageData.ReadByte());
                                    string Password = M.MessageData.ReadString(PwdLen);

                                    // If password is correct
                                    if (A.Password.Equals(Password) && A.IsBanned == false && A.LoggedGCLID == 0)
                                    {
                                        int Number = (int)(M.MessageData.ReadSByte());
                                        if (Number > -1 && Number < 10)
                                        {
                                            // Delete the character
                                            if (A.Character[Number].QuestLog != null)
                                                A.Character[Number].QuestLog = null;
                                            ActorInstance.Delete(A.Character[Number]);
                                            for (int i = Number; i < 9; ++i)
                                            {
                                                A.Character[i] = A.Character[i + 1];
                                            }
                                            A.Character[9] = null;

                                            // Save missing characters
                                            ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "Save",
                                                new object[] { A.AccountBase, Number, null });


                                            // Send back new character list
                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(GCLID);

                                            for (int i = 0; i < 10; ++i)
                                            {
                                                if (A.Character[i] != null)
                                                {
                                                    Pa.Write((byte)(A.Character[i].Name.Length));
                                                    Pa.Write(A.Character[i].Name, false);
                                                    Pa.Write((ushort)(A.Character[i].Actor.ID));
                                                    Pa.Write((byte)(A.Character[i].gender));
                                                    Pa.Write((byte)(A.Character[i].FaceTex));
                                                    Pa.Write((byte)(A.Character[i].Hair));
                                                    Pa.Write((byte)(A.Character[i].Beard));
                                                    Pa.Write((byte)(A.Character[i].BodyTex));
                                                }
                                            }

                                            RCEnet.Send(M.FromID, MessageTypes.P_DeleteCharacter, Pa.ToArray(), true);
                                        }
                                        else
                                        {
                                            PacketWriter Pa78 = new PacketWriter();
                                            Pa78.Write(GCLID);
                                            Pa78.Write((byte)78);

                                            RCEnet.Send(M.FromID, MessageTypes.P_DeleteCharacter, Pa78.ToArray(), true);
                                        }
                                    }
                                    else
                                    {
                                        PacketWriter Pa78 = new PacketWriter();
                                        Pa78.Write(GCLID);
                                        Pa78.Write((byte)78);

                                        RCEnet.Send(M.FromID, MessageTypes.P_DeleteCharacter, Pa78.ToArray(), true);
                                    }
                                }
                                else
                                {
                                    PacketWriter Pa78 = new PacketWriter();
                                    Pa78.Write(GCLID);
                                    Pa78.Write((byte)78);

                                    RCEnet.Send(M.FromID, MessageTypes.P_DeleteCharacter, Pa78.ToArray(), true);
                                }

                                break;
                            }
                        case MessageTypes.P_ClientInfo:
                            {
                                byte Command = M.MessageData.ReadByte();

                                // An update from an account server
                                if (Command == (byte)'A')
                                {
                                    string Username = M.MessageData.ReadString(M.MessageData.ReadByte());

                                    Account A = Account.Find(Username);
                                    if (A != null)
                                    {
                                        uint GCLID = M.MessageData.PeekUInt32();
                                        A.ActorStat = M.MessageData.ReadBytes(4);
                                        A.LoggedGCLID = GCLID;
                                    }
                                    else
                                    {
                                        Log.WriteLine("Stats couldn't update account: " + Username);
                                    }
                                }
                                else if (Command == (byte)'Z') // From Zone Server
                                {
                                    uint GCLID = M.MessageData.ReadUInt32();

                                    Account A = Account.FindFromGCLID(GCLID);
                                    if (A != null)
                                    {
                                        A.ZoneStat = M.MessageData.ReadBytes(M.MessageData.Length - M.MessageData.Location);
                                    }
                                }
                                else if (Command == (byte)'M') // From Master Server
                                {
                                    // Username to find
                                    string Username = M.MessageData.ReadString(M.MessageData.ReadByte());

                                    Account A = Account.Find(Username);
                                    if (A != null)
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'O');
                                        Pa.Write((byte)A.Username.Length);
                                        Pa.Write(A.Username, false);
                                        Pa.Write(A.LoggedGCLID);
                                        Pa.Write(A.ZoneStat, 0);

                                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientInfo, Pa.ToArray(), true);
                                    }
                                    else
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'N');
                                        Pa.Write((byte)Username.Length);
                                        Pa.Write(Username, false);

                                        RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientInfo, Pa.ToArray(), true);
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
                Log.WriteLine("Networking Exception: ");
                Log.WriteLine(E.Message);
                Log.WriteLine(E.StackTrace);
                Log.WriteLine("");

                foreach (RCE_Message M in RCE_Message.RCE_MessageList)
                {
                    Log.WriteLine("Packet dropped: " + M.MessageType.ToString() + " " + M.Connection.ToString() + " " + M.FromID.ToString());
                    RCE_Message.Delete(M);
                }
                RCE_Message.Clean();

            }
#endif

        }
    }
}
