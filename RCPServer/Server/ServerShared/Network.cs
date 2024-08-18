using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class Network
    {
        protected delegate void SendQueuedDelegate(int destination, int packetType, byte[] packet, bool reliable);
        protected delegate void SendPartyUpdateDelegate(ActorInstance ai);

        protected static SendQueuedDelegate sendQueued;
        protected static SendPartyUpdateDelegate sendPartyUpdate;

        public static void SendQueued(int destination, int packetType, byte[] packet, bool reliable)
        {
            sendQueued(destination, packetType, packet, reliable);
        }

        public static void SendPartyUpdate(ActorInstance ai)
        {
            sendPartyUpdate(ai);
        }
    }
}
