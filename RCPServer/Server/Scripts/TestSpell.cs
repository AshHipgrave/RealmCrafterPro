using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    class TestSpell : ScriptBase
    {
        public void OnCast(Actor actor, Actor context, string abilityName, int abilityLevel)
        {
            actor.Output("Cast spell " + abilityName + " which is level " + abilityLevel + " on " + context.Name);
        }

        public void OnCast(Actor actor, Actor context, int abilityLevel)
        {
            actor.Output("Cast spell without name");
        }
    }
}
