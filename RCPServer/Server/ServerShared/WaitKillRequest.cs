using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class WaitKillRequest : Scripting.WaitKillRequest
    {
        ushort currentCount = 0;
        bool callScript = false;
        string callbackScript = "", callbackMethod = "";
        public event Scripting.WaitKillRequest.RequestHandler OnKill;

        public WaitKillRequest(ushort waitActorID, ushort waitKillCount)
        {
            actorID = waitActorID;
            killCount = waitKillCount;
        }

        public WaitKillRequest(string scriptName, string methodName, ushort waitActorID, ushort waitKillCount)
        {
            actorID = waitActorID;
            killCount = waitKillCount;
            callbackScript = scriptName;
            callbackMethod = methodName;
            callScript = true;
        }

        public ushort CurrentCount
        {
            get { return currentCount; }
            set { currentCount = value; }
        }

        public bool CallScript
        {
            get
            {
                return callScript;
            }
        }

        public string CallbackScript
        {
            get { return callbackScript; }
        }

        public string CallbackMethod
        {
            get { return callbackMethod; }
        }

        public void Invoke(ActorInstance ai)
        {
            if (OnKill != null)
                OnKill.Invoke(ai, this);
        }

        public void Serialize(Scripting.PacketWriter pa)
        {
            pa.Write((ushort)actorID);
            pa.Write((ushort)killCount);
            pa.Write((ushort)currentCount);
            pa.Write((byte)callbackScript.Length);
            pa.Write(callbackScript, false);
            pa.Write((byte)callbackMethod.Length);
            pa.Write(callbackMethod, false);
        }

        public static WaitKillRequest Deserialize(Scripting.PacketReader pa)
        {
            ushort actorID = pa.ReadUInt16();
            ushort killCount = pa.ReadUInt16();
            ushort currentCount = pa.ReadUInt16();
            string CBScript = pa.ReadString(pa.ReadByte());
            string CBMethod = pa.ReadString(pa.ReadByte());

            WaitKillRequest Wk = new WaitKillRequest(CBScript, CBMethod, actorID, killCount);
            Wk.CurrentCount = currentCount;
            return Wk;
        }
    }
}
