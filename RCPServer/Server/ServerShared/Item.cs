using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;

namespace RCPServer
{
    public class Item : Scripting.Item
    {
        #region Members
        public const string FileDirectory = "Data/Server Data/Items.s3db";
        public uint ID = uint.MaxValue;
		public string Name = "Undefined Item";
		public string ExclusiveRace = "";
        public string ExclusiveClass = ""; // If this item can only be used by a certain race and/or class
		public string Script = "";
        public string Method = "";      // Called when the item is right clicked
		public ItemTypes ItemType = 0;              // Should be one of the constants above
		public int Value = 0;
        public int Mass = 0;           // Average monetary value, and item weight
		public int ThumbnailTexID  = int.MaxValue;        // The texture ID for the image seen in the inventory system

        public uint[] GubbinTemplates = null;
        //public ushort MMeshID = 65535;
        //public ushort FMeshID = 65535;      // Weapon/hat/shield/chest/forearm/shin mesh IDs
		//public ushort[] Gubbins = new ushort[6];            // Flags to activate gubbins when item is equipped
		
        public Attributes Attributes = new Attributes(); // An actor attributes object (for extra weapon damage effects, armour use, food eating, etc.)
		public bool TakesDamage = false;           // True if using this item reduces its health, False for it to be indestructable
		public SlotNames SlotType = 0;              // Should be set to one of the slot type constants in Inventories.bb
		public int WeaponDamage = 0;
        public int WeaponDamageType = 0;
        public WeaponType WeaponType = 0; // Weapon specific
		
		// Ranged weapon specific
		public int RangedProjectile = 65535;
		public string RangedAnimation = "";
		public float Range = 0.0f;
		
		public int ArmourLevel = 0;                                // Armour specific
		public uint EatEffectsLength = 0;                           // Potion or ingredients specific
		public uint ImageID = int.MaxValue;                                    // Image item specific (Texture ID)
		public string MiscData = "";                                  // General use for misc items
		public bool Stackable = false;                                  // Item can be stacked up
        #endregion

        #region Properties
        public override uint IDNum
        {
            get
            {
                return ID;
            }
        }

        public override string ItemName
        {
            get
            {
                return Name;
            }
        }

        public override int Armour
        {
            get
            {
                return ArmourLevel;
            }
        }

        public override string Class
        {
            get
            {
                return ExclusiveClass;
            }
        }

        public override bool IsDamagable
        {
            get
            {
                return TakesDamage;
            }
        }

        public override int Damage
        {
            get
            {
                return WeaponDamage;
            }
        }

        public override int DamageType
        {
            get
            {
                return WeaponDamageType;
            }
        }

        public override uint EatEffectsTime
        {
            get
            {
                return EatEffectsLength;
            }
        }

        public override bool IsStackable
        {
            get
            {
                return Stackable;
            }
        }

        public override string MethodName
        {
            get
            {
                return Method;
            }
        }

        public override string ScriptFile
        {
            get
            {
                return Script;
            }
        }

        public override string Misc
        {
            get
            {
                return MiscData;
            }
        }

        public override int Price
        {
            get
            {
                return Value;
            }
        }

        public override string Race
        {
            get
            {
                return ExclusiveRace;
            }
        }

        public override float WeaponRange
        {
            get
            {
                return Range;
            }
        }

        public override int Weight
        {
            get
            {
                return Mass;
            }
        }
        #endregion 

        #region Methods
        public Item()
        {
        }

        public bool SlotsMatch(byte slotI)
        {
            if (slotI >= (byte)ItemSlots.Backpack)
                return true;

            switch (ItemType)
            {
                case ItemTypes.I_Weapon:
                    {
                        if (slotI == (byte)ItemSlots.Weapon)
                            return true;
                        break;
                    }

                case ItemTypes.I_Armour:
                    {
                        switch (SlotType)
                        {
                            case SlotNames.Shield:
                                {
                                    if (slotI == (byte)ItemSlots.Shield)
                                        return true;
                                    break;
                                }

                            case SlotNames.Hat:
                                {
                                    if (slotI == (byte)ItemSlots.Hat)
                                        return true;
                                    break;
                                }

                            case SlotNames.Chest:
                                {
                                    if (slotI == (byte)ItemSlots.Chest)
                                        return true;
                                    break;
                                }

                            case SlotNames.Hand:
                                {
                                    if (slotI == (byte)ItemSlots.Hand)
                                        return true;
                                    break;
                                }

                            case SlotNames.Belt:
                                {
                                    if (slotI == (byte)ItemSlots.Belt)
                                        return true;
                                    break;
                                }

                            case SlotNames.Legs:
                                {
                                    if (slotI == (byte)ItemSlots.Legs)
                                        return true;
                                    break;
                                }

                            case SlotNames.Feet:
                                {
                                    if (slotI == (byte)ItemSlots.Feet)
                                        return true;
                                    break;
                                }
                        }
                        break;
                    }

                case ItemTypes.I_Ring:
                    {
                        if (SlotType == SlotNames.Ring)
                        {
                            if (slotI >= (byte)ItemSlots.Ring1 && slotI <= (byte)ItemSlots.Ring4)
                                return true;
                        }
                        else
                        {
                            if (slotI >= (byte)ItemSlots.Amulet1 && slotI <= (byte)ItemSlots.Amulet2)
                                return true;
                        }

                        break;
                    }
            }

            return false;
        }
        #endregion

        #region Scripting override
        public override int GetAttributeMaxValue(int index)
        {
            if(index > Attributes.Value.Length || index < 0)
                return 0;

            return Attributes.Maximum[index];
            
        }

        public override int  GetAttributeValue(int index)
        {
            if (index > Attributes.Value.Length || index < 0)
                return 0;

            return Attributes.Maximum[index];

        }

        public override Scripting.ItemTypes GetItemType()
        {
            int itemNum = (int)ItemType;
            return (Scripting.ItemTypes)itemNum;
        }

        public override Scripting.ItemSlot GetSlot()
        {
            int itemSlotNum = (int)SlotType;
            return (Scripting.ItemSlot)itemSlotNum;
        }     
        #endregion

        #region Static Members
        public static Dictionary<uint, Item> Items = new Dictionary<uint, Item>();
        #endregion

        #region Static Methods
        public static Item Find(string name)
        {
            foreach (Item It in Items.Values)
            {
                if (It.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return It;
                }
            }

            return null;
        }

        public static int Load(string path)
        {


            List<Item> defaultItemList = new List<Item>();

            if (!File.Exists(path))
                return -1;

            // Create database connection
            SqliteConnection databaseConnection = new SqliteConnection();
            databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", path);
            databaseConnection.Open();

            // Query table to get all items
            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Items";

            int itemCount = 0;
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Item item = new Item();
                item.ID = (uint)reader.GetInt64(reader.GetOrdinal("ID"));
                item.Name = reader.GetString(reader.GetOrdinal("Name"));
                item.ExclusiveRace = reader.GetString(reader.GetOrdinal("Race"));
                item.ExclusiveClass = reader.GetString(reader.GetOrdinal("Class"));
                item.Script = reader.GetString(reader.GetOrdinal("Script"));
                item.Method = reader.GetString(reader.GetOrdinal("Method"));
                item.ItemType = (ItemTypes)reader.GetByte(reader.GetOrdinal("Type"));
                item.Value = reader.GetInt32(reader.GetOrdinal("Value"));
                item.Mass = reader.GetInt32(reader.GetOrdinal("Mass"));
                item.TakesDamage = reader.GetBoolean(reader.GetOrdinal("TakesDamage"));
                item.ThumbnailTexID = reader.GetInt32(reader.GetOrdinal("ThumbnailID"));

                // Read gubbin data out
                byte[] buffer = (byte[])reader.GetValue(reader.GetOrdinal("Gubbins"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
                {
                    uint gubbinCount = binaryReader.ReadUInt32();
                    item.GubbinTemplates = new uint[gubbinCount];

                    for (int i = 0; i < gubbinCount; i++)
                        item.GubbinTemplates[i] = binaryReader.ReadUInt32();
                }

                item.SlotType = (SlotNames)reader.GetInt16(reader.GetOrdinal("SlotType"));
                item.Stackable = reader.GetBoolean(reader.GetOrdinal("Stackable"));

                // Read attribute data out
                buffer = (byte[])reader.GetValue(reader.GetOrdinal("Attributes"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
                {
                    uint attributeCount = binaryReader.ReadUInt32();

                    for (int i = 0; i < attributeCount; i++)
                        item.Attributes.Value[i] = binaryReader.ReadInt32() - 5000; // Deducts 5k for some reason
                }

                // Item type specific data out
                buffer = (byte[])reader.GetValue(reader.GetOrdinal("ItemTypeData"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
                {
                    switch (item.ItemType)
                    {
                        case ItemTypes.I_Weapon:
                            item.WeaponDamage = binaryReader.ReadInt32();
                            item.WeaponDamageType = binaryReader.ReadInt32();
                            item.WeaponType = (WeaponType)binaryReader.ReadByte();
                            item.RangedProjectile = binaryReader.ReadInt32();
                            item.Range = binaryReader.ReadSingle();
                            item.RangedAnimation = binaryReader.ReadString();
                            break;

                        case ItemTypes.I_Armour:
                            item.ArmourLevel = binaryReader.ReadInt32();
                            break;

                        case ItemTypes.I_Potion:
                        case ItemTypes.I_Ingredient:
                            item.EatEffectsLength = binaryReader.ReadUInt32();
                            break;

                        case ItemTypes.I_Image:
                            item.ImageID = binaryReader.ReadUInt32();
                            break;

                        case ItemTypes.I_Other:
                            item.MiscData = binaryReader.ReadString();
                            break;
                    }
                }

                Items.Add(item.ID, item);
                defaultItemList.Add(item);
                itemCount++;
            }

            databaseConnection.Close();

            // Create array for scripting access 
            DefaultItems = defaultItemList.ToArray();

            return itemCount;
        }
        #endregion
    }
}
