using System;
using System.Collections.Generic;
using System.Text;
using Scripting;
using Scripting.Forms;
using Scripting.Math;

namespace UserScripts
{
    /// <summary>
    /// Script for zone label trigger for when a player
    /// goes near the private treasure chest.
    /// </summary>
    public class ExampleTradePrivate : ScriptBase
    {
        /// <summary>
        /// Data to be displayed on the banner
        /// </summary>
        const string DisplayString = "\\c=#ff0000Private Chest";

        /// <summary>
        /// Called when a player enters the assigned trigger
        /// </summary>
        /// <param name="actor"></param>
        public void OnEnter(Actor actor)
        {
            ExampleCenterPrint.OnEnter(actor, DisplayString);
        }

        /// <summary>
        /// Called when a player exits the assigned trigger
        /// </summary>
        /// <param name="actor"></param>
        public void OnExit(Actor actor)
        {
            ExampleCenterPrint.OnExit(actor, DisplayString);
        }
    }
}
