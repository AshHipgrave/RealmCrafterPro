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
            foreach(QueuedPacket Q2 in QueuedPacket.QueuedList)
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
                        Pa.Write(ai.GCLID);
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
                        case MessageTypes.P_GUIEvent:
                            {
                                if (M.MessageData != null)
                                {
                                    // Find Player
                                    uint GCLID = M.MessageData.ReadUInt32();
                                    ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                    if (AI != null)
                                    {

                                        // JB: Removed, don't allow stacking of network messages
                                        //while(M.MessageData.Location < M.MessageData.Length - 1)
                                        ScriptManager.ExecuteSpecialScriptObject(typeof(Scripting.Forms.Control), "ProcessEvent", new object[] { AI.ClientControls, M.MessageData });
                                        //                                     try
                                        //                                     {
                                        //                                         Scripting.Forms.Control.ProcessEvent(AI.ClientControls, M.MessageData);
                                        //                                     }
                                        //                                     catch (System.Exception ex)
                                        //                                     {
                                        //                                         ScriptManager.HandleException(ex);
                                        //                                     }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_Instance:
                            {
                                byte Command = M.MessageData.ReadByte();

                                if (Command == (byte)'R')
                                {
                                    string Name = M.MessageData.ReadString(M.MessageData.ReadByte());
                                    ushort RequestedID = M.MessageData.ReadUInt16();
                                    ushort ActualID = M.MessageData.ReadUInt16();

                                    Area Ar = Area.Find(Name);
                                    if (Ar == null)
                                        Log.WriteLine("Couldn't complete InstanceRequest as area was not found: '" + Name + "'");

                                    Ar.CompleteInstanceRequest(RequestedID, ActualID);
                                }


                                break;
                            }
                        case MessageTypes.P_ChangeAreaRequest:
                            {
                                byte StartType = M.MessageData.ReadByte();
                                uint GCLID = M.MessageData.ReadUInt32();

                                string AreaName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                ushort InstanceID = M.MessageData.ReadUInt16();
                                int PortalID = M.MessageData.ReadInt32();

                                int SerializedActorLength = M.MessageData.ReadInt32();

                                PacketWriter ReAllocator = new PacketWriter();
                                ReAllocator.Write(GCLID);

                                ActorInstance AI = ActorInstance.CreateFromPacketReader(M.MessageData, ReAllocator);
                                AI.AssignRuntimeID();
                                AI.RNID = M.FromID;
                                Area Ar = Area.Find(AreaName);

                                int ScriptsLen = M.MessageData.ReadInt32();
                                PacketReader ScriptData = new PacketReader(M.MessageData.ReadBytes(ScriptsLen));

                                ScriptBase.LoadPersistentInstance(ref ScriptData, AI);

                                Log.WriteLine(AI.name + ": Now on this server");

                                //TODO: Remove this debug message
                                //                             PacketWriter DPa = new PacketWriter();
                                //                             DPa.Write(AI.GCLID);
                                //                             DPa.Write((byte)0);
                                //                             DPa.Write((byte)(253));
                                //                             DPa.Write("You are connected to: " + Server.HostName, false);
                                //                             RCEnet.Send(M.FromID, MessageTypes.P_ChatMessage, DPa.ToArray(), true);

                                AI.SetArea(
                                    Ar, InstanceID, -1, PortalID,
                                    AI.Position);

                                RCEnet.Send(M.FromID, MessageTypes.P_ReAllocateIDs, ReAllocator.ToArray(), true);

                                if (StartType == (byte)'S')
                                {
                                    // Remove any serialized dialogs
                                    LinkedListNode<RCPServer.ProgressBar> PrNode = AI.ProgressBars.First;
                                    while (PrNode != null)
                                    {
                                        ProgressBar Pr = PrNode.Value;
                                        PrNode = PrNode.Next;

                                        Pr.Remove();
                                    }

                                    // Run login script
                                    ScriptManager.DelayedExecute("Login", "OnLogin", AI, AI, null);

                                    // Send actionbar data, runtimeID and XP bar level
                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write(GCLID);
                                    Pa.Write((byte)(0));
                                    for (int i = 0; i < 36; ++i)
                                    {
                                        Pa.Write((ushort)(AI.ActionBar.Slots[i].Length));
                                        Pa.Write(AI.ActionBar.Slots[i], false);

                                        if ((i + 1) % 3 == 0 && i > 0)
                                        {
                                            Network.SendQueued(M.FromID, MessageTypes.P_StartGame, Pa.ToArray(), true);
                                            Pa = new PacketWriter();
                                            Pa.Write(GCLID);
                                            Pa.Write((byte)(i + 1));
                                        }
                                    }
                                    Pa = new PacketWriter();
                                    Pa.Write(GCLID);
                                    Pa.Write((ushort)AI.RuntimeID);
                                    RCEnet.Send(M.FromID, MessageTypes.P_StartGame, Pa.ToArray(), true);
                                    //Pa = new PacketWriter();
                                    //Pa.Write(GCLID);
                                    //Pa.Write((byte)0);
                                    //Pa.Write((byte)(254));
                                    //Pa.Write(Server.LoginMessage, false);
                                    //RCEnet.Send(M.FromID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                    Pa = new PacketWriter();
                                    Pa.Write(GCLID);
                                    Pa.Write("B", false);
                                    Pa.Write((byte)(AI.XPBarLevel));
                                    RCEnet.Send(M.FromID, MessageTypes.P_XPUpdate, Pa.ToArray(), true);

                                    // Send active actor effects
                                    foreach (ActorEffect AE in AI.ActorEffects)
                                    {
                                        AE.CreatedTime = Server.MilliSecs();
                                        Pa = new PacketWriter();
                                        Pa.Write(GCLID);
                                        Pa.Write("A", false);
                                        Pa.Write(AE.AllocID);
                                        Pa.Write(AE.IconTexID);
                                        Pa.Write(AE.Name, false);
                                        RCEnet.Send(M.FromID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                                    }

                                    // Shaders
                                    foreach (KeyValuePair<string, Scripting.Math.ShaderParameter> Kvp in AI.ShaderParameters)
                                    {
                                        Pa = new PacketWriter();
                                        Pa.Write(GCLID);
                                        Pa.Write((ushort)AI.RuntimeID);
                                        Pa.Write((byte)Kvp.Value.GetParameterType());
                                        Scripting.Math.Vector4 v4 = Kvp.Value.GetParameterVector4();
                                        Pa.Write(v4.X);
                                        Pa.Write(v4.Y);
                                        Pa.Write(v4.Z);
                                        Pa.Write(v4.W);
                                        Pa.Write((byte)Kvp.Key.Length);
                                        Pa.Write(Kvp.Key, false);

                                        RCEnet.Send(AI.RNID, MessageTypes.P_ShaderConstant, Pa.ToArray(), true);
                                    }
                                }
                                else
                                {
                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write((byte)'R');
                                    Pa.Write(AI.RuntimeID);
                                    Pa.Write(AI.RuntimeID);

                                    if (M.MessageData.Location < M.MessageData.Length - 1)
                                    {
                                        // Forms
                                        ushort FormCount = M.MessageData.ReadUInt16();
                                        for (int i = 0; i < FormCount; ++i)
                                        {
                                            string FormName = M.MessageData.ReadString();
                                            int ReadLength = M.MessageData.ReadInt32();
                                            PacketReader SerializedForm = new PacketReader(M.MessageData.ReadBytes(ReadLength));

                                            object FormInst = Scripting.ScriptManager.Instantiate(FormName);

                                            if (FormInst != null)
                                            {
                                                Scripting.Forms.Control C = FormInst as Scripting.Forms.Control;
                                                C.Deserialize(SerializedForm, C, AI, AI.ClientControls);

                                                PacketWriter FormPa = C.ControlPropertyPacket();

                                                AI.ScriptedForms.AddLast(C);
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_ChatMessage:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (M.MessageData.Length > 0 && AI != null)
                                {
                                    // A command?
                                    if (M.MessageData.PeekByte() == '/' || M.MessageData.PeekByte() == '\\')
                                    {
                                        M.MessageData.ReadByte();
                                        string Command = M.MessageData.ReadString(M.MessageData.Length - 5);
                                        string Params = "";
                                        int SpacePos = Command.IndexOf(' ');

                                        if (SpacePos > -1)
                                        {
                                            Params = Command.Substring(SpacePos + 1).Trim();
                                            Command = Command.Substring(0, SpacePos).ToUpper();
                                        }
                                        else
                                        {
                                            Command = Command.ToUpper();
                                        }

                                        #region SlashCommandProcessing
                                        /*if (Command.Equals(Language.Get(LanguageString.SKick), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            ActorInstance A2 = ActorInstance.FindFromName(Params);
                                            if (A2 != null && A2.RNID > 0)
                                            {
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write(0);
                                                Pa.Write(A2.RNID);
                                                RCEnet.Send(0, RCEnet.PlayerKicked, Pa.ToArray(), true);

                                                Pa = new PacketWriter();
                                                Pa.Write(A2.GCLID);
                                                RCEnet.Send(A2.RNID, MessageTypes.P_KickedPlayer, Pa.ToArray(), true);
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SUnIgnore), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        ActorInstance A2 = ActorInstance.FindFromName(Params);
                                        if (A2 != null && A2 != AI && A2.RNID >= 0)
                                        {
                                            //TODO: FIX!
                                            //if (AI.Account.Ignoring(A2.Account))
                                            {
                                                //AI.Account.Ignore.Remove(A2.Account.User);
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write(AI.GCLID);
                                                Pa.Write((byte)0);
                                                Pa.Write((byte)(253));
                                                //Pa.Write(Language.Get(LanguageString.UnIgnoring) + " " + Params, false);
                                                Pa.Write("Ignoring is not reimplmented!", false);
                                                RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SIgnore), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        ActorInstance A2 = ActorInstance.FindFromName(Params);
                                        if (A2 != null && A2 != AI && A2.RNID >= 0)
                                        {
                                            //TODO: FIX!
                                            //if (!AI.Account.Ignoring(A2.Account))
                                            {
                                                //AI.Account.Ignore.Add(A2.Account.User);
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write(AI.GCLID);
                                                Pa.Write((byte)0);
                                                Pa.Write((byte)(253));
                                                //Pa.Write(Language.Get(LanguageString.Ignoring) + " " + Params, false);
                                                Pa.Write("Ignoring is not reimplemented!", false);
                                                RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SPet), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.NumberOfSlaves > 0)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);
                                            if (Datas.Length == 3)
                                            {
                                                int Found = 0;
                                                foreach (ActorInstance AI2 in ActorInstance.ActorInstanceList)
                                                {
                                                    if (AI2.Leader == AI)
                                                    {
                                                        ++Found;
                                                        if (Datas[0].Equals("all", StringComparison.CurrentCultureIgnoreCase) || Datas[0].Equals(AI2.Name, StringComparison.CurrentCultureIgnoreCase))
                                                        {
                                                            AI2.CommandPet(Command, Datas[3]);
                                                            if (!Datas[0].Equals("all", StringComparison.CurrentCultureIgnoreCase))
                                                                break;
                                                        }
                                                        if (Found == AI.NumberOfSlaves)
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SLeave), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        AI.LeaveParty();
                                    } else if (Command.Equals(Language.Get(LanguageString.SOk), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        Party Party = AI.AcceptPending;
                                        if (Party != null)
                                        {
                                            // Check there is free space
                                            if (Party.Members < 8)
                                            {
                                                // Remove from old party
                                                AI.LeaveParty();

                                                // Add to new party and notify players
                                                for (int i = 0; i < 8; ++i)
                                                {
                                                    if (Party.Player[i] != null)
                                                    {
                                                        PacketWriter Pa = new PacketWriter();
                                                        Pa.Write(Party.Player[i].GCLID);
                                                        Pa.Write((byte)0);
                                                        Pa.Write((byte)(254));
                                                        Pa.Write(AI.Name + " " + Language.Get(LanguageString.XHasJoinedParty), false);
                                                        RCEnet.Send(Party.Player[i].RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                                    }
                                                    else if (AI.AcceptPending != null)
                                                    {
                                                        PacketWriter Pa = new PacketWriter();
                                                        Pa.Write(AI.GCLID);
                                                        Pa.Write((byte)0);
                                                        Pa.Write((byte)(254));
                                                        Pa.Write(Language.Get(LanguageString.YouHaveJoinedParty), false);
                                                        RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                                        AI.PartyID = Party;
                                                        AI.AcceptPending = null;
                                                        Party.Player[i] = AI;
                                                        ++Party.Members;
                                                    }
                                                }

                                                for (int i = 0; i < 8; ++i)
                                                    if (Party.Player[i] != null)
                                                        SendPartyUpdate(Party.Player[i]);

                                                // Run script
                                                ScriptManager.DelayedExecute("Party", "OnJoin", AI, AI, null);
                                            }
                                            else // Party is full
                                            {
                                                PacketWriter Pa = new PacketWriter();
                                                Pa.Write(AI.GCLID);
                                                Pa.Write((byte)0);
                                                Pa.Write((byte)(254));
                                                Pa.Write(Language.Get(LanguageString.CouldNotJoinParty), false);
                                                RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                                AI.AcceptPending = null;
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SInvite), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        ActorInstance A2 = ActorInstance.FindFromName(Params);
                                        if (A2 != null && A2 != AI)
                                        {
                                            if (A2.RNID > 0)
                                            {
                                                Party Party = AI.PartyID;

                                                // Create new party if required
                                                if (Party == null)
                                                {
                                                    Party = new Party();
                                                    AI.PartyID = Party;
                                                    Party.Members = 1;
                                                    Party.Player[0] = AI;
                                                    ScriptManager.DelayedExecute("Party", "OnJoin", AI, AI, null);
                                                }

                                                // Check there's a free space in the party
                                                if (Party.Members < 8)
                                                {
                                                    A2.AcceptPending = Party;
                                                    PacketWriter Pa1 = new PacketWriter();
                                                    PacketWriter Pa2 = new PacketWriter();
                                                    Pa1.Write(A2.GCLID);
                                                    Pa2.Write(A2.GCLID);
                                                    Pa1.Write((byte)0);
                                                    Pa2.Write((byte)0);
                                                    Pa1.Write((byte)(254));
                                                    Pa2.Write((byte)(254));
                                                    Pa1.Write(Language.Get(LanguageString.PartyInvite) + " " + AI.Name, false);
                                                    Pa2.Write(Language.Get(LanguageString.PartyInviteInstruction), false);
                                                    RCEnet.Send(A2.RNID, MessageTypes.P_ChatMessage, Pa1.ToArray(), true);
                                                    RCEnet.Send(A2.RNID, MessageTypes.P_ChatMessage, Pa2.ToArray(), true);
                                                }
                                                else // Party is full
                                                {
                                                    PacketWriter Pa = new PacketWriter();
                                                    Pa.Write(AI.GCLID);
                                                    Pa.Write((byte)0);
                                                    Pa.Write((byte)(254));
                                                    Pa.Write(Language.Get(LanguageString.CouldNotInviteParty), false);
                                                    RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SXP), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                            AI.GiveXP(Convert.ToInt32(Params), true);
                                    } else if (Command.Equals(Language.Get(LanguageString.SGold), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            int Change = Convert.ToInt32(Params);
                                            AI.Gold += Change;

                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(AI.GCLID);

                                            if (Change > 0)
                                            {
                                                Pa.Write("U", false);
                                                Pa.Write(Change);
                                            }
                                            else
                                            {
                                                Pa.Write("D", false);
                                                Pa.Write(Math.Abs(Change));
                                            }

                                            RCEnet.Send(AI.RNID, MessageTypes.P_GoldChange, Pa.ToArray(), true);
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SSetAttribute), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);

                                            int Attr = Attribute.FindAttribute(Datas[0]);

                                            if (Attr > -1)
                                            {
                                                if (Attr == Server.HealthStat || Attr == Server.SpeedStat || Attr == Server.EnergyStat)
                                                {
                                                    AI.UpdateAttribute(Attr, Convert.ToInt32(Datas[1]));
                                                }
                                                else
                                                {
                                                    AI.Attributes.Value[Attr] = Convert.ToInt32(Datas[1]);
                                                    PacketWriter Pa = new PacketWriter();
                                                    Pa.Write(AI.GCLID);
                                                    Pa.Write("A", false);
                                                    Pa.Write((ushort)(AI.RuntimeID));
                                                    Pa.Write((byte)(Attr));
                                                    Pa.Write((ushort)(AI.Attributes.Maximum[Attr]));
                                                    RCEnet.Send(AI.RNID, MessageTypes.P_StatUpdate, Pa.ToArray(), true);
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SSetAttributeMax), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);

                                            int Attr = Attribute.FindAttribute(Datas[0]);

                                            if (Attr > -1)
                                            {
                                                if (Attr == Server.HealthStat || Attr == Server.SpeedStat || Attr == Server.EnergyStat)
                                                {
                                                    AI.UpdateAttributeMax(Attr, Convert.ToInt32(Datas[1]));
                                                }
                                                else
                                                {
                                                    AI.Attributes.Maximum[Attr] = Convert.ToInt32(Datas[1]);
                                                    PacketWriter Pa = new PacketWriter();
                                                    Pa.Write(AI.GCLID);
                                                    Pa.Write("M", false);
                                                    Pa.Write((ushort)(AI.RuntimeID));
                                                    Pa.Write((byte)(Attr));
                                                    Pa.Write((ushort)(AI.Attributes.Maximum[Attr]));
                                                    RCEnet.Send(AI.RNID, MessageTypes.P_StatUpdate, Pa.ToArray(), true);
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SScript), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);

                                            if (Datas.Length >= 2)
                                            {
                                                ScriptManager.DelayedExecute(Datas[0], Datas[1], AI, AI, null);
                                            }
                                        }
                                    }else if (Command.Equals(Language.Get(LanguageString.SGM), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(0);
                                            Pa.Write((byte)0);
                                            Pa.Write((byte)(254));
                                            Pa.Write("<GM> <" + AI.Name + "> " + Params, false);

                                            byte[] PaA = Pa.ToArray();


                                            foreach(ActorInstance A2 in ActorInstance.ActorInstanceList)
                                            {
                                                if (A2.RNID > 0 && A2.Account.IsGM)
                                                {
                                                    //TODO: Change this to HostToNetworkOrder!
                                                    BitConverter.GetBytes(A2.GCLID).CopyTo(PaA, 0);
                                                    RCEnet.Send(A2.RNID, MessageTypes.P_ChatMessage, PaA, true);
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SG), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if(AI.TeamID > 0)
                                        {
                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(0);
                                            Pa.Write((byte)0);
                                            Pa.Write((byte)(251));
                                            Pa.Write("<G> <" + AI.Name + "> " + Params, false);
                                            
                                            byte[] PaA = Pa.ToArray();

                                           
                                            foreach(ActorInstance A2 in ActorInstance.ActorInstanceList)
                                            {
                                                if (A2.RNID > 0 && A2.TeamID == AI.TeamID)
                                                {
                                                    //TODO: Change this to HostToNetworkOrder!
                                                    BitConverter.GetBytes(A2.GCLID).CopyTo(PaA, 0);
                                                    RCEnet.Send(A2.RNID, MessageTypes.P_ChatMessage, PaA, true);
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SP), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        Party Party = AI.PartyID;
                                        if(Party != null)
                                        {
                                            PacketWriter Pa = new PacketWriter();
                                            Pa.Write(0);
                                            Pa.Write((byte)0);
                                            Pa.Write((byte)(251));
                                            Pa.Write("<PARTY> <" + AI.Name + "> " + Params, false);

                                            byte[] PaA = Pa.ToArray();

                                            for(int i = 0; i < 8; ++i)
                                                if (Party.Player[i] != null && Party.Player[i] != AI)
                                                {
                                                    //TODO: Change this to HostToNetworkOrder!
                                                    BitConverter.GetBytes(Party.Player[i].GCLID).CopyTo(PaA, 0);
                                                    RCEnet.Send(Party.Player[i].RNID, MessageTypes.P_ChatMessage, PaA, true);
                                                }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SPM), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        string Name = CommaSplit.Split(Params)[0];
                                        Params = Params.Substring(Name.Length + 1).Trim();
                                        
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write(0);
                                        Pa.Write((byte)0);
                                        Pa.Write((byte)(252));
                                        Pa.Write(AI.Name + ": " + Params, false);

                                        byte[] PaA = Pa.ToArray();

                                        foreach(ActorInstance A2 in ActorInstance.ActorInstanceList)
                                        {
                                            if (A2.RNID > 0 && A2.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                //TODO: Change this to HostToNetworkOrder!
                                                BitConverter.GetBytes(A2.GCLID).CopyTo(PaA, 0);
                                                RCEnet.Send(A2.RNID, MessageTypes.P_ChatMessage, PaA, true);
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SAllPlayers), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        int Players = 0;
                                        foreach (ActorInstance A2 in ActorInstance.ActorInstanceList)
                                        {
                                            if (A2.RNID > 0)
                                                ++Players;
                                        }

                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write(AI.GCLID);
                                        Pa.Write((byte)0);
                                        Pa.Write((byte)(254));
                                        Pa.Write(Language.Get(LanguageString.PlayersInGame) + " " + (Players - 1).ToString(), false);
                                        RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);

                                    } else if (Command.Equals(Language.Get(LanguageString.SWarp), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);
                                            Area Ar = Area.Find(Datas[0]);

                                            if (Ar != null)
                                            {
                                                int Instance = Convert.ToInt32(Datas[1]);
                                                for (int i = 0; i < Ar.Portals.Length; ++i)
                                                {
                                                    if (Ar.Portals[i].Name.Length > 0)
                                                    {
                                                        AI.SetArea(Ar, Instance, -1, i);
                                                    }
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SWarpOther), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);

                                            foreach (ActorInstance A2 in ActorInstance.ActorInstanceList)
                                            {
                                                if (A2.RNID > 0 && A2.Name.Equals(Datas[0], StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    Area Ar = Area.Find(Datas[1]);
                                                    int Instance = Convert.ToInt32(Datas[2]);

                                                    for (int i = 0; i < Ar.Portals.Length; ++i)
                                                    {
                                                        if (Ar.Portals[i].Name.Length > 0)
                                                        {
                                                            A2.SetArea(Ar, Instance, -1, i);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    } else if (Command.Equals(Language.Get(LanguageString.SAbility), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (AI.accountDM)
                                        {
                                            string[] Datas = CommaSplit.Split(Params);

                                            foreach (Spell Sp in Spell.Spells)
                                            {
                                                if (Sp.Name.Equals(Datas[0], StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    AI.AddSpell(Sp.ID, Convert.ToInt32(Datas[1]));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (Command.Equals(Language.Get(LanguageString.SGive), StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        // Make sure it's a GM account
                                        if (AI.accountDM)
                                        {
                                            // Find the request item
                                            foreach (Item It in Item.Items)
                                            {
                                                if (It.Name.Equals(Params, StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    // Create the item
                                                    ItemInstance II = ItemInstance.CreateItemInstance(It);
                                                    II.Assignment = 1;
                                                    II.AssignTo = AI;

                                                    // Ask client to specify a slot to put it in
                                                    PacketWriter Pa = new PacketWriter();
                                                    Pa.Write(AI.GCLID);
                                                    Pa.Write("G", false);
                                                    Pa.Write(II.AllocID);
                                                    Pa.Write((ushort)(It.ID));
                                                    Pa.Write((ushort)(II.Assignment));
                                                    RCEnet.Send(AI.RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);

                                                    break;
                                                }
                                            }
                                        }
                                    }*/
                                        //                                     } else if (Command.Equals(Language.Get(LanguageString.SWeather), StringComparison.CurrentCultureIgnoreCase))
                                        //                                     {
                                        //                                         if (AI.accountDM)
                                        //                                         {
                                        //                                             Area.AreaInstance AInstance = AI.ServerArea;
                                        // 
                                        //                                             // Choose new weather
                                        //                                             if (Params.Equals("sun", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                 || Params.Equals("sunny", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                 || Params.Equals("normal", StringComparison.CurrentCultureIgnoreCase))
                                        //                                             {
                                        //                                                 AInstance.CurrentWeather = Weather.Sun;
                                        //                                             }
                                        //                                             else if (Params.Equals("rain", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                 || Params.Equals("rainy", StringComparison.CurrentCultureIgnoreCase))
                                        //                                             {
                                        //                                                 AInstance.CurrentWeather = Weather.Rain;
                                        //                                             }
                                        //                                             else if (Params.Equals("snow", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                || Params.Equals("snowy", StringComparison.CurrentCultureIgnoreCase))
                                        //                                             {
                                        //                                                 AInstance.CurrentWeather = Weather.Snow;
                                        //                                             }
                                        //                                             else if (Params.Equals("fog", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                || Params.Equals("foggy", StringComparison.CurrentCultureIgnoreCase))
                                        //                                             {
                                        //                                                 AInstance.CurrentWeather = Weather.Fog;
                                        //                                             }
                                        //                                             else if (Params.Equals("wind", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                || Params.Equals("windy", StringComparison.CurrentCultureIgnoreCase))
                                        //                                             {
                                        //                                                 AInstance.CurrentWeather = Weather.Wind;
                                        //                                             }
                                        //                                             else if (Params.Equals("storm", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                || Params.Equals("stormy", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                || Params.Equals("thunder", StringComparison.CurrentCultureIgnoreCase)
                                        //                                                || Params.Equals("lightning", StringComparison.CurrentCultureIgnoreCase))
                                        //                                             {
                                        //                                                 AInstance.CurrentWeather = Weather.Storm;
                                        //                                             }
                                        //                                             AInstance.CurrentWeatherTime = Server.Random.Next(2500, 10000);
                                        // 
                                        //                                             // Tell players in this area
                                        //                                             PacketWriter Pa = new PacketWriter();
                                        //                                             Pa.Write(0);
                                        //                                             Pa.Write(AInstance.AllocID);
                                        //                                             Pa.Write((byte)(AInstance.CurrentWeather));
                                        // 
                                        //                                             byte[] PaA = Pa.ToArray();
                                        // 
                                        //                                             foreach (Area.InstanceSector Sector in AInstance.Sectors)
                                        //                                             {
                                        //                                                 LinkedListNode<ActorInstance> AINode = Sector.Players.First;
                                        // 
                                        //                                                 while (AINode != null)
                                        //                                                 {
                                        //                                                     ActorInstance A2 = AINode.Value;
                                        //                                                     AINode = AINode.Next;
                                        // 
                                        //                                                     if (A2 != null && A2.RNID > 0)
                                        //                                                     {
                                        //                                                         //TODO: Use HostToNetworkOrder
                                        //                                                         BitConverter.GetBytes(A2.GCLID).CopyTo(PaA, 0);
                                        //                                                         RCEnet.Send(A2.RNID, MessageTypes.P_WeatherChange, PaA, true);
                                        //                                                     }
                                        //                                                 }
                                        //                                             }
                                        // 
                                        //                                             // Force an update for all areas with the weather linked to this area
                                        //                                             if (AInstance.ID == 0)
                                        //                                             {
                                        //                                                 foreach (Area Ar in Area.AreaList)
                                        //                                                 {
                                        //                                                     if (Ar.WeatherLinkArea == AInstance.Area)
                                        //                                                     {
                                        //                                                         for (int i = 0; i < 100; ++i)
                                        //                                                         {
                                        //                                                             if (Ar.Instances[i] != null)
                                        //                                                                 Ar.Instances[i].CurrentWeatherTime = 0;
                                        //                                                         }
                                        //                                                     }
                                        //                                                 }
                                        //                                             }
                                        //                                         }
                                        //                                     } else if (Command.Equals(Language.Get(LanguageString.STime), StringComparison.CurrentCultureIgnoreCase))
                                        //                                     {
                                        //                                         string Hour = WorldEnvironment.TimeH.ToString();
                                        //                                         string Minute = WorldEnvironment.TimeM.ToString();
                                        //                                         if (Hour.Length == 1)
                                        //                                             Hour = "0" + Hour;
                                        //                                         if (Minute.Length == 1)
                                        //                                             Minute = "0" + Minute;
                                        // 
                                        //                                         PacketWriter Pa = new PacketWriter();
                                        //                                         Pa.Write(AI.GCLID);
                                        //                                         Pa.Write((byte)0);
                                        //                                         Pa.Write((byte)(254));
                                        //                                         Pa.Write("Time: " + Hour + ":" + Minute, false);
                                        //                                         RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                        // 
                                        //                                     }
                                        //else
                                        {
                                            //ScriptManager.Execute("SlashCommands", "Main", new object[] { AI, Command, Params }, AI, null);
                                            ScriptManager.ExecuteSpecialScriptObject(Server.ChatProcessor, "OnSlashCommand",
                                                new object[] { AI, Command, Params });
                                        }
                                        #endregion
                                    }
                                    else // General chatter - broadcast messages
                                    {
                                        string MessageText = M.MessageData.ReadString(M.MessageData.Length - 4);

                                        ScriptManager.ExecuteSpecialScriptObject(Server.ChatProcessor, "OnChatText",
                                            new object[] { AI, MessageText });
                                        //                                     PacketWriter Pa = new PacketWriter();
                                        //                                     string StrLayout = "<" + AI.Name + "> " + M.MessageData.ReadString(M.MessageData.Length);
                                        //                                     Pa.Write(StrLayout, false);
                                        //                                     Area.AreaInstance AInstance = AI.ServerArea;
                                        //                                     ActorInstance A2 = AInstance.FirstInZone;
                                        //                                     while (A2 != null)
                                        //                                     {
                                        //                                         if (A2.RNID > 0)
                                        //                                             RCEnet.Send(A2.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                        //                                         A2 = A2.nextInZone;
                                        //                                     }
                                        // 
                                        //                                     Log.WriteLine(StrLayout);
                                    }
                                }
                                break;
                            }
                        case MessageTypes.P_RepositionActor:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null)
                                    AI.IgnoreUpdate = false;

                                break;
                            }
                        case MessageTypes.P_ChangeArea:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null)
                                    AI.IgnoreUpdate = false;

                                break;
                            }
                        case MessageTypes.P_SelectScenery:
                            {
                                //                             uint GCLID = M.MessageData.ReadUInt32();
                                //                             ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                // 
                                // 				            if(AI != null)
                                //                             {
                                // 					            int ID = M.MessageData.ReadUInt16();
                                // 					            if(ID > 0 && ID <= 500)
                                //                                 {
                                //                                     Area.AreaInstance AInstance = AI.ServerArea;
                                // 						            if(AInstance.OwnedScenery[ID - 1] != null)
                                //                                     {
                                // 							            if(AInstance.OwnedScenery[ID - 1].AccountName.Equals(AI.accountName, StringComparison.CurrentCultureIgnoreCase))
                                //                                         {
                                //                                             int Idx = AI.accountIndex;
                                // 
                                // 								            if(AInstance.OwnedScenery[ID - 1].CharNumber == Idx)
                                //                                             {
                                // 									            // Allow scenery animation
                                //                                                 PacketWriter Pa = new PacketWriter();
                                //                                                 Pa.Write(AI.GCLID);
                                //                                                 Pa.Write(M.MessageData.ReadInt32());
                                // 
                                //                                                 RCEnet.Send(AI.RNID, MessageTypes.P_SelectScenery, Pa.ToArray(), true);
                                // 
                                // 									            
                                // 								            }
                                //                                         }
                                // 						            }
                                // 					            }
                                // 				            }

                                break;
                            }
                        case MessageTypes.P_Jump:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null)
                                {
                                    if (AI.Mount == null)
                                    {
                                        // Tell other players in the same area
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write(0);
                                        Pa.Write((ushort)(AI.RuntimeID));

                                        byte[] PaA = Pa.ToArray();

                                        int MinX = (int)AI.CurrentSector.Sector.SectorX - 1;
                                        int MinZ = (int)AI.CurrentSector.Sector.SectorZ - 1;
                                        int MaxX = (int)AI.CurrentSector.Sector.SectorX + 2;
                                        int MaxZ = (int)AI.CurrentSector.Sector.SectorZ + 2;

                                        if (MinX < 0)
                                            MinX = 0;
                                        if (MinZ < 0)
                                            MinZ = 0;
                                        if (MaxX > AI.ServerArea.Sectors.GetLength(0))
                                            MaxX = AI.ServerArea.Sectors.GetLength(0);
                                        if (MaxZ > AI.ServerArea.Sectors.GetLength(1))
                                            MaxZ = AI.ServerArea.Sectors.GetLength(1);

                                        LinkedListNode<ActorInstance> A2Node;

                                        for (int z = MinZ; z < MaxZ; ++z)
                                        {
                                            for (int x = MinX; x < MaxX; ++x)
                                            {
                                                A2Node = AI.ServerArea.Sectors[x, z].Players.First;

                                                while (A2Node != null)
                                                {
                                                    if (A2Node.Value != null && A2Node.Value.RNID > 0)
                                                    {
                                                        //TODO: Use HostToNetworkOrder
                                                        BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                                        RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_Jump, PaA, true);
                                                    }

                                                    A2Node = A2Node.Next;
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_ActionBarUpdate:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null)
                                {

                                    byte Initial = M.MessageData.ReadByte();
                                    byte Num = M.MessageData.ReadByte();
                                    if (Num >= 0 && Num < 36)
                                    {
                                        if (Initial == (byte)('S'))
                                            AI.ActionBar.Slots[Num] = "S" + M.MessageData.ReadString(M.MessageData.Length - M.MessageData.Location);
                                        else if (Initial == (byte)('I'))
                                            AI.ActionBar.Slots[Num] = "I" + M.MessageData.ReadString(2);
                                        else if (Initial == (byte)('N'))
                                            AI.ActionBar.Slots[Num] = "";
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_SpellUpdate:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                if (AI != null)
                                {
                                    byte MessageType = M.MessageData.ReadByte();

                                    switch (MessageType)
                                    {
                                        case 85: // 'U' Player has unmemorized a spell
                                            {
                                                if (Server.RequireMemorise)
                                                {
                                                    int Num = (int)(M.MessageData.ReadUInt16());

                                                    for (int i = 0; i < 10; ++i)
                                                    {
                                                        if (AI.MemorisedSpells[i] == Num)
                                                        {
                                                            AI.MemorisedSpells[i] = 5000;
                                                            break;
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                        case 77: // 'M' Player is memorizing a spell
                                            {
                                                if (Server.RequireMemorise)
                                                {
                                                    int KnownNum = (int)(M.MessageData.ReadUInt16());

                                                    if (KnownNum >= 0 && KnownNum <= 999)
                                                    {
                                                        MemorizingSpell MS = new MemorizingSpell();
                                                        MS.KnownNum = KnownNum;
                                                        MS.CreatedTime = Server.MilliSecs();
                                                        AI.MemorizingSpells.AddLast(MS);
                                                    }
                                                }

                                                break;
                                            }
                                        case 70: // 'F' Player is firing a spell
                                            {
                                                // Spell ID and target
                                                int Num = (int)(M.MessageData.ReadUInt16());
                                                ActorInstance Context = null;

                                                if (M.MessageData.Length > 7)
                                                {
                                                    int RuntimeID = (int)(M.MessageData.ReadUInt16());
                                                    Context = ActorInstance.FindFromRuntimeID(RuntimeID);
                                                }

                                                // Convert ID into known spell number
                                                bool Found = false;
                                                for (int i = 0; i < 1000; ++i)
                                                {
                                                    if (AI.KnownSpells[i] == Num)
                                                    {
                                                        Num = i;
                                                        Found = true;
                                                        break;
                                                    }
                                                }

                                                if (Found == true)
                                                {
                                                    // Spell must be memorized to fire
                                                    if (Server.RequireMemorise)
                                                    {
                                                        for (int i = 0; i < 10; ++i)
                                                        {
                                                            if (AI.MemorisedSpells[i] == Num)
                                                            {
                                                                Spell Sp = Spell.Spells[AI.KnownSpells[Num]];

                                                                if (Sp != null)
                                                                {
                                                                    if (AI.SpellCharge[i] <= 0)
                                                                    {
                                                                        string method = Sp.Method;
                                                                        if (string.IsNullOrEmpty(Sp.Method))
                                                                            method = "Main";

                                                                        ScriptManager.Execute(Sp.Script, method, new object[] { AI, Context, Sp.Name, AI.SpellLevels[Num] }, AI, null);
                                                                        AI.SpellCharge[i] = Sp.RechargeTime;
                                                                    }
                                                                    else
                                                                    {

                                                                        PacketWriter Pa = new PacketWriter();
                                                                        Pa.Write(AI.GCLID);
                                                                        Pa.Write((byte)0);
                                                                        Pa.Write((byte)(253));
                                                                        Pa.Write(Language.Get(LanguageString.AbilityNotRecharged), false);
                                                                        RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                                                    }
                                                                }

                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else // Fire spell directly
                                                    {
                                                        Spell Sp = Spell.Spells[AI.KnownSpells[Num]];

                                                        if (Sp != null)
                                                        {
                                                            if (AI.SpellCharge[Num] <= 0)
                                                            {
                                                                string method = Sp.Method;
                                                                if (string.IsNullOrEmpty(Sp.Method))
                                                                    method = "Main";

                                                                ScriptManager.Execute(Sp.Script, method, new object[] { AI, Context, Sp.Name, AI.SpellLevels[Num] }, AI, null);
                                                                AI.SpellCharge[Num] = Sp.RechargeTime;
                                                            }
                                                            else
                                                            {

                                                                PacketWriter Pa = new PacketWriter();
                                                                Pa.Write(AI.GCLID);
                                                                Pa.Write((byte)0);
                                                                Pa.Write((byte)(253));
                                                                Pa.Write(Language.Get(LanguageString.AbilityNotRecharged), false);
                                                                RCEnet.Send(AI.RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                                                            }
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }

                                }

                                break;
                            }
                        case MessageTypes.P_Dialog:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                byte MessageType = M.MessageData.ReadByte();

                                switch (MessageType)
                                {
                                    case 79: // 'O' Dialog option picked
                                        {
                                            int ScriptID = M.MessageData.ReadInt32();

                                            RCPServer.ActorDialog D = RCPServer.ActorDialog.FindActorDialog(ScriptID);
                                            if (D != null)
                                                D.ReceiveInput(M.MessageData.ReadByte() - 1);

                                            break;
                                        }
                                }

                                break;
                            }
                        case MessageTypes.P_ScriptInput:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                int ScriptID = M.MessageData.ReadInt32();
                                string InputData = M.MessageData.ReadString(M.MessageData.Length - 8);

                                // WaitInput callback!
                                throw new Exception("JB: WaitInput not implemented");

                                break;
                            }
                        case MessageTypes.P_EatItem:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                if (AI != null)
                                {
                                    byte Slot = M.MessageData.ReadByte();
                                    int Amount = (int)(M.MessageData.ReadUInt16());

                                    if (Slot >= 0 && Slot < 50 && Amount > 0)
                                    {
                                        if (AI.Inventory.Items[Slot] != null && AI.Inventory.Amounts[Slot] >= Amount)
                                        {
                                            if (AI.Inventory.Items[Slot].Item.ItemType == ItemTypes.I_Armour || AI.Inventory.Items[Slot].Item.ItemType == ItemTypes.I_Ingredient
                                                || AI.Inventory.Items[Slot].Item.ItemType == ItemTypes.I_Potion || AI.Inventory.Items[Slot].Item.ItemType == ItemTypes.I_Image)
                                            {
                                                if (AI.Actor.Class.Equals(AI.Inventory.Items[Slot].Item.ExclusiveClass, StringComparison.CurrentCultureIgnoreCase) || AI.Inventory.Items[Slot].Item.ExclusiveClass.Length == 0)
                                                {
                                                    if (AI.Actor.Race.Equals(AI.Inventory.Items[Slot].Item.ExclusiveRace, StringComparison.CurrentCultureIgnoreCase) || AI.Inventory.Items[Slot].Item.ExclusiveRace.Length == 0)
                                                    {
                                                        // Create buff
                                                        string EffectName = AI.Inventory.Items[Slot].Item.Name;
                                                        bool Found = false;
                                                        ActorEffect FoundAE = null;

                                                        foreach (ActorEffect AE in AI.ActorEffects)
                                                        {
                                                            if (AE.Name.Equals(EffectName, StringComparison.CurrentCultureIgnoreCase))
                                                            {
                                                                Found = true;
                                                                FoundAE = AE;
                                                                break;
                                                            }
                                                        }

                                                        if (!Found)
                                                        {
                                                            FoundAE = new ActorEffect();
                                                            FoundAE.Name = EffectName;
                                                            AI.ActorEffects.AddLast(FoundAE);

                                                            PacketWriter Pa = new PacketWriter();
                                                            Pa.Write(AI.GCLID);
                                                            Pa.Write((byte)('A'));
                                                            Pa.Write(FoundAE.AllocID);
                                                            Pa.Write(AI.Inventory.Items[Slot].Item.ThumbnailTexID);
                                                            Pa.Write(FoundAE.Name, false);

                                                            RCEnet.Send(AI.RNID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                                                        }

                                                        FoundAE.CreatedTime = Server.MilliSecs();
                                                        FoundAE.Length = (uint)AI.Inventory.Items[Slot].Item.EatEffectsLength * 1000;

                                                        for (int i = 0; i < 40; ++i)
                                                        {
                                                            if (AI.Inventory.Items[Slot].Attributes.Value[i] != 0)
                                                            {
                                                                int Old = FoundAE.Attributes.Value[i];
                                                                FoundAE.Attributes.Value[i] = AI.Inventory.Items[Slot].Attributes.Value[i];

                                                                PacketWriter Pa = new PacketWriter();
                                                                Pa.Write(AI.GCLID);
                                                                Pa.Write((byte)('E'));
                                                                Pa.Write((byte)(i));
                                                                Pa.Write(FoundAE.Attributes.Value[i] - Old);

                                                                AI.Attributes.Value[i] = AI.Attributes.Value[i] + (FoundAE.Attributes.Value[i] - Old);
                                                                RCEnet.Send(AI.RNID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                                                            }
                                                        }

                                                        // Execute Item Script
                                                        if (AI.Inventory.Items[Slot].Item.Script.Length > 0 && AI.Inventory.Amounts[Slot] > 0)
                                                        {
                                                            string method = AI.Inventory.Items[Slot].Item.Method;
                                                            if (AI.Inventory.Items[Slot].Item.Method.Length == 0)
                                                                method = "Main";

                                                            
                                                            bool result = ScriptManager.Execute(AI.Inventory.Items[Slot].Item.Script, method, new object[] { AI, null, AI.Inventory.Items[Slot] }, AI, null);
                                                            //RCScript.Log("Triggered item script: " + method + " " + result);
                                                        }

                                                        // Remove item
                                                        AI.Inventory.Amounts[Slot] = AI.Inventory.Amounts[Slot] - Amount;
                                                        if (AI.Inventory.Amounts[Slot] <= 0)
                                                            AI.Inventory.Items[Slot] = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_ItemScript:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                ActorInstance A2 = null;

                                if (AI != null && (M.MessageData.Length == 5 || M.MessageData.Length == 7))
                                {
                                    byte SlotIndex = M.MessageData.ReadByte();
                                    if (M.MessageData.Length == 7)
                                        A2 = ActorInstance.FindFromRuntimeID(M.MessageData.ReadUInt16());

                                    if (SlotIndex >= 0 && SlotIndex < 50)
                                    {
                                        if (AI.Inventory.Items[SlotIndex] != null)
                                        {
                                            if (AI.Inventory.Items[SlotIndex].Item.ExclusiveClass.Length == 0 || AI.Actor.Race.Equals(AI.Inventory.Items[SlotIndex].Item.ExclusiveClass, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                if (AI.Inventory.Items[SlotIndex].Item.ExclusiveRace.Length == 0 || AI.Actor.Race.Equals(AI.Inventory.Items[SlotIndex].Item.ExclusiveRace, StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (AI.Inventory.Items[SlotIndex].Item.Script.Length > 0 && AI.Inventory.Amounts[SlotIndex] > 0)
                                                    {
                                                        string method = AI.Inventory.Items[SlotIndex].Item.Method;
                                                        if(method.Length == 0)
                                                            method = "Main";

                                                        bool result = ScriptManager.Execute(AI.Inventory.Items[SlotIndex].Item.Script, method, new object[] { AI, A2, AI.Inventory.Items[SlotIndex]}, AI, null);
                                                        //RCScript.Log("Triggered item script: " + AI.Inventory.Items[SlotIndex].Item.Script + " " + method + " " + result);

                                                    }
                                                }
                                            }                                        }
                                    }
                                }
                                break;
                            }
                        case MessageTypes.P_RightClick:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                if (AI != null && M.MessageData.Length == 6)
                                {
                                    ActorInstance A2 = ActorInstance.FindFromRuntimeID(M.MessageData.ReadUInt16());

                                    if (A2 != null)
                                    {
                                        Scripting.Math.SectorVector DistVec = AI.Position - A2.Position;
                                        float XDist = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                        float ZDist = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                        float YDist = Math.Abs(DistVec.Y);
                                        float Dist = (XDist * XDist) + (ZDist * ZDist) + (YDist * YDist);

                                        if (Dist < Server.InteractDist)
                                        {
                                            // Start right click script
                                            if (A2.Script.Length > 0)
                                            {
                                                bool Running = false;

                                                if (ScriptBase.FindScriptFromTags(AI, A2) != null)
                                                    Running = true;

                                                if (Running == false)
                                                    ScriptManager.Execute(A2.Script, "OnInteract", new object[] { AI, A2 }, AI, A2);

                                            }
                                            else if (A2.Actor.Rideable == true) // No script to run, if actor can be ridden, mount it
                                            {
                                                if ((A2.Leader == null || A2.Leader == AI) && AI.Mount == null)
                                                {
                                                    AI.mount = A2;
                                                    A2.rider = AI;
                                                    A2.AIMode = AIModes.AI_Wait;

                                                    ScriptManager.Execute("Mount", "Mount", new object[] { AI, A2 }, AI, A2);
                                                }
                                            }

                                            // Continue any paused scripts waiting for this conversation
                                            // Update WaitSpeaks
                                            LinkedListNode<WaitSpeakRequest> WsNode = AI.WaitSpeakRequests.First;
                                            while (WsNode != null)
                                            {
                                                WaitSpeakRequest Ws = WsNode.Value;
                                                LinkedListNode<WaitSpeakRequest> Del = WsNode;
                                                WsNode = WsNode.Next;

                                                if ((string.IsNullOrEmpty(Ws.ZoneName) || Ws.ZoneName.Equals(A2.Area, StringComparison.CurrentCultureIgnoreCase)) && Ws.ActorName.Equals(A2.name, StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (Ws.CallScript)
                                                    {
                                                        ScriptManager.DelayedExecute(Ws.CallbackScript, Ws.CallbackMethod, new object[] { AI, A2 }, AI, A2);
                                                    }
                                                    else
                                                    {
                                                        Ws.Invoke(AI, A2);
                                                    }

                                                    AI.WaitSpeakRequests.Remove(Del);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Log.WriteLine("DistFail: " + Server.InteractDist);
                                        }
                                    }
                                    else
                                    {
                                        Log.WriteLine("A2 was null");
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_SceneryInteract:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                if (AI != null && AI.CurrentSector != null)
                                {
                                    Scripting.Math.SectorVector Position = new Scripting.Math.SectorVector();
                                    Position.X = M.MessageData.ReadSingle();
                                    Position.Y = M.MessageData.ReadSingle();
                                    Position.Z = M.MessageData.ReadSingle();
                                    Position.SectorX = M.MessageData.ReadUInt16();
                                    Position.SectorZ = M.MessageData.ReadUInt16();
                                    ushort MeshID = M.MessageData.ReadUInt16();

                                    InteractiveSceneryInstance SceneObj = null;

                                    // Make sure position is in a valid sector
                                    if (Position.SectorX < AI.ServerArea.Sectors.GetLength(0) && Position.SectorZ < AI.ServerArea.Sectors.GetLength(1))
                                    {
                                        for (int i = 0; i < AI.ServerArea.Sectors[Position.SectorX, Position.SectorZ].SceneryInstances.Length; ++i)
                                        {
                                            InteractiveSceneryInstance Si = AI.ServerArea.Sectors[Position.SectorX, Position.SectorZ].SceneryInstances[i];

                                            if (Si.Scenery.Position == Position && Si.Scenery.MeshID == MeshID)
                                            {
                                                SceneObj = Si;
                                                break;
                                            }
                                        }
                                    }

                                    if (SceneObj != null)
                                    {
                                        // Check distance
                                        Scripting.Math.SectorVector DistVec = Position - AI.Position;
                                        float XDist = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                        float ZDist = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                        float YDist = Math.Abs(DistVec.Y);
                                        float Dist = (XDist * XDist) + (ZDist * ZDist) + (YDist * YDist);

                                        if (Dist < Server.InteractDist)
                                        {
                                            if (SceneObj.Scenery.ScriptName.Length > 0)
                                            {
                                                // If we are not already interacting, then interact
                                                if (ScriptBase.FindScriptFromTags(AI, SceneObj) == null)
                                                    ScriptManager.Execute(SceneObj.Scenery.ScriptName, "OnInteract", new object[] { AI, SceneObj }, AI, SceneObj);
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_AttackActor:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null && M.MessageData.Length == 6)
                                {
                                    // Check combat delay and whether actor is riding a mount, to prevent cheating
                                    if (Server.MilliSecs() - AI.LastAttack >= Server.CombatDelay && AI.Mount == null)
                                    {
                                        ActorInstance A2 = ActorInstance.FindFromRuntimeID(M.MessageData.ReadUInt16());
                                        if (A2 != null)
                                        {
                                            Area.AreaInstance AInstance = AI.ServerArea;
                                            if (A2.RNID < 0 || AInstance.Area.PvP == true)
                                            {
                                                AI.Attack(A2);
                                                    //Log.WriteLine("Attack Failed?");
                                                AI.AITarget = A2;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_InventoryUpdate:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                byte Request = M.MessageData.ReadByte();

                                switch (Request)
                                {
                                    case 80: //'P': // Request to pick up a dropped item
                                        {
                                            int AllocID = M.MessageData.ReadInt32();
                                            DroppedItem D = DroppedItem.FromAllocID(AllocID);

                                            if (D != null)
                                            {
                                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                                if (AI != null)
                                                {
                                                    byte SlotI = M.MessageData.ReadByte();

                                                    if (AI.Inventory.Items[SlotI] == null || (AI.Inventory.Items[SlotI].Identical(D.Item) && D.Item.Item.Stackable == true && SlotI >= (int)ItemSlots.Backpack))
                                                    {
                                                        if (D.Item.Item.SlotsMatch(SlotI) && AI.Actor.HasSlot(SlotI, D.Item.Item))
                                                        {
                                                            // Put into players' inventory
                                                            if (AI.Inventory.Items[SlotI] != null)
                                                                AI.Inventory.Items[SlotI] = null;
                                                            else
                                                                AI.Inventory.Amounts[SlotI] = 0;

                                                            AI.Inventory.Items[SlotI] = D.Item;
                                                            AI.Inventory.Amounts[SlotI] = AI.Inventory.Amounts[SlotI] + D.Amount;
                                                            if (SlotI < (int)ItemSlots.Backpack)
                                                                AI.SendEquippedUpdate();

                                                            // Tell this player he received it
                                                            PacketWriter Pa = new PacketWriter();
                                                            Pa.Write(AI.GCLID);
                                                            Pa.Write((byte)('R'));
                                                            Pa.Write(D.AllocID);
                                                            Pa.Write(SlotI);

                                                            RCEnet.Send(AI.RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);

                                                            // Tell other players it's gone
                                                            Pa = new PacketWriter();
                                                            Pa.Write(0);
                                                            Pa.Write((byte)('P'));
                                                            Pa.Write(D.AllocID);

                                                            byte[] PaA = Pa.ToArray();

                                                            int MinX = (int)D.Position.SectorX - 1;
                                                            int MinZ = (int)D.Position.SectorZ - 1;
                                                            int MaxX = (int)D.Position.SectorX + 2;
                                                            int MaxZ = (int)D.Position.SectorZ + 2;

                                                            if (MinX < 0)
                                                                MinX = 0;
                                                            if (MinZ < 0)
                                                                MinZ = 0;
                                                            if (MaxX > D.ServerHandle.Sectors.GetLength(0))
                                                                MaxX = D.ServerHandle.Sectors.GetLength(0);
                                                            if (MaxZ > D.ServerHandle.Sectors.GetLength(1))
                                                                MaxZ = D.ServerHandle.Sectors.GetLength(1);

                                                            LinkedListNode<ActorInstance> A2Node;

                                                            for (int z = MinZ; z < MaxZ; ++z)
                                                            {
                                                                for (int x = MinX; x < MaxX; ++x)
                                                                {
                                                                    A2Node = D.ServerHandle.Sectors[x, z].Players.First;

                                                                    while (A2Node != null)
                                                                    {
                                                                        if (A2Node.Value != null && A2Node.Value.RNID > 0 && A2Node.Value != AI)
                                                                        {
                                                                            //TODO: Use HostToNetworkOrder
                                                                            BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                                                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_InventoryUpdate, PaA, true);
                                                                        }

                                                                        A2Node = A2Node.Next;
                                                                    }
                                                                }
                                                            }

                                                            DroppedItem.Delete(D);
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                        }


                                    case 68: //'D': // Item Dropped
                                        {
                                            ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                            if (AI != null)
                                            {
                                                byte Slot = M.MessageData.ReadByte();
                                                ushort Amount = M.MessageData.ReadUInt16();
                                                ItemInstance DropResult = AI.Inventory.Drop(Slot, Amount);
                                                if (DropResult != null)
                                                {
                                                    AI.SendEquippedUpdate();

                                                    // Create item on the floor
                                                    DroppedItem D = new DroppedItem(AI.CurrentSector);
                                                    D.Item = DropResult;
                                                    D.Amount = Amount;
                                                    D.Position = AI.Position;
                                                    D.ServerHandle = AI.ServerArea;

                                                    // Tell other players in this area
                                                    PacketWriter Pa = new PacketWriter();
                                                    Pa.Write(0);
                                                    Pa.Write((byte)('D'));
                                                    Pa.Write((ushort)(Amount));
                                                    Pa.Write(D.Position);
                                                    Pa.Write(D.AllocID);
                                                    Pa.Write(D.Item.ToArray());

                                                    byte[] PaA = Pa.ToArray();

                                                    int MinX = (int)D.Position.SectorX - 1;
                                                    int MinZ = (int)D.Position.SectorZ - 1;
                                                    int MaxX = (int)D.Position.SectorX + 2;
                                                    int MaxZ = (int)D.Position.SectorZ + 2;

                                                    if (MinX < 0)
                                                        MinX = 0;
                                                    if (MinZ < 0)
                                                        MinZ = 0;
                                                    if (MaxX > D.ServerHandle.Sectors.GetLength(0))
                                                        MaxX = D.ServerHandle.Sectors.GetLength(0);
                                                    if (MaxZ > D.ServerHandle.Sectors.GetLength(1))
                                                        MaxZ = D.ServerHandle.Sectors.GetLength(1);

                                                    LinkedListNode<ActorInstance> A2Node;

                                                    for (int z = MinZ; z < MaxZ; ++z)
                                                    {
                                                        for (int x = MinX; x < MaxX; ++x)
                                                        {
                                                            A2Node = D.ServerHandle.Sectors[x, z].Players.First;

                                                            while (A2Node != null)
                                                            {
                                                                if (A2Node.Value != null && A2Node.Value.RNID > 0 && A2Node.Value != AI)
                                                                {
                                                                    //TODO: Use HostToNetworkOrder
                                                                    BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                                                    RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_InventoryUpdate, PaA, true);
                                                                }

                                                                A2Node = A2Node.Next;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                        }

                                    case 71: //'G': // Reply to given item message
                                        {
                                            ActorInstance AI = ActorInstance.FromGCLID(GCLID);

                                            byte Dat = M.MessageData.ReadByte();
                                            ItemInstance II = ItemInstance.FromAllocID(M.MessageData.ReadInt32());
                                            if (II != null && AI != null)
                                            {
                                                if (II.Assignment > 0)
                                                {
                                                    if (Dat == (byte)('Y'))
                                                    {
                                                        byte SlotI = M.MessageData.ReadByte();
                                                        if (M.MessageData.Location != M.MessageData.Length)
                                                            throw new Exception("Port Error: SlotI is supposed to be the LAST byte");

                                                        if (AI.Inventory.Items[SlotI] == null || (AI.Inventory.Items[SlotI].Identical(II) && II.Item.Stackable == true && SlotI >= (byte)ItemSlots.Backpack))
                                                        {
                                                            if (II.Item.SlotsMatch(SlotI) && AI.Actor.HasSlot(SlotI, II.Item))
                                                            {
                                                                if (AI.Inventory.Items[SlotI] != null)
                                                                    AI.Inventory.Items[SlotI] = null;
                                                                else
                                                                    AI.Inventory.Amounts[SlotI] = 0;

                                                                AI.Inventory.Items[SlotI] = II;
                                                                AI.Inventory.Amounts[SlotI] = AI.Inventory.Amounts[SlotI] + II.Assignment;
                                                                II.Assignment = 0;
                                                                if (SlotI < (byte)ItemSlots.Backpack)
                                                                    AI.SendEquippedUpdate();
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        II.ClientInstanceRemoved();
                                                    }
                                                }
                                            }

                                            break;
                                        }

                                    case 83: //'S': // Swap
                                    case 65: //'A': // Add
                                        {
                                            ushort RuntimeID = M.MessageData.ReadUInt16();
                                            byte SlotA = M.MessageData.ReadByte();
                                            byte SlotB = M.MessageData.ReadByte();
                                            ushort Amount = M.MessageData.ReadUInt16();

                                            ActorInstance AI = ActorInstance.FindFromRuntimeID(RuntimeID);
                                            ActorInstance AIFrom = ActorInstance.FromGCLID(GCLID);

                                            // Check that the actor instance is value (eg, not editing someone elses inventory)
                                            if (AI != null && AIFrom != null)
                                            {
                                                bool IsPet = false;
                                                int Slaves = AIFrom.NumberOfSlaves;
                                                while (Slaves > 0)
                                                {
                                                    foreach (ActorInstance Slave in ActorInstance.ActorInstanceList)
                                                    {
                                                        if (Slave.Leader == AIFrom)
                                                        {
                                                            --Slaves;
                                                            if (Slave == AI)
                                                            {
                                                                IsPet = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                if ((AI == AIFrom || IsPet == true) && (Amount == 0 || Amount <= AI.Inventory.Amounts[SlotA]))
                                                {
                                                    if (Request == (byte)('S'))
                                                    {
                                                        AI.Inventory.Swap(AI, SlotA, SlotB, Amount);
                                                        if (SlotA < (byte)ItemSlots.Backpack || SlotB < (byte)ItemSlots.Backpack)
                                                            AI.SendEquippedUpdate();
                                                    }
                                                    else
                                                    {
                                                        AI.Inventory.Add(AI, SlotA, SlotB, Amount);
                                                        if (SlotB < (byte)ItemSlots.Backpack)
                                                            AI.SendEquippedUpdate();
                                                    }
                                                }
                                            }

                                            break;
                                        }
                                }

                                break;
                            }
                        case MessageTypes.P_Dismount:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null && AI.Mount != null)
                                {
                                    ScriptManager.DelayedExecute("Mount", "OnDismount", new object[] { AI, AI.Mount }, AI, AI.Mount);
                                    if (AI.mount.RNID < 0)
                                        AI.mount.AIMode = AIModes.AI_Patrol;

                                    AI.mount.rider = null;
                                    AI.mount.WalkingBackward = false;
                                    AI.mount.Destination = AI.mount.Position;
                                    AI.mount = null;
                                }
                                break;
                            }
                        case MessageTypes.P_StandardUpdate:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null)
                                {
                                    // Only process when the client isn't changing zone
                                    if (AI.IgnoreUpdate == false)
                                    {
                                        // Player can only move if hes not mounted
                                        if (AI.Rider == null)
                                        {
                                            AI.Destination = M.MessageData.ReadSectorVector();
                                            AI.PreviousPosition = AI.Position;
                                            AI.Position = M.MessageData.ReadSectorVector();

                                            //short YawShort = (short)M.MessageData.ReadUInt16();
                                            float Yaw = (float)M.MessageData.ReadSingle();
                                            //Yaw /= 32767.0f;
                                            //Yaw *= 180.0f;

                                            AI.Yaw = Yaw;

                                            AI.IsRunning = M.MessageData.ReadByte() > 0;

                                            byte Flags = M.MessageData.ReadByte();
                                            AI.WalkingBackward = (Flags & 1) > 0;
                                            AI.StrafingLeft = (Flags & 2) > 0;
                                            AI.StrafingRight = (Flags & 4) > 0;

                                            AI.Position.FixValues();

                                            // Players cannot run backwards, prevent cheating
                                            if (AI.WalkingBackward)
                                                AI.IsRunning = false;

                                            // Check sector
                                            if (AI.ServerArea != null)
                                            {
                                                int MaxX = AI.ServerArea.Sectors.GetLength(0);
                                                int MaxZ = AI.ServerArea.Sectors.GetLength(1);

                                                int Sx = AI.Position.SectorX;
                                                int Sz = AI.Position.SectorZ;

                                                if (Sx >= MaxX)
                                                    Sx = MaxX - 1;
                                                if (Sz >= MaxZ)
                                                    Sz = MaxZ - 1;

                                                if (AI.ServerArea.Sectors[Sx, Sz] != AI.CurrentSector)
                                                {
                                                    AI.ServerArea.ActorChangedSector(AI);

                                                    if (AI.mount != null)
                                                        AI.mount.ServerArea.ActorChangedSector(AI.mount);
                                                }
                                            }


                                            // Move mount to same position if the player is riding one
                                            if (AI.mount != null)
                                            {
                                                AI.mount.Position = AI.Position;
                                                AI.mount.PreviousPosition = AI.Position;
                                                AI.mount.Destination = AI.Destination;
                                                AI.mount.IsRunning = AI.IsRunning;
                                                AI.mount.WalkingBackward = AI.WalkingBackward;
                                            }
                                        }
                                    }
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
                        case MessageTypes.P_ServerList:
                            {
                                while (M.MessageData != null && M.MessageData.Location <= M.MessageData.Length - 1)
                                {
                                    byte Cmd = M.MessageData.ReadByte();

                                    if (Cmd == (byte)'A')
                                    {
                                        byte StrLen = M.MessageData.ReadByte();
                                        string ZoneName = M.MessageData.ReadString(StrLen);
                                        ushort InstanceID = M.MessageData.ReadUInt16();

                                        Area Ar = Area.Find(ZoneName);
                                        if (Ar == null)
                                        {
                                            Log.WriteLine("Error: Could not enable zone (" + ZoneName + ":" + InstanceID.ToString() + ") as it does not exist!");
                                        }
                                        else
                                        {
                                            if (!Ar.Instances.ContainsKey(InstanceID) || Ar.Instances[InstanceID] == null)
                                                Ar.CreateInstance(InstanceID);

                                            Ar.Instances[InstanceID].Active = true;
                                            foreach (Area.InstanceSector Sector in Ar.Instances[InstanceID].Sectors)
                                                Sector.Active = true;
                                            Log.WriteLine("Zone (" + ZoneName + ":" + InstanceID.ToString() + ") passed for processing.");

                                            // TODO: Passed with MT2!!
                                            byte VirtualZoneID = 0;
                                            string Script = Ar.Script;
                                            if (string.IsNullOrEmpty(Script))
                                                Script = "DefaultZoneScript";

                                            ScriptManager.Execute(Script, "OnActivate", new object[] { (Scripting.ZoneInstance)Ar.Instances[InstanceID], VirtualZoneID }, Ar.Instances[InstanceID], null);

                                            int DataLength = M.MessageData.Length - M.MessageData.Location;//M.MessageData.ReadInt32();
                                            if (DataLength > 0)
                                            {
                                                Ar.Instances[InstanceID].Deserialize(M.MessageData);
                                            }
                                        }
                                    }
                                    else if (Cmd == (byte)'U')
                                    {
                                        byte StrLen = M.MessageData.ReadByte();
                                        string ZoneName = M.MessageData.ReadString(StrLen);
                                        ushort InstanceID = M.MessageData.ReadUInt16();

                                        Area Ar = Area.Find(ZoneName);
                                        if (Ar == null)
                                        {
                                            Log.WriteLine("Error: Could not disable zone (" + ZoneName + ":" + InstanceID.ToString() + ") as it does not exist!");
                                        }
                                        else
                                        {
                                            if (Ar.Instances[InstanceID] != null)
                                            {
                                                if (Ar.Instances[InstanceID].Active)
                                                {
                                                    Ar.Instances[InstanceID].Active = false;
                                                    foreach (Area.InstanceSector Sector in Ar.Instances[InstanceID].Sectors)
                                                        Sector.Active = false;
                                                    Log.WriteLine("Zone (" + ZoneName + ":" + InstanceID.ToString() + ") disabled.");

                                                    // TODO: Passed with MT2!!!
                                                    byte VirtualZoneID = 0;

                                                    // Close active zone script
                                                    ScriptBase RunningScript = ScriptBase.FindScriptFromTags(Ar.Instances[InstanceID], null);
                                                    if (RunningScript != null)
                                                    {
                                                        ScriptManager.ExecuteSpecialScriptObject(RunningScript, "OnDeactivate", new object[] { Ar.Instances[InstanceID], VirtualZoneID });
                                                    }

                                                    // Remove all players in zone
                                                    LinkedList<ActorInstance> MoveActors = new LinkedList<ActorInstance>();

                                                    foreach (Area.InstanceSector Sector in Ar.Instances[InstanceID].Sectors)
                                                    {
                                                        LinkedListNode<ActorInstance> AINode = Sector.Players.First;

                                                        while (AINode != null)
                                                        {
                                                            if (AINode.Value != null)
                                                            {
                                                                if (AINode.Value.CurrentSector != Sector)
                                                                    AINode.Value.CurrentSector = Sector;
                                                                MoveActors.AddLast(AINode.Value);
                                                            }
                                                            AINode = AINode.Next;
                                                        }
                                                    }

                                                    foreach (ActorInstance AIt in MoveActors)
                                                    {
                                                        int MinX = AIt.Position.SectorX - 1;
                                                        int MinZ = AIt.Position.SectorZ - 1;
                                                        int MaxX = AIt.Position.SectorX + 2;
                                                        int MaxZ = AIt.Position.SectorZ + 2;

                                                        if (MinX < 0) MinX = 0;
                                                        if (MinZ < 0) MinZ = 0;
                                                        if (MaxX > Ar.Sectors.GetLength(0))
                                                            MaxX = Ar.Sectors.GetLength(0);
                                                        if (MaxZ > Ar.Sectors.GetLength(1))
                                                            MaxZ = Ar.Sectors.GetLength(1);

                                                        List<Area.InstanceSector> MovedFrom = new List<Area.InstanceSector>(10);

                                                        for (int z = MinZ; z < MaxZ; ++z)
                                                        {
                                                            for (int x = MinX; x < MaxX; ++x)
                                                            {
                                                                MovedFrom.Add(Ar.Instances[InstanceID].Sectors[x, z]);
                                                            }
                                                        }

                                                        Ar.Instances[InstanceID].ActorLeftSectors(AIt, MovedFrom);
                                                        AIt.SetArea(Ar, InstanceID, -1, -1, AIt.Position);
                                                    }

                                                    int DataLength = M.MessageData.ReadInt32();

                                                    // It wants it saved!
                                                    if (DataLength == -1)
                                                    {
                                                        Ar.RemoveInstance(InstanceID, true);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (Cmd == (byte)'R')
                                    {
                                        byte StrLen = M.MessageData.ReadByte();
                                        string ZoneName = M.MessageData.ReadString(StrLen);
                                        ushort InstanceID = M.MessageData.ReadUInt16();

                                        Area Ar = Area.Find(ZoneName);
                                        if (Ar == null)
                                        {
                                            Log.WriteLine("Error: Could not delete zone (" + ZoneName + ":" + InstanceID.ToString() + ") as it does not exist!");
                                        }
                                        else
                                        {
                                            Ar.RemoveInstance(InstanceID, false);
                                            Ar.Instances.Remove(InstanceID);

                                            Log.WriteLine(string.Format("Zone ({0}:{1}) was removed", new string[] { ZoneName, InstanceID.ToString() }));
                                        }
                                    }
                                    else if (Cmd == (byte)'S')
                                    {
                                        ZoneServer.DisconnectTime = Server.MilliSecs();

                                        // Bye
                                        Server.IsRunning = false;
                                        Log.WriteLine("Shutdown by MasterServer request.");
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_ProxyConnected:
                            {
                                System.Net.IPAddress Address = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                ushort Port = M.MessageData.ReadUInt16();
                                string HostName = M.MessageData.ReadString();

                                ProxyServer Pr = ProxyServer.FromAddress(Address, Port);
                                if (Pr != null)
                                {
                                    Pr.ServerConnection = M.FromID;

                                    Log.WriteLine("ProxyServer Reconnected (" + Address.ToString() + ":" + Port + "/" + HostName + ")", ConsoleColor.Yellow);
                                }
                                else
                                {
                                    Pr = new ProxyServer();
                                    Pr.ServerConnection = M.FromID;
                                    Pr.InternalAddress = Address;
                                    Pr.InternalHostPort = Port;
                                    Pr.MachineHostName = HostName;
                                    ProxyServer.ProxyServers.AddLast(Pr);

                                    Log.WriteLine("ProxyServer Connected (" + Address.ToString() + ":" + Port + "/" + HostName + ")", ConsoleColor.Green);
                                }

                                break;
                            }
                        case MessageTypes.P_ActorInfoRequest:
                            {
                                byte MsgType = M.MessageData.ReadByte();

                                if (MsgType == (byte)'F') // Find request
                                {
                                    uint AllocID = M.MessageData.ReadUInt32();
                                    string ActorName = M.MessageData.ReadString(M.MessageData.ReadByte());

                                    ActorInstance AI = ActorInstance.FindFromName(ActorName);

                                    // Found the actor in question!
                                    if (AI != null)
                                    {
                                        ActorInfo Info = ActorInfo.FromHandle(AI);
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'Z'); // ZoneServer
                                        Pa.Write((byte)'Y'); // Yes
                                        Pa.Write(AllocID);

                                        // To be online, it must be connected from somewhere!
                                        ProxyServer Pr = ProxyServer.FromConnection(AI.RNID);
                                        if (Pr != null)
                                        {
                                            Pa.Write(Pr.InternalAddress.GetAddressBytes(), 0, 4);
                                            Pa.Write((ushort)Pr.InternalHostPort);

                                            Info.Serialize(Pa);

                                            RCEnet.Send(M.FromID, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);
                                        }
                                    }
                                    else // Not found
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write((byte)'Z'); // ZoneServer
                                        Pa.Write((byte)'N'); // No
                                        Pa.Write(AllocID);

                                        RCEnet.Send(M.FromID, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);
                                    }
                                }
                                // Result
                                else if (MsgType == (byte)'R')
                                {
                                    uint AllocID = M.MessageData.ReadUInt32();
                                    Scripting.ActorInfoState State = (Scripting.ActorInfoState)M.MessageData.ReadByte();

                                    ActorInfoRequest Request = ActorInfo.RequestFromAllocID(AllocID);
                                    if (Request != null)
                                    {
                                        // Handle online and offline
                                        if (State == ActorInfoState.Online || State == ActorInfoState.Offline)
                                        {
                                            // Online! Read current server info
                                            System.Net.IPAddress Addr = new System.Net.IPAddress(M.MessageData.ReadBytes(4));
                                            ushort Port = M.MessageData.ReadUInt16();

                                            // Setup ActorInfo instance
                                            ushort Len = M.MessageData.ReadUInt16();
                                            ActorInfo Info = ActorInfo.FromPacket(M.MessageData);

                                            // Find/set server
                                            ProxyServer Pr = ProxyServer.FromAddress(Addr, Port);
                                            Info.proxyServer = Pr;
                                            if (Pr != null)
                                                Info.ServerConnection = Pr.ServerConnection;

                                            // Invoke callback
                                            Request.Invoke(Info);
                                            ActorInfo.Requests.Remove(Request);
                                        }
                                        else
                                        {
                                            // Handle timeout or 'not found'
                                            ActorInfo Info = ActorInfo.FromTimeout(Request.ActorName, State);
                                            Request.Invoke(Info);
                                            ActorInfo.Requests.Remove(Request);
                                        }
                                    }
                                }

                                break;
                            }
                        case MessageTypes.P_ActorInfoCommand:
                            {
                                uint GCLID = M.MessageData.ReadUInt32();
                                byte Cmd = M.MessageData.ReadByte();

                                ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                if (AI != null)
                                {
                                    // Output Message
                                    if (Cmd == (byte)'O')
                                    {
                                        byte TabID = M.MessageData.ReadByte();
                                        byte R = M.MessageData.ReadByte();
                                        byte G = M.MessageData.ReadByte();
                                        byte B = M.MessageData.ReadByte();
                                        string Msg = M.MessageData.ReadString(M.MessageData.ReadByte());

                                        AI.Output(TabID, Msg, System.Drawing.Color.FromArgb(R, G, B));
                                    }
                                    // Warp
                                    else if (Cmd == (byte)'W')
                                    {
                                        string ZoneName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                        string PortalName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                        int InstanceID = M.MessageData.ReadInt32();

                                        AI.Warp(ZoneName, PortalName, InstanceID);
                                    }
                                    // Kill
                                    else if (Cmd == (byte)'K')
                                    {
                                        AI.Kill();
                                    }
                                    // Script
                                    else if (Cmd == (byte)'S')
                                    {
                                        string ScriptName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                        string MethodName = M.MessageData.ReadString(M.MessageData.ReadByte());
                                        byte[] UserData = M.MessageData.ReadBytes(M.MessageData.ReadUInt16());

                                        ScriptManager.DelayedExecute(ScriptName, MethodName, new object[] { AI, UserData }, AI, null);
                                    }
                                }

                                break;
                            }
                        case RCEnet.PlayerTimedOut:
                        case RCEnet.PlayerHasLeft:
                        case RCEnet.PlayerKicked:
                            {
                                M.FromID = M.MessageData.ReadInt32();

                                // Lost the master server, this is a catastropic failure!
                                if (M.FromID == Server.MasterConnection)
                                {
                                    Log.WriteLine("Warning: MasterServer lost... reconnecting...", ConsoleColor.Yellow);
                                    Server.MasterConnection = 0;
                                    break;
                                }

                                // Proxy server disconnected
                                ProxyServer Pr = ProxyServer.FromConnection(M.FromID);
                                if (M.FromID != 0 && Pr != null)
                                {
                                    // Important because active ActorInfos will need to have their commands fail
                                    Pr.ServerConnection = 0;

                                    // Note: Do not remove the proxy server, a proper cluster will only see this event happen during major network
                                    // issues or maintenance updates (when the server is specifically pulled offline).
                                    Log.WriteLine("ProxyServer Disconnected (" + Pr.InternalAddress + ":" + Pr.InternalHostPort + "/" + Pr.MachineHostName + ")", ConsoleColor.Red);
                                }


                                break;
                            }
                        case MessageTypes.P_ClientDrop:
                            {
                                byte DropType = M.MessageData.ReadByte();

                                if (DropType == (byte)'T')
                                {
                                    uint GCLID = M.MessageData.ReadUInt32();

                                    ActorInstance AI = ActorInstance.FromGCLID(GCLID);
                                    if (AI != null)
                                    {
                                        PacketWriter SaveStream = new PacketWriter();
                                        SaveStream.Write((byte)'S');
                                        SaveStream.Write(GCLID);

                                        byte[] SerializedAI = AI.Serialize().ToArray();

                                        SaveStream.Write(SerializedAI.Length);
                                        SaveStream.Write(SerializedAI, 0);

                                        PacketWriter ScriptPersistance = new PacketWriter();
                                        AI.Logout(ScriptPersistance);

                                        byte[] SaveBytes = ScriptPersistance.ToArray();
                                        SaveStream.Write(SaveBytes.Length);
                                        SaveStream.Write(SaveBytes, 0);

                                        List<Scripting.Forms.Control> DeleteControls = new List<Scripting.Forms.Control>();
                                        DeleteControls.AddRange(AI.ScriptedForms);

                                        foreach (Scripting.Forms.Control C in DeleteControls)
                                            AI.CloseDialog(C, false);

                                        RCEnet.Send(M.FromID, MessageTypes.P_ClientDrop, SaveStream.ToArray(), true);
                                    }
                                    else
                                    {
                                        PacketWriter ErrStream = new PacketWriter();
                                        ErrStream.Write((byte)'N');
                                        ErrStream.Write(GCLID);

                                        RCEnet.Send(M.FromID, MessageTypes.P_ClientDrop, ErrStream.ToArray(), true);
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
