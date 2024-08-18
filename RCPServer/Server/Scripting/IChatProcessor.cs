using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Interface for a class which will process chat inputs and slash commands
    /// </summary>
    public interface IChatProcessor
    {
        /// <summary>
        /// Method called when a user enters chat text.
        /// </summary>
        /// <param name="actor">Handle of the player.</param>
        /// <param name="message">Message which the player wrote.</param>
        void OnChatText(Actor actor, string message);

        /// <summary>
        /// Method called when a user enters chat texture prefixed with a '/'.
        /// </summary>
        /// <param name="actor">Handle of the player.</param>
        /// <param name="command">Uppercase commandtext.</param>
        /// <param name="data">Command parameters</param>
        void OnSlashCommand(Actor actor, string command, string data);
    }
}
