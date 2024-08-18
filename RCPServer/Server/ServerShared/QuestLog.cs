using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class QuestLog
    {
        string[] entryName = new string[500];
        //string[] entryStatus = new string[500];
        List<byte[]> entryStatus = new List<byte[]>();
        List<byte[]> entryData = new List<byte[]>();

        public string[] EntryName
        {
            get { return entryName; }
        }

        /// <summary>
        /// Status of a quest in a byte array. NOTE: This has a maximum of 64kb of space.
        /// </summary>
        public List<byte[]> EntryStatus
        {
            get { return entryStatus; }
        }

        /// <summary>
        /// Tag data of a quest
        /// </summary>
        public List<byte[]> EntryData
        {
            get { return entryData; }
        }

        public QuestLog()
        {
            for (int i = 0; i < entryName.Length; ++i)
            {
                entryName[i] = "";

                entryStatus.Add(null);
                entryData.Add(new byte[0]);
            }
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            if (entryStatus.Count != entryName.Length)
                throw new Exception("QuestLog EntryName and EntryStatus Length do not match!");

            Pa.Write((ushort)entryName.Length);
            for (int i = 0; i < entryName.Length; ++i)
            {
                Pa.Write((byte)entryName[i].Length);
                Pa.Write(entryName[i], false);

                if (entryStatus[i] == null)
                {
                    Pa.Write((ushort)0);
                }
                else
                {
                    Pa.Write((ushort)entryStatus[i].Length);
                    Pa.Write(entryStatus[i], 0);
                }

                Pa.Write((ushort)entryData[i].Length);
                Pa.Write(entryData[i], 0);
            }
        }

        public void Deserialize(Scripting.PacketReader Pa)
        {
            ushort Length = Pa.ReadUInt16();

            for (int i = 0; i < Length; ++i)
            {
                entryName[i] = Pa.ReadString(Pa.ReadByte());
                entryStatus[i] = Pa.ReadBytes(Pa.ReadUInt16());
                entryData[i] = Pa.ReadBytes(Pa.ReadUInt16());
            }
        }
    }
}
