using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Information container returned from a FindActorRequest.
    /// </summary>
    public class ActorInfo
    {
        #region Properties
        /// <summary>
        /// Gets the state of the actor referred to in the request.
        /// </summary>
        /// <remarks>
        /// This property will return the outcome of the request and should always be checked before using this object further.
        /// 
        /// If the original request used the ActorInfoRequestState.Offline option then the best outcome of this request will be
        /// ActorInfoState.Offline. This is because the request will only be dispatched to an Account Server rather than other
        /// servers in the cluster. When this state appears, it is possible to evaluate the AccountName, Banned and GM properties.
        /// 
        /// If a NotFound state is returned then the Name property will contain the name sent in the original request.
        /// 
        /// Note: All properties contained within the ActorInfo class (except State and Name) will be invalid if any state other
        /// than ActorInfoState.Online is returned, this is because appearance information can only be obtained from players who
        /// are online.
        /// 
        /// Note: The timeout states mean that the request could not be completed because not all of the servers who were sent the
        /// request replied. A timeout should never occur during the lifetime of a game since checks are relatively fast, though
        /// if one did occur then the an investigation into the current load placed on the cluster should be launched in order to
        /// prevent further request timeouts.
        /// </remarks>
        public virtual ActorInfoState State { get { return ActorInfoState.NotFound; } }

        /// <summary>
        /// Gets the name of the actor in the request.
        /// </summary>
        public virtual string Name { get { return ""; } }

        /// <summary>
        /// Gets the account name which owns the requested actor.
        /// </summary>
        public virtual string AccountName { get { return ""; } }

        /// <summary>
        /// Gets the class name of the actor.
        /// </summary>
        public virtual string Class { get { return ""; } }

        /// <summary>
        /// Gets the race of the actor.
        /// </summary>
        public virtual string Race { get { return ""; } }

        /// <summary>
        /// Gets the name of the zone which the actor is currently in.
        /// </summary>
        public virtual string ZoneName { get { return ""; } }

        /// <summary>
        /// Gets whether the account owned by the actor is banned.
        /// </summary>
        public virtual bool Banned { get { return false; } }

        /// <summary>
        /// Gets whether the account owned by this actor is a BM.
        /// </summary>
        public virtual bool GM { get { return false; } }

        /// <summary>
        /// Gets the Base Actor ID of the actor (For appearance purposes).
        /// </summary>
        public virtual uint BaseActorID { get { return 0; } }

        /// <summary>
        /// Gets the beard gubbin index of the actor (For appearance purposes).
        /// </summary>
        public virtual int Beard { get { return 0; } }

        /// <summary>
        /// Gets the clothing texture index of the actor (For appearance purposes).
        /// </summary>
        public virtual int Clothes { get { return 0; } }

        /// <summary>
        /// Gets the face texture index of the actor (For appearance purposes).
        /// </summary>
        public virtual int Face { get { return 0; } }

        /// <summary>
        /// Gets the hair gubbin index of the actor (For appearance purposes).
        /// </summary>
        public virtual int Hair { get { return 0; } }

        /// <summary>
        /// Gets the gender of the actor (1 = Male, 2 = Female, 3 (returned) = Has no gender).
        /// </summary>
        public virtual int Gender { get { return 0; } }

        /// <summary>
        /// Gets the handle of this actor if it is on the same server (is null otherwise).
        /// </summary>
        public virtual Actor Handle { get { return null; } }

        /// <summary>
        /// Get the position of this actor.
        /// </summary>
        public virtual global::Scripting.Math.SectorVector Position { get { return global::Scripting.Math.SectorVector.Zero; } }

        #endregion
        #region Methods

        /// <summary>
        /// Write a message in the chat area.
        /// </summary>
        /// <param name="tabID">Unique Tab ID to write to.</param>
        /// <param name="message">Message to write.</param>
        public virtual void Output(byte tabID, string message) { }

        /// <summary>
        /// Write a message in the chat area.
        /// </summary>
        /// <param name="tabID">Unique Tab ID to write to.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="color">Color of the message.</param>
        public virtual void Output(byte tabID, string message, System.Drawing.Color color) { }

        /// <summary>
        /// Move the actor to a new zone.
        /// </summary>
        /// <remarks>
        /// Warping will cause a client to load a new zone if the zoneName argument is different to the zone they are
        /// currently in.
        /// </remarks>
        /// <param name="zoneName">Name of the zone to move to.</param>
        /// <param name="portalName">Name of the portal to spawn the actor at.</param>
        public virtual void Warp(string zoneName, string portalName) {  }

        /// <summary>
        /// Move the actor to a new zone.
        /// </summary>
        /// <remarks>
        /// Warping will cause a client to load a new zone if the zoneName argument is different to the zone they are
        /// currently in.
        /// </remarks>
        /// <param name="zoneName">Name of the zone to move to.</param>
        /// <param name="portalName">Name of the portal to spawn the actor at.</param>
        /// <param name="instanceNumber">The instance to place the actor in.</param>
        public virtual void Warp(string zoneName, string portalName, int instanceNumber) {  }

        /// <summary>
        /// Kill this actor.
        /// </summary>
        public virtual void Kill() { }

        /// <summary>
        /// Execute a script 
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="methodName"></param>
        /// <param name="userData"></param>
        public virtual void ExecuteScript(string scriptName, string methodName, byte[] userData) { }

        #endregion

    }
}
