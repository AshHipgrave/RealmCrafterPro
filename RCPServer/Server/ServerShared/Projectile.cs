using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;

namespace RCPServer
{
    public class Projectile
    {
        public int ID = 0;
        public string Name = "Undefined Projectile";
        public string Emitter1 = "", Emitter2 = "";
        public ushort MeshID = 65535;
        public ushort Emitter1TexID = 65535;
        public ushort Emitter2TexID = 65535;
        public ushort Damage = 0;
        public ushort DamageType = 0;
        public byte Homing = 0;
        public byte HitChance = 0;
        public byte Speed = 0;

        public Projectile()
        {

        }

        protected static Projectile[] Projectiles = new Projectile[5000];

        public static Projectile FromList(int id)
        {
            if (id < 0 || id >= Projectiles.Length)
                return null;

            return Projectiles[id];
        }

        public static Projectile Find(string name)
        {
            foreach (Projectile P in Projectiles)
            {
                if (P.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return P;
            }
            return null;
        }

        public static int Load(string path)
        {
            for (int i = 0; i < Projectiles.Length; ++i)
                Projectiles[i] = null;

            BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

            int Count = 0;

            while (!F.Eof)
            {
                Projectile P = new Projectile();
                P.ID = F.ReadUInt16();
			    P.Name = F.ReadBBString();
			    P.MeshID = F.ReadUInt16();
			    P.Emitter1 = F.ReadBBString();
			    P.Emitter2 = F.ReadBBString();
			    P.Emitter1TexID = F.ReadUInt16();
			    P.Emitter2TexID = F.ReadUInt16();
			    P.Homing = F.ReadByte();
			    P.HitChance = F.ReadByte();
			    P.Damage = F.ReadUInt16();
			    P.DamageType = F.ReadUInt16();
			    P.Speed = F.ReadByte();

			    Projectiles[P.ID] = P;
			    ++Count;
            }

            F.Close();
            return Count;
        }
    }
}
