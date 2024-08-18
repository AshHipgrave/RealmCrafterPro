using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Basic account class for saving/loading player accounts.
    /// </summary>
    public class AccountBase
    {
        string user = "";
        string pass = "";
        string email = "";
        bool isGM = false;
        bool isBanned = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="emailAddress"></param>
        /// <param name="gm"></param>
        /// <param name="banned"></param>
        public AccountBase(string username, string password, string emailAddress, bool gm, bool banned)
        {
            user = username;
            pass = password;
            email = emailAddress;
            isGM = gm;
            isBanned = banned;
        }

        /// <summary>
        /// Get account username.
        /// </summary>
        public string Username
        {
            get { return user; }
            set { user = value; }
        }

        /// <summary>
        /// Get account password.
        /// </summary>
        public string Password
        {
            get { return pass; }
            set { pass = value; }
        }

        /// <summary>
        /// Get account email address.
        /// </summary>
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        /// <summary>
        /// Get or set accounts Game Master status.
        /// </summary>
        public bool IsGM
        {
            get { return isGM; }
            set { isGM = value; }
        }

        /// <summary>
        /// Get or set whether this account is banned.
        /// </summary>
        public bool IsBanned
        {
            get { return isBanned; }
            set { isBanned = value; }
        }
    }
}
