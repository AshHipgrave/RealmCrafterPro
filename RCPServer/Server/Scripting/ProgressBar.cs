using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Onscreen Progressbar Instance.
    /// </summary>
    public class ProgressBar
    {
        /// <summary>
        /// Change the value of the progressbar.
        /// </summary>
        /// <param name="value">New value to set (between 0 and the specified maximum).</param>
        public virtual void Update(int value) { }

        /// <summary>
        /// Remove this progressbar.
        /// </summary>
        /// <remarks>This command will remove both the client and server instance of the progressbar. Further use
        /// of this class will either fail or cause an exception to be thrown.
        /// </remarks>
        public virtual void Remove() { }
    }
}
