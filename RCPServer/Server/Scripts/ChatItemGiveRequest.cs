//
// This source file is courtesy of the RCP MMO Kit
// https://bitbucket.org/Siroro/rcpkit/wiki/Home
//
using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

class ChatItemGiveRequest : ScriptBase
{
    ActorInfoRequest ItemRequest;
    Actor Player;
    string TargetName;
    string ItemName;
    int Amount;
    public void GivePlayerItem(Actor player, string targetName, string itemName, int amount)
    {
        Player = player;
        TargetName = targetName;
        ItemName = itemName;
        Amount = amount;
        ItemRequest = new ActorInfoRequest(targetName, ActorInfoRequestState.Online);
        RegisterCallback(ItemRequest, "Complete", new ActorInfoRequest.RequestHandler(ItemRequest_Complete));
        RCScript.PostActorInfoRequest(ItemRequest);
    }



    private void ItemRequest_Complete(ActorInfo actorInfo)
    {
        if (actorInfo.State == ActorInfoState.Online)
        {
            Actor Player2 = actorInfo.Handle;
            Player2.GiveItem(ItemName, Amount);
        }
        else
        {
            Player.Output(TargetName + " is not online");
        }
        UnRegisterCallback(ItemRequest, "Complete", new ActorInfoRequest.RequestHandler(ItemRequest_Complete));
    }
}
