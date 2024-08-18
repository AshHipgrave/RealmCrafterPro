using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ActorInfo : Scripting.ActorInfo
    {
        #region Static Members
        // List of active requests
        public static LinkedList<Scripting.ActorInfoRequest> Requests = new LinkedList<Scripting.ActorInfoRequest>();

        // Update ZoneServer requests
        public static void Update()
        {
            LinkedListNode<Scripting.ActorInfoRequest> RNode = Requests.First;
            while (RNode != null)
            {
                Scripting.ActorInfoRequest R = RNode.Value;
                LinkedListNode<Scripting.ActorInfoRequest> Del = RNode;
                RNode = RNode.Next;

                if (R.IsExpired(Server.MilliSecs()))
                {
                    ActorInfo Info = ActorInfo.FromTimeout(R.ActorName, Scripting.ActorInfoState.RequestTimedOut_ZoneServer);
                    R.Invoke(Info);

                    Requests.Remove(Del);
                }
            }
        }

        public static Scripting.ActorInfoRequest RequestFromAllocID(uint id)
        {
            foreach (Scripting.ActorInfoRequest R in Requests)
            {
                if (R.AllocID == id)
                    return R;
            }

            return null;
        }
        
        // Get ActorInfo from data sent back from the accountscript
        public static ActorInfo FromOffline(string actorName, string setAccountName, bool isBanned, bool isGM)
        {
            ActorInfo I = new ActorInfo();
            I.state = Scripting.ActorInfoState.Offline;
            I.name = actorName;
            I.accountName = setAccountName;
            I.banned = isBanned;
            I.gm = isGM;

            return I;
        }

        // Get ActorInfo from an existing actor (great for just copying data)
        public static ActorInfo FromHandle(ActorInstance ai)
        {
            ActorInfo Info = new ActorInfo();
            Info.internalHandle = ai;
            Info.GCLID = ai.GCLID;
            Info.state = ai.InGame ? Scripting.ActorInfoState.Online : Scripting.ActorInfoState.Offline;
            Info.name = ai.name;
            Info.accountName = ai.accountName;
            Info.actorClass = ai.Class;
            Info.actorRace = ai.Race;
            Info.zoneName = ai.ServerArea.Name;
            Info.banned = ai.Banned;
            Info.gm = ai.GM;
            Info.baseActorID = ai.BaseActorID;
            Info.beard = ai.Beard;
            Info.clothes = ai.Clothes;
            Info.face = ai.Face;
            Info.hair = ai.Hair;
            Info.gender = ai.Gender;
            Info.position = ai.Position;

            return Info;
        }

        // From Packet (deserialize)
        public static ActorInfo FromPacket(Scripting.PacketReader pa)
        {
            ActorInfo I = new ActorInfo();
            I.state = (Scripting.ActorInfoState)pa.ReadByte();
            I.GCLID = pa.ReadUInt32();
            I.name = pa.ReadString(pa.ReadByte());
            I.accountName = pa.ReadString(pa.ReadByte());
            I.actorClass = pa.ReadString(pa.ReadByte());
            I.actorRace = pa.ReadString(pa.ReadByte());
            I.zoneName = pa.ReadString(pa.ReadByte());

            byte Flags = pa.ReadByte();
            I.banned = (Flags & 1) > 0;
            I.gm = (Flags & 2) > 0;

            I.baseActorID = pa.ReadUInt16();
            I.beard = pa.ReadByte();
            I.clothes = pa.ReadByte();
            I.face = pa.ReadByte();
            I.hair = pa.ReadByte();
            I.gender = pa.ReadByte();
            I.position = pa.ReadSectorVector();

            return I;
        }

        // Get ActorInfo from a timeout state
        public static ActorInfo FromTimeout(string name, Scripting.ActorInfoState state)
        {
            ActorInfo Info = new ActorInfo();
            Info.state = state;
            Info.name = name;

            return Info;
        }

        #endregion


        Scripting.ActorInfoState state = Scripting.ActorInfoState.NotFound;
        string name = "", accountName = "", actorClass = "", actorRace = "", zoneName = "";
        bool banned = false, gm = false;
        uint baseActorID = 65535;
        int beard = 0, clothes = 0, face = 0, hair = 0, gender = 1;
        Scripting.Math.SectorVector position = Scripting.Math.SectorVector.Zero;
        public object proxyServer; // Naughty, but keeps Info shared
        public int ServerConnection = 0;
        uint GCLID = 0;
        ActorInstance internalHandle;



        #region Properties
        public override Scripting.ActorInfoState State { get { return state; } }

        public override string Name { get { return name; } }

        public override string AccountName { get { return accountName; } }

        public override string Class { get { return actorClass; } }

        public override string Race { get { return actorRace; } }

        public override string ZoneName { get { return zoneName; } }

        public override bool Banned { get { return banned; } }

        public override bool GM { get { return gm; } }

        public override uint BaseActorID { get { return baseActorID; } }

        public override int Beard { get { return beard; } }

        public override int Clothes { get { return clothes; } }

        public override int Face { get { return face; } }

        public override int Hair { get { return hair; } }

        public override int Gender { get { return gender; } }

        public override global::Scripting.Math.SectorVector Position { get { return position; } }

        public override Scripting.Actor Handle { get { return internalHandle; } }

        #endregion
        #region Methods
        public void Serialize(Scripting.PacketWriter pa)
        {
            Scripting.PacketWriter Pa = new Scripting.PacketWriter();
            Pa.Write((byte)state);
            Pa.Write(GCLID);
            Pa.Write((byte)name.Length);
            Pa.Write(name, false);
            Pa.Write((byte)accountName.Length);
            Pa.Write(accountName, false);
            Pa.Write((byte)actorClass.Length);
            Pa.Write(actorClass, false);
            Pa.Write((byte)actorRace.Length);
            Pa.Write(actorRace, false);
            Pa.Write((byte)zoneName.Length);
            Pa.Write(zoneName, false);

            byte Flags = 0;
            Flags += (byte)(banned ? 1 : 0);
            Flags += (byte)(gm ? 2 : 0);
            Pa.Write(Flags);
            Pa.Write((ushort)baseActorID);
            Pa.Write((byte)beard);
            Pa.Write((byte)clothes);
            Pa.Write((byte)face);
            Pa.Write((byte)hair);
            Pa.Write((byte)gender);
            Pa.Write(position);

            pa.Write((ushort)Pa.Length);
            pa.Write(Pa.ToArray());
        }

        private bool IsLocal()
        {
            if (internalHandle == null)
                return false;
            return true;
        }

        public override void Output(byte tabID, string message)
        {
            Output(tabID, message, System.Drawing.Color.White);
        }

        public override void Output(byte tabID, string message, System.Drawing.Color color)
        {
            if (state != Scripting.ActorInfoState.Online)
                return;

            if (internalHandle != null)
            {
                internalHandle.Output(tabID, message, color);
                return;
            }

            if (ServerConnection == 0)
                return;

            if (message.Length > 255)
                message = message.Substring(0, 255);

            Scripting.PacketWriter Pa = new Scripting.PacketWriter();
            Pa.Write(GCLID);
            Pa.Write((byte)'O');
            Pa.Write((byte)tabID);
            Pa.Write(color.R);
            Pa.Write(color.G);
            Pa.Write(color.B);
            Pa.Write((byte)message.Length);
            Pa.Write(message, false);

            RCEnet.Send(ServerConnection, MessageTypes.P_ActorInfoCommand, Pa.ToArray(), true);
        }

        public override void Warp(string zoneName, string portalName)
        {
            Warp(zoneName, portalName, 0);
        }

        public override void Warp(string zoneName, string portalName, int instanceNumber)
        {
            if(state != Scripting.ActorInfoState.Online)
                return;

            if(internalHandle != null)
            {
                internalHandle.Warp(zoneName, portalName, instanceNumber);
                return;
            }

            if(ServerConnection == 0)
                return;

            Scripting.PacketWriter Pa = new Scripting.PacketWriter();
            Pa.Write(GCLID);
            Pa.Write((byte)'W');
            Pa.Write((byte)zoneName.Length);
            Pa.Write(zoneName, false);
            Pa.Write((byte)portalName.Length);
            Pa.Write(portalName, false);
            Pa.Write(instanceNumber);

            RCEnet.Send(ServerConnection, MessageTypes.P_ActorInfoCommand, Pa.ToArray(), true);
        }

        public override void Kill()
        {
            if (state != Scripting.ActorInfoState.Online)
                return;

            if (internalHandle != null)
            {
                internalHandle.Kill();
                return;
            }

            if (ServerConnection == 0)
                return;

            Scripting.PacketWriter Pa = new Scripting.PacketWriter();
            Pa.Write(GCLID);
            Pa.Write((byte)'K');

            RCEnet.Send(ServerConnection, MessageTypes.P_ActorInfoCommand, Pa.ToArray(), true);
        }

        public override void ExecuteScript(string scriptName, string methodName, byte[] userData)
        {
            if (state != Scripting.ActorInfoState.Online)
                return;

            if (internalHandle != null)
            {
                Scripting.ScriptManager.DelayedExecute(scriptName, methodName, new object[] { internalHandle, userData }, internalHandle, null);

                return;
            }

            if (ServerConnection == 0)
                return;

            
            Scripting.PacketWriter Pa = new Scripting.PacketWriter();
            Pa.Write(GCLID);
            Pa.Write((byte)'S');
            Pa.Write((byte)scriptName.Length);
            Pa.Write(scriptName, false);
            Pa.Write((byte)methodName.Length);
            Pa.Write(methodName, false);
            Pa.Write((ushort)userData.Length);
            Pa.Write(userData);

            RCEnet.Send(ServerConnection, MessageTypes.P_ActorInfoCommand, Pa.ToArray(), true);
        }

        #endregion
    }
}
