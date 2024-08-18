using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class WaitSpeakRequest : Scripting.WaitSpeakRequest
    {
        bool callScript = false;
        string callbackScript = "", callbackMethod = "";
        public event Scripting.WaitSpeakRequest.RequestHandler OnInteract;

        public WaitSpeakRequest(string waitZoneName, string waitActorName)
        {
            zoneName = waitZoneName;
            actorName = waitActorName;
        }

        public WaitSpeakRequest(string scriptName, string methodName, string waitZoneName, string waitActorName)
        {
            zoneName = waitZoneName;
            actorName = waitActorName;
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

        public void Invoke(ActorInstance ai, ActorInstance context)
        {
            if (OnInteract != null)
                OnInteract.Invoke(ai, context);
        }

        public void Serialize(Scripting.PacketWriter pa)
        {
            pa.Write((byte)zoneName.Length);
            pa.Write(zoneName, false);
            pa.Write((byte)actorName.Length);
            pa.Write(actorName, false);
            pa.Write((byte)callbackScript.Length);
            pa.Write(callbackScript, false);
            pa.Write((byte)callbackMethod.Length);
            pa.Write(callbackMethod, false);
        }

        public static WaitSpeakRequest Deserialize(Scripting.PacketReader pa)
        {
            string ZName = pa.ReadString(pa.ReadByte());
            string AName = pa.ReadString(pa.ReadByte());
            string CBScript = pa.ReadString(pa.ReadByte());
            string CBMethod = pa.ReadString(pa.ReadByte());

            return new WaitSpeakRequest(CBScript, CBMethod, ZName, AName);
        }
    }
}
