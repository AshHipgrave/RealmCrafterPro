using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ProxyZoneInstance
    {
        // List of active zone instances
        public static LinkedList<ProxyZoneInstance> ZoneInstances = new LinkedList<ProxyZoneInstance>();

        // Find or create a zone instance with specific details
        public static ProxyZoneInstance FindOrCreate(string name, ushort id)
        {
            LinkedListNode<ProxyZoneInstance> Node = ZoneInstances.First;

            while (Node != null)
            {
                if (Node.Value.Name.Equals(name) && Node.Value.ID == id)
                    return Node.Value;

                Node = Node.Next;
            }

            return new ProxyZoneInstance(id, name);
        }

        // Find a zone instance with specific details
        public static ProxyZoneInstance Find(string name, ushort id)
        {
            LinkedListNode<ProxyZoneInstance> Node = ZoneInstances.First;

            while (Node != null)
            {
                if (Node.Value.Name.Equals(name) && Node.Value.ID == id)
                    return Node.Value;

                Node = Node.Next;
            }

            return null;
        }

        // Parent Instance
        public string Name = "";

        // Instance Index (or AllocID)
        public int ID = 0;

        // Server which is currently processing this instance
        // If this is null, it means that the instance must
        // immediatly be allocated a host.
        public ClusterServerNode ServerHost;

        // Constructor
        public ProxyZoneInstance(int id, string name)
        {
            ID = id;
            Name = name;

            ZoneInstances.AddLast(this);
        }


    }
}
