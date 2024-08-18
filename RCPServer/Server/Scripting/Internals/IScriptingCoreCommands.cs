using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Internals
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScriptingCoreCommands
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raceName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        uint ActorID(string raceName, string className);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="zoneName"></param>
        /// <param name="position"></param>
        /// <param name="interactScript"></param>
        /// <param name="deathScript"></param>
        /// <param name="instanceIndex"></param>
        /// <returns></returns>
        Actor Spawn(uint id, string zoneName, global::Scripting.Math.SectorVector position, string script, int instanceIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factionName"></param>
        /// <param name="otherFactionName"></param>
        /// <returns></returns>
        int DefaultFactionRating(string factionName, string otherFactionName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        void PostActorInfoRequest(ActorInfoRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="message"></param>
        /// <param name="destinationChannel"></param>
        /// <param name="rangeInSectors"></param>
        void DispatchMessage(Actor actor, string message, byte destinationChannel, int rangeInSectors);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool PostZoneInstanceRequest(ZoneInstanceRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        bool ZoneOutdoors(string zoneName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="amount"></param>
        /// <param name="zoneName"></param>
        /// <param name="position"></param>
        /// <param name="instanceID"></param>
        void SpawnItem(string itemName, int amount, string zoneName, global::Scripting.Math.SectorVector position, int instanceID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        byte[] GetSuperGlobal(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetSuperGlobal(string name, byte[] value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="emitterName"></param>
        /// <param name="textureID"></param>
        /// <param name="timeLength"></param>
        /// <param name="offset"></param>
        void CreateStaticEmitter(string zoneName, string emitterName, int textureID, int timeLength, global::Scripting.Math.SectorVector offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}
