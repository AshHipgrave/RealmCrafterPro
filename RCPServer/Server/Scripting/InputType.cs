using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Input Type Enumeration for TextBox Prompts
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// Any type of input.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Text only input.
        /// </summary>
        Text = 1,

        /// <summary>
        /// Integer only input.
        /// </summary>
        Integer = 2,

        /// <summary>
        /// Floating point number intput.
        /// </summary>
        Floating = 3
    }
}
