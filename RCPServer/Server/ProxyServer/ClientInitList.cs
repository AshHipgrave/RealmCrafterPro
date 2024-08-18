using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace RCPServer
{
    public class ClientInitList
    {
        // List to store waiting clients
        public static LinkedList<ClientInitList> WaitingClients = new LinkedList<ClientInitList>();

        public static void UpdateClientInitList()
        {
            LinkedListNode<ClientInitList> Node = WaitingClients.First;
            uint Now = Server.MilliSecs();

            while (Node != null)
            {
                LinkedListNode<ClientInitList> Delete = null;

                // Waited too long!
                if (Node.Value.JoinTime + 10000 < Now)
                {
                    // If its a waiting client, drop it
                    if (Node.Value.ConnectionID != 0)
                    {
                        PacketWriter Err = new PacketWriter();
                        Err.Write((byte)'N');
                        Err.Write((byte)'T');
                        RCEnet.Send(Node.Value.ConnectionID, MessageTypes.P_ConnectInit, Err.ToArray(), true);
                    }

                    // Delete this client
                    Delete = Node;
                }

                Node = Node.Next;
                if (Delete != null)
                    WaitingClients.Remove(Delete);
            }
        }

        // Get Init Instance from JoinID
        public static ClientInitList FromJoinID(int id)
        {
            LinkedListNode<ClientInitList> Node = WaitingClients.First;

            while (Node != null)
            {
                if (Node.Value.JoinID == id)
                    return Node.Value;

                Node = Node.Next;
            }

            return null;
        }

        // Time which the client joined, too long and we'll drop it.
        public uint JoinTime = 0;

        // Client sends us an ID, store for reference.
        public int JoinID = 0;

        // Connection (peer) handle of client.
        public int ConnectionID = 0;

        // Global Client ID (Used to identify this machine on any server during its session).
        public uint GCLID = 0;

        // AccountServer information (to direct client to on connect)
        public System.Net.IPAddress AcAddress = System.Net.IPAddress.None;
        public ushort AcPort = 0;
    }
}
