using System;
using System.Collections.Generic;
using System.Text;
using Scripting;
using Environment = System.Environment;

namespace RCPServer
{
    public partial class ActorInstance : Scripting.Actor
    {
        #region Members
        public uint GCLID = 0;
        public Account Account = null;
        public string accountName = "";
        public string accountEmail = "";
        public bool accountBanned = false;
        public bool accountDM = false;
        public int accountIndex = 0;

        public Actor Actor = null;

        
        // Removed - We're using maintained sector lists now!
        //public ActorInstance nextInZone = null; //Linked list containing all actors in zone

        public Area.InstanceSector CurrentSector;
        public Area.InstanceSector SpawnedSector;
        public int SourceSP = 0;
        
        public ActorInstance leader = null; // For slaves, pets, etc.
        public ActorInstance AITarget = null; // AI stuff
        public ActorInstance rider = null;
        public ActorInstance mount = null; // Mount riding
        public Attributes Attributes = new Attributes();     // Replaces Actor\Attributes which is merely the default actor attributes
        public Inventory Inventory = new Inventory();       // The actor's inventory slots!
        public Area.AreaInstance ServerArea = null;
        public Party AcceptPending = null; // Holds the handle of a Party object (or 0 if the actor is not in a party)
        public Party PartyID = null;

        public LinkedList<ActorEffect> ActorEffects = new LinkedList<ActorEffect>();
        public LinkedList<RCPServer.ActorDialog> Dialogs = new LinkedList<RCPServer.ActorDialog>();
        public LinkedList<RCPServer.ProgressBar> ProgressBars = new LinkedList<RCPServer.ProgressBar>();
        public LinkedList<RCPServer.WaitTimeRequest> WaitTimeRequests = new LinkedList<RCPServer.WaitTimeRequest>();
        public LinkedList<RCPServer.WaitSpeakRequest> WaitSpeakRequests = new LinkedList<RCPServer.WaitSpeakRequest>();
        public LinkedList<RCPServer.WaitKillRequest> WaitKillRequests = new LinkedList<RCPServer.WaitKillRequest>();
        public LinkedList<RCPServer.WaitItemRequest> WaitItemRequests = new LinkedList<RCPServer.WaitItemRequest>();
        public LinkedList<Scripting.Forms.Control> ScriptedForms = new LinkedList<Scripting.Forms.Control>();
        public LinkedList<Scripting.Forms.Control> ClientControls = new LinkedList<Scripting.Forms.Control>();
        public Dictionary<string, Scripting.Math.ShaderParameter> ShaderParameters = new Dictionary<string, Scripting.Math.ShaderParameter>();
        public List<RCPServer.Area.Trigger> InTriggers = new List<RCPServer.Area.Trigger>();
        public int FormAllocIDs = 1; // Used as a client->Server allocID. Its per-actor to save re-definition on server change.
        public PacketWriter PropertyPacket = new PacketWriter();

        public int LastPortal = 0;
        public uint LastPortalTime = 0;
        public int TeamID = 0; // Used to allow scripting to put people together in teams
        public int level = 0;
        public int xp = 0;
        public int XPBarLevel = 0;
        public int homeFaction = 0;               // Faction this actor belongs to (0-100 with 0 meaning no faction)
        public int NumberOfSlaves = 0;            // Whether this actor owns any slaves (to speed up saving actor instances)
        public int reputation = 0;
        public int Gold = 0;
        public int RNID = 0;                      // RottNet ID (-1 for AI actors, 0 for not-in-game)
        public int RuntimeID = -1;                 // Assigned by server
        
        public int CurrentWaypoint = 0;
        public AIModes AIMode = 0;
        public bool IsRunning = false;
        public uint LastAttack = 0;
        public int[] FactionRatings = new int[100];        // Individual ratings with each faction for this actor - start off as home faction defaults
        public int[] Resistances = new int[20];           // Resistances against damage types
        public uint[] KnownSpells = new uint[1000];
        public int[] SpellLevels = new int[1000];
        public int[] MemorisedSpells = new int[10];
        public LinkedList<MemorizingSpell> MemorizingSpells = new LinkedList<MemorizingSpell>();
        public int[] SpellCharge = new int[1000]; // How long until the spell is usable
        public int Underwater = 0;
        public uint AIPatrolPause = 0;

        public Scripting.Math.SectorVector Position = new Scripting.Math.SectorVector();
        public Scripting.Math.SectorVector PreviousPosition = new Scripting.Math.SectorVector();
        public Scripting.Math.SectorVector Destination = new Scripting.Math.SectorVector();
        public int DefaultDamageType { get { return Actor.DefaultDamageType; } }
        public float Yaw = 0.0f;

        public ushort FaceTex = 0;
        public ushort hair = 0;
        public ushort beard = 0;
        public ushort BodyTex = 0; // Fixed throughout a character's life unless altered by scripting

        public string Area = "";
        public string name = "", tag = "";
        public string Script = "";                   // Script which executes when character is selected (for traders mainly)
        public string DeathScript = "";              // Script which executes when actor is killed (NPCs only)
        public Dictionary<string, byte[]> ScriptGlobals = new Dictionary<string, byte[]>();

        public byte gender = 0; // 0 for male, 1 for female

        public bool WalkingBackward = false;
        public bool StrafingLeft = false;
        public bool StrafingRight = false;
        public bool IgnoreUpdate = false;
        public QuestLog QuestLog = new QuestLog();
        public ActionBarData ActionBar = new ActionBarData();
        #endregion

        #region Methods
        public ActorInstance()
        {
            for(int i = 0; i < 10; ++i)
		    {
			    MemorisedSpells[i] = 0;
		    }

		    for(int i = 0; i < 20; ++i)
			    Resistances[i] = 0;

		    for(int i = 0; i < 100; ++i)
			    FactionRatings[i] = 0;
    		
		    for(int i = 0; i < 1000; ++i)
		    {
			    KnownSpells[i] = 0;
			    SpellLevels[i] = 0;
			    SpellCharge[i] = 0;
		    }

            ActorInstanceList.AddLast(this);
        }

        public PacketWriter Serialize()
        {
            PacketWriter Pa = new PacketWriter();

            Pa.Write(GCLID);

            Pa.Write((byte)name.Length);
            Pa.Write(name, false);
            Pa.Write((byte)tag.Length);
            Pa.Write(tag, false);

            Pa.Write(FormAllocIDs);

            Pa.Write((byte)accountName.Length);
            Pa.Write(accountName, false);
            Pa.Write((byte)accountEmail.Length);
            Pa.Write(accountEmail, false);

            byte AccountFlags = 0;
            AccountFlags += (byte)(accountBanned ? 1 : 0);
            AccountFlags += (byte)(accountDM ? 2 : 0);
            Pa.Write(AccountFlags);
            Pa.Write(accountIndex);

            Pa.Write((byte)Area.Length);
            Pa.Write(Area, false);

            if (this.Actor != null)
                Pa.Write((ushort)this.Actor.ID);
            else
                Pa.Write((ushort)65535);

            Attributes.Serialize(Pa);
            Inventory.Serialize(Pa);

            if (AcceptPending != null)
            {
                AcceptPending.Serialize(Pa);
            }
            else
            {
                Pa.Write((byte)0);
            }

            if (PartyID != null)
            {
                PartyID.Serialize(Pa);
            }
            else
            {
                Pa.Write((byte)0);
            }

            ushort AECount = 0;
            foreach (ActorEffect AE in ActorEffects)
            {
                if (AE != null)
                    ++AECount;
            }

            Pa.Write(AECount);
            foreach (ActorEffect AE in ActorEffects)
            {
                AE.Serialize(Pa);
            }

            Pa.Write(TeamID);
            Pa.Write(level);
            Pa.Write(xp);
            Pa.Write(XPBarLevel);
            Pa.Write(homeFaction);
            Pa.Write(reputation);
            Pa.Write(Gold);

            // Verify lengths of the following loops
            // These checks are necessary because we could run out of bounds on the deserialize
            if (FactionRatings.Length > 255)
                throw new Exception("FactionRatings.Length uses incorrect type!");
            if (Resistances.Length > 255)
                throw new Exception("Resistances.Length uses incorrect type!");
            if (MemorisedSpells.Length > 255)
                throw new Exception("MemorisedSpells.Length uses incorrect type!");
            if (KnownSpells.Length > 65535)
                throw new Exception("KnownSpells.Length uses incorrect type!");
            if (SpellLevels.Length > 65535)
                throw new Exception("SpellLevels.Length uses incorrect type!");

            Pa.Write((byte)FactionRatings.Length);
            for (int i = 0; i < FactionRatings.Length; ++i)
                Pa.Write(FactionRatings[i]);

            Pa.Write((byte)Resistances.Length);
            for (int i = 0; i < Resistances.Length; ++i)
                Pa.Write(Resistances[i]);

            Pa.Write((ushort)KnownSpells.Length);
            for (int i = 0; i < KnownSpells.Length; ++i)
                Pa.Write(KnownSpells[i]);

            Pa.Write((ushort)SpellLevels.Length);
            for (int i = 0; i < SpellLevels.Length; ++i)
                Pa.Write(SpellLevels[i]);

            Pa.Write((byte)MemorisedSpells.Length);
            for (int i = 0; i < MemorisedSpells.Length; ++i)
                Pa.Write(MemorisedSpells[i]);

            ushort MsCount = 0;
            foreach (MemorizingSpell Ms in MemorizingSpells)
            {
                if (Ms != null)
                    ++MsCount;
            }


            Pa.Write(MsCount);
            foreach (MemorizingSpell Ms in MemorizingSpells)
            {
                Ms.Serialize(Pa);
            }

            Pa.Write(Position);
            Pa.Write(FaceTex);
            Pa.Write(hair);
            Pa.Write(beard);
            Pa.Write(BodyTex);

            Pa.Write(gender);

            QuestLog.Serialize(Pa);
            ActionBar.Serialize(Pa);

            Pa.Write((ushort)ScriptGlobals.Count);
            foreach (KeyValuePair<string, byte[]> Kvp in ScriptGlobals)
            {
                Pa.Write((byte)Kvp.Key.Length);
                Pa.Write(Kvp.Key, false);

                Pa.Write((ushort)Kvp.Value.Length);
                Pa.Write(Kvp.Value, 0);
            }

            Pa.Write((ushort)ProgressBars.Count);
            foreach (ProgressBar Pr in ProgressBars)
            {
                Pr.Serialize(Pa);
            }

            // Count wait timers (since not all can be serialized)
            ushort Count = 0;
            foreach (WaitTimeRequest Wr in WaitTimeRequests)
            {
                if (Wr.CallScript)
                    ++Count;
            }

            Pa.Write((ushort)Count);
            foreach (WaitTimeRequest Wr in WaitTimeRequests)
            {
                if (Wr.CallScript)
                {
                    Wr.Serialize(Pa);
                }
            }

            // Count wait speak (since not all can be serialized)
            Count = 0;
            foreach (WaitSpeakRequest Ws in WaitSpeakRequests)
            {
                if (Ws.CallScript)
                    ++Count;
            }

            Pa.Write((ushort)Count);
            foreach (WaitSpeakRequest Ws in WaitSpeakRequests)
            {
                if (Ws.CallScript)
                {
                    Ws.Serialize(Pa);
                }
            }

            // Count wait kill (since not all can be serialized)
            Count = 0;
            foreach (WaitKillRequest Wk in WaitKillRequests)
            {
                if (Wk.CallScript)
                    ++Count;
            }

            Pa.Write((ushort)Count);
            foreach (WaitKillRequest Wk in WaitKillRequests)
            {
                if (Wk.CallScript)
                {
                    Wk.Serialize(Pa);
                }
            }

            // Count wait item (since not all can be serialized)
            Count = 0;
            foreach (WaitItemRequest Wi in WaitItemRequests)
            {
                if (Wi.CallScript)
                    ++Count;
            }

            Pa.Write((ushort)Count);
            foreach (WaitItemRequest Wi in WaitItemRequests)
            {
                if (Wi.CallScript)
                {
                    Wi.Serialize(Pa);
                }
            }

            Pa.Write((ushort)ShaderParameters.Count);
            foreach (KeyValuePair<string, Scripting.Math.ShaderParameter> sKvp in ShaderParameters)
            {
                Pa.Write((byte)sKvp.Key.Length);
                Pa.Write(sKvp.Key, false);
                Pa.Write((byte)sKvp.Value.GetParameterType());
                Scripting.Math.Vector4 V4 = sKvp.Value.GetParameterVector4();
                Pa.Write(V4.X);
                Pa.Write(V4.Y);
                Pa.Write(V4.Z);
                Pa.Write(V4.W);
            }
                
            // For future use (6)
            Pa.Write((ushort)0);
            Pa.Write((ushort)0);
            Pa.Write((ushort)0);
            Pa.Write((ushort)0);
            Pa.Write((ushort)0);
            Pa.Write((ushort)0);

            return Pa;
        }

        public void Deserialize(PacketReader Pa, PacketWriter ReAllocator)
        {
            GCLID = Pa.ReadUInt32();
            name = Pa.ReadString(Pa.ReadByte());
            tag = Pa.ReadString(Pa.ReadByte());

            FormAllocIDs = Pa.ReadInt32();

            accountName = Pa.ReadString(Pa.ReadByte());
            accountEmail = Pa.ReadString(Pa.ReadByte());

            byte AccountFlags = Pa.ReadByte();
            accountBanned = (AccountFlags & 1) > 0;
            accountDM = (AccountFlags & 2) > 0;
            accountIndex = Pa.ReadInt32();

            Area = Pa.ReadString(Pa.ReadByte());

            ushort AcID = Pa.ReadUInt16();
            if (AcID != 65535)
            {
                this.Actor = RCPServer.Actor.Actors[AcID];
            }

            Attributes.Deserialize(Pa);
            Inventory.Deserialize(Pa, ReAllocator);

            // If first byte is 0, return null
            AcceptPending = Party.Deserialize(Pa);
            PartyID = Party.Deserialize(Pa);

            ushort AECount = Pa.ReadUInt16();
            for (ushort i = 0; i < AECount; ++i)
            {
                ActorEffect AE = ActorEffect.Deserialize(Pa, ReAllocator);
                if (AE != null)
                    ActorEffects.AddLast(AE);
            }

            TeamID = Pa.ReadInt32();
            level = Pa.ReadInt32();
            xp = Pa.ReadInt32();
            XPBarLevel = Pa.ReadInt32();
            homeFaction = Pa.ReadInt32();
            reputation = Pa.ReadInt32();
            Gold = Pa.ReadInt32();

            FactionRatings = new int[Pa.ReadByte()];
            for (int i = 0; i < FactionRatings.Length; ++i)
                FactionRatings[i] = Pa.ReadInt32();

            Resistances = new int[Pa.ReadByte()];
            for (int i = 0; i < Resistances.Length; ++i)
                Resistances[i] = Pa.ReadInt32();

            KnownSpells = new uint[Pa.ReadUInt16()];
            for (int i = 0; i < KnownSpells.Length; ++i)
                KnownSpells[i] = Pa.ReadUInt32();

            SpellLevels = new int[Pa.ReadUInt16()];
            for (int i = 0; i < SpellLevels.Length; ++i)
                SpellLevels[i] = Pa.ReadInt32();

            MemorisedSpells = new int[Pa.ReadByte()];
            for (int i = 0; i < MemorisedSpells.Length; ++i)
                MemorisedSpells[i] = Pa.ReadInt32();

            ushort MsCount = Pa.ReadUInt16();
            for (ushort i = 0; i < MsCount; ++i)
            {
                MemorizingSpell Ms = MemorizingSpell.Deserialize(Pa);
                MemorizingSpells.AddLast(Ms);
            }

            Position = Pa.ReadSectorVector();
            FaceTex = Pa.ReadUInt16();
            hair = Pa.ReadUInt16();
            beard = Pa.ReadUInt16();
            BodyTex = Pa.ReadUInt16();

            gender = Pa.ReadByte();

            QuestLog.Deserialize(Pa);
            ActionBar.Deserialize(Pa);

            ushort Count = Pa.ReadUInt16();
            for(ushort i = 0; i < Count; ++i)
            {
                string KeyName = Pa.ReadString(Pa.ReadByte());
                byte[] KeyValue = Pa.ReadBytes(Pa.ReadUInt16());

                ScriptGlobals.Add(KeyName, KeyValue);
            }


            Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                ProgressBar Pr = ProgressBar.Deserialize(this, Pa, ReAllocator);
                if (Pr != null)
                    ProgressBars.AddLast(Pr);
            }

            Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                WaitTimeRequest Wr = WaitTimeRequest.Deserialize(Pa);
                if (Wr != null)
                    WaitTimeRequests.AddLast(Wr);
            }

            Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                WaitSpeakRequest Ws = WaitSpeakRequest.Deserialize(Pa);
                if (Ws != null)
                    WaitSpeakRequests.AddLast(Ws);
            }

            Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                WaitKillRequest Wk = WaitKillRequest.Deserialize(Pa);
                if (Wk != null)
                    WaitKillRequests.AddLast(Wk);
            }

            Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                WaitItemRequest Wi = WaitItemRequest.Deserialize(Pa);
                if (Wi != null)
                    WaitItemRequests.AddLast(Wi);
            }

            Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                string Key = Pa.ReadString(Pa.ReadByte());
                Scripting.Math.ShaderParameterType PType = (Scripting.Math.ShaderParameterType)Pa.ReadByte();
                float X = Pa.ReadSingle();
                float Y = Pa.ReadSingle();
                float Z = Pa.ReadSingle();
                float W = Pa.ReadSingle();

                ShaderParameters.Add(Key, new Scripting.Math.ShaderParameter(PType, X, Y, Z, W));
            }
            
            // For future use (6)
            Count = Pa.ReadUInt16();
            Count = Pa.ReadUInt16();
            Count = Pa.ReadUInt16();
            Count = Pa.ReadUInt16();
            Count = Pa.ReadUInt16();
            Count = Pa.ReadUInt16();
        }

        public void SetArea(Area ar, int instance)
        {
            SetArea(ar, instance, -1, 0, new Scripting.Math.SectorVector());
        }

        public void SetArea(Area ar, int instance, int waypoint)
        {
            SetArea(ar, instance, waypoint, 0, new Scripting.Math.SectorVector());
        }

        public void SetArea(Area ar, int instance, int waypoint, int portal)
        {
            SetArea(ar, instance, waypoint, portal, new Scripting.Math.SectorVector());
        }

        public void SetArea(Area ar, int instance, int waypoint, int portal, Scripting.Math.SectorVector position)
        {
            // Set flag to ignore standard update until the client has notified us that is has completed the move
            // TODO
            IgnoreUpdate = true;

            // Get old zone
            Area.AreaInstance OldAr = ServerArea;
            Scripting.Math.SectorVector OldArPosition = Position;

            // Warp mount first
            if (mount != null)
                mount.SetArea(ar, instance, waypoint, portal);

            // Warp pets
            if (OldAr != null)
            {
                int Slaves = NumberOfSlaves;

                foreach(Area.InstanceSector Sector in OldAr.Sectors)
                {
                    LinkedListNode<ActorInstance> AINode = Sector.Actors.First;

                    while(AINode != null)
                    {
                        ActorInstance Slave = AINode.Value;

                        // Go to next before SetArea
                        AINode = AINode.Next;

                        if(Slave != null && Slave.Leader == this)
                        {
                            Slave.SetArea(ar, instance, waypoint, portal);
                            --Slaves;
                        }
                    }

                    if (Slaves <= 0)
                        break;
                }
            }

            // Set new position
            if (waypoint == -1 || waypoint >= ar.Waypoints.Length)
            {
                // Portal
                if (portal > -1 && portal < ar.Portals.Length)
                {
                    Yaw = ar.Portals[portal].Yaw;
                    Position = ar.Portals[portal].Position;
                    LastPortal = portal;
                    LastPortalTime = Server.MilliSecs();
                }
                else // Direct Position
                {
                    Position = position;
                }
            }
            else // Waypoint
            {
                Yaw = 0.0f;
                Position = ar.Waypoints[waypoint].Position;
                CurrentWaypoint = waypoint;
                LastPortal = 0;
            }

            // Reset target
            AITarget = null;
            
            // Set new position
            Destination = Position;
            PreviousPosition = Position;

            // If old and new zones are different
            // Added '|| ServerArea == null' to compare occasional non update serverArea when logging in.
            if ((ar.Instances.ContainsKey(instance) && ar.Instances[instance] != OldAr) || ServerArea == null)
            {
                // Remove actor from old zone
                if (OldAr != null)
                {
                    if (CurrentSector != null)
                    {
                        CurrentSector.Players.Remove(this);
                        CurrentSector.Actors.Remove(this);
                    }
                }

                // Put actor into new zone
                ServerArea = ar.Instances[instance];
                if (ar.Instances[instance].Active)
                {
                    int SectorX = Position.SectorX;
                    int SectorZ = Position.SectorZ;

                    if (SectorX >= ar.Instances[instance].Sectors.GetLength(0))
                        SectorX = ar.Instances[instance].Sectors.GetLength(0) - 1;
                    if (SectorZ >= ar.Instances[instance].Sectors.GetLength(1))
                        SectorZ = ar.Instances[instance].Sectors.GetLength(1) - 1;

                    CurrentSector = ar.Instances[instance].Sectors[SectorX, SectorZ];

                    if (RNID > 0)
                        ar.Instances[instance].Sectors[SectorX, SectorZ].Players.AddLast(this);
                    else
                        ar.Instances[instance].Sectors[SectorX, SectorZ].Actors.AddLast(this);

                }
                Area = ar.Name;
            }

            // Actor is human
            if (RNID > 0)
            {
                if (!ar.Instances.ContainsKey(instance) || ar.Instances[instance] == null || !ar.Instances[instance].Active)
                {
                    // Close Dialogs
                    LinkedListNode<ActorDialog> AdNode = Dialogs.First;
                    while(AdNode != null)
                    {
                        ActorDialog Ad = AdNode.Value;
                        AdNode = AdNode.Next;

                        Ad.Close();
                    }

                    // Run exit script
                    if (OldAr != null)
                        if (OldAr.Area.Script.Length > 0)
                            ScriptManager.Execute(OldAr.Area.Script, "OnExit", this, this, null);

                    byte[] SerializedActor = Serialize().ToArray();

                    PacketWriter CARequest = new PacketWriter();

                    // Main Header
                    CARequest.Write((byte)'C');
                    CARequest.Write(GCLID);
                    CARequest.Write((byte)ar.Name.Length);
                    CARequest.Write(ar.Name, false);
                    CARequest.Write((ushort)instance); // Instance
                    CARequest.Write(portal); // Portal ID

                    // Actor
                    CARequest.Write(SerializedActor.Length);
                    CARequest.Write(SerializedActor, 0);

                    // Pause any persistent scripts and stop others
                    PacketWriter Scripts = new PacketWriter();
                    ScriptBase.SavePersistentInstances(ref Scripts, this, true);

                    // Scripts
                    CARequest.Write(Scripts.Length);
                    CARequest.Write(Scripts.ToArray(), 0);

                    // Forms
                    Scripting.Forms.Control[] TempScriptedForms = new Scripting.Forms.Control[ScriptedForms.Count];
                    ScriptedForms.CopyTo(TempScriptedForms, 0);

                    foreach (Scripting.Forms.Control C in TempScriptedForms)
                    {
                        C.Closing(true);
                    }

                    CARequest.Write((ushort)ScriptedForms.Count);
                    

                    foreach (Scripting.Forms.Control C in ScriptedForms)
                    {
                        CARequest.Write(C.GetType().Name, true);

                        PacketWriter SerializedForm = new PacketWriter();
                        C.Serialize(SerializedForm);

                        CARequest.Write(SerializedForm.Length);
                        CARequest.Write(SerializedForm.ToArray(), 0);

                        // Remove the major instance
                        C.ClientInstanceRemoved(ClientControls);
                    }



                    // ReDirect
                    Log.WriteLine(Name + ": Directed to zone '" + ar.Name + "'");
                    RCEnet.Send(RNID, MessageTypes.P_ChangeAreaRequest, CARequest.ToArray(), true);

                    // Unload actor data
                    ActorInstance.Delete(this);
                    if (RuntimeID > -1)
                        if (ActorInstance.RuntimeIDList[RuntimeID] == this)
                            ActorInstance.RuntimeIDList[RuntimeID] = null;

                }
                else
                {
                    // Run entry/exist scripts
                    if (ar.Script.Length > 0)
                        ScriptManager.DelayedExecute(ar.Script, "OnEnter", new object[] { this, (Scripting.ZoneInstance)ar.Instances[instance] }, this, null);

                    if (OldAr != null)
                        if (OldAr.Area.Script.Length > 0)
                            ScriptManager.DelayedExecute(OldAr.Area.Script, "OnExit", this, this, null);

                    // Tell him he's changed zone
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(GCLID);
                    Pa.Write(Position);
                    Pa.Write(Yaw);
                    Pa.Write((byte)(ar.PvP ? 1 : 0));
                    Pa.Write((ushort)(ar.Gravity));
                    Pa.Write(ServerArea.AllocID);
                    Pa.Write((byte)0);//(ar.Instances[instance].CurrentWeather));
                    Pa.Write((byte)(Area.Length));
                    Pa.Write(Area, false);
                    RCEnet.Send(RNID, MessageTypes.P_ChangeArea, Pa.ToArray(), true);

                    // Tell him about any dropped items in this zone
//                     foreach (DroppedItem D in DroppedItem.DroppedItemList)
//                     {
//                         if (D.ServerHandle == ServerArea)
//                         {
//                             Pa = new PacketWriter();
//                             Pa.Write(GCLID);
//                             Pa.Write("D", false);
//                             Pa.Write((ushort)(D.Amount));
//                             Pa.Write(D.Position);
//                             Pa.Write(D.AllocID);
//                             Pa.Write(D.Item.ToArray());
//                             RCEnet.Send(RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);
//                         }
//                     }

                    // Send a stats packet to account servers to inform them of the current player location
                    PacketWriter Stats = new PacketWriter();
                    Stats.Write((byte)'Z');
                    Stats.Write(GCLID);
                    Stats.Write(Server.PrivateAddr.GetAddressBytes(), 0);
                    Stats.Write((ushort)Server.PrivatePort);
                    Stats.Write((byte)ar.Name.Length);
                    Stats.Write(ar.Name, false);
                    Stats.Write((ushort)instance);

                    RCEnet.Send(Server.MasterConnection, MessageTypes.P_ClientInfo, Stats.ToArray(), true);
                }
            }

            // If the new area is different to the old
            if (ar.Instances.ContainsKey(instance))
            {
                if (ar.Instances[instance] != OldAr)
                {
                    // If this actor still belongs to a spawnpoint, remove him
                    /* Ben - NOPE - spawn dupe bug be here.
                    if (SourceSP > -1)
                    {
                        if (SpawnedSector != null)
                            --SpawnedSector.Spawned[SourceSP];
                        SourceSP = -1;
                    }
                    */
                    // Tell others about him / tell him about others (in the new zone)
                    // Only do this if we process the new zone
                    if (ar.Instances[instance].Active)
                    {
                        byte[] NewGuyA = this.ToArray();
                        PacketWriter NewGuyP = new PacketWriter();
                        NewGuyP.Write(0);
                        NewGuyP.Write(NewGuyA, 0);
                        byte[] NewGuy = NewGuyP.ToArray();

                        int MinX = (int)Position.SectorX - 1;
                        int MinZ = (int)Position.SectorZ - 1;
                        int MaxX = (int)Position.SectorX + 2;
                        int MaxZ = (int)Position.SectorZ + 2;

                        if (MinX < 0)
                            MinX = 0;
                        if (MinZ < 0)
                            MinZ = 0;
                        if (MaxX > ar.Sectors.GetLength(0))
                            MaxX = ar.Sectors.GetLength(0);
                        if (MaxZ > ar.Sectors.GetLength(1))
                            MaxZ = ar.Sectors.GetLength(1);

                        LinkedListNode<ActorInstance> A2Node;

                        for (int z = MinZ; z < MaxZ; ++z)
                        {
                            for (int x = MinX; x < MaxX; ++x)
                            {
                                A2Node = ar.Instances[instance].Sectors[x, z].Players.First;

                                // Also send dropped items
                                foreach (DroppedItem D in ar.Instances[instance].Sectors[x, z].DroppedItems)
                                {
                                    PacketWriter DPa = new PacketWriter();
                                    DPa.Write(GCLID);
                                    DPa.Write("D", false);
                                    DPa.Write((ushort)(D.Amount));
                                    DPa.Write(D.Position);
                                    DPa.Write(D.AllocID);
                                    DPa.Write(D.Item.ToArray());
                                    RCEnet.Send(RNID, MessageTypes.P_InventoryUpdate, DPa.ToArray(), true);
                                }

                                while (A2Node != null)
                                {
                                    if (A2Node.Value != null && A2Node.Value.RuntimeID > -1 && A2Node.Value != this)
                                    {
                                        // Message to existing player about new player
                                        if (A2Node.Value.RNID > 0)
                                        {
                                            //TODO: Use HostToNetworkOrder
                                            BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(NewGuy, 0);
                                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_NewActor, NewGuy, true);
                                        }

                                        // Message to new player about existing player
                                        if (RNID > 0)
                                        {
                                            PacketWriter A2p = new PacketWriter();
                                            A2p.Write(GCLID);
                                            A2p.Write(A2Node.Value.ToArray(), 0);

                                            RCEnet.Send(RNID, MessageTypes.P_NewActor, A2p.ToArray(), true);
                                        }
                                    }

                                    A2Node = A2Node.Next;
                                }

                                A2Node = ar.Instances[instance].Sectors[x, z].Actors.First;

                                while (A2Node != null)
                                {
                                    if (A2Node.Value != null && A2Node.Value.RuntimeID > -1 && A2Node.Value != this)
                                    {
                                        // Message to existing player about new player
                                        if (A2Node.Value.RNID > 0)
                                        {
                                            //TODO: Use HostToNetworkOrder
                                            BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(NewGuy, 0);
                                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_NewActor, NewGuy, true);
                                        }

                                        // Message to new player about existing player
                                        if (RNID > 0)
                                        {
                                            PacketWriter A2p = new PacketWriter();
                                            A2p.Write(GCLID);
                                            A2p.Write(A2Node.Value.ToArray(), 0);

                                            RCEnet.Send(RNID, MessageTypes.P_NewActor, A2p.ToArray(), true);
                                        }
                                    }

                                    A2Node = A2Node.Next;
                                }
                            }
                        }
                    }

                    // Tell players in his old area that he has now left
                    if (OldAr != null)
                    {
                        PacketWriter Gonep = new PacketWriter();
                        Gonep.Write(0);
                        Gonep.Write((ushort)RuntimeID);
                        byte[] PaRNID = Gonep.ToArray();

                        int MinX = (int)OldArPosition.SectorX - 1;
                        int MinZ = (int)OldArPosition.SectorZ - 1;
                        int MaxX = (int)OldArPosition.SectorX + 2;
                        int MaxZ = (int)OldArPosition.SectorZ + 2;

                        if (MinX < 0)
                            MinX = 0;
                        if (MinZ < 0)
                            MinZ = 0;
                        if (MaxX > OldAr.Sectors.GetLength(0))
                            MaxX = OldAr.Sectors.GetLength(0);
                        if (MaxZ > OldAr.Sectors.GetLength(1))
                            MaxZ = OldAr.Sectors.GetLength(1);

                        LinkedListNode<ActorInstance> A2Node;

                        for (int z = MinZ; z < MaxZ; ++z)
                        {
                            for (int x = MinX; x < MaxX; ++x)
                            {
                                A2Node = OldAr.Sectors[x, z].Players.First;

                                while (A2Node != null)
                                {
                                    if (A2Node.Value != null && A2Node.Value.RNID > 0)
                                    {
                                        //TODO: Use HostToNetworkOrder
                                        BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaRNID, 0);
                                        RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_ActorGone, PaRNID, true);
                                    }

                                    A2Node = A2Node.Next;
                                }
                            }
                        }
                    }
                }
                else // If he's warped to the same area he was already in, tell the players he has changed position
                {
                    // Move us, if necessary
                    if (OldArPosition.SectorX != Position.SectorX || OldArPosition.SectorZ != Position.SectorZ)
                        ar.Instances[instance].ActorChangedSector(this);

                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(0);
                    Pa.Write((byte)'M');
                    Pa.Write((ushort)(RuntimeID));
                    Pa.Write(Position);
                    Pa.Write((byte)(0));
                    byte[] PaA = Pa.ToArray();

                    int MinX = (int)Position.SectorX - 1;
                    int MinZ = (int)Position.SectorZ - 1;
                    int MaxX = (int)Position.SectorX + 2;
                    int MaxZ = (int)Position.SectorZ + 2;

                    if (MinX < 0)
                        MinX = 0;
                    if (MinZ < 0)
                        MinZ = 0;
                    if (MaxX > ar.Sectors.GetLength(0))
                        MaxX = ar.Sectors.GetLength(0);
                    if (MaxZ > ar.Sectors.GetLength(1))
                        MaxZ = ar.Sectors.GetLength(1);

                    LinkedListNode<ActorInstance> A2Node;

                    for (int z = MinZ; z < MaxZ; ++z)
                    {
                        for (int x = MinX; x < MaxX; ++x)
                        {
                            A2Node = ar.Instances[instance].Sectors[x, z].Players.First;

                            while (A2Node != null)
                            {
                                if (A2Node.Value != null && A2Node.Value.RNID > 0)
                                {
                                    //TODO: Use HostToNetworkOrder
                                    BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                    RCEnet.Send(A2Node.Value.RNID, MessageTypes. P_RepositionActor, PaA, true);
                                }

                                A2Node = A2Node.Next;
                            }
                        }
                    }
                }
            }
        }

        public byte[] ToArray()
        {
            PacketWriter Pa = new PacketWriter();

            Pa.Write(ServerArea.AllocID);
            
            Pa.Write((ushort)(RuntimeID));
            Pa.Write((ushort)(Level));
            Pa.Write(XP);
	        Pa.Write((ushort)(Actor.ID));
            Pa.Write(Position);
            Pa.Write(Yaw);
                
            if(RNID == -1)
                Pa.Write((byte)(0));
            else
                Pa.Write((byte)(1));

	        Pa.Write((byte)(Name.Length));
            Pa.Write(Name, false);
	        Pa.Write((byte)(Tag.Length));
            Pa.Write(Tag, false);
	        
            if(Actor.Genders == 0)
                Pa.Write((byte)(gender));
	        
            Pa.Write((ushort)(Reputation));
	        Pa.Write((ushort)(FaceTex));
            Pa.Write((ushort)(Hair));
            Pa.Write((ushort)(BodyTex));
            Pa.Write((ushort)(Beard));
	        Pa.Write((ushort)(Attributes.Value[Server.SpeedStat]));
            Pa.Write((ushort)(Attributes.Maximum[Server.SpeedStat]));
	        Pa.Write((ushort)(Attributes.Value[Server.HealthStat]));
            Pa.Write((ushort)(Attributes.Maximum[Server.HealthStat]));

            if (Inventory.Items[(int)ItemSlots.Weapon] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Weapon].Item.ID));
	        else
		        Pa.Write((ushort)(65535));

            if (Inventory.Items[(int)ItemSlots.Shield] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Shield].Item.ID));
	        else
		        Pa.Write((ushort)(65535));

            if (Inventory.Items[(int)ItemSlots.Hat] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Hat].Item.ID));
	        else
		        Pa.Write((ushort)(65535));

            if (Inventory.Items[(int)ItemSlots.Chest] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Chest].Item.ID));
            else
                Pa.Write((ushort)(65535));
	        
            Pa.Write((byte)(homeFaction));
	       
            for(int i = 0; i < 100; ++i)
		        Pa.Write((byte)(FactionRatings[i]));

            Pa.Write((ushort)ShaderParameters.Count);
            foreach (KeyValuePair<string, Scripting.Math.ShaderParameter> sKvp in ShaderParameters)
            {
                Pa.Write((byte)sKvp.Key.Length);
                Pa.Write(sKvp.Key, false);
                Pa.Write((byte)sKvp.Value.GetParameterType());
                Scripting.Math.Vector4 V4 = sKvp.Value.GetParameterVector4();
                Pa.Write(V4.X);
                Pa.Write(V4.Y);
                Pa.Write(V4.Z);
                Pa.Write(V4.W);
            }

            return Pa.ToArray();
        }

        public void AssignRuntimeID()
        {
            ++LastRuntimeID;
            while (RuntimeIDList[LastRuntimeID] != null)
            {
                ++LastRuntimeID;
                if (LastRuntimeID > 65534)
                    LastRuntimeID = 0;
            }

            RuntimeIDList[LastRuntimeID] = this;
            RuntimeID = LastRuntimeID;

            ++LastRuntimeID;
            if (LastRuntimeID > 65534)
                LastRuntimeID = 0;
        }     
        


        public override void CreateParty()
        {
            if (PartyID != null)
                PartyID.RemoveMember(this);

            PartyID = new Party();
            PartyID.AddMember(this);
            ScriptManager.Execute("Party", "Create", new object[]{this, PartyID}, this, null);
        
        }

        public override void JoinParty(PartyInstance p)
        {
            Party party = p as Party;

            if (PartyID != null)
                PartyID.RemoveMember(this);

            party.AddMember(this);
            ScriptManager.Execute("Party", "Join", new object[]{this, party}, this, null);
        }

        public override void LeaveParty()
        {
            if (PartyID != null)
            {
                PartyID.RemoveMember(this);
                ScriptManager.Execute("Party", "Leave", new object[]{this, PartyID}, this, null);
            }
        }

        // Updates an attribute and tells all the players in zone about it
        public void UpdateAttribute(int att, int value)
        {
            Attributes.Value[att] = value;
            if (Attributes.Value[att] > Attributes.Maximum[att])
                Attributes.Value[att] = Attributes.Maximum[att];

            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write("A", false);
                Pa.Write((ushort)(RuntimeID));
                Pa.Write((byte)(att));
                Pa.Write((ushort)(Attributes.Value[att]));
                byte[] PaA = Pa.ToArray();

                int MinX = (int)CurrentSector.Sector.SectorX - 1;
                int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                LinkedListNode<ActorInstance> A2Node;

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            if (A2Node.Value != null && A2Node.Value.RNID > 0)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_StatUpdate, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        // Updates an attribute maximum and tells all players in zone about it
        public void UpdateAttributeMax(int att, int value)
        {
            Attributes.Maximum[att] = value;

            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write("M", false);
                Pa.Write((ushort)(RuntimeID));
                Pa.Write((byte)(att));
                Pa.Write((ushort)(value));
                byte[] PaA = Pa.ToArray();

                int MinX = (int)CurrentSector.Sector.SectorX - 1;
                int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                LinkedListNode<ActorInstance> A2Node;

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            if (A2Node.Value != null && A2Node.Value.RNID > 0)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_StatUpdate, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        // Gives a known spell (ability) to an actor instance
        public void AddSpell(uint spellID, int level)
        {
            if (level < 1)
                return;

            Spell Sp = Spell.Spells[spellID];

            if (Sp == null)
                return;

            // Find a free slot
            for (int i = 0; i < 1000; ++i)
            {
                if (SpellLevels[i] <= 0)
                {
                    // Add the spell
                    KnownSpells[i] = spellID;
                    SpellLevels[i] = level;

                    // If they are a player in game, tell them
                    if (RNID > 0)
                    {
                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(GCLID);
                        Pa.Write("A", false);
                        Pa.Write((ushort)(level));
                        Pa.Write((ushort)(spellID));
                        Pa.Write((ushort)(Sp.ThumbnailTexID));
                        Pa.Write((ushort)(Sp.RechargeTime));
                        Pa.Write((ushort)(Sp.Name.Length));
                        Pa.Write(Sp.Name, false);
                        Pa.Write((ushort)(Sp.Description.Length));
                        Pa.Write(Sp.Description, false);
                        Pa.Write((byte)(0));
                        RCEnet.Send(RNID, MessageTypes.P_KnownSpellUpdate, Pa.ToArray(), true);
                    }

                    // Done
                    break;
                }
            }
        }

        public void DeleteSpell(int id)
        {
            // Remove
            Spell Sp = Spell.Spells[KnownSpells[id]];
	        KnownSpells[id] = 0;
	        SpellLevels[id] = 0;

            for(int i = 0; i < 10; ++i)
            {
		        if(MemorisedSpells[i] == id)
                {
                    MemorisedSpells[i] = 5000;
                    break;
                }
            }
	        
	        // If they are a player in game, tell them
	        if(RNID > 0 && Sp != null)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write((byte)'D');
                Pa.Write(Sp.Name, false);

                RCEnet.Send(RNID, MessageTypes.P_KnownSpellUpdate, Pa.ToArray(), true);
            }
        }

        // Gives XP points to an actor instance
        public override void GiveXP(int xpAmount, bool share)
        {
            bool ignoreParty = !share;
            // Give the points to the leader if this actor has a leader
            if (Leader != null)
            {
                Leader.GiveXP(xpAmount, !ignoreParty);
                return;
            }

            // Share with other party members in same area, if any
            if (ignoreParty == false)
            {
                Party Party = PartyID;
                if (Party != null)
                {
                    int Members = 0;
                    for (int i = 0; i < 8; ++i)
                        if (Party.Players[i] != null)
                            if (Party.Players[i].ServerArea == ServerArea)
                                ++Members;
                    int PartyXP = XP / Members;

                    for (int i = 0; i < 8; ++i)
                        if (Party.Players[i] != null && Party.Players[i] != this)
                            if (Party.Players[i].ServerArea == ServerArea)
                                Party.Players[i].GiveXP(PartyXP, false);

                    xp = PartyXP + (XP % Party.Members);
                }
            }

            // Add gain to character
            xp += xpAmount;

            // Call script and tell player if it's a human character
            if (RNID > 0)
            {
                ScriptManager.DelayedExecute("LevelUp", "Main", this, this, null);
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write("M", false);
                Pa.Write(xpAmount);
                RCEnet.Send(RNID, MessageTypes.P_XPUpdate, Pa.ToArray(), true);
            }
        }

        // Give instructions to a pet
        public void CommandPet(string command, string param)
        {
            // Wait at current position
            if (command.Equals("wait", StringComparison.CurrentCultureIgnoreCase) || command.Equals("stay", StringComparison.CurrentCultureIgnoreCase))
            {
                AIMode = AIModes.AI_PetWait;
                Destination = Position;
                AITarget = null;
            } // Follow leader
            else if (command.Equals("follow", StringComparison.CurrentCultureIgnoreCase) || command.Equals("come", StringComparison.CurrentCultureIgnoreCase))
            {
                AIMode = AIModes.AI_Pet;
                AITarget = null;
            } // Pet to attack leader's target
            else if (command.Equals("attack", StringComparison.CurrentCultureIgnoreCase))
            {
                if (Actor.Aggressiveness < 3)
                {
                    if (leader.AITarget != null)
                    {
                        AITarget = leader.AITarget;
                        AIMode = AIModes.AI_PetChase;
                    }
                }
            } // Rename pet
            else if (command.Equals("name", StringComparison.CurrentCultureIgnoreCase))
            {
                // Check validity
                if (Server.IsNameValid(param))
                {
                    // Set pet name
                    Name = param;
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(0);
                    Pa.Write((ushort)(RuntimeID));
                    Pa.Write((byte)(Name.Length));
                    Pa.Write(Name, false);
                    Pa.Write(Tag, false);
                    byte[] PaA = Pa.ToArray();


                    int MinX = (int)CurrentSector.Sector.SectorX - 1;
                    int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                    int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                    int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                    if (MinX < 0)
                        MinX = 0;
                    if (MinZ < 0)
                        MinZ = 0;
                    if (MaxX > ServerArea.Sectors.GetLength(0))
                        MaxX = ServerArea.Sectors.GetLength(0);
                    if (MaxZ > ServerArea.Sectors.GetLength(1))
                        MaxZ = ServerArea.Sectors.GetLength(1);

                    LinkedListNode<ActorInstance> A2Node;

                    for (int z = MinZ; z < MaxZ; ++z)
                    {
                        for (int x = MinX; x < MaxX; ++x)
                        {
                            A2Node = ServerArea.Sectors[x, z].Players.First;

                            while (A2Node != null)
                            {
                                if (A2Node.Value != null && A2Node.Value.RNID > 0)
                                {
                                    //TODO: Use HostToNetworkOrder
                                    BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                    RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_NameChange, PaA, true);
                                }

                                A2Node = A2Node.Next;
                            }
                        }
                    }
                }
            }
        }

        public void FreeActorInstanceSlaves()
        {
            if (NumberOfSlaves == 0)
                return;

            foreach (ActorInstance A2 in ActorInstanceList)
            {
                if (A2.Leader == this)
                {
                    A2.FreeActorInstanceSlaves();
                    A2.FreeActorInstance();
                }
            }
        }

        public void FreeActorInstance()
        {
            if (RuntimeID > -1)
                if (RuntimeIDList[RuntimeID] == this)
                    RuntimeIDList[RuntimeID] = null;

            if (leader != null)
                --leader.NumberOfSlaves;

            ActorInstance.Delete(this);
        }

        public void Logout(PacketWriter persistance)
        {
            // Run logout script immediately
            ScriptManager.Execute("Logout", "OnLogout", this, this, null);

            // Reset target
            AITarget = null;

            // Tell other players in the same zone and remove this player from the linked list
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)(RuntimeID));
            byte[] PaA = Pa.ToArray();

            Area.AreaInstance AInstance = ServerArea;
            if (AInstance != null)
            {
                if (CurrentSector != null)
                {
                    CurrentSector.Players.Remove(this);
                    CurrentSector.Actors.Remove(this);
                }

                int MinX = (int)CurrentSector.Sector.SectorX - 1;
                int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                LinkedListNode<ActorInstance> A2Node;

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            if (A2Node.Value != null && A2Node.Value.RNID > 0)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_ActorGone, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
            ServerArea = null;

            // JB: I didn't write this comment
            // Dehorsify
            if (rider != null)
            {
                rider.mount = null;
                rider = null;
            }
            if (mount != null)
            {
                mount.rider = null;
                mount = null;
            }

            // Update timers on active actor effects
            foreach (ActorEffect AE in ActorEffects)
            {
                AE.Length = AE.Length - (Server.MilliSecs() - AE.CreatedTime);
            }

            // Pause any persistent scripts and stop others
            ScriptBase.SavePersistentInstances(ref persistance, this, false);

            // Leave party
            LeaveParty();

            RNID = 0;
            if (RuntimeID > -1)
                if (ActorInstance.RuntimeIDList[RuntimeID] == this)
                    ActorInstance.RuntimeIDList[RuntimeID] = null;
            RuntimeID = -1;

            ActorInstance.Delete(this);
        }

        public void SendEquippedUpdate()
        {
            // Execute script
            ScriptManager.Execute("EquipChange", "Main", this, this, null);

            // Create packet with item IDs
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((byte)('O'));
            Pa.Write((ushort)(RuntimeID));

            if (Inventory.Items[(int)ItemSlots.Weapon] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Weapon].Item.ID));
            else
                Pa.Write((ushort)(65535));

            if (Inventory.Items[(int)ItemSlots.Shield] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Shield].Item.ID));
            else
                Pa.Write((ushort)(65535));

            if (Inventory.Items[(int)ItemSlots.Chest] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Chest].Item.ID));
            else
                Pa.Write((ushort)(65535));

            if (Inventory.Items[(int)ItemSlots.Hat] != null)
                Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Hat].Item.ID));
            else
                Pa.Write((ushort)(65535));

//             // Find out which gubbins to activate
//             bool[] ActivateGubbins = new bool[6];
//             for (int i = 0; i < 6; ++i)
//                 ActivateGubbins[i] = false;
// 
//             for (int i = 0; i < (int)ItemSlots.Backpack; ++i)
//             {
//                 if (Inventory.Items[i] != null)
//                 {
//                     for (int j = 0; j < 6; ++i)
//                         if (Inventory.Items[i].Item.Gubbins[j] > 0)
//                             ActivateGubbins[j] = true;
//                 }
//             }
// 
//             for (int i = 0; i < 6; ++i)
//                 Pa.Write((byte)(ActivateGubbins[i] ? 1 : 0));

            byte[] PaA = Pa.ToArray();

            int MinX = (int)CurrentSector.Sector.SectorX - 1;
            int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
            int MaxX = (int)CurrentSector.Sector.SectorX + 2;
            int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

            if (MinX < 0)
                MinX = 0;
            if (MinZ < 0)
                MinZ = 0;
            if (MaxX > ServerArea.Sectors.GetLength(0))
                MaxX = ServerArea.Sectors.GetLength(0);
            if (MaxZ > ServerArea.Sectors.GetLength(1))
                MaxZ = ServerArea.Sectors.GetLength(1);

            LinkedListNode<ActorInstance> A2Node;

            for (int z = MinZ; z < MaxZ; ++z)
            {
                for (int x = MinX; x < MaxX; ++x)
                {
                    A2Node = ServerArea.Sectors[x, z].Players.First;

                    while (A2Node != null)
                    {
                        if (A2Node.Value != null && A2Node.Value.RNID > 0)
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

        public bool Attack(ActorInstance A2)
        {

            int initialTargetHealth = A2.Attributes.Value[Server.HealthStat];
            if (Attributes.Value[Server.HealthStat] > 0 && initialTargetHealth> 0)
            {

                int MinX = 0;
                int MinZ = 0;
                int MaxX = 0;
                int MaxZ = 0;
                LinkedListNode<ActorInstance> A2Node;

                // Get distance between the actor instances
                Scripting.Math.SectorVector DistanceVec = Position - A2.Position;

                float XDist = Math.Abs((((float)DistanceVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistanceVec.X);
                float ZDist = Math.Abs((((float)DistanceVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistanceVec.Z);
                float YDist = Math.Abs(DistanceVec.Y);
                float Dist = (XDist * XDist) + (ZDist * ZDist);

                if (A2.Human || A2.Actor.Environment != ActorEnvironment.Walk)
                {
                    if (A2.Actor.Environment == ActorEnvironment.Amphibious && A2.UnderWater)
                        Dist = (XDist * XDist) + (ZDist * ZDist) + (YDist * YDist);
                    else if (A2.Actor.Environment != ActorEnvironment.Amphibious)
                        Dist = (XDist * XDist) + (ZDist * ZDist) + (YDist * YDist);
                }

                //int Damage = 0;
                //int DamageType = 0;



                float CheckDist = 0.0f;

                // Check if this is actually a projectile attack
                if (Inventory.Items[(int)ItemSlots.Weapon] != null)
                {
                    if (Inventory.Items[(int)ItemSlots.Weapon].Item.WeaponType == WeaponType.Ranged)
                    {
                        if (Inventory.Items[(int)ItemSlots.Weapon].ItemHealth > 0)
                        {
                            // Fixed function
                            if (Server.CombatFormula != 4)
                            {
                                // In range?
                                CheckDist = Inventory.Items[(int)ItemSlots.Weapon].Item.Range + Actor.Radius + A2.Actor.Radius;
                                if (Dist > CheckDist * CheckDist)
                                    return false;

                                // Tell other players in the same area
                                PacketWriter Pa = new PacketWriter();
                                Pa.Write(0);
                                Pa.Write((byte)('O'));
                                Pa.Write((ushort)(RuntimeID));
                                Pa.Write((ushort)(A2.RuntimeID));
                                byte[] PaA = Pa.ToArray();

                                MinX = (int)CurrentSector.Sector.SectorX - 1;
                                MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                                MaxX = (int)CurrentSector.Sector.SectorX + 2;
                                MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                                if (MinX < 0)
                                    MinX = 0;
                                if (MinZ < 0)
                                    MinZ = 0;
                                if (MaxX > ServerArea.Sectors.GetLength(0))
                                    MaxX = ServerArea.Sectors.GetLength(0);
                                if (MaxZ > ServerArea.Sectors.GetLength(1))
                                    MaxZ = ServerArea.Sectors.GetLength(1);

                                for (int z = MinZ; z < MaxZ; ++z)
                                {
                                    for (int x = MinX; x < MaxX; ++x)
                                    {
                                        A2Node = ServerArea.Sectors[x, z].Players.First;

                                        while (A2Node != null)
                                        {
                                            if (A2Node.Value != null && A2Node.Value.RNID > 0 && A2Node.Value != this && A2Node.Value != A2)
                                            {
                                                //TODO: Use HostToNetworkOrder
                                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AttackActor, PaA, true);
                                            }

                                            A2Node = A2Node.Next;
                                        }
                                    }
                                }

                                // Launch projectile
                                Projectile P = Projectile.FromList(Inventory.Items[(int)ItemSlots.Weapon].Item.RangedProjectile);
                                if (P != null)
                                {
                                    FireProjectile(P, A2);
                                    LastAttack = Server.MilliSecs();
                                }
                            }
                            else // Attack script
                            {
                                // Check both actors are allowed to engage in combat
                                if (Actor.Aggressiveness == 3 || A2.Actor.Aggressiveness == 3)
                                    return false;

                                // Check faction ratings
                                if (FactionRatings[A2.homeFaction] > 150)
                                    return false;

                                // Store time of attack
                                LastAttack = Server.MilliSecs();

                                // Make attacked actor angry if it's defensive
                                if (A2.RNID == -1)
                                {
                                    if (A2.Actor.Aggressiveness == 1)
                                    {
                                        A2.AITarget = this;
                                        A2.AIMode = AIModes.AI_Chase;
                                    }
                                    else if (A2.Actor.Aggressiveness == 2 && A2.AITarget == null) // Or if it's aggressive and has no target...
                                    {
                                        A2.AITarget = this;
                                        A2.AIMode = AIModes.AI_Chase;
                                    }
                                }

                                ScriptManager.Execute("Attack", "Main", new object[] { this, A2 }, this, A2);
                            }

                            return true;
                        }
                        else
                        {
                            if (RNID > 0)
                            {
                                PacketWriter Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write((byte)0);
                                Pa.Write((byte)(253));
                                Pa.Write(Language.Get(LanguageString.WeaponDamaged), false);

                                RCEnet.Send(RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
                            }
                            return false;
                        }
                    }
                }

                // Check both actors are allowed to engage in combat
                if (Actor.Aggressiveness == 3 || A2.Actor.Aggressiveness == 3)
                    return false;

                // Check faction ratings
                if (FactionRatings[A2.homeFaction] > 150)
                    return false;

                // Check distance is acceptable
                CheckDist = 4.0f + Actor.Radius + A2.Actor.Radius;
                if (Dist > CheckDist * CheckDist)
                    return false;

                // Store time of attack
                LastAttack = Server.MilliSecs();

                // Make attacked actor angry if it's defensive
                if (A2.RNID == -1)
                {
                    if (A2.Actor.Aggressiveness == 1)
                    {
                        A2.AITarget = this;
                        A2.AIMode = AIModes.AI_Chase;
                    }
                    else if (A2.Actor.Aggressiveness == 2 && A2.AITarget == null) // Or if it's aggressive and has no target...
                    {
                        A2.AITarget = this;
                        A2.AIMode = AIModes.AI_Chase;
                    }
                }



                // Calculate damage
                // Normal formula
                if (Server.CombatFormula == 1)
                {
                    A2.AICallForHelp();
                    ScriptManager.Execute("DefaultAttack", "NormalFormula", new object[] { this, A2 }, this, A2);

                }
                else if (Server.CombatFormula == 2) // No strength bonus or penalty
                {
                    A2.AICallForHelp();
                    ScriptManager.Execute("DefaultAttack", "NoBonusOrPenaltyFormula", new object[] { this, A2 }, this, A2);

                }
                else if (Server.CombatFormula == 3) // Multiplied formula
                {
                    A2.AICallForHelp();
                    ScriptManager.Execute("DefaultAttack", "MultipliedFormula", new object[] { this, A2 }, this, A2);

                }
                else if (Server.CombatFormula == 4) // Scripted
                {
                    A2.AICallForHelp();
                    ScriptManager.Execute("Attack", "Main", new object[] { this, A2 }, this, A2);
                    //goto SkipAttackNet;
                }

                // Damage weapon
                if (Server.WeaponDamage > 0)
                {
                    if (Inventory.Items[(int)ItemSlots.Weapon] != null)
                    {
                        if (RNID > 0)
                        {
                            PacketWriter Pa = new PacketWriter();
                            Pa.Write(GCLID);
                            Pa.Write((byte)((int)ItemSlots.Weapon));
                            Pa.Write((ushort)(Inventory.Items[(int)ItemSlots.Weapon].ItemHealth));

                            RCEnet.Send(RNID, MessageTypes.P_ItemHealth, Pa.ToArray(), true);
                        }
                    }
                }

                // Damage armour
                if (Server.ArmourDamage > 0)
                {
                    for (int i = (int)ItemSlots.Shield; i <= (int)ItemSlots.Feet; ++i)
                    {
                        if (Inventory.Items[i] != null)
                        {
                            if (RNID > 0)
                            {
                                PacketWriter Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write((byte)(i));
                                Pa.Write((ushort)(Inventory.Items[i].ItemHealth));

                                RCEnet.Send(RNID, MessageTypes.P_ItemHealth, Pa.ToArray(), true);
                            }
                        }
                    }
                }

                // Apply damage to target actor
                // if(Damage > 0)
                //A2.Attributes.Value[Server.HealthStat] = A2.Attributes.Value[Server.HealthStat] - Damage;
            
                // Tell player(s) if applicable
                PacketWriter Pa1 = new PacketWriter();
                PacketWriter Pa2 = new PacketWriter();
                /*
            // Guess the damage done by getting HP before attack script and after and finding the difference. 
            int damage = initialTargetHealth - A2.Attributes.Value[Server.HealthStat];

            int damageType = Actor.DefaultDamageType;
            // Work out damage type by checking what actor is holding
            if (Inventory.Items[(int)ItemSlots.Weapon] != null)
                damage = Inventory.Items[(int)ItemSlots.Weapon].DamageType;


            Pa1.Write(GCLID);
            Pa1.Write((byte)('H'));
            Pa1.Write((ushort)(A2.RuntimeID));
            Pa1.Write((ushort)(initialTargetHealth + 1));
            Pa1.Write((byte)(damageType));

            Pa2.Write(A2.GCLID);
            Pa2.Write((byte)('Y'));
            Pa2.Write((ushort)(RuntimeID));
            Pa2.Write((ushort)(initialTargetHealth + 1));
            Pa2.Write((byte)(damageType));

            if (RNID > 0)
                RCEnet.Send(RNID, MessageTypes.P_AttackActor, Pa1.ToArray(), true);

            if (A2.RNID > 0)
                RCEnet.Send(A2.RNID, MessageTypes.P_AttackActor, Pa2.ToArray(), true);
            */
                // Tell other players in the same area
                Pa1 = new PacketWriter();
                Pa1.Write(0);
                Pa1.Write((byte)('O'));
                Pa1.Write((ushort)(RuntimeID));
                Pa1.Write((ushort)(A2.RuntimeID));
                byte[] Pa1A = Pa1.ToArray();

                MinX = (int)CurrentSector.Sector.SectorX - 1;
                MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                MaxX = (int)CurrentSector.Sector.SectorX + 2;
                MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            if (A2Node.Value != null && A2Node.Value.RNID > 0
                                && A2Node.Value != this && A2Node.Value != A2)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(Pa1A, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AttackActor, Pa1A, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }

                //SkipAttackNet:

                // If target was a player with pets, make pets attack too
                if (RNID > 0)
                {
                    if (NumberOfSlaves > 0)
                    {
                        int Found = 0;

                        foreach (ActorInstance A in ActorInstance.ActorInstanceList)
                        {
                            if (A.Leader == this)
                            {
                                ++Found;
                                if (A.Actor.Aggressiveness < 3 && A.AITarget == null)
                                {
                                    A.AITarget = A2;
                                    A.AIMode = AIModes.AI_PetChase;
                                }

                                if (Found == NumberOfSlaves)
                                    break;
                            }
                        }
                    }
                }

                // Death
                if (A2.Attributes.Value[Server.HealthStat] <= 0)
                {
                    Target = null;
                    A2.KillActor(this);

                    if (!Human)
                    {
                        if (Leader != null)
                        {
                            AIMode = AIModes.AI_Pet;
                        }
                        else
                        {
                            AIMode = AIModes.AI_Wait;
                        }
                    }
                }
          
                return true;
            }
            return false;
        }

        public void KillActor(ActorInstance Killer)
        {
	        // Tell players in the same area if it was an AI actor dying
	        if(RNID < 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((ushort)(RuntimeID));
		        
                if(Killer != null)
                    Pa.Write((ushort)(Killer.RuntimeID));

                byte[] PaA = Pa.ToArray();

                int MinX = (int)CurrentSector.Sector.SectorX - 1;
                int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                LinkedListNode<ActorInstance> A2Node;

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            if (A2Node.Value != null && A2Node.Value.RNID > 0)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_ActorDead, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
	        }



	        // Continue any paused scripts waiting for this event
            LinkedListNode<WaitKillRequest> WkNode = Killer.WaitKillRequests.First;
            while (WkNode != null)
            {
                WaitKillRequest Wk = WkNode.Value;
                LinkedListNode<WaitKillRequest> Del = WkNode;
                WkNode = WkNode.Next;

                if (Wk.ActorID == this.Actor.ID)
                {
                    Wk.CurrentCount++;

                    if (Wk.CurrentCount >= Wk.KillCount)
                    {
                        if (Wk.CallScript)
                        {
                            ScriptManager.DelayedExecute(Wk.CallbackScript, Wk.CallbackMethod, new object[] { Killer, Wk }, Killer, null);
                        }
                        else
                        {
                            Wk.Invoke(Killer);
                        }

                        Killer.WaitKillRequests.Remove(Del);
                    }
                }
            }

	        // Human death
	        if(RNID > 0)
            {
		        // Run script
                ScriptManager.Execute("Death", "OnDeath", new object[] {this, Killer}, this, Killer);

		        // Any AI actors targeting this player should stop
                foreach(ActorInstance A2 in ActorInstance.ActorInstanceList)
                {
			        if(A2.AITarget == this)
                        A2.AITarget = null;
		        }
	        }
	        else // Remove AI actors from game
            {
		        // Optional AI death script
// 		        Params$ = A\Actor\Race$ + "," + A\Actor\Class$ + ", " + A\X# + "," + A\Y# + "," + A\Z#
// 		        For i = 0 To 9
// 			        Params$ = Params$ + "," + A\ScriptGlobals$[i]
                // 		        Next
                if (DeathScript.Length > 0)
                    ScriptManager.Execute(DeathScript, "OnDeath", new object[] {this, Killer}, this, Killer);
                else
                    ScriptManager.Execute("Death", "OnAIDeath", new object[] { this, Killer }, this, Killer);

                if (CurrentSector != null)
                {
                    CurrentSector.Players.Remove(this);
                    CurrentSector.Actors.Remove(this);
                }

		        // Remove from spawn point if attached to one
		        if(SourceSP > -1)
                    if (SpawnedSector != null)
                    {

                        SpawnedSector.Spawned[SourceSP]--;
                        
                    }

                ScriptBase.ForceClose(this);
                FreeActorInstance();
            }
        
        }

        public void AICallForHelp()
        {
            if(AITarget != null)
            {
                int MinX = (int)CurrentSector.Sector.SectorX - 1;
                int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                LinkedListNode<ActorInstance> A2Node;

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            ActorInstance A2 = A2Node.Value;

                            if (A2 != null && A2.Actor.Aggressiveness != 3 && A2.Actor.Aggressiveness != 0)
                            {
                                if (A2.AIMode != AIModes.AI_Chase)
                                {
                                    // Must have a faction rating of 90% or more to help, and not be a pet
                                    if (A2.FactionRatings[homeFaction] >= 190)
                                    {
                                        if (A2 != this && A2.Leader == null)
                                        {
                                            Scripting.Math.SectorVector DistVec = Position - A2.Position;
                                            float XDist = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                            float ZDist = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                            float YDist = Math.Abs(DistVec.Y);

                                            float Dist = (XDist * XDist) + (YDist * YDist) + (ZDist * ZDist);
                                            if (Dist < (float)(A2.Actor.AggressiveRange * A2.Actor.AggressiveRange))
                                            {
                                                A2.AIMode = AIModes.AI_Chase;
                                                A2.AITarget = AITarget;
                                            }

                                        }
                                    }
                                }
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }

	        }
        }

        public void AILookForTargets()
        {
            if(Actor.Aggressiveness == 2)
            {
                int MinX = (int)CurrentSector.Sector.SectorX - 1;
                int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
                int MaxX = (int)CurrentSector.Sector.SectorX + 2;
                int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

                if (MinX < 0)
                    MinX = 0;
                if (MinZ < 0)
                    MinZ = 0;
                if (MaxX > ServerArea.Sectors.GetLength(0))
                    MaxX = ServerArea.Sectors.GetLength(0);
                if (MaxZ > ServerArea.Sectors.GetLength(1))
                    MaxZ = ServerArea.Sectors.GetLength(1);

                LinkedListNode<ActorInstance> A2Node;

                for (int z = MinZ; z < MaxZ; ++z)
                {
                    for (int x = MinX; x < MaxX; ++x)
                    {
                        A2Node = ServerArea.Sectors[x, z].Players.First;

                        while (A2Node != null)
                        {
                            ActorInstance A2 = A2Node.Value;

                            // Must have a faction rating under 50% to be attacked
                            if (FactionRatings[A2.homeFaction] < 150)
                            {
                                if (A2 != null && A2.Actor.Aggressiveness != 3)
                                {
                                    if (A2 != this)
                                    {
                                        Scripting.Math.SectorVector DistVec = Position - A2.Position;
                                        float XDist = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                        float ZDist = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                        float YDist = Math.Abs(DistVec.Y);

                                        float Dist = (XDist * XDist) + (YDist * YDist) + (ZDist * ZDist);
                                        if (Dist < (float)(Actor.AggressiveRange * Actor.AggressiveRange))
                                        {
                                            AIMode = AIModes.AI_Chase;
                                            AITarget = A2;
                                        }

                                    }
                                }
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
	        }
        }

        public void FireProjectile(Projectile P, ActorInstance A2)
        {
            // Check both actors are allowed to engage in combat
	        if(Actor.Aggressiveness == 3 || A2.Actor.Aggressiveness == 3)
                return;

	        // Check faction ratings
	        if(FactionRatings[A2.homeFaction] > 150)
                return;

	        // Tell all players about the projectile so they can display it
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)(RuntimeID));
            Pa.Write((ushort)(A2.RuntimeID));
            Pa.Write(P.MeshID);
            Pa.Write(P.Emitter1TexID);
            Pa.Write(P.Emitter2TexID);
            Pa.Write((byte)(P.Homing));
            Pa.Write((byte)(P.Speed));
            Pa.Write((byte)(P.Emitter1.Length));
            Pa.Write(P.Emitter1, false);
            Pa.Write(P.Emitter2, false);

            byte[] PaA = Pa.ToArray();

            int MinX = (int)CurrentSector.Sector.SectorX - 1;
            int MinZ = (int)CurrentSector.Sector.SectorZ - 1;
            int MaxX = (int)CurrentSector.Sector.SectorX + 2;
            int MaxZ = (int)CurrentSector.Sector.SectorZ + 2;

            if (MinX < 0)
                MinX = 0;
            if (MinZ < 0)
                MinZ = 0;
            if (MaxX > ServerArea.Sectors.GetLength(0))
                MaxX = ServerArea.Sectors.GetLength(0);
            if (MaxZ > ServerArea.Sectors.GetLength(1))
                MaxZ = ServerArea.Sectors.GetLength(1);

            LinkedListNode<ActorInstance> A2Node;

            for (int z = MinZ; z < MaxZ; ++z)
            {
                for (int x = MinX; x < MaxX; ++x)
                {
                    A2Node = ServerArea.Sectors[x, z].Players.First;

                    while (A2Node != null)
                    {
                        if (A2Node.Value != null && A2Node.Value.RNID > 0)
                        {
                            //TODO: Use HostToNetworkOrder
                            BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_Projectile, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }

	        // Does the projectile hit the target?
	        int ToHit = Server.Random.Next(100);
	        if(ToHit <= P.HitChance)
            {
		        // Calculate damage
		        int AP = A2.Inventory.GetArmourLevel() + (A2.Resistances[P.DamageType] - 100);
		        int Damage = (P.Damage + Server.Random.Next(-5, 5)) - AP;
		        if(Damage < 1)
                    Damage = 1;

		        // Apply damage
		        A2.Attributes.Value[Server.HealthStat] = A2.Attributes.Value[Server.HealthStat] - Damage;

		        // Tell player(s) if applicable
                PacketWriter Pa1 = new PacketWriter();
                PacketWriter Pa2 = new PacketWriter();

                Pa1.Write(GCLID);
                Pa1.Write((byte)('H'));
                Pa1.Write((ushort)(A2.RuntimeID));
                Pa1.Write((ushort)(Damage + 1));
                Pa1.Write((byte)(P.DamageType));

                Pa2.Write(A2.GCLID);
                Pa2.Write((byte)('Y'));
                Pa2.Write((ushort)(RuntimeID));
                Pa2.Write((ushort)(Damage + 1));
                Pa2.Write((byte)(P.DamageType));

                if(RNID > 0)
			        RCEnet.Send(RNID, MessageTypes.P_AttackActor, Pa1.ToArray(), true);
		        
		        if(A2.RNID > 0)
			        RCEnet.Send(A2.RNID, MessageTypes.P_AttackActor, Pa2.ToArray(), true);
		        

		        // Make attacked actor angry if it's defensive
		        if(A2.RNID == -1)
                {
			        if(A2.Actor.Aggressiveness == 1)
                    {
				        A2.AITarget = this;
				        A2.AIMode = AIModes.AI_Chase;
				        A2.AICallForHelp();
			        }else if(A2.Actor.Aggressiveness == 2 && A2.AITarget == null) // Or if it's aggressive and has no target...
                    {
				        A2.AITarget = this;
				        A2.AIMode = AIModes.AI_Chase;
				        A2.AICallForHelp();
			        }
		        }

		        // Death
                if (A2.Attributes.Value[Server.HealthStat] <= 0)
                    A2.KillActor(this);
	        }
        }
        #endregion

        #region Static Members
        static int LastRuntimeID = 1;
        static ActorInstance[] RuntimeIDList = new ActorInstance[65535];
        public static LinkedList<ActorInstance> ActorInstanceList = new LinkedList<ActorInstance>();
        static LinkedList<ActorInstance> ActorInstanceDelete = new LinkedList<ActorInstance>();
        #endregion

        #region Static Methods
        public static ActorInstance CreateActorInstance(Actor actor)
        {
            if (actor == null)
            {
                Log.WriteLine("Could not create ActorInstance; actor does not exist!");
                return null;
            }

            ActorInstance A = new ActorInstance();
            A.Actor = actor;
		    A.name = A.Actor.Race;
		    A.homeFaction = A.Actor.DefaultFaction;
    		
		    for(int i = 0; i < A.FactionRatings.Length; ++i)
			    A.FactionRatings[i] = Faction.Factions[A.homeFaction].DefaultRatings[i];
    		
		    for(int i = 0; i < A.Attributes.Value.Length; ++i)
		    {
			    A.Attributes.Value[i] = A.Actor.Attributes.Value[i];
			    A.Attributes.Maximum[i] = A.Actor.Attributes.Maximum[i];
		    }

		    for(int i = 0; i < A.Resistances.Length; ++i)
			    A.Resistances[i] = A.Actor.Resistances[i];
    		
		    for(int i = 0; i < A.MemorisedSpells.Length; ++i)
			    A.MemorisedSpells[i] = 5000; // No spell memorised
    		
		    if(A.Actor.Genders == 2)
			    A.gender = 1;

		    A.level = 1;
		    A.RuntimeID = -1;
		    A.LastAttack = Server.MilliSecs();
		    A.SourceSP = -1;
		    A.LastPortal = -1;
		    A.IgnoreUpdate = false;
    		
		    return A;
        }

        public static ActorInstance CreateFromPacketReader(Scripting.PacketReader Pa, Scripting.PacketWriter ReAllocator)
        {
            ActorInstance AI = new ActorInstance();

            AI.Deserialize(Pa, ReAllocator);

            return AI;
        }

        public static ActorInstance FindFromRuntimeID(int id)
        {
            if (id < 0 || id > 65534)
                return null;

            return RuntimeIDList[id];
        }

        public static ActorInstance FindFromRNID(int id)
        {
            foreach (ActorInstance AI in ActorInstanceList)
                if (AI.RNID == id)
                    return AI;
            return null;
        }

        public static ActorInstance FromGCLID(uint gclid)
        {
            if (gclid == 0)
                return null;

            foreach (ActorInstance AI in ActorInstanceList)
                if (AI.GCLID == gclid)
                    return AI;
            return null;
        }

        public static ActorInstance FindFromName(string name)
        {
            foreach (ActorInstance AI in ActorInstanceList)
                if (AI.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return AI;
            return null;
        }

        public static ActorInstance FindPlayerFromName(string name)
        {
            foreach (ActorInstance AI in ActorInstanceList)
                if (AI.RNID > -1 && AI.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return AI;
            return null;
        }

        public static void Delete(ActorInstance item)
        {
            ActorInstanceDelete.AddLast(item);
        }

        public static void Clean()
        {
            foreach (ActorInstance AI in ActorInstanceDelete)
            {
                ActorInstanceList.Remove(AI);
            }
            ActorInstanceDelete.Clear();
        }

        public static void UpdateActorInstances(bool broadcast)
        {
            // Current ticks for effects updates and such
            uint T = Server.MilliSecs();

            // Recharging this fame?
            bool Recharge = false;
            if (T - Server.LastSpellRecharge > 100)
            {
                Recharge = true;
                Server.LastSpellRecharge = T;
            }

            foreach (ActorInstance AI in ActorInstanceList)
            {
                Scripting.Math.SectorVector OldPosition = AI.Position;

                LinkedList<ActorEffect> Removals = new LinkedList<ActorEffect>();

                // Update actor effects
                foreach (ActorEffect AE in AI.ActorEffects)
                {
                    // Alive and online
                    if (AI.RNID != 0)
                    {
                        // Remove effect when time is up
                        if (T - AE.CreatedTime > AE.Length)
                        {
                            int i = 0;

                            // Tell client if applicable
                            if (AI.RNID > 0)
                            {
                                PacketWriter Pa = new PacketWriter();
                                Pa.Write(AI.GCLID);
                                Pa.Write((byte)'R');
                                Pa.Write(AE.AllocID);
                                for (i = 0; i < 40; ++i)
                                {
                                    Pa.Write(AE.Attributes.Value[i]);
                                }

                                RCEnet.Send(AI.RNID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                            }

                            // Remove effect
                            for (i = 0; i < 40; ++i)
                            {
                                AI.Attributes.Value[i] -= AE.Attributes.Value[i];
                            }
                            Removals.AddLast(AE);
                        }
                    }
                } // Actor effects

                foreach (ActorEffect AE in Removals)
                {
                    AI.ActorEffects.Remove(AE);
                }
                Removals.Clear();

                LinkedList<MemorizingSpell> SpellRemovals = new LinkedList<MemorizingSpell>();
                foreach (MemorizingSpell MS in AI.MemorizingSpells)
                {
                    if (T - MS.CreatedTime > 6000)
                    {
                        if (AI.SpellLevels[MS.KnownNum] > 0)
                        {
                            for (int i = 0; i < 10; ++i)
                            {
                                if (AI.MemorisedSpells[i] == 5000)
                                {
                                    AI.MemorisedSpells[i] = MS.KnownNum;
                                    AI.SpellCharge[i] = 0;
                                    break;
                                }
                            }
                        }
                        SpellRemovals.AddLast(MS);
                    }
                }

                foreach (MemorizingSpell MS in SpellRemovals)
                {
                    AI.MemorizingSpells.Remove(MS);
                }
                SpellRemovals.Clear();

                // Update custom dialogs
                if (AI.RNID > 0 || AI.RNID < -1)
                {
                    if (AI.PropertyPacket.Length > 0)
                    {
                        PacketWriter GUIUpdate = new PacketWriter();
                        GUIUpdate.Write(AI.GCLID);
                        GUIUpdate.Write(AI.PropertyPacket.ToArray(), 0);
                        AI.PropertyPacket.Clear();

                        RCEnet.Send(AI.RNID, MessageTypes.P_PropertiesUpdate, GUIUpdate.ToArray(), true);
                    }
                }

                // Update WaitTimers
                LinkedListNode<WaitTimeRequest> WrNode = AI.WaitTimeRequests.First;
                while (WrNode != null)
                {
                    WaitTimeRequest Wr = WrNode.Value;
                    LinkedListNode<WaitTimeRequest> Del = WrNode;
                    WrNode = WrNode.Next;

                    if (Wr.Hour == WorldEnvironment.Hour && Wr.Minute == WorldEnvironment.Minute)
                    {
                        if (Wr.CallScript)
                        {
                            ScriptManager.DelayedExecute(Wr.CallbackScript, Wr.CallbackMethod, new object[] { AI, Wr }, AI, null);
                        }
                        else
                        {
                            Wr.Invoke(AI);
                        }

                        AI.WaitTimeRequests.Remove(Del);
                    }
                }

                // Update WaitItems
                LinkedListNode<WaitItemRequest> WiNode = AI.WaitItemRequests.First;
                while (WiNode != null)
                {
                    WaitItemRequest Wi = WiNode.Value;
                    LinkedListNode<WaitItemRequest> Del = WiNode;
                    WiNode = WiNode.Next;

                    if(AI.Inventory.HasItem(Wi.ItemName) == Wi.ItemCount)
                    {
                        if (Wi.CallScript)
                        {
                            ScriptManager.DelayedExecute(Wi.CallbackScript, Wi.CallbackMethod, new object[] { AI, Wi }, AI, null);
                        }
                        else
                        {
                            Wi.Invoke(AI);
                        }

                        AI.WaitItemRequests.Remove(Del);
                    }
                }

                Scripting.Math.SectorVector DistVec = AI.Destination - AI.Position;
                float XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                float ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
                float YDist = DistVec.Y;

                if(AI.RuntimeID > -1)
                {
			        // Recharge spells
			        if(Recharge == true)
                    {
				        if(Server.RequireMemorise)
                        {
                            for(int i = 0; i < 10; ++i)
                            {
						        if(AI.SpellCharge[i] > 0)
                                    AI.SpellCharge[i] -= 100;
					        }
				        }else
                        {
                            for(int i = 0; i < 1000; ++i)
                            {
						        if(AI.SpellCharge[i] > 0)
                                    AI.SpellCharge[i] -= 100;
					        }
				        }
			        }

			        // Move (except mounts)
			        if(AI.Rider == null)
                    {
                        float Speed = 0.0f;

				        if(AI.Mount == null)
                        {
                            Speed = 1.5f * (((float)AI.Attributes.Value[Server.SpeedStat]) / ((float)AI.Attributes.Maximum[Server.SpeedStat]));
				        }else
                        {
                            Speed = 1.5f * (((float)AI.mount.Attributes.Value[Server.SpeedStat]) / ((float)AI.mount.Attributes.Maximum[Server.SpeedStat]));
				        }
				        if(AI.WalkingBackward)
                            Speed *= 0.5f;

				        if(AI.IsRunning)
                        {
					        bool Allowed = true;
					        if(Server.EnergyStat > -1 && AI.Mount == null)
                            {
						        AI.Attributes.Value[Server.EnergyStat] = AI.Attributes.Value[Server.EnergyStat] - 1;
						        if(AI.Attributes.Value[Server.EnergyStat] <= 0)
                                {
							        Allowed = false;
							        AI.Attributes.Value[Server.EnergyStat] = 0;
							        AI.IsRunning = false;
						        }
					        }
					        if(Allowed)
                                Speed *= 2.0f;
				        }

                        //if(AI.ServerArea.Name == "DefaultZone")
                        //    Log.WriteLine("Sect: " + AI.RuntimeID.ToString() + "; X: " + AI.CurrentSector.Sector.SectorX + "; Z: " + AI.CurrentSector.Sector.SectorZ);
				        
				        if(Math.Abs(XDist) > 0.5f || Math.Abs(ZDist) > 0.5f)
                        {
                            if (AI.Actor.Environment != ActorEnvironment.Fly)
                            {
                                AI.Position.X += (XDist / (Math.Abs(XDist) + Math.Abs(ZDist) + Math.Abs(YDist))) * Speed;
                                AI.Position.Z += (ZDist / (Math.Abs(XDist) + Math.Abs(ZDist) + Math.Abs(YDist))) * Speed;

                                //Log.WriteLine("Dest: " + AI.RuntimeID.ToString() + "; " + AI.Destination.X + ", " + AI.Destination.Z);
//                                  string Dest = AI.Destination.X.ToString() + ", " + AI.Destination.Z;
//                                  Dest.PadLeft(30);
//                                  Log.WriteLine("Dist: " + AI.RuntimeID.ToString() + "; " + XDist + ", " + ZDist + "; Dest: " + Dest);

                                AI.Position.Y += (YDist / (Math.Abs(XDist) + Math.Abs(ZDist) + Math.Abs(YDist))) * Speed;
                                AI.Position.FixValues();
                            }
                            else
                            {
                                AI.Position.X += (XDist / (Math.Abs(XDist) + Math.Abs(ZDist))) * Speed;
                                AI.Position.Z += (ZDist / (Math.Abs(XDist) + Math.Abs(ZDist))) * Speed;
                                AI.Position.FixValues();
                            }
				        }
			        }else // Mounts stay with their rider
                    {
                        AI.Position = AI.rider.Position;
			        }

			        // Underwater damage
			        if(AI.Actor.Environment != ActorEnvironment.Swim)
                    {
				        Area.ServerWater Underwater = null;
                        float DistUnder = 0.0f;

                        foreach(Area.ServerWater SW in AI.CurrentSector.Sector.Waters)
                        {
					        if(AI.Position.Y < SW.Position.Y + 0.5f)
                            {
                                if (AI.Position.WithinSectorDimension(SW.Position, SW.Width, SW.Depth))
                                {
                                    if (AI.Underwater == 0)
                                        AI.Underwater = (int)T;
                                    Underwater = SW;
                                    DistUnder = SW.Position.Y - AI.Position.Y;

                                    break;
                                }
					        }
				        }

				        if(Underwater == null)
                        {
					        AI.Underwater = 0;

					        // Restore breath
					        if(Server.BreathStat > -1)
                            {
						        if(AI.Attributes.Value[Server.BreathStat] < AI.Attributes.Maximum[Server.BreathStat] && Server.Random.Next(1, 10) == 1)
                                {
							        AI.Attributes.Value[Server.BreathStat] += 1;
							        if(AI.RNID > 0)
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write(AI.GCLID);
                                        Pa.Write((byte)'A');
                                        Pa.Write((ushort)AI.RuntimeID);
                                        Pa.Write((byte)Server.BreathStat);
                                        Pa.Write((ushort)AI.Attributes.Value[Server.BreathStat]);

								        RCEnet.Send(AI.RNID, MessageTypes.P_StatUpdate, Pa.ToArray(), true);
							        }
						        }
					        }
				        }else if(T - AI.Underwater >= 1000)
                        {
					        AI.Underwater += 1000;

					        // Remove breath, or health if none left
					        if(Server.BreathStat > -1 && DistUnder > 2.0f)
                            {
						        AI.Attributes.Value[Server.BreathStat] -= 1;
						        if(AI.Attributes.Value[Server.BreathStat] < 0)
                                {
							        AI.Attributes.Value[Server.BreathStat] = 0;
                                    AI.UpdateAttribute(Server.HealthStat, AI.Attributes.Value[Server.HealthStat] - 1);
							        if(AI.Attributes.Value[Server.HealthStat] <= 0)
                                    {
								        AI.Attributes.Value[Server.HealthStat] = 0;
								        AI.KillActor(null);
							        }
						        }

						        if(AI.RNID > 0)
                                {
                                    PacketWriter Pa = new PacketWriter();
                                    Pa.Write(AI.GCLID);
                                    Pa.Write((byte)'A');
                                    Pa.Write((ushort)AI.RuntimeID);
							        Pa.Write((byte)Server.BreathStat);
                                    Pa.Write((ushort)AI.Attributes.Value[Server.BreathStat]);

							        RCEnet.Send(AI.RNID, MessageTypes.P_StatUpdate, Pa.ToArray(), true);
						        }
					        }

					        // Water damage
					        if(Underwater.Damage > 0)
                            {
						        int Damage = Underwater.Damage - (AI.Resistances[Underwater.DamageType] - 100);
						        if(Damage < 1)
                                    Damage = 1;

						        AI.UpdateAttribute(Server.HealthStat, AI.Attributes.Value[Server.HealthStat] - Damage);
						        if(AI.Attributes.Value[Server.HealthStat] <= 0)
                                {
							        AI.Attributes.Value[Server.HealthStat] = 0;
							        AI.KillActor(null);
						        }
					        }
				        }
			        }else
                    {
				        AI.Underwater = 1;
			        } // Underwater damage

			        // Update AI
			        if(AI.RNID == -1)
                    {
                        // AI can move sectors, but its processing has to happen here since
                        // player movement happens on P_StandardUpdate
                        if (OldPosition.SectorX != AI.Position.SectorX || OldPosition.SectorZ != AI.Position.SectorZ)
                            AI.ServerArea.ActorChangedSector(AI);


				        // Wait mode
				        if(AI.AIMode == AIModes.AI_Wait)
                        {
					        // Look for targets now and then
                            if (Server.Random.Next(1, 10) == 1)
                                AI.AILookForTargets();
                            else
                                AI.AIMode = AIModes.AI_Run;
                            
				        }else if(AI.AIMode == AIModes.AI_Patrol || AI.AIMode == AIModes.AI_Run) // Patrol mode
                        {
                            DistVec = AI.Position - AI.Destination;
                            XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                            ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
                            float Dist = (XDist * XDist) + (ZDist * ZDist);

                            if (Dist > 4.0f)
                            {
                                // Carry on heading to waypoint
                                AI.IsRunning = true;
                                //AI.Destination = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Position;
                                AI.AILookForTargets();
                            }
                            else
                            {                                
                                // Find next waypoint
                                int NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Previous;
                                //RCScript.Log("Current waypoint: Next A:" + AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextA + " Next B: " + AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextB);
                                if (Server.Random.Next(2) + 1 == 1)
                                {
                                    NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextA;
                                    if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                    {
                                        NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextB;
                                    }
                                }
                                else
                                {
                                    NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextB;
                                    if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                        NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextA;

                                }

                                if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                {
                                    //RCScript.Log("Next waypoint is too large: " + NextWP);
                                    NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Previous;

                                }
                                if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                {
                                    // NextA/B/Previous are out of range so must be free roam spawn
                                    // Find spawn to get spawn range
                                    Area.Waypoint waypoint = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint];                                  
                                    for (int i = 0; i < AI.ServerArea.Area.Spawns.Length; i++)
                                    {
                                        Area.Spawn spawn = AI.ServerArea.Area.Spawns[i];
                                        if (spawn.Waypoint.ID == waypoint.ID)
                                        {
                                            // Found relevant spawn for waypoint
                                            AI.Destination = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Position;
                                            AI.Destination.X = AI.Destination.X + (-spawn.Range + (Server.Rnd(spawn.Range) * 2));
                                            AI.Destination.Z = AI.Destination.Z + (-spawn.Range + (Server.Rnd(spawn.Range) * 2));
                                            break;
                                        }
                                    }


                                }
                                else
                                {
                                    //Log.WriteLine("WP: " + AI.RuntimeID + "; Cur: " + AI.CurrentWaypoint + "; Next: " + NextWP);

                                    AI.Destination = AI.ServerArea.Area.Waypoints[NextWP].Position;
                                    //AI.Destination.X = AI.Destination.X + Server.Rnd(-2.0f, 2.0f);
                                    //AI.Destination.Z = AI.Destination.Z + Server.Rnd(-2.0f, 2.0f);
                                    //AI.Destination.FixValues();
                                    AI.CurrentWaypoint = NextWP;

                                    // Waypoint pause
                                    if (AI.ServerArea.Area.Waypoints[NextWP].Pause > 0)
                                    {
                                        AI.AIMode = AIModes.AI_PatrolPause;
                                        AI.AIPatrolPause = T;
                                    }
                                }
                            }

                            #region Old broken waypoint code -Commented out but never forgotten - Ben
                            /*
					        if(true || XDist <= 2.0f && ZDist <= 2.0f)
                            {
						        // Set running state
                              
                                
						        if(AI.AIMode == AIModes.AI_Run)
							        AI.IsRunning = true;
						        else
							        AI.IsRunning = false;
                                //AI.IsRunning = true;
						        // Find auto-movement range if there is one
						        float SpawnRange = 0.0f;
                                bool Found = false;

                                for (int i = 0; i < AI.CurrentSector.Sector.Spawns.Length; ++i)
                                {
                                    if (AI.ServerArea.Area.WaypointEqualsID(AI.CurrentSector.Sector.Spawns[i].Waypoint, AI.CurrentWaypoint))
                                    {
                                        SpawnRange = AI.CurrentSector.Sector.Spawns[i].Range;
                                        Found = true;
                                        break;
                                    }
                                }

                                if (!Found)
                                {
                                    for (int i = 0; i < AI.ServerArea.Area.Spawns.Length; ++i)
                                    {
                                        if (AI.ServerArea.Area.WaypointEqualsID(AI.ServerArea.Area.Spawns[i].Waypoint, AI.CurrentWaypoint))
                                        {
                                            SpawnRange = AI.ServerArea.Area.Spawns[i].Range;
                                            break;
                                        }
                                    }
                                }

						        // Auto-move within an area
						        if(SpawnRange >= 5.0f)
                                {
                                    AI.Destination = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Position;
                                    AI.Destination.X = AI.Destination.X + Server.Rnd(-SpawnRange, SpawnRange);
                                    AI.Destination.Z = AI.Destination.X + Server.Rnd(-SpawnRange, SpawnRange);
                                    AI.Destination.FixValues();

						        }else // Follow waypoints
                                {
							        AI.Position.Y = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Position.Y + Server.Rnd(-1.5f, 1.5f);
							        AI.PreviousPosition = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Position;

                                    int NextWP = 0;
							        if(Server.Random.Next(1, 2) == 1)
                                    {
                                        NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextA;
								        if(NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                            NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextB;
							        }else
                                    {
                                        NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextB;
                                        if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                            NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].NextA;
							        }
                                    if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                        NextWP = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Previous;
                                    if (NextWP >= AI.ServerArea.Area.Waypoints.Length)
                                    {
								        AI.AIMode = AIModes.AI_Wait;
							        }else
                                    {
                                        //Log.WriteLine("WP: " + AI.RuntimeID + "; Cur: " + AI.CurrentWaypoint + "; Next: " + NextWP);
                                       
                                        AI.Destination = AI.ServerArea.Area.Waypoints[NextWP].Position;
                                        AI.Destination.X = AI.Destination.X + Server.Rnd(-2.0f, 2.0f);
                                        AI.Destination.Z = AI.Destination.Z + Server.Rnd(-2.0f, 2.0f);
                                        AI.Destination.FixValues();
								        AI.CurrentWaypoint = NextWP;
								        
                                        // Waypoint pause
								        if(AI.ServerArea.Area.Waypoints[NextWP].Pause > 0)
                                        {
									        AI.AIMode = AIModes.AI_PatrolPause;
									        AI.AIPatrolPause = T;
								        }
							        }
						        }
					        } // Waypoint distance check

					        // Look for targets now and then
					        if(Server.Random.Next(1, 10) == 1)
                                AI.AILookForTargets();
                             */
#endregion
                        }
                        else if(AI.AIMode == AIModes.AI_PatrolPause) // Paused while on patrol mode
                        {
					        if(T - AI.AIPatrolPause >= AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Pause * 1000)
                            {
						        AI.AIMode = AIModes.AI_Patrol;
					        }else
                            {
						        // Look for targets now and then
						        if(Server.Random.Next(1, 10) == 1)
                                    AI.AILookForTargets();
					        }
				        }else if(AI.AIMode == AIModes.AI_Chase) // Attack mode
                        {
					        // Target dead
					        if(AI.AITarget != null)
						        if(AI.AITarget.Attributes.Value[Server.HealthStat] <= 0)
                                    AI.AITarget = null;

					        // Lost target
					        if(AI.AITarget == null)
                            {
						        AI.AIMode = AIModes.AI_Patrol;
						        AI.Destination = AI.ServerArea.Area.Waypoints[AI.CurrentWaypoint].Position;
						        AI.IsRunning = false;
					        }else // Chase target
                            {
						        // Target left game
						        if(AI.AITarget.RNID == 0 || AI.AITarget.ServerArea != AI.ServerArea)
                                {
							        AI.AITarget = null;
						        }else // Target available - kick its arse! < Original comment, NOT JB
                                {
                                    DistVec = AI.Position - AI.AITarget.Position;
							        XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                                    ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
							        float Dist = (XDist * XDist) + (ZDist * ZDist);
							        float CheckDist = 4.0f + AI.Actor.Radius + AI.AITarget.Actor.Radius;

							        if(Dist > CheckDist * CheckDist)
                                    {
                                        AI.Destination = AI.AITarget.Position;
								        AI.IsRunning = true;
							        }else
                                    {
                                        AI.Destination = AI.Position;
								        AI.IsRunning = false;
								        
                                        // Attempt to hit target
								        if(T - AI.LastAttack >= Server.CombatDelay)
                                        {
                                            if(AI.Attack(AI.AITarget))
                                            {
                                                AI.Destination = AI.Position;
                                            }
								        }
							        }
						        }
					        }
				        }else if(AI.AIMode == AIModes.AI_Pet) // Pet AI
                        {
					        // Move towards leader's position
                            AI.Destination = AI.leader.Position;

                            //JB: This line gets removed, because it only makes a fake
                            // Y interpolation.. just use destination Y in 2.50.
					        //AI.Y = AI.Y + ((AI.leader.Y - AI.Y) / 50.0f);
					        
                            AI.IsRunning = AI.leader.IsRunning;
					        
                            // When close enough to leader, stop moving
                            DistVec = AI.Position - AI.leader.Position;
                            XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                            ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
					        float Dist = (XDist * XDist) + (ZDist * ZDist);
					        float CheckDist = 4.0f + AI.Actor.Radius + AI.leader.Actor.Radius;

					        if(Dist <= CheckDist * CheckDist)
                            {
                                AI.Destination = AI.Position;
					        }

					        // Keep updated with leader's target
					        if(AI.Actor.Aggressiveness < 3)
                            {
						        if(AI.leader.AITarget != null)
                                {
                                    DistVec = AI.leader.AITarget.Position - AI.leader.Position;
                                    XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                                    ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
							        Dist = (XDist * XDist) + (ZDist * ZDist);
							        if(Dist <= 1000.0f)
                                    {
								        AI.AITarget = AI.leader.AITarget;
								        AI.AIMode = AIModes.AI_PetChase;
							        }
						        }
					        }
				        }else if(AI.AIMode == AIModes.AI_PetChase) // Pet AI attack mode
                        {
					        // Keep updated with leader's target
					        AI.AITarget = AI.leader.AITarget;
					        if(AI.AITarget != null)
                                AI.AIMode = AIModes.AI_Pet;

					        // Check if leader is too far away
                            DistVec = AI.Position - AI.leader.Position;
                            XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                            ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
					        float Dist = (XDist * XDist) + (ZDist * ZDist);
					        if(Dist > 3500.0f)
                                AI.AITarget = null;

					        // Target dead
					        if(AI.AITarget != null)
						        if(AI.AITarget.Attributes.Value[Server.HealthStat] <= 0)
                                    AI.AITarget = null;

					        // Lost target
					        if(AI.AITarget == null)
                            {
						        AI.AIMode = AIModes.AI_Pet;
					        }else // Chase target
                            {
						        // Target left game
						        if(AI.AITarget.RNID == 0 || AI.AITarget.ServerArea != AI.ServerArea)
                                {
							        AI.AITarget = null;
						        }else // Target available - attack it
                                {
							        AI.Destination = AI.AITarget.Position;
							        AI.IsRunning = true;

							        // Attempt to hit target
							        if(T - AI.LastAttack >= Server.CombatDelay)
                                    {
								        if(AI.Attack(AI.AITarget))
                                        {
                                            AI.Destination = AI.Position;
                                        }
							        }
						        }
					        }
                        }
                        else if (AI.AIMode == AIModes.AI_MoveToDestination)
                        {
                            DistVec = AI.Position - AI.Destination;
                            XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                            ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
                            float Dist = (XDist * XDist) + (ZDist * ZDist);
                           
                            if (Dist > 4.0f)
                            {
                              
                                AI.IsRunning = true;
                            }
                            else
                            {
                                AI.Destination = AI.Position;
                                AI.IsRunning = false;
                                AI.AIMode = AIModes.AI_Wait;
                            }

                        }

                        // JB: Update yaw
                        DistVec = AI.Destination - AI.Position;
                        XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
                        ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
                        AI.Yaw = (float)Math.Atan2(ZDist, -XDist);
                        AI.Yaw *= (180.0f / (float)Math.PI);
                        AI.Yaw -= 90.0f;

                        while (AI.Yaw > 180.0f)
                            AI.Yaw -= 360.0f;
                        while (AI.Yaw < -180.0f)
                            AI.Yaw += 360.0f;

			        } // A.I.
                } // AI.RuntimeID > -1

            } // Actor Instance

            // Tell all human actor instances about other actor instances in their area (not too frequently)
	
	        bool AlsoUpdateMiddleRange = false;
	        if(Server.MilliSecs() - Server.LastCompleteUpdate > Server.CompleteUpdateMS)
            {
		        AlsoUpdateMiddleRange = true;
		        Server.LastCompleteUpdate = Server.MilliSecs();
	        }
        				
	        if(broadcast)
            {
                foreach(ActorInstance AI in ActorInstance.ActorInstanceList)
                {
			        if(AI.RNID > 0 && AI.CurrentSector != null)
                    {

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
                                bool OnActors = false;

                                while (A2Node != null)
                                {
                                    ActorInstance A2 = A2Node.Value;

                                    if (A2.RuntimeID > -1 && AI != A2)
                                    {
                                        PacketWriter Pa = new PacketWriter();
                                        Pa.Write(AI.GCLID);
                                        Pa.Write((ushort)A2.RuntimeID);
                                        Pa.Write(A2.Position);
                                        Pa.Write((byte)(A2.IsRunning ? 1 : 0));

                                        byte Flags = 0;
                                        Flags += (byte)(A2.WalkingBackward ? 1 : 0);
                                        Flags += (byte)(A2.StrafingLeft ? 2 : 0);
                                        Flags += (byte)(A2.StrafingRight ? 4 : 0);

                                        Pa.Write(Flags);
                                        Pa.Write(A2.Destination);


                                        if (A2.Mount != null)
                                        {
                                            Pa.Write((ushort)A2.mount.RuntimeID);
                                        }
                                        else
                                        {
                                            Pa.Write((ushort)0);
                                        }

                                        //short YawShort = (short)((A2.Yaw / 180.0f) * 32767.0f);
                                        Pa.Write((float)A2.Yaw);

                                        if (A2 == AI)
                                        {
                                            if (Server.EnergyStat > -1)
                                            {
                                                Pa.Write((ushort)A2.Attributes.Value[Server.EnergyStat]);
                                            }
                                        }
                                        else if (A2.Actor.Environment == ActorEnvironment.Fly)
                                        {
                                            //                                             float YPos = A2.Y;
                                            // 
                                            //                                             if (A2.AIMode < AIModes.AI_Pet && A2.RNID < 1)
                                            //                                             {
                                            //                                                 Scripting.Math.SectorVector DistVec = A2.PreviousPosition - A2.Destination;
                                            //                                                 float XDist = Math.Abs((((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X);
                                            //                                                 float ZDist = Math.Abs((((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z);
                                            //                                                 float TotalDist = (XDist * XDist) + (ZDist * ZDist);
                                            //                                                 XDist = Math.Abs(A2.X - A2.DestX);
                                            //                                                 ZDist = Math.Abs(A2.Z - A2.DestZ);
                                            //                                                 float DoneDist = (XDist * XDist) + (ZDist * ZDist);
                                            //                                                 YPos = A2.Y + ((AI.ServerArea.Area.Waypoints[A2.CurrentWaypoint].Y - A2.Y) * (DoneDist / TotalDist));
                                            //                                             }
                                            // 
                                            //                                             Pa.Write(YPos);
                                                
                                            //JB: TODO: Compatibility
                                            Pa.Write(A2.Position.Y);
                                        }



                                        RCEnet.Send(AI.RNID, MessageTypes.P_StandardUpdate, Pa.ToArray(), false);

                                    }

                                    A2Node = A2Node.Next;
                                    if (A2Node == null && !OnActors)
                                    {
                                        OnActors = true;
                                        A2Node = AI.ServerArea.Sectors[x, z].Actors.First;
                                    }
                                        
                                } // A2
                            } // x
                        } // z

			        } // RNID
		        } // foreach ActorInstance
	        } // if(broadcast)
        }
        #endregion
    }
}
