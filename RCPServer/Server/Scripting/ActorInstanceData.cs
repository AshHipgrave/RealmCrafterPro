using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Data storage class for saving and loading actors
    /// </summary>
    public class ActorInstanceData
    {
        byte[] serializedAI;
        byte[] serializedScripts;

        /// <summary>
        /// Default Constructor for data storage
        /// </summary>
        /// <param name="actorInstanceData">Byte array of serialized actor data.</param>
        /// <param name="scriptsData">Byte array of serialized script (persistant) data.</param>
        public ActorInstanceData(byte[] actorInstanceData, byte[] scriptsData)
        {
            serializedAI = actorInstanceData;
            serializedScripts = scriptsData;
        }

        /// <summary>
        /// Gets the serialized actor instance.
        /// </summary>
        public byte[] SerializedAI
        {
            get { return serializedAI; }
        }

        /// <summary>
        /// Gets the serialized persistant script data.
        /// </summary>
        public byte[] SerializedScripts
        {
            get { return serializedScripts; }
        }
    }
}
