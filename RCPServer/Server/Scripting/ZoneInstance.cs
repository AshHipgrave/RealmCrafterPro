using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Zone Instance Class.
    /// </summary>
    public class ZoneInstance
    {
        /// <summary>
        /// Get the name of the zone.
        /// </summary>
        public virtual string Name { get { return ""; } }

        /// <summary>
        /// Get the ID of this instance.
        /// </summary>
        /// <remarks>An ID of 0 means the base instance of the zone.</remarks>
        public virtual int InstanceIndex { get { return 0; } }

        /// <summary>
        /// Get a list of actors within the given sector 'box'.
        /// </summary>
        /// <param name="actorType">Type of actor to retreive.</param>
        /// <param name="minSectorX">Lower, inclusive, X sector.</param>
        /// <param name="minSectorZ">Lower, inclusive, Z sector.</param>
        /// <param name="maxSectorX">Upper, exclusive, X sector.</param>
        /// <param name="maxSectorZ">Upper, exclusive, Z sector.</param>
        /// <returns>List of actors found.</returns>
        public virtual LinkedList<Actor> GetActors(ActorType actorType, ushort minSectorX, ushort minSectorZ, ushort maxSectorX, ushort maxSectorZ) { return null; }
    }
}
