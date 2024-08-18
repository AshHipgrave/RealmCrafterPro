using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;

namespace RCPServer
{
    public class SuperGlobals
    {
        // In 2.50, super globals are now byte arrays indexed by name!
        public static Dictionary<string, byte[]> Globals = new Dictionary<string, byte[]>();
        //public static string[] Globals = new string[100];

        public static bool Load(string path)
        {
            try
            {
                BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

                // Read a header, to automagically update the existing file
                if (F.ReadChar() != 'R' || F.ReadChar() != 'C' || F.ReadChar() != '2' || F.ReadChar() != '5')
                {
                    F.Close();
                    Log.WriteLine("SuperGlobals file definition was the incorrect version. I will attempt to update it.");
                    throw new FileNotFoundException("Incorrect file version");
                }

                while (!F.Eof)
                {
                    string Key = F.ReadBBString();
                    int Length = F.ReadInt32();
                    byte[] Data = new byte[Length];

                    F.Read(Data, 0, Length);

                    Globals.Add(Key, Data);
                }

                F.Close();

                return true;
            }
            catch (FileNotFoundException)
            {
                FileStream F = File.Create(path);

                F.WriteByte((byte)'R');
                F.WriteByte((byte)'C');
                F.WriteByte((byte)'2');
                F.WriteByte((byte)'5');

                F.Close();

                return true;
            }catch
            {
               return false;
            }
        }

        public static bool Save(string path)
        {
            BBBinaryWriter F = new BBBinaryWriter(File.Open(path, FileMode.Create));

            F.Write((byte)'R');
            F.Write((byte)'C');
            F.Write((byte)'2');
            F.Write((byte)'5');

            foreach (KeyValuePair<string, byte[]> I in Globals)
            {
                F.WriteBBString(I.Key);
                F.Write(I.Value.Length);
                F.Write(I.Value);
            }

            F.Close();

            return true;
        }
    }
}
