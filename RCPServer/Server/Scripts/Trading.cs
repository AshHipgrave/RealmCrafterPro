using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Script used to intialize trading
    /// </summary>
    public class Trading : Scripting.ScriptBase
    {
        // Requests used for searches
        ActorInfoRequest InviteRequest;
        ActorInfoRequest DenyRequest;
        ActorInfoRequest AcceptRequest;
        Actor Player;

        public void InviteTrade(Actor actor, string otherName)
        {
            Player = actor;

            // Find the other player
            InviteRequest = new ActorInfoRequest(otherName, ActorInfoRequestState.Online);
            RegisterCallback(InviteRequest, "Complete", new ActorInfoRequest.RequestHandler(InviteRequest_Complete));
            RCScript.PostActorInfoRequest(InviteRequest);
        }

        public void AcceptTrade(Actor actor, string data)
        {
            Player = actor;

            // Read in actor name
            PacketReader Reader = new PacketReader(Player.Globals["Trading"]);
            Reader.ReadByte();
            string ActorName = Reader.ReadString();

            // Find the other player
            AcceptRequest = new ActorInfoRequest(ActorName, ActorInfoRequestState.Online);
            RegisterCallback(AcceptRequest, "Complete", new ActorInfoRequest.RequestHandler(AcceptRequest_Complete));
            RCScript.PostActorInfoRequest(AcceptRequest);
        }

        public void DenyTrade(Actor actor, string data)
        {
            Player = actor;

            // Read in actor name
            PacketReader Reader = new PacketReader(Player.Globals["Trading"]);
            Reader.ReadByte();
            string ActorName = Reader.ReadString();

            // Find the other player
            DenyRequest = new ActorInfoRequest(ActorName, ActorInfoRequestState.Online);
            RegisterCallback(DenyRequest, "Complete", new ActorInfoRequest.RequestHandler(DenyRequest_Complete));
            RCScript.PostActorInfoRequest(DenyRequest);
        }

        public void InviteRequest_Complete(ActorInfo actorInfo)
        {
            // He must be online and 'local'
            if (actorInfo.State == ActorInfoState.Online && actorInfo.Handle != null)
            {
                Actor Player2 = actorInfo.Handle;
                if (!Player2.Globals.ContainsKey("Trading"))
                    Player2.Globals["Trading"] = new byte[0];

                // Must be in the same zone (Range check could be added)
                if (Player2.Zone == Player.Zone)
                {
                    // Double check that both players can trade
                    if ((Player.Globals["Trading"].Length == 0 || Player.Globals["Trading"][0] == 0)
                        && (Player2.Globals["Trading"].Length == 0 || Player2.Globals["Trading"][0] == 0))
                    {
                        // Store the invite in player2
                        PacketWriter P2Writer = new PacketWriter();
                        P2Writer.Write((byte)1);
                        P2Writer.Write(Player.Name, true);
                        Player2.Globals["Trading"] = P2Writer.ToArray();

                        // Send messages
                        Player2.Output(Player.Name + " has invite you to trade! /AcceptTrade or /DenyTrade");
                        Player.Output("Invited " + Player2.Name + " to trade.");
                    }
                    else
                    {
                        Player.Output(Player2.Name + " is already trading!");
                    }
                }
                else
                {
                    Player.Output(Player2.Name + " is in a different area to you!");
                }
            }
            else
            {
                if (actorInfo.State == ActorInfoState.Online)
                    Player.Output(actorInfo.Name + " is in a different area to you.");
                else
                    Player.Output(actorInfo.Name + " doesn't exist or if offline!");
            }

            // Cleanup
            UnRegisterCallback(InviteRequest, "Complete", new ActorInfoRequest.RequestHandler(InviteRequest_Complete));
        }

        public void AcceptRequest_Complete(ActorInfo actorInfo)
        {
            // He must be online and 'local'
            if (actorInfo.State == ActorInfoState.Online && actorInfo.Handle != null)
            {
                Actor Player2 = actorInfo.Handle;
                if (!Player2.Globals.ContainsKey("Trading"))
                    Player2.Globals["Trading"] = new byte[0];

                // Must be in the same zone (Range check could be added)
                if (Player2.Zone == Player.Zone)
                {
                    // Check if the other player is now trading (this is bad)
                    if (Player2.Globals["Trading"].Length > 0 && Player2.Globals["Trading"][0] > 0)
                    {
                        Player.Output(Player2.Name + " is now trading with someone else.");
                    }
                    else
                    {
                        // Yay, we can both trade!

                        // Store the trade in player
                        Player.Globals["Trading"][0] = 2;

                        // Store the trade in player2
                        PacketWriter P2Writer = new PacketWriter();
                        P2Writer.Write((byte)2);
                        P2Writer.Write(Player.Name, true);
                        Player2.Globals["Trading"] = P2Writer.ToArray();

                        // Create windows
                        TradeWindow P1Window = new TradeWindow();
                        TradeWindow P2Window = new TradeWindow();

                        P1Window.SetupWindow(Player2.Name, P2Window);
                        P2Window.SetupWindow(Player.Name, P1Window);

                        Player.CreateDialog(P1Window);
                        Player2.CreateDialog(P2Window);
                    }
                }
                else
                {
                    Player.Output("Unable to trade, one of you switched zones.");
                }
            }
            else
            {
                if (actorInfo.State == ActorInfoState.Online)
                    Player.Output("Unable to trade, one of you switched zones.");
                else
                    Player.Output(actorInfo.Name + " went offline!");
            }

            // Cleanup
            UnRegisterCallback(InviteRequest, "Complete", new ActorInfoRequest.RequestHandler(AcceptRequest_Complete));
        }

        public void DenyRequest_Complete(ActorInfo actorInfo)
        {
            // He must be online and 'local'
            if (actorInfo.State == ActorInfoState.Online)
                actorInfo.Output(0, Player.Name + " denied your request!");

            Player.Output("Denied trading request.");
            Player.Globals["Trading"] = new byte[0];

            // Cleanup
            UnRegisterCallback(InviteRequest, "Complete", new ActorInfoRequest.RequestHandler(DenyRequest_Complete));
        }


    }
}
