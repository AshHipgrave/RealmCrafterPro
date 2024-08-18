using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ZoneServer
    {
        // List of all zone servers currently connected
        public static LinkedList<ZoneServer> ZoneServers = new LinkedList<ZoneServer>();

        // Most free zone server (performing the most well).
        public static ZoneServer MostFree = null;

        // Connection handle for server
        public int ServerConnection = 0;

        // Used for visual reference of machine (such as server3 or 'Jareds Computer').
        public string MachineHostName = "MUnknown";

        // IP or address of server.
        public System.Net.IPAddress Address = System.Net.IPAddress.None;

        // Connection port for internal transactions (zone servers are internal only).
        public ushort HostPort = 0;

        // Average time spent updating (over 30 means < 33 FPS).
        public int AvgUpdate = 0;

        // Number of zone instances being handles
        public int InstanceCount = 0;

        // Number of people connected over all zones
        public int ClientCount = 0;

        // Link to an HTTP server
        public HTTPModules.AdminServers.WorldServerInstance LinkedServer;

        // Get ZoneServer from internal address
        public static ZoneServer FromAddress(System.Net.IPAddress address, ushort port)
        {
            foreach (ZoneServer Zs in ZoneServers)
            {
                if (Zs.HostPort == port && Zs.Address.Equals(address))
                    return Zs;
            }

            return null;
        }

        // Get ZoneServer from internal connection
        public static ZoneServer FromConnection(int connection)
        {
            foreach (ZoneServer Zs in ZoneServers)
            {
                if(Zs.ServerConnection == connection)
                    return Zs;
            }

            return null;
        }
    }
}
