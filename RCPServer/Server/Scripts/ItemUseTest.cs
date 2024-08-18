using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    class ItemUseTest : ScriptBase
    {
        public void Main(Actor actor, object o)
        {
            actor.Output("Used item!");
        }
        public void Main(Actor actor)
        {
            actor.Output("Used item!");
        }

        public void OnUse(Actor actor, Actor target)
        {
            actor.Output("FUCK THIS");
        }



    }
}
