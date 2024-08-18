using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Forms;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Form for showing the owner of a scenery object
    /// </summary>
    public partial class SceneryTradeOwner : Form
    {
        SceneryInstance SceneryObject;
        int Cost;

        public SceneryTradeOwner()
        {
            InitializeComponent();
        }

        // Called from the OnInteract script to setup the window display
        public void SetupWindow(SceneryInstance sceneryObject, int cost)
        {
            SceneryObject = sceneryObject;

            // Check globals
            CheckGlobals();

            // Setup cost
            Cost = cost;
            CostLabel.Text = Cost.ToString();

            // Setup owner
            PacketReader Reader = new PacketReader(sceneryObject.Globals["Owner"]);
            string OwnerName = Reader.ReadString();

            // If there is an owner, write it and disable 'buy'
            if (OwnerName.Length > 0)
            {
                OwnerLabel.Text = OwnerName;
                BuyButton.Enabled = false;
            }
            else
            {
                OwnerLabel.Text = "None";
                BuyButton.Enabled = true;
            }
        }

        // Check that the correct globals exist in a readable format
        private void CheckGlobals()
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
        
        // Player clicked the X
        private void form_Closed(object sender, FormEventArgs e)
        {
            Actor.CloseDialog(this);
        }

        // Player clicked 'Close'
        private void CancelButton_Click(object sender, FormEventArgs e)
        {
            Actor.CloseDialog(this);
        }
        
        // Player wants to buy
        private void BuyButton_Click(object sender, FormEventArgs e)
        {
            // Check to make sure user isn't exploiting SDK
            if (!BuyButton.Enabled)
                return;

            // Can the actor afford it?
            if (Actor.Money >= Cost)
            {
                // Take money
                Actor.Money -= Cost;

                // The 'AccountName' just makes it more difficult to steal in case
                // a database implementation or other script exploit allows the user
                // to share the same name as someone else.
                PacketWriter NewOwner = new PacketWriter();
                NewOwner.Write(Actor.Name, true);
                NewOwner.Write(Actor.AccountName, true);

                // Save
                SceneryObject.Globals["Owner"] = NewOwner.ToArray();

                // Close window
                Actor.CloseDialog(this);
            }
            else
            {
                Actor.Output("You are too poor to buy this chest!", System.Drawing.Color.Red);
            }
        }
        
        
        
    }
}
