using System;
using System.Collections.Generic;
using System.Text;
using Scripting;
using System.Reflection;

namespace RCPServer
{
    public class ScriptingCoreCommands : Scripting.Internals.IScriptingCoreCommands
    {
        public static void Initialize()
        {
            ScriptingCoreCommands Cmd = new ScriptingCoreCommands();
            Scripting.RCScript.SetCommands(Cmd);
            Scripting.Actor.SetCommands(Cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raceName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public uint ActorID(string raceName, string className)
        {
            for (uint i = 0; i < Actor.Actors.Count; ++i)
            {
                if (Actor.Actors[i] != null)
                {
                    if (Actor.Actors[i].Race.Equals(raceName, StringComparison.CurrentCultureIgnoreCase)
                        && Actor.Actors[i].Class.Equals(className, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Actor.Actors[i].ID;
                    }
                }
            }

            return 65535;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="zoneName"></param>
        /// <param name="position"></param>
        /// <param name="interactScript"></param>
        /// <param name="deathScript"></param>
        /// <param name="instanceIndex"></param>
        /// <returns></returns>
        public Scripting.Actor Spawn(uint id, string zoneName, global::Scripting.Math.SectorVector position, string script, int instanceIndex)
        {
            if(position == null)
                return null;

            if(id == 65535 || id >= Actor.Actors.Count || Actor.Actors[id] == null)
            {
                RCPServer.Log.WriteLine(string.Format("Actor.Spawn({0}, {1}:{2}) Failed - Actor ID doesn't exist", id, zoneName, instanceIndex));
                return null;
            }

            position.FixValues();
            Area Ar = Area.Find(zoneName);
            if (Ar == null)
            {
                RCPServer.Log.WriteLine(string.Format("Actor.Spawn({0}, {1}:{2}) Failed - Area not found", id, zoneName, instanceIndex));
                return null;
            }

            if(!Ar.Instances.ContainsKey(instanceIndex))
            {
                RCPServer.Log.WriteLine(string.Format("Actor.Spawn({0}, {1}:{2}) Failed - Instance not found", id, zoneName, instanceIndex));
                return null;
            }

            if (!Ar.Instances[instanceIndex].Active)
            {
                RCPServer.Log.WriteLine(string.Format("Actor.Spawn({0}, {1}:{2}) Failed - Instance isn't on this server", id, zoneName, instanceIndex));
                return null;
            }

            Area.AreaInstance Inst = Ar.Instances[instanceIndex];

            if (position.SectorX >= Ar.Sectors.GetLength(0) || position.SectorZ >= Ar.Sectors.GetLength(1))
            {
                RCPServer.Log.WriteLine(string.Format("Actor.Spawn({0}, {1}:{2}) Failed - Sector not found", id, zoneName, instanceIndex));
                return null;
            }

            Area.InstanceSector Sector = Inst.Sectors[position.SectorX, position.SectorZ];
            if(!Sector.Active)
            {
                RCPServer.Log.WriteLine(string.Format("Actor.Spawn({0}, {1}:{2}) Failed - Sector isn't on this server", id, zoneName, instanceIndex));
                return null;
            }

            ActorInstance AI = ActorInstance.CreateActorInstance(Actor.Actors[(uint)id]);
            AI.SpawnedSector = Sector;
            AI.RNID = -1;
            AI.AssignRuntimeID();
            AI.SetArea(Ar, instanceIndex, -1, -1, position);
            AI.AIMode = Scripting.AIModes.AI_Wait;
            AI.Script = script;
            AI.DeathScript = script;
            if(!string.IsNullOrEmpty(script))
                ScriptManager.Execute(script, "OnSpawn", new object[] { AI }, AI, null);

            return AI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factionName"></param>
        /// <param name="otherFactionName"></param>
        /// <returns></returns>
        public int DefaultFactionRating(string factionName, string otherFactionName)
        {
            for (int i = 0; i < Faction.Factions.Length; ++i)
            {
                if (Faction.Factions[i] != null && Faction.Factions[i].Name.Equals(factionName, StringComparison.CurrentCultureIgnoreCase))
                {
                    for (int j = 0; j < Faction.Factions.Length; ++j)
                    {
                        if (Faction.Factions[j] != null && Faction.Factions[j].Name.Equals(otherFactionName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return Faction.Factions[i].DefaultRatings[j];
                        }
                    }

                    return 0;
                }
            }

            return 0;
        }

        public void PostActorInfoRequest(ActorInfoRequest request)
        {
            if (string.IsNullOrEmpty(request.ActorName))
                throw new ArgumentException("request must contain a valid actor name!");

            // Before a request can be properly sent off, we should check locally (if looking for online/all)
            if (request.RequestState == ActorInfoRequestState.Online || request.RequestState == ActorInfoRequestState.All)
            {
                ActorInstance AI = ActorInstance.FindFromName(request.ActorName);

                // Found! post reply immediately
                if (AI != null)
                {
                    ActorInfo Info = ActorInfo.FromHandle(AI);
                    request.Invoke(Info);
                    return;
                }
            }

            // Send request to master
            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)'N'); // New
            Pa.Write((uint)request.AllocID);
            Pa.Write((byte)request.ActorName.Length);
            Pa.Write(request.ActorName, false);
            Pa.Write((byte)request.RequestState);

            if (Server.MasterConnection != 0)
                RCEnet.Send(Server.MasterConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);

            // Add to internal list, for waiting
            ActorInfo.Requests.AddLast(request);
            request.Posted(Server.MilliSecs());
        }

        public void DispatchMessage(Scripting.Actor actor, string message, byte destinationChannel, int rangeInSectors)
        {
            if (actor == null)
                return;

            ActorInstance AI = actor as ActorInstance;
            if (AI == null)
                return;

            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((byte)destinationChannel);
            //string StrLayout = "<" + AI.Name + "> " + message;
            string StrLayout = message;
            Pa.Write(StrLayout, false);
            byte[] PaA = Pa.ToArray();

            int MinX = (int)AI.CurrentSector.Sector.SectorX - rangeInSectors;
            int MinZ = (int)AI.CurrentSector.Sector.SectorZ - rangeInSectors;
            int MaxX = (int)AI.CurrentSector.Sector.SectorX + rangeInSectors + 1;
            int MaxZ = (int)AI.CurrentSector.Sector.SectorZ + rangeInSectors + 1;

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

                    while(A2Node != null)
                    {
                        if (A2Node.Value != null && A2Node.Value.RNID > 0)
                        {
                            //TODO: Use HostToNetworkOrder
                            BitConverter.GetBytes(A2Node.Value.GCLID).CopyTo(PaA, 0);
                            RCEnet.Send(A2Node.Value.RNID, MessageTypes.P_ChatMessage, PaA, true);
                        }

                        A2Node = A2Node.Next;
                    }
                }
            }

            //RCPServer.Log.WriteLine(StrLayout);
        }

        public bool PostZoneInstanceRequest(ZoneInstanceRequest request)
        {
            if(request == null)
                return false;

            Area Ar = Area.Find(request.ZoneName);
            if (Ar == null)
                return false;

            Ar.CreateInstance(request);

            return true;
        }

        public bool ZoneOutdoors(string zoneName)
        {
            Area Ar = Area.Find(zoneName);
            if (Ar == null)
            {
                Log("ZoneOutdoors failed because zone '" + zoneName + "' does not exist.");
                return false;
            }

            return Ar.Outdoors;
        }


        public void SpawnItem(string itemName, int amount, string zoneName, global::Scripting.Math.SectorVector position, int instanceID)
        {
            Area Ar = Area.Find(zoneName);
            if (Ar == null)
            {
                Log("SpawnItem failed because zone '" + zoneName + "' does not exist.");
                return;
            }

            if (!Ar.Instances.ContainsKey(instanceID))
            {
                Log("SpawnItem failed because '" + zoneName + ":" + instanceID + "' is an invalid instance ID");
                return;
            }

            if (Ar.Instances[instanceID] == null)
            {
                Log("SpawnItem failed because zone '" + zoneName + ":" + instanceID + "' does not exist.");
                return;
            }

            if (Ar.Sectors.GetLength(0) <= position.SectorX || Ar.Sectors.GetLength(1) <= position.SectorZ)
            {
                Log("SpawnItem failed because sector was incorrect!");
                return;
            }

            Item ItemTemplate = Item.Find(itemName);
            if (ItemTemplate == null)
            {
                Log("SpawnItem failed because item '" + itemName + "' does not exist.");
                return;
            }

            DroppedItem D = new DroppedItem(Ar.Instances[instanceID].Sectors[position.SectorX, position.SectorZ]);
            D.Item = ItemInstance.CreateItemInstance(ItemTemplate);
            D.Item.SendingToClient();
            D.Amount = amount;
            D.Position = position;
            D.ServerHandle = Ar.Instances[instanceID];

            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((byte)'D');
            Pa.Write((ushort)D.Amount);
            Pa.Write(D.Position);
            Pa.Write(D.AllocID);
            Pa.Write(D.Item.ToArray());
            byte[] PaA = Pa.ToArray();


            int MinX = (int)position.SectorX - 1;
            int MinZ = (int)position.SectorZ - 1;
            int MaxX = (int)position.SectorX + 2;
            int MaxZ = (int)position.SectorZ + 2;

            if (MinX < 0)
                MinX = 0;
            if (MinZ < 0)
                MinZ = 0;
            if (MaxX > Ar.Sectors.GetLength(0))
                MaxX = Ar.Sectors.GetLength(0);
            if (MaxZ > Ar.Sectors.GetLength(1))
                MaxZ = Ar.Sectors.GetLength(1);

            LinkedListNode<ActorInstance> A2Node;

            for (int z = MinZ; z < MaxZ; ++z)
            {
                for (int x = MinX; x < MaxX; ++x)
                {
                    A2Node = Ar.Instances[instanceID].Sectors[x, z].Players.First;

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


        public byte[] GetSuperGlobal(string name)
        {
            return SuperGlobals.Globals[name];
        }

        public void SetSuperGlobal(string name, byte[] value)
        {
            if (SuperGlobals.Globals.ContainsKey(name))
            {
                SuperGlobals.Globals[name] = value;
            }
            else
            {
                SuperGlobals.Globals.Add(name, value);
            }
        }

        public void CreateStaticEmitter(string zoneName, string emitterName, int textureID, int timeLength, global::Scripting.Math.SectorVector offset)
        {
            PacketWriter Pa = new PacketWriter();
            Pa.Write(0);
            Pa.Write((ushort)textureID);
            Pa.Write(timeLength);
            Pa.Write((ushort)0);
            Pa.Write(offset);
            Pa.Write(emitterName, false);
            byte[] PaA = Pa.ToArray();

            Area Ar = Area.Find(zoneName);
            if (Ar == null)
            {
                Log("CreateStaticEmitter failed because zone '" + zoneName + "' does not exist.");
                return;
            }

            if (Ar.Instances[0] == null)
            {
                Log("CreateStaticEmitter failed because zone '" + zoneName + "' has no instances.");
                return;
            }

            int MinX = (int)offset.SectorX - 1;
            int MinZ = (int)offset.SectorZ - 1;
            int MaxX = (int)offset.SectorX + 2;
            int MaxZ = (int)offset.SectorZ + 2;

            if (MinX < 0)
                MinX = 0;
            if (MinZ < 0)
                MinZ = 0;
            if (MaxX > Ar.Sectors.GetLength(0))
                MaxX = Ar.Sectors.GetLength(0);
            if (MaxZ > Ar.Sectors.GetLength(1))
                MaxZ = Ar.Sectors.GetLength(1);

            LinkedListNode<ActorInstance> A2Node;

            for (int z = MinZ; z < MaxZ; ++z)
            {
                for (int x = MinX; x < MaxX; ++x)
                {
                    A2Node = Ar.Instances[0].Sectors[x, z].Players.First;

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

        public void Log(string message)
        {
            RCPServer.Log.WriteLine(message);
        }
    }
}
