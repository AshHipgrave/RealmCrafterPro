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

#pragma once

#include "Temp.h"
#include <NGinString.h>
#include <List.h>
#include <BlitzPlus.h>

#include "Default Project.h"
#include "Language.h"
#include "Items.h"
#include "Actors.h"
#include "Packets.h"

// Slot names
#define Slot_Weapon    1
#define Slot_Shield    2
#define Slot_Hat       3
#define Slot_Chest     4
#define Slot_Hand      5
#define Slot_Belt      6
#define Slot_Legs      7
#define Slot_Feet      8
#define Slot_Ring      9
#define Slot_Amulet    10
#define Slot_Backpack  11

// Slot array indices
#define SlotI_Weapon    0
#define SlotI_Shield    1
#define SlotI_Hat       2
#define SlotI_Chest     3
#define SlotI_Hand      4
#define SlotI_Belt      5
#define SlotI_Legs      6
#define SlotI_Feet      7
#define SlotI_Ring1     8
#define SlotI_Ring2     9
#define SlotI_Ring3     10
#define SlotI_Ring4     11
#define SlotI_Amulet1   12
#define SlotI_Amulet2   13
#define SlotI_Backpack  14

struct Item;
struct ItemInstance;
struct Actor;


// Inventory object
struct Inventory
{
	ItemInstance* Items[50];
	int Amounts[50];
	int My_AttrID;				// Two dimensional array for Attributes
	int My_ID;					// Required for MySQL

	ClassDecl(Inventory, InventoryList, InventoryDelete);
};

// Gets the overall armour level for an inventory
int GetArmourLevel(Inventory* I);

// Gets the total mass contained in an inventory
int InventoryMass(Inventory* I);

// Drops an item from an inventory to the floor
ItemInstance* InventoryDrop(ActorInstance* A, int SlotFrom, int Amount, int TellServer = true);

// Moves items from one pile to another of the same item type
bool InventoryAdd(ActorInstance* A, int SlotFrom, int SlotTo, int Amount,  bool TellServer = true);

// Moves items between inventory slots
bool InventorySwap(ActorInstance* A, int SlotA, int SlotB, int Amount = 0, bool TellServer = true);

// Returns true if the items specified are present
bool InventoryHasItem(Inventory* I, string Item, int Amount);

// Checks if an actor has a particular slot index
int ActorHasSlot(Actor* A, int SlotI, Item* I);

// Checks an item matches a particular slot type
bool SlotsMatch(Item* I, int SlotI);

