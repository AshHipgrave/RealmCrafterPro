using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    public class KillReqTest : ScriptBase 
    {
        public void WaitKill(Actor actor, WaitKillRequest wk)
        {
            actor.Output("KILLS DONE!");
        }
    }
}
