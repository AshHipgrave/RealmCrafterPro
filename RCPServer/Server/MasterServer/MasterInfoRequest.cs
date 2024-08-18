using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class MasterInfoRequest
    {
        public static LinkedList<MasterInfoRequest> Requests = new LinkedList<MasterInfoRequest>();
        uint allocIDs = 0;

        public uint PostTime = 0;
        public string ActorName;
        public Scripting.ActorInfoRequestState State;
        public uint ZSAllocID;
        public uint AllocID;
        public int WaitingForServers = 0;
        public ZoneServer Server;
        public byte[] InfoData;

        public MasterInfoRequest()
        {
            AllocID = ++allocIDs;
        }

        // Get instance from AllocID
        public static MasterInfoRequest FromAllocID(uint id)
        {
            foreach (MasterInfoRequest R in Requests)
            {
                if (R.AllocID == id)
                    return R;
            }

            return null;
        }

        // Update for timeouts
        public static void Update()
        {
            LinkedListNode<MasterInfoRequest> RNode = Requests.First;
            while (RNode != null)
            {
                MasterInfoRequest R = RNode.Value;
                LinkedListNode<MasterInfoRequest> Del = RNode;
                RNode = RNode.Next;

                // Give eight seconds to timeout
                if (RCPServer.Server.MilliSecs() - R.PostTime > 8000)
                {
                    Scripting.PacketWriter Pa = new Scripting.PacketWriter();
                    Pa.Write((byte)'R');
                    Pa.Write((uint)R.ZSAllocID);

                    if (R.InfoData == null)
                    {
                        Pa.Write((byte)Scripting.ActorInfoState.RequestTimedOut_MasterServer);
                    }
                    else
                    {
                        Pa.Write((byte)Scripting.ActorInfoState.Offline);
                        Pa.Write((uint)0); Pa.Write((ushort)0);
                        Pa.Write((ushort)R.InfoData.Length);
                        Pa.Write(R.InfoData);
                    }

                    if (R.Server != null && R.Server.ServerConnection != 0)
                        RCEnet.Send(R.Server.ServerConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);

                    Requests.Remove(Del);
                }
            }
        }
    }
}
