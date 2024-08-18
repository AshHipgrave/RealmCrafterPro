using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public class Inventory
    {
        ItemInstance[] items = new ItemInstance[50];
        int[] amounts = new int[50];

        public ItemInstance[] Items
        {
            get { return items; }
        }

        public int[] Amounts
        {
            get { return amounts; }
        }

        public Inventory()
        {
            for (int i = 0; i < items.Length; ++i)
            {
                items[i] = null;
                amounts[i] = 0;
            }
        }

        public void Serialize(Scripting.PacketWriter Pa)
        {
            if (items.Length != amounts.Length)
                throw new Exception("Critical Error: Inventory Items and Amounts Length do not match!");
            if (items.Length > 255)
                throw new Exception("Inventory.items.Length uses an incorrect type!");

            Pa.Write((byte)items.Length);
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i] == null)
                {
                    Pa.Write((byte)0);
                    continue;
                }
                else
                {
                    Pa.Write((byte)1);
                    items[i].Serialize(Pa);
                }
            }

            for (int i = 0; i < items.Length; ++i)
            {
                Pa.Write(amounts[i]);
            }
        }

        public void Deserialize(Scripting.PacketReader Pa, Scripting.PacketWriter ReAllocator)
        {
            byte Length = Pa.ReadByte();
            if (Length != items.Length)
                throw new Exception("Critical Error: Packet Items Length did not match internal Length!");

            for (int i = 0; i < Length; ++i)
            {
                items[i] = ItemInstance.Deserialize(Pa, ReAllocator);
            }

            for (int i = 0; i < Length; ++i)
            {
                amounts[i] = Pa.ReadInt32();
            }
        }

        public ItemInstance Drop(byte slotFrom, int amount)
        {
        	// Check the drop is legal
	        if(slotFrom < 0 || slotFrom > 45)
                return null;

	        if(Amounts[slotFrom] < amount || Items[slotFrom] == null)
                return null;

	        // Remove from inventory
	        ItemInstance I = Items[slotFrom];
	        Amounts[slotFrom] = Amounts[slotFrom] - amount;
            if (Amounts[slotFrom] <= 0)
                Items[slotFrom] = null;

	        return I;
        }

        public bool Add(ActorInstance A, byte slotFrom, byte slotTo, int amount)
        {
            // Check the addition is legal
	        if(slotFrom < 0 || slotFrom > 45 || slotTo < (byte)ItemSlots.Backpack || slotTo > 45)
                return false;

	        if(Amounts[slotFrom] < 1 || Items[slotFrom] == null)
                return false;

	        if(Items[slotTo] == null)
                return false;

	        if(A.Actor.HasSlot(slotFrom, Items[slotFrom].Item) == false)
		        return false;
	        else if(A.Actor.HasSlot(slotTo, Items[slotTo].Item) == false)
		        return false;

            if (Items[slotFrom].Identical(Items[slotTo]) == false)
                return false;

	        // Do it
	        Amounts[slotTo] = Amounts[slotTo] + amount;
	        Amounts[slotFrom] = Amounts[slotFrom] - amount;
	        if(Amounts[slotFrom] <= 0)
                Items[slotFrom] = null;

	        return true;
        }

        public bool Swap(ActorInstance A, byte slotA, byte slotB, int amount)
        {
            // Check the swap is legal
	        if(slotB < (byte)ItemSlots.Backpack && amount > 1)
                return false;

	        if(slotA < 0 || slotA > 45 || slotB < 0 || slotB > 45)
                return false;

	        if(Items[slotA] == null)
                return false;

	        Item I = Items[slotA].Item;
	        if(A.Actor.HasSlot(slotA, I) == false || A.Actor.HasSlot(slotB, I) == false)
                return false;
	        if(Items[slotA].Item.SlotsMatch(slotB) == false)
                return false;
	        if(Items[slotB] != null)
		        if(Items[slotB].Item.SlotsMatch(slotA) == false)
                    return false;

	        // Swap them
	        if(amount == 0 || Items[slotB] != null)
            {
		        // Do not allow multiple stacked items to go into a non backpack slot
		        if((slotA < (byte)ItemSlots.Backpack && Amounts[slotB] > 1) || (slotB < (byte)ItemSlots.Backpack && Amounts[slotA] > 1))
			        return false;

		        ItemInstance ItemA = Items[slotA];
		        int AmountA = Amounts[slotA];
		        Items[slotA] = Items[slotB];
		        Amounts[slotA] = Amounts[slotB];
		        Items[slotB] = ItemA;
		        Amounts[slotB] = AmountA;
	        
	        }else // Move a certain amount only
            {
		        Amounts[slotB] = amount;
		        Amounts[slotA] = Amounts[slotA] - amount;
		        if(Amounts[slotA] < 1)
                {
			        Items[slotB] = Items[slotA];
			        Items[slotA] = null;
		        }else
                {
			        Items[slotB] = Items[slotA].CopyItemInstance();
		        }
	        }

	        return true;
        }

        public int HasItem(string itemName)
        {
            int FoundAmount = 0;
            for (int i = 0; i < 45; ++i)
            {
                if (Items[i] != null)
                {
                    if (Items[i].Item.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        FoundAmount += Amounts[i];
                    }
                }
            }

            return FoundAmount;
        }

        public int HasItem(ushort itemID)
        {
            int FoundAmount = 0;
            for (int i = 0; i < 45; ++i)
            {
                if (Items[i] != null)
                {
                    if (Items[i].ID == itemID)
                    {
                        FoundAmount += Amounts[i];
                    }
                }
            }

            return FoundAmount;
        }

        public int GetArmourLevel()
        {
            int AP = 0;
            for(int i = (int)ItemSlots.Shield; i <= (int)ItemSlots.Feet; ++i)
            {
		        if(Items[i] != null)
                {
			        if(Items[i].Item.ItemType == ItemTypes.I_Armour)
                    {
                        if (Items[i].ItemHealth > 0)
                            AP += Items[i].Item.ArmourLevel;
			        }
		        }
	        }
	        
            return AP;
        }
    }
}
