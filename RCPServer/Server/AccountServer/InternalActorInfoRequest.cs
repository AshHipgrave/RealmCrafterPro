using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class InternalActorInfoRequest : Scripting.ActorInfoRequest
    {
        public InternalActorInfoRequest(string name, uint setAllocID)
            : base(name, Scripting.ActorInfoRequestState.All)
        {
            allocID = setAllocID;
        }
    }
}
