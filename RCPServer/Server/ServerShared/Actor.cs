using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;

namespace RCPServer
{
    public class Actor
    {
        public uint ID = 0;
        public string Race = "";
        public string Class = "";
        public string Description = "";
        public string StartArea = "";
        public string StartPortal = "";
        public float Radius = 0;            // For server use, since the server is not aware of the details of the mesh itself
        public float Scale = 0;             // Actor scale, applied to the base mesh

        public ushort MaleMesh = 0, FemaleMesh = 0;
        public ushort[] DefaultGubbins = null;
        
        // Each outer list is an ID (ie ActorInstance.Hair)
        // Each inner list is a set of gubbins, both are restricted to 256 entries.
        public List<ushort[]> Beards      = new List<ushort[]>();
        public List<ushort[]> MaleHairs   = new List<ushort[]>();
        public List<ushort[]> FemaleHairs = new List<ushort[]>();

        public ActorTextureSet[] MaleFaceIDs = new ActorTextureSet[0];   // Allowed face textures for male
        public ActorTextureSet[] FemaleFaceIDs = new ActorTextureSet[0]; // Allowed face textures for female
        public ActorTextureSet[] MaleBodyIDs = new ActorTextureSet[0];   // Allowed body textures for the male
        public ActorTextureSet[] FemaleBodyIDs = new ActorTextureSet[0]; // Allowed body textures for the female
        public ushort[] MSpeechIDs = new ushort[16];   // Male sound IDs for speech
        public ushort[] FSpeechIDs = new ushort[16];   // Female sound IDs for speech
        public ushort BloodTexID = 65535;       // For blood particles
        public byte Genders = 0;            // 0 for normal (male and female), 1 for male only, 2 for female only, 3 for no genders
        public Attributes Attributes = new Attributes();
        public int[] Resistances = new int[20];     // Damage type resistances
        public ushort MAnimationSet = 65535;    // The ID of the male animation set to use
        public ushort FAnimationSet = 65535;    // The ID of the female animation set to use
        public bool Playable = false;           // Can a player be this actor?
        public bool Rideable = false;           // Can this actor be ridden by another?
        public byte Aggressiveness = 0;     // Aggressiveness - 0 = passive, 1 = attack when provoked, 2 = attack on sight, 3 = no combat
        public int AggressiveRange = 0;     // From how nearby will the actor detect targets?
        public byte TradeMode = 0;          // 0 = will not trade, 1 = trades for free (pack mules!), 2 = charges for trade (salesman)
        public ActorEnvironment Environment = 0;        // Whether actor walks, swims, flies, etc.
        public ushort InventorySlots = 0;   // Short (up to 16 true/false flags) for the slots defined in Inventories.bb
        public byte DefaultDamageType = 0;
        public byte DefaultFaction = 0;     // Initial home faction for instances of this actor
        public int XPMultiplier = 0;        // How much experience another actor gets for killing an instance of this actor
        public bool PolyCollision = false;      // True for polygonal collision instead of ellipsoid

        public Actor()
        {
            Race = Class = Description = "Warning, Undefined Actor!";
            StartArea = StartPortal = "";
            Radius = Scale = 0.0f;

            MaleMesh = 65535;
            FemaleMesh = 65535;

            for (int i = 0; i < 16; ++i)
            {
                MSpeechIDs[i] = 65535;
                FSpeechIDs[i] = 65535;
            }
        }

        protected bool GetFlag(int value, int flag)
        {
            return ((value >> flag) & 1) > 0;
        }

        public bool HasSlot(byte slotI, Item I)
        {
            // If it's an equipped slot
	        if(slotI < (byte)ItemSlots.Backpack)
            {
		        if(I.ExclusiveRace.Length > 0)
                {

			        // Allow even disabled equipment slots to be used if the item is exclusive to this race
			        if(I.ExclusiveRace.Equals(Race, StringComparison.CurrentCultureIgnoreCase))
                    {
				        return true;
			        }else // Never allow the slot if the item is exclusive to another race
                    {
                        return false;
			        }
		        }

		        // Never allow the slot if the item is exclusive to another race
		        if(I.ExclusiveClass.Length > 0)
                {
			        if(!I.ExclusiveClass.Equals(Class, StringComparison.CurrentCultureIgnoreCase))
                        return false;
		        }
	        }

	        // Check whether the slot is disabled
            ItemSlots SlotI = (ItemSlots)slotI;
	        switch(SlotI)
            {
		        case ItemSlots.Weapon:
			        return GetFlag(InventorySlots, (int)SlotNames.Weapon - 1);
		        case ItemSlots.Shield:
			        return GetFlag(InventorySlots, (int)SlotNames.Shield - 1);
		        case ItemSlots.Hat:
			        return GetFlag(InventorySlots, (int)SlotNames.Hat - 1);
		        case ItemSlots.Chest:
			        return GetFlag(InventorySlots, (int)SlotNames.Chest - 1);
		        case ItemSlots.Hand:
			        return GetFlag(InventorySlots, (int)SlotNames.Hand - 1);
		        case ItemSlots.Belt:
			        return GetFlag(InventorySlots, (int)SlotNames.Belt - 1);
		        case ItemSlots.Legs:
			        return GetFlag(InventorySlots, (int)SlotNames.Legs - 1);
		        case ItemSlots.Feet:
			        return GetFlag(InventorySlots, (int)SlotNames.Feet - 1);
		        case ItemSlots.Ring1:
                case ItemSlots.Ring2:
                case ItemSlots.Ring3:
                case ItemSlots.Ring4:
			        return GetFlag(InventorySlots, (int)SlotNames.Ring - 1);
		        case ItemSlots.Amulet1:
                case ItemSlots.Amulet2:
			        return GetFlag(InventorySlots, (int)SlotNames.Amulet - 1);
		        default:
			        return GetFlag(InventorySlots, (int)SlotNames.Backpack - 1);
            }
        }

        // Big array, make it smaller in the future?
        public static Dictionary<uint, Actor> Actors = new Dictionary<uint, Actor>();

        const string ActorsSQLTable = "Actors";
        public const string ActorsDatabase = "Data/Server Data/Actors.s3db";

        public static int Load(string path)
	    {
            // Create database connection
            SqliteConnection databaseConnection = new SqliteConnection();
            databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", path);
            databaseConnection.Open();

            // Query table to get all items
            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + ActorsSQLTable;

            int actorCount = 0;
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Actor actor = new Actor();

                actor.ID = (uint)reader.GetInt64(reader.GetOrdinal("ID"));
                actor.Race = reader.GetString(reader.GetOrdinal("Race"));
                actor.Class = reader.GetString(reader.GetOrdinal("Class"));
                actor.Description = reader.GetString(reader.GetOrdinal("Description"));
                actor.StartArea = reader.GetString(reader.GetOrdinal("StartArea"));
                actor.StartPortal = reader.GetString(reader.GetOrdinal("StartPortal"));
                actor.MAnimationSet = (ushort)reader.GetInt64(reader.GetOrdinal("MaleAnimationSet"));
                actor.FAnimationSet = (ushort)reader.GetInt64(reader.GetOrdinal("FemaleAnimationSet"));
                actor.Scale = reader.GetFloat(reader.GetOrdinal("Scale"));
                actor.Radius = reader.GetFloat(reader.GetOrdinal("Radius"));
                actor.MaleMesh = (ushort)reader.GetInt64(reader.GetOrdinal("MaleMesh"));
                actor.FemaleMesh = (ushort)reader.GetInt64(reader.GetOrdinal("FemaleMesh"));

                // Gubbins
                byte[] data = (byte[])reader.GetValue(reader.GetOrdinal("GubbinData"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    byte gubbinCount = binaryReader.ReadByte();
                    actor.DefaultGubbins = new ushort[gubbinCount];
                    for (int i = 0; i < gubbinCount; i++)
                        actor.DefaultGubbins[i] = binaryReader.ReadUInt16();
                }

                // Beards
                data = (byte[])reader.GetValue(reader.GetOrdinal("Beards"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (uint bid = 0; bid < count; bid++)
                    {
                        uint gubbinCount = binaryReader.ReadUInt32();
                        ushort[] temps = new ushort[gubbinCount];
                        for (uint i = 0; i < gubbinCount; i++)
                            temps[i] = (ushort)binaryReader.ReadUInt32();

                        actor.Beards.Add(temps);
                    }
                }

                // Male Hair
                data = (byte[])reader.GetValue(reader.GetOrdinal("MaleHair"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (uint bid = 0; bid < count; bid++)
                    {
                        uint gubbinCount = binaryReader.ReadUInt32();
                        ushort[] temps = new ushort[gubbinCount];
                        for (uint i = 0; i < gubbinCount; i++)
                            temps[i] = (ushort)binaryReader.ReadUInt32();

                        actor.MaleHairs.Add(temps);
                    }
                }

                // Female Hair
                data = (byte[])reader.GetValue(reader.GetOrdinal("FemaleHair"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (uint bid = 0; bid < count; bid++)
                    {
                        uint gubbinCount = binaryReader.ReadUInt32();
                        ushort[] temps = new ushort[gubbinCount];
                        for (uint i = 0; i < gubbinCount; i++)
                            temps[i] = (ushort)binaryReader.ReadUInt32();

                        actor.FemaleHairs.Add(temps);
                    }
                }

                // Male Face IDs
                data = (byte[])reader.GetValue(reader.GetOrdinal("MaleFaceTextures"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    actor.MaleFaceIDs = new ActorTextureSet[count];
                    for (int i = 0; i < count; i++)
                    {
                        actor.MaleFaceIDs[i] = new ActorTextureSet((ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32(),
                            (ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32());
                    }
                }

                // Female Face IDs
                data = (byte[])reader.GetValue(reader.GetOrdinal("FemaleFaceTextures"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    actor.FemaleFaceIDs = new ActorTextureSet[count];
                    for (int i = 0; i < count; i++)
                    {
                        actor.FemaleFaceIDs[i] = new ActorTextureSet((ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32(),
                            (ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32());
                    }
                }

                // Male Body IDs
                data = (byte[])reader.GetValue(reader.GetOrdinal("MaleBodyTextures"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    actor.MaleBodyIDs = new ActorTextureSet[count];
                    for (int i = 0; i < count; i++)
                    {
                        actor.MaleBodyIDs[i] = new ActorTextureSet((ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32(),
                            (ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32());
                    }
                }

                // Female Body IDs
                data = (byte[])reader.GetValue(reader.GetOrdinal("FemaleBodyTextures"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    actor.FemaleBodyIDs = new ActorTextureSet[count];
                    for (int i = 0; i < count; i++)
                    {
                        actor.FemaleBodyIDs[i] = new ActorTextureSet((ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32(),
                            (ushort)binaryReader.ReadUInt32(), (ushort)binaryReader.ReadUInt32());
                    }
                }

                // Male speech
                data = (byte[])reader.GetValue(reader.GetOrdinal("MaleSpeech"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (int i = 0; i < count; i++)
                    {
                        actor.MSpeechIDs[i] = (ushort)binaryReader.ReadUInt32();
                    }
                }

                // Female speech
                data = (byte[])reader.GetValue(reader.GetOrdinal("FemaleSpeech"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (int i = 0; i < count; i++)
                    {
                        actor.FSpeechIDs[i] = (ushort)binaryReader.ReadUInt32();
                    }
                }

                actor.BloodTexID = (ushort)reader.GetInt64(reader.GetOrdinal("BloodTexture"));

                // Attributes
                data = (byte[])reader.GetValue(reader.GetOrdinal("Attributes"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();

                    for (int i = 0; i < count; i++)
                    {
                        actor.Attributes.Value[i] = (ushort)binaryReader.ReadUInt32();
                        actor.Attributes.Maximum[i] = (ushort)binaryReader.ReadUInt32();
                    }
                }

                // Resistances
                data = (byte[])reader.GetValue(reader.GetOrdinal("Resistances"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (int i = 0; i < count; i++)
                    {
                        actor.Resistances[i] = (ushort)binaryReader.ReadUInt32();
                    }
                }

                actor.Genders = reader.GetByte(reader.GetOrdinal("Genders"));
                actor.Playable = reader.GetBoolean(reader.GetOrdinal("Playable"));
                actor.Rideable = reader.GetBoolean(reader.GetOrdinal("Ridable"));
                actor.Aggressiveness = reader.GetByte(reader.GetOrdinal("Aggression"));
                actor.AggressiveRange = reader.GetInt32(reader.GetOrdinal("AggressionRange"));
                actor.TradeMode = reader.GetByte(reader.GetOrdinal("TradeMode"));
                actor.Environment = (ActorEnvironment)reader.GetByte(reader.GetOrdinal("EnvironmentType"));
                actor.InventorySlots = (ushort)reader.GetInt32(reader.GetOrdinal("InventorySlots"));
                actor.DefaultDamageType = reader.GetByte(reader.GetOrdinal("DefaultDamageType"));
                actor.DefaultFaction = reader.GetByte(reader.GetOrdinal("DefaultFaction"));
                actor.XPMultiplier = reader.GetInt32(reader.GetOrdinal("XPMultiplier"));
                actor.PolyCollision = reader.GetBoolean(reader.GetOrdinal("PolygonCollision"));

                Actors.Add(actor.ID, actor);

                actorCount++;
            }
		    return actorCount;
	    }
    }
}
