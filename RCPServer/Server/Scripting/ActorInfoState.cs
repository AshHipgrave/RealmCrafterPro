using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Enumeration of possible actor states for an ActorInfo object.
    /// </summary>
    public enum ActorInfoState
    {
        /// <summary>
        /// Player was found by an AccountServer but no ZoneServers, making him offline.
        /// </summary>
        Offline = 0,

        /// <summary>
        /// Player was found by a ZoneServer and is online.
        /// </summary>
        Online = 1,

        /// <summary>
        /// Request received 'NotFound' responses from all servers.
        /// </summary>
        NotFound = 2,
        
        /// <summary>
        /// Request took too long and timed out on the current server.
        /// 
        /// NOTE: This timeout should required investigation if it occurs.
        /// </summary>
        RequestTimedOut_ZoneServer = 3,

        /// <summary>
        /// Request took too long and timed out on the Master Server.
        /// 
        /// NOTE: This timeout should required investigation if it occurs.
        /// </summary>
        RequestTimedOut_MasterServer = 4
    }
}
