using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public class BankInteract : ScriptBase
    {
        public void OnInteract(Actor actor, SceneryInstance scenery)
        {
            if (actor.FindControl(BankWindow.BankWindow_Name) == null)
            {
                // Use this to just call the bank window for the actor
                BankWindow bankForm = new BankWindow(actor);
                actor.CreateDialog(bankForm);
            }
        }
    }
}