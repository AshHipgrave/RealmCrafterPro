using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    /// <summary>
    /// Represents an internal list of cluster nodes which a proxy must
    /// remain connected to.
    /// </summary>
    public class ClusterServerNode
    {
        // Internal list
        public static LinkedList<ClusterServerNode> Nodes = new LinkedList<ClusterServerNode>();

        // Update cluster list (reconnections and such)
        public static void UpdateNodes()
        {
            LinkedListNode<ClusterServerNode> Node = Nodes.First;
            while (Node != null)
            {
                LinkedListNode<ClusterServerNode> DelNode = null;

                if (Node.Value.Shutdown)
                {
                    if (Node.Value.LastDisconnect + 10000 < Server.MilliSecs())
                    {
                        RCEnet.Disconnect(Node.Value.Connection);
                        Node.Value.Connection = 0;

                        DelNode = Node;

                        Client.RemoveByServer(Node.Value);

                        Log.WriteLine("Master: Disconnect from: " + Node.Value.Address.ToString() + ": " + Node.Value.Port.ToString() + "/" + Node.Value.MachineName, ConsoleColor.Yellow);
                    }
                }
                else if(Node.Value.Connection == 0 && Node.Value.LastDisconnect + 10000 < Server.MilliSecs())
                {
                    // Attempt to reconnect
                    Log.Write("Attempting to reconnect to " + (Node.Value.IsZoneServer ? "Zone Server" : "Account Server") + ": " + Node.Value.Address.ToString() + ":" + Node.Value.Port.ToString() + "/" + Node.Value.MachineName + "... ");

                    Node.Value.Connection = RCEnet.Connect(Node.Value.Address.ToString(), Node.Value.Port);
                    if (Node.Value.Connection <= 0 && Node.Value.Connection > -6)
                    {
                        Log.WriteLine(string.Format("Could not connect: '{0}'", new string[] { RCEnet.ConnectionErrors[Math.Abs(Node.Value.Connection)] }), ConsoleColor.Red);
                        Node.Value.Connection = 0;
                        Node.Value.LastDisconnect = Server.MilliSecs();
                    }
                    else
                    {
                        Log.WriteOK();
                        ClusterServerNode.Nodes.AddLast(Node);
                    }
                }

                Node = Node.Next;
                if (DelNode != null)
                    Nodes.Remove(DelNode);
            }
        }

        public static ClusterServerNode Find(System.Net.IPAddress Address, ushort Port)
        {
            LinkedListNode<ClusterServerNode> Node = Nodes.First;
            while (Node != null)
            {
                if (Node.Value.Address.Equals(Address) && Node.Value.Port == Port)
                {
                    return Node.Value;
                }

                Node = Node.Next;
            }

            return null;
        }

        // Find server by peer
        public static ClusterServerNode FromPeer(int peer)
        {
            LinkedListNode<ClusterServerNode> Node = Nodes.First;
            while (Node != null)
            {
                if(Node.Value.Connection == peer)
                    return Node.Value;

                Node = Node.Next;
            }

            return null;
        }

        // Handle to the RCEnet connection
        public int Connection = 0;

        // Last disconnection time (we wait 10 seconds for a reconnect)
        public uint LastDisconnect = 0;

        // IP Address
        public System.Net.IPAddress Address = System.Net.IPAddress.None;

        // Port
        public ushort Port = 0;

        // Name
        public string MachineName = "";

        // Is a zone server (false = account server)
        public bool IsZoneServer = false;

        // Has this server been asked to shutdown?
        public bool Shutdown = false;

        
    }
}
