using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Forms;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Trading dialogs for sharing items and money between players.
    /// </summary>
    public partial class TradeWindow : Form
    {
        // Holds the handle of the form displayed to the other player
        TradeWindow OtherForm;

        // Put all the ItemButton objects into an array.
        // This could have been done in the first place but
        // it would mean that the UI couldn't be edited.
        ItemButton[] MyItems = new ItemButton[10];
        ItemButton[] TheirItems = new ItemButton[10];

        // The 'Money' TextBox object can't send events. So we periodically
        // need to read its property and display it on the other players
        // form.
        Timer TextBoxTimer;

        // If the player clicks "Trade" then this becomes true
        // unless he clicks it again to cancel it.
        // It works as a 'ready' status so that the trade won't
        // be completed until both parties have agree'd.
        bool FinishedTrading = false;
    
        public TradeWindow()
        {
            InitializeComponent();

            TextBoxTimer = new Timer(5000, true);
            TextBoxTimer.Tick += new Scripting.Forms.EventHandler(TextBoxTimer_Tick);
            TextBoxTimer.Start();

#region ItemsToArray
            // Put the GUI handles into arrays (for easier access)
            MyItems[0] = itemButton1;
            MyItems[1] = itemButton2;
            MyItems[2] = itemButton3;
            MyItems[3] = itemButton4;
            MyItems[4] = itemButton5;
            MyItems[5] = itemButton6;
            MyItems[6] = itemButton7;
            MyItems[7] = itemButton8;
            MyItems[8] = itemButton9;
            MyItems[9] = itemButton10;

            TheirItems[0] = itemButton11;
            TheirItems[1] = itemButton12;
            TheirItems[2] = itemButton13;
            TheirItems[3] = itemButton14;
            TheirItems[4] = itemButton15;
            TheirItems[5] = itemButton16;
            TheirItems[6] = itemButton17;
            TheirItems[7] = itemButton18;
            TheirItems[8] = itemButton19;
            TheirItems[9] = itemButton20;
#endregion
        }

        // Called after window is created, sets label data
        public void SetupWindow(string otherName, TradeWindow otherForm)
        {
            MoneyLabel.Text = "0";
            label3.Text = "Items " + otherName + " is Trading:";
            this.Text = "Trading With: " + otherName;
            OtherForm = otherForm;
        }

        // Five seconds passed since last textbox update
        void TextBoxTimer_Tick(object sender, FormEventArgs e)
        {
 	        if(OtherForm != null)
            {
                // Comparison saved a bit of bandwidth
                if(!OtherForm.MoneyLabel.Text.Equals(MoneyText.Text))
                    OtherForm.MoneyLabel.Text = MoneyText.Text;
            }
        }
        
        // Player clicked the corner X
        private void form_Closed(object sender, FormEventArgs e)
        {
            // CloseDialog calls Closing(false) which we use
            // to dispatch the final messages.
            Actor.CloseDialog(this);
        }

        // Player clicked 'Cancel'
        private void CancelButton_Click(object sender, FormEventArgs e)
        {
            // Its the same as closing
            form_Closed(sender, e);
        }

        // This method is called when the form is going to be destroyed.
        // If the actor is zoning, it will be re-created on the other end
        // which fails our purpose since the actor will be too far away
        // to realistically trade (though other games might allow this,
        // the default behaviour is to close the window).
        // Is is also important not to call Actor.CloseDialog() if forZoning
        // is false, since it will create an infinite loop!
        public override void Closing(bool forZoning)
        {
            // If the other form is open, it has to be closed
            // Note: References to OtherForm have to be removed
            // to prevent the cyclic reference.
            if (OtherForm != null)
            {
                // If we both completed, then its not cancelled
                if(FinishedTrading && OtherForm.FinishedTrading)
                    OtherForm.Actor.Output("Trade complete!");
                else
                    OtherForm.Actor.Output("Other cancelled trading!");
                OtherForm.OtherForm = null;
                OtherForm.Actor.CloseDialog(OtherForm);
                OtherForm.Actor.Globals["Trading"] = new byte[0];
                OtherForm = null;
            }

            // Notify and cleanup
            if(FinishedTrading && OtherForm.FinishedTrading)
                OtherForm.Actor.Output("Trade complete!");
            else
                OtherForm.Actor.Output("Cancelled trading!");
            Actor.Globals["Trading"] = new byte[0];
            TextBoxTimer.Stop();
            TextBoxTimer.Tick -= new Scripting.Forms.EventHandler(TextBoxTimer_Tick);

            // Close form if we're zoning
            if(forZoning)
                Actor.CloseDialog(this);
        }

        // Get an index from a button handle
        private int GetMySlotIndex(ItemButton button)
        {
            for(int i = 0; i < MyItems.Length; ++i)
                if(MyItems[i] == button)
                    return i;
            return 0;
        }
		
        // Player right clicked his slot (removes the item)
		private void Item_RightClick(object sender, FormEventArgs e)
		{
            // Quick sanity check
            if(sender == null || !(sender is ItemButton))
                return;

            // Get handle and index
		    ItemButton ClickedButton = sender as ItemButton;
            int ButtonIndex = GetMySlotIndex(ClickedButton);
            
            // Remove item
            ClickedButton.ItemID = 65535;
            ClickedButton.ItemAmount = 0;

            // Remove it on the other actors window
            if(OtherForm != null)
            {
                OtherForm.TheirItems[ButtonIndex].ItemID = 65535;
                OtherForm.TheirItems[ButtonIndex].ItemAmount = 0;
            }else
            {
                // Code shouldn't get to this point, but just in case
                Actor.CloseDialog(this);
            }
		}
		
        // Player dropped an item into a slot
		private void Item_DraggedFromInventory(object sender, FormEventArgs e)
		{
		    // Quick sanity check
            if(sender == null || !(sender is ItemButton))
                return;

            // Get handle and index
		    ItemButton ClickedButton = sender as ItemButton;
            int ButtonIndex = GetMySlotIndex(ClickedButton);

            // Set it on the other actors window
            if(OtherForm != null)
            {
                OtherForm.TheirItems[ButtonIndex].ItemID = ClickedButton.ItemID;
                OtherForm.TheirItems[ButtonIndex].ItemAmount = ClickedButton.ItemAmount;
            }else
            {
                // Code shouldn't get to this point, but just in case
                Actor.CloseDialog(this);
            }
		}
        
        // Player clicked 'Trade'
        private void TradeButton_Click(object sender, FormEventArgs e)
        {
            // He has already clicked it, cancel!
            if(FinishedTrading)
            {
                FinishedTrading = false;
                TradeButton.Text = "Trade";
                return;
            }else
            {
                // Set
                FinishedTrading = true;
                
                // Should get to this!
                if(OtherForm == null)
                {
                    Actor.CloseDialog(this);
                    return;
                }

                // Other player isn't done, wait.
                if(!OtherForm.FinishedTrading)
                {
                    TradeButton.Text = "Not Ready";
                    return;
                }

                // Other player is done, so carry out the trade.
                // My items first
                for(int i = 0; i < MyItems.Length; ++i)
                {
                    // Always carry out a HasItem check so prevent hackers
                    if(MyItems[i].ItemID != 65535 && MyItems[i].ItemAmount > 0 && Actor.HasItem(MyItems[i].ItemID) == MyItems[i].ItemAmount)
                    {
                        Actor.GiveItem(MyItems[i].ItemID, -MyItems[i].ItemAmount);
                        OtherForm.Actor.GiveItem(MyItems[i].ItemID, MyItems[i].ItemAmount);
                    }
                }

                // My money (and avoid invalid cast exception)
                try
                {
                    int GiveMoney = Convert.ToInt32(MoneyText.Text);

                    if(GiveMoney > Actor.Money)
                        GiveMoney = Actor.Money;

                    Actor.Money -= GiveMoney;
                    OtherForm.Actor.Money += GiveMoney;

                }catch(Exception)
                {
                }

                // Their items
                for(int i = 0; i < OtherForm.MyItems.Length; ++i)
                {
                    // Always carry out a HasItem check so prevent hackers
                    if(OtherForm.MyItems[i].ItemID != 65535 && OtherForm.MyItems[i].ItemAmount > 0 && Actor.HasItem(OtherForm.MyItems[i].ItemID) == OtherForm.MyItems[i].ItemAmount)
                    {
                        OtherForm.Actor.GiveItem(OtherForm.MyItems[i].ItemID, -OtherForm.MyItems[i].ItemAmount);
                        Actor.GiveItem(OtherForm.MyItems[i].ItemID, OtherForm.MyItems[i].ItemAmount);
                    }
                }

                // Their money (and avoid invalid cast exception)
                try
                {
                    int GiveMoney = Convert.ToInt32(OtherForm.MoneyText.Text);

                    if(GiveMoney > OtherForm.Actor.Money)
                        GiveMoney = OtherForm.Actor.Money;

                    OtherForm.Actor.Money -= GiveMoney;
                    Actor.Money += GiveMoney;

                }catch(Exception)
                {
                }

                // Trade complete!
                Actor.CloseDialog(this);
            }
        }
        
    }
}
