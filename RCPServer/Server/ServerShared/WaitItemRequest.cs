using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class WaitItemRequest : Scripting.WaitItemRequest
    {
        bool callScript = false;
        string callbackScript = "", callbackMethod = "";
        public event Scripting.WaitItemRequest.RequestHandler OnWaitItem;

        public WaitItemRequest(string waitItemName, ushort waitItemCount)
        {
            itemName = waitItemName;
            itemCount = waitItemCount;
        }

        public WaitItemRequest(string scriptName, string methodName, string waitItemName, ushort waitItemCount)
        {
            itemName = waitItemName;
            itemCount = waitItemCount;
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
            if (OnWaitItem != null)
                OnWaitItem.Invoke(ai, this);
        }

        public void Serialize(Scripting.PacketWriter pa)
        {
            pa.Write((byte)itemName.Length);
            pa.Write(itemName, false);
            pa.Write((ushort)itemCount);
            pa.Write((byte)callbackScript.Length);
            pa.Write(callbackScript, false);
            pa.Write((byte)callbackMethod.Length);
            pa.Write(callbackMethod, false);
        }

        public static WaitItemRequest Deserialize(Scripting.PacketReader pa)
        {
            string itemName = pa.ReadString(pa.ReadByte());
            ushort itemCount = pa.ReadUInt16();
            string CBScript = pa.ReadString(pa.ReadByte());
            string CBMethod = pa.ReadString(pa.ReadByte());

            WaitItemRequest Wk = new WaitItemRequest(CBScript, CBMethod, itemName, itemCount);
            return Wk;
        }
    }
}
