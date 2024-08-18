using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    public class WaitScripts : ScriptBase
    {
        public void LocalWaitTime(Actor actor, Actor contextActor)
        {
            WaitTimeRequest WaitTimer = actor.CreateWaitTimeRequest(WorldEnvironment.Hour + 1, WorldEnvironment.Minute);
            actor.Output("LocalWaitTime Start: " + WorldEnvironment.Hour.ToString() + ":" + WorldEnvironment.Minute.ToString());
            RCScript.Log("LocalWaitTime Start:" + WorldEnvironment.Hour.ToString() + ":" + WorldEnvironment.Minute.ToString());

            RegisterCallback(WaitTimer, "Elapsed", new WaitTimeRequest.RequestHandler(WaitTimer_Elapsed));
        }

        public void WaitTimer_Elapsed(Actor actor, WaitTimeRequest request)
        {
            actor.Output("WaitScripts.WaitTimer_Elapsed Triggered at: " + request.Hour + ":" + request.Minute);
            RCScript.Log("WaitScripts.WaitTimer_Elapsed Triggered at: " + request.Hour + ":" + request.Minute);
            ClearCallbacks();
        }

        public void OnWaitTime(Actor actor, WaitTimeRequest request)
        {
            //actor.Output("WaitScripts.OnWaitTime Triggered at: " + request.Hour + ":" + request.Minute);
            RCScript.Log("WaitScripts.OnWaitTime Triggered at: " + request.Hour + ":" + request.Minute);
        }

        public void OnWaitSpeak(Actor actor, Actor contextActor)
        {
            actor.Output("WaitScripts.OnWaitSpeak Triggered by: " + actor.Name + " > " + contextActor.Name);
            RCScript.Log("WaitScripts.OnWaitSpeak Triggered by: " + actor.Name + " > " + contextActor.Name);
        }

        public void OnWaitKill(Actor actor, WaitKillRequest request)
        {
            actor.Output("WaitScripts.OnWaitKill Triggered by: " + actor.Name);
            RCScript.Log("WaitScripts.OnWaitKill Triggered by: " + actor.Name);
        }

        public void OnWaitItem(Actor actor, WaitItemRequest request)
        {
            actor.Output("WaitScripts.OnWaitItem Triggered by: " + actor.Name);
            RCScript.Log("WaitScripts.OnWaitItem Triggered by: " + actor.Name);
        }
    }
}
