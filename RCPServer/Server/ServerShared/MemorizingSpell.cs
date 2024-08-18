using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class MemorizingSpell
    {
        public int KnownNum = 0;
        public uint CreatedTime = 0;

        public MemorizingSpell()
        {
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            Pa.Write((ushort)KnownNum);
            Pa.Write(Server.MilliSecs() - CreatedTime);
        }

        public static MemorizingSpell Deserialize(Scripting.PacketReader Pa)
        {
            MemorizingSpell Ms = new MemorizingSpell();
            Ms.KnownNum = Pa.ReadUInt16();
            Ms.CreatedTime = Server.MilliSecs() + Pa.ReadUInt32();

            return Ms;
        }


    }
}
