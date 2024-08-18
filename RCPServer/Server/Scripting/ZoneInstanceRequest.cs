using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Delegate used for instance creation/search events
    /// </summary>
    /// <param name="e"></param>
    public delegate void ZoneInstanceRequestEventHandler(ZoneInstanceRequest e);

    /// <summary>
    /// Class used during the creation/location of a new zone instance.
    /// </summary>
    public class ZoneInstanceRequest
    {
        string areaName;
        ushort requestedID = 65535;
        ushort actualID = 65535;
        object tag = null;
        bool succeeded = false;

        /// <summary>
        /// Create an instance request.
        /// </summary>
        /// <remarks>
        /// If a specific instance ID is required then use the alternative constructor,
        /// otherwise a new zone instance will always be created.
        /// </remarks>
        /// <param name="zoneName"></param>
        public ZoneInstanceRequest(string zoneName)
        {
            areaName = zoneName;
        }

        /// <summary>
        /// Create an instance request with an optional 'requestID'.
        /// </summary>
        /// <remarks>
        /// The 'requestID' value allows a search to be placed on a specific ID before
        /// an actor is warped this. This prevents a critical error being thrown if the
        /// actor tries to warp to a zone instance which does not exist.
        /// </remarks>
        /// <param name="zoneName">Name of the zone to create an instance from.</param>
        /// <param name="requestID">ID of instance to search for or give preference.</param>
        public ZoneInstanceRequest(string zoneName, ushort requestID)
        {
            areaName = zoneName;
            requestedID = requestID;
        }

        /// <summary>
        /// HIDEFROMUSER
        /// </summary>
        public void Complete()
        {
            if (Completed != null)
                Completed.Invoke(this);
        }

        /// <summary>
        /// Name of the zone the request is associated with.
        /// </summary>
        public string ZoneName
        {
            get { return areaName; }
        }

        /// <summary>
        /// The requested ID of the instance (for searching).
        /// </summary>
        public ushort RequestedID
        {
            get { return requestedID; }
        }

        /// <summary>
        /// The true ID of the instance (on completion).
        /// </summary>
        public ushort ActualID
        {
            get { return actualID; }
            set { actualID = value; }
        }

        /// <summary>
        /// Optional 'tag' object to help identify this request.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Get whether the request passed or failed.
        /// </summary>
        public bool Succeeded
        {
            get { return succeeded; }
            set { succeeded = value; }
        }

        /// <summary>
        /// Event called when the request has been completed.
        /// </summary>
        public event ZoneInstanceRequestEventHandler Completed;
    }
}
