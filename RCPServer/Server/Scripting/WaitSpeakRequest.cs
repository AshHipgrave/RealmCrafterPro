using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Request class for WaitSpeak event handling
    /// </summary>
    public class WaitSpeakRequest
    {
        /// <summary>
        /// Delegate for callbacks when a WaitSpeak event is triggered.
        /// </summary>
        /// <param name="actor">Actor the request relates to.</param>
        /// <param name="context">Handle of actor which is being spoken too.</param>
        public delegate void RequestHandler(Actor actor, Actor context);

        /// <summary>
        ///  DO NOT DOCUMENT
        /// </summary>
        protected string zoneName = "";

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        protected string actorName = "";

        /// <summary>
        /// Gets the hour which the event will be triggered.
        /// </summary>
        public string ZoneName
        {
            get { return zoneName; }
        }

        /// <summary>
        /// Gets the minute in which the event will be triggered.
        /// </summary>
        public string ActorName
        {
            get { return actorName; }
        }


    }
}
