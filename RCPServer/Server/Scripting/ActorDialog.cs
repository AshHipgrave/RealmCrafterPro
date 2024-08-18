using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Delegate used for dialog input events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DialogEventHandler(object sender, DialogEventArgs e);

    /// <summary>
    /// Input/Output Dialog for Player to Script conversations.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class ActorDialog
    {
        /// <summary>
        /// Display a list of inputs to the player.
        /// </summary>
        /// <remarks>
        /// An input is similar to an output, except the player is able to click it to provide a response to
        /// the script. In order to read the input back you MUST register "InputClick" as a callback.
        /// </remarks>
        /// <param name="inputs">Array of inputs to display. It is possible to inline the array creations: .Input("Clicked", new
        /// string[] {"Input 1", "Input 2"});</param>
        public virtual void Input(string[] inputs) { }

        /// <summary>
        /// Display a message in the dialog.
        /// </summary>
        /// <remarks>
        /// By default, all outputs will be the color as defined in the Interface Editor. It is possible to use the overloaded Output
        /// method to set a full color. Additionally, a color tag can be added to any string to modify the display color at any point.
        /// See ##TODO:InlineTextProcessing.
        /// </remarks>
        /// <param name="message">Text to display.</param>
        public virtual void Output(string message) { }

        /// <summary>
        /// Display a message of a particular color in the dialog.
        /// </summary>
        /// <remarks>
        /// The color argument will override the color set in the Interface Editor, additionally, a color tag can be added to any string
        /// to modify the display color at any point. See ##TODO:InlineTextProcessing.
        /// </remarks>
        /// <param name="message">Text to display.</param>
        /// <param name="color">Color to draw the message in.</param>
        public virtual void Output(string message, System.Drawing.Color color) { }

        /// <summary>
        /// Close this dialog.
        /// </summary>
        public virtual void Close() { }
    }
}
