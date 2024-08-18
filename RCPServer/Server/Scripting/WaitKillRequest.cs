using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Request class for WaitKill event handling
    /// </summary>
    public class WaitKillRequest
    {
        /// <summary>
        /// Delegate for callbacks when a WaitKill event is triggered.
        /// </summary>
        /// <param name="actor">Actor the request relates to.</param>
        /// <param name="request">Handle of the request.</param>
        public delegate void RequestHandler(Actor actor, WaitKillRequest request);

        /// <summary>
        ///  DO NOT DOCUMENT
        /// </summary>
        protected ushort actorID = 65535;

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        protected ushort killCount = 1;

        /// <summary>
        /// Gets the hour which the event will be triggered.
        /// </summary>
        public ushort ActorID
        {
            get { return actorID; }
        }

        /// <summary>
        /// Gets the minute in which the event will be triggered.
        /// </summary>
        public ushort KillCount
        {
            get { return killCount; }
        }


    }
}
