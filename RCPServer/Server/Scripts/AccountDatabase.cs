using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;
using System.Text.RegularExpressions;

namespace Scripting
{
    // THIS SCRIPT IS DEPRECATED, AND IS ONLY HERE FOR BACKWARDS COMPATIBILITY AND REFERENCE
    // PLEASE USE AccountDatabaseSQLite.cs
    public class AccountDatabase : IAccountDatabase
    {
        class ActorInstance
        {
            public string Name;
            public string AccountName;
            public bool Banned;
            public bool GM;
            public int ID;
        }

        static string FilePath = @"Data/Server Data/Accounts/";

        // Use this to speed up searches!
        static LinkedList<ActorInstance> SearchDB = new LinkedList<ActorInstance>();

        /// <summary>
        /// Initialize account database.
        /// </summary>
        public void Initialize(AccountAddEventHandler addDelegate)
        {
            // Gather list of account files in account directory
            // Exceptions thrown here are caught by the loading phase
            string[] Files = Directory.GetFiles(FilePath, "*.dat");

            // Pre-load each account
            foreach (string FileName in Files)
            {
                // Read header
                using (BBBinaryReader Reader = new BBBinaryReader(File.OpenRead(FileName)))
                {

                    string Username = Path.GetFileNameWithoutExtension(FileName);
                    string Password = Reader.ReadBBString();
                    string Email = Reader.ReadBBString();
                    bool GM = Reader.ReadBoolean();
                    bool Banned = Reader.ReadBoolean();

                    // Read through actor data and pull out instances
                    // This isn't necessary, but it saves a massive amount of CPU
                    // and IO read time when processing Offline/All ActorInfo requests
                    int ID = 0;
                    while (!Reader.Eof)
                    {
                        int ActorLength = Reader.ReadInt32();
                        byte[] SerializedAI = Reader.ReadBytes(ActorLength);

                        int ScriptsLength = Reader.ReadInt32();
                        byte[] SerializedScripts = Reader.ReadBytes(ScriptsLength);

                        ActorInstance AI = new ActorInstance();
                        AI.AccountName = Username;
                        AI.Banned = Banned;
                        AI.GM = GM;

                        // The serialized actor data begins with a 32-bit ID followed by a namestring with an 8-bit length
                        byte Length = SerializedAI[4];
                        AI.Name = ASCIIEncoding.ASCII.GetString(SerializedAI, 5, Length);
                        AI.ID = ID;

                        SearchDB.AddLast(AI);
                        ++ID;
                    }

                    Reader.Close();

                    // New account base
                    addDelegate.Invoke(
                        new AccountBase(Username, Password, Email, GM, Banned),
                        true);
                }



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

            // Attempt to write file
            using (BBBinaryWriter Writer = new BBBinaryWriter(File.Open(Path.Combine(FilePath, account.Username + ".dat"), FileMode.Create)))
            {

                // Write account header
                Writer.WriteBBString(account.Password);
                Writer.WriteBBString(account.Email);
                Writer.Write(account.IsGM);
                Writer.Write(account.IsBanned);

                // Done
                Writer.Close();
            }

            completionDelegate.Invoke(account, true);
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

            // Attempt to read account
            using (BBBinaryReader Reader = new BBBinaryReader(File.OpenRead(Path.Combine(FilePath, account.Username + ".dat"))))
            {

                // Read account header
                Reader.ReadBBString(); // Password
                Reader.ReadBBString(); // Email
                Reader.ReadBoolean(); // GM
                Reader.ReadBoolean(); // Banned

                // Setup output
                LoadCompleteEventArgs Args = new LoadCompleteEventArgs(account);

                // Read in actor data
                while (!Reader.Eof)
                {
                    int ActorLength = Reader.ReadInt32();
                    byte[] SerializedAI = Reader.ReadBytes(ActorLength);

                    int ScriptsLength = Reader.ReadInt32();
                    byte[] SerializedScripts = Reader.ReadBytes(ScriptsLength);

                    Args.ActorInstances.Add(new ActorInstanceData(SerializedAI, SerializedScripts));
                }

                // Close reader
                Reader.Close();

                // Run completion callback
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
            List<ActorInstanceData> ActorInstances = new List<ActorInstanceData>();

            // Read existing data (so we can insert this actor instance)
            // Attempt to read account
            using (BBBinaryReader Reader = new BBBinaryReader(File.OpenRead(Path.Combine(FilePath, account.Username + ".dat"))))
            {

                // Read account header
                Reader.ReadBBString(); // Password
                Reader.ReadBBString(); // Email
                Reader.ReadBoolean(); // GM
                Reader.ReadBoolean(); // Banned

                // Read in actor data
                while (!Reader.Eof)
                {
                    int ActorLength = Reader.ReadInt32();
                    byte[] SerializedAI = Reader.ReadBytes(ActorLength);

                    int ScriptsLength = Reader.ReadInt32();
                    byte[] SerializedScripts = Reader.ReadBytes(ScriptsLength);

                    ActorInstances.Add(new ActorInstanceData(SerializedAI, SerializedScripts));
                }

                // Close reader
                Reader.Close();
            }

            // Replace existing actor data with our new data
            if (index >= ActorInstances.Count)
            {
                ActorInstances.Add(data);

                if (data != null)
                {
                    ActorInstance AI = new ActorInstance();
                    AI.AccountName = account.Username;
                    AI.Banned = account.IsBanned;
                    AI.GM = account.IsGM;
                    AI.ID = ActorInstances.Count - 1;

                    // The serialized actor data begins with a 32-bit ID followed by a namestring with an 8-bit length
                    byte Length = data.SerializedAI[4];
                    AI.Name = ASCIIEncoding.ASCII.GetString(data.SerializedAI, 5, Length);

                    SearchDB.AddLast(AI);
                }
            }
            else
            {
                ActorInstances[index] = data;

                LinkedListNode<ActorInstance> AINode = SearchDB.First;
                while (AINode != null)
                {
                    ActorInstance AI = AINode.Value;

                    if (AI.ID == index && AI.AccountName == account.Username)
                    {
                        if (data == null)
                        {
                            SearchDB.Remove(AINode);
                            break;
                        }
                        else
                        {
                            // The serialized actor data begins with a 32-bit ID followed by a namestring with an 8-bit length
                            byte Length = data.SerializedAI[4];
                            string Name = ASCIIEncoding.ASCII.GetString(data.SerializedAI, 5, Length);

                            AI.Name = Name;
                            AI.Banned = account.IsBanned;
                            AI.GM = account.IsGM;
                            break;
                        }
                    }

                    AINode = AINode.Next;
                }
            }

            // Attempt to write file
            using (BBBinaryWriter Writer = new BBBinaryWriter(File.Open(Path.Combine(FilePath, account.Username + ".dat"), FileMode.Create)))
            {

                // Write account header
                Writer.WriteBBString(account.Password);
                Writer.WriteBBString(account.Email);
                Writer.Write(account.IsGM);
                Writer.Write(account.IsBanned);

                // Actor data
                foreach (ActorInstanceData AID in ActorInstances)
                {
                    // Input 'data' will be null on deletion, so we skip that when writing.
                    if (AID != null)
                    {
                        Writer.Write(AID.SerializedAI.Length);
                        Writer.Write(AID.SerializedAI);

                        Writer.Write(AID.SerializedScripts.Length);
                        Writer.Write(AID.SerializedScripts);
                    }
                }

                // Done
                Writer.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {

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