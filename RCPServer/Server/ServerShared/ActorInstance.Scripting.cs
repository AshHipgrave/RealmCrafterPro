using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace RCPServer
{
    partial class ActorInstance
    {
        #region Scripting
        #region GUI

        public override void CreateDialog(Scripting.Forms.Control control)
        {
            if (RNID == -1 || RNID == 0)
                throw new InvalidOperationException("Actor.CreateDialog() cannot be called for actors who are not players!");

            control.SendingToClient(this, ref FormAllocIDs, ClientControls);
            PacketWriter Pa = control.ControlPropertyPacket();

            ScriptedForms.AddLast(control);
            PacketWriter ProxMsg = new PacketWriter();
            ProxMsg.Write(GCLID);
            ProxMsg.Write(Pa.ToArray(), 0);

            RCEnet.Send(RNID, MessageTypes.P_OpenForm, ProxMsg.ToArray(), true);
        }

        public override void CloseDialog(Scripting.Forms.Control control)
        {
            CloseDialog(control, true);
        }

        public void CloseDialog(Scripting.Forms.Control control, bool sendPacket)
        {
            if (control.ClientMaster == null || control.Actor == null)
                return;

            // Only the top-level control can be destroyed
            control = control.ClientMaster;
            control.Closing(false);

            // Get the packet required to delete the client instance
            PacketWriter Pa = control.GetDeletePacket();

            // Remove
            control.ClientInstanceRemoved(ClientControls);
            ScriptedForms.Remove(control);

            if (sendPacket)
            {
                // Send to client
                PacketWriter ProxMsg = new PacketWriter();
                ProxMsg.Write(GCLID);
                ProxMsg.Write(Pa.ToArray(), 0);

                RCEnet.Send(RNID, MessageTypes.P_CloseForm, ProxMsg.ToArray(), true);
            }
        }

        public override Scripting.Forms.Control FindControl(string name)
        {
            foreach (Scripting.Forms.Control C in ScriptedForms)
            {
                if (C.Name.Equals(name))
                    return C;
            }

            return null;
        }

        public override void PostPropertyPacket(PacketWriter pa)
        {
            PropertyPacket.Write(pa.ToArray(), 0, pa.Length);
        }

        #endregion
        #region Abilities
        public override void AddAbility(string name)
        {
            AddAbility(name, 1);
        }

        public override void AddAbility(string name, int level)
        {
            if (Level <= 0)
                Level = 1;

            // Check it's not already known
            bool Known = false;
            for (int i = 0; i < 1000; ++i)
            {
                if (SpellLevels[i] > 0)
                {
                    if (Spell.Spells[KnownSpells[i]].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Known = true;
                        break;
                    }
                }
            }

            if (!Known)
            {
                foreach (Spell Sp in Spell.Spells.Values)
                {
                    if (Sp != null && Sp.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        AddSpell(Sp.ID, level);
                        return;
                    }
                }
            }
        }

        public override void RemoveAbility(string name)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (SpellLevels[i] > 0)
                {
                    if (Spell.Spells[KnownSpells[i]].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        DeleteSpell(i);
                    }
                }
            }
        }

        public override int GetAbilityLevel(string name)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (SpellLevels[i] > 0)
                {
                    if (Spell.Spells[KnownSpells[i]].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        return SpellLevels[i];
                }
            }

            return 0;
        }

        public override void SetAbilityLevel(string name, int level)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (SpellLevels[i] > 0)
                {
                    if (Spell.Spells[KnownSpells[i]].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SpellLevels[i] = level;
                        if (RNID > 0)
                        {
                            PacketWriter Pa = new PacketWriter();
                            Pa.Write(GCLID);
                            Pa.Write((byte)'L');
                            Pa.Write(level);
                            Pa.Write(Spell.Spells[KnownSpells[i]].Name, false);

                            RCEnet.Send(RNID, MessageTypes.P_KnownSpellUpdate, Pa.ToArray(), true);
                        }

                        break;
                    }
                }
            }
        }

        public override bool AbilityKnown(string name)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (SpellLevels[i] > 0)
                {
                    if (Spell.Spells[KnownSpells[i]].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public override bool AbilityMemorized(string name)
        {
            for (int i = 0; i < 10; ++i)
            {
                if (MemorisedSpells[i] != 5000)
                {
                    uint ID = KnownSpells[MemorisedSpells[i]];
                    if (Spell.Spells[ID].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region Properties
        public uint GlobalID
        {
            get { return GCLID; }
        }

        public override int Aggressiveness
        {
            get
            {
                return Actor.Aggressiveness;
            }
        }

        public override Scripting.AIModes AIState
        {
            get
            {
                return AIMode;
            }
            set
            {
                AIMode = value;
            }
        }

        public override int Beard
        {
            get
            {
                return beard;
            }
            set
            {
                if(gender == 0)
                {
                    beard = (ushort)value;

                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(0);
                    Pa.Write((byte)'D');
                    Pa.Write((ushort)RuntimeID);
                    Pa.Write((byte)beard);
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
                                    RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AppearanceUpdate, PaA, true);
                                }

                                A2Node = A2Node.Next;
                            }
                        }
                    }
                }
            }
        }

        public override void SendEnvironmentUpdate(byte[] packetData)
        {
            if (RNID > 0)
            {
                // TODO: Use HostToNetworkOrder if necessary
                BitConverter.GetBytes(GCLID).CopyTo(packetData, 0);

                RCEnet.Send(RNID, MessageTypes.P_WeatherChange, packetData, true);
            }
        }

        public override Scripting.Math.ShaderParameter GetShaderParameter(string name)
        {
            if (!ShaderParameters.ContainsKey(name))
                return new Scripting.Math.ShaderParameter(0);

            return ShaderParameters[name];
        }

        public override void SetShaderParameter(string name, Scripting.Math.ShaderParameter parameter)
        {
            ShaderParameters[name] = parameter;

            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)RuntimeID);
            Pa.Write((byte)parameter.GetParameterType());
            Scripting.Math.Vector4 v4 = parameter.GetParameterVector4();
            Pa.Write(v4.X);
            Pa.Write(v4.Y);
            Pa.Write(v4.Z);
            Pa.Write(v4.W);
            Pa.Write((byte)name.Length);
            Pa.Write(name, false);
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
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_ShaderConstant, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }
        }

        public override int Clothes
        {
            get
            {
                return BodyTex;
            }
            set
            {
                BodyTex = (ushort)value;

                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((byte)'B');
                Pa.Write((ushort)RuntimeID);
                Pa.Write((byte)BodyTex);
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
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AppearanceUpdate, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        public override int Face
        {
            get
            {
                return FaceTex;
            }
            set
            {
                FaceTex = (ushort)value;

                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((byte)'F');
                Pa.Write((ushort)RuntimeID);
                Pa.Write((byte)FaceTex);
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
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AppearanceUpdate, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        public override int Gender
        {
            get
            {
                if(gender == 0)
                {
                    if(Actor.Genders == 3)
                        return 3;
                    else
                        return 1;
                }else
                {
                    return 2;
                }
            }
            set
            {
                gender = (byte)(value - 1);
		        if(gender == 2)
                    gender = 0;

		        if(Actor.Genders == 2 && gender != 1)
                    gender = 1;

		        if((Actor.Genders == 1 || Actor.Genders == 3) && gender != 0)
                    gender = 0;

                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((byte)'G');
                Pa.Write((ushort)RuntimeID);
                Pa.Write((byte)gender);
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
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AppearanceUpdate, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        public override System.Collections.Generic.Dictionary<string, byte[]> Globals
        {
            get
            {
                return ScriptGlobals;
            }
        }

        public override int Hair
        { 
            get
            { 
                return hair; 
            } 
            set 
            { 
                hair = (ushort)value;

                PacketWriter Pa = new PacketWriter();
                Pa.Write((byte)'H');
                Pa.Write((ushort)RuntimeID);
                Pa.Write((byte)hair);
            } 
        }

        public override int Level
        {
            get
            {
                return level;
            }
            set
            {
                xp = 0;
                level = value;
                PacketWriter Pa = null;

                // Tell this player if actor is human
		        if(RNID > 0)
                {
                    Pa = new PacketWriter();
                    Pa.Write(GCLID);
                    Pa.Write((byte)'U');
                    Pa.Write((ushort)level);

                    RCEnet.Send(RNID, MessageTypes.P_XPUpdate, Pa.ToArray(), true);
                }

		        // Tell all other players
                Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((byte)'L');
                Pa.Write((ushort)RuntimeID);
                Pa.Write((ushort)level);
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
                            if (A2Node.Value != null && A2Node.Value != this && A2Node.Value.RNID > 0)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_XPUpdate, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        public override Scripting.Actor Target 
        {
            get 
            {
                return AITarget;
            } 
            set
            { 
                if(!(value is ActorInstance))
                    return;
           
                ActorInstance Actor2 = value as ActorInstance;
                AITarget = Actor2;
                // Set this actor so it can attack and chase its target
                //Actor.Aggressiveness = 3; - template actor
                AIMode = AIModes.AI_Chase;
                /*
                if(Actor2 != null)
                {
                    if(Actor.Aggressiveness != 3 && Actor2.Actor.Aggressiveness != 3)
                    {
                        if(Actor2.FactionRatings[homeFaction] < 150)
                            AITarget = Actor2;
                    }
                }else
                {
                    AITarget = null;
                }
                 */
            }
        }

//         public override global::Scripting.Math.Vector3 Destination
//         {
//             get
//             {
//                 return new Scripting.Math.Vector3(DestX, 0.0f, DestZ);
//             }
//             set
//             {
//                 DestX = value.X;
//                 DestZ = value.Z;
//             }
//         }

        public override Scripting.Actor Leader
        {
            get
            {
                return leader;
            }
            set
            {
                if(!(value is ActorInstance))
                    return;

                if(RNID == -1)
                {
                    // Remove current leader
			        if(leader != null)
                    {
				        --leader.NumberOfSlaves;
				        leader = null;
				        AIMode = AIModes.AI_Wait;
			        }
			        
                    // Set new one, if any
			        ActorInstance NewLeader = value as ActorInstance;
			        if(NewLeader != null)
                    {
                        leader = NewLeader;
				        ++leader.NumberOfSlaves;

				        // Make sure it no longer belongs to any spawn point
				        if(SourceSP > -1)
                        {
                            if(SpawnedSector != null)
    					        --SpawnedSector.Spawned[SourceSP];
					        SourceSP = -1;
				        }
				        AIMode = AIModes.AI_Pet;
			        }else // No leader!
                    {
				        // Assign to first available waypoint
				        bool Found = false;

                        for (int i = 0; i < CurrentSector.Sector.Waypoints.Length; ++i)
                        {
                            PreviousPosition = Position;
                            AIMode = AIModes.AI_Patrol;
                            Destination = CurrentSector.Sector.Waypoints[i].Position;
                            CurrentWaypoint = CurrentSector.Sector.Waypoints[i].ID;
                            Destination.X += Server.Rnd(-5.0f, 5.0f);
                            Destination.Z += Server.Rnd(-5.0f, 5.0f);
                            Destination.FixValues();
                            Found = true;
                            break;
                        }

                        if (!Found)
                        {
                            for (int i = 0; i < ServerArea.Area.Waypoints.Length; ++i)
                            {
                                if (ServerArea.Area.Waypoints[i].Previous < ServerArea.Area.Waypoints.Length)
                                {
                                    PreviousPosition = Position;
                                    AIMode = AIModes.AI_Patrol;
                                    Destination = ServerArea.Area.Waypoints[i].Position;
                                    Destination.X += Server.Rnd(-5.0f, 5.0f);
                                    Destination.Z += Server.Rnd(-5.0f, 5.0f);
                                    Destination.FixValues();
                                    CurrentWaypoint = i;
                                    Found = true;
                                    break;
                                }
                            }
                        }

				        // Die
				        if(!Found)
                            KillActor(null);
			        }
                }
            }
        }

        public override int Reputation
        {
            get
            {
                return reputation;
            }
            set
            {
                reputation = value;
                if(RNID > 0)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(0);
                    Pa.Write((byte)'R');
                    Pa.Write((ushort)RuntimeID);
                    Pa.Write((ushort)reputation);
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
        }

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

                if (CurrentSector == null)
                    return;

                // Set name
                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((ushort)(RuntimeID));
                Pa.Write((byte)(name.Length));
                Pa.Write(name, false);
                Pa.Write(tag, false);
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

        public override string Tag
        {
            get
            {
                return tag; 
            }
            set
            {
                tag = value;
                Name = name;
            }
        }

        public override int ActorGroup
        {
            get
            {
                return TeamID;
            }
            set
            {
                TeamID = value;
            }
        }
        
        public override int Money
        {
            get
            {
                return Gold;
            }
            set
            {
                int Change = value - Gold;
                Gold = value;
                if(Gold < 0)
                    Gold = 0;

                if(RNID > 0)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(GCLID);

                    if(Change > 0)
                    {    
                        Pa.Write((byte)'U');
                        Pa.Write(Change);
                    }else
                    {
                        Pa.Write((byte)'D');
                        Pa.Write(Math.Abs(Change));
                    }

                    RCEnet.Send(RNID, MessageTypes.P_GoldChange, Pa.ToArray(), true);
                }
            }
        }

        public override int XP
        { 
            get 
            {
                return xp; 
            }
        }

        public override int XPMultiplier
        {
            get
            { 
                return Actor.XPMultiplier;
            }
        }

        public override ZoneInstance Zone 
        { 
            get 
            { 
                return ServerArea; 
            } 
        }

        public override Scripting.Actor Mount 
        { 
            get
            { 
                return mount;
            } 
        }

        public override Scripting.Actor Rider 
        {
            get
            { 
                return rider; 
            } 
        }

        public override bool Outdoors 
        {
            get 
            { 
                return ServerArea.Area.Outdoors; 
            } 
        }

        public override bool UnderWater
        {
            get
            {
                return Underwater != 0; 
            } 
        }
        
        public override uint BaseActorID
        {
            get
            {
                return this.Actor.ID;
            } 
        }
        
        public override bool Human
        {
            get
            {
                return RNID > -1;
            }
        }
        
        public override string Class
        {
            get
            {
                return Actor.Class;
            }
        }
        
        public override string Race
        {
            get
            {
                return Actor.Race;
            }
        }
        
        public override string HomeFaction
        {
            get
            {
                return Faction.Factions[homeFaction].Name;
            }
            set
            {
                for(int i = 0; i < Faction.Factions.Length; ++i)
                {
                    if(Faction.Factions[i].Name.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        homeFaction = i;
                        break;
                    }
                }
            }
        }

        public override string AccountEmail
        {
            get
            {
                return accountEmail;
            }
        }

        public override string AccountName
        {
            get
            {
                return accountName;
            }
        }
        
        public override bool InGame
        {
            get
            {
                return RNID > 0;
            }
        }
        
        public override bool Banned
        {
            get
            {
                return accountBanned;
            }
            set
            {
                accountBanned = value;
            }
        }

        public override bool GM
        {
           get
           {
               return accountDM;
           }
           set
           {
               accountDM = value;
           }
        }


        public override PartyInstance GetCurrentParty()
        {
            return PartyID as PartyInstance;
        }

        #endregion

        #region Attributes
        public override void SetAttribute(string name, int value)
        {
            int Attr = Attribute.FindAttribute(name);
		    if(Attr > -1)
            {
			    // Important attribute, tell everyone
			    if(Attr == Server.HealthStat || Attr == Server.SpeedStat || Attr == Server.EnergyStat)
                {
				    UpdateAttribute(Attr, value);

					// Death - Don't do death check cause it causes death twice in combat and other things. Also doesn't allow knowledge of killer. - BEN
				    //if(Attributes.Value[Server.HealthStat] <= 0)
                       // KillActor(null);
			    }else // Unimportant attribute, only tell specific player (if it is a human player)
                {
				    Attributes.Value[Attr] = value;
				    if(Attributes.Value[Attr] > Attributes.Maximum[Attr])
					    Attributes.Value[Attr] = Attributes.Maximum[Attr];
				    
				    if(RNID > 0)
                    {
                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(GCLID);
                        Pa.Write((byte)'A');
                        Pa.Write((ushort)RuntimeID);
                        Pa.Write((byte)Attr);
                        Pa.Write((ushort)Attributes.Value[Attr]);

                        RCEnet.Send(RNID, MessageTypes.P_StatUpdate, Pa.ToArray(), true);
				    }
			    }
            }
        }

        public override int GetAttribute(string name)
        {
            int Attr = Attribute.FindAttribute(name);
            if (Attr > -1)
                return Attributes.Value[Attr];
            return 0;
        }

        public override void SetAttributeMax(string name, int value)
        {
            int Attr = Attribute.FindAttribute(name);
            if (Attr > -1)
            {
                // If its important, tell everyone
                if (Attr == Server.HealthStat || Attr == Server.SpeedStat || Attr == Server.EnergyStat)
                {
                    UpdateAttributeMax(Attr, value);
                }
                else // Unimportant attribute, only tell specific player (if its a human player)
                {
                    Attributes.Maximum[Attr] = value;
                    if (RNID > 0)
                    {
                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(GCLID);
                        Pa.Write((byte)'M');
                        Pa.Write((ushort)RuntimeID);
                        Pa.Write((byte)Attr);
                        Pa.Write((ushort)Attributes.Maximum[Attr]);

                        RCEnet.Send(RNID, MessageTypes.P_StatUpdate, Pa.ToArray(), true);
                    }
                }
            }
        }

        public override int GetAttributeMax(string name)
        {
            int Attr = Attribute.FindAttribute(name);
            if (Attr > -1)
                return Attributes.Maximum[Attr];
            return 0;
        }

        public override void SetResistance(string name, int value)
        {
            int Attr = DamageType.FindDamageType(name);
            if (Attr > -1)
                Resistances[Attr] = value;
        }
        
        public override int GetResistance(string name)
        {
            int Attr = DamageType.FindDamageType(name);
            if (Attr > -1)
                return Resistances[Attr];
            return 0;
        }

        public override int GetResistance(int name)
        {
            DamageType damageType = DamageType.DamageTypes[name];
            int attr = DamageType.FindDamageType(damageType.Type);
            if (attr > -1)
                return attr;

            return 0;
        }

        #endregion

        #region Position
        public override global::Scripting.Math.SectorVector GetPosition()
        {
            return Position;
        }

        public override void SetPosition(global::Scripting.Math.SectorVector position)
        {
            SetPosition(position, false, false);
        }

        public override void SetPosition(global::Scripting.Math.SectorVector position, bool moveCamera)
        {
            SetPosition(position, moveCamera, false);
        }

        public override void SetPosition(global::Scripting.Math.SectorVector position, bool moveCamera, bool useCollision)
        {
            if (Position.SectorX != position.SectorX || Position.SectorZ != position.SectorZ)
            {
                ServerArea.ActorChangedSector(this);
            }

            Position = position;
            Destination = position;

            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((byte)'M');
            Pa.Write((ushort)RuntimeID);
            Pa.Write(Position);
            Pa.Write((byte)(moveCamera ? 1 : 0));
            Pa.Write((byte)(useCollision ? 1 : 0));
            byte[] PaA = Pa.ToArray();

            // Notify people in current sector range of the move
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
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_RepositionActor, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }
        }
        #endregion

        #region General Methods
        public override int PartyMembersCount()
        {
            return PartyID.Members;
        }

     

        public override Scripting.Actor PartyMember(int index)
        {
            if (index < PartyID.Members)
            {
                int Count = 0;
                for (int i = 0; i < 8; ++i)
                {
                    if (PartyID.Players[i] != null && PartyID.Players[i] != this)
                    {
                        ++Count;
                        if (Count == index)
                            return PartyID.Players[i];
                    }
                }
            }

            return null;
        }

        public override void ChangeMoney(int amount)
        {
            Gold += amount;
            if (Gold < 0)
                Gold = 0;

            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);

                if (amount > 0)
                {
                    Pa.Write((byte)'U');
                    Pa.Write(amount);
                }
                else
                {
                    Pa.Write((byte)'D');
                    Pa.Write(Math.Abs(amount));
                }

                RCEnet.Send(RNID, MessageTypes.P_GoldChange, Pa.ToArray(), true);
            }
        }

        public override Scripting.ItemInstance Backpack(ItemSlot slot)
        {
            return Inventory.Items[(int)slot];
        }
        
        public override int CountPets()
        {
            return NumberOfSlaves;
        }

        public override bool HasEffect(string effect)
        {
            foreach (ActorEffect AE in ActorEffects)
            {
                if (AE.Name.Equals(effect, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public override void CallForHelp()
        {
            AICallForHelp();
        }

        public override float DistanceFrom(Scripting.Actor other)
        {
            return (float)Math.Sqrt(DistanceFromSq(other));
        }

        public override float DistanceFromSq(Scripting.Actor other)
        {
            if (other == null || !(other is ActorInstance))
                return 0.0f;

            ActorInstance Actor2 = other as ActorInstance;
            Scripting.Math.SectorVector DistVec = Position - Actor2.Position;
            float XDist = (((float)DistVec.SectorX) * Scripting.Math.SectorVector.SectorSize) + DistVec.X;
            float ZDist = (((float)DistVec.SectorZ) * Scripting.Math.SectorVector.SectorSize) + DistVec.Z;
            float YDist = DistVec.Y;
            XDist *= XDist;
            YDist *= YDist;
            ZDist *= ZDist;

            return XDist + YDist + ZDist;
        }

        public override void AddEffect(string effectName, string attributeName, int attributeValue, int timer, int iconID)
        {
            ActorEffect FoundAE = null;
            foreach (ActorEffect AE in ActorEffects)
            {
                if (AE.Name.Equals(effectName, StringComparison.CurrentCultureIgnoreCase))
                {
                    FoundAE = AE;
                    break;
                }
            }

            if (FoundAE == null)
            {
                FoundAE = new ActorEffect();
                FoundAE.Name = attributeName;
                FoundAE.IconTexID = (ushort)iconID;
                if (RNID > 0)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(GCLID);
                    Pa.Write((byte)'A');
                    Pa.Write(FoundAE.AllocID);
                    Pa.Write((ushort)FoundAE.IconTexID);
                    Pa.Write(FoundAE.Name, false);
                    RCEnet.Send(RNID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                }
            }

            FoundAE.CreatedTime = Server.MilliSecs();
            FoundAE.Length = (uint)timer * 1000;
            int Att = Attribute.FindAttribute(attributeName);
            if (Att > -1)
            {
                int Old = FoundAE.Attributes.Value[Att];
                FoundAE.Attributes.Value[Att] = attributeValue;
                Attributes.Value[Att] += FoundAE.Attributes.Value[Att] - Old;
                if (RNID > 0)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(GCLID);
                    Pa.Write((byte)'E');
                    Pa.Write((byte)Att);
                    Pa.Write(FoundAE.Attributes.Value[Att] - Old);

                    RCEnet.Send(RNID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                }
                
            }
        }

        public override void RemoveEffect(string effectName)
        {
            ActorEffect FoundAE = null;
            foreach (ActorEffect AE in ActorEffects)
            {
                if (AE.Name.Equals(effectName, StringComparison.CurrentCultureIgnoreCase))
                {
                    FoundAE = AE;
                    break;
                }
            }

            if (FoundAE != null)
            {
                if (RNID > 0)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(GCLID);
                    Pa.Write((byte)'R');
                    Pa.Write(FoundAE.AllocID);
                    for (int i = 0; i < 40; ++i)
                        Pa.Write(FoundAE.Attributes.Value[i]);
                    RCEnet.Send(RNID, MessageTypes.P_ActorEffect, Pa.ToArray(), true);
                }

                for (int i = 0; i < 40; ++i)
                    Attributes.Value[i] -= FoundAE.Attributes.Value[i];

                ActorEffects.Remove(FoundAE);
            }
        }

        public override void Animate(string animation, float speed, bool fixedSpeed)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)RuntimeID);
            Pa.Write((byte)(fixedSpeed ? 1 : 0));
            Pa.Write(speed);
            Pa.Write(animation, false);
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
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AnimateActor, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }
        }

        public override void Change(uint newID)
        {
            // Check ID is valid
            bool Found = false;
            foreach (RCPServer.Actor AT in RCPServer.Actor.Actors.Values)
            {
                if (AT != null && AT.ID == newID)
                {
                    Found = true;
                    break;
                }
            }

            if (Found)
            {
                Actor = RCPServer.Actor.Actors[newID];
                if (Actor.Genders == 2 && Gender != 1)
                    Gender = 1;
                if ((Actor.Genders == 1 || Actor.Genders == 3) && Gender != 0)
                    Gender = 0;

                // Tell other players in the area if actor is online
                if (RNID > 0)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(0);
                    Pa.Write((byte)'C');
                    Pa.Write((ushort)RuntimeID);
                    Pa.Write((ushort)newID);
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
                                    RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_AppearanceUpdate, PaA, true);
                                }

                                A2Node = A2Node.Next;
                            }
                        }
                    }
                }
            }
            
        }

        public override void FireProjectile(Scripting.Actor target, string projectileName)
        {
            if (target == null)
                return;
            if (!(target is ActorInstance))
                return;

            Projectile P = Projectile.Find(projectileName);

            ActorInstance A2 = target as ActorInstance;

            FireProjectile(P, A2);
        }

        public override void GiveKillXP(Scripting.Actor target)
        {
            if (target == null)
                return;

            if(!(target is ActorInstance))
                return;

            ActorInstance Actor2 = target as ActorInstance;

            int Diff = Actor2.Level - Level;
            if (Diff < 1)
                Diff = 1;
            int NewXP = (Diff * Actor2.Actor.XPMultiplier) + Server.Random.Next(0, 20);
            GiveXP(NewXP);
        }

        public override void GiveXP(int amount)
        {
            GiveXP(amount, true);
        }
        
        public override void Kill()
        {
            Kill(null);
        }

        public override void Kill(Scripting.Actor killer)
        {
            Attributes.Value[Server.HealthStat] = 0;
            KillActor(killer as ActorInstance);
        }

        public override void Rotate(float yaw)
        {
            Yaw = yaw;
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((byte)'R');
            Pa.Write((ushort)RuntimeID);
            Pa.Write(Yaw);
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
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_RepositionActor, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }
        }

        public override void UpdateXPBar(int amount)
        {
            XPBarLevel = amount;
            PacketWriter Pa = new PacketWriter();
            Pa.Write(GCLID);
            Pa.Write((byte)'B');
            Pa.Write(amount);

            if (RNID > 0)
                RCEnet.Send(RNID, MessageTypes.P_XPUpdate, Pa.ToArray(), true);
        }

        public override bool Warp(string zoneName, string portalName)
        {
            return Warp(zoneName, portalName, 0);
        }

        public override bool Warp(string zoneName, string portalName, int instanceNumber)
        {
            Area WarpTo = null;
            foreach (Area Ar in RCPServer.Area.AreaList)
            {
                if (Ar.Name.Equals(zoneName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WarpTo = Ar;
                    break;
                }
            }

            if (WarpTo == null)
                return false;

            int Portal = 0;
            for (int i = 0; i < WarpTo.Portals.Length; ++i)
            {
                if (WarpTo.Portals[i].Name.Equals(portalName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Portal = i;
                    break;
                }
            }

            SetArea(WarpTo, instanceNumber, -1, Portal);
            return true;
        }

        public override void PlaySound(int soundID)
        {
            PlaySound(soundID, false);
        }

        public override void PlaySound(int soundID, bool playGlobal)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)soundID);
            Pa.Write((ushort)RuntimeID);
            byte[] PaA = Pa.ToArray();

            // Play to all if we should
            if (playGlobal)
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
                            if (A2Node.Value != null && A2Node.Value.RNID > 0)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_Sound, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
            else if(RNID > 0) // Play to single person only
            {
                //TODO: Use HostToNetworkOrder
                BitConverter.GetBytes(GCLID).CopyTo(PaA, 0);
                RCEnet.Send(RNID, MessageTypes.P_Sound, PaA, true);
            }
        }

        public override void PlaySpeech(SpeechType type)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)type);
            Pa.Write((ushort)RuntimeID);
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
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_Speech, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }

        }

        public override void BubbleOutput(string message)
        {
            BubbleOutput(message, System.Drawing.Color.White);
        }

        public override void BubbleOutput(string message, System.Drawing.Color color)
        {
            if (ServerArea != null)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(0);
                Pa.Write((ushort)RuntimeID);
                Pa.Write((byte)color.R);
                Pa.Write((byte)color.G);
                Pa.Write((byte)color.B);
                Pa.Write(message, false);
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
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_BubbleMessage, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
        }

        public override void Output(string message)
        {
            Output(0, message, System.Drawing.Color.White);
        }

        public override void Output(byte tabID, string message)
        {
            Output(tabID, message, System.Drawing.Color.White);
        }

        public override void Output(string message, System.Drawing.Color color)
        {
            Output(0, message, color);
        }

        public override void Output(byte tabID, string message, System.Drawing.Color color)
        {
            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write((byte)tabID);
                Pa.Write((byte)250);
                Pa.Write((byte)color.R);
                Pa.Write((byte)color.G);
                Pa.Write((byte)color.B);
                Pa.Write(message, false);

                RCEnet.Send(RNID, MessageTypes.P_ChatMessage, Pa.ToArray(), true);
            }
        }

        public override void CreateChatTab(byte id, string name, int width)
        {
            if(RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write((byte)'O');
                Pa.Write((byte)id);
                Pa.Write((byte)(width > 255 ? 255 : width));
                Pa.Write((byte)name.Length);
                Pa.Write(name, false);

                RCEnet.Send(RNID, MessageTypes.P_ChatTab, Pa.ToArray(), true);
            }
        }

        public override void RemoveChatTab(byte id)
        {
            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write((byte)'C');
                Pa.Write((byte)id);

                RCEnet.Send(RNID, MessageTypes.P_ChatTab, Pa.ToArray(), true);
            }
        }

        public override void SwitchToTab(byte id)
        {
            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write((byte)'S');
                Pa.Write((byte)id);

                RCEnet.Send(RNID, MessageTypes.P_ChatTab, Pa.ToArray(), true);
            }
        }

        public override void GiveItem(string itemName, int amount)
        {
            // Find the requested item
            foreach (Item It in Item.Items.Values)
            {
                if (It == null)
                    continue;

                if (It.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
                {
                    GiveItemImpl(It, amount);
                    return;
                }
            }
        }

        public override void GiveItem(ushort itemID, int amount)
        {
            //if (itemID >= Item.Items.Length || itemID == 65535) - ID's can be up to uint.max now - Ben
                //return;

            if (Item.Items[itemID] != null)
                GiveItemImpl(Item.Items[itemID], amount);
        }

        private void GiveItemImpl(Item It, int amount)
        { 
            // Give
            if (amount > 0)
            {
                // Human
                if (RNID > 0)
                {
                    // Create the item
                    ItemInstance II = ItemInstance.CreateItemInstance(It);
                    II.Assignment = amount;
                    II.AssignTo = this;
                    II.SendingToClient();

                    // Ask client to specify a slot to put it in
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write(GCLID);
                    Pa.Write((byte)'G');
                    Pa.Write(II.AllocID);
                    Pa.Write((ushort)It.ID);
                    Pa.Write((ushort)II.Assignment);

                    RCEnet.Send(RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);
                }
                else // AI
                {
                    ItemInstance II = ItemInstance.CreateItemInstance(It);

                    for (int i = 0; i <= 45; ++i)
                    {
                        if (Inventory.Items[i] == null || (II.Identical(Inventory.Items[i]) && II.Item.Stackable == true && i >= (int)ItemSlots.Backpack))
                        {
                            if (It.SlotsMatch((byte)i) && Actor.HasSlot((byte)It.SlotType, It))
                            {
                                // Only put one item in this slot if it is an equipped slot
                                int ThisAmount = amount;
                                if (i < 45)
                                    ThisAmount = 1;

                                // Put in slot
                                if (Inventory.Items[i] != null)
                                {
                                    Inventory.Items[i].ClientInstanceRemoved();
                                    Inventory.Items[i] = null;
                                }
                                else
                                {
                                    Inventory.Amounts[i] = 0;
                                }
                                Inventory.Items[i] = II;
                                Inventory.Amounts[i] = Inventory.Amounts[i] + ThisAmount;

                                // Visual stuff
                                if (i == (int)ItemSlots.Weapon || i == (int)ItemSlots.Shield || i == (int)ItemSlots.Hat || i == (int)ItemSlots.Chest)
                                    SendEquippedUpdate();

                                // If all items have been placed, exit loop
                                amount -= ThisAmount;
                                if (amount == 0)
                                    break;
                            }
                        }
                    }
                } // End AI Check
            }
            else // Take
            {
                amount = Math.Abs(amount);
                for (int i = 0; i <= 45; ++i)
                {
                    if (Inventory.Items[i] != null)
                    {
                        if (Inventory.Items[i].Item == It)
                        {
                            int AmountTaken = 0;

                            // Delete item
                            if (Inventory.Amounts[i] <= amount)
                            {
                                AmountTaken = Inventory.Amounts[i];
                                amount = amount - Inventory.Amounts[i];
                                Inventory.Items[i].ClientInstanceRemoved();
                                Inventory.Items[i] = null;
                                Inventory.Amounts[i] = 0;
                            }
                            else
                            {
                                Inventory.Amounts[i] -= amount;
                                AmountTaken = amount;
                                amount = 0;
                            }

                            // Tell player if required
                            if (RNID > 0)
                            {
                                PacketWriter Pa = new PacketWriter();
                                Pa.Write(GCLID);
                                Pa.Write((byte)'T');
                                Pa.Write((byte)i);
                                Pa.Write((ushort)AmountTaken);

                                RCEnet.Send(RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);
                            }

                            // Update equipment if required
                            if (i == (int)ItemSlots.Weapon || i == (int)ItemSlots.Shield || i == (int)ItemSlots.Hat || i == (int)ItemSlots.Chest)
                                SendEquippedUpdate();

                            if (amount == 0)
                                break;
                        }
                    }
                }
            }

        }

        public override int HasItem(string itemName)
        {
            return Inventory.HasItem(itemName);
        }

        public override int HasItem(ushort itemID)
        {
            return Inventory.HasItem(itemID);
        }

        public override void ChangeFactionRating(string factionName, int amount)
        {
            for (int i = 0; i < Faction.Factions.Length; ++i)
            {
                if (Faction.Factions[i].Name.Equals(factionName, StringComparison.CurrentCultureIgnoreCase))
                {
                    FactionRatings[i] += 100;
                    if (FactionRatings[i] < 0)
                        FactionRatings[i] = 0;
                    else if (FactionRatings[i] > 200)
                        FactionRatings[i] = 200;
                    break;
                }
            }

            return;
        }

        public override void SetFactionRating(string factionName, int rating)
        {
            for (int i = 0; i < Faction.Factions.Length; ++i)
            {
                if (Faction.Factions[i].Name.Equals(factionName, StringComparison.CurrentCultureIgnoreCase))
                {
                    FactionRatings[i] = rating + 100;
                    if (FactionRatings[i] < 0)
                        FactionRatings[i] = 0;
                    else if (FactionRatings[i] > 200)
                        FactionRatings[i] = 200;
                    break;
                }
            }
        }

        public override int GetFactionRating(string factionName)
        {
            for (int i = 0; i < Faction.Factions.Length; ++i)
            {
                if (Faction.Factions[i].Name.Equals(factionName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return FactionRatings[i] - 100;
                }
            }

            return 0;
        }

        public override void CreateQuest(string name, string status, System.Drawing.Color statusColor)
        {
            if (RNID > 0)
            {
                int FreeSpace = -1;
                bool AlreadyExists = false;
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Length == 0)
                    {
                        FreeSpace = i;
                    }
                    else if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        AlreadyExists = true;
                        break;
                    }
                }

                if (AlreadyExists == false && FreeSpace > -1)
                {
                    PacketWriter Pa = new PacketWriter();
                    Pa.Write((byte)statusColor.R);
                    Pa.Write((byte)statusColor.G);
                    Pa.Write((byte)statusColor.B);
                    Pa.Write(status, false);
                    byte[] PaA = Pa.ToArray();

                    QuestLog.EntryName[FreeSpace] = name;
                    QuestLog.EntryStatus[FreeSpace] = PaA;

                    PacketWriter Pa1 = new PacketWriter();
                    Pa1.Write(GCLID);
                    Pa1.Write((byte)'N');
                    Pa1.Write((byte)name.Length);
                    Pa1.Write(name, false);
                    Pa1.Write((ushort)PaA.Length);
                    Pa1.Write(PaA, 0);

                    RCEnet.Send(RNID, MessageTypes.P_QuestLog, Pa1.ToArray(), true);
                }
            }
        }

        public override void RemoveQuest(string name)
        {
            if (RNID > 0)
            {
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        QuestLog.EntryName[i] = "";
                        QuestLog.EntryStatus[i] = null;

                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(GCLID);
                        Pa.Write((byte)'D');
                        Pa.Write(name, false);

                        RCEnet.Send(RNID, MessageTypes.P_QuestLog, Pa.ToArray(), true);
                        return;
                    }
                }
            }
        }

        public override void CompleteQuest(string name)
        {
            if (RNID > 0)
            {
                byte[] NewStatus = new byte[] { 255, 225, 100, 254 };
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        QuestLog.EntryStatus[i] = NewStatus;

                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(GCLID);
                        Pa.Write((byte)'U');
                        Pa.Write((byte)name.Length);
                        Pa.Write(name, false);
                        Pa.Write((ushort)4);
                        Pa.Write(NewStatus, 0, 4);

                        RCEnet.Send(RNID, MessageTypes.P_QuestLog, Pa.ToArray(), true);
                        break;
                    }
                }
            }
        }

        public override bool QuestComplete(string name)
        {
            if (RNID > 0)
            {
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        byte[] Data = QuestLog.EntryStatus[i];
                        if (Data[0] == 255 || Data[1] == 225 || Data[2] == 100 || Data[3] == 254)
                            return true;
                        return false;
                    }
                }
            }

            return false;
        }

        public override string QuestStatus(string name)
        {
            if (RNID > 0)
            {
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        PacketReader Pa = new PacketReader(QuestLog.EntryStatus[i]);
                        Pa.ReadByte();
                        Pa.ReadByte();
                        Pa.ReadByte();
                        return Pa.ReadString(Pa.Length - Pa.Location);
                    }
                }
            }

            return "";
        }

        public override byte[] GetQuestData(string name)
        {
            if (RNID > 0)
            {
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return QuestLog.EntryData[i];
                    }
                }
            }

            return new byte[0];
        }

        public override void SetQuestData(string name, byte[] data)
        {
            if (RNID > 0)
            {
                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        QuestLog.EntryData[i] = data;
                    }
                }
            }
        }

        public override void UpdateQuest(string name, string status, System.Drawing.Color statusColor)
        {
            if (RNID > 0)
            {
                PacketWriter StatusWriter = new PacketWriter();
                StatusWriter.Write((byte)statusColor.R);
                StatusWriter.Write((byte)statusColor.G);
                StatusWriter.Write((byte)statusColor.B);
                StatusWriter.Write(status, false);
                byte[] StatusWriterArray = StatusWriter.ToArray();

                for (int i = 0; i < 500; ++i)
                {
                    if (QuestLog.EntryName[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        QuestLog.EntryStatus[i] = StatusWriterArray;
                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(GCLID);
                        Pa.Write((byte)'U');
                        Pa.Write((byte)name.Length);
                        Pa.Write(name, false);
                        Pa.Write((ushort)StatusWriterArray.Length);
                        Pa.Write(StatusWriterArray, 0);

                        RCEnet.Send(RNID, MessageTypes.P_QuestLog, Pa.ToArray(), true);
                        break;
                    }
                }
            }
        }
        #endregion

        #region Callbacks
        public override Scripting.ActorDialog OpenDialog(Scripting.Actor contextActor, string titleText)
        {
            return OpenDialog(contextActor, titleText, 65535);
        }

        public override Scripting.ActorDialog OpenDialog(Scripting.Actor contextActor, string titleText, int backgroundID)
        {
            if (contextActor != null && (contextActor as ActorInstance) == null)
                return null;
            ActorInstance ContextActor = contextActor != null ? contextActor as ActorInstance : null;

            // New instance
            ActorDialog D = new ActorDialog(this, ContextActor, titleText, (ushort)backgroundID);

            // Add to both lists
            if(ContextActor != null)
                ContextActor.Dialogs.AddLast(D);
            this.Dialogs.AddLast(D);

            return D;
        }

        public override Scripting.WaitItemRequest CreateWaitItemRequest(string itemName, int itemCount)
        {
            RCPServer.WaitItemRequest Request = new RCPServer.WaitItemRequest(itemName, (ushort)itemCount);

            WaitItemRequests.AddLast(Request);
            return Request;
        }

        public override Scripting.WaitItemRequest CreateWaitItemRequest(string scriptName, string methodName, string itemName, int itemCount)
        {
            RCPServer.WaitItemRequest Request = new RCPServer.WaitItemRequest(scriptName, methodName, itemName, (ushort)itemCount);

            WaitItemRequests.AddLast(Request);
            return Request;
        }

        public override Scripting.WaitItemRequest[] GetWaitItemRequests()
        {
            WaitItemRequest[] Out = new WaitItemRequest[WaitItemRequests.Count];
            WaitItemRequests.CopyTo(Out, 0);

            return Out;
        }

        public override Scripting.WaitKillRequest CreateWaitKillRequest(uint actorID, int killCount)
        {
            RCPServer.WaitKillRequest Request = new RCPServer.WaitKillRequest((ushort)actorID, (ushort)killCount);

            WaitKillRequests.AddLast(Request);
            return Request;
        }

        public override Scripting.WaitKillRequest CreateWaitKillRequest(string scriptName, string methodName, uint actorID, int killCount)
        {
            RCPServer.WaitKillRequest Request = new RCPServer.WaitKillRequest(scriptName, methodName, (ushort)actorID, (ushort)killCount);

            WaitKillRequests.AddLast(Request);
            return Request;
        }

        public override Scripting.WaitKillRequest[] GetWaitKillRequests()
        {
            WaitKillRequest[] Out = new WaitKillRequest[WaitKillRequests.Count];
            WaitKillRequests.CopyTo(Out, 0);

            return Out;
        }


        public override Scripting.WaitSpeakRequest CreateWaitSpeakRequest(string zoneName, string actorName)
        {
            RCPServer.WaitSpeakRequest Request = new RCPServer.WaitSpeakRequest(zoneName, actorName);

            WaitSpeakRequests.AddLast(Request);
            return Request;
        }


        public override Scripting.WaitSpeakRequest CreateWaitSpeakRequest(string scriptName, string methodName, string zoneName, string actorName)
        {
            RCPServer.WaitSpeakRequest Request = new RCPServer.WaitSpeakRequest(scriptName, methodName, zoneName, actorName);

            WaitSpeakRequests.AddLast(Request);
            return Request;
        }

        public override Scripting.WaitSpeakRequest[] GetWaitSpeakRequests()
        {
            WaitSpeakRequest[] Out = new WaitSpeakRequest[WaitSpeakRequests.Count];
            WaitSpeakRequests.CopyTo(Out, 0);

            return Out;
        }


        public override Scripting.WaitTimeRequest CreateWaitTimeRequest(int hour, int minute)
        {
            RCPServer.WaitTimeRequest Request = new RCPServer.WaitTimeRequest(hour, minute);

            WaitTimeRequests.AddLast(Request);
            return Request;
        }


        public override Scripting.WaitTimeRequest CreateWaitTimeRequest(string scriptName, string methodName, int hour, int minute)
        {
            RCPServer.WaitTimeRequest Request = new RCPServer.WaitTimeRequest(scriptName, methodName, hour, minute);

            WaitTimeRequests.AddLast(Request);
            return Request;
        }

        public override Scripting.WaitTimeRequest[] GetWaitTimeRequests()
        {
            WaitTimeRequest[] Out = new WaitTimeRequest[WaitTimeRequests.Count];
            WaitTimeRequests.CopyTo(Out, 0);

            return Out;
        }
        #endregion

        #region Effects
        public override Scripting.ProgressBar CreateProgressBar(System.Drawing.Color color, float x, float y, float width, float height, int maxiumum, int value)
        {
            return CreateProgressBar(color, x, y, width, height, maxiumum, value, "");
        }

        public override Scripting.ProgressBar CreateProgressBar(System.Drawing.Color color, float x, float y, float width, float height, int maxiumum, int value, string label)
        {
            ProgressBar P = new ProgressBar(this, color, x, y, width, height, maxiumum, value, label);

            // Add to list
            this.ProgressBars.AddLast(P);

            return P;
        }

        public override void CreateEmitter(string emitterName, int textureID, int timeLength)
        {
            CreateEmitter(emitterName, textureID, timeLength, new Scripting.Math.Vector3(0, 0, 0), null);
        }

        public override void CreateEmitter(string emitterName, int textureID, int timeLength, global::Scripting.Math.Vector3 offset)
        {
            CreateEmitter(emitterName, textureID, timeLength, offset, null);
        }

        public void CreateEmitter(string emitterName, int textureID, int timeLength, global::Scripting.Math.Vector3 offset, Scripting.Actor displayTo)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)textureID);
            Pa.Write(timeLength);
            Pa.Write((ushort)RuntimeID);
            Pa.Write(offset.X);
            Pa.Write(offset.Y);
            Pa.Write(offset.Z);
            byte[] PaA = Pa.ToArray();

            // Show to all actors
            if (displayTo == null)
            {
                // Send to actors in the same zone
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
                                RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_CreateEmitter, PaA, true);
                            }

                            A2Node = A2Node.Next;
                        }
                    }
                }
            }
            else // Display to specific actor
            {
                if (displayTo is ActorInstance)
                {
                    //TODO: Use HostToNetworkOrder
                    BitConverter.GetBytes((displayTo as ActorInstance).GCLID).CopyTo(PaA, 0);
                    RCEnet.Send((displayTo as ActorInstance).RNID, MessageTypes.P_CreateEmitter, PaA, true);
                }
            }
        }

        public override void CreateFloatingNumber(int value, System.Drawing.Color color)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)RuntimeID);
            Pa.Write(value);
            Pa.Write((byte)color.R);
            Pa.Write((byte)color.G);
            Pa.Write((byte)color.B);
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
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_FloatingNumber, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }
        }

        public override void ScreenFlash(System.Drawing.Color color, int timeLength, int textureID)
        {
            if (RNID > 0)
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write(GCLID);
                Pa.Write((byte)color.R);
                Pa.Write((byte)color.G);
                Pa.Write((byte)color.B);
                Pa.Write((byte)color.A);
                Pa.Write(timeLength);
                Pa.Write((ushort)textureID);

                RCEnet.Send(RNID, MessageTypes.P_ScreenFlash, Pa.ToArray(), true);
            }
        }

        public override void SetDestination(Scripting.Math.SectorVector position)
        {
            //base.SetDestination(position);
            AIMode = AIModes.AI_MoveToDestination;
            Destination = position;
        }

        #endregion
        #endregion
    }
}
