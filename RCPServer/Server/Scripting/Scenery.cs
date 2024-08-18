using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Math;

namespace Scripting
{
    /// <summary>
    /// An interactive scenery object.
    /// </summary>
    public class Scenery
    {
        /// <summary>
        /// Gets the position of the scenery object.
        /// </summary>
        public virtual SectorVector Position
        {
            get { return new SectorVector(); }
        }

        /// <summary>
        /// Gets the Media Manager Mesh ID of the object.
        /// </summary>
        public virtual ushort MeshID
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the script executed when the given scenery is selected.
        /// </summary>
        public virtual string ScriptName
        {
            get { return ""; }
        }
    }
}
