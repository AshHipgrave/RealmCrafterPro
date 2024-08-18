using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Community.CsharpSqlite.SQLiteClient;
using System.Data;
using System.Reflection;

namespace Server_Converter
{
    class Program
    {
        const int TotalAttributes = 40;
        const int TotalFactions = 100;
        const int TotalDamageTypes = 20;
        public enum ItemType
        {
            Weapon = 1,
            Armour = 2,
            Ring = 3,
            Potion = 4,
            Ingredient = 5,
            Image = 6,
            Other = 7
        }


        static void Main(string[] args)
        {
            string itemFile = "Items.dat";
            string itemDB = "Items.s3db";
            string factionFile = "Factions.dat";
            string factionDB = "Factions.s3db";
            string spellFile = "Spells.dat";
            string spellDB = "Spells.s3db";
            string actorFile = "Actors.dat";
            string actorDB = "Actors.s3db";
#if RELEASE
            itemFile = args[0];
            itemDB = args[1];
            factionFile = args[2];
            factionDB = args[3];
            spellFile = args[4];
            spellDB = args[5];
            actorFile = args[6];
            actorDB = args[7];
#endif

            ConvertActors(actorFile, actorDB);
            ConvertSpells(spellFile, spellDB);
            ConvertItems(itemFile, itemDB);
            ConvertFactions(factionFile, factionDB);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

        }

        /// <summary>
        /// DV: Added this since there seems to be no CREATE TABLE calls.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string GetSQLResource(string name)
        {
            string findName = "Server_Converter." + name;

            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(findName);
                StreamReader reader = new StreamReader(stream);

                string output = reader.ReadToEnd();
                reader.Close();


                return output;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error: Couldn't find SQL resource '{0}'", findName);
            }

            return "";
        }

        public static void ConvertActors(string actorsFile, string actorsDB)
        {
            // DV: Delete if it exists (since we're supposed to be doing a full conversion, not stacking
            // new data onto existing data!
            File.Delete(actorsDB);

            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", actorsDB);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();

            // DV: Create actors table first
            IDbCommand tblCmd = databaseConnection.CreateCommand();
            tblCmd.CommandText = GetSQLResource("Actors.sql");
            tblCmd.ExecuteNonQuery();

            BinaryReader F = Blitz.ReadFile(actorsFile);

            int Actors = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                IDbCommand cmd = databaseConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO Actors (ID, Race, Class, Description, StartArea, StartPortal, MaleAnimationSet, FemaleAnimationSet, Scale, Radius, MaleMesh, " +
                "FemaleMesh, GubbinData, Beards, MaleHair, FemaleHair, MaleFaceTextures, FemaleFaceTextures, MaleBodyTextures, FemaleBodyTextures, MaleSpeech, FemaleSpeech, BloodTexture, " +
                "Attributes, Resistances, Genders, Playable, Ridable, Aggression, AggressionRange, TradeMode, EnvironmentType, InventorySlots, DefaultDamageType, DefaultFaction, " +
                "XPMultiplier, PolygonCollision) VALUES (@ID, @Race, @Class, @Description, @StartArea, @StartPortal, @MaleAnimationSet, @FemaleAnimationSet, @Scale, @Radius, @MaleMesh, " +
                "@FemaleMesh, @GubbinData, @Beards, @MaleHair, @FemaleHair, @MaleFaceTextures, @FemaleFaceTextures, @MaleBodyTextures, @FemaleBodyTextures, @MaleSpeech, @FemaleSpeech, @BloodTexture, " +
                "@Attributes, @Resistances, @Genders, @Playable, @Ridable, @Aggression, @AggressionRange, @TradeMode, @EnvironmentType, @InventorySlots, @DefaultDamageType, @DefaultFaction, " +
                "@XPMultiplier, @PolygonCollision)";

                uint id = F.ReadUInt16();
                string race = Blitz.ReadString(F);
                string exClass = Blitz.ReadString(F);
                string desc = Blitz.ReadString(F);
                string startArea = Blitz.ReadString(F);
                string startPortal = Blitz.ReadString(F);
                uint maleAnimationSet = F.ReadUInt16();
                uint femaleAnimationSet = F.ReadUInt16();
                float scale = F.ReadSingle();
                float radius = F.ReadSingle();

                uint maleMesh = F.ReadUInt16();
                uint femaleMesh = F.ReadUInt16();

                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = (long)id });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Race", Value = race });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Class", Value = exClass });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Description", Value = desc });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@StartArea", Value = startArea });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@StartPortal", Value = startPortal });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleAnimationSet", Value = (long)maleAnimationSet });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleAnimationSet", Value = (long)femaleAnimationSet });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Scale", Value = scale });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Radius", Value = radius });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleMesh", Value = (long)maleMesh });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleMesh", Value = (long)femaleMesh });

                byte gubbinCount = F.ReadByte();
                ushort[] tempGubbins = new ushort[gubbinCount];

                for (int i = 0; i < gubbinCount; ++i)
                {
                    tempGubbins[i] = F.ReadUInt16();
                }

                // Write gubbins
                using(BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    writer.Write(gubbinCount);
                    for(int i = 0; i < gubbinCount; i++)
                        writer.Write(tempGubbins[i]);

                    var memoryStream = writer.BaseStream as MemoryStream;
                    if(memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@GubbinData", Value = memoryStream.ToArray() });
                    }
                }

                // Write beards
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    byte Count = F.ReadByte();
                    writer.Write((uint)Count);
                    for (int bid = 0; bid < Count; ++bid)
                    {
                        gubbinCount = F.ReadByte();
                        writer.Write((uint)gubbinCount);

                        for (int i = 0; i < gubbinCount; ++i)
                            writer.Write((uint)F.ReadUInt16());
                    }

                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Beards", Value = memoryStream.ToArray() });
                    }
                }

                // Write male hair
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    byte count = F.ReadByte();
                    writer.Write((uint)count);

                    for (int bid = 0; bid < count; ++bid)
                    {
                        gubbinCount = F.ReadByte();
                        writer.Write((uint)gubbinCount);

                        for (int i = 0; i < gubbinCount; ++i)
                            writer.Write((uint)F.ReadUInt16());

                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleHair", Value = memoryStream.ToArray() });
                    }
                }


                // Write female hair
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    byte count = F.ReadByte();
                    writer.Write((uint)count);

                    for (int bid = 0; bid < count; ++bid)
                    {
                        gubbinCount = F.ReadByte();
                        writer.Write((uint)gubbinCount);

                        for (int i = 0; i < gubbinCount; ++i)
                            writer.Write((uint)F.ReadUInt16());

                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleHair", Value = memoryStream.ToArray() });
                    }
                }


                // Write male face IDs
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    uint count = (uint)F.ReadByte();
                    writer.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleFaceTextures", Value = memoryStream.ToArray() });
                    }
                }
                // Write female face IDs
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    uint count = (uint)F.ReadByte();
                    writer.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleFaceTextures", Value = memoryStream.ToArray() });
                    }
                }

                // Write male body IDs
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    uint count = (uint)F.ReadByte();
                    writer.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleBodyTextures", Value = memoryStream.ToArray() });
                    }
                }

                // Write female body IDs
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    uint count = (uint)F.ReadByte();
                    writer.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleBodyTextures", Value = memoryStream.ToArray() });
                    }
                }

                // Write Male speech
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    uint count = 16;
                    writer.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleSpeech", Value = memoryStream.ToArray() });
                    }
                }

                // Write Female speech
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    uint count = 16;
                    writer.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleSpeech", Value = memoryStream.ToArray() });
                    }
                }

                // Server has this commented out, so I'm commenting it out here too.
                //for (int i = 0; i < 6; ++i)
                //    A.HairColors[i] = F.ReadInt32();

                uint bloodTex = F.ReadUInt16();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@BloodTexture", Value = (long)bloodTex });


                // Write attributes
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    writer.Write((uint)TotalAttributes);
                    for (int i = 0; i < TotalAttributes; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                        writer.Write((uint)F.ReadUInt16());
                    }
                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Attributes", Value = memoryStream.ToArray() });
                    }
                }

                // Write resistances
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                {
                    writer.Write((uint)TotalDamageTypes);
                    for (int i = 0; i < TotalDamageTypes; ++i)
                    {
                        writer.Write((uint)F.ReadUInt16());
                    }

                    var memoryStream = writer.BaseStream as MemoryStream;
                    if (memoryStream != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Resistances", Value = memoryStream.ToArray() });
                    }
                }

                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Genders", Value = F.ReadByte() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Playable", Value = F.ReadBoolean() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Ridable", Value = F.ReadBoolean() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Aggression", Value = F.ReadByte() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@AggressionRange", Value = F.ReadInt32() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@TradeMode", Value = F.ReadByte() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@EnvironmentType", Value = F.ReadByte() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@InventorySlots", Value = F.ReadInt32() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@DefaultDamageType", Value = F.ReadByte() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@DefaultFaction", Value = F.ReadByte() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@XPMultiplier", Value = F.ReadInt32() });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@PolygonCollision", Value = F.ReadBoolean() });

                if (cmd.ExecuteNonQuery() > 0)
                    Console.WriteLine("Inserted Actor: " + race + " " + exClass);
                else
                    Console.WriteLine("Actor " + race + " " + exClass + " failed.");

                Actors++;
            }

            F.Close();

            databaseConnection.Close();
        }

        public static void ConvertSpells(string spellFile, string spellDB)
        {
            // DV: Delete if it exists (since we're supposed to be doing a full conversion, not stacking
            // new data onto existing data!
            File.Delete(spellDB);

            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", spellDB);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();

            // DV: Create table first
            IDbCommand tblCmd = databaseConnection.CreateCommand();
            tblCmd.CommandText = GetSQLResource("Spells.sql");
            tblCmd.ExecuteNonQuery();

            BinaryReader F = Blitz.ReadFile(spellFile);


            int Abilities = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                //Ability A = new Ability(true);
                uint id = F.ReadUInt16();
                //Index[A.ID] = A;
                string name = Blitz.ReadString(F);
                string desc = Blitz.ReadString(F);
                int thumb = F.ReadUInt16();
                string race = Blitz.ReadString(F);
                string exClass = Blitz.ReadString(F);
                int recharge = F.ReadInt32();
                string script = Blitz.ReadString(F);
               string method = Blitz.ReadString(F);

               // Create query
               IDbCommand cmd = databaseConnection.CreateCommand();
               cmd.CommandText = "INSERT INTO Spells (ID, Name, Description, ThumbnailID, Race, Class, RechargeTime, Script, Method) VALUES (@ID, @Name,@Description, @ThumbnailID, @Race, @Class, @RechargeTime, @Script, @Method)";
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = (long)id });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Name", Value = name });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Description", Value = desc });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ThumbnailID", Value = thumb });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Race", Value = race });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Class", Value = exClass });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@RechargeTime", Value = recharge });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Script", Value = script });
               cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Method", Value = method });

               if (cmd.ExecuteNonQuery() > 0)
                   Console.WriteLine("Inserted Spell: " + name);
               else
                   Console.WriteLine("Spell " + name + " failed.");
                Abilities++;
            }

            F.Close();
        }

        public static void ConvertFactions(string factionFile, string factionDB)
        {
            // DV: Delete if it exists (since we're supposed to be doing a full conversion, not stacking
            // new data onto existing data!
            File.Delete(factionDB);

            // Connect to db
            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", factionDB);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();

            // DV: Create table first
            IDbCommand tblCmd = databaseConnection.CreateCommand();
            tblCmd.CommandText = GetSQLResource("Factions.sql");
            tblCmd.ExecuteNonQuery();

            BinaryReader F = Blitz.ReadFile(factionFile);

            int FactionsLoaded = 0;

            string[] factionNames = new string[TotalFactions];
            byte[,] ratings = new byte[TotalFactions, TotalFactions];
            for (int i = 0; i < TotalFactions; ++i)
            {
                factionNames[i] = Blitz.ReadString(F);
                if (factionNames[i] != "")
                {
                    FactionsLoaded++;
                }
            }

            for (int i = 0; i < TotalFactions; ++i)
            {
                for (int j = 0; j < TotalFactions; ++j)
                {
                    ratings[i, j] = F.ReadByte();
                }
            }

            // Write sql data
            for (int i = 0; i < TotalFactions; i++)
                if (factionNames[i] != "")
                {
                    // Create query
                    IDbCommand cmd = databaseConnection.CreateCommand();
                    cmd.CommandText = "INSERT INTO Factions (ID, Name, Ratings) VALUES (@ID, @Name,@Ratings)";
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Name", Value = factionNames[i] });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = i });

                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                    {
                        writer.Write(TotalFactions);
                        // Write factions as ints incase we want wider values in the future
                        for (int j = 0; j < TotalFactions; j++)
                            writer.Write((int)ratings[i, j]);

                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Ratings", Value = (writer.BaseStream as MemoryStream).ToArray() });
                    }
                    if (cmd.ExecuteNonQuery() > 0)
                        Console.WriteLine("Inserted faction: " + factionNames[i]);
                    else
                        Console.WriteLine("Faction " + factionNames[i] + " failed.");
                }

            F.Close();

            databaseConnection.Close();
        }


        public static void ConvertItems(string itemFile, string itemDB)
        {
            // DV: Delete if it exists (since we're supposed to be doing a full conversion, not stacking
            // new data onto existing data!
            File.Delete(itemDB);

            // Connect to db
            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", itemDB);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();

            // DV: Create table first
            IDbCommand tblCmd = databaseConnection.CreateCommand();
            tblCmd.CommandText = GetSQLResource("Items.sql");
            tblCmd.ExecuteNonQuery();

            // Convert items from binary to SQLite

            BinaryReader F = Blitz.ReadFile(itemFile);

            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                // Create query
                IDbCommand cmd = databaseConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO Items (ID, Name, Race, Class, Script, Method, Type, Value, Mass, TakesDamage, ThumbnailID, Gubbins, SlotType, Stackable, Attributes, ItemTypeData)" +
                    " VALUES (@ID,@Name,@Race,@Class,@Script, @Method, @Type, @Value, @Mass, @TakesDamage, @ThumbnailID, @Gubbins, @SlotType, @Stackable, @Attributes, @ItemTypeData)";



                // Read item data
                ushort itemID = F.ReadUInt16();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = (long)itemID });

                string name = Blitz.ReadString(F);
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Name", Value = name });

                string race = Blitz.ReadString(F);
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Race", Value = race });

                string exClass = Blitz.ReadString(F);
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Class", Value = exClass });

                string script = Blitz.ReadString(F);
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Script", Value = script });
                // if (!I.Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                // I.Script += ".cs";

                string method = Blitz.ReadString(F);
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Method", Value = method });

                byte itemType = F.ReadByte();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Type", Value = itemType });

                int price = F.ReadInt32();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Value", Value = price });

                int mass = F.ReadUInt16();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Mass", Value = mass });

                bool takesDamage = F.ReadBoolean();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@TakesDamage", Value = takesDamage });

                int thumbnailID = F.ReadUInt16();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ThumbnailID", Value = thumbnailID });

                // Write gubbin to byte array, first uint is length, every uint  after is a gubbin value. (changed values to uint for more space later on when reading)
                using (BinaryWriter gubbinStream = new BinaryWriter(new MemoryStream()))
                {
                    byte GubbinCount = F.ReadByte();
                    gubbinStream.Write((uint)GubbinCount);
                    for (int i = 0; i < GubbinCount; ++i)
                    {
                        gubbinStream.Write((uint)F.ReadUInt16());
                    }

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Gubbins", Value = (gubbinStream.BaseStream as MemoryStream).ToArray() });
                }

                ushort slotType = F.ReadUInt16();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@SlotType", Value = slotType });

                bool stackable = F.ReadBoolean();
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Stackable", Value = stackable });

                // Write attributes to byte array, convert to uint values so more attributes can be used later on
                using (BinaryWriter attributeWriter = new BinaryWriter(new MemoryStream()))
                {
                    attributeWriter.Write((uint)TotalAttributes);
                    for (int j = 0; j < TotalAttributes; ++j)
                    {
                        attributeWriter.Write((int)(F.ReadUInt16())); // Note - nfc why it deducts 5000
                    }

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Attributes", Value = (attributeWriter.BaseStream as MemoryStream).ToArray() });
                }

                // Write item type specific data as byte array - so sqlite doesnt end up with a shit tonne of fields.
                using (BinaryWriter typeDataWriter = new BinaryWriter(new MemoryStream()))
                {
                    switch ((ItemType)itemType)
                    {
                        case ItemType.Weapon:
                            ushort wepDamage = F.ReadUInt16();
                            typeDataWriter.Write((int)wepDamage);
                            ushort damageType = F.ReadUInt16();
                            typeDataWriter.Write((int)damageType);
                            ushort weaponType = F.ReadUInt16();
                            typeDataWriter.Write((byte)weaponType);
                            ushort projectile = F.ReadUInt16();
                            typeDataWriter.Write((int)projectile);
                            float range = F.ReadSingle();
                            typeDataWriter.Write(range);
                            string rangedAnimation = Blitz.ReadString(F);
                            typeDataWriter.Write(rangedAnimation);
                            break;
                        case ItemType.Armour:
                            ushort armourLevel = F.ReadUInt16();
                            typeDataWriter.Write((int)armourLevel);
                            break;
                        case ItemType.Potion:
                        case ItemType.Ingredient:
                            ushort effectLength = F.ReadUInt16();
                            typeDataWriter.Write((uint)effectLength);
                            break;
                        case ItemType.Image:
                            ushort imageID = F.ReadUInt16();
                            typeDataWriter.Write((uint)imageID);
                            break;
                        case ItemType.Other:
                            string misc = Blitz.ReadString(F);
                            typeDataWriter.Write(misc);
                            break;
                    }
                    if(typeDataWriter.BaseStream.Length > 0)
                        cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ItemTypeData", Value = (typeDataWriter.BaseStream as MemoryStream).ToArray() });
                }

                if (cmd.ExecuteNonQuery() > 0)
                {
                    Console.WriteLine("Inserted item: " + name);
                }
                else
                    throw new Exception("something didnt work sonny");
            }

            F.Close();

            databaseConnection.Close();
        }



    }
}
