using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Request class for WaitTime event handling
    /// </summary>
    public class WaitTimeRequest
    {
        /// <summary>
        /// Delegate for callbacks when a WaitTime event is triggered.
        /// </summary>
        /// <param name="actor">Actor the request relates to.</param>
        /// <param name="request">Handle of the request.</param>
        public delegate void RequestHandler(Actor actor, WaitTimeRequest request);

        /// <summary>
        ///  DO NOT DOCUMENT
        /// </summary>
        protected int hour = 0;

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        protected int minute = 0;

        /// <summary>
        /// Gets the hour which the event will be triggered.
        /// </summary>
        public int Hour
        {
            get { return hour; }
        }

        /// <summary>
        /// Gets the minute in which the event will be triggered.
        /// </summary>
        public int Minute
        {
            get { return minute; }
        }


    }
}
