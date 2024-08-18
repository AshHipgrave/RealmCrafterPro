using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public class Death : ScriptBase
    {
        static Random Rand = new Random();
        Actor Player;
        Scripting.Timer RespawnTimer;

        public void OnDeath(Actor actor, Actor killer)
        {
            // Inform player
            if (killer != null && !string.IsNullOrEmpty(killer.Name))
                if (killer.Human)
                    actor.Output("You were killed by: " + killer.Name);
                else
                    actor.Output("You were killed a " + killer.Race);
            else
                actor.Output("You have died...");

            // Play death animation
            int DeathAnim = Rand.Next(0, 3);
            if (DeathAnim == 0)
                actor.Animate("Death 1", 0.2f, false);
            else if (DeathAnim == 1)
                actor.Animate("Death 2", 0.2f, false);
            else
                actor.Animate("Death 3", 0.2f, false);

            actor.SetAttribute("Health", 0);

            // We have to wait now for a second so that the animation plays in full
            Player = actor;
            RespawnTimer = new Scripting.Timer(1000, false);
            RegisterCallback(RespawnTimer, "Tick", new Scripting.Forms.EventHandler(RespawnTimer_Tick));
            RespawnTimer.Start();

            Reward(actor, killer);

        }

        public void OnAIDeath(Actor actor, Actor killer)
        {
            // Play death animation
            int DeathAnim = Rand.Next(0, 3);
            if (DeathAnim == 0)
                actor.Animate("Death 1", 0.2f, false);
            else if (DeathAnim == 1)
                actor.Animate("Death 2", 0.2f, false);
            else
                actor.Animate("Death 3", 0.2f, false);

            actor.SetAttribute("Health", 0);

            Reward(actor, killer);
        }

        private void Reward(Actor actor, Actor killer)
        {
            // Apply kill rewards - make sure only give rewards to players
            if (killer != null ? killer.Human : false)
            {

                // Reduce faction rating if it isn't already at -100%
                int factionRating = killer.GetFactionRating(killer.HomeFaction) - 1;

                if (factionRating < 0)
                    factionRating = 0;

                killer.SetFactionRating(killer.HomeFaction, factionRating);

                // Give XP to the killer
                int Diff = actor.Level - killer.Level;
                if (Diff < 1)
                    Diff = 1;

                int XP = (Diff * actor.XPMultiplier) + Rand.Next(0, 20);
                killer.GiveXP(XP, true);
            }
        }

        private void RespawnTimer_Tick(object sender, Scripting.Forms.FormEventArgs e)
        {
            // Restore player health and remove money (can also remove previous items here)
            Player.SetAttribute("Health", Player.GetAttributeMax("Health"));
            Player.ChangeMoney(-10);

            // Warp back to start of zone (consider dedicated respawn points for larger zones).
            Player.Warp(Player.Zone.Name, "Start");

            // Clean up script code
            UnRegisterCallback(RespawnTimer, "Tick", new Scripting.Forms.EventHandler(RespawnTimer_Tick));
        }


    }
}
