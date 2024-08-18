using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ActionBarData
    {
        string[] slots = new string[36];

        public ActionBarData()
        {
            for (int i = 0; i < slots.Length; ++i)
                slots[i] = "";
        }

        public string[] Slots
        {
            get { return slots; }
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            Pa.Write((byte)slots.Length);

            for (int i = 0; i < slots.Length; ++i)
            {
                Pa.Write((byte)slots[i].Length);
                Pa.Write(slots[i], false);
            }
        }

        public void Deserialize(Scripting.PacketReader Pa)
        {
            int Length = Pa.ReadByte();

            for (int i = 0; i < Length; ++i)
            {
                slots[i] = Pa.ReadString(Pa.ReadByte());
            }
        }
    }
}
