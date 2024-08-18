using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Script for tradable scenery objects (chests) which can be bought and owned.
    /// </summary>
    public class SceneryTradeOwnable : ScriptBase
    {
        public void OnInteract(Actor actor, SceneryInstance scenery)
        {
            // Check globals (for owner storage)
            CheckGlobals(scenery);

            // Is this player the owner?
            if (IsActorOwner(actor, scenery))
            {
                // Show him the trade window
                SceneryTradeWindow TradeForm = new SceneryTradeWindow();
                TradeForm.SetupWindow(scenery);
                actor.CreateDialog(TradeForm);
            }
            else
            {
                // Show him the owner information window
                SceneryTradeOwner OwnerForm = new SceneryTradeOwner();
                OwnerForm.SetupWindow(scenery, 10);
                actor.CreateDialog(OwnerForm);
            }
        }

        // Check if the given actor owns the scenery
        private bool IsActorOwner(Actor actor, SceneryInstance scenery)
        {
            // Read packet
            PacketReader Reader = new PacketReader(scenery.Globals["Owner"]);
            string ActorName = Reader.ReadString();
            string AccountName = Reader.ReadString();

            if (actor.Name.Equals(ActorName) && actor.AccountName.Equals(AccountName))
                return true;
            return false;
        }

        // Check that the correct globals exist in a readable format
        private void CheckGlobals(SceneryInstance SceneryObject)
        {
            if (!SceneryObject.Globals.ContainsKey("Owner")
                || SceneryObject.Globals["Owner"] == null
                || SceneryObject.Globals["Owner"].Length == 0)
            {
                PacketWriter Tmp = new PacketWriter();
                Tmp.Write("", true);
                Tmp.Write("", true);

                SceneryObject.Globals["Owner"] = Tmp.ToArray();
            }
        }
    }
}
