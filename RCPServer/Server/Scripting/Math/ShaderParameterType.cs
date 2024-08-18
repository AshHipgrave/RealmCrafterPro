using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Math
{
    /// <summary>
    /// Enumeration of possible shader types
    /// </summary>
    public enum ShaderParameterType
    {
        /// <summary>
        /// A one dimensional value.
        /// </summary>
        Float = 0,

        /// <summary>
        /// Two dimensional value.
        /// </summary>
        Vector2 = 1,

        /// <summary>
        /// Three dimensional value.
        /// </summary>
        Vector3 = 2,

        /// <summary>
        /// Four dimensional value.
        /// </summary>
        Vector4 = 3,

        /// <summary>
        /// Special value representing 'time' in seconds.
        /// </summary>
        Time = 4,

        /// <summary>
        /// Special value representing 'time' in seconds with a reset parameter.
        /// </summary>
        TimeReset = 5,

        /// <summary>
        /// Specular value represention 'time' looping within a boundary.
        /// </summary>
        TimeLoop = 6
    }
}
