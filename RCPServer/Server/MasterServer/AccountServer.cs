using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class AccountServer
    {
        // List of all zone servers currently connected
        public static LinkedList<AccountServer> AccountServers = new LinkedList<AccountServer>();
        public static AccountServer MostFree = null;

        // Used for visual reference of machine (such as server3 or 'Jareds Computer').
        public string MachineHostName = "MUnknown";

        // IP or address of server.
        public System.Net.IPAddress Address = System.Net.IPAddress.None;

        // Connection port for internal transactions (zone servers are internal only)
        public ushort HostPort = 0;

        // Average time spent updating (over 30 means < 33 FPS).
        public int AvgUpdate = 0;

        // Connection handle for server
        public int ServerConnection = 0;

        // Link to HTTP server instance
        public HTTPModules.AdminServers.WorldServerInstance LinkedServer;

        // Get AccountServer from internal address
        public static AccountServer FromAddress(System.Net.IPAddress address, ushort port)
        {
            foreach (AccountServer Ac in AccountServers)
            {
                if (Ac.HostPort == port && Ac.Address.Equals(address))
                    return Ac;
            }

            return null;
        }
    }
}
