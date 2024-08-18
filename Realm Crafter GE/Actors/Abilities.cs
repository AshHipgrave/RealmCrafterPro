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
// Realm Crafter Abilities (previously Spells) module by Rob W (rottbott@hotmail.com)
// Original version March 2005, C# port October 2006

using System;
using System.IO;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;
using System.Collections.Generic;

namespace RealmCrafter
{
    // Ability class
    public class Ability
    {
        // Index
        public static Ability FirstAbility;
        public static Dictionary<uint, Ability> Index = new Dictionary<uint, Ability>();

        // Members
        public string Description;
        public string ExclusiveClass;
        public string ExclusiveRace;
        public uint ID;
        public string Method;
        public string Name;

        // Linked list

        public Ability NextAbility;
        public int RechargeTime;
        public string Script;
        public ushort ThumbnailTexID;
        // Load/save abilities from/to file

        // Constructor
        public Ability()
        {
            // Get Free ID
            for(uint i = 0; i < uint.MaxValue; i++)
                if (!Index.ContainsKey(i))
                {
                    ID = i;
                    break;
                }

            this.Name = "New ability";
            this.Description = string.Empty;
            this.RechargeTime = 2000;
            this.Script = string.Empty;
            this.Method = string.Empty;
            this.ExclusiveRace = string.Empty;
            this.ExclusiveClass = string.Empty;
            this.NextAbility = FirstAbility;
            FirstAbility = this;
            /*
            for (int i = 0; i < 65535; ++i)
            {
                if (Index[i] == null)
                {
                    Index[i] = this;
                    this.ID = i;
                    this.Name = "New ability";
                    this.Description = string.Empty;
                    this.RechargeTime = 2000;
                    this.Script = string.Empty;
                    this.Method = string.Empty;
                    this.ExclusiveRace = string.Empty;
                    this.ExclusiveClass = string.Empty;
                    this.NextAbility = FirstAbility;
                    FirstAbility = this;
                    return;
                }
            }*/

            //throw new AbilityException("Maximum number of abilities already created!");
        }

        private Ability(bool DoNotAssignID)
        {
            this.Name = "New ability";
            this.Description = string.Empty;
            this.RechargeTime = 2000;
            this.Script = string.Empty;
            this.Method = string.Empty;
            this.ExclusiveRace = string.Empty;
            this.ExclusiveClass = string.Empty;
            this.NextAbility = FirstAbility;
            FirstAbility = this;
            return;
        }

        const string SpellsSQLTable = "Spells";
        public const string SpellDatabase = @"Data\Server Data\Spells.s3db";

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
            cmd.CommandText = "SELECT * FROM " + SpellsSQLTable;

            int spellCount = 0;
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Ability ability = new Ability(true);

                ability.ID = (uint)reader.GetInt64(reader.GetOrdinal("ID"));
                ability.Name = reader.GetString(reader.GetOrdinal("Name"));
                ability.Description = reader.GetString(reader.GetOrdinal("Description"));
                ability.ThumbnailTexID = (ushort)reader.GetInt32(reader.GetOrdinal("ThumbnailID"));
                ability.ExclusiveRace = reader.GetString(reader.GetOrdinal("Race"));
                ability.ExclusiveClass = reader.GetString(reader.GetOrdinal("Class"));
                ability.RechargeTime = reader.GetInt32(reader.GetOrdinal("RechargeTime"));
                ability.Script = reader.GetString(reader.GetOrdinal("Script"));

                if (!ability.Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                    ability.Script += ".cs";

                ability.Method = reader.GetString(reader.GetOrdinal("Method"));
                Index.Add(ability.ID, ability);

                spellCount++;
            }


            databaseConnection.Close();
            return spellCount;

            #region Old Binary file
            /*
            BinaryReader F = Blitz.ReadFile(Filename);
            if (F == null)
            {
                return -1;
            }

            int Abilities = 0;
            long FileLength = F.BaseStream.Length;

            while (F.BaseStream.Position < FileLength)
            {
                Ability A = new Ability(true);
                A.ID = F.ReadUInt16();
                Index[A.ID] = A;
                A.Name = Blitz.ReadString(F);
                A.Description = Blitz.ReadString(F);
                A.ThumbnailTexID = F.ReadUInt16();
                A.ExclusiveRace = Blitz.ReadString(F);
                A.ExclusiveClass = Blitz.ReadString(F);
                A.RechargeTime = F.ReadInt32();
                A.Script = Blitz.ReadString(F);

                if (!A.Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                    A.Script += ".cs";

                A.Method = Blitz.ReadString(F);

                Abilities++;
            }

            F.Close();
            
            return Abilities;*/
            #endregion
        }

        public static bool Save(string filename)
        {
            if (!File.Exists(filename))
                return false;

            SqliteConnection databaseConnection = new SqliteConnection();
            databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", filename);
            databaseConnection.Open();

            foreach (Ability ability in Ability.Index.Values)
            {
                // Check if entry already exists
                IDbCommand cmd = databaseConnection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM {0} WHERE ID={1}", SpellsSQLTable, ability.ID);
                IDataReader reader = cmd.ExecuteReader();
                bool updateQuery = reader.Read();

                // Create query to update or insert
                cmd = databaseConnection.CreateCommand();

                // Changable symbol because SET uses $ and INSERT uses @
                string assignSymbol = "@";
                if (updateQuery)
                {
                    // Update entry
                    cmd.CommandText = "UPDATE " + SpellsSQLTable + " SET Name=$Name, Description=$Description, ThumbnailID=$ThumbnailID, Race=$Race, Class=$Class, RechargeTime=$RechargeTime, Script=$Script, Method=$Method " +
                        "WHERE ID=@ID";
                    assignSymbol = "$";
                }
                else
                {
                    // Insert entry
                    cmd.CommandText = "INSERT INTO " + SpellsSQLTable + " (ID, Name, Description, ThumbnailID, Race, Class, RechargeTime, Script, Method)" +
                        " VALUES (@ID,@Name,@Description, @ThumbnailID, @Race, @Class, @RechargeTime, @Script, @Method)";
                }

                // Add parameters for query that work for either UPDATE OR SET so only one change needs to be made
                cmd.Parameters.Add(new SqliteParameter { ParameterName = "@ID", Value = (long)ability.ID });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Name", Value = ability.Name });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Description", Value = ability.Description });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "ThumbnailID", Value = (int)ability.ThumbnailTexID });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Race", Value = ability.ExclusiveRace });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Class", Value = ability.ExclusiveClass });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "RechargeTime", Value = ability.RechargeTime });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Script", Value = ability.Script });
                cmd.Parameters.Add(new SqliteParameter { ParameterName = assignSymbol + "Method", Value = ability.Method });

                if (updateQuery ? cmd.ExecuteReader().RecordsAffected > 0 : cmd.ExecuteNonQuery() > 0)
                {
                    Console.WriteLine("Inserted/Updated item: " + ability.Name);
                }
                else
                    throw new Exception("Query did not affect anything.");
            }

            databaseConnection.Close();

            return true;

            #region Old binary file
            /*
            BinaryWriter F = Blitz.WriteFile(Filename);
            if (F == null)
            {
                return false;
            }

            Ability A = FirstAbility;
            while (A != null)
            {
                F.Write((ushort) A.ID);
                Blitz.WriteString(A.Name, F);
                Blitz.WriteString(A.Description, F);
                F.Write(A.ThumbnailTexID);
                Blitz.WriteString(A.ExclusiveRace, F);
                Blitz.WriteString(A.ExclusiveClass, F);
                F.Write(A.RechargeTime);

                string script = A.Script;
                if (script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                    script = script.Substring(0, script.Length - 3);

                Blitz.WriteString(script, F);
                Blitz.WriteString(A.Method, F);

                A = A.NextAbility;
            }

            F.Close();

            return true;
             */
            #endregion
        }

        // Removes this ability from the index and linked list
        public void Delete()
        {
            Index[ID] = null;
            Ability A = FirstAbility;
            if (A == this)
            {
                FirstAbility = NextAbility;
            }
            else
            {
                while (A != null)
                {
                    if (A.NextAbility == this)
                    {
                        A.NextAbility = NextAbility;
                        NextAbility = null;
                        break;
                    }

                    A = A.NextAbility;
                }
            }
        }
    }

    // Ability creation exception
    public class AbilityException : Exception
    {
        public AbilityException(string Message) : base(Message)
        {
        }
    }
}