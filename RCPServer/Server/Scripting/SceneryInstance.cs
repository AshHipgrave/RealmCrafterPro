using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Zone Instances version of am interactive scenery object.
    /// </summary>
    public class SceneryInstance
    {
        /// <summary>
        /// Get the master scenery object for this instance.
        /// </summary>
        public virtual Scenery Scenery { get { return null; } }

        /// <summary>
        /// Get the dictionary of script global data for this instance.
        /// </summary>
        /// <remarks>
        /// Script Globals are data sets which are specific to a given entity.
        /// 
        /// The dictionary value allows globals to be accessed directly by a string key instead
        /// of an index, which keeps script access consistant and tidy.
        /// 
        /// Value data is a byte array, this is to avoid complex string processing with numeric data
        /// and to allow stream commands to be used with it. For example, read access can use a
        /// PacketReader and write access can use a PacketWriter.
        /// </remarks>
        public virtual System.Collections.Generic.Dictionary<string, byte[]> Globals { get { return null; } }
    }
}
