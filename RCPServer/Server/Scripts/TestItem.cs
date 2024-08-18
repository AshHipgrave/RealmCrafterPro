using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace Scripts
{
    class TestItem : ScriptBase
    {
        public void OnUse(Actor actor, Actor context, ItemInstance item)
        {
            if (context != null)
            {
                actor.Output("Used " + item.Name + " on " + context.Name + ".");
            }
            else
            {
                actor.Output("Used " + item.Name + " on self.");
            }
        }

        public void Main(Actor actor, Actor context, ItemInstance item)
        {
            if (context != null)
            {
                actor.Output("Used " + item.Name + " on " + context.Name + ".");
            }
            else
            {
                actor.Output("Used " + item.Name + " on self.");
            }
        }
    }
}
