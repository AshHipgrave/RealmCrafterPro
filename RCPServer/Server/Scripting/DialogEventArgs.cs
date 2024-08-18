using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Event arguments used for dialog input callbacks.
    /// </summary>
    public class DialogEventArgs : System.EventArgs
    {
        int option;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inOption"></param>
        public DialogEventArgs(int inOption)
        {
            option = inOption;
        }

        /// <summary>
        /// Get the selected option (0-based).
        /// </summary>
        public int Option
        {
            get
            {
                return option;
            }
        }
    }
}
