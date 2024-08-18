using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Forms;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Enter description here
    /// </summary>
    public partial class SceneryTradeWindow : Form
    {
        // Array of slots
        ItemButton[] Slots;

        // Handle of scenery object
        SceneryInstance SceneryObject;
    
        public SceneryTradeWindow()
        {
            InitializeComponent();

            // The 'slots' array could be defined programatically by the
            // designer but it would make it impossible to use the form
            // editor. So build the slots array by hand instead.
            Slots = new ItemButton[5];
            Slots[0] = itemButton1;
            Slots[1] = itemButton2;
            Slots[2] = itemButton3;
            Slots[3] = itemButton4;
            Slots[4] = itemButton5;
        }

        // Called from the OnInteract script to setup the window display
        public void SetupWindow(SceneryInstance sceneryObject)
        {
            SceneryObject = sceneryObject;

            // Check globals
            CheckGlobals();

            // Setup initial values
            PacketReader Reader = new PacketReader(sceneryObject.Globals["Inventory"]);

            for (int i = 0; i < 5; ++i)
            {
                Slots[i].ItemID = Reader.ReadUInt16();
                Slots[i].ItemAmount = Reader.ReadUInt16();
            }
        }

        // Check that the correct globals exist in a readable format
        private void CheckGlobals()
        {
            if (!SceneryObject.Globals.ContainsKey("Inventory")
                || SceneryObject.Globals["Inventory"] == null
                || SceneryObject.Globals["Inventory"].Length == 0)
            {
                PacketWriter Tmp = new PacketWriter();

                for (int i = 0; i < 5; ++i)
                {
                    Tmp.Write((ushort)65535);
                    Tmp.Write((ushort)0);
                }

                SceneryObject.Globals["Inventory"] = Tmp.ToArray();
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

        // Called when form is closing
        public override void Closing(bool forZoning)
        {
            if (forZoning)
                Actor.CloseDialog(this);
        }

        // Get an index from a button handle
        private int GetSlotIndex(ItemButton button)
        {
            for (int i = 0; i < Slots.Length; ++i)
                if (Slots[i] == button)
                    return i;
            return 0;
        }
        
        // Player right clicked his slot (removes the item)
		private void Item_RightClick(object sender, FormEventArgs e)
        {
            // Quick sanity check
            if (sender == null || !(sender is ItemButton))
                return;

            // Get handle and index
            ItemButton ClickedButton = sender as ItemButton;

            // Add item to player inventory
            Actor.GiveItem(ClickedButton.ItemID, ClickedButton.ItemAmount);

            // Remove from Scenery inventory
            // Rather than just filtering through a packet reader
            // its easier just to manually change the byte values for removals
            int Index = GetSlotIndex(ClickedButton);
            byte[] RawInventory = SceneryObject.Globals["Inventory"];
            RawInventory[(Index * 4) + 0] = 0xff;
            RawInventory[(Index * 4) + 1] = 0xff;
            RawInventory[(Index * 4) + 2] = 0x0;
            RawInventory[(Index * 4) + 3] = 0x0;

            // Remove item
            ClickedButton.ItemID = 65535;
            ClickedButton.ItemAmount = 0;
        }
		
        // Player dropped an item into a slot
        private void Item_DraggedFromInventory(object sender, FormEventArgs e)
        {
            // Quick sanity check
            if (sender == null || !(sender is ItemButton))
                return;

            // Get handle and index
            ItemButton ClickedButton = sender as ItemButton;

            // Remove from player inventory (after check)
            if (Actor.HasItem(ClickedButton.ItemID) >= ClickedButton.ItemAmount)
            {
                Actor.GiveItem(ClickedButton.ItemID, -ClickedButton.ItemAmount);
            }
            else
            {
                return;
            }

            // Give to scenery inventory
            int Index = GetSlotIndex(ClickedButton);
            PacketWriter NewInventory = new PacketWriter();
            PacketReader Reader = new PacketReader(SceneryObject.Globals["Inventory"]);

            // Copy
            for (int i = 0; i < 5; ++i)
            {
                if (i == Index)
                {
                    NewInventory.Write(ClickedButton.ItemID);
                    NewInventory.Write(ClickedButton.ItemAmount);
                    Reader.ReadUInt16();
                    Reader.ReadUInt16();
                }
                else
                {
                    NewInventory.Write(Reader.ReadUInt16());
                    NewInventory.Write(Reader.ReadUInt16());
                }
            }

            SceneryObject.Globals["Inventory"] = NewInventory.ToArray();
        }
    }
}
