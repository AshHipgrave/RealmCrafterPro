using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    public class Spawn : ScriptBase
    {
        /// <summary>
        /// This is called when no spawn script is set on a NPC.
        /// </summary>
        /// <param name="actor"></param>
        public void OnSpawn(Actor actor)
        {
            //RCScript.DispatchMessage(actor, actor.Name + " has spawned", 0, 2);
        }
    }
}
