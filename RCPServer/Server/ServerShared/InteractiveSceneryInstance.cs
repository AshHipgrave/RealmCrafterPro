using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class InteractiveSceneryInstance : Scripting.SceneryInstance
    {
        InteractiveScenery sceneryHandle;
        Dictionary<string, byte[]> globals;

        public InteractiveSceneryInstance(InteractiveScenery inSceneryHandle, Dictionary<string, byte[]> inGlobals)
        {
            sceneryHandle = inSceneryHandle;
            globals = inGlobals;
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            Pa.Write(sceneryHandle.Position);
            Pa.Write(sceneryHandle.MeshID);
            Pa.Write((ushort)globals.Count);
            foreach (KeyValuePair<string, byte[]> Kvp in globals)
            {
                Pa.Write((byte)Kvp.Key.Length);
                Pa.Write(Kvp.Key, false);

                Pa.Write((ushort)Kvp.Value.Length);
                Pa.Write(Kvp.Value, 0);
            }
        }

        public void Deserialize(Scripting.PacketReader Pa)
        {


            ushort Count = Pa.ReadUInt16();
            for (ushort i = 0; i < Count; ++i)
            {
                string KeyName = Pa.ReadString(Pa.ReadByte());
                byte[] KeyValue = Pa.ReadBytes(Pa.ReadUInt16());

                globals.Add(KeyName, KeyValue);
            }
        }

        public override Scripting.Scenery Scenery
        {
            get { return sceneryHandle; }
        }

        public override Dictionary<string, byte[]> Globals
        {
            get { return globals; }
        }
    }
}
