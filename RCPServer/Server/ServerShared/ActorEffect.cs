using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
	public class ActorEffect
	{
        public string Name = "";
        public Attributes Attributes = new Attributes();
        public uint CreatedTime = 0;
        public uint Length = 0;
        public ushort IconTexID = 65535;
        public int AllocID = 0;

        public ActorEffect()
        {
            ++AllocIDs;
            AllocID = AllocIDs;
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            Pa.Write((byte)Name.Length);
            Pa.Write(Name, false);
            Attributes.Serialize(Pa);

            // Store time active rather than time existing
            Pa.Write(Server.MilliSecs() - CreatedTime);

            Pa.Write(Length);
            Pa.Write(IconTexID);
            Pa.Write(AllocID);
        }

        public static ActorEffect Deserialize(Scripting.PacketReader Pa, Scripting.PacketWriter ReAllocator)
        {
            ActorEffect AE = new ActorEffect();
            AE.Name = Pa.ReadString(Pa.ReadByte());
            AE.Attributes.Deserialize(Pa);

            // Recalculate time
            AE.CreatedTime = Server.MilliSecs() + Pa.ReadUInt32();

            AE.Length = Pa.ReadUInt32();
            AE.IconTexID = Pa.ReadUInt16();
            int tAllocID = Pa.ReadInt32();

            ReAllocator.Write((byte)'E');
            ReAllocator.Write(tAllocID);
            ReAllocator.Write(AE.AllocID);

            return AE;
        }

        public static int AllocIDs = 0;
	}
}
