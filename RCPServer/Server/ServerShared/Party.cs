using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace RCPServer
{
    public class Party : PartyInstance 
    {
        public int Members { get { return Players.Count; } }
        public List<ActorInstance> Players;
        public byte[] Data;

        public Party()
        {
            Players = new List<ActorInstance>();

        }

        public void AddMember(ActorInstance actor)
        {
            Players.Add(actor);
            actor.PartyID = this;
        }

        public void RemoveMember(ActorInstance actor)
        {
            Players.Remove(actor);
            actor.PartyID = null;
        }

        // Scripting overloads

        public override List<Scripting.Actor> GetCurrentMembers()
        {
            List<Scripting.Actor> scriptActorList = new List<Scripting.Actor>();
            foreach (ActorInstance a in Players)
                scriptActorList.Add(a as Scripting.Actor);

            return scriptActorList;
        }

        public override byte[] GetData()
        {
            return Data;
        }

        public override void SetData(byte[] data)
        {
            Data = data;
        }

        // NOTE DOES NOT SAVE PARTY - JUST DROPS TEMP DATA IN

        public void Serialize(Scripting.PacketWriter Pa)
        {
            // Write temp data to stop server from throwing a fit
            Pa.Write((byte)8);
            for (int i = 0; i < 8; ++i)
            {
                Pa.Write((byte)i);
                Pa.Write((uint)0);
            }

        }

        public static Party Deserialize(Scripting.PacketReader Pa)
        {
            byte Length = Pa.ReadByte();
            if (Length == 0)
                return null;

            Party P = new Party();
            for (int i = 0; i < Length; ++i)
            {
                byte Idx = Pa.ReadByte();
                uint GCLID = Pa.ReadUInt32();

            }

            return null;
        }
    }
}
