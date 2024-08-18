using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Scripting;

namespace RCPServer
{
    public class RCE_Message
    {
        public int Connection = 0;
        public int MessageType = 0;
        public PacketReader MessageData = null;
        public int FromID = 0;

        public RCE_Message()
        {
            RCE_MessageList.AddLast(this);
        }

        public static LinkedList<RCE_Message> RCE_MessageList = new LinkedList<RCE_Message>();
        public static LinkedList<RCE_Message> RCE_MessageDelete = new LinkedList<RCE_Message>();

        public static void Delete(RCE_Message item)
        {
            RCE_MessageDelete.AddLast(item);
        }

        public static void Clean()
        {
            foreach (RCE_Message item in RCE_MessageDelete)
            {
                RCE_MessageList.Remove(item);
            }

            RCE_MessageDelete.Clear();
        }
    }

    public class RCEnet
    {
        [DllImport("rcenet")]
        private static extern int RCE_StartHost(int localPort, string gameData, int maximumPlayers, string logFile, bool append);

        [DllImport("rcenet")]
        private static extern int RCE_Connect(string hostName, int hostPort, int myPort, string myName, string myData, string logFile, bool append);

        [DllImport("rcenet")]
        private static extern void RCE_Disconnect(int connection);

        [DllImport("rcenet")]
        private static extern void RCE_Update();

        [DllImport("rcenet")]
        private static extern void RCE_FSend(int Destination, int MessageType, byte[] MessageData, int ReliableFlag, int mLength);

        [DllImport("rcenet")]
        private static extern int RCE_MoveToFirstMessage();

        [DllImport("rcenet")]
        private static extern int RCE_AreMoreMessage();

        [DllImport("rcenet")]
        private static extern int RCE_GetMessageConnection();

        [DllImport("rcenet")]
        private static extern int RCE_GetConnectionID();

        [DllImport("rcenet")]
        private static extern int RCE_GetMessageType();

        [DllImport("rcenet")]
        private static extern void RCE_GetMessageData(byte[] messageData);

        [DllImport("rcenet")]
        private static extern int RCE_MessageLength();

        [DllImport("rcenet")]
        private static extern int RCE_LastDisconnectedPeer();

        // RCE_Connect errors
        public const int PortInUse = -1;
        public const int HostNotFound = -2;
        public const int TimedOut        = -3;
        public const int ServerFull      = -4;
        public const int ConnectionInUse = -5;

        public static string[] ConnectionErrors = new string[] { "Unknown", "Port In Use", "Host not Found", "Timed Out", "Server Full", "Connection In Use" };

        // Local message types for user
        public const int PlayerTimedOut     = 200;
        public const int PlayerHasLeft      = 201;
        public const int PlayerKicked       = 202;

        public static int Connect(string hostName, int hostPort)
        {
            int LocalPort = 11000;
            int Connection = 0;

            do
            {
                ++LocalPort;
                Connection = RCE_Connect(hostName, hostPort, LocalPort, "X", "", "", false);
            } while (LocalPort == ConnectionInUse || LocalPort == PortInUse);

            return Connection;
        }

        public static int StartHost(int port, int maximumPlayers)
        {
            return RCE_StartHost(port, "nothing", maximumPlayers, "", false);
        }

        public static void Disconnect(int connection)
        {
            RCE_Disconnect(connection);
        }

        public static void Update()
        {
            RCE_Update();

            if (RCE_MoveToFirstMessage() != 0)
            {
                do
                {
                    RCE_Message M = new RCE_Message();
                    M.Connection = RCE_GetConnectionID();
                    M.FromID = RCE_GetMessageConnection();
                    M.MessageType = RCE_GetMessageType();

                    int Length = RCE_MessageLength();

                    if (Length > 0)
                    {
                        byte[] MsgBank = new byte[Length];
                        RCE_GetMessageData(MsgBank);
                        M.MessageData = new PacketReader(MsgBank);
                    }
                } while (RCE_AreMoreMessage() != 0);
            }
        }

        public static void Send(int destination, int messageType, byte[] messageData, int a, int b)
        {
            Send(destination, messageType, messageData, false);
        }

        public static void Send(int destination, int messageType, byte[] messageData, bool reliable)
        {
            RCE_FSend(destination, messageType, messageData, reliable ? 1 : 0, messageData.Length);
        }

        public static int LastDisconnectedPeer()
        {
            return RCE_LastDisconnectedPeer();
        }
    }
}
