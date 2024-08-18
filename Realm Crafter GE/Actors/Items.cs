//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
// Realm Crafter Items module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port October 2006

using System;
using System.IO;
using System.Collections.Generic;
using Community.CsharpSqlite.SQLiteClient;
using System.Data;

namespace RealmCrafter
{
    // Describes an item
    public class Item
    {
        // Constants
        public enum ItemType
        {
            Weapon = 1,
            Armour = 2,
            Ring = 3,
            Potion = 4,
            Ingredient = 5,
            Image = 6,
            Other = 7
        } ;

        public enum ArmourType
        {
            Hat = 0,
            Shirt = 1,
            Trousers = 2,
            Gloves = 3,
            Boots = 4,
            Shield = 5
        } ;

        public enum WeaponType
        {
            OneHand = 1,
            TwoHand = 2,
            Ranged = 3
        } ;

        public const string ItemsSQLTable = "Items";


        // Damage types
        public const int TotalDamageTypes = 20;
        public static string[] DamageTypes = new string[TotalDamageTypes];


        public static bool LoadDamageTypes(string Filename)
        {
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return false;
            }

            for (int i = 0; i < TotalDamageTypes; ++i)
            {
                DamageTypes[i] = Blitz.ReadString(F);
            }

            F.Close();

            return true;
        }

        public static void SaveDamageTypes(string Filename)
        {
            BinaryWriter F = Blitz.WriteFile(Filename);
            if(F == null)
                return;

            for (int i = 0; i < TotalDamageTypes; ++i)
                Blitz.WriteString(DamageTypes[i], F);

            F.Close();

        }

        public static int FindDamageType(string Name)
        {
            Name = Name.ToUpper();
            for (int i = 0; i < TotalDamageTypes; ++i)
            {
                if (DamageTypes[i].ToUpper() == Name)
                {
                    return i;
                }
            }
            return -1;
        }

        // Static members
        public static bool WeaponDamageEnabled, ArmourDamageEnabled;

        // Items index
        public static Dictionary<uint, Item> Index = new Dictionary<uint, Item>();

        // Members
        public uint ID;
        public string Name;
        public string ExclusiveRace, ExclusiveClass; // If this item can only be used by a particular race/class
        public string Script, Method; // Script to  call when item is right-clicked
        public ItemType IType;
        public int Value, Mass;
        public bool Stackable; // Item can be stacked in an inventory
        public int ThumbnailTexID;//, MMeshID, FMeshID; // Texture ID to show in inventory, and item mesh
        public List<GubbinTemplate> GubbinTemplates = new List<GubbinTemplate>();
        public uint[] TempGubbins = new uint[0];
        //public bool[] Gubbins = new bool[6]; // Flags to activate gubbins when the item is equipped
        public Attributes Attributes; // Default attributes for the item
        public bool TakesDamage; // True if this item can be damaged
        public Inventory.SlotType SlotType; // Type of inventory slot this item equips to
        // Item type specific members
        public int WeaponDamage, WeaponDamageType;
        public WeaponType WType;
        public int RangedProjectile;
        public string RangedAnimation;
        public float Range;
        public int ArmourLevel;
        public uint EatEffectsLength;
        public uint ImageID;
        public string MiscData;

        // Linked list
        public static Item FirstItem;
        public Item NextItem;

        // Constructor
        public Item()
        {
            Attributes = new Attributes();
            Init();

            // Assign free id
            for(uint id = 0; id < uint.MaxValue; id++)
                if (!Index.ContainsKey(id))
                {
                    ID = id;
                    break;
                }

            /*
            for (ushort i = 0; i < 65535; ++i)
            {
                if (Index[i] == null)
                {
                    Index[i] = this;
                    ID = i;
                    Attributes = new Attributes();
                    Init();
                    NextItem = FirstItem;
                    FirstItem = this;
                    return;
                }
            }
            throw new ItemException("Maximum number of items already created!");*/
        }

        private Item(bool DoNotAssignID)
        {
            Attributes = new Attributes();
            Init();
            NextItem = FirstItem;
            FirstItem = this;
            return;
        }

        // Sets default values
        public void Init()
        {
            Name = "";
            ExclusiveRace = "";
            ExclusiveClass = "";
            Script = "";
            Method = "";
            ThumbnailTexID = 0;
            IType = ItemType.Weapon;
            SlotType = Inventory.SlotType.Weapon;
            Stackable = false;
            Value = 1;
            Mass = 1;
            ImageID = 65535;
            RangedProjectile = 10000;
            RangedAnimation = "";
            MiscData = "";
        }

        // Removes this item from the index and linked list
        public void Delete()
        {
            Index.Remove(ID);
            Item I = FirstItem;
            if (I == this)
            {
                FirstItem = NextItem;
            }
            else
            {
                while (I != null)
                {
                    if (I.NextItem == this)
                    {
                        I.NextItem = NextItem;
                        NextItem = null;
                        break;
                    }
                    I = I.NextItem;
                }
            }
        }

        // Finds an item from its name
        public static Item Find(string Name)
        {
            Name = Name.ToUpper();
            Item I = FirstItem;
            while (I != null)
            {
                if (I.Name.ToUpper() == Name)
                {
                    return I;
                }
                I = I.NextItem;
            }
            return null;
        }

        // Loads all items from a file and returns the number loaded
        public static int Load(string filename)
        {
            if (!File.Exists(filename))
                return -1;

            // Create database connection
            SqliteConnection databaseConnection = new SqliteConnection();
            databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", filename);
            databaseConnection.Open();

            // Query table to get all items
            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + ItemsSQLTable;

            int itemCount = 0;
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Item item = new Item(true);
                item.ID = (uint)reader.GetInt64(reader.GetOrdinal("ID"));
                item.Name = reader.GetString(reader.GetOrdinal("Name"));
                item.ExclusiveRace = reader.GetString(reader.GetOrdinal("Race"));
                item.ExclusiveClass = reader.GetString(reader.GetOrdinal("Class"));
                item.Script = reader.GetString(reader.GetOrdinal("Script"));
                item.Method = reader.GetString(reader.GetOrdinal("Method"));
                item.IType = (ItemType)reader.GetByte(reader.GetOrdinal("Type"));
                item.Value = reader.GetInt32(reader.GetOrdinal("Value"));
                item.Mass = reader.GetInt32(reader.GetOrdinal("Mass"));
                item.TakesDamage = reader.GetBoolean(reader.GetOrdinal("TakesDamage"));
                item.ThumbnailTexID = reader.GetInt32(reader.GetOrdinal("ThumbnailID"));

                // Read gubbin data out
                byte[] buffer = (byte[])reader.GetValue(reader.GetOrdinal("Gubbins"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
                {
                    uint gubbinCount = binaryReader.ReadUInt32();
                    item.TempGubbins = new uint[gubbinCount];

                    for (int i = 0; i < gubbinCount; i++)
                        item.TempGubbins[i] = binaryReader.ReadUInt32();
                }

                item.SlotType = (Inventory.SlotType)reader.GetInt16(reader.GetOrdinal("SlotType"));
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
                    switch (item.IType)
                    {
                        case ItemType.Weapon:
                            item.WeaponDamage = binaryReader.ReadInt32();
                            item.WeaponDamageType = binaryReader.ReadInt32();
                            item.WType = (WeaponType)binaryReader.ReadByte();
                            item.RangedProjectile = binaryReader.ReadInt32();
                            item.Range = binaryReader.ReadSingle();
                            item.RangedAnimation = binaryReader.ReadString();
                            break;

                        case ItemType.Armour:
                            item.ArmourLevel = binaryReader.ReadInt32();
                            break;

                        case ItemType.Potion:
                        case ItemType.Ingredient:
                            item.EatEffectsLength = binaryReader.ReadUInt32();
                            break;

                        case ItemType.Image:
                            item.ImageID = binaryReader.ReadUInt32();
                            break;

                        case ItemType.Other:
                            item.MiscData = binaryReader.ReadString();
                            break;
                    }
                }

                Index.Add(item.ID, item);
                itemCount++;
            }

            databaseConnection.Close();

            return itemCount;

            #region Old binary file reader
            /*
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return -1;
            }

            int Items = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                Item I = new Item(true);
                I.ID = F.ReadUInt16();
                Index[I.ID] = I;
                I.Name = Blitz.ReadString(F);
                I.ExclusiveRace = Blitz.ReadString(F);
                I.ExclusiveClass = Blitz.ReadString(F);
                I.Script = Blitz.ReadString(F);

                if (!I.Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                    I.Script += ".cs";

                I.Method = Blitz.ReadString(F);
                I.IType = (ItemType) F.ReadByte();
                I.Value = F.ReadInt32();
                I.Mass = F.ReadUInt16();
                I.TakesDamage = F.ReadBoolean();
                I.ThumbnailTexID = F.ReadUInt16();

                // Temporary (Remove after converted)
//                 for (int j = 0; j < 6; ++j)
//                 {
//                     F.ReadUInt16();
//                 }
//                 F.ReadUInt16();
//                 F.ReadUInt16();

                // Add after converted
                byte GubbinCount = F.ReadByte();
                I.TempGubbins = new ushort[GubbinCount];

                for (int i = 0; i < GubbinCount; ++i)
                {
                    I.TempGubbins[i] = F.ReadUInt16();
                }

                I.SlotType = (Inventory.SlotType) F.ReadUInt16();
                I.Stackable = F.ReadBoolean();
                for (int j = 0; j < Attributes.TotalAttributes; ++j)
                {
                    I.Attributes.Value[j] = (int) F.ReadUInt16() - 5000;
                }
                switch (I.IType)
                {
                    case ItemType.Weapon:
                        I.WeaponDamage = F.ReadUInt16();
                        I.WeaponDamageType = F.ReadUInt16();
                        I.WType = (WeaponType) F.ReadUInt16();
                        I.RangedProjectile = F.ReadUInt16();
                        I.Range = F.ReadSingle();
                        I.RangedAnimation = Blitz.ReadString(F);
                        break;
                    case ItemType.Armour:
                        I.ArmourLevel = F.ReadUInt16();
                        break;
                    case ItemType.Potion:
                        goto case ItemType.Ingredient;
                    case ItemType.Ingredient:
                        I.EatEffectsLength = F.ReadUInt16();
                        break;
                    case ItemType.Image:
                        I.ImageID = F.ReadUInt16();
                        break;
                    case ItemType.Other:
                        I.MiscData = Blitz.ReadString(F);
                        break;
                }

                Items++;
            }

            F.Close();
            return Items;
             */
            #endregion
        }

        // Saves all items to a file
        public static bool Save(string filename)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                // Create database connection
                SqliteConnection databaseConnection = new SqliteConnection();
                databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", filename);
                databaseConnection.Open();

                // Update items if they exist or insert them if they don't 
                foreach (Item item in Index.Values)
                {
                    // Check if entry already exists
                    IDbCommand cmd = databaseConnection.CreateCommand();
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE ID={1}", ItemsSQLTable, item.ID);
                    IDataReader reader = cmd.ExecuteReader();
                    bool updateQuery = reader.Read();


                    // Create query to update or insert
                    cmd = databaseConnection.CreateCommand();
                    // Changable symbol because SET uses $ and INSERT uses @

                    string assignSymbol = "@";
                    if (updateQuery)
                    {
                        // Update entry
                        cmd.CommandText = "UPDATE Items SET Name=$Name, Race=$Race, Class=$Class, Script=$Script, Method=$Method, Type=$Type, Value=$Value, Mass=$Mass, TakesDamage=$TakesDamage, ThumbnailID=$ThumbnailID, Gubbins=$Gubbins, SlotType=$SlotType, Stackable=$Stackable, Attributes=$Attributes, ItemTypeData=$ItemTypeData " +
                            "WHERE ID=@ID";
                        assignSymbol = "$";
                    }
                    else
                    {
                        // Insert entry
                        cmd.CommandText = "INSERT INTO Items (ID, Name, Race, Class, Script, Method, Type, Value, Mass, TakesDamage, ThumbnailID, Gubbins, SlotType, Stackable, Attributes, ItemTypeData)" +
                            " VALUES (@ID,@Name,@Race,@Class,@Script, @Method, @Type, @Value, @Mass, @TakesDamage, @ThumbnailID, @Gubbins, @SlotType, @Stackable, @Attributes, @ItemTypeData)";
                    }


                    // Add parameters for query that work for either UPDATE OR SET so only one change needs to be made
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = (long)item.ID });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Name", Value = item.Name });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Race", Value = item.ExclusiveRace });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Class", Value = item.ExclusiveClass });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Script", Value = item.Script });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Method", Value = item.Method });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Type", Value = (byte)item.IType });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Value", Value = item.Value });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Mass", Value = item.Mass });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "TakesDamage", Value = item.TakesDamage });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "ThumbnailID", Value = item.ThumbnailTexID });

                    // Write gubbin to byte array, first uint is length, every uint  after is a gubbin value. (changed values to uint for more space later on when reading)
                    using (BinaryWriter gubbinStream = new BinaryWriter(new MemoryStream()))
                    {
                        if (item.TempGubbins != null)
                        {
                            uint gubbinCount = (uint)item.TempGubbins.Length;
                            gubbinStream.Write(gubbinCount);
                            for (int i = 0; i < gubbinCount; ++i)
                            {
                                gubbinStream.Write(item.TempGubbins[i]);
                            }
                        }
                        else
                            gubbinStream.Write((uint)0);
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Gubbins", Value = (gubbinStream.BaseStream as MemoryStream).ToArray() });
                    }

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "SlotType", Value = (ushort)item.SlotType });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Stackable", Value = item.Stackable });

                    // Write attributes to byte array, convert to uint values so more attributes can be used later on
                    using (BinaryWriter attributeWriter = new BinaryWriter(new MemoryStream()))
                    {
                        attributeWriter.Write((uint)Attributes.TotalAttributes);
                        for (int j = 0; j < Attributes.TotalAttributes; ++j)
                        {
                            attributeWriter.Write((int)item.Attributes.Value[j]); // Note - nfc why it deducts 5000
                        }

                        cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Attributes", Value = (attributeWriter.BaseStream as MemoryStream).ToArray() });
                    }

                    // Write item type specific data as byte array - so sqlite doesnt end up with a shit tonne of fields.
                    using (BinaryWriter typeDataWriter = new BinaryWriter(new MemoryStream()))
                    {
                        switch (item.IType)
                        {
                            case ItemType.Weapon:
                                typeDataWriter.Write(item.WeaponDamage);
                                typeDataWriter.Write(item.WeaponDamageType);
                                typeDataWriter.Write((byte)item.WType);
                                typeDataWriter.Write(item.RangedProjectile);
                                typeDataWriter.Write(item.Range);
                                typeDataWriter.Write(item.RangedAnimation);
                                break;
                            case ItemType.Armour:
                                typeDataWriter.Write(item.ArmourLevel);
                                break;
                            case ItemType.Potion:
                            case ItemType.Ingredient:
                                typeDataWriter.Write(item.EatEffectsLength);
                                break;
                            case ItemType.Image:
                                typeDataWriter.Write(item.ImageID);
                                break;
                            case ItemType.Other:
                                typeDataWriter.Write(item.MiscData);
                                break;
                        }
                        var memoryStream = typeDataWriter.BaseStream as MemoryStream;
                        if (memoryStream != null)
                            cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "ItemTypeData", Value = memoryStream });
                    }


                    if (updateQuery ? cmd.ExecuteReader().RecordsAffected > 0 : cmd.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine("Inserted/Updated item: " + item.Name);
                    }
                    else
                        throw new Exception("Query did not affect anything.");
                }
                return true;

                #region Old binary file
                /*
                BinaryWriter F = Blitz.WriteFile(Filename);
                if (F == null)
                {
                    return false;
                }
                Item I = FirstItem;
                while (I != null)
                {
                    F.Write(I.ID);
                    Blitz.WriteString(I.Name, F);
                    Blitz.WriteString(I.ExclusiveRace, F);
                    Blitz.WriteString(I.ExclusiveClass, F);

                    string writeScript = I.Script;
                    if (writeScript.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                        writeScript = writeScript.Substring(0, writeScript.Length - 3);

                    Blitz.WriteString(writeScript, F);
                    Blitz.WriteString(I.Method, F);
                    F.Write((byte) I.IType);
                    F.Write(I.Value);
                    F.Write((ushort) I.Mass);
                    F.Write(I.TakesDamage);
                    F.Write(I.ThumbnailTexID);
//                     for (int j = 0; j < 6; ++j)
//                     {
//                         if (I.Gubbins[j])
//                         {
//                             F.Write((ushort) 1);
//                         }
//                         else
//                         {
//                             F.Write((ushort) 0);
//                         }
//                     }
//                     F.Write(I.MMeshID);
//                     F.Write(I.FMeshID);


                    F.Write((byte)I.GubbinTemplates.Count);
                    foreach (GubbinTemplate T in I.GubbinTemplates)
                    {
                        F.Write((ushort)T.ID);
                    }


                    F.Write((ushort) I.SlotType);
                    F.Write(I.Stackable);
                    for (int j = 0; j < Attributes.TotalAttributes; ++j)
                    {
                        F.Write((ushort) (I.Attributes.Value[j] + 5000));
                    }

                    switch (I.IType)
                    {
                        case ItemType.Weapon:
                            F.Write((ushort) I.WeaponDamage);
                            F.Write((ushort) I.WeaponDamageType);
                            F.Write((ushort) I.WType);
                            F.Write((ushort) I.RangedProjectile);
                            F.Write(I.Range);
                            Blitz.WriteString(I.RangedAnimation, F);
                            break;
                        case ItemType.Armour:
                            F.Write((ushort) I.ArmourLevel);
                            break;
                        case ItemType.Potion:
                            goto case ItemType.Ingredient;
                        case ItemType.Ingredient:
                            F.Write((ushort) I.EatEffectsLength);
                            break;
                        case ItemType.Image:
                            F.Write(I.ImageID);
                            break;
                        case ItemType.Other:
                            Blitz.WriteString(I.MiscData, F);
                            break;
                    }
                    I = I.NextItem;
                }
                F.Close();

                return true;
                                  */
                #endregion
            }
            else
            {
                return false;
            }

        }
    }

    // An actual instance of an item in the game world
    public class ItemInstance
    {
        // Members
        public Item Item;
        public Attributes Attributes;
        public byte ItemHealth; // Health remaining as a percentage

        // Constructor
        public ItemInstance(Item I)
        {
            Item = I;
            ItemHealth = 100;
            Attributes = new Attributes();
            for (int j = 0; j < Attributes.TotalAttributes; ++j)
            {
                Attributes.Value[j] = Item.Attributes.Value[j];
            }
        }

        // Returns the length in bytes of an item instance in string form
        public const int ItemInstancestringLength = 83;
        // Converts this item instance to a string
        public string ConvertToString()
        {
            string Pa = Blitz.StrFromInt((int)Item.ID, 2);
            for (int i = 0; i < Attributes.TotalAttributes; ++i)
            {
                Pa += Blitz.StrFromInt(Attributes.Value[i] + 5000, 2);
            }
            Pa += Blitz.StrFromInt(ItemHealth, 1);
            return Pa;
        }

        // Reconstructs an item instance from a string
        public static ItemInstance ConvertFromstring(string Pa)
        {
            if (Pa.Length < ItemInstancestringLength)
            {
                return null;
            }

            ItemInstance II = new ItemInstance(Item.Index[(uint)Blitz.IntFromStr(Pa.Substring(0, 2))]);
            int Offset = 2;
            for (int i = 0; i < Attributes.TotalAttributes; ++i)
            {
                II.Attributes.Value[i] = (ushort) Blitz.IntFromStr(Pa.Substring(Offset, 2));
                Offset += 2;
            }
            II.ItemHealth = (byte) Blitz.IntFromStr(Pa.Substring(Offset, 1));
            return II;
        }

        // Writes an item instance to a stream
        public static bool Write(BinaryWriter F, ItemInstance I)
        {
            if (I == null)
            {
                F.Write((ushort) 65535);
                return false;
            }

            F.Write(I.Item.ID);
            for (int j = 0; j < Attributes.TotalAttributes; ++j)
            {
                F.Write((ushort) (I.Attributes.Value[j] + 5000));
            }
            F.Write(I.ItemHealth);

            return true;
        }

        // Reads an item instance from a stream
        public static ItemInstance Read(BinaryReader F)
        {
            ushort ID = F.ReadUInt16();
            if (ID == 65535 || Item.Index[ID] == null)
            {
                return null;
            }

            ItemInstance I = new ItemInstance(Item.Index[ID]);
            for (int j = 0; j < Attributes.TotalAttributes; ++j)
            {
                I.Attributes.Value[j] = F.ReadUInt16() - 5000;
            }
            I.ItemHealth = F.ReadByte();

            return I;
        }

        // Compares two item instances and returns true if they are identical
    }

    // Item creation exception
    internal class ItemException : Exception
    {
        public ItemException(string Message) : base(Message)
        {
        }
    }
}