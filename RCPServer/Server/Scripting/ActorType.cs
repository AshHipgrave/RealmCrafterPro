using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Actor Type Enumeration
    /// </summary>
    public enum ActorType
    {
        /// <summary>
        /// Actor is a human player.
        /// </summary>
        Player = 1,

        /// <summary>
        /// Actor is a non-player character.
        /// </summary>
        NonPlayer = 2,

        /// <summary>
        /// Actor is either human or NPC.
        /// </summary>
        Any = 3
    }
}
