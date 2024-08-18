using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Scripting.Math;

namespace Scripting
{
    public class ChatProcessor : IChatProcessor
    {
        // Used to filter swearwords
        Regex[] FilterPatterns;

        /// <summary>
        /// Constructor
        /// </summary>
        public ChatProcessor()
        {
            // Create some wordfilters
            FilterPatterns = new Regex[2];
            FilterPatterns[0] = new Regex("([f|F][u|U][c|C][k|K])");
            FilterPatterns[1] = new Regex("([s|S][h|H][i|I][t|T])");
        }

        /// <summary>
        /// Process chat text from a user.
        /// </summary>
        /// <param name="actor">Player handle.</param>
        /// <param name="message">Players message.</param>
        public void OnChatText(Actor actor, string message)
        {
            // Clean up message
            for (int i = 0; i < FilterPatterns.Length; ++i)
                message = FilterPatterns[i].Replace(message, "****");

            // Broadcast message
            RCScript.DispatchMessage(actor, "<" + actor.Name + "> " + message, 0, 1);
        }

        /// <summary>
        /// Process a slash command from a user.
        /// </summary>
        /// <param name="actor">Player handle.</param>
        /// <param name="command">Uppercase commandname.</param>
        /// <param name="data">Command parameters</param>
        public void OnSlashCommand(Actor actor, string command, string data)
        {
            // Process commands

            // User is emoting
            if (command.Equals("me", StringComparison.CurrentCultureIgnoreCase))
            {
                RCScript.DispatchMessage(actor, "* " + actor.Name + " " + data, 0, 1);
            }

            // Send a message to everyone in the zone
            else if (command.Equals("yell", StringComparison.CurrentCultureIgnoreCase))
            {
                RCScript.DispatchMessage(actor, "<" + actor.Name + "> " + data, 0, 1000);
            }

            // Player is asking to trade or accepted a trade
            else if (command.Equals("trade", StringComparison.CurrentCultureIgnoreCase))
            {
                Trade(actor, command, data);
            }
            else if (command.Equals("accepttrade", StringComparison.CurrentCultureIgnoreCase))
            {
                AcceptTrade(actor, command, data);
            }
            else if (command.Equals("denytrade", StringComparison.CurrentCultureIgnoreCase))
            {
                DenyTrade(actor, command, data);
            }
            else if (command.Equals("testinst", StringComparison.CurrentCultureIgnoreCase))
            {
                TestInst(actor, command, data);
            }
            else if (command.Equals("testzone", StringComparison.CurrentCultureIgnoreCase))
            {
                TestZone(actor, command, data);
            }
            else if (command.Equals("twarp", StringComparison.CurrentCultureIgnoreCase))
            {
                actor.Warp(data, "Start");
            }
            else if (command.Equals("gmtest", StringComparison.CurrentCultureIgnoreCase))
            {
                if (actor.GM)
                    actor.Output("You are a GM!");
                else
                    actor.Output("You are not a GM!");
            }
            else if (command.Equals("gmswitch", StringComparison.CurrentCultureIgnoreCase))
            {
                actor.GM = !actor.GM;
                actor.Output("GM Switched");
            }
            else if (command.Equals("createparty", StringComparison.CurrentCultureIgnoreCase))
            {
                actor.CreateParty();

                actor.Output("You have created a party.");

            }
            else if (command.Equals("partyinvite", StringComparison.CurrentCultureIgnoreCase))
            {
                PartyInstance party = actor.GetCurrentParty();

                if (party != null)
                {
                    SectorVector position = actor.GetPosition();
                    ushort radius = 2;

                    // Clamp sector into ushort range 
                    ushort minX =  (ushort)(position.SectorX - radius);
                    ushort minZ = (ushort)(position.SectorZ - radius);
                    ushort maxX = (ushort)(position.SectorX + radius);
                    ushort maxZ = (ushort)(position.SectorZ + radius);

                if (minX != (int)(position.SectorX - radius))
                    minX = 0;

                if (minZ != (int)(position.SectorZ - radius))
                    minZ = 0;

                if (maxX != (int)(position.SectorX + radius))
                    maxX = 65535;

                if (maxZ != (int)(position.SectorZ + radius))
                    maxZ = 65535;
                     
                    // Send invite to relevant actor
                    LinkedList<Actor> actors = actor.Zone.GetActors(ActorType.Player, minX, minZ, maxX, maxZ);

                    foreach (Actor a in actors)
                        if (a.Human && a.Name == data && a.GetCurrentParty() != party)
                        {
                            // Actor with given name
                            UserScripts.PartyInvite window = new UserScripts.PartyInvite(actor);
                            a.CreateDialog(window);
                            break;
                        }      
                }
            }
            else if (command.Equals("leaveparty", StringComparison.CurrentCultureIgnoreCase))
            {
                PartyInstance party = actor.GetCurrentParty();

                if (party != null)
                {
                    List<Actor> members = party.GetCurrentMembers();
                    foreach (Actor a in members)
                        a.Output(actor.Name + " has left the party.");

                    actor.LeaveParty();
                }
            }
            else if (command.Equals("setupkillreq", StringComparison.CurrentCultureIgnoreCase))
            {
                actor.CreateWaitKillRequest("KillReqTest", "WaitKill", Actor.ActorID("Lizard Folk", "Fighter"), 1);
            }
            else
            {
                actor.Output("Unknown command: " + command);
                RCScript.Log("Unknown Command(" + command + ", " + actor.Name + ")");
            }
        }

        // Implementation of /trade
        private void Trade(Actor actor, string command, string data)
        {
            // If he has no trading global, make an empty one (as if trading was completed last time)
            if (!actor.Globals.ContainsKey("Trading"))
                actor.Globals["Trading"] = new byte[0];

            // Is the player OK to trade?
            if (actor.Globals["Trading"].Length == 0 || actor.Globals["Trading"][0] == 0)
            {
                // Launch trade script
                RCScript.Execute("Trading", "InviteTrade", new object[] { actor, data }, actor, null);
            }
        }

        // Implementation of /accepttrade
        private void AcceptTrade(Actor actor, string command, string data)
        {
            // If he has no trading global, make an empty one (as if trading was completed last time)
            if (!actor.Globals.ContainsKey("Trading"))
                actor.Globals["Trading"] = new byte[0];

            // Check if an accept is waiting
            if (actor.Globals["Trading"].Length > 0 && actor.Globals["Trading"][0] == 1)
            {
                // Launch trade script
                RCScript.Execute("Trading", "AcceptTrade", new object[] { actor, data }, actor, null);
            }
        }

        // Implementation of /denytrade
        private void DenyTrade(Actor actor, string command, string data)
        {
            // If he has no trading global, make an empty one (as if trading was completed last time)
            if (!actor.Globals.ContainsKey("Trading"))
                actor.Globals["Trading"] = new byte[0];

            // Check if an accept is waiting
            if (actor.Globals["Trading"].Length > 0 && actor.Globals["Trading"][0] == 1)
            {
                // Launch trade script
                RCScript.Execute("Trading", "DenyTrade", new object[] { actor, data }, actor, null);
            }
        }

        // My own testing slash command
        private void TestInst(Actor actor, string command, string data)
        {
            if (!RCScript.DelayedExecute("InstanceTest", "Main", new object[] { actor }, actor, null))
            {
                RCScript.Log("Couldn't find script!");
            }
        }

        private void TestZone(Actor actor, string command, string data)
        {
            string LogMsg = string.Format("{0} is in {1}:{2}", new object[] { actor.Name, actor.Zone.Name, actor.Zone.InstanceIndex });

            actor.Output(LogMsg, System.Drawing.Color.Yellow);
            RCScript.Log(LogMsg);
        }
    }
}
