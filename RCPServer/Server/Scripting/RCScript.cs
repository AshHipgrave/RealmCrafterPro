using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Core Scripting Class
    /// </summary>
    public class RCScript
    {
        /// <summary>
        /// Very important internal variable. It calls the mirrored commands in the main assembly which do not exist in the scripting library.
        /// </summary>
        protected static Internals.IScriptingCoreCommands Commands;


        /// <summary>
        /// Post a request for actor information.
        /// </summary>
        /// <param name="request">Handle to request object containing search parameters and callback.</param>
        public static void PostActorInfoRequest(ActorInfoRequest request)
        {
            Commands.PostActorInfoRequest(request);
        }

        /// <summary>
        /// Broadcast a message to a particular channel from the given actor.
        /// </summary>
        /// <param name="actor">Handle of the actor to broadcast from.</param>
        /// <param name="message">Message text.</param>
        /// <param name="destinationChannel">Unique ID of Tab to write to</param>
        /// <param name="rangeInSectors">Number of sectors which this message will reach.</param>
        public static void DispatchMessage(Actor actor, string message, byte destinationChannel, int rangeInSectors)
        {
            Commands.DispatchMessage(actor, message, destinationChannel, rangeInSectors);
        }

        /// <summary>
        /// Set the command object.
        /// </summary>
        /// <remarks>
        /// For the host application only. Do not touch this.
        /// </remarks>
        /// <param name="c"></param>
        public static void SetCommands(Internals.IScriptingCoreCommands c)
        {
            Commands = c;
        }

        /// <summary>
        /// Posts a request to create or find a zone instance.
        /// </summary>
        /// <remarks>
        /// A zone instance might exist on any server, or not exist at all. A Zone Instance Request
        /// will dispatch a find-or-create message to the master server.
        /// 
        /// The Completion event is invoked with the result when it becomes available.
        /// </remarks>
        /// <param name="request"></param>
        public static bool PostZoneInstanceRequest(ZoneInstanceRequest request)
        {
            return Commands.PostZoneInstanceRequest(request);
        }

        /// <summary>
        /// Get whether a zone it outdoors.
        /// </summary>
        /// <param name="zoneName">Name of the zone to find.</param>
        /// <returns>True if the zone is outdoors.</returns>
        public static bool ZoneOutdoors(string zoneName)
        {
            return Commands.ZoneOutdoors(zoneName);
        }

        /// <summary>
        /// Spawn a dropped item in a zone.
        /// </summary>
        /// <param name="itemName">Name of the item to spawn.</param>
        /// <param name="amount">Amount of items in the 'container'.</param>
        /// <param name="zoneName">Name of the zone in which to spawn the item.</param>
        /// <param name="position">Location of the item container.</param>
        public static void SpawnItem(string itemName, int amount, string zoneName, global::Scripting.Math.SectorVector position)
        {
            SpawnItem(itemName, amount, zoneName, position, 0);
        }

        /// <summary>
        /// Spawn a dropped item in a zone.
        /// </summary>
        /// <param name="itemName">Name of the item to spawn.</param>
        /// <param name="amount">Amount of items in the 'container'.</param>
        /// <param name="zoneName">Name of the zone in which to spawn the item.</param>
        /// <param name="position">Location of the item container.</param>
        /// <param name="instanceID">Instance ID of the specified zone.</param>
        public static void SpawnItem(string itemName, int amount, string zoneName, global::Scripting.Math.SectorVector position, int instanceID)
        {
            Commands.SpawnItem(itemName, amount, zoneName, position, instanceID);
        }

        /// <summary>
        /// Write a message to the system log.
        /// </summary>
        /// <remarks>
        /// Saves a message to the server log as well as the console output.
        /// </remarks>
        /// <param name="message">Message to write.</param>
        public static void Log(string message)
        {
            Commands.Log(message);
        }

        /// <summary>
        /// Get the value of a super global.
        /// </summary>
        /// <remarks>
        /// A super global is a variable which is accessible by all scripts at any time. The data is saved when the
        /// server shuts down, so they can store persistant data.<br />
        /// <br />
        /// <b>Note:</b> Super Globals use string based names in order to be found. This can cause a slowdown when there
        /// are many global variables present. There is no hardcoded limit, so use these variables carefully.
        /// </remarks>
        /// <param name="name">Name of the super global to find.</param>
        public static byte[] GetSuperGlobal(string name)
        {
            return Commands.GetSuperGlobal(name);
        }

        /// <summary>
        /// Set the value of a super global.
        /// </summary>
        /// <remarks>
        /// A super global is a variable which is accessible by all scripts at any time. The data is saved when the
        /// server shuts down, so they can store persistant data.<br />
        /// <br />
        /// <b>Note:</b> Super Globals use string based names in order to be found. This can cause a slowdown when there
        /// are many global variables present. There is no hardcoded limit, so use these variables carefully.
        /// </remarks>
        /// <param name="name">Name of the super global to set.</param>
        /// <param name="value">Byte Array of data to set.</param>
        public static void SetSuperGlobal(string name, byte[] value)
        {
            Commands.SetSuperGlobal(name, value);
        }

        /// <summary>
        /// Execute a script immediately.
        /// </summary>
        /// <remarks>
        /// This method will instance a new script and execute its' entrypoint. The new script will block the current
        /// script until it has completed. This only applies to its root method rather than any callbacks it registers.<br />
        /// <br />
        /// In most cases, DelayedExecute is a safer option to choose.
        /// </remarks>
        /// <param name="name">Name of the script object to create.</param>
        /// <param name="entrypoint">Entry method to the script.</param>
        /// <param name="arguments">Array of arguments to be passed to the method</param>
        /// <param name="tag">Initial tag for the script (Crucial if it depends upon an actor).</param>
        /// <param name="tag2">Secondary tag for the script (Crucial if it depends upon a context actor).</param>
        public static bool Execute(string name, string entrypoint, object[] arguments, object tag, object tag2)
        {
            return ScriptManager.Execute(name, entrypoint, arguments, tag, tag2);
        }

        /// <summary>
        /// Execute a script in a deferred context.
        /// </summary>
        /// <remarks>
        /// If a script needs to be executed without an immediate effect, it can be run after the current script
        /// has finished. Most script executions are deferred in order to prevent the server from locking during
        /// important processes.
        /// </remarks>
        /// <param name="name">Name of the script object to create.</param>
        /// <param name="entrypoint">Entry method to the script.</param>
        /// <param name="arguments">Array of arguments to be passed to the method</param>
        /// <param name="tag">Initial tag for the script (Crucial if it depends upon an actor).</param>
        /// <param name="tag2">Secondary tag for the script (Crucial if it depends upon a context actor).</param>
        public static bool DelayedExecute(string name, string entrypoint, object[] arguments, object tag, object tag2)
        {
            return ScriptManager.DelayedExecute(name, entrypoint, arguments, tag, tag2);
        }

        /// <summary>
        /// Create a particle emitter somewhere in the zone.
        /// </summary>
        /// <param name="zoneName">Name of the zone to spawn the emitter inside.</param>
        /// <param name="emitterName">Name of the emitter to use.</param>
        /// <param name="textureID">MediaID of the texture to apply to the emitter.</param>
        /// <param name="timeLength">Length of time the emitter can exist for.</param>
        /// <param name="offset">Location of the new emitter inside the zone.</param>
        public static void CreateStaticEmitter(string zoneName, string emitterName, int textureID, int timeLength, global::Scripting.Math.SectorVector offset)
        {
            Commands.CreateStaticEmitter(zoneName, emitterName, textureID, timeLength, offset);
        }
        
        // Persistant(Flag)
    }
}
