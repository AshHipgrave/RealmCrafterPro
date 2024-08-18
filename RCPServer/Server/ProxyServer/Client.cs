using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace RCPServer
{
    // Represents a client machine currently accounting/ingame
    public class Client
    {
        // List of clients
        public static LinkedList<Client> Clients = new LinkedList<Client>();

        // Get a client handle from its remote peer
        public static Client FromPeer(int connection)
        {
            LinkedListNode<Client> Node = Clients.First;
            while (Node != null)
            {
                if (Node.Value.Connection == connection)
                    return Node.Value;

                Node = Node.Next;
            }

            return null;
        }

        // Get a client handle from its Global ID
        public static Client FromGCLID(uint gclid)
        {
            LinkedListNode<Client> Node = Clients.First;
            while (Node != null)
            {
                if (Node.Value.GCLID == gclid)
                    return Node.Value;

                Node = Node.Next;
            }

            return null;
        }

        // Disconnect all clients connected to a particular server
        public static void RemoveByServer(ClusterServerNode node)
        {
            LinkedListNode<Client> Node = Clients.First;
            LinkedListNode<Client> Delete = null;

            while (Node != null)
            {
                Delete = null;

                if (Node.Value.CurrentServer == node)
                {
                    RCEnet.Disconnect(Node.Value.Connection);
                    Node.Value.CurrentServer = null;
                    Delete = Node;
                }

                Node = Node.Next;

                if (Delete != null)
                    Clients.Remove(Delete);
            }
        }

        // Remote Peer
        public int Connection = 0;

        // Global Client ID
        public uint GCLID = 0;

        // Disconnection timer to drop a client
        public uint DisconnectedTime = 0;

        // Current server connection
        public ClusterServerNode CurrentServer = null;

        public Stopwatch PingTimer;
        public bool IsWaitingForPong;

        public Client()
        {
            PingTimer = new Stopwatch();
            PingTimer.Reset();
            PingTimer.Start();
           
        }

        public void ResetPing()
        {
            PingTimer.Reset();
            PingTimer.Start();
            IsWaitingForPong = false;
        }

        public bool IsTimedOut { get { return PingTimer.ElapsedMilliseconds > Timeout && IsWaitingForPong; } }

        public int Timeout = 15000;
    }
}
