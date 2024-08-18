using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Request class to find another player.
    /// </summary>
    public class ActorInfoRequest
    {
        /// <summary>
        /// Delegate for completion for an ActorInfoRequest.
        /// </summary>
        /// <param name="actorInfo">Handle to actor information container.</param>
        public delegate void RequestHandler(ActorInfo actorInfo);

        static uint allocIDs = 0;

        ActorInfoRequestState requestState = ActorInfoRequestState.All;

        /// <summary>
        /// 
        /// </summary>
        protected string actorName = "";
        uint postTime = 0;

        /// <summary>
        /// 
        /// </summary>
        protected uint allocID;
        
        /// <summary>
        /// Event to handle the completion of the request.
        /// </summary>
        public event RequestHandler Complete;

        /// <summary>
        /// Constructor of an ActorInfoRequest object. Enter the name of the player to search for and its state.
        /// </summary>
        /// <remarks>
        /// The search state helps to accelerate the search process, so try not to perform an 'All' search if its
        /// unnecessary.
        /// 
        /// An 'Online' search has the request directed to other ZoneServers to check their player lists and return
        /// the data contained within an ActorInfo object.
        /// 
        /// An 'Offline' search is directed to the least burdened AccountServer to search its records through the
        /// account script.
        /// 
        /// An 'All' search performs an online search before an offline search as the former will return more data.
        /// </remarks>
        /// <param name="findActorName">Name of the player to search for.</param>
        /// <param name="findState">States to search for.</param>
        public ActorInfoRequest(string findActorName, ActorInfoRequestState findState)
        {
            actorName = findActorName;
            requestState = findState;
            allocID = ++allocIDs;
        }

        /// <summary>
        /// Gets the state to search for.
        /// </summary>
        public ActorInfoRequestState RequestState
        {
            get { return requestState; }
        }

        /// <summary>
        /// Gets the actor name to search for.
        /// </summary>
        public string ActorName
        {
            get { return actorName; }
        }

        /// <summary>
        /// Gets the allocated internal ID for this object.
        /// </summary>
        public uint AllocID
        {
            get { return allocID; }
        }

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        /// <param name="info"></param>
        public void Invoke(ActorInfo info)
        {
            try
            {
            if (Complete != null)
                Complete.Invoke(info);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                ScriptManager.HandleException(ex);
            }
        }

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        /// <param name="info"></param>
        public void Posted(uint time)
        {
            postTime = time;
        }

        /// <summary>
        /// DO NOT DOCUMENT
        /// </summary>
        /// <param name="info"></param>
        public bool IsExpired(uint now)
        {
            // Expires after 10 seconds
            if (now - postTime > 10000)
                return true;
            return false;
        }
    }
}
