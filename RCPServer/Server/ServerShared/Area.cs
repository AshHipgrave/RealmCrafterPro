using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;
using Scripting.Math;

namespace RCPServer
{
    public partial class Area
    {
        #region Child Classes
        public class ZoneSector
        {
            // Local references to zone data
            // Note: These references are the same as the
            // ones stored in the main zone list, this secondary
            // list is kept for the sake if faster processing.
            public Trigger[] Triggers;
            public Waypoint[] Waypoints;
            public Portal[] Portals;
            public Spawn[] Spawns;
            public ServerWater[] Waters;
            public InteractiveScenery[] Sceneries;

            // Sector position
            public ushort SectorX = 0, SectorZ = 0;

            // Sector assignment ID (As allocated in editor and redirected by MS)
            public byte ServerID = 0;
        }

        public class InstanceSector
        {
            // Reference to the master sector from which it inherits
            public ZoneSector Sector = null;

            // Parent instance
            public AreaInstance Instance = null;

            // Whether sector is active for processing
            public bool Active = false;

            // Surrounding sectors (for gather)
            public InstanceSector Xp, Xm, Zp, Zm;

            // Players in sector
            public LinkedList<ActorInstance> Players = new LinkedList<ActorInstance>();

            // NPCs in sector
            public LinkedList<ActorInstance> Actors = new LinkedList<ActorInstance>();

            // Interactive Sceneries
            public InteractiveSceneryInstance[] SceneryInstances = null;


            // Spawn times + counts
            public uint[] SpawnLast = null;
            public int[] Spawned = null;

            // Dropped Items in sector
            public LinkedList<RCPServer.DroppedItem> DroppedItems = new LinkedList<DroppedItem>();

            public InstanceSector(AreaInstance instance, ZoneSector sector)
            {
                Instance = instance;
                Sector = sector;

                SpawnLast = new uint[sector.Spawns.Length];
                Spawned = new int[sector.Spawns.Length];

                // Initial spawn point times
                for (int i = 0; i < SpawnLast.Length; ++i)
                {
                    SpawnLast[i] = Server.MilliSecs();
                    Spawned[i] = 0;
                }

                // Interactive Scenery Instances
                SceneryInstances = new InteractiveSceneryInstance[sector.Sceneries.Length];
                for (int i = 0; i < sector.Sceneries.Length; ++i)
                {
                    SceneryInstances[i] = new InteractiveSceneryInstance(sector.Sceneries[i], new Dictionary<string,byte[]>());
                    instance.InteractiveSceneryList.Add(SceneryInstances[i]);
                }
            }
        }

        public partial class AreaInstance : Scripting.ZoneInstance
        {
            public uint FirstEmpty = 0;
            public int AllocID = 0;
            public bool Active = false; // False by default! MasterServer will change this.
            public Area Area = null;
            public int ID = 0;
            //public ActorInstance FirstInZone = null;

            public InstanceSector[,] Sectors = null;

            // This list is only accessed on load and save
            public List<InteractiveSceneryInstance> InteractiveSceneryList = new List<InteractiveSceneryInstance>();

            protected static int Allocations = 0;

            public AreaInstance(Area area)
            {
                Area = area; 
  
                int MaxSectorX = area.Sectors.GetLength(0);
                int MaxSectorZ = area.Sectors.GetLength(1);

                Sectors = new InstanceSector[MaxSectorX, MaxSectorZ];

                List<ServerWater> TWaters = new List<ServerWater>();

                for (int z = 0; z < MaxSectorZ; ++z)
                {
                    for (int x = 0; x < MaxSectorX; ++x)
                    {
                        Sectors[x, z] = new InstanceSector(this, area.Sectors[x, z]);
                    }
                }

                for (int z = 0; z < MaxSectorZ; ++z)
                {
                    for (int x = 0; x < MaxSectorX; ++x)
                    {
                        if (x < MaxSectorX - 1)
                            Sectors[x, z].Xp = Sectors[x + 1, z];
                        if (x > 1)
                            Sectors[x, z].Xm = Sectors[x - 1, z];
                        if (z < MaxSectorZ - 1)
                            Sectors[x, z].Zp = Sectors[x, z + 1];
                        if (z > 1)
                            Sectors[x, z].Zm = Sectors[x, z - 1];
                    }
                }
            }

            public void Deserialize(PacketReader reader)
            {
                ushort InterCount = reader.ReadUInt16();

                if (InterCount > 0)
                {
                    for (int i = 0; i < InterCount; ++i)
                    {
                        byte Exists = reader.ReadByte();

                        if (Exists > 0)
                        {
                            Scripting.Math.SectorVector Position = reader.ReadSectorVector();
                            ushort MeshID = reader.ReadUInt16();

                            InteractiveSceneryInstance Inst = SceneryInstanceFromPositionID(ref Position, MeshID);
                            if (Inst != null)
                                Inst.Deserialize(reader);
                            else
                                new InteractiveSceneryInstance(new InteractiveScenery(new SectorVector(), 65535, ""), new System.Collections.Generic.Dictionary<string, byte[]>()).Deserialize(reader);
                            
                        }
                    }
                }

                if (reader.Location < reader.Length)
                {
                    ushort DroppedCount = reader.ReadUInt16();

                    for (int i = 0; i < DroppedCount; ++i)
                    {
                        byte DataLength = reader.ReadByte();
                        byte[] Data = reader.ReadBytes(DataLength);
                        ushort Amount = reader.ReadUInt16();
                        SectorVector Position = reader.ReadSectorVector();

                        if(Position.SectorX >= Sectors.GetLength(0) || Position.SectorZ >= Sectors.GetLength(1))
                            continue;

                        DroppedItem D = new DroppedItem(Sectors[Position.SectorX, Position.SectorZ]);
                        D.Amount = Amount;
                        D.Item = ItemInstance.FromArray(Data);
                        if(D.Item != null)
                            D.Item.SendingToClient();
                        D.Position = Position;
                        D.ServerHandle = this;
                    }
                }
            }

            private InteractiveSceneryInstance SceneryInstanceFromPositionID(ref Scripting.Math.SectorVector position, ushort meshID)
            {
                foreach (InteractiveSceneryInstance Inst in InteractiveSceneryList)
                {
                    if (Inst.Scenery.MeshID == meshID && Inst.Scenery.Position == position)
                        return Inst;
                }

                return null;
            }

            public void ActorLeftSectors(ActorInstance ai, List<InstanceSector> movedFrom)
            {
                PacketWriter Gonep = new PacketWriter();
                Gonep.Write(0);
                Gonep.Write((ushort)ai.RuntimeID);
                byte[] PaRNID = Gonep.ToArray();

                PacketWriter AIGone = new PacketWriter();
                AIGone.Write(ai.GCLID);
                AIGone.Write((ushort)0);
                byte[] PaAIGone = AIGone.ToArray();

                for (int i = 0; i < movedFrom.Count; ++i)
                {
                    InstanceSector Sector = movedFrom[i];

                    // Remove, just in case
                    Sector.Players.Remove(ai);
                    Sector.Actors.Remove(ai);

                    // Send current actor the 'gone' message (if necessary)
                    if (ai.RNID > 0)
                    {
                        foreach (ActorInstance A2 in Sector.Players)
                        {
                            if (A2 != ai)
                            {
                                //TODO: Use HostToNetworkOrder
                                BitConverter.GetBytes((ushort)A2.RuntimeID).CopyTo(PaAIGone, 4);
                                RCEnet.Send(ai.RNID, MessageTypes.P_ActorGone, PaAIGone, true);
                            }
                        }

                        foreach (ActorInstance A2 in Sector.Actors)
                        {
                            //TODO: Use HostToNetworkOrder
                            BitConverter.GetBytes((ushort)A2.RuntimeID).CopyTo(PaAIGone, 4);
                            RCEnet.Send(ai.RNID, MessageTypes.P_ActorGone, PaAIGone, true);
                        }
                    }


                    // Send 'actor gone'
                    foreach (ActorInstance A2 in Sector.Players)
                    {
                        if (A2.RNID > 0)
                        {
                            //TODO: Use HostToNetworkOrder
                            BitConverter.GetBytes(A2.GCLID).CopyTo(PaRNID, 0);
                            RCEnet.Send(A2.RNID, MessageTypes.P_ActorGone, PaRNID, true);
                        }
                    }
                }
            }

            public void ActorEnteredSectors(ActorInstance ai, List<InstanceSector> movedTo)
            {
                byte[] NewGuyA = ai.ToArray();
                PacketWriter NewGuyP = new PacketWriter();
                NewGuyP.Write(0);
                NewGuyP.Write(NewGuyA, 0);
                byte[] NewGuy = NewGuyP.ToArray();

                for (int i = 0; i < movedTo.Count; ++i)
                {
                    InstanceSector Sector = movedTo[i];

                    // Send 'New actor' to existing players, and those players to our player
                    foreach (ActorInstance A2 in Sector.Players)
                    {
                        if (A2 == ai)
                            continue;

                        if (A2.RNID > 0)
                        {
                            //TODO: Use HostToNetworkOrder
                            BitConverter.GetBytes(A2.GCLID).CopyTo(NewGuy, 0);
                            RCEnet.Send(A2.RNID, MessageTypes.P_NewActor, NewGuy, true);
                        }

                        if (ai.RNID > 0)
                        {
                            PacketWriter Pa = new PacketWriter();
                            Pa.Write(ai.GCLID);
                            Pa.Write(A2.ToArray(), 0);

                            RCEnet.Send(ai.RNID, MessageTypes.P_NewActor, Pa.ToArray(), true);
                        }
                    }

                    // Send 'New actor' of NPCs, and those players to our player
                    foreach (ActorInstance A2 in Sector.Actors)
                    {
                        if (ai.RNID > 0)
                        {
                            PacketWriter Pa = new PacketWriter();
                            Pa.Write(ai.GCLID);
                            Pa.Write(A2.ToArray(), 0);

                            RCEnet.Send(ai.RNID, MessageTypes.P_NewActor, Pa.ToArray(), true);
                        }
                    }

                    foreach (DroppedItem D in Sector.DroppedItems)
                    {
                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(ai.GCLID);
                        Pa.Write("D", false);
                        Pa.Write((ushort)(D.Amount));
                        Pa.Write(D.Position);
                        Pa.Write(D.AllocID);
                        Pa.Write(D.Item.ToArray());
                        RCEnet.Send(ai.RNID, MessageTypes.P_InventoryUpdate, Pa.ToArray(), true);
                    }
                }
            }


            public void ActorChangedSector(ActorInstance ai)
            {
                List<InstanceSector> MovedFrom = new List<InstanceSector>();
                List<InstanceSector> MovedTo = new List<InstanceSector>();

                if (ai.Position.SectorX >= Sectors.GetLength(0))
                    ai.Position.SectorX = (ushort)(Sectors.GetLength(0) - 1);
                if (ai.Position.SectorZ >= Sectors.GetLength(1))
                    ai.Position.SectorZ = (ushort)(Sectors.GetLength(1) - 1);

                

                if (ai.CurrentSector != null)
                {
                    ushort PreviousSectorX = ai.CurrentSector.Sector.SectorX;
                    ushort PreviousSectorZ = ai.CurrentSector.Sector.SectorZ;

                    int MinX = ai.Position.SectorX - 1;
                    int MinZ = ai.Position.SectorZ - 1;
                    int MaxX = ai.Position.SectorX + 2;
                    int MaxZ = ai.Position.SectorZ + 2;

                    if (MinX < 0) MinX = 0;
                    if (MinZ < 0) MinZ = 0;
                    if (MaxX > Sectors.GetLength(0))
                        MaxX = Sectors.GetLength(0);
                    if (MaxZ > Sectors.GetLength(1))
                        MaxZ = Sectors.GetLength(1);

                    int PrevMinX = PreviousSectorX - 1;
                    int PrevMinZ = PreviousSectorZ - 1;
                    int PrevMaxX = PreviousSectorX + 1;
                    int PrevMaxZ = PreviousSectorZ + 1;

                    if (PrevMinX < 0) PrevMinX = 0;
                    if (PrevMinZ < 0) PrevMinZ = 0;
                    if (PrevMaxX > Sectors.GetLength(0))
                        PrevMaxX = Sectors.GetLength(0) - 1;
                    if (PrevMaxZ > Sectors.GetLength(1))
                        PrevMaxZ = Sectors.GetLength(1) - 1;

                    for (int z = MinZ; z < MaxZ; ++z)
                    {
                        for (int x = MinX; x < MaxX; ++x)
                        {
                            // Check if the previous range include the new testing sector
                            if (!(x >= PrevMinX && x <= PrevMaxX
                                && z >= PrevMinZ && z <= PrevMaxZ))
                            {
                                // We just moved this this sector, or its suddenly in range
                                MovedTo.Add(Sectors[x, z]);
                            }
                        }
                    }

                    for (int z = PrevMinZ; z <= PrevMaxZ; ++z)
                    {
                        for (int x = PrevMinX; x <= PrevMaxX; ++x)
                        {
                            // Check if the previous range include the new testing sector
                            if (!(x >= MinX && x < MaxX
                                && z >= MinZ && z < MaxZ))
                            {
                                // We just moved this this sector, or its suddenly in range
                                MovedFrom.Add(Sectors[x, z]);
                            }
                        }
                    }

                    // Remove instance from his current sector
                    ai.CurrentSector.Players.Remove(ai);
                }
                else
                {
                    // If the actor was never properly assigned a sector, then we override the sector system
                    // to give it a 'fresh' entry. This will cause the actor to be removed and re-created, but
                    // will result in correct tracking in the future.

                    int MinX = ai.Position.SectorX - 1;
                    int MinZ = ai.Position.SectorZ - 1;
                    int MaxX = ai.Position.SectorX + 2;
                    int MaxZ = ai.Position.SectorZ + 2;

                    if (MinX < 0) MinX = 0;
                    if (MinZ < 0) MinZ = 0;
                    if (MaxX > Sectors.GetLength(0))
                        MaxX = Sectors.GetLength(0);
                    if (MaxZ > Sectors.GetLength(1))
                        MaxZ = Sectors.GetLength(1);

                    int PrevMinX = ai.PreviousPosition.SectorX - 1;
                    int PrevMinZ = ai.PreviousPosition.SectorZ - 1;
                    int PrevMaxX = ai.PreviousPosition.SectorX + 1;
                    int PrevMaxZ = ai.PreviousPosition.SectorZ + 1;

                    if (PrevMinX < 0) PrevMinX = 0;
                    if (PrevMinZ < 0) PrevMinZ = 0;
                    if (PrevMaxX > Sectors.GetLength(0))
                        PrevMaxX = Sectors.GetLength(0);
                    if (PrevMaxZ > Sectors.GetLength(1))
                        PrevMaxZ = Sectors.GetLength(1);

                    for (int z = MinZ; z < MaxZ; ++z)
                    {
                        for (int x = MinX; x < MaxX; ++x)
                        {
                            MovedTo.Add(Sectors[x, z]);
                            MovedFrom.Add(Sectors[x, z]);
                        }
                    }
                }

                ActorLeftSectors(ai, MovedFrom);
                ActorEnteredSectors(ai, MovedTo);

                ai.CurrentSector = Sectors[ai.Position.SectorX, ai.Position.SectorZ];
                if (ai.RNID > 0)
                    ai.CurrentSector.Players.AddLast(ai);
                else
                    ai.CurrentSector.Actors.AddLast(ai);
            }

//             public void UpdateWeather()
//             {
//                 --CurrentWeatherTime;
// 
// 	            // Time to update the weather for this area
// 	            if(CurrentWeatherTime <= 0)
//                 {
// 		            // Get weather from linked area
// 		            if(Area.WeatherLinkArea != null)
//                     {
// 			            CurrentWeatherTime = Area.WeatherLinkArea.Instances[0].CurrentWeatherTime;
// 			            CurrentWeather = Area.WeatherLinkArea.Instances[0].CurrentWeather;
// 		            }else // Choose own weather from probabilities
//                     {
// 			            CurrentWeatherTime = Server.Random.Next(2500, 10000);
// 			            CurrentWeather = 0;
// 			            int NewWeather = Server.Random.Next(1, 100);
// 			            int Min = 0;
// 
// 			            for(int i = 0; i < 5; ++i)
//                         {
// 				            if(Area.WeatherChance[i] > 0)
//                             {
// 					            int Max = Min + Area.WeatherChance[i];
// 					            if(NewWeather >= Min && NewWeather < Max)
//                                 {
//                                     CurrentWeather = (Weather)(i + 1);
//                                     break;
//                                 }
// 					            Min = Max;
// 				            }
// 			            }
// 		            }
// 
// 		            // Inform players in this area
//                     PacketWriter Pa = new PacketWriter();
//                     Pa.Write(0);
//                     Pa.Write(AllocID);
//                     Pa.Write((byte)CurrentWeather);
//                     byte[] PaA = Pa.ToArray();
// 
//                     foreach (InstanceSector Sector in Sectors)
//                     {
//                         LinkedListNode<ActorInstance> AINode = Sector.Players.First;
// 
//                         while (AINode != null)
//                         {
//                             ActorInstance AI = AINode.Value;
//                             AINode = AINode.Next;
// 
//                             if (AI != null && AI.RNID > 0)
//                             {
//                                 //TODO: Use HostToNetworkOrder
//                                 BitConverter.GetBytes(AI.GCLID).CopyTo(PaA, 0);
//                                 RCEnet.Send(AI.RNID, MessageTypes.P_WeatherChange, PaA, true);
//                             }
//                         }
//                     }
// 	            }
//             }

            public static AreaInstance CreateAreaInstance(Area a, int id)
            {
                // New Instance
                AreaInstance AInstance = new AreaInstance(a);
                a.Instances[id] = AInstance;
                AInstance.ID = id;

                ++Allocations;
                AInstance.AllocID = Allocations;

                // Done
                return AInstance;
            }
        }

        public class ServerWater
        {
            public Area Area = null;
            public SectorVector Position = new SectorVector();
            public float Width = 0.0f, Depth = 0.0f;
            public int Damage = 0;
            public int DamageType = 0;

            public bool IsInSector(int x, int z)
            {
                // If our designation is the same, then the result is simple
                if (Position.SectorX == x && Position.SectorZ == z)
                    return true;

                // If we are more than one out, then the object is too big to consider processing anyway
                // Honestly, who needs an object bigger than 768 units?!
                if (Math.Abs(x - Position.SectorX) > 1 || Math.Abs(z - Position.SectorZ) > 1)
                    return false;

                // If 'OK' is true, then its not within the given sector
                bool XOK = false;
                bool ZOK = false;

                if (x > Position.SectorX)
                {
                    XOK = (Position.X > 0.0f);
                }
                else if (x < Position.SectorX)
                {
                    XOK = Position.X < (Server.SectorSize - Width);
                }

                if (z > Position.SectorZ)
                {
                    ZOK = (Position.Z > 0.0f);
                }
                else if (z < Position.SectorZ)
                {
                    ZOK = Position.Z < (Server.SectorSize - Depth);
                }

                return ((XOK == false) && (ZOK == false));
            }
        }

        public class Trigger
        {
            public SectorVector Position = new SectorVector();
            public float Width = 0, Height = 0, Depth = 0;
            public bool IsSquare = false;
            public string ScriptName = "";
            public string Name = "";

            public bool IsInSector(int x, int z)
            {
                // If our designation is the same, then the result is simple
                if (Position.SectorX == x && Position.SectorZ == z)
                    return true;

                // If we are more than one out, then the object is too big to consider processing anyway
                // Honestly, who needs an object bigger than 768 units?!
                if (Math.Abs(x - Position.SectorX) > 1 || Math.Abs(z - Position.SectorZ) > 1)
                    return false;

                // If 'OK' is true, then its not within the given sector
                bool XOK = false;
                bool ZOK = false;

                if (x > Position.SectorX)
                {
                    if (IsSquare)
                        XOK = (Position.X > 0.0f);
                    else
                        XOK = (Position.X > Width);
                }
                else if (x < Position.SectorX)
                {
                    XOK = Position.X < (Server.SectorSize - Width);
                }

                if (z > Position.SectorZ)
                {
                    if (IsSquare)
                        ZOK = (Position.Z > 0.0f);
                    else
                        ZOK = (Position.Z > Width);
                }
                else if (z < Position.SectorZ)
                {
                    if(IsSquare)
                        ZOK = Position.Z < (Server.SectorSize - Depth);
                    else
                        ZOK = Position.Z < (Server.SectorSize - Width);
                }

                return ((XOK == false) && (ZOK == false));
            }
        }

        public class Waypoint
        {
            public SectorVector Position = new SectorVector();
            public int Previous = 0;
            public int NextA = 65535, NextB = 65535;
            public uint Pause = 0;
            public int ID = 0;
            
        }

        public class Portal
        {
            public SectorVector Position = new SectorVector();
            public float Width = 0, Height = 0, Depth = 0;
            public bool IsSquare = false;
            public float Yaw = 0;
            public string Name = "";
            public string LinkArea = "", LinkName = "";

            public bool IsInSector(int x, int z)
            {
                // If our designation is the same, then the result is simple
                if (Position.SectorX == x && Position.SectorZ == z)
                    return true;

                // If we are more than one out, then the object is too big to consider processing anyway
                // Honestly, who needs an object bigger than 768 units?!
                if (Math.Abs(x - Position.SectorX) > 1 || Math.Abs(z - Position.SectorZ) > 1)
                    return false;

                // If 'OK' is true, then its not within the given sector
                bool XOK = false;
                bool ZOK = false;

                if (x > Position.SectorX)
                {
                    if (IsSquare)
                        XOK = (Position.X > 0.0f);
                    else
                        XOK = (Position.X > Width);
                }
                else if (x < Position.SectorX)
                {
                    XOK = Position.X < (Server.SectorSize - Width);
                }

                if (z > Position.SectorZ)
                {
                    if (IsSquare)
                        ZOK = (Position.Z > 0.0f);
                    else
                        ZOK = (Position.Z > Width);
                }
                else if (z < Position.SectorZ)
                {
                    if (IsSquare)
                        ZOK = Position.Z < (Server.SectorSize - Depth);
                    else
                        ZOK = Position.Z < (Server.SectorSize - Width);
                }

                return ((XOK == false) && (ZOK == false));
            }
        }

        public class Spawn
        {
            public ushort ActorID = 65535;
            public Waypoint Waypoint;
            public float Size = 0;
            public string Script = "";
            public uint Frequency = 0;
            public int Max = 0;
            public float Range = 0;

            public bool IsInSector(int x, int z)
            {
                // If our designation is the same, then the result is simple
                if (Waypoint.Position.SectorX == x && Waypoint.Position.SectorZ == z)
                    return true;

                // If we are more than one out, then the object is too big to consider processing anyway
                // Honestly, who needs an object bigger than 768 units?!
                if (Math.Abs(x - Waypoint.Position.SectorX) > 1 || Math.Abs(z - Waypoint.Position.SectorZ) > 1)
                    return false;

                // If 'OK' is true, then its not within the given sector
                bool XOK = false;
                bool ZOK = false;

                if (x > Waypoint.Position.SectorX)
                {
                    XOK = (Waypoint.Position.X > Size);
                }
                else if (x < Waypoint.Position.SectorX)
                {
                    XOK = Waypoint.Position.X < (Server.SectorSize - Size);
                }

                if (z > Waypoint.Position.SectorZ)
                {
                    ZOK = (Waypoint.Position.Z > Size);
                }
                else if (z < Waypoint.Position.SectorZ)
                {
                    ZOK = Waypoint.Position.Z < (Server.SectorSize - Size);
                }

                return ((XOK == false) && (ZOK == false));
            }
        }

        #endregion

        #region Members
        // Area name
	    public string Name = "";
        //public bool Active = true;

	    // Environment
	    public int[] WeatherChance = new int[5];
	    public bool Outdoors = false;
	    public string WeatherLink = "";
        public Area WeatherLinkArea = null;

	    // Area scripts
	    public string Script = "";
	    
        // Script triggers
        public Trigger[] Triggers;
	    
        // Waypoints
        public Waypoint[] Waypoints;

        // Portals
        public Portal[] Portals;
	    
        // Spawn points
        public Spawn[] Spawns;

        // Water
        public List<ServerWater> WaterAreas = new List<ServerWater>();

        // Sectors
        public ZoneSector[,] Sectors = null;

        

	    // Is PvP allowed
	    public bool PvP = false;

	    // Gravity strength (0-1000)
	    public int Gravity = 0;

	    // Track instances
        public Dictionary<int, AreaInstance> Instances = new Dictionary<int, AreaInstance>();
        public LinkedList<ZoneInstanceRequest> CreationRequests = new LinkedList<ZoneInstanceRequest>();
        //public AreaInstance[] Instances = new AreaInstance[100];
        #endregion

        #region Methods
        public Area()
        {
            AreaList.AddLast(this);
        }

        public bool WaypointEqualsID(Waypoint a, int b)
        {
            Waypoint WaypointB = null;

            if (b >= 0 && b < Waypoints.Length)
            {
                WaypointB = Waypoints[b];
            }

            if (a == WaypointB)
                return true;
            return false;
        }

        public void CreateInstance(ZoneInstanceRequest request)
        {
            CreationRequests.AddLast(request);

            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)'C');
            Pa.Write((byte)Name.Length);
            Pa.Write(Name, false);
            Pa.Write(request.RequestedID);

            RCEnet.Send(Server.MasterConnection, MessageTypes.P_Instance, Pa.ToArray(), true);
        }

        public void CompleteInstanceRequest(ushort requestedID, ushort actualID)
        {
            LinkedListNode<ZoneInstanceRequest> Node = CreationRequests.First;

            while(Node != null)
            {
                if (Node.Value.RequestedID == requestedID)
                {
                    Node.Value.ActualID = actualID;
                    if (actualID == 65535)
                        Node.Value.Succeeded = false;
                    else
                        Node.Value.Succeeded = true;

                    Node.Value.Complete();

                    CreationRequests.Remove(Node);
                    return;
                }
            }
        }

        public AreaInstance CreateInstance(int id)
        {
            // Validate ID
            if (id > 0)
            {
                // Invalid
                if (id >= 65535)
                    return null;

                //  Already exists
                if (Instances.ContainsKey(id) && Instances[id] != null)
                    return null;
            }
            else if (id == 0)
            {
                Log.WriteLine("Couldn't complete CreateInstance() as passed id was '0'");
            }

            // Create instance
            AreaInstance AInstance = new AreaInstance(this);
            AInstance.ID = id;

            if (!Instances.ContainsKey(id))
            {
                Instances.Add(id, AInstance);
            }
            else
            {
                Instances[id] = AInstance;
            }

            // Done
            return AInstance;
        }

        public void RemoveInstance(int id, bool sendToMaster)
        {
            if (Instances[id] == null)
                return;

            // Move players to instance 0 and delete AI
            foreach (InstanceSector Sector in Instances[id].Sectors)
            {
                LinkedListNode<ActorInstance> AINode = Sector.Players.First;

                while (AINode != null)
                {
                    ActorInstance AI = AINode.Value;
                    AINode = AINode.Next;

                    if (AI != null)
                    {
                        if (AI.RNID > 0)
                        {
                            AI.SetArea(this, 0, -1, -1, AI.Position);
                        }
                        else
                        {
                            AI.FreeActorInstance();
                        }
                    }
                }
            }


            Instances[id].Active = false;
//             if (Instances[id].InteractiveSceneryList.Count == 0)
//             {
// 
//                 // Delete dropped items
//                 foreach (DroppedItem D in DroppedItem.DroppedItemList)
//                 {
//                     if (D.ServerHandle == Instances[id])
//                     {
//                         D.Item.ClientInstanceRemoved();
//                         D.Item = null;
//                         D.ServerHandle = null;
// 
//                         DroppedItem.Delete(D);
//                     }
//                 }
//                 DroppedItem.Clean();
// 
//                 if (!sendToMaster)
//                     return;
// 
//                 PacketWriter Pa2 = new PacketWriter();
//                 Pa2.Write((byte)Name.Length);
//                 Pa2.Write(Name, false);
//                 Pa2.Write((ushort)id);
//                 Pa2.Write((ushort)0);
// 
// 
// 
//                 // Send saved data to master
//                 if (Server.MasterConnection != 0)
//                     RCEnet.Send(Server.MasterConnection, MessageTypes.P_RemoveInstance, Pa2.ToArray(), true);
// 
//                 return;
//             }

            // If there is owned scenery, it must be saved
            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)Name.Length);
            Pa.Write(Name, false);
            Pa.Write((ushort)id);
            Pa.Write((ushort)Instances[id].InteractiveSceneryList.Count);

            // Free owned scenery for the instance
            for (int i = 0; i < Instances[id].InteractiveSceneryList.Count; ++i)
            {
                InteractiveSceneryInstance Scn = Instances[id].InteractiveSceneryList[i];

                if (Scn != null)
                {
                    Pa.Write((byte)1);
                    Scn.Serialize(Pa);
                }
                else
                {
                    Pa.Write((byte)0);
                }

                Instances[id].InteractiveSceneryList[i] = null;
            }
            Instances[id].InteractiveSceneryList.Clear();


            // Save and delete dropped items

            // Delete dropped items
            int ItemCount = 0;
            foreach (DroppedItem D in DroppedItem.DroppedItemList)
            {
                if (D.ServerHandle == Instances[id])
                {
                    ++ItemCount;
                }
            }

            Pa.Write((ushort)ItemCount);

            foreach (DroppedItem D in DroppedItem.DroppedItemList)
            {
                if (D.ServerHandle == Instances[id])
                {
                    byte[] ItemArray = D.Item.ToArray();
                    Pa.Write((byte)ItemArray.Length);
                    Pa.Write(ItemArray, 0);
                    Pa.Write((ushort)D.Amount);
                    Pa.Write(D.Position);

                    D.Item.ClientInstanceRemoved();
                    D.Item = null;
                    D.ServerHandle = null;

                    DroppedItem.Delete(D);
                }
            }
            DroppedItem.Clean();

            if (!sendToMaster)
                return;

            // Send saved data to master
            if (Server.MasterConnection != 0)
                RCEnet.Send(Server.MasterConnection, MessageTypes.P_RemoveInstance, Pa.ToArray(), true);

        }
        #endregion

        #region Static Members
        public static LinkedList<Area> AreaList = new LinkedList<Area>();
        public static LinkedList<Area> AreaDelete = new LinkedList<Area>();
        #endregion

        #region Static Methods
        public static int Load(string path)
        {
            string[] Files = Directory.GetFiles(path, "*.dat");

            foreach (string FileName in Files)
            {
                BBBinaryReader F = new BBBinaryReader(File.OpenRead(FileName));

		        Area A = new Area();

		        A.Name = Path.GetFileNameWithoutExtension(FileName);

                List<Portal> TPortals = new List<Portal>();
                List<Spawn> TSpawns = new List<Spawn>();
                List<Waypoint> TWaypoints = new List<Waypoint>();
                List<Trigger> TTriggers = new List<Trigger>();
                InteractiveScenery[] TInter = new InteractiveScenery[0];
				
                while (F.BaseStream.Position < F.BaseStream.Length)
                {
                    FileChunk Chunk = FileChunk.ReadChunk(F);

                    if (Chunk.Name == "PROP" && Chunk.Version == 1)
                    {
                        A.Script = F.ReadBBString();
                        A.PvP = F.ReadBoolean();
                        A.Gravity = F.ReadUInt16();
                        A.Outdoors = F.ReadBoolean();

                        ushort SectorsX = F.ReadUInt16();
                        ushort SectorsZ = F.ReadUInt16();

                        A.Sectors = new ZoneSector[SectorsX, SectorsZ];
                        //A.SectorAssignments = new byte[A.SectorsX, A.SectorsZ];
                    }
                    else if (Chunk.Name == "ZONE" && Chunk.Version == 1)
                    {
                        // TODO: Add after MT2
                        Chunk.Skip();
//                         if (Z.SectorsX == 0 || Z.SectorsZ == 0)
//                         {
//                             System.Windows.Forms.MessageBox.Show("Zone contained 0 width sectors, skipping sector assignments!");
//                             Chunk.Skip();
//                             continue;
//                         }
// 
//                         if (Chunk.Length != Z.SectorsX * Z.SectorsZ)
//                         {
//                             System.Windows.Forms.MessageBox.Show("Sector assignment lengths were incorrect. Either the chunk is in the wrong order or corrupt. Skipping.");
//                             Chunk.Skip();
//                             continue;
//                         }
// 
//                         for (int z = 0; z < Z.SectorsZ; ++z)
//                         {
//                             for (int x = 0; x < Z.SectorsX; ++x)
//                             {
//                                 Z.SectorAssignments[x, z] = F.ReadByte();
//                             }
//                         }
                    }
                    else if (Chunk.Name == "PORT" && Chunk.Version == 1)
                    {
                        ushort Cnt = F.ReadUInt16();

                        for (int i = 0; i < Cnt; ++i)
                        {
                            Portal P = new Portal();

                            P.Name = F.ReadBBString();
                            P.LinkArea = F.ReadBBString();
                            P.LinkName = F.ReadBBString();

                            ushort SectorX = F.ReadUInt16();
                            ushort SectorZ = F.ReadUInt16();
                            float X = F.ReadSingle();
                            float Y = F.ReadSingle();
                            float Z = F.ReadSingle();
                            P.Position = new SectorVector(SectorX, SectorZ, X, Y, Z);
                            P.IsSquare = F.ReadByte() > 0;

                            if (!P.IsSquare)
                            {
                                P.Width = F.ReadSingle();
                                P.Yaw = F.ReadSingle();
                                F.ReadSingle();
                            }
                            else
                            {
                                P.Width = F.ReadSingle();
                                P.Height = F.ReadSingle();
                                P.Depth = F.ReadSingle();

                                // In GE, the position is the center of the portal (though it is scaled by half dimensions, to W/H/D are correct).
                                P.Position.X -= P.Width * 0.5f;
                                P.Position.Y -= P.Height * 0.5f;
                                P.Position.Z -= P.Depth * 0.5f;
                                P.Position.FixValues();
                            }

                            TPortals.Add(P);
                        }
                    }
                    else if (Chunk.Name == "TRIG" && Chunk.Version == 1)
                    {
                        ushort Cnt = F.ReadUInt16();

                        for (int i = 0; i < Cnt; ++i)
                        {
                            ushort SectorX = F.ReadUInt16();
                            ushort SectorZ = F.ReadUInt16();
                            float X = F.ReadSingle();
                            float Y = F.ReadSingle();
                            float Z = F.ReadSingle();
                            bool IsSquare = F.ReadByte() > 0;
                            float DimX = F.ReadSingle();
                            float DimY = F.ReadSingle();
                            float DimZ = F.ReadSingle();
                            string Script = F.ReadBBString();
                            if(Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                                Script = Script.Substring(0, Script.Length - 3);

                            Trigger T = new Trigger();
                            T.Position = new SectorVector(SectorX, SectorZ, X, Y, Z);
                            T.IsSquare = IsSquare;
                            T.Width = DimX;
                            T.Height = DimY;
                            T.Depth = DimZ;
                            T.ScriptName = Script;

                            if (T.IsSquare)
                            {

                                // In GE, the position is the center of the portal (though it is scaled by half dimensions, to W/H/D are correct).
                                T.Position.X -= T.Width * 0.5f;
                                T.Position.Y -= T.Height * 0.5f;
                                T.Position.Z -= T.Depth * 0.5f;
                                T.Position.FixValues();
                            }

                            TTriggers.Add(T);
                        }
                    }
                    else if (Chunk.Name == "WAYP" && Chunk.Version == 1)
                    {
                        ushort Cnt = F.ReadUInt16();

                        for (int i = 0; i < Cnt; ++i)
                        {
                            ushort SectorX = F.ReadUInt16();
                            ushort SectorZ = F.ReadUInt16();
                            float X = F.ReadSingle();
                            float Y = F.ReadSingle();
                            float Z = F.ReadSingle();
                            ushort TNextA = F.ReadUInt16();
                            ushort TNextB = F.ReadUInt16();
                            ushort TPrev = F.ReadUInt16();
                            int PauseTime = F.ReadInt32();

                            Waypoint W = new Waypoint();
                            W.Position = new SectorVector(SectorX, SectorZ, X, Y, Z);
                            W.NextA = TNextA;
                            W.NextB = TNextB;
                            W.Previous = TPrev;
                            W.Pause = (uint)PauseTime;
                            W.ID = i;

                            TWaypoints.Add(W);
                        }
                    }
                    else if (Chunk.Name == "SPAW" && Chunk.Version == 1)
                    {
                        ushort Cnt = F.ReadUInt16();

                        for (int i = 0; i < Cnt; ++i)
                        {
                            ushort ActorID = F.ReadUInt16();
                            ushort WPID = F.ReadUInt16();
                            float Size = F.ReadSingle();
                            string Script = F.ReadBBString();
                            ushort Max = F.ReadUInt16();
                            ushort Freq = F.ReadUInt16();
                            float Range = F.ReadSingle();

                            if (WPID < TWaypoints.Count)
                            {
                                Waypoint W = TWaypoints[WPID];

                                Spawn S = new Spawn();
                                S.ActorID = ActorID;
                                S.Frequency = Freq;
                                S.Max = Max;
                                S.Range = Range;
                                S.Script = Script;
                                S.Size = Size;
                                S.Waypoint = W;

                                TSpawns.Add(S);

                            }

                        }
                    }
                    else if (Chunk.Name == "WATR" && Chunk.Version == 1)
                    {
                        ushort Cnt = F.ReadUInt16();

                        for (int i = 0; i < Cnt; ++i)
                        {
                            ushort SectorX = F.ReadUInt16();
                            ushort SectorZ = F.ReadUInt16();
                            float X = F.ReadSingle();
                            float Y = F.ReadSingle();
                            float Z = F.ReadSingle();

                            ServerWater W = new ServerWater();
                            W.Area = A;
                            A.WaterAreas.Add(W);
                            W.Position = new SectorVector(SectorX, SectorZ, X, Y, Z);
                            W.Width = F.ReadSingle();
                            W.Depth = F.ReadSingle();
                            W.Damage = F.ReadUInt16();
                            W.DamageType = F.ReadUInt16();
                        }
                    }
                    else if (Chunk.Name == "ISCN" && Chunk.Version == 1)
                    {
                        ushort Cnt = F.ReadUInt16();
                        TInter = new InteractiveScenery[Cnt];

                        for (int i = 0; i < Cnt; ++i)
                        {
                            ushort SectorX = F.ReadUInt16();
                            ushort SectorZ = F.ReadUInt16();
                            float X = F.ReadSingle();
                            float Y = F.ReadSingle();
                            float Z = F.ReadSingle();
                            ushort MeshID = F.ReadUInt16();
                            string ScriptName = F.ReadBBString();

                            TInter[i] = new InteractiveScenery(new SectorVector(SectorX, SectorZ, X, Y, Z), MeshID, ScriptName);
                        }
                    }
                    else
                    {
                        Log.WriteLine("Unable to read zone chunk: " + Chunk.Name + ":" + Chunk.Version.ToString() + ".");
                        Chunk.Skip();
                    }
                }

                F.Close();

                A.Portals = new Portal[TPortals.Count];
                A.Triggers = new Trigger[TTriggers.Count];
                A.Waypoints = new Waypoint[TWaypoints.Count];
                A.Spawns = new Spawn[TSpawns.Count];
                TPortals.CopyTo(A.Portals);
                TTriggers.CopyTo(A.Triggers);
                TWaypoints.CopyTo(A.Waypoints);
                TSpawns.CopyTo(A.Spawns);

                // Just define a size for now till the format change
                int MaxSectorX = A.Sectors.GetLength(0);
                int MaxSectorZ = A.Sectors.GetLength(1);

                List<ServerWater> TWaters = new List<ServerWater>();

                for (int z = 0; z < MaxSectorZ; ++z)
                {
                    for (int x = 0; x < MaxSectorX; ++x)
                    {
                        TTriggers.Clear();
                        TPortals.Clear();
                        TWaypoints.Clear();
                        TSpawns.Clear();
                        TWaters.Clear();

                        foreach (Trigger T in A.Triggers)
                        {
                            if(T.IsInSector(x, z))
                                TTriggers.Add(T);
                        }

                        foreach (Waypoint T in A.Waypoints)
                        {
                            if (T.Position.SectorX == x && T.Position.SectorZ == z)
                                TWaypoints.Add(T);
                        }

                        foreach (Portal T in A.Portals)
                        {
                            if(T.IsInSector(x, z))
                                TPortals.Add(T);
                        }

                        foreach (Spawn T in A.Spawns)
                        {
                            if(T.Waypoint != null)
                                if(T.IsInSector(x, z))
                                    TSpawns.Add(T);
                        }

                        foreach (ServerWater T in A.WaterAreas)
                        {
                            if(T.IsInSector(x, z))
                                TWaters.Add(T);
                        }



                        A.Sectors[x, z] = new ZoneSector();
                        A.Sectors[x, z].SectorX = (ushort)x;
                        A.Sectors[x, z].SectorZ = (ushort)z;
                        A.Sectors[x, z].Triggers = new Trigger[TTriggers.Count];
                        A.Sectors[x, z].Portals = new Portal[TPortals.Count];
                        A.Sectors[x, z].Waypoints = new Waypoint[TWaypoints.Count];
                        A.Sectors[x, z].Spawns = new Spawn[TSpawns.Count];
                        A.Sectors[x, z].Waters = new ServerWater[TWaters.Count];

                        int Count = 0;
                        for (int i = 0; i < TInter.Length; ++i)
                        {
                            if (TInter[i].Position.SectorX == x && TInter[i].Position.SectorZ == z)
                            {
                                ++Count;
                            }
                        }

                        A.Sectors[x, z].Sceneries = new InteractiveScenery[Count];
                        Count = 0;
                        for (int i = 0; i < TInter.Length; ++i)
                        {
                            if (TInter[i].Position.SectorX == x && TInter[i].Position.SectorZ == z)
                            {
                                A.Sectors[x, z].Sceneries[Count] = TInter[i];

                                ++Count;
                            }
                        }

                        TTriggers.CopyTo(A.Sectors[x, z].Triggers);
                        TPortals.CopyTo(A.Sectors[x, z].Portals);
                        TWaypoints.CopyTo(A.Sectors[x, z].Waypoints);
                        TSpawns.CopyTo(A.Sectors[x, z].Spawns);
                        TWaters.CopyTo(A.Sectors[x, z].Waters);
                    }
                }

                // Create default instance
                AreaInstance AInstance = AreaInstance.CreateAreaInstance(A, 0);
            }

            return AreaList.Count;
        }

        public static Area Find(string name)
        {
            foreach (Area A in Area.AreaList)
            {
                if (A.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return A;
            }

            return null;
        }

        public static void Delete(Area item)
        {
            AreaDelete.AddLast(item);
        }

        public static void Clean()
        {
            foreach (Area A in AreaDelete)
            {
                AreaList.Remove(A);
            }
            AreaDelete.Clear();
        }
        #endregion
    }
}
