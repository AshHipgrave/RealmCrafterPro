using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Script for tradable scenery objects (chests) usable by ALL.
    /// </summary>
    public class SceneryTradePublic : ScriptBase
    {
        public void OnInteract(Actor actor, SceneryInstance scenery)
        {
            // Fairly simple, just show the form dialog, it does the rest
            SceneryTradeWindow TradeForm = new SceneryTradeWindow();
            TradeForm.SetupWindow(scenery);
            actor.CreateDialog(TradeForm);
        }
    }
}
