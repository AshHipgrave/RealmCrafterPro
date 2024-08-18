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
// Realm Crafter Actors module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port October 2006
namespace RealmCrafter
{
    using RenderingServices;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using Community.CsharpSqlite.SQLiteClient;
using System.Data;

    // Actor template class
    public class Actor
    {
        // Constants
        public enum AIMode
        {
            Wait = 0,
            Patrol = 1,
            Run = 2,
            Chase = 3,
            PatrolPause = 4,
            Pet = 5,
            PetChase = 6,
            PetWait = 7
        } ;

        public enum Speech
        {
            Greet1 = 0,
            Greet2 = 1,
            Bye1 = 2,
            Bye2 = 3,
            Attack1 = 4,
            Attack2 = 5,
            Hit1 = 6,
            Hit2 = 7,
            RequestHelp = 8,
            Death = 9,
            FootstepDry = 10,
            FootstepWet = 11
        } ;

        public enum Environment
        {
            Amphibious = 0,
            Swim = 1,
            Fly = 2,
            Walk = 3
        } ;

        // Get individual bits from an unsigned short
        public static bool GetFlag(ushort Number, int Flag)
        {
            return ((Number >> Flag) & 1) == 1;
        }

        // Factions
        public const int TotalFactions = 100;
        public static string[] FactionNames = new string[TotalFactions];
        public static byte[,] FactionDefaultRatings = new byte[TotalFactions,TotalFactions];

        /*  ONE DAY WE'LL USE THIS AND ALL WILL BE RIGHT WITH THE WORLD. - Ben
        public static Dictionary<string, Faction> Factions = new Dictionary<string, Faction>(); -

        public class Faction
        {
            public string Name;
            public Dictionary<string, int> Ratings;

            public Faction(string name)
            {
                Name = name;
                Ratings = new Dictionary<string, int>();
            }
        }
        */

        public const string FactionDatabase = @"Data\Server Data\Factions.s3db";
        const string SQLFactionsTable = "Factions";

        public static int LoadFactions(string filename)
        {
            if (!File.Exists(filename))
                return -1;

            for (int i = 0; i < TotalFactions; i++)
                FactionNames[i] = "";

            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", filename);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();

            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + SQLFactionsTable;
             IDataReader reader = cmd.ExecuteReader();
             int factionCount = 0;
             while (reader.Read())
             {
                 int index = reader.GetInt32(reader.GetOrdinal("ID"));
                 FactionNames[index] = reader.GetString(reader.GetOrdinal("Name"));
                 byte[] data = (byte[])reader.GetValue(reader.GetOrdinal("Ratings"));

                 // Read ratings out
                 using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                 {
                     // This is for future use when we make factions not a fixed array
                     int totalFactions = binaryReader.ReadInt32();

                     // Reads and writes ratings as int for larger range in future
                     for (int i = 0; i < TotalFactions; i++)
                         FactionDefaultRatings[index, i] = (byte)binaryReader.ReadInt32();
                 }

                 factionCount++;
             }
            databaseConnection.Close();
            //throw new Exception("FACTION COUNT " + factionCount);
            return factionCount;

            #region Old binary file
            /*
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return -1;
            }

            int FactionsLoaded = 0;
            for (int i = 0; i < TotalFactions; ++i)
            {
                FactionNames[i] = Blitz.ReadString(F);
                if (FactionNames[i] != "")
                {
                    FactionsLoaded++;
                }
            }

            for (int i = 0; i < TotalFactions; ++i)
            {
                for (int j = 0; j < TotalFactions; ++j)
                {
                    FactionDefaultRatings[i, j] = F.ReadByte();
                }
            }
            
            F.Close();*/
            #endregion
        }

        public static bool SaveFactions(string filename)
        {
            if (!File.Exists(filename))
                return false;

            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", filename);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();


            // Write sql data
            for (int i = 0; i < TotalFactions; i++)
                if (FactionNames[i] != "")
                {
                    // Check if entry already exists 
                    IDbCommand cmd = databaseConnection.CreateCommand();
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE ID={1}", SQLFactionsTable, i);
                    IDataReader reader = cmd.ExecuteReader();
                    bool updateQuery = reader.Read();

                    string assignSymbol = "@";
                    if (updateQuery)
                        assignSymbol = "$";

                    // Create query
                    cmd = databaseConnection.CreateCommand();

                    // Update if already exists or insert otherwise
                    if (!updateQuery)
                        cmd.CommandText = "INSERT INTO Factions (ID, Name, Ratings) VALUES (@ID, @Name,@Ratings)";
                    else
                        cmd.CommandText = "UPDATE Factions SET Name=$Name, Ratings=$Ratings WHERE ID=@ID";

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = i });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Name", Value = FactionNames[i] });                    

                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                    {
                        writer.Write(TotalFactions);
                        // Write factions as ints incase we want wider values in the future
                        for (int j = 0; j < TotalFactions; j++)
                            writer.Write((int)FactionDefaultRatings[i, j]);

                        cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Ratings", Value = (writer.BaseStream as MemoryStream).ToArray() });
                    }

                    if (updateQuery ? cmd.ExecuteReader().RecordsAffected > 0 : cmd.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine("Inserted/Updated item: " + FactionNames[i] );
                    }
                    else
                        throw new Exception("Query did not affect anything.");
                }


            databaseConnection.Close();

            return true;
            /*
            BinaryWriter F = Blitz.WriteFile(Filename);
            if (F == null)
            {
                return false;
            }

            for (int i = 0; i < TotalFactions; ++i)
            {
                Blitz.WriteString(FactionNames[i], F);
            }

            for (int i = 0; i < TotalFactions; ++i)
            {
                for (int j = 0; j < TotalFactions; ++j)
                {
                    F.Write(FactionDefaultRatings[i, j]);
                }
            }

            F.Close();
            return true;*/
        }

        // Actors index
        public static Dictionary<uint, Actor> Index = new Dictionary<uint, Actor>();

        // Members
        public uint ID;
        public string Race, Class, Description;
        public string StartArea, StartPortal;
        public Attributes Attributes;
        public ushort[] Resistances = new ushort[Item.TotalDamageTypes];
        public float Radius, Scale; // Radius is for server use since it doesn't have access to the actual mesh data
        //public ushort[] MeshIDs = new ushort[8]; // 2 base meshes (male/female) and six gubbins
        public ushort MaleMesh = 0, FemaleMesh = 0;
        public List<GubbinTemplate> DefaultGubbins = new List<GubbinTemplate>();

        public ushort[] TempGubbins = new ushort[0]; // Used to store TemplateIDs until they are loaded.
        public List<ushort[]> TempBeards = new List<ushort[]>();
        public List<ushort[]> TempMaleHairs = new List<ushort[]>();
        public List<ushort[]> TempFemaleHairs = new List<ushort[]>();

        // Each outer list is a hair ID (in ActorInstance)
        // Each inner list is a set of gubbins, both are restricted to 256 entries.
        public List<List<GubbinTemplate>> Beards = new List<List<GubbinTemplate>>();
        public List<List<GubbinTemplate>> MaleHairs = new List<List<GubbinTemplate>>();
        public List<List<GubbinTemplate>> FemaleHairs = new List<List<GubbinTemplate>>();

        public ActorTextureSet[] MaleFaceIDs = new ActorTextureSet[0]; // Male face textures
        public ActorTextureSet[] FemaleFaceIDs = new ActorTextureSet[0]; // Female face tetures
        public ActorTextureSet[] MaleBodyIDs = new ActorTextureSet[0]; // Male body textures
        public ActorTextureSet[] FemaleBodyIDs = new ActorTextureSet[0]; // Female body textures
        public ushort[] MaleSpeechIDs = new ushort[16]; // Male speech sounds
        public ushort[] FemaleSpeechIDs = new ushort[16]; // Female speech sounds
        public int[] HairColors = new int[6];
        public ushort BloodTexID; // Blood particles texture
        public byte Genders; // 0 for normal (male and female), 1 for male only, 2 for female only, 3 for no genders
        public ushort MaleAnimationSet, FemaleAnimationSet;
        public bool Playable, Rideable;
        public byte Aggressiveness;
        public int AggressiveRange;
        public byte TradeMode;
        public Environment EType;
        public ushort InventorySlots;
        public byte DefaultDamageType;
        public byte DefaultFaction;
        public int XPMultiplier;
        public bool PolyCollision;

        // Linked list
        public static Actor FirstActor;
        public Actor NextActor;

        // Constructor
        public Actor()
        {
            for (uint i = 0; i < uint.MaxValue; ++i)
            {
                if (!Index.ContainsKey(i))
                {
                    ID = i;
                    return;
                }
            }
            throw new ActorException("Maximum number of actors already created!");
        }

        private Actor(bool DoNotAssignID)
        {
            Attributes = new Attributes();
            Init();
            NextActor = FirstActor;
            FirstActor = this;
            return;
        }

        // Sets default values
        public void Init()
        {
            for (int j = 0; j < Attributes.TotalAttributes; ++j)
            {
                Attributes.Maximum[j] = 100;
            }

            for (int j = 0; j < 6; ++j)
            {
                HairColors[j] = System.Drawing.Color.White.ToArgb();
            }

            for (int j = 0; j < 12; ++j)
            {
                MaleSpeechIDs[j] = 65535;
                FemaleSpeechIDs[j] = 65535;
            }
            InventorySlots = 0xFFFF;
            MaleMesh = 0;
            FemaleMesh = 0;
            Scale = 1f;
            AggressiveRange = 50;
            Race = "";
            Class = "";
            Description = "";
            StartArea = "";
            StartPortal = "";
        }

        // Removes this actor from the index and linked list
        public void Delete()
        {
            Index[ID] = null;
            Actor I = FirstActor;
            if (I == this)
            {
                FirstActor = NextActor;
            }
            else
            {
                while (I != null)
                {
                    if (I.NextActor == this)
                    {
                        I.NextActor = NextActor;
                        NextActor = null;
                        break;
                    }
                    I = I.NextActor;
                }
            }
        }

        const string ActorSQLTable = "Actors";
        public const string ActorDatabase = @"Data\Server Data\Actors.s3db";

        // Loads all actors from file
        public static int Load(string filename)
        {

            if (!File.Exists(filename))
                return -1;

            int actorCount = 0;
            
            // Create database connection
            SqliteConnection databaseConnection = new SqliteConnection();
            databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", filename);
            databaseConnection.Open();

            // Query table to get all actors
            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + ActorSQLTable;

            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Actor actor = new Actor(true);

                actor.ID = (uint)reader.GetInt64(reader.GetOrdinal("ID"));
                actor.Race = reader.GetString(reader.GetOrdinal("Race"));
                actor.Class = reader.GetString(reader.GetOrdinal("Class"));
                actor.Description = reader.GetString(reader.GetOrdinal("Description"));
                actor.StartArea = reader.GetString(reader.GetOrdinal("StartArea"));
                actor.StartPortal = reader.GetString(reader.GetOrdinal("StartPortal"));
                actor.MaleAnimationSet = (ushort)reader.GetInt64(reader.GetOrdinal("MaleAnimationSet"));
                actor.FemaleAnimationSet = (ushort)reader.GetInt64(reader.GetOrdinal("FemaleAnimationSet"));
                actor.Scale = reader.GetFloat(reader.GetOrdinal("Scale"));
                actor.Radius = reader.GetFloat(reader.GetOrdinal("Radius"));
                actor.MaleMesh = (ushort)reader.GetInt64(reader.GetOrdinal("MaleMesh"));
                actor.FemaleMesh = (ushort)reader.GetInt64(reader.GetOrdinal("FemaleMesh"));
              
                // Gubbins
                byte[] data = (byte[])reader.GetValue(reader.GetOrdinal("GubbinData"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    byte gubbinCount = binaryReader.ReadByte();
                    actor.TempGubbins = new ushort[gubbinCount];
                    for (int i = 0; i < gubbinCount; i++)
                        actor.TempGubbins[i] = binaryReader.ReadUInt16();
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

                        actor.TempBeards.Add(temps);
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

                        actor.TempMaleHairs.Add(temps);
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

                        actor.TempFemaleHairs.Add(temps);
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
                        actor.MaleSpeechIDs[i] = (ushort)binaryReader.ReadUInt32();
                    }
                }

                // Female speech
                data = (byte[])reader.GetValue(reader.GetOrdinal("FemaleSpeech"));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    uint count = binaryReader.ReadUInt32();
                    for (int i = 0; i < count; i++)
                    {
                        actor.FemaleSpeechIDs[i] = (ushort)binaryReader.ReadUInt32();
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
                actor.EType = (Environment)reader.GetByte(reader.GetOrdinal("EnvironmentType"));
                actor.InventorySlots = (ushort)reader.GetInt32(reader.GetOrdinal("InventorySlots"));
                actor.DefaultDamageType = reader.GetByte(reader.GetOrdinal("DefaultDamageType"));
                actor.DefaultFaction = reader.GetByte(reader.GetOrdinal("DefaultFaction"));
                actor.XPMultiplier = reader.GetInt32(reader.GetOrdinal("XPMultiplier"));
                actor.PolyCollision = reader.GetBoolean(reader.GetOrdinal("PolygonCollision"));

                Index.Add(actor.ID, actor);

                actorCount++;
            }

            return actorCount;
            #region Old binary file
            /*
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return -1;
            }

            int Actors = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                Actor A = new Actor(true);
                A.ID = F.ReadUInt16();
                Index[A.ID] = A;

                A.Race = Blitz.ReadString(F);
                A.Class = Blitz.ReadString(F);
                A.Description = Blitz.ReadString(F);
                A.StartArea = Blitz.ReadString(F);
                A.StartPortal = Blitz.ReadString(F);
                A.MaleAnimationSet = F.ReadUInt16();
                A.FemaleAnimationSet = F.ReadUInt16();
                A.Scale = F.ReadSingle();
                A.Radius = F.ReadSingle();

                A.MaleMesh = F.ReadUInt16();
                A.FemaleMesh = F.ReadUInt16();

                byte GubbinCount = F.ReadByte();
                A.TempGubbins = new ushort[GubbinCount];

                for (int i = 0; i < GubbinCount; ++i)
                {
                    A.TempGubbins[i] = F.ReadUInt16();
                }


                byte Count = F.ReadByte();
                for (int bid = 0; bid < Count; ++bid)
                {
                    GubbinCount = F.ReadByte();

                    ushort[] Temps = new ushort[GubbinCount];

                    for (int i = 0; i < GubbinCount; ++i)
                        Temps[i] = F.ReadUInt16();

                    A.TempBeards.Add(Temps);
                }

                Count = F.ReadByte();
                for (int bid = 0; bid < Count; ++bid)
                {
                    GubbinCount = F.ReadByte();

                    ushort[] Temps = new ushort[GubbinCount];

                    for (int i = 0; i < GubbinCount; ++i)
                        Temps[i] = F.ReadUInt16();

                    A.TempMaleHairs.Add(Temps);
                }

                Count = F.ReadByte();
                for (int bid = 0; bid < Count; ++bid)
                {
                    GubbinCount = F.ReadByte();

                    ushort[] Temps = new ushort[GubbinCount];

                    for (int i = 0; i < GubbinCount; ++i)
                        Temps[i] = F.ReadUInt16();

                    A.TempFemaleHairs.Add(Temps);
                }


//                 for (int i = 0; i < 5; ++i)
//                 {
//                     F.ReadUInt16();
//                 }
//                 for (int i = 0; i < 5; ++i)
//                 {
//                     F.ReadUInt16();
//                 }
//                 for (int i = 0; i < 5; ++i)
//                 {
//                     F.ReadUInt16();
//                 }



//                 // Temporary, till next save!
//                 for (int i = 0; i < 6; ++i)
//                 {
//                     F.ReadUInt16();
//                 }


//                 for (int i = 0; i < 5; ++i)
//                 {
//                     A.BeardIDs[i] = F.ReadUInt16();
//                 }
//                 for (int i = 0; i < 5; ++i)
//                 {
//                     A.MaleHairIDs[i] = F.ReadUInt16();
//                 }
//                 for (int i = 0; i < 5; ++i)
//                 {
//                     A.FemaleHairIDs[i] = F.ReadUInt16();
//                 }


                
                A.MaleFaceIDs = new ActorTextureSet[F.ReadByte()];
                for (int i = 0; i < A.MaleFaceIDs.Length; ++i)
                {
                    A.MaleFaceIDs[i] = new ActorTextureSet(F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16());
                }

                A.FemaleFaceIDs = new ActorTextureSet[F.ReadByte()];
                for (int i = 0; i < A.FemaleFaceIDs.Length; ++i)
                {
                    A.FemaleFaceIDs[i] = new ActorTextureSet(F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16());
                }

                A.MaleBodyIDs = new ActorTextureSet[F.ReadByte()];
                for (int i = 0; i < A.MaleBodyIDs.Length; ++i)
                {
                    A.MaleBodyIDs[i] = new ActorTextureSet(F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16());
                }

                A.FemaleBodyIDs = new ActorTextureSet[F.ReadByte()];
                for (int i = 0; i < A.FemaleBodyIDs.Length; ++i)
                {
                    A.FemaleBodyIDs[i] = new ActorTextureSet(F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16(), F.ReadUInt16());
                }

                for (int i = 0; i < 16; ++i)
                {
                    A.MaleSpeechIDs[i] = F.ReadUInt16();
                }
                for (int i = 0; i < 16; ++i)
                {
                    A.FemaleSpeechIDs[i] = F.ReadUInt16();
                }

                // Server has this commented out, so I'm commenting it out here too.
                //for (int i = 0; i < 6; ++i)
                //    A.HairColors[i] = F.ReadInt32();

                A.BloodTexID = F.ReadUInt16();
                for (int i = 0; i < Attributes.TotalAttributes; ++i)
                {
                    A.Attributes.Value[i] = F.ReadUInt16();
                    A.Attributes.Maximum[i] = F.ReadUInt16();
                }
                for (int i = 0; i < Item.TotalDamageTypes; ++i)
                {
                    A.Resistances[i] = F.ReadUInt16();
                }
                A.Genders = F.ReadByte();
                A.Playable = F.ReadBoolean();
                A.Rideable = F.ReadBoolean();
                A.Aggressiveness = F.ReadByte();
                A.AggressiveRange = F.ReadInt32();
                A.TradeMode = F.ReadByte();
                A.EType = (Environment) F.ReadByte();
                A.InventorySlots = (ushort) F.ReadInt32();
                A.DefaultDamageType = F.ReadByte();
                A.DefaultFaction = F.ReadByte();
                A.XPMultiplier = F.ReadInt32();
                A.PolyCollision = F.ReadBoolean();

                Actors++;
            }

            F.Close();
            return Actors;
             */
            #endregion
        }

        //// Saves all actors to file
        public static bool Save(string filename)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                SqliteConnection databaseConnection = new SqliteConnection();
                string cs = string.Format("Version=3,uri=file:{0}", filename);
                databaseConnection.ConnectionString = cs;
                databaseConnection.Open();

                foreach (Actor actor in Index.Values)
                {

                    // Check if entry already exists
                    IDbCommand cmd = databaseConnection.CreateCommand();
                    cmd.CommandText = string.Format("DELETE FROM {0} WHERE ID={1}", ActorSQLTable, actor.ID);
                    if (cmd.ExecuteNonQuery() > 0) { }
                    
                    // Insert Actor row
                    cmd = databaseConnection.CreateCommand();
                    cmd.CommandText = "INSERT INTO Actors (ID, Race, Class, Description, StartArea, StartPortal, MaleAnimationSet, FemaleAnimationSet, Scale, Radius, MaleMesh, " +
                    "FemaleMesh, GubbinData, Beards, MaleHair, FemaleHair, MaleFaceTextures, FemaleFaceTextures, MaleBodyTextures, FemaleBodyTextures, MaleSpeech, FemaleSpeech, BloodTexture, " +
                    "Attributes, Resistances, Genders, Playable, Ridable, Aggression, AggressionRange, TradeMode, EnvironmentType, InventorySlots, DefaultDamageType, DefaultFaction, " +
                    "XPMultiplier, PolygonCollision) VALUES (@ID, @Race, @Class, @Description, @StartArea, @StartPortal, @MaleAnimationSet, @FemaleAnimationSet, @Scale, @Radius, @MaleMesh, " +
                    "@FemaleMesh, @GubbinData, @Beards, @MaleHair, @FemaleHair, @MaleFaceTextures, @FemaleFaceTextures, @MaleBodyTextures, @FemaleBodyTextures, @MaleSpeech, @FemaleSpeech, @BloodTexture, " +
                    "@Attributes, @Resistances, @Genders, @Playable, @Ridable, @Aggression, @AggressionRange, @TradeMode, @EnvironmentType, @InventorySlots, @DefaultDamageType, @DefaultFaction, " +
                    "@XPMultiplier, @PolygonCollision)";

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = (long)actor.ID });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Race", Value = actor.Race });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Class", Value = actor.Class });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Description", Value = actor.Description });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@StartArea", Value = actor.StartArea });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@StartPortal", Value = actor.StartPortal });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleAnimationSet", Value = (long)actor.MaleAnimationSet });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleAnimationSet", Value = (long)actor.FemaleAnimationSet });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Scale", Value = actor.Scale });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Radius", Value = actor.Radius });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@MaleMesh", Value = (long)actor.MaleMesh });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@FemaleMesh", Value = (long)actor.FemaleMesh });

                    // Write gubbins                  
                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                    {
                        if (actor.TempGubbins == null)
                        {
                            writer.Write((byte)0);
                        }
                        else
                        {
                            writer.Write((byte)actor.TempGubbins.Length);
                            for (int i = 0; i < actor.TempGubbins.Length; i++)
                                writer.Write(actor.TempGubbins[i]);
                        }
                        var memoryStream = writer.BaseStream as MemoryStream;
                        if (memoryStream != null)
                        {
                            cmd.Parameters.Add(new SqliteParameter { ParameterName = "@GubbinData", Value = memoryStream.ToArray() });
                        }
                    }

                    // Write beards
                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                    {
                        if (actor.TempBeards == null)
                            writer.Write((uint)0);
                        else
                        {

                            writer.Write((uint)actor.TempBeards.Count);
                            for (int bid = 0; bid < actor.TempBeards.Count; ++bid)
                            {
                                writer.Write((uint)actor.TempBeards[bid].Length);

                                for (int i = 0; i < actor.TempBeards[bid].Length; ++i)
                                    writer.Write((uint)actor.TempBeards[bid][i]);
                            }
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
                        if (actor.TempMaleHairs == null)
                            writer.Write((uint)0);
                        else
                        {
                            writer.Write((uint)actor.TempMaleHairs.Count);

                            for (int bid = 0; bid < actor.TempMaleHairs.Count; ++bid)
                            {
                                writer.Write((uint)actor.TempMaleHairs[bid].Length);

                                for (int i = 0; i < actor.TempMaleHairs[bid].Length; ++i)
                                    writer.Write((uint)actor.TempMaleHairs[bid][i]);

                            }
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
                        if (actor.TempFemaleHairs == null)
                            writer.Write((uint)0);
                        else
                        {
                            writer.Write((uint)actor.TempFemaleHairs.Count);

                            for (int bid = 0; bid < actor.TempFemaleHairs.Count; ++bid)
                            {
                                writer.Write((uint)actor.TempFemaleHairs[bid].Length);

                                for (int i = 0; i < actor.TempFemaleHairs[bid].Length; ++i)
                                    writer.Write((uint)actor.TempFemaleHairs[bid][i]);

                            }
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
                        if (actor.MaleFaceIDs == null)
                            writer.Write((uint)0);
                        else
                        {
                            uint count = (uint)actor.MaleFaceIDs.Length;
                            writer.Write(count);
                            for (int i = 0; i < count; ++i)
                            {
                                writer.Write((uint)actor.MaleFaceIDs[i].Tex0);
                                writer.Write((uint)actor.MaleFaceIDs[i].Tex1);
                                writer.Write((uint)actor.MaleFaceIDs[i].Tex2);
                                writer.Write((uint)actor.MaleFaceIDs[i].Tex3);
                            }
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
                        if (actor.FemaleFaceIDs == null)
                            writer.Write((uint)0);
                        else
                        {
                            uint count = (uint)actor.FemaleFaceIDs.Length;
                            writer.Write(count);
                            for (int i = 0; i < count; ++i)
                            {
                                writer.Write((uint)actor.FemaleFaceIDs[i].Tex0);
                                writer.Write((uint)actor.FemaleFaceIDs[i].Tex1);
                                writer.Write((uint)actor.FemaleFaceIDs[i].Tex2);
                                writer.Write((uint)actor.FemaleFaceIDs[i].Tex3);
                            }
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
                        if (actor.MaleBodyIDs == null)
                            writer.Write((uint)0);
                        else
                        {
                            uint count = (uint)actor.MaleBodyIDs.Length;
                            writer.Write(count);
                            for (int i = 0; i < count; ++i)
                            {
                                writer.Write((uint)actor.MaleBodyIDs[i].Tex0);
                                writer.Write((uint)actor.MaleBodyIDs[i].Tex1);
                                writer.Write((uint)actor.MaleBodyIDs[i].Tex2);
                                writer.Write((uint)actor.MaleBodyIDs[i].Tex3);
                            }
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
                        if (actor.FemaleBodyIDs == null)
                            writer.Write((uint)0);
                        else
                        {
                            uint count = (uint)actor.FemaleBodyIDs.Length;
                            writer.Write(count);
                            for (int i = 0; i < count; ++i)
                            {
                                writer.Write((uint)actor.FemaleBodyIDs[i].Tex0);
                                writer.Write((uint)actor.FemaleBodyIDs[i].Tex1);
                                writer.Write((uint)actor.FemaleBodyIDs[i].Tex2);
                                writer.Write((uint)actor.FemaleBodyIDs[i].Tex3);
                            }
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
                        if (actor.MaleSpeechIDs == null)
                            writer.Write((uint)0);
                        else
                        {
                            uint count = (uint)actor.MaleSpeechIDs.Length;
                            writer.Write(count);
                            for (int i = 0; i < count; ++i)
                            {
                                writer.Write((uint)actor.MaleSpeechIDs[i]);
                            }
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
                        if (actor.FemaleSpeechIDs == null)
                            writer.Write((uint)0);
                        else
                        {
                            uint count = (uint)actor.FemaleSpeechIDs.Length;
                            writer.Write(count);
                            for (int i = 0; i < count; ++i)
                            {
                                writer.Write((uint)actor.FemaleSpeechIDs[i]);
                            }
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

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@BloodTexture", Value = (long)actor.BloodTexID });


                    // Write attributes
                    using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
                    {
                        writer.Write((uint)Attributes.TotalAttributes);
                        for (int i = 0; i < Attributes.TotalAttributes; ++i)
                        {
                            writer.Write((uint)actor.Attributes.Value[i]);
                            writer.Write((uint)actor.Attributes.Maximum[i]);
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
                        writer.Write((uint)actor.Resistances.Length);
                        for (int i = 0; i < actor.Resistances.Length; ++i)
                        {
                            writer.Write((uint)actor.Resistances[i]);
                        }

                        var memoryStream = writer.BaseStream as MemoryStream;
                        if (memoryStream != null)
                        {
                            cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Resistances", Value = memoryStream.ToArray() });
                        }
                    }

                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Genders", Value = actor.Genders });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Playable", Value = actor.Playable });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Ridable", Value = actor.Rideable });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@Aggression", Value = actor.Aggressiveness});
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@AggressionRange", Value = actor.AggressiveRange });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@TradeMode", Value = actor.TradeMode});
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@EnvironmentType", Value = (byte)actor.EType });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@InventorySlots", Value = (int)actor.InventorySlots });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@DefaultDamageType", Value = actor.DefaultDamageType });
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@DefaultFaction", Value = actor.DefaultFaction});
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@XPMultiplier", Value = actor.XPMultiplier});
                    cmd.Parameters.Add(new SqliteParameter { ParameterName = "@PolygonCollision", Value = actor.PolyCollision });

                    if (cmd.ExecuteNonQuery() > 0)
                        Console.WriteLine("Inserted Actor: " + actor.Race + " " + actor.Class);
                    else
                    {
                        MessageBox.Show("Actor query failed.");
                    }
                    



                }
                /*
                BinaryWriter F = Blitz.WriteFile(Filename);
                if (F == null)
                {
                    return false;
                }

                Actor A = FirstActor;
                while (A != null)
                {
                    F.Write((ushort) A.ID);
                    Blitz.WriteString(A.Race, F);
                    Blitz.WriteString(A.Class, F);
                    Blitz.WriteString(A.Description, F);
                    Blitz.WriteString(A.StartArea, F);
                    Blitz.WriteString(A.StartPortal, F);
                    F.Write(A.MaleAnimationSet);
                    F.Write(A.FemaleAnimationSet);
                    F.Write(A.Scale);
                    F.Write(A.Radius);

                    F.Write(A.MaleMesh);
                    F.Write(A.FemaleMesh);

                    F.Write((byte)A.DefaultGubbins.Count);
                    foreach (GubbinTemplate T in A.DefaultGubbins)
                    {
                        F.Write((ushort)T.ID);
                    }

                    // Write beards
                    F.Write((byte)A.Beards.Count);
                    for (int bid = 0; bid < A.Beards.Count; ++bid)
                    {
                        F.Write((byte)A.Beards[bid].Count);

                        for (int i = 0; i < A.Beards[bid].Count; ++i)
                            F.Write((ushort)A.Beards[bid][i].ID);
                    }

                    // Write Hairs
                    F.Write((byte)A.MaleHairs.Count);
                    for (int bid = 0; bid < A.MaleHairs.Count; ++bid)
                    {
                        F.Write((byte)A.MaleHairs[bid].Count);

                        for (int i = 0; i < A.MaleHairs[bid].Count; ++i)
                            F.Write((ushort)A.MaleHairs[bid][i].ID);
                    }

                    F.Write((byte)A.FemaleHairs.Count);
                    for (int bid = 0; bid < A.FemaleHairs.Count; ++bid)
                    {
                        F.Write((byte)A.FemaleHairs[bid].Count);

                        for (int i = 0; i < A.FemaleHairs[bid].Count; ++i)
                            F.Write((ushort)A.FemaleHairs[bid][i].ID);
                    }



                    F.Write((byte)A.MaleFaceIDs.Length);
                    for (int i = 0; i < A.MaleFaceIDs.Length; ++i)
                    {
                        F.Write(A.MaleFaceIDs[i].Tex0);
                        F.Write(A.MaleFaceIDs[i].Tex1);
                        F.Write(A.MaleFaceIDs[i].Tex2);
                        F.Write(A.MaleFaceIDs[i].Tex3);
                    }
                    F.Write((byte)A.FemaleFaceIDs.Length);
                    for (int i = 0; i < A.FemaleFaceIDs.Length; ++i)
                    {
                        F.Write(A.FemaleFaceIDs[i].Tex0);
                        F.Write(A.FemaleFaceIDs[i].Tex1);
                        F.Write(A.FemaleFaceIDs[i].Tex2);
                        F.Write(A.FemaleFaceIDs[i].Tex3);
                    }
                    F.Write((byte)A.MaleBodyIDs.Length);
                    for (int i = 0; i < A.MaleBodyIDs.Length; ++i)
                    {
                        F.Write(A.MaleBodyIDs[i].Tex0);
                        F.Write(A.MaleBodyIDs[i].Tex1);
                        F.Write(A.MaleBodyIDs[i].Tex2);
                        F.Write(A.MaleBodyIDs[i].Tex3);
                    }
                    F.Write((byte)A.FemaleBodyIDs.Length);
                    for (int i = 0; i < A.FemaleBodyIDs.Length; ++i)
                    {
                        F.Write(A.FemaleBodyIDs[i].Tex0);
                        F.Write(A.FemaleBodyIDs[i].Tex1);
                        F.Write(A.FemaleBodyIDs[i].Tex2);
                        F.Write(A.FemaleBodyIDs[i].Tex3);
                    }

                    for (int i = 0; i < 16; ++i)
                    {
                        F.Write(A.MaleSpeechIDs[i]);
                    }
                    for (int i = 0; i < 16; ++i)
                    {
                        F.Write(A.FemaleSpeechIDs[i]);
                    }

                    //// Server has this commented out, so I'm commenting it out here too.
                    ////for (int i = 0; i < 6; ++i)
                    ////    F.Write((int)A.HairColors[i]);

                    F.Write(A.BloodTexID);
                    for (int i = 0; i < Attributes.TotalAttributes; ++i)
                    {
                        F.Write((ushort) A.Attributes.Value[i]);
                        F.Write((ushort) A.Attributes.Maximum[i]);
                    }

                    for (int i = 0; i < Item.TotalDamageTypes; ++i)
                    {
                        F.Write(A.Resistances[i]);
                    }
                    F.Write(A.Genders);
                    F.Write(A.Playable);
                    F.Write(A.Rideable);
                    F.Write(A.Aggressiveness);
                    F.Write(A.AggressiveRange);
                    F.Write(A.TradeMode);
                    F.Write((byte) A.EType);
                    F.Write((int) A.InventorySlots);
                    F.Write(A.DefaultDamageType);
                    F.Write(A.DefaultFaction);
                    F.Write(A.XPMultiplier);
                    F.Write(A.PolyCollision);

                    A = A.NextActor;
                }

                F.Close();
                */
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class GubbinPreviewInstance
    {
        public GubbinActorTemplate ActorTemplate;
        public Entity Mesh;
        public Entity EmitterEN;
        public RenderingServices.Light Light;

        public GubbinPreviewInstance(GubbinActorTemplate actorTemplate)
        {
            ActorTemplate = actorTemplate;
        }

        public void Update(Entity EN)
        {
            if (Light == null || EN == null || ActorTemplate == null || string.IsNullOrEmpty(ActorTemplate.AssignedBoneName))
                return;

            Entity Bone = EN.FindChild(ActorTemplate.AssignedBoneName);
            if (Bone == null)
                return;

            float Bx = Bone.X(true);
            float By = Bone.Y(true);
            float Bz = Bone.Z(true);

            Vector3 Position = ActorTemplate.Position;
            Vector3 Rotation = ActorTemplate.Rotation;

            Entity Piv = Entity.CreatePivot();
            Piv.Parent(Bone, false);
            Piv.Position(Position.X, Position.Y, Position.Z);
            Piv.Rotate(Rotation.X, Rotation.Y, Rotation.Z);

            Bx = Piv.X(true);
            By = Piv.Y(true);
            Bz = Piv.Z(true);

            //PreviewLight.EN.Position(Bx, By, Bz);
            //PreviewLight.MoverPivot.Position(Bx, By, Bz);
            Light.Position(Bx, By, Bz);

            Piv.Free();
        }
    }

    // Actor instance class
    public class ActorInstance : IDisposable
    {
        // Actor instances index
        public static ActorInstance[] Index = new ActorInstance[65536];
        public static int LastRuntimeID = 0;
        public static LinkedList<ActorInstance> List = new LinkedList<ActorInstance>();

        // Player (client only)
        public static ActorInstance Me;

        // Actor this is an instance of
        public Actor Actor;

        

        // Actor instance settings
        public float X, Y, Z, OldX, OldZ, DestX, DestZ, Yaw;
        public bool WalkingBackward;
        public string Zone;
        public ServerZone.ZoneInstance ServerZone;
        //// public Account Account;
        public string Name, Tag;
        public int LastPortal, LastTrigger, LastPortalTime;
        public int TeamID; // Used by the scripting language
        public int PartyID, AcceptPending; // Used by the party system (PartyID is 0 when not in any party)
        public byte Gender; // 0 for male, 1 for female

        public object EN, CollisionEN, ShadowEN, NametagEN;
        // HatEN stores the hair entity if no hat is worn

        public List<GubbinPreviewInstance> HatENs = new List<GubbinPreviewInstance>();
        public List<GubbinPreviewInstance> BeardENs = new List<GubbinPreviewInstance>();
        public List<GubbinPreviewInstance> ChestENs = new List<GubbinPreviewInstance>();
        public List<GubbinPreviewInstance> WeaponENs = new List<GubbinPreviewInstance>();
        public List<GubbinPreviewInstance> ShieldENs = new List<GubbinPreviewInstance>();


        public List<GubbinPreviewInstance> GubbinEN = new List<GubbinPreviewInstance>();

        public ushort FaceTex, Hair, Beard, BodyTex; // Fixed throughout a character's life unless altered by scripting
        public ushort Level;
        public int XP;
        public byte XPBarLevel;
        public byte HomeFaction; // Faction this actor belongs to (0-100 with 0 meaning no faction)

        public byte[] FactionRatings = new byte[Actor.TotalFactions];
        // Individual ratings with each faction for this character

        public Attributes Attributes; // Overrides the Actor.Attributes which are merely the default initial values
        public ushort[] Resistances = new ushort[Item.TotalDamageTypes]; // Resistances against damage types
        public string Script, DeathScript; // Scripts for when the character is interacted with and for when it dies
        public Inventory Inventory; // This character's inventory slots
        public ActorInstance Leader; // For slaves (pets etc.)
        public byte NumberOfSlaves; // Whether this character owns any slaves, and if so how many
        public ushort Reputation;
        public int Money;
        public int RNID; // RottNet ID (-1 for AI actors, 0 for a player character not logged in)
        public int RuntimeID; // Assigned by the server to identify this character
        public int[] AnimSeqs = new int[150];
        public int SourceSP, CurrentWaypoint; // AI stuff
        public Actor.AIMode AIMode;
        public ActorInstance AITarget;
        public ActorInstance Rider, Mount; // If this character is riding another or being ridden
        public bool IsRunning;
        public long LastAttack;
        public int FootstepPlayedThisCycle; // Used by the client to play one footstep noise per animation cycle
        public string[] ScriptGlobals = new string[10];
        public ushort[] KnownSpells = new ushort[1000], SpellLevels = new ushort[1000], MemorisedSpells = new ushort[10];
        public int[] SpellCharge = new int[1000]; // How long until a spell is usable again

        public byte IsTrading;
        // 0 for not trading, 1 for trading with NPC, 2 for trading with pet, 3 for trading with player, 4/5 for accepted trading with player

        public ActorInstance TradingActor;
        public string TradeResult;
        public bool Underwater;

        // Constructor
        public ActorInstance(Actor Actor)
        {
            if (Actor == null)
            {
                throw new ActorException("Could not create actor instance - actor does not exist.");
            }

            List.AddLast(this);
            Attributes = new Attributes();
            Inventory = new Inventory();
            this.Actor = Actor;
            Name = Actor.Race;
            HomeFaction = Actor.DefaultFaction;
            Level = 1;
            RuntimeID = -1;
            LastAttack = System.Diagnostics.Stopwatch.GetTimestamp();
            SourceSP = -1;
            LastTrigger = -1;
            LastPortal = -1;
            if (Actor.Genders == 2)
            {
                Gender = 1;
            }
            for (int i = 0; i < Actor.TotalFactions; ++i)
            {
                FactionRatings[i] = Actor.FactionDefaultRatings[HomeFaction, i];
            }
            for (int i = 0; i < Attributes.TotalAttributes; ++i)
            {
                Attributes.Value[i] = Actor.Attributes.Value[i];
                Attributes.Maximum[i] = Actor.Attributes.Maximum[i];
            }

            for (int i = 0; i < Item.TotalDamageTypes; ++i)
            {
                Resistances[i] = Actor.Resistances[i];
            }
            for (int i = 0; i < 10; ++i)
            {
                MemorisedSpells[i] = 5000; // No spell memorised
            }
        }

        // Frees this actor instance (but not any slaves of it)
        public void Dispose()
        {
            if (RuntimeID >= 0 && Index[RuntimeID] == this)
            {
                Index[RuntimeID] = null;
            }

            List.Remove(this);

            if (Leader != null)
            {
                Leader.NumberOfSlaves--;
            }
        }

        //// Frees any and all slaves of an actor instance (RECURSIVE)
        public void FreeSlaves()
        {
            ActorInstance ToDispose = null;
            LinkedListNode<ActorInstance> Node = List.First;
            while (Node != null)
            {
                if (NumberOfSlaves == 0)
                {
                    break;
                }
                if (Node.Value.Leader == this)
                {
                    Node.Value.FreeSlaves();
                    ToDispose = Node.Value;
                }

                Node = Node.Next;
                if (ToDispose != null)
                {
                    ToDispose.Dispose();
                    ToDispose = null;
                }
            }
        }

        // Finds an actor instance by RottNet ID
        public static ActorInstance FindFromRNID(int RNID)
        {
            LinkedListNode<ActorInstance> Node = List.First;
            while (Node != null)
            {
                if (Node.Value.RNID == RNID)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            return null;
        }

        //// Finds an actor instance by name
        public static ActorInstance FindFromName(string Name)
        {
            Name = Name.ToUpper();
            LinkedListNode<ActorInstance> Node = List.First;

            while (Node != null)
            {
                if (Node.Value.Name.ToUpper() == Name)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }

            return null;
        }

        //// Finds a player character only actor instance by name
        public static ActorInstance FindPlayerFromName(string Name)
        {
            Name = Name.ToUpper();
            LinkedListNode<ActorInstance> Node = List.First;
            while (Node != null)
            {
                if (Node.Value.RNID >= 0 && Node.Value.Name.ToUpper() == Name)
                {
                    return Node.Value;
                }
                Node = Node.Next;
            }
            return null;
        }

        // Writes this actor instance to a stream
        public void Write(BinaryWriter Stream)
        {
        }

        // Reads this actor instance from a stream
        public static ActorInstance Read(BinaryReader Stream)
        {
            return null;
        }
    }

    // Actor/object attributes class
    public class Attributes
    {
        // Total number of attributes available. This must match the hard-coded value in RCClient.
        public const int TotalAttributes = 40;

        // Static stuff
        public static int AssignmentPoints;
        public static string[] Names = new string[TotalAttributes];
        public static bool[] IsSkill = new bool[TotalAttributes];
        public static bool[] Hidden = new bool[TotalAttributes];

        // Fixed attributes
        public static ushort HealthStat, EnergyStat, BreathStat, ToughnessStat, StrengthStat, SpeedStat;

        // Per-object stuff
        public int[] Value = new int[TotalAttributes];
        public int[] Maximum = new int[TotalAttributes];

        // Find an attribute from its name
        public int Find(string Name)
        {
            Name = Name.ToUpper();
            for (int i = 0; i < TotalAttributes; ++i)
            {
                if (Names[i].ToUpper() == Name)
                {
                    return i;
                }
            }
            return -1;
        }

        // Load/save attributes to or from a file
        public static bool Load(string Filename)
        {
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return false;
            }

            AssignmentPoints = F.ReadByte();
            for (int i = 0; i < TotalAttributes; ++i)
            {
                Names[i] = Blitz.ReadString(F);
                IsSkill[i] = F.ReadBoolean();
                Hidden[i] = F.ReadBoolean();
            }

            F.Close();

            return true;
        }

        public static bool Save(string Filename)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                BinaryWriter F = Blitz.WriteFile(Filename);
                if (F == null)
                {
                    return false;
                }

                F.Write((byte) AssignmentPoints);
                for (int i = 0; i < TotalAttributes; ++i)
                {
                    Blitz.WriteString(Names[i], F);
                    F.Write(IsSkill[i]);
                    F.Write(Hidden[i]);
                }

                F.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LoadFixed() // load from Fixed Attributes.DAT
        {
            FileStream FS = new FileStream(@"Data\Server Data\Fixed Attributes.dat", FileMode.Open, FileAccess.Read);
            BinaryReader F = new BinaryReader(FS);

            HealthStat = F.ReadUInt16();
            EnergyStat = F.ReadUInt16();
            BreathStat = F.ReadUInt16();
            ToughnessStat = F.ReadUInt16();
            StrengthStat = F.ReadUInt16();
            SpeedStat = F.ReadUInt16();

            F.Close();
        }

        public static void SaveFixed()
        {
            FileStream FS = new FileStream(@"Data\Server Data\Fixed Attributes.dat", FileMode.Truncate, FileAccess.Write);
            BinaryWriter F = new BinaryWriter(FS);

            F.Write(HealthStat);
            F.Write(EnergyStat);
            F.Write(BreathStat);
            F.Write(ToughnessStat);
            F.Write(StrengthStat);
            F.Write(SpeedStat);

            F.Close();

            FS = new FileStream(@"Data\Game Data\Fixed Attributes.dat", FileMode.Truncate, FileAccess.Write);
            F = new BinaryWriter(FS);

            F.Write(HealthStat);
            F.Write(EnergyStat);
            F.Write(BreathStat);
            F.Write(StrengthStat);
            F.Write(SpeedStat);

            F.Close();
        }
    }

    // Actor creation exception
    internal class ActorException : Exception
    {
        public ActorException(string Message) : base(Message)
        {
        }
    }
}