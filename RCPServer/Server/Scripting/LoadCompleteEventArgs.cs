using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Event Arguments class for transferring loaded account data
    /// back into the Account Server.
    /// </summary>
    public class LoadCompleteEventArgs : System.EventArgs
    {
        AccountBase accountHandle;
        List<ActorInstanceData> instances;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="handle">Account handle which relates to the attached data.</param>
        public LoadCompleteEventArgs(AccountBase handle)
        {
            accountHandle = handle;
            instances = new List<ActorInstanceData>();
        }

        /// <summary>
        /// Gets the account handle for the instances data.
        /// </summary>
        public AccountBase AccountHandle
        {
            get { return accountHandle; }
        }

        /// <summary>
        /// Gets a list of ActorInstanceData for each character which an account owns.
        /// </summary>
        public List<ActorInstanceData> ActorInstances
        {
            get { return instances; }
        }
    }
}
