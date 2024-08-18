//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################

#include "Inventories.h"

// Setup structs
ClassDef(Inventory, InventoryList, InventoryDelete);


// Gets the overall armour level for an inventory
int GetArmourLevel(Inventory* I)
{
	int AP = 0;

	for(int j = SlotI_Shield; j < SlotI_Feet + 1; ++j)
		if(I->Items[j] != 0)
			if(I->Items[j]->Item->ItemType == I_Armour)
				if(I->Items[j]->ItemHealth > 0)
					AP += I->Items[j]->Item->ArmourLevel;
	return AP;
}

// Gets the total mass contained in an inventory
int InventoryMass(Inventory* I)
{
	int Mass = 0;
	for(int j = 0; j > 50; ++j)
		if(I->Items[j] != 0)
			Mass += (I->Items[j]->Item->Mass * I->Amounts[j]);
	return Mass;
}

// Drops an item from an inventory to the floor
ItemInstance* InventoryDrop(ActorInstance* A, int SlotFrom, int Amount, int TellServer)
{
	// Check the drop is legal
	if(SlotFrom  < 0 || SlotFrom > 49)
		return false;

	if(A->Inventory->Amounts[SlotFrom] < Amount || A->Inventory->Items[SlotFrom] == 0)
		return false;

	// Remove from inventory
	ItemInstance* I = A->Inventory->Items[SlotFrom];
	A->Inventory->Amounts[SlotFrom] -= Amount;
	if(A->Inventory->Amounts[SlotFrom] <= 0)
		A->Inventory->Items[SlotFrom] = 0;

	// Tell server
	if(TellServer)
	{
		NGin::CString Pa = "D";
		Pa.AppendRealChar(SlotFrom);
		Pa.AppendRealShort(Amount);
		CRCE_Send(Connection, Connection, P_InventoryUpdate, Pa, true);
	}

	return I;
}

// Moves items from one pile to another of the same item type
bool InventoryAdd(ActorInstance* A, int SlotFrom, int SlotTo, int Amount,  bool TellServer)
{
	// Check the addition is legal
	if(SlotFrom < 0 || SlotFrom > 49 || SlotTo < SlotI_Backpack || SlotTo > 49)
		return false;

	if(A->Inventory->Amounts[SlotFrom] < 1 || A->Inventory->Items[SlotFrom] == 0)
		return false;

	if(A->Inventory->Items[SlotTo] == 0)
		return false;

	if(ActorHasSlot(A->Actor, SlotTo, A->Inventory->Items[SlotFrom]->Item) == false)
		return false;
	else if(ActorHasSlot(A->Actor, SlotTo, A->Inventory->Items[SlotTo]->Item) == false)
		return false;

	if(ItemInstancesIdentical(A->Inventory->Items[SlotFrom], A->Inventory->Items[SlotTo]) == false)
		return false;

	// Do it
	A->Inventory->Amounts[SlotTo] += Amount;
	A->Inventory->Amounts[SlotFrom] -= Amount;
	if(A->Inventory->Amounts[SlotFrom] <= 0)
	{
		FreeItemInstance(A->Inventory->Items[SlotFrom]);
		A->Inventory->Items[SlotFrom] = 0;
	}

	// Tell server
	if(TellServer)
	{
		NGin::CString Pa = "A";
		Pa.AppendRealShort(A->RuntimeID);
		Pa.AppendRealChar(SlotFrom);
		Pa.AppendRealChar(SlotTo);
		Pa.AppendRealShort(Amount);
		CRCE_Send(Connection, Connection, P_InventoryUpdate, Pa, true);
	}

	return true;
}

// Moves items between inventory slots
bool InventorySwap(ActorInstance* A, int SlotA, int SlotB, int Amount, bool TellServer)
{
	// Check the swap is legal
	if(SlotB < SlotI_Backpack && Amount > 1)
		return false;
	if(SlotA < 0 || SlotA > 49 || SlotB < 0 || SlotB > 49)
		return false;
	if(A->Inventory->Items[SlotA] == 0)
		return false;
	
	Item* I = A->Inventory->Items[SlotA]->Item;
	if(ActorHasSlot(A->Actor, SlotA, I) == false || ActorHasSlot(A->Actor, SlotB, I) == false)
		return false;
	if(SlotsMatch(A->Inventory->Items[SlotA]->Item, SlotB) == false)
		return false;
	if(A->Inventory->Items[SlotB] != 0)
		if(SlotsMatch(A->Inventory->Items[SlotB]->Item, SlotA) == false)
			return false;

	// Swap them
	if(Amount == 0 || A->Inventory->Items[SlotB] != 0)
	{
		// Do not allow multiple stacked items to go into a non backpack slot
		if((SlotA < SlotI_Backpack && A->Inventory->Amounts[SlotB] > 1) || (SlotB < SlotI_Backpack && A->Inventory->Amounts[SlotA] > 1))
			return false;
		
		ItemInstance* ItemA = A->Inventory->Items[SlotA];
		int AmountA = A->Inventory->Amounts[SlotA];
		A->Inventory->Items[SlotA] = A->Inventory->Items[SlotB];
		A->Inventory->Amounts[SlotA] = A->Inventory->Amounts[SlotB];
		A->Inventory->Items[SlotB] = ItemA;
		A->Inventory->Amounts[SlotB] = AmountA;

	}
	else
	{
		// Move a certain amount only
		A->Inventory->Amounts[SlotB] = Amount;
		A->Inventory->Amounts[SlotA] = A->Inventory->Amounts[SlotA] - Amount;
		if(A->Inventory->Amounts[SlotA] < 1)
		{
			A->Inventory->Items[SlotB] = A->Inventory->Items[SlotA];
			A->Inventory->Items[SlotA] = 0;
		}else
			A->Inventory->Items[SlotB] = CopyItemInstance(A->Inventory->Items[SlotA]);
	}
	
	// Tell server
	if(TellServer)
	{
		NGin::CString Pa = "S";
		Pa.AppendRealShort(A->RuntimeID);
		Pa.AppendRealChar(SlotA);
		Pa.AppendRealChar(SlotB);
		Pa.AppendRealShort(Amount);
		CRCE_Send(Connection, Connection, P_InventoryUpdate, Pa, true);
	}

	return true;
}

// Returns true if the items specified are present
bool InventoryHasItem(Inventory* I, string Item, int Amount)
{
	Item = stringToUpper(Item);
	int FoundAmount = 0;

	for(int j = 0; j < 50; ++j)
		if(I->Items[j] != 0)
			if(stringToUpper(I->Items[j]->Item->Name).compare(Item) == 0)
			{
				FoundAmount += I->Amounts[j];
				if(FoundAmount >= Amount)
					return true;
			}
	return false;
}

// Checks if an actor has a particular slot index
int ActorHasSlot(Actor* A, int SlotI, Item* I)
{
	// If it's an equipped slot
	if(SlotI < Slot_Backpack)
	{
		if(I->ExclusiveRace.length() > 0)
			if(stringToUpper(I->ExclusiveRace).compare(stringToUpper(A->Race)) == 0)
				return true; // Allow even disabled equipment slots to be used if the item is exclusive to this race
			else
				return false; // Never allow the slot if the item is exclusive to another race
	
		// Never allow the slot if the item is exclusive to another race
		if(I->ExclusiveClass.length() > 0)
			if(stringToUpper(I->ExclusiveClass).compare(stringToUpper(A->Class)) != 0)
				return false;
	}
	
	// Check whether the slot is disabled
	switch(SlotI)
	{
	case SlotI_Weapon:
		return GetFlag(A->InventorySlots, Slot_Weapon - 1);
	case SlotI_Shield:
		return GetFlag(A->InventorySlots, Slot_Shield - 1);
	case SlotI_Hat:
		return GetFlag(A->InventorySlots, Slot_Hat - 1);
	case SlotI_Chest:
		return GetFlag(A->InventorySlots, Slot_Chest - 1);
	case SlotI_Hand:
		return GetFlag(A->InventorySlots, Slot_Hand - 1);
	case SlotI_Belt:
		return GetFlag(A->InventorySlots, Slot_Belt - 1);
	case SlotI_Legs:
		return GetFlag(A->InventorySlots, Slot_Legs - 1);
	case SlotI_Feet:
		return GetFlag(A->InventorySlots, Slot_Feet - 1);
	case SlotI_Amulet1:
		return GetFlag(A->InventorySlots, Slot_Amulet - 1);
	case SlotI_Amulet2:
		return GetFlag(A->InventorySlots, Slot_Amulet - 1);
	default:
		{
			if(SlotI == SlotI_Ring1 || SlotI == SlotI_Ring2 || SlotI == SlotI_Ring3 || SlotI == SlotI_Ring4)
				return GetFlag(A->InventorySlots, Slot_Ring - 1);
			else
				return GetFlag(A->InventorySlots, Slot_Backpack - 1);
		}
	}

}

// Checks an item matches a particular slot type
bool SlotsMatch(Item* I, int SlotI)
{
	if(SlotI >= SlotI_Backpack)
		return true;
	switch(I->ItemType)
	{
	case I_Weapon:
		if(SlotI == SlotI_Weapon)
			return true;
		break;
	case I_Armour:
		switch(I->SlotType)
		{
		case Slot_Shield:
			if(SlotI == SlotI_Shield)
				return true;
			break;
		case Slot_Hat:
			if(SlotI == SlotI_Hat)
				return true;
			break;
		case Slot_Chest:
			if(SlotI == SlotI_Chest)
				return true;
			break;
		case Slot_Hand:
			if(SlotI == SlotI_Hand)
				return true;
			break;
		case Slot_Belt:
			if(SlotI == SlotI_Belt)
				return true;
			break;
		case Slot_Legs:
			if(SlotI == SlotI_Legs)
				return true;
			break;
		case Slot_Feet:
			if(SlotI == SlotI_Feet)
				return true;
			break;
		}
		break;
	case I_Ring:
		if(I->SlotType == Slot_Ring)
		{
			if(SlotI >= SlotI_Ring1 && SlotI < SlotI_Ring4)
				return true;
		}
		else
		{
			if(SlotI >= SlotI_Amulet1 && SlotI < SlotI_Amulet2)
				return true;
		}
		break;
	}

	return false;
}

