using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public class Login : ScriptBase
    {
        public void OnLogin(Actor actor)
        {
            // Make sure doesn't have negative hp - if so die to reset it
            if (actor.GetAttribute("Health") < 1)
                actor.Kill();

            // Default player setup
            actor.CreateChatTab(0, "Main", 60);
            actor.CreateChatTab(1, "PM: OtherPlayer", 130);
            actor.SwitchToTab(0);

            actor.AddAbility("Spell1");
            actor.AddAbility("Spell2");
            actor.AddAbility("Spell3");
            actor.AddAbility("Spell4");
            actor.AddAbility("Spell5");
            actor.AddAbility("Spell6");


             actor.GiveItem("Torch", 1);
             actor.GiveItem("Imperial Helmet", 1);
             actor.GiveItem("Adventure Stone", 20);


            RCScript.Log(string.Format("Scripting.Login.Main(\"{0}\") Called.", actor.Name));

//             actor.Output(WorldEnvironment.Hour.ToString() + ":" + WorldEnvironment.Minute.ToString());
//             RCScript.Log(WorldEnvironment.Hour.ToString() + ":" + WorldEnvironment.Minute.ToString());

//             WaitItemRequest[] CurrentRequests = actor.GetWaitItemRequests();
//             bool Found = CurrentRequests.Length > 0;
// 
//             if(!Found)
//                 actor.CreateWaitItemRequest("WaitScripts", "OnWaitItem", "Torch", 1);

            //RCScript.DelayedExecute("WaitScripts", "LocalWaitTime", new object[] { actor, null }, actor, null);
            
        }
    }
}

