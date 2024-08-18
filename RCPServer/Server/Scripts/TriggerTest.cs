using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Enter description here
    /// </summary>
    public class TriggerTest : ScriptBase
    {

        public void OnEnter(Actor actor)
        {
            actor.Output("Entered Trigger!");
        }

        public void OnExit(Actor actor)
        {
            actor.Output("Exited Trigger!");
        }
    }
}
