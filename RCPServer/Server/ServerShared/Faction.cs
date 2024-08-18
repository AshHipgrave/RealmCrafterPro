using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;
using Community.CsharpSqlite.SQLiteClient;
using System.Data;

namespace RCPServer
{
    public class Faction
    {
        string name;
        int[] defaultRatings = new int[100];

        public string Name
        {
            get { return name; }
        }

        public int[] DefaultRatings
        {
            get { return defaultRatings; }
        }

        public Faction()
        {
            name = "";
            for (int i = 0; i < defaultRatings.Length; ++i)
                defaultRatings[i] = 0;
        }

        const int TotalFactions = 100;
        public const string FactionDatabase = "Data/Server Data/Factions.s3db";
        public static Faction[] Factions = new Faction[TotalFactions];

        public static int Load(string path)
        {
            for (int i = 0; i < Factions.Length; ++i)
                Factions[i] = new Faction();

            SqliteConnection databaseConnection = new SqliteConnection();
            string cs = string.Format("Version=3,uri=file:{0}", path);
            databaseConnection.ConnectionString = cs;
            databaseConnection.Open();

            IDbCommand cmd = databaseConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Factions";
            IDataReader reader = cmd.ExecuteReader();
            int factionCount = 0;
            while (reader.Read())
            {
                int index = reader.GetInt32(reader.GetOrdinal("ID"));
                Factions[index].name = reader.GetString(reader.GetOrdinal("Name"));
                byte[] data = (byte[])reader.GetValue(reader.GetOrdinal("Ratings"));

                // Read ratings out
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
                {
                    // This is for future use when we make factions not a fixed array
                    int totalFactions = binaryReader.ReadInt32();

                    // Reads and writes ratings as int for larger range in future
                    for (int i = 0; i < TotalFactions; i++)
                        Factions[index].defaultRatings[i] = (byte)binaryReader.ReadInt32();
                }

                factionCount++;
            }
            databaseConnection.Close();
            return factionCount;
            #region Old binary file
            /*
            BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

            int Count = 0;

            for (int i = 0; i < Factions.Length; ++i)
            {
                Factions[i].name = F.ReadBBString();
                if (Factions[i].Name.Length > 0)
                    ++Count;
            }

            for(int i = 0; i < Factions.Length; ++i)
                for(int j = 0; j < Factions[i].defaultRatings.Length; ++j)
                    Factions[i].defaultRatings[j] = (int)(F.ReadSByte());

            F.Close();
            return Count;
             */
            #endregion
        }
    }
}
