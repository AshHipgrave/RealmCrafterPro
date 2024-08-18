using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Forms Event Delegate.
    /// </summary>
    /// <param name="sender">Control which triggered the event.</param>
    /// <param name="e">Event information.</param>
    public delegate void EventHandler(object sender, FormEventArgs e);

    /// <summary>
    /// Handler for events triggered on a remote client.
    /// </summary>
    /// <param name="sender">Control the event relates to.</param>
    /// <param name="reader">Event information passed from client.</param>
    public delegate void FormProcessEventHandler(Control sender, PacketReader reader);
}
