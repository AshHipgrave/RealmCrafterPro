//
// This source file is courtesy of the RCP MMO Kit
// https://bitbucket.org/Siroro/rcpkit/wiki/Home
//
using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

class ChatWhisperRequest : ScriptBase
{
    Actor From;
    string Message;
    String To;
    ActorInfoRequest Request;

    /// <summary>
    /// Send a whisper to a player.
    /// </summary>
    public void SendWhisper(Actor from, string to, string message)
    {
        From = from;
        Message = message;
        To = to;

        // Setup request (search for online only)
        Request = new ActorInfoRequest(to, ActorInfoRequestState.All);
        RegisterCallback(Request, "Complete", new ActorInfoRequest.RequestHandler(Request_Complete));

        // Post request
        RCScript.PostActorInfoRequest(Request);
    }

    /// <summary>
    /// Called when the request is completed
    /// </summary>
    public void Request_Complete(ActorInfo actorInfo)
    {
        RCScript.Log(actorInfo.State.ToString());
        // Always check the state
        if (actorInfo.State == ActorInfoState.Online)
        {
            From.Output(0, "To [" + actorInfo.Name + "]: " + Message, System.Drawing.Color.Magenta);
            actorInfo.Output(0, "[" + From.Name + "] whispers: " + Message, System.Drawing.Color.Magenta);
        }
        else
        {
            From.Output(0, To + " is currently offline");
        }

        // Cleanup
        UnRegisterCallback(Request, "Complete", new ActorInfoRequest.RequestHandler(Request_Complete));
    }
}



