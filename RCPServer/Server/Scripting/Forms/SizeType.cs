using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Defines how a size is processed
    /// </summary>
    public enum SizeType
    {
        /// <summary>
        /// Value is absolute (size is in pixels)
        /// </summary>
        Absolute = 0,

        /// <summary>
        /// Value is relative (portion of the screen)
        /// </summary>
        Relative = 1
    }
}
