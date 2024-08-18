using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Scripting;

namespace RCPServer
{
    public class Account
    {
        #region Member Variables
        ActorInstance[] character = new ActorInstance[10];
        PacketReader[] scripts = new PacketReader[10];
        List<string> ignore = new List<string>();
        AccountBase accountBase;
        uint loggedGCLID = 0;
        byte[] actorStat = new byte[0];
        byte[] zoneStat = new byte[0];
        #endregion

        #region Methods
        public Account(AccountBase setBase)
        {
            accountBase = setBase;
        }

        public byte[] ActorStat
        {
            get { return actorStat; }
            set { actorStat = value; }
        }
        
        public byte[] ZoneStat
        {
            get { return zoneStat; }
            set { zoneStat = value; }
        }

        public uint LoggedGCLID
        {
            get { return loggedGCLID; }
            set { loggedGCLID = value; }
        }

        public string Username
        {
            get { return accountBase.Username; }
        }

        public string Password
        {
            get { return accountBase.Password; }
        }

        public string Email
        {
            get { return accountBase.Email; }
        }

        public bool IsGM
        {
            get { return accountBase.IsGM; }
            set { accountBase.IsGM = value; }
        }

        public bool IsBanned
        {
            get { return accountBase.IsBanned; }
            set { accountBase.IsBanned = value; }
        }

        public AccountBase AccountBase
        {
            get { return accountBase; }
        }

        public ActorInstance[] Character
        {
            get { return character; }
        }

        public PacketReader[] Scripts
        {
            get { return scripts; }
        }
        #endregion

        public class WaitInstance
        {
            // List to store all instances
            static LinkedList<WaitInstance> Instances = new LinkedList<WaitInstance>();

            // Members
            AccountBase baseHandle;
            uint waitTime = 0;
            int waitPacket = 0;
            int connectionHandle = 0;
            uint gclid = 0;

            // Constructor
            public WaitInstance(AccountBase accountBase, int packetType, int connection, uint ingclid)
            {
                baseHandle = accountBase;
                waitPacket = packetType;
                waitTime = Server.MilliSecs();
                connectionHandle = connection;
                gclid = ingclid;

                Instances.AddLast(this);
            }
            
            // Get Connection Handle
            public int Connection
            {
                get { return connectionHandle; }
            }

            // Get GCLID
            public uint GCLID
            {
                get { return gclid; }
            }

            // Get packet type
            public int PacketType
            {
                get { return waitPacket; }
            }

            // Find an instance
            public static WaitInstance FindAndRemove(AccountBase handle)
            {
                LinkedListNode<WaitInstance> Node = Instances.First;

                while (Node != null)
                {
                    if (Node.Value.baseHandle == handle)
                    {
                        Instances.Remove(Node);
                        return Node.Value;
                    }

                    Node = Node.Next;
                }

                return null;
            }

            // Update all instances
            public static void Update()
            {
                LinkedListNode<WaitInstance> Node = Instances.First;

                while (Node != null)
                {
                    // Timeout
                    if (Node.Value.waitTime + 10000 < Server.MilliSecs())
                    {
                        PacketWriter Pa78 = new PacketWriter();
                        Pa78.Write(Node.Value.GCLID);
                        Pa78.Write((byte)78);

                        RCEnet.Send(Node.Value.Connection, Node.Value.PacketType, Pa78.ToArray(), true);

                        LinkedListNode<WaitInstance> Delete = Node;
                        Node = Node.Next;
                        Instances.Remove(Delete);
                    }
                    else
                    {
                        Node = Node.Next;
                    }
                }
            }
        }

        #region Static Variables
        public static List<Account> Accounts = new List<Account>();
        static Regex UserValidation = new Regex("^[0-9a-zA-Z_]");
        static Regex PassValidation = new Regex("^[0-9a-zA-Z._]");
        static Regex EmailValidation = new Regex("^[0-9a-zA-Z*+-.=_@]");
        #endregion

        #region Static Methods

        /// <summary>
        /// New account was added.
        /// 
        /// This is either called on startup (populate initial list); when an account
        /// is made in an external database system (via web e-commerce) or when a user
        /// clicks 'New Account' in game.
        /// </summary>
        /// <param name="baseHandle"></param>
        /// <param name="success"></param>
        public static void AddAccountCallback(AccountBase baseHandle, bool success)
        {
            if (success)
            {
                Account A = new Account(baseHandle);
                Accounts.Add(A);

                WaitInstance I = WaitInstance.FindAndRemove(baseHandle);
                if (I != null)
                {
                    PacketWriter Pa89 = new PacketWriter();
                    Pa89.Write(I.GCLID);
                    Pa89.Write((byte)89);

                    RCEnet.Send(I.Connection, MessageTypes.P_CreateAccount, Pa89.ToArray(), true);
                }
            }
            else
            {
                WaitInstance I = WaitInstance.FindAndRemove(baseHandle);
                if (I != null)
                {
                    PacketWriter Pa78 = new PacketWriter();
                    Pa78.Write(I.GCLID);
                    Pa78.Write((byte)78);

                    RCEnet.Send(I.Connection, MessageTypes.P_CreateAccount, Pa78.ToArray(), true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="actorName"></param>
        /// <param name="accountName"></param>
        /// <param name="isBanned"></param>
        /// <param name="isGM"></param>
        public static void OnActorInfoRequestCallback(ActorInfoRequest request, string actorName, string accountName, bool isBanned, bool isGM)
        {
            if (request == null)
                return;

            // Search failed
            if (string.IsNullOrEmpty(actorName))
            {
                PacketWriter Pa = new PacketWriter();
                Pa.Write((byte)'A'); // AccountServer
                Pa.Write((byte)'N'); // No
                Pa.Write(request.AllocID);

                if (Server.MasterConnection != 0)
                    RCEnet.Send(Server.MasterConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);
            }
            else
            {
                ActorInfo Info = ActorInfo.FromOffline(actorName, accountName, isBanned, isGM);
                PacketWriter Pa = new PacketWriter();
                Pa.Write((byte)'A'); // AccountServer
                Pa.Write((byte)'Y'); // Yes
                Pa.Write(request.AllocID);

                Info.Serialize(Pa);

                if (Server.MasterConnection != 0)
                    RCEnet.Send(Server.MasterConnection, MessageTypes.P_ActorInfoRequest, Pa.ToArray(), true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseHandle"></param>
        /// <param name="e"></param>
        public static void LoadAccountCallback(AccountBase baseHandle, LoadCompleteEventArgs e)
        {
            // Get 'master' account
            Account A = null;
            foreach (Account Ac in Accounts)
            {
                if (Ac.AccountBase == baseHandle)
                {
                    A = Ac;
                    break;
                }
            }

            if (A == null)
                return;

            // Get WaitInstance
            WaitInstance I = WaitInstance.FindAndRemove(baseHandle);

            // Setup actor data
            for (int i = 0; i < (e.ActorInstances.Count > 10 ? 10 : e.ActorInstances.Count); ++i)
            {
                PacketWriter ReAllocator = new PacketWriter();
                PacketReader Reader = new PacketReader(e.ActorInstances[i].SerializedAI);
                A.Character[i] = ActorInstance.CreateFromPacketReader(Reader, ReAllocator);
                A.Scripts[i] = new PacketReader(e.ActorInstances[i].SerializedScripts);
            }
            
            // Send list to client
            PacketWriter Pa = new PacketWriter();
            Pa.Write(I.GCLID);
            Pa.Write("Y", false);

            for (int i = 0; i < 10; ++i)
            {
                if (A.Character[i] != null)
                {
                    Pa.Write((byte)(A.Character[i].Name.Length));
                    Pa.Write(A.Character[i].Name, false);
                    Pa.Write((ushort)(A.Character[i].Actor.ID));
                    Pa.Write((byte)(A.Character[i].gender));
                    Pa.Write((byte)(A.Character[i].FaceTex));
                    Pa.Write((byte)(A.Character[i].Hair));
                    Pa.Write((byte)(A.Character[i].Beard));
                    Pa.Write((byte)(A.Character[i].BodyTex));
                }
            }

            RCEnet.Send(I.Connection, MessageTypes.P_VerifyAccount, Pa.ToArray(), true);
        }


        /// <summary>
        /// Reads accounts from the RCS accounts.dat file
        /// NOTE: Function must be removed from RCP server
        /// </summary>
        /// <param name="path">Path to accounts file</param>
        /// <returns>Number of accounts loaded</returns>
        //         public static int Load(string path)
        //         {
        //             // Remove existing accounts
        //             Accounts.Clear();
        // 
        //             BBBinaryReader F = null;
        // 
        //             try
        //             {
        //                 F = new BBBinaryReader(File.OpenRead(path));
        //             }
        //             catch (FileNotFoundException)
        //             {
        //                 return 0;
        //             }
        // 
        //             if (F.BaseStream.Length == 0)
        //                 return 0;
        // 
        //             while (!F.Eof)
        //             {
        //                 String Username = F.ReadBBString();
        //                 String Password = F.ReadBBString();
        //                 String Email = F.ReadBBString();
        // 
        //                 Account A = new Account(Username, Password, Email);
        //                 A.IsDM = F.ReadBoolean();
        //                 A.IsBanned = F.ReadBoolean();
        //                 string Ignore = F.ReadBBString();
        //                 string[] Ignores = new Regex("[,]").Split(Ignore);
        //                 A.Ignore.AddRange(Ignores);
        // 
        // 
        //                 int Characters = (int)(F.ReadByte());
        //                 for (int i = 0; i < Characters; ++i)
        //                 {
        //                     A.Character[i] = ReadActorInstance(F);
        //                     A.Character[i].accountIndex = i;
        // 
        //                     for (int j = 0; j < 500; ++j)
        //                     {
        //                         A.Character[i].QuestLog.EntryName[j] = F.ReadBBString();
        // 
        //                         int Length = F.ReadInt32();
        //                         byte[] Buffer = new byte[Length];
        //                         F.Read(Buffer, 0, Length);
        //                         A.Character[i].QuestLog.EntryStatus[j] = Buffer;
        //                     }
        // 
        //                     for(int j = 0; j < 36; ++j)
        //                         A.Character[i].ActionBar.Slots[j] = F.ReadBBString();
        // 
        // 				    if(A.Character[i] == null)
        // 				    {
        //                         A.Character[i].QuestLog = null;
        //                         A.Character[i].ActionBar = null;
        // 				    }else
        //                     {
        //                         A.Character[i].accountName = A.User;
        //                         A.Character[i].accountEmail = A.Email;
        //                         A.Character[i].accountBanned = A.IsBanned;
        //                         A.Character[i].accountDM = A.IsDM;
        //                         A.Character[i].Account = A;
        //                     }
        //                 }
        // 
        //                 Accounts.Add(A);
        //             }
        // 
        //             F.Close();
        // 
        //             return Accounts.Count;
        //         }
        // 
        //         public static void Save()
        //         {
        //             Log.WriteLine("Account.Save() Ignored");
        //             return;
        // 
        //             BBBinaryWriter F = new BBBinaryWriter(File.Open("Data/Server Data/Accounts.dat", FileMode.Create));
        // 
        //             foreach (Account A in Accounts)
        //             {
        //                 F.WriteBBString(A.User);
        //                 F.WriteBBString(A.Pass);
        //                 F.WriteBBString(A.Email);
        //                 F.Write(A.IsDM);
        //                 F.Write(A.IsBanned);
        // 
        //                 string Ignore = "";
        //                 foreach (string Is in A.Ignore)
        //                 {
        //                     Ignore += Is + ",";
        //                 }
        //                 Ignore = Ignore.Substring(0, Ignore.Length - 1);
        //                 F.WriteBBString(Ignore);
        // 
        //                 int Chars = 0;
        //                 for (int i = 0; i < 10; ++i)
        //                 {
        //                     if (A.Character[i] != null)
        //                         ++Chars;
        //                 }
        //                 F.Write((byte)(Chars));
        // 
        //                 for (int i = 0; i < 10; ++i)
        //                 {
        //                     if (A.Character[i] != null)
        //                     {
        //                         WriteActorInstance(F, A.Character[i]);
        //                         for (int j = 0; j < 500; ++j)
        //                         {
        //                             F.WriteBBString(A.Character[i].QuestLog.EntryName[j]);
        // 
        //                             if (A.Character[i].QuestLog.EntryStatus[j] == null)
        //                             {
        //                                 F.Write(0);
        //                             }
        //                             else
        //                             {
        //                                 F.Write(A.Character[i].QuestLog.EntryStatus[j].Length);
        //                                 F.Write(A.Character[i].QuestLog.EntryStatus[j]);
        //                             }
        //                         }
        //                         for(int j = 0; j < 36; ++j)
        //                             F.WriteBBString(A.Character[i].ActionBar.Slots[j]);
        //                     }
        //                 }
        // 
        //             }
        // 
        //             F.Close();
        //         }

        //         public static void Add(string user, string pass, string email)
        //         {
        //             // Create account
        // 
        //             Account A = new Account(user, pass, email);
        //             Accounts.Add(A);
        // 
        //             // Add to accounts file
        //             BBBinaryWriter F = new BBBinaryWriter(File.Open("Data/Server Data/Accounts.dar", FileMode.Append));
        //             F.WriteBBString(user);
        //             F.WriteBBString(pass);
        //             F.WriteBBString(email);
        //             F.Write((byte)(0));
        //             F.Close();
        //         }

        public static bool VerifyStrings(string user, string pass, string email)
        {
            AccountNameResult result = new AccountNameResult() { Result = true, Username = user };

             ScriptManager.ExecuteSpecialScriptObject(Server.AccountDatabase, "VerifyUsername",
                                        new object[] { result });

            bool Characters = (result.Result)
                && (PassValidation.IsMatch(pass))
                && (EmailValidation.IsMatch(email));

            bool Lengths = (user.Length < 51) && (pass.Length < 51) && (email.Length < 201);

            return Characters && Lengths;
        }

        public static Account Find(string name)
        {
            foreach (Account A in Account.Accounts)
            {
                if (A.Username.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return A;
            }

            return null;
        }

        public static Account FindFromGCLID(uint gclid)
        {
            foreach (Account A in Account.Accounts)
            {
                if (A.loggedGCLID == gclid)
                    return A;
            }

            return null;
        }

        public static bool ActorNameTaken(string name)
        {
            foreach (ActorInstance AI in ActorInstance.ActorInstanceList)
            {
                if (AI.RNID >= 0)
                    if (AI.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        return true;
            }

            return false;
        }

        public static ItemInstance ReadItemInstance(BBBinaryReader f)
        {
            ushort ID = f.ReadUInt16();
		    if(ID == 65535)
			    return null;

		    ItemInstance I = null;
		    if(Item.Items[ID] != null)
		    {
			    I = ItemInstance.CreateItemInstance(Item.Items[ID]);
			    for(int j = 0; j < 40; ++j)
                    I.Attributes.Value[j] = (int)(f.ReadUInt16()) - 5000;
			    I.ItemHealth = f.ReadByte();
		    }else
		    {
			    Log.WriteLine("Item not found: " + ID.ToString());
                f.BaseStream.Seek(81, SeekOrigin.Current);
		    }

		    return I;
        }

        public static void WriteItemInstance(BBBinaryWriter stream, ItemInstance i)
        {
            if (i == null)
            {
                stream.Write((ushort)(65535));
                return;
            }

            stream.Write((ushort)(i.Item.ID));
            for (int j = 0; j < 40; ++j)
                stream.Write((ushort)(i.Attributes.Value[j] + 5000));
            stream.Write((byte)(i.ItemHealth));
        }
        #endregion
    }
}
