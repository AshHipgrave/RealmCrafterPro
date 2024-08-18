using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Enter description here
    /// </summary>
    public class SpawnTest : ScriptBase
    {
        ActorInfoRequest Request;

        /// <summary>
        /// Method called when assigned actor spawns
        /// </summary>
        /// <param name="contextActor"></param>
        public void OnSpawn(Actor contextActor)
        {
            // Rename this guy to Bob
            //RCScript.Log("Renaming: " + contextActor.Name + " > Bob");
           // contextActor.Name = "Bob";
        }

        /// <summary>
        /// Method called when a player right clicks the actor.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="contextActor"></param>
        public void OnInteract(Actor actor, Actor contextActor)
        {
            actor.Output(1, "You've interacted with: " + contextActor.Name);

            // Create an post a request to look for another player called 'dfg'
           // Request = new ActorInfoRequest("dfg", ActorInfoRequestState.All);
            //RegisterCallback(Request, "Complete", new ActorInfoRequest.RequestHandler(Request_Complete));
            //RCScript.PostActorInfoRequest(Request);
           actor.Target = contextActor;

        }

        /// <summary>
        /// Method called when the ActorInfoRequest is completed
        /// </summary>
        /// <param name="actorInfo"></param>
        public void Request_Complete(ActorInfo actorInfo)
        {
            // If the player is online
            if (actorInfo.State == ActorInfoState.Online)
            {
                // Send a whisper message
                actorInfo.Output(1, "Whisper!", System.Drawing.Color.Magenta);
            }

            // Cleanup
            UnRegisterCallback(Request, "Complete", new ActorInfoRequest.RequestHandler(Request_Complete));
        }


        /// <summary>
        /// Method called when actor dies
        /// </summary>
        /// <param name="victim"></param>
        /// <param name="killer"></param>
        public void OnDeath(Actor victim, Actor killer)
        {
            RCScript.Log("OnDeath");
            if (killer.Human)
            {
                killer.Output("You killed: " + victim.Name);
            }else if(victim.Human)
            {
                victim.Output("You were killed by: " + killer.Name);
            }
        }
    }
}
