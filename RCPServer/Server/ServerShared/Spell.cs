using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;
using Community.CsharpSqlite.SQLiteClient;
using System.Data;

namespace RCPServer
{
    public class Spell
    {
        public uint ID = 0;
        public string Name = "";
        public string Description = "";				// Name and description displayed in the spellbook
        public ushort ThumbnailTexID = 65535;					// Icon displayed in the spellbook
        public string ExclusiveRace = "";
        public string ExclusiveClass = "";	// If this spell can only be used by a certain race and/or class
        public int RechargeTime = 0;						// Time taken to recharge after casting in milliseconds
        public string Script = "";
        public string Method = "";					// Script to run when cast

        public static Dictionary<uint, Spell> Spells = new Dictionary<uint, Spell>();

        const string SpellsSQLTable = "Spells";
        public const string SpellsDatabase = "Data/Server Data/Spells.s3db";

        public static int Load(string path)
        {
            // Create database connection
            SqliteConnection databaseConnection = new SqliteConnection();
            databaseConnection.ConnectionString = string.Format("Version=3,uri=file:{0}", path);
            databaseConnection.Open();

            // Query table to get all items
            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + SpellsSQLTable;

            int spellCount = 0;
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Spell ability = new Spell();

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
                Spells.Add(ability.ID, ability);

                spellCount++;
            }


            databaseConnection.Close();

            return spellCount;

            #region Old Binary file
            /*
            for (int i = 0; i < Spells.Length; ++i)
                Spells[i] = null;

            BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

            int Count = 0;

            while (!F.Eof)
            {
                Spell S = new Spell();
			    S.ID = F.ReadUInt16();
			    S.Name = F.ReadBBString();
			    S.Description = F.ReadBBString();
			    S.ThumbnailTexID = F.ReadUInt16();
			    S.ExclusiveRace = F.ReadBBString();
			    S.ExclusiveClass = F.ReadBBString();
			    S.RechargeTime = F.ReadInt32();
			    S.Script = F.ReadBBString();
			    S.Method = F.ReadBBString();

                Spells[S.ID] = S;
			    ++Count;
            }

            F.Close();
            return Count;
             */
            #endregion
        }
    }
}
