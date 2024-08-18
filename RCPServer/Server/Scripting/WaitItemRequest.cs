using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Request class for WaitItem event handling
    /// </summary>
    public class WaitItemRequest
    {
        /// <summary>
        /// Delegate for callbacks when a WaitItem event is triggered.
        /// </summary>
        /// <param name="actor">Actor the request relates to.</param>
        /// <param name="request">Handle of the request.</param>
        public delegate void RequestHandler(Actor actor, WaitItemRequest request);

        /// <summary>
        ///  DO NOT DOCUMENT
        /// </summary>
        protected string itemName = "";

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        protected ushort itemCount = 1;

        /// <summary>
        /// Gets the hour which the event will be triggered.
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
        }

        /// <summary>
        /// Gets the minute in which the event will be triggered.
        /// </summary>
        public ushort ItemCount
        {
            get { return itemCount; }
        }


    }
}
