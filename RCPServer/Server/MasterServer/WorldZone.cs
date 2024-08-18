using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RCPServer
{
    public class WorldZone
    {
        public string Name = "Unknown";

        public static List<WorldZone> WorldZoneList = new List<WorldZone>();

        public static WorldZone Find(string name)
        {
            foreach (WorldZone Zone in WorldZoneList)
            {
                if (Zone.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return Zone;
            }

            return null;
        }

        public static int Load(string path)
        {
            string[] Files = Directory.GetFiles(path, "*.dat");

            foreach (string FileName in Files)
            {
                WorldZone WI = new WorldZone();
                WI.Name = Path.GetFileNameWithoutExtension(FileName);
                WorldZoneList.Add(WI);

                // Always add a default zone
                new ZoneInstance(0, WI, null);

                // Change instances
                //TODO: Change this to support unlimited instances
//                 for (int k = 0; k < 100; ++k)
//                 {
//                     if (File.Exists(Path.Combine(path, @"Ownerships\" + WI.Name + " (" + k.ToString() + ") Ownerships.dat")))
//                     {
//                         // Pass a null host handle so that this zone is allocated when a world server joins
//                         new ZoneInstance(k, WI, null);
//                     }
//                 }
            }

            return ZoneInstance.ZoneInstances.Count;
        }

        public static byte[] Load(ZoneInstance inst)
        {
            if (inst.InstanceOf == null)
                return BitConverter.GetBytes(0);

            FileStream Stream = null;

            string Filepath = @"Data\Server Data\Areas\Ownerships\" + inst.InstanceOf.Name + " (" + inst.ID + ").dat";

            if(File.Exists(Filepath))
            {
                using (Stream = File.Open(Filepath, FileMode.Open))
                {
                    byte[] Data = new byte[Stream.Length];
                    Stream.Read(Data, 0, Data.Length);

                    return Data;
                }
            }

            return BitConverter.GetBytes(0);
        }

        public static void Save(ZoneInstance inst, byte[] data)
        {
            if (inst.InstanceOf == null)
                return;

            FileStream Stream = null;

            using (Stream = File.Open(@"Data\Server Data\Areas\Ownerships\" + inst.InstanceOf.Name + " (" + inst.ID + ").dat", FileMode.Create))
            {
                Stream.Write(data, 0, data.Length);
                Stream.Close();
            }
        }
    }
}
