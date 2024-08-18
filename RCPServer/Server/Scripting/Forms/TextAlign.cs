using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Text alignment for GUI labels
    /// </summary>
    public enum TextAlign
    {
        /// <summary>
        /// Use skin default.
        /// </summary>
        Default = -1,

        /// <summary>
        /// Align to the left.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Align to the middle.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Align to the right.
        /// </summary>
        Right = 2,

        /// <summary>
        /// Align to the top.
        /// </summary>
        Top = 0,

        /// <summary>
        /// Align to the middle.
        /// </summary>
        Middle = 1,

        /// <summary>
        /// Align to the bottom.
        /// </summary>
        Bottom = 2
    }
}
