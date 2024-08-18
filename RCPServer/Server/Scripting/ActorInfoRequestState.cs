using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// State to search for in ActorInfoRequest
    /// </summary>
    public enum ActorInfoRequestState
    {
        /// <summary>
        /// Search for an online player.
        /// </summary>
        Online = 0,

        /// <summary>
        /// Search for an offline player.
        /// </summary>
        Offline = 1,

        /// <summary>
        /// Search for a player both online and offline.
        /// </summary>
        All = 2
    }
}
