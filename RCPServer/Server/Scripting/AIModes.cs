using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// NPC AI State.
    /// </summary>
    public enum AIModes
    {
        /// <summary>
        /// Actor is idle.
        /// </summary>
        AI_Wait = 0,

        /// <summary>
        /// Actor is patrolling
        /// </summary>
        AI_Patrol = 1,

        /// <summary>
        /// Actor is running to a destination.
        /// </summary>
        AI_Run = 2,

        /// <summary>
        /// Actor is chasing another actor.
        /// </summary>
        AI_Chase = 3,

        /// <summary>
        /// Actor has stopped patrolling.
        /// </summary>
        AI_PatrolPause = 4,

        /// <summary>
        /// Actor is a pet.
        /// </summary>
        AI_Pet = 5,

        /// <summary>
        /// Actor is a pet chasing another actor.
        /// </summary>
        AI_PetChase = 6,

        /// <summary>
        /// Actor is a pet who has stopped.
        /// </summary>
        AI_PetWait = 7,

        /// <summary>
        /// Actor will move to given destination then switch back to AI_Wait
        /// </summary>
        AI_MoveToDestination = 8
    }
}
