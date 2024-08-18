using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{

    public class Attributes
    {
        int[] value = new int[40];
        int[] maximum = new int[40];

        public int[] Value
        {
            get { return value; }
        }

        public int[] Maximum
        {
            get { return maximum; }
        }

        public Attributes()
        {
            for (int i = 0; i < value.Length; ++i)
            {
                value[i] = 0;
                maximum[i] = 0;
            }
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            if (value.Length != maximum.Length)
                throw new Exception("Critical Error: Attribute Value and Maximum Lengths did not match!");
            if (value.Length > 255)
                throw new Exception("Attributes.value.Length is using an incorrect type!");


            Pa.Write((byte)value.Length);
            for (int i = 0; i < value.Length; ++i)
            {
                Pa.Write(value[i]);
                Pa.Write(maximum[i]);
            }
        }

        public void Deserialize(Scripting.PacketReader Pa)
        {
            byte Length = Pa.ReadByte();
            if (Length != value.Length)
                throw new Exception("Critical Error: Attribute length from packet did not match!");

            for (int i = 0; i < Length; ++i)
            {
                value[i] = Pa.ReadInt32();
                maximum[i] = Pa.ReadInt32();
            }
        }
    }

}
