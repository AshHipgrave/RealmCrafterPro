using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Actor Speech Type Enumeration.
    /// </summary>
    public enum SpeechType
    {
        /// <summary>
        /// Greeting remark
        /// </summary>
        Greeting1 = 0,

        /// <summary>
        /// Greeting remark (alternate)
        /// </summary>
        Greeting2 = 1,

        /// <summary>
        /// Farewell remark
        /// </summary>
        Goodbye1 = 2,

        /// <summary>
        /// Farewell remark (alternate)
        /// </summary>
        Goodbye2 = 3,

        /// <summary>
        /// Attacking
        /// </summary>
        Attack1 =  4,

        /// <summary>
        /// Attacking (alternate)
        /// </summary>
        Attack2 = 5,

        /// <summary>
        /// Taking damage
        /// </summary>
        Hit1 = 6,

        /// <summary>
        /// Taking damage (alternate)
        /// </summary>
        Hit2 = 7,

        /// <summary>
        /// Calling for help sound.
        /// </summary>
        RequestHelp = 8,

        /// <summary>
        /// Death south
        /// </summary>
        Death = 9,

        /// <summary>
        /// Foot sounds when not raining.
        /// </summary>
        FootstepDry = 10,

        /// <summary>
        /// Foot sounds when raining.
        /// </summary>
        FootstepWet = 11
    }
}
