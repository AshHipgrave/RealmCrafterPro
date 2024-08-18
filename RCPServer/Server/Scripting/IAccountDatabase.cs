using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Delegate used when an account load has completed.
    /// </summary>
    /// <param name="handle">Handle of account.</param>
    /// <param name="e">Arguments structure, or null if failed.</param>
    public delegate void AccountLoadEventHandler(AccountBase handle, LoadCompleteEventArgs e);

    /// <summary>
    /// Delegate used when an account is added.
    /// </summary>
    /// <param name="handle">Handle of account.</param>
    /// <param name="result">Success or Fail.</param>
    public delegate void AccountAddEventHandler(AccountBase handle, bool result);

    /// <summary>
    /// Delegate for the completion of an ActorInfoRequest through the account database.
    /// </summary>
    /// <param name="request">Request passed to the OnActorInfoRequest method</param>
    /// <param name="actorName">Case-correct name of the actor.</param>
    /// <param name="accountName">Account name of the actor.</param>
    /// <param name="isBanned">Actor banned status.</param>
    /// <param name="isGM">Actor moderator status.</param>
    public delegate void AccountActorInfoRequestHandler(ActorInfoRequest request, string actorName, string accountName, bool isBanned, bool isGM);

    /// <summary>
    /// Interface for scripted account management.
    /// 
    /// See documentation for more details.
    /// </summary>
    public interface IAccountDatabase
    {
        /// <summary>
        /// Initialize account database.
        /// </summary>
        /// <remarks>
        /// Initialize() is called during server startup which is the ideal time to
        /// connect to external databases and pre-load any necessary account related data.
        /// 
        /// A list of 'AccountBase' objects must be returned so that authentication
        /// of players can take place before their individual actor data is loaded.
        /// 
        /// NOTE: Special care needs to be taken when using multiple account servers as only
        /// the 'most available' account server is used for transactions, causing other servers
        /// not to hold up to date information.
        /// </remarks>
        /// <param name="addDelegate">Method to call when an account instance must be added.</param>
        void Initialize(AccountAddEventHandler addDelegate);

        /// <summary>
        /// Add an account to the account database.
        /// 
        /// Note: The completionDelegate must be invoked from the same thread in which this method
        /// was called.
        /// </summary>
        /// <param name="account">Account information to add.</param>
        /// <param name="completionDelegate">Method to call with completion data.</param>
        void Add(AccountBase account, AccountAddEventHandler completionDelegate);

        /// <summary>
        /// Load an accounts characters.
        /// 
        /// Note: The completionDelegate must be invoked from the same thread in which this method
        /// was called.
        /// </summary>
        /// <param name="account">Account information to load.</param>
        /// <param name="completionDelegate">Method to call with completion data.</param>
        void Load(AccountBase account, AccountLoadEventHandler completionDelegate);

        /// <summary>
        /// Save an accounts characters.
        /// 
        /// Input 'data' will be null when the specified index has been deleted.
        /// </summary>
        /// <param name="account">Account information to save.</param>
        /// <param name="index">Index of the given actor instance.</param>
        /// <param name="data">Array of characters belonging to the account.</param>
        void Save(AccountBase account, int index, ActorInstanceData data);

        /// <summary>
        /// Update account database. 
        /// 
        /// Handle any asynchronous completion here.
        /// </summary>
        void Update();

        /// <summary>
        /// Perform a search for the given actor name to see if it exists.
        /// </summary>
        /// <param name="request">Handle of request object.</param>
        /// <param name="completionDelegate">Callback for completion of search.</param>
        void OnActorInfoRequest(ActorInfoRequest request, AccountActorInfoRequestHandler completionDelegate);
    }
}
