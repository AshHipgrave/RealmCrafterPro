using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Defines how a position is processed
    /// </summary>
    public enum PositionType
    {
        /// <summary>
        /// Value is absolute (in pixels).
        /// </summary>
        Absolute = 0,

        /// <summary>
        /// Value is relative (portion of screen).
        /// </summary>
        Relative = 1,

        /// <summary>
        /// Value is relative but centered.
        /// </summary>
        Centered = 2
    }
}
