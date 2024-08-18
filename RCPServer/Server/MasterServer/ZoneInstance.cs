using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace RCPServer
{
    public class ZoneInstance
    {
        // List of active zone instances
        public static LinkedList<ZoneInstance> ZoneInstances = new LinkedList<ZoneInstance>();

        // Parent Instance
        public WorldZone InstanceOf;

        // Instance Index (or AllocID)
        public int ID = 0;

        // Server which is currently processing this instance
        // If this is null, it means that the instance must
        // immediatly be allocated a host.
        public ZoneServer ServerHost;

        // Number of real world clients inside the instance
        public int ConnectedClients = 0;

        // Constructor
        public ZoneInstance(int id, WorldZone parent, ZoneServer host)
        {
            ID = id;
            InstanceOf = parent;
            ServerHost = host;

            ZoneInstances.AddLast(this);
            lock (HTTPModules.AdminServers.ZoneInstance.Instances)
            {
                HTTPModules.AdminServers.ZoneInstance Zi = new HTTPModules.AdminServers.ZoneInstance(parent.Name, id, 0, false);
                HTTPModules.AdminServers.ZoneInstance.Instances.Add(Zi);
            }
        }

        protected class ReBalanceInstance
        {
            public List<ZoneInstance> Instances = new List<ZoneInstance>();
            public ZoneServer Server;
        }

        // Find an existing instance
        public static ZoneInstance Find(string name, int id)
        {
            foreach (ZoneInstance Zi in ZoneInstances)
            {
                if (Zi.ID == id && Zi.InstanceOf.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return Zi;
                }
            }

            return null;
        }

        // Find an existing instance or create a new one
        public static ZoneInstance FindOrCreate(string name, int id)
        {
            WorldZone Zone = WorldZone.Find(name);
            if(Zone == null)
                return null;

            List<ZoneInstance> CopyToList = new List<ZoneInstance>();

            // Find it
            foreach (ZoneInstance Instance in ZoneInstances)
            {
                if (Instance.InstanceOf.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    if(Instance.ID == id)
                        return Instance;

                    if (id == 65535)
                        CopyToList.Add(Instance);
                }
            }

            // If the ID is 65535, then we need to create a new ID;
            for (int i = 0; i < 65535; ++i)
            {
                bool Found = false;

                for (int f = 0; f < CopyToList.Count; ++f)
                {
                    if (CopyToList[f].ID == i)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    id = i;
                    break;
                }
            }

            // Not found, create it
            ZoneInstance Inst = new ZoneInstance(id, Zone, ZoneServer.MostFree);

            // Tell the zone server to create it and manage it
            PacketWriter Pa = new PacketWriter();
            Pa.Write((byte)'A');
            Pa.Write((byte)Zone.Name.Length);
            Pa.Write(Zone.Name, false);
            Pa.Write((ushort)id);

            byte[] Data = WorldZone.Load(Inst);
            Pa.Write(Data.Length);
            Pa.Write(Data, 0);

            if(ZoneServer.MostFree != null)
                RCEnet.Send(ZoneServer.MostFree.ServerConnection, MessageTypes.P_ServerList, Pa.ToArray(), true);

            PacketWriter ProxyWriter = new PacketWriter();
            ProxyWriter.Write((byte)'U');

            ProxyWriter.Write((byte)Zone.Name.Length);
            ProxyWriter.Write(Zone.Name, false);
            ProxyWriter.Write((ushort)id);

            if (ZoneServer.MostFree != null)
            {
                ProxyWriter.Write(Inst.ServerHost.Address.GetAddressBytes(), 0);
                ProxyWriter.Write((ushort)Inst.ServerHost.HostPort);
            }
            else
            {
                ProxyWriter.Write(System.Net.IPAddress.None.GetAddressBytes(), 0);
                ProxyWriter.Write((ushort)0);
            }

            byte[] ProxyWriterA = ProxyWriter.ToArray();
            foreach (ProxyServer Ps in ProxyServer.ProxyServers)
            {
                if (Ps.ServerConnection != 0)
                    RCEnet.Send(Ps.ServerConnection, MessageTypes.P_ServerList, ProxyWriterA, true);
            }

            return Inst;
        }

        // Rebalance zones across network (Should kick out existing players)
        public static void MasterReBalance()
        {
            if (ZoneServer.ZoneServers.Count == 0)
                return;
            
            // This is a very simple spread function since we don't have control over 'unique' or
            // high demand zones.
            List<ReBalanceInstance> Servers = new List<ReBalanceInstance>();

            // Build a list of servers
            foreach (ZoneServer Si in ZoneServer.ZoneServers)
            {
                ReBalanceInstance RI = new ReBalanceInstance();
                RI.Server = Si;

                Servers.Add(RI);
            }

            // Assign instances to server one by one (card dealer style)
            int ServerIndex = 0;
            foreach (ZoneInstance Zi in ZoneInstances)
            {
                Servers[ServerIndex].Instances.Add(Zi);
                ++ServerIndex;
                if (ServerIndex == Servers.Count)
                    ServerIndex = 0;
            }

            // Notify servers of new structure
            PacketWriter ProxyWriter = new PacketWriter();

            foreach (ReBalanceInstance Ri in Servers)
            {
                PacketWriter ZoneWriter = new PacketWriter();

                foreach (ZoneInstance Zi in ZoneInstances)
                {
                    bool HasServer = Ri.Instances.Contains(Zi);
                    bool IsServer = Zi.ServerHost == Ri.Server;

                    if (HasServer && !IsServer)
                        ZoneWriter.Write((byte)'A');
                    else if(!HasServer && IsServer)
                        ZoneWriter.Write((byte)'U');

                    ZoneWriter.Write((byte)Zi.InstanceOf.Name.Length);
                    ZoneWriter.Write(Zi.InstanceOf.Name, false);
                    ZoneWriter.Write((ushort)Zi.ID);
                    ZoneWriter.Write(0);
                }

                foreach (ZoneInstance Zi in Ri.Instances)
                {
                    if (Zi.ServerHost != Ri.Server)
                    {
                        ProxyWriter.Write((byte)'U');

                        ProxyWriter.Write((byte)Zi.InstanceOf.Name.Length);
                        ProxyWriter.Write(Zi.InstanceOf.Name, false);
                        ProxyWriter.Write((ushort)Zi.ID);

                        ProxyWriter.Write(Ri.Server.Address.GetAddressBytes(), 0);
                        ProxyWriter.Write((ushort)Ri.Server.HostPort);
                    }
                }

                if(Ri.Server.ServerConnection != 0)
                    RCEnet.Send(Ri.Server.ServerConnection, MessageTypes.P_ServerList, ZoneWriter.ToArray(), true);
            }

            byte[] ProxyWriterA = ProxyWriter.ToArray();
            foreach (ProxyServer Ps in ProxyServer.ProxyServers)
            {
                if(Ps.ServerConnection != 0)
                    RCEnet.Send(Ps.ServerConnection, MessageTypes.P_ServerList, ProxyWriterA, true);
            }

            foreach (ReBalanceInstance Ri in Servers)
            {
                foreach (ZoneInstance Zi in Ri.Instances)
                {
                    Zi.ServerHost = Ri.Server;
                }
            }

            

        }

    }
}
