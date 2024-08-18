using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ActorDialog : Scripting.ActorDialog
    {
        static int AllocIDs = 0;
        static LinkedList<ActorDialog> OpenDialogs = new LinkedList<ActorDialog>();

        public static ActorDialog FindActorDialog(int id)
        {
            foreach (ActorDialog D in OpenDialogs)
            {
                if (D.allocID == id)
                    return D;
            }

            return null;
        }

        bool waitingForInput = false;
        ActorInstance actor;
        ActorInstance contextActor;
        int allocID;
        public event Scripting.DialogEventHandler InputClick;

        public ActorDialog(ActorInstance inActor, ActorInstance inContextActor, string title, ushort backgroundID)
        {
            allocID = ++AllocIDs;

            actor = inActor;
            contextActor = inContextActor;

            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'N');
            Writer.Write(allocID);

            if (contextActor == null)
                Writer.Write((ushort)0);
            else
                Writer.Write((ushort)contextActor.RuntimeID);
            Writer.Write(backgroundID);
            Writer.Write(title, false);

            RCEnet.Send(actor.RNID, MessageTypes.P_Dialog, Writer.ToArray(), true);
            OpenDialogs.AddLast(this);
        }

        public override void Close()
        {
            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'C');
            Writer.Write(allocID);

            RCEnet.Send(actor.RNID, MessageTypes.P_Dialog, Writer.ToArray(), true);

            actor.Dialogs.Remove(this);
            if (contextActor != null)
                contextActor.Dialogs.Remove(this);
            OpenDialogs.Remove(this);

            // Remove a cyclic reference
            actor = null;
            contextActor = null;
            
        }

        public override void Output(string message)
        {
            Output(message, System.Drawing.Color.White);
        }

        public override void Output(string message, System.Drawing.Color color)
        {
            if (waitingForInput)
                throw new Exception("Error: ActorDialog.Output() called while already waiting for client input!");

            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'T');
            Writer.Write((byte)color.R);
            Writer.Write((byte)color.G);
            Writer.Write((byte)color.B);
            Writer.Write(allocID);
            Writer.Write(message, false);

            RCEnet.Send(actor.RNID, MessageTypes.P_Dialog, Writer.ToArray(), true);
        }

        public override void Input(string[] inputs)
        {
            if (waitingForInput)
                throw new Exception("Error: ActorDialog.Input() called while already waiting for client input!");

            Scripting.PacketWriter Writer = new Scripting.PacketWriter();
            Writer.Write(actor.GCLID);
            Writer.Write((byte)'O');
            Writer.Write(allocID);
            for(int i = 0; i < inputs.Length; ++i)
            {
                if(inputs[i].Length > 255)
                    throw new Exception("Input: [" + i.ToString() + ", '" + inputs[i].Substring(0, 10) + "...'] is too long! (" + inputs[i].Length.ToString() + " > 255)");

                Writer.Write((byte)inputs[i].Length);
                Writer.Write(inputs[i], false);
            }

            waitingForInput = true;
            RCEnet.Send(actor.RNID, MessageTypes.P_Dialog, Writer.ToArray(), true);
        }

        public void ReceiveInput(int index)
        {
            if (!waitingForInput)
                return;

            Scripting.DialogEventArgs E = new Scripting.DialogEventArgs(index);

            // Disable input wait before callback to allow the callback to Output()
            waitingForInput = false;
            InputClick.Invoke(this, E);
        }
    }
}
