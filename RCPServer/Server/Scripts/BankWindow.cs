using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Forms;
using Scripting;
using Scripting.Math;
namespace UserScripts
{
    /// <summary>
    /// Bank Script
    /// By Ben Johnson
    /// Based on SceneryTradePublic by Jared Belkus
    /// Last Majorly Edited 7/12/10
    /// </summary>
    public partial class BankWindow : Form
    {
        // Tweakables
        const int Max_Slots_Width = 20;
        const int Max_Slots_Height = 8;
        const int Slot_Size = 42;
        const int Slot_Padding = 6;
        const int Page_Slots_Width = 10;
        const int Page_Slots_Height = 4;

        public const string BankWindow_Name = "BankWindow";

        // Global names  - change these if they clash with another script
        const string Bank_Name = "Bank";
        const string Bank_Money = "Bank_Money";
        const ushort EmptyItem = 65535;
        // Do not change
        const int Bank_Byte_Size = ((Max_Slots_Width * Max_Slots_Height) * 4) * 2;
        const int Max_Pages = (Max_Slots_Width / Page_Slots_Width);

        private ItemButton[] Slots;

        private uint Money;
        private int CurrentPage;


        public BankWindow(Actor actor)
        {
            CheckBankGlobal(actor);
            CurrentPage = 0;

            // Read money amount stored in bank
            PacketReader Reader = new PacketReader(actor.Globals[Bank_Money]);
            Money = Reader.PeekUInt32();

            // Create reader to read through all item slots
            // Create item slots
            Slots = new ItemButton[Page_Slots_Width * Page_Slots_Height];
            int xOffset = 6;
            int yOffset = 72;
            int indexCount = 0;
            int xPos = xOffset;

            // NOTE - needs to be done before InitializeComponent() otherwise GUI components won't appear
            for (int x = 0; x < Page_Slots_Width; x++)
            {
                int yPos = yOffset;
                for (int y = 0; y < Page_Slots_Height; y++)
                {
                    ItemButton itemButton = new ItemButton();
                    // Setup size and positioning and other attributes
                    itemButton.Location = new Vector2(xPos, yPos);
                    itemButton.PositionType = PositionType.Absolute;
                    itemButton.Size = new Vector2(Slot_Size, Slot_Size);
                    itemButton.SizeType = SizeType.Absolute;
                    itemButton.Name = "itemButton" + indexCount;
                    itemButton.Text = "button";
                    itemButton.CanDragToInventory = true;
                    itemButton.CanDragFromInventory = true;


                    // Setup event handlers
                    itemButton.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
                    itemButton.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
                    itemButton.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);

                    // Add to array and this forms controls
                    Slots[indexCount] = itemButton;
                    this.Controls.Add(Slots[indexCount]);
                    yPos += Slot_Size + Slot_Padding;
                    indexCount++;
                }

                xPos += Slot_Size + Slot_Padding;
            }


            InitializeComponent();
            Populate_Page(actor);

            Name = BankWindow_Name;

            MoneyCount.Text = "" + Money;

            base.Closed += Closed;
        }

        private void Closed(object o, EventArgs args)
        {
            // Close dialog for actor if closed button is pressed
            Actor.CloseDialog(this);
        }

        private void Populate_Page(Actor actor)
        {
            PageLabel.Text = "Page " + (CurrentPage + 1) + "/" + Max_Pages;


            PacketReader reader = new PacketReader(actor.Globals[Bank_Name]);
            // Jump to items for this page
            // Reader.Location =  (((Page_Slots_Width  * 4) * 2) * ((Page_Slots_Height * 4) * 2)) * CurrentPage;
            for (int i = 0; i < (Page_Slots_Width * Page_Slots_Height) * CurrentPage; i++)
            {
                reader.ReadUInt16();
                reader.ReadUInt16();
            }

            // Read item IDs 
            for (int i = 0; i < Page_Slots_Width * Page_Slots_Height; i++)
            {
                // Check to make sure not gone over bank max
                if (reader.Location < reader.Length)
                {
                    Slots[i].Enabled = true;
                    Slots[i].ItemID = reader.ReadUInt16();
                    Slots[i].ItemAmount = reader.ReadUInt16();
                }
                else
                {
                    // if not full page of slots disable those that cannot be used
                    Slots[i].Enabled = false;
                    Slots[i].ItemID = EmptyItem;
                    Slots[i].ItemAmount = 0;
                }

            }

        }


        // Player right clicked his slot (removes the item)
        private void Item_RightClick(object sender, FormEventArgs e)
        {
            // Quick sanity check
            if (sender == null || !(sender is ItemButton))
                return;

            CheckBankGlobal(Actor);

            // Get handle and index
            ItemButton ClickedButton = sender as ItemButton;
            if (ClickedButton.ItemID == EmptyItem)
                return;


            // Add item to player inventory
            Actor.GiveItem(ClickedButton.ItemID, ClickedButton.ItemAmount);

            // Remove from Bank
            // Rather than just filtering through a packet reader
            // its easier just to manually change the byte values for removals
            int Index = GetSlotIndex(ClickedButton);
            byte[] RawInventory = Actor.Globals[Bank_Name];
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

            CheckBankGlobal(Actor);

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

            // Give to player global bank
            int Index = GetSlotIndex(ClickedButton);

            PacketWriter Bank = new PacketWriter();
            PacketReader Reader = new PacketReader(Actor.Globals[Bank_Name]);

            // Copy
            for (int i = 0; i < Max_Slots_Width * Max_Slots_Height; ++i)
            {
                if (i == Index)
                {
                    Bank.Write(ClickedButton.ItemID);
                    Bank.Write(ClickedButton.ItemAmount);
                    Reader.ReadUInt16();
                    Reader.ReadUInt16();
                }
                else
                {
                    Bank.Write(Reader.ReadUInt16());
                    Bank.Write(Reader.ReadUInt16());
                }
            }

            Actor.Globals[Bank_Name] = Bank.ToArray();
        }


        int GetSlotIndex(ItemButton item)
        {
            // Get index of Item slot in Bank array(NOT SLOT ARRAY because its offset by page)
            for (int i = 0; i < Page_Slots_Width * Page_Slots_Height; i++)
            {
                if (Slots[i] == item)
                    return ((Page_Slots_Width * Page_Slots_Height) * CurrentPage) + i;
            }
            return 0;
        }

        void CheckBankGlobal(Actor actor)
        {
            // Create bank global if it does not exist
            if (!actor.Globals.ContainsKey(Bank_Name))
            {
                // Fill with empty items
                PacketWriter writer = new PacketWriter();
                for (int i = 0; i < Max_Slots_Width * Max_Slots_Height; i++)
                {
                    writer.Write(EmptyItem);
                    writer.Write((ushort)0);
                }

                actor.Globals.Add(Bank_Name, writer.ToArray());
            }
            else if (actor.Globals[Bank_Name].Length != Bank_Byte_Size)
            {
                // Bank size has been altered - change players global to match it and retain items
                PacketReader reader = new PacketReader(actor.Globals[Bank_Name]);
                PacketWriter writer = new PacketWriter();
                for (int i = 0; i < Max_Slots_Width * Max_Slots_Height; i++)
                {
                    if (reader.Location < reader.Length)
                    {
                        // Copy old bank in
                        writer.Write(reader.ReadUInt16());
                        writer.Write(reader.ReadUInt16());
                    }
                    else
                    {
                        // fill empty slot
                        writer.Write(EmptyItem);
                        writer.Write((ushort)0);
                    }
                }

                actor.Globals[Bank_Name] = writer.ToArray();

            }

            // Check Bank_Money global exists
            if (!actor.Globals.ContainsKey(Bank_Money))
            {
                // Make global and put 0 in it, this could be used to store multiple currencies
                // by adding more values if you wanted
                PacketWriter writer = new PacketWriter();
                writer.Write((uint)0);

                actor.Globals.Add(Bank_Money, writer.ToArray());
            }

        }

        private void WithdrawMoney(object sender, FormEventArgs e)
        {
            // Make sure user input is numeric
            uint value = 0;
            if (uint.TryParse(MoneyAmount.Text, out value))
            {
                // Make sure user number is less than or equal to actual money in bank
                if (value <= Money)
                {
                    Money -= value;
                    Actor.Money += (int)value;
                    // Update global
                    PacketWriter writer = new PacketWriter();
                    writer.Write((uint)Money);
                    Actor.Globals[Bank_Money] = writer.ToArray();

                    // Update label
                    MoneyCount.Text = "" + Money;
                }
            }
            else
            {
                // Show an error or alert of some kind here if
                // user input is not numeric 

            }

            MoneyAmount.Text = "0";
        }

        private void DepositMoney(object sender, FormEventArgs e)
        {
            // Make sure user input is numeric
            uint value = 0;
            if (uint.TryParse(MoneyAmount.Text, out value))
            {
                // Make sure user number is less than or equal to actual money in actors inventory
                if (value <= Actor.Money)
                {
                    Money += value;
                    Actor.Money -= (int)value;
                    // Update global
                    PacketWriter writer = new PacketWriter();
                    writer.Write((uint)Money);
                    Actor.Globals[Bank_Money] = writer.ToArray();

                    // Update label
                    MoneyCount.Text = "" + Money;
                }
            }
            else
            {
                // Show an error or alert of some kind here if
                // user input is not numeric 

            }

            MoneyAmount.Text = "0";
        }

        private void Previous_Click(object sender, FormEventArgs e)
        {
            CurrentPage--;
            if (CurrentPage < 0)
                CurrentPage = 0;
            else
                Populate_Page(Actor);
        }

        private void Next_Click(object sender, FormEventArgs e)
        {
            CurrentPage++;
            if (CurrentPage >= Max_Pages)
                CurrentPage = Max_Pages - 1;
            else
                Populate_Page(Actor);
        }
       

    }
}