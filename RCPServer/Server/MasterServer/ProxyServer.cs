using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class ProxyServer
    {
        // List of all proxy servers currently connected
        public static LinkedList<ProxyServer> ProxyServers = new LinkedList<ProxyServer>();
        public static ProxyServer MostFree = null;
        public static ProxyServer MasterProxy = null;

        // Get ProxyServer from connection ID
        public static ProxyServer FromConnection(int connectionID)
        {
            foreach (ProxyServer Ps in ProxyServers)
            {
                if (Ps.ServerConnection == connectionID)
                    return Ps;
            }

            return null;
        }

        // Get ProxyServer from internal address
        public static ProxyServer FromAddress(System.Net.IPAddress address, ushort port)
        {
            foreach (ProxyServer Ps in ProxyServers)
            {
                if (Ps.InternalHostPort == port && Ps.InternalAddress.Equals(address))
                    return Ps;
            }

            return null;
        }

        // Connection handle for server
        public int ServerConnection = 0;

        // Used for visual reference of machine (such as server3 or 'Jareds Computer').
        public string MachineHostName = "MUnknown";

        // IP or addresses of server.
        public System.Net.IPAddress InternalAddress = System.Net.IPAddress.None;
        public System.Net.IPAddress ExternalAddress = System.Net.IPAddress.None;

        // Connection ports for internal transactions (zone servers are internal only).
        public ushort InternalHostPort = 0;
        public ushort ExternalHostPort = 0;

        // Average time spent updating (over 30 means < 33 FPS).
        public int AvgUpdate = 0;

        // Number of people connected over all zones
        public int ClientCount = 0;

        // Hourly Averages of bandwidth
        public int[] HourlySetsTX = new int[24];
        public int[] HourlySetsRX = new int[24];
        public int[] HourlyClients = new int[24];

        // Link to HTTP server instance
        public HTTPModules.AdminServers.WorldServerInstance LinkedServer;
    }
}
