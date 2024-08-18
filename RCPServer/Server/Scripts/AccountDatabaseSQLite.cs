using System;
using System.Collections.Generic;
using System.Text;
using Community.CsharpSqlite.SQLiteClient;
using Community.CsharpSqlite;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace Scripting
{
    // Remember, multiple IAccountDatabase objects can cause RCP to select the wrong one. 
    // Ensure you've disabled MySQL in AccountDatabaseMySQL to use SQLite
    public class AccountDatabaseSQLite /*: IAccountDatabase*/
    {
        class ActorInstance
        {
            public string Name;
            public string AccountName;
            public bool Banned;
            public bool GM;
            public int ID;
        }


        String connString = string.Format("Version=3,uri=file:{0}", DatabaseSettings.SQLiteAccountsFile);
        Dictionary<string, AccountBase> AccountReferences = new Dictionary<string, AccountBase>();
        //private int GarbageRunDelay = 60;
        private int CurrentQuery = 0;
        private DateTime LastUpdate;
        //private DateTime GarbageRun;  
        private SqliteConnection sql_con;
        AccountAddEventHandler AddDelegate;
        // Use this to speed up searches!
        static LinkedList<ActorInstance> SearchDB = new LinkedList<ActorInstance>();

        /// <summary>
        /// Initialize account database.
        /// </summary>
        public void Initialize(AccountAddEventHandler addDelegate)
        {
            AddDelegate = addDelegate;
            // String connString = "Data Source=" + FilePath;
            using (sql_con = new SqliteConnection(connString))
            {
                sql_con.Open();
                IDbCommand AcctCmd = sql_con.CreateCommand();
                AcctCmd.CommandText = "SELECT * FROM Accounts";


                using (IDataReader dr = AcctCmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string Username = dr.GetValue(0).ToString();
                        string Password = dr.GetValue(1).ToString();
                        string Email = dr.GetValue(2).ToString();
                        bool GM = dr.GetBoolean(3);
                        bool Banned = dr.GetBoolean(4);

                        IDbCommand CharCmd = sql_con.CreateCommand();
                        CharCmd.CommandText = "SELECT * FROM Characters WHERE Username = @Username ORDER BY ID";

                        CharCmd.Parameters.Add(new SqliteParameter("@Username", Username));

                        using (IDataReader CharDR = CharCmd.ExecuteReader())
                        {
                            while (CharDR.Read())
                            {
                                ActorInstance AI = new ActorInstance();
                                AI.AccountName = Username;
                                AI.Banned = Banned;
                                AI.GM = GM;
                                object CharacterID = CharDR.GetValue(1);
                                AI.ID = int.Parse(CharacterID.ToString());
                                AI.Name = (string)CharDR.GetValue(2);
                                SearchDB.AddLast(AI);
                            }
                        }

                        AccountReferences.Add(Username, new AccountBase(Username, Password, Email, GM, Banned));
                        addDelegate.Invoke(AccountReferences[Username], true);
                        LastUpdate = DateTime.Now;
                        //GarbageRun =DateTime.Now;
                        //GC.Collect();
                    }
                }
                sql_con.Close();
            }
        }


        /// <summary>
        /// Add an account to the account database
        /// </summary>
        public void Add(AccountBase account, AccountAddEventHandler completionDelegate)
        {
            // Check
            if (account == null)
            {
                completionDelegate.Invoke(account, false);
                return;
            }

            using (sql_con = new SqliteConnection(connString))
            {
                sql_con.Open();
                IDbCommand sqlComm = sql_con.CreateCommand();
                sqlComm.CommandText = "begin";
                sqlComm.ExecuteNonQuery();

                IDbCommand NewAcct = sql_con.CreateCommand();
                NewAcct.CommandText = "INSERT INTO Accounts (Username, Password, EMail, GM, Banned) VALUES (@Username,@Password,@EMail,@GM,@Banned)";

                NewAcct.Parameters.Add(new SqliteParameter("@Username", account.Username));
                NewAcct.Parameters.Add(new SqliteParameter("@Password", account.Password));
                NewAcct.Parameters.Add(new SqliteParameter("@EMail", account.Email));
                NewAcct.Parameters.Add(new SqliteParameter("@GM", account.IsGM));
                NewAcct.Parameters.Add(new SqliteParameter("@Banned",account.IsBanned));
                NewAcct.ExecuteNonQuery();
                sqlComm = sql_con.CreateCommand();
                sqlComm.CommandText = "end";
                sqlComm.ExecuteNonQuery();
            }

            RCScript.Log("Account created: " + account.Username);
            AccountReferences.Add(account.Username, account);
            completionDelegate.Invoke(AccountReferences[account.Username], true);

            // Uncomment this to create 100000 test accounts.
            // Revert after test complete
            //using (sql_con = new SQLiteConnection(connString))
            //{
            //    sql_con.Open();
            //    SQLiteCommand sqlComm = new SQLiteCommand("begin", sql_con);
            //    sqlComm.ExecuteNonQuery();
            //    for (int i = 0; i < 100000; i++)
            //    {
            //
            //        SQLiteCommand NewAcct = new SQLiteCommand("INSERT INTO Accounts (Username, Password, EMail, GM, Banned) VALUES (@Username,@Password,@EMail,@GM,@Banned)", sql_con);
            //        NewAcct.Parameters.Add("@Username", System.Data.DbType.String).Value = account.Username + i.ToString();
            //        NewAcct.Parameters.Add("@Password", System.Data.DbType.String).Value = account.Password;
            //        NewAcct.Parameters.Add("@EMail", System.Data.DbType.String).Value = account.Email;
            //        NewAcct.Parameters.Add("@GM", System.Data.DbType.Boolean).Value = account.IsGM;
            //        NewAcct.Parameters.Add("@Banned", System.Data.DbType.Boolean).Value = account.IsBanned;
            //        NewAcct.ExecuteNonQuery();
            //    }
            //    sqlComm = new SQLiteCommand("end", sql_con);
            //    sqlComm.ExecuteNonQuery();
            //
            //    AccountReferences.Add(account.Username, account);
            //    completionDelegate.Invoke(AccountReferences[account.Username], true);
            //}


        }

        /// <summary>
        /// Load an accounts actors.
        /// </summary>
        public void Load(AccountBase account, AccountLoadEventHandler completionDelegate)
        {
            // Check
            if (account == null)
            {
                completionDelegate.Invoke(account, null);
                return;
            }

            LoadCompleteEventArgs Args = new LoadCompleteEventArgs(account);
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                IDbCommand CharCmd = conn.CreateCommand();
                CharCmd.CommandText = "SELECT * FROM Characters WHERE Username = @Username ORDER BY ID";
                CharCmd.Parameters.Add(new SqliteParameter("@Username", account.Username));

                using (IDataReader CharDR = CharCmd.ExecuteReader())
                {
                    while (CharDR.Read())
                    {
                        byte[] SerializedAI = (byte[])CharDR.GetValue(3);
                        byte[] SerializedScripts = (byte[])CharDR.GetValue(4);
                        Args.ActorInstances.Add(new ActorInstanceData(SerializedAI, SerializedScripts));
                    }
                }
                completionDelegate.Invoke(account, Args);
            }
        }

        /// <summary>
        /// Save an accounts actors.
        /// </summary>
        public void Save(AccountBase account, int index, ActorInstanceData data)
        {
            // Check
            if (account == null)
                return;


            // Setup output
            //List<ActorInstanceData> ActorInstances = new List<ActorInstanceData>();

            //Delete character
            if (data == null)
            {
                using (sql_con = new SqliteConnection(connString))
                {
                    sql_con.Open();
                    IDbCommand CharCmd = sql_con.CreateCommand();
                    CharCmd.CommandText = "DELETE FROM Characters WHERE Username = @Username AND ID = @index";
                    CharCmd.Parameters.Add(new SqliteParameter("@Username", account.Username));
                    CharCmd.Parameters.Add(new SqliteParameter("@index", index));
                    CharCmd.ExecuteNonQuery();

                    LinkedListNode<ActorInstance> AINode = SearchDB.First;
                    while (AINode != null)
                    {
                        ActorInstance AI = AINode.Value;
                        if (AI.ID == index && AI.AccountName == account.Username)
                        {
                            SearchDB.Remove(AINode);
                        }
                        AINode = AINode.Next;
                    }
                    return;
                }
            }

            byte Length = data.SerializedAI[4];
            String Name = ASCIIEncoding.ASCII.GetString(data.SerializedAI, 5, Length);

            using (sql_con = new SqliteConnection(connString))
            {
                sql_con.Open();
                bool written = false;

                // Update account details which may have been edited via scripts.
                IDbCommand AcctCmd = sql_con.CreateCommand();
                AcctCmd.CommandText = "UPDATE Accounts SET Username = @Username, Password = @Password, EMail=@EMail, GM = @GM, Banned = @Banned WHERE Username =@Username";
                AcctCmd.Parameters.Add(new SqliteParameter("@Username", account.Username));
                AcctCmd.Parameters.Add(new SqliteParameter("@Password", account.Password));
                AcctCmd.Parameters.Add(new SqliteParameter("@EMail", account.Email));
                AcctCmd.Parameters.Add(new SqliteParameter("@GM", account.IsGM));
                AcctCmd.Parameters.Add(new SqliteParameter("@Banned", account.IsBanned));
                AcctCmd.ExecuteNonQuery();

                //Update character if it already exists.
                IDbCommand CharCmd = sql_con.CreateCommand();
                CharCmd.CommandText = "SELECT * FROM Characters WHERE ActorName = @Name";
                CharCmd.Parameters.Add(new SqliteParameter("@Name", Name));
                using (IDataReader CharDR = CharCmd.ExecuteReader())
                {
                    while (CharDR.Read())
                    {
                        CharCmd = sql_con.CreateCommand();
                        CharCmd.CommandText = "UPDATE Characters SET Username = @Username, ID = @Index, ActorName = @Name, SerializedAI = @serializedAI, SerializedScripts = @serializedScripts WHERE ActorName = @Name AND Username = @Username";

                        CharCmd.Parameters.Add(new SqliteParameter("@Username", account.Username));
                        CharCmd.Parameters.Add(new SqliteParameter("@Index", index));
                        CharCmd.Parameters.Add(new SqliteParameter("@Name", Name));
                        CharCmd.Parameters.Add(new SqliteParameter("@serializedAI", data.SerializedAI));
                        CharCmd.Parameters.Add(new SqliteParameter("@serializedScripts", data.SerializedScripts));

                        CharCmd.ExecuteNonQuery();
                        written = true;
                    }
                }

                //Character was just created, create a new database entry
                if (!written)
                {
                    CharCmd = sql_con.CreateCommand();
                    CharCmd.CommandText = "Insert INTO Characters (Username, ID, ActorName, SerializedAI, SerializedScripts) VALUES(@UserName,@Index,@Name,@serializedAI,@serializedScripts)";
                    CharCmd.Parameters.Add(new SqliteParameter("@UserName", account.Username));
                    CharCmd.Parameters.Add(new SqliteParameter("@Index", index));
                    CharCmd.Parameters.Add(new SqliteParameter("@Name",  Name));
                    CharCmd.Parameters.Add(new SqliteParameter("@serializedAI", data.SerializedAI));
                    CharCmd.Parameters.Add(new SqliteParameter("@serializedScripts", data.SerializedScripts));
                    CharCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (DatabaseSettings.AccountUpdatePollingEnabled)
            {
                int Difference = DateTime.Compare(DateTime.Now, LastUpdate.AddSeconds(DatabaseSettings.AccountsPollingDelay));
                if (Difference > 0)
                {
                    using (sql_con = new SqliteConnection(connString))
                    {
                        LastUpdate = DateTime.Now;
                        sql_con.Open();

                        IDbCommand AcctCmd = sql_con.CreateCommand();
                        AcctCmd.CommandText = "SELECT * FROM Accounts LIMIT @Start,@End";
                        AcctCmd.Parameters.Add(new SqliteParameter("@Start", CurrentQuery));
                        AcctCmd.Parameters.Add(new SqliteParameter("@End", DatabaseSettings.AccountUpdatesPerQuery));
                        int count = 0;
                        using (IDataReader dr = AcctCmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string Username = (string)dr.GetValue(0);
                                if (AccountReferences.ContainsKey(Username))
                                {
                                    // AccountReferences[account.Username].Username = (string)dr.GetValue(0);
                                    AccountReferences[Username].Password = (string)dr.GetValue(1);
                                    AccountReferences[Username].Email = (string)dr.GetValue(2);
                                    AccountReferences[Username].IsGM = (bool)dr.GetValue(3);
                                    AccountReferences[Username].IsBanned = (bool)dr.GetValue(4);
                                    count++;
                                }
                                else
                                {
                                    string Password = dr.GetValue(1).ToString();
                                    string Email = dr.GetValue(2).ToString();
                                    bool GM = (bool)dr.GetValue(3);
                                    bool Banned = (bool)dr.GetValue(4);
                                    AccountReferences.Add(Username, new AccountBase(Username, Password, Email, GM, Banned));
                                    AddDelegate.Invoke(AccountReferences[Username], true);
                                    RCScript.Log("Account created: " + Username);
                                }
                            }
                        }
                        CurrentQuery += DatabaseSettings.AccountUpdatesPerQuery;
                        if (count < DatabaseSettings.AccountUpdatesPerQuery)
                        {
                            CurrentQuery = 0;
                        }
                    }
                }
                //int GCDifference = DateTime.Compare(DateTime.Now, GarbageRun.AddSeconds(GarbageRunDelay));
                //if (GCDifference > 0)
                //{
                //    GC.Collect();
                //    RCScript.Log("Garbage collected");
                //    GarbageRun = DateTime.Now;
                //}
            }
        }

        /// <summary>
        /// Perform a search for the given actor name to see if it exists.
        /// </summary>
        /// <param name="request">Handle of request object.</param>
        /// <param name="completionDelegate">Callback for completion of search.</param>
        public void OnActorInfoRequest(ActorInfoRequest request, AccountActorInfoRequestHandler completionDelegate)
        {
            foreach (ActorInstance AI in SearchDB)
            {
                if (AI.Name.Equals(request.ActorName, StringComparison.CurrentCultureIgnoreCase))
                {
                    completionDelegate.Invoke(request, AI.Name, AI.AccountName, AI.Banned, AI.GM);
                    return;
                }
            }

            // Failed!
            completionDelegate.Invoke(request, null, null, false, false);
        }

        static Regex UserValidation = new Regex("^[0-9a-zA-Z_]");

        public void VerifyUsername(AccountNameResult result)
        {
            // Note change to string.Empty if not a valid username
            if (!UserValidation.IsMatch(result.Username))
            {
                result.Result = false;
                return;
            }

            result.Result = true;

        }
    }
}