using System;
using System.Collections.Generic;
using System.Text;
using Scripting;
using Scripting.Forms;
using Scripting.Math;

namespace UserScripts
{
    /// <summary>
    /// Example script for printing a banner message on the client screen.
    /// Used by the default trading trigger scripts to display the current
    /// chest type.
    /// </summary>
    public class ExampleCenterPrint
    {
        /// <summary>
        /// Name of display label (for recycling).
        /// </summary>
        const string BannerName = "CenterPrint";

        /// <summary>
        /// Called when a player enters the assigned trigger
        /// </summary>
        /// <param name="actor"></param>
        public static void OnEnter(Actor actor, string displayString)
        {
            // A label might already be onscreen if triggers overlap or
            // if the server lags when updating. Its better to recycle
            // a label than have several rendered on the client at once.
            Label Display = actor.FindControl(BannerName) as Label;
            bool Create = false;

            // No existing display, so create one.
            if (Display == null)
            {
                Display = new Label();
                Create = true;
            }

            // Setup or reset data.
            Display.Text = displayString;
            Display.Name = BannerName;
            Display.Size = new Vector2(300, 16);
            Display.SizeType = SizeType.Absolute;
            Display.Align = TextAlign.Center;
            Display.Location = new Vector2(0.5f, 0.3f);
            Display.PositionType = PositionType.Centered;

            // Create on client if necessary.
            if (Create)
            {
                actor.CreateDialog(Display);
            }
        }

        /// <summary>
        /// Called when a player exits the assigned trigger
        /// </summary>
        /// <param name="actor"></param>
        public static void OnExit(Actor actor, string displayString)
        {
            // Find control
            Label Display = actor.FindControl(BannerName) as Label;

            // If it exists and its currently in use for *this* script, then delete it.
            if (Display != null && Display.Text.Equals(displayString))
                actor.CloseDialog(Display);
        }
    }
}
