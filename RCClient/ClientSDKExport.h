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
#include "Temp.h"
#include "Inventories.h"
#include "Items.h"
#include "Spells.h"

namespace RealmCrafter
{
	bool InventorySwap(int SlotA, int SlotB, int Amount = 0);
	void InventoryDrop(int SlotFrom, int Amount);
	void UseItem(int SlotIndex, int Amount);
	std::string GetItemPathFromID(int id);
	std::string GetSpellPathFromID(int id);
	std::string GetSpellName(int id);
	std::string GetItemName(int id);
	std::string GetItemTypeString(unsigned short itemID);
	bool GetItemTakesDamage(unsigned short itemID);
	int GetItemHealth(int slot);
	int GetItemValue(unsigned short itemID);
	int GetItemMass(unsigned short itemID);
	bool GetItemStackable(unsigned short itemID);
	int GetItemType(unsigned short itemID);
	int GetItemWeaponDamage(unsigned short itemID);
	std::string GetItemWeaponDamageType(unsigned short itemID);
	std::string GetItemWeaponType(unsigned short itemID);
	int GetItemArmourLevel(unsigned short itemID);
	int GetItemEatEffects(unsigned short itemID);
	std::string GetItemExclusiveRace(unsigned short itemID);
	std::string GetItemExclusiveClass(unsigned short itemID);
	bool GetPlayerIsExclusiveRace(unsigned short itemID);
	bool GetPlayerIsExclusiveClass(unsigned short itemID);
	std::string GetSpellDescription(unsigned short spellID);
	std::string GetSpellNameFromID(int id);
}