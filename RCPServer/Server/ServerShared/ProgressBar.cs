using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ProgressBar : Scripting.ProgressBar
    {
        static int AllocIDs = 0;
        static LinkedList<ProgressBar> OpenBars = new LinkedList<ProgressBar>();

        public static ProgressBar FindActorProgressBar(int id)
        {
            foreach (ProgressBar P in OpenBars)
            {
                if (P.allocID == id)
                    return P;
            }

            return null;
        }

        ActorInstance actor;
        int allocID;

        public ProgressBar(ActorInstance inActor)
        {
            actor = inActor;
            AllocIDs = ++AllocIDs;

            OpenBars.AddLast(this);
        }

        public ProgressBar(ActorInstance inActor, System.Drawing.Color color, float x, float y, float width, float height, int maximum, int value, string label)
        {
            allocID = ++AllocIDs;

            actor = inActor;

            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'C');
            Writer.Write((byte)color.R);
            Writer.Write((byte)color.G);
            Writer.Write((byte)color.B);
            Writer.Write(x);
            Writer.Write(y);
            Writer.Write(width);
            Writer.Write(height);
            Writer.Write(allocID);
            Writer.Write((ushort)maximum);
            Writer.Write((ushort)value);
            Writer.Write(label, false);

            RCEnet.Send(actor.RNID, MessageTypes.P_ProgressBar, Writer.ToArray(), true);
            OpenBars.AddLast(this);
        }

        public override void Remove()
        {
            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'D');
            Writer.Write(allocID);

            RCEnet.Send(actor.RNID, MessageTypes.P_ProgressBar, Writer.ToArray(), true);

            actor.ProgressBars.Remove(this);
            OpenBars.Remove(this);

            // Remove a cyclic reference
            actor = null;
        }

        public override void Update(int value)
        {
            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'U');
            Writer.Write(allocID);
            Writer.Write((ushort)value);

            RCEnet.Send(actor.RNID, MessageTypes.P_ProgressBar, Writer.ToArray(), true);
        }

        public void Serialize(Scripting.PacketWriter pa)
        {
            pa.Write(allocID);
        }

        public static ProgressBar Deserialize(ActorInstance ai, Scripting.PacketReader pa, Scripting.PacketWriter ReAllocator)
        {
            ProgressBar Pr = new ProgressBar(ai);
            ReAllocator.Write((byte)'P');
            ReAllocator.Write(pa.ReadInt32());
            ReAllocator.Write(Pr.allocID);

            return Pr;
        }

       
    }
}
