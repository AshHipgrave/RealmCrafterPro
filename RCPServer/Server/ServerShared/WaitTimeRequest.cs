using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class WaitTimeRequest : Scripting.WaitTimeRequest
    {
        bool callScript = false;
        string callbackScript = "", callbackMethod = "";
        public event Scripting.WaitTimeRequest.RequestHandler Elapsed;

        public WaitTimeRequest(int waitHour, int waitMinute)
        {
            hour = waitHour;
            minute = waitMinute;
        }

        public WaitTimeRequest(string scriptName, string methodName, int waitHour, int waitMinute)
        {
            hour = waitHour;
            minute = waitMinute;
            callbackScript = scriptName;
            callbackMethod = methodName;
            callScript = true;
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
            if (Elapsed != null)
                Elapsed.Invoke(ai, this);
        }

        public void Serialize(Scripting.PacketWriter pa)
        {
            pa.Write((ushort)(short)hour);
            pa.Write((ushort)(short)minute);
            pa.Write((byte)callbackScript.Length);
            pa.Write(callbackScript, false);
            pa.Write((byte)callbackMethod.Length);
            pa.Write(callbackMethod, false);
        }

        public static WaitTimeRequest Deserialize(Scripting.PacketReader pa)
        {
            int WaitHour = (short)pa.ReadUInt16();
            int WaitMinute = (short)pa.ReadUInt16();
            string CBScript = pa.ReadString(pa.ReadByte());
            string CBMethod = pa.ReadString(pa.ReadByte());

            return new WaitTimeRequest(CBScript, CBMethod, WaitHour, WaitMinute);
        }
    }
}
