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
#include "ClientSDKExport.h"

namespace RealmCrafter
{
	bool InventorySwap(int SlotA, int SlotB, int Amount)
	{
		return ::InventorySwap(Me, SlotA, SlotB, Amount);
	}

	void InventoryDrop(int SlotFrom, int Amount)
	{
		::InventoryDrop(Me, SlotFrom, Amount);
	}

	void UseItem(int SlotIndex, int Amount)
	{
		::UseItem(SlotIndex, Amount);
	}

	std::string GetItemPathFromID(int id)
	{
		if(id < 0 || id >= 65535 || ItemList[id] == NULL || ItemList[id]->ThumbnailTexID == 65535)
			return "Data\\Textures\\GUI\\Backpack.bmp";

		return string("Data\\Textures\\") + GetTextureNameNoFlag(ItemList[id]->ThumbnailTexID);
	}

	std::string GetSpellPathFromID(int id)
	{
		if(id < 0 || id >= 65535 || SpellsList[id] == NULL || SpellsList[id]->ThumbnailTexID == 65535)
			return "Data\\Textures\\GUI\\Backpack.bmp";

		return string("Data\\Textures\\") + GetTextureNameNoFlag(SpellsList[id]->ThumbnailTexID);
	}


	std::string GetSpellNameFromID(int id)
	{
		if(id < 0 || id >= 65535 || SpellsList[id] == NULL || SpellsList[id]->ThumbnailTexID == 65535)
			return "";

		return SpellsList[id]->Name;
	}

	std::string GetSpellName(int id)
	{
		if(id < 0 || id > 65534)
			return "";

		if(SpellsList[id] != NULL)
			return SpellsList[id]->Name;
		return "";
	}

	std::string GetItemName(int id)
	{
		if(id < 0 || id > 65534)
			return "";

		if(ItemList[id] != NULL)
			return ItemList[id]->Name;
		return "";
	}

	std::string GetItemTypeString(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return "";

		return ::GetItemType(ItemList[itemID]);
	}

	bool GetItemTakesDamage(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return false;

		return ItemList[itemID]->TakesDamage > 0;
	}

	int GetItemHealth(int slot)
	{
		if(Me == NULL || Me->Inventory == NULL || Me->Inventory->Items[slot] == NULL)
			return 0;

		return 100 - Me->Inventory->Items[slot]->ItemHealth;
	}

	int GetItemValue(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return 0;

		return ItemList[itemID]->Value;
	}

	int GetItemMass(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return 0;

		return ItemList[itemID]->Mass;
	}

	bool GetItemStackable(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return false;

		return ItemList[itemID]->Stackable;
	}

	int GetItemType(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return 0;

		return ItemList[itemID]->ItemType;
	}

	int GetItemWeaponDamage(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return 0;

		return ItemList[itemID]->WeaponDamage;
	}

	std::string GetItemWeaponDamageType(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return "";

		return DamageTypes[ItemList[itemID]->WeaponDamageType];
	}

	std::string GetItemWeaponType(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return "";

		return GetWeaponType(ItemList[itemID]);
	}

	int GetItemArmourLevel(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return 0;

		return ItemList[itemID]->ArmourLevel;
	}

	int GetItemEatEffects(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return 0;

		return ItemList[itemID]->EatEffectsLength;
	}

	std::string GetSpellDescription(unsigned short spellID)
	{
		if(spellID == 65535 || SpellsList[spellID] == NULL)
			return "";

		return SpellsList[spellID]->Description;
	}

	std::string GetItemExclusiveRace(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return "";

		return ItemList[itemID]->ExclusiveRace;
	}

	std::string GetItemExclusiveClass(unsigned short itemID)
	{
		if(itemID == 65535 || ItemList[itemID] == NULL)
			return "";

		return ItemList[itemID]->ExclusiveClass;
	}

	bool GetPlayerIsExclusiveRace(unsigned short itemID)
	{
		std::string Name = GetItemExclusiveRace(itemID);
		if(Name.length() == 0 || Me == NULL || Me->Actor == NULL || Me->Actor->Race.length() == 0)
			return false;

		return Name.compare(Me->Actor->Race) == 0;
	}

	bool GetPlayerIsExclusiveClass(unsigned short itemID)
	{
		std::string Name = GetItemExclusiveClass(itemID);
		if(Name.length() == 0 || Me == NULL || Me->Actor == NULL || Me->Actor->Class.length() == 0)
			return false;

		return Name.compare(Me->Actor->Class) == 0;
	}

}