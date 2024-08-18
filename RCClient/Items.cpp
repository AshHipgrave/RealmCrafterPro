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

#include "Items.h"

string DamageTypes[20];
int WeaponDamage, ArmourDamage;
Item* ItemList[65534];


ClassDef(Item, ItemList, ItemDelete);
ClassDef(ItemInstance, ItemInstanceList, ItemInstanceDelete);
ClassDef(DroppedItem, DroppedItemList, DroppedItemDelete);


// Returns the correct length in bytes of an item instance in string form
int ItemInstanceStringLength()
{
	return 83;
}

NGin::CString StrFromFloat(float I)
{
	NGin::CString S;
	S.AppendRealFloat(I);
	return S;
}

NGin::CString StrFromInt(int I)
{
	NGin::CString S;
	S.AppendRealInt(I);
	return S;
}

NGin::CString StrFromShort(short int I)
{
	NGin::CString S;
	S.AppendRealShort(I);
	return S;
}

NGin::CString StrFromChar(char I)
{
	NGin::CString S;
	S.AppendRealChar(I);
	return S;
}

short int ShortFromStr(NGin::CString& S)
{
	return S.GetRealShort(0);
}

char CharFromStr(NGin::CString& S)
{
	return S.GetRealChar(0);
}

float FloatFromStr(NGin::CString& S)
{
	return S.GetRealFloat(0);
}

int IntFromStr(NGin::CString& S)
{
	return S.GetRealInt(0);
}



// Converts an item instance to a string
NGin::CString ItemInstanceToString(ItemInstance* I)
{
	NGin::CString Pa;

	if(I == 0)
		return NGin::CString("");
	
	Pa.Append(StrFromShort(I->Item->ID));
	for(int j = 0; j < 40; ++j)
		Pa.Append(StrFromShort(I->Attributes->Value[j] + 5000));
	Pa.AppendRealChar(I->ItemHealth);
	
	return Pa;
}


// Reconstructs an item instance from a string
ItemInstance* ItemInstanceFromString(NGin::CString Pa)
{
	if(Pa.Length() < ItemInstanceStringLength())
		return 0;

	ItemInstance* I = CreateItemInstance(ItemList[ShortFromStr(Pa.Substr(0, 2))]);
	int Offset = 2;
	for(int j = 0; j < 40; ++j)
	{
		I->Attributes->Value[j] = ShortFromStr(Pa.Substr(Offset, 2)) - 5000;
		Offset += 2;
	}
	I->ItemHealth = CharFromStr(Pa.Substr(Offset, 1));

	return I;
}

// Writes an item instance to a stream
bool WriteItemInstance(FILE* Stream, ItemInstance* I)
{
	if(I == 0)
	{
		WriteShort(Stream, 65535);
		return false;
	}

	WriteShort(Stream, I->Item->ID);
	for(int j = 0; j < 40; ++j)
		WriteShort(Stream, I->Attributes->Value[j] + 5000);
	WriteByte(Stream, I->ItemHealth);

	return true;
}

// Reads an item instance from a stream
ItemInstance* ReadItemInstance(FILE* Stream)
{
	int ID = (int)ReadShort(Stream);
	if(ID == 65535)
		return 0;

	ItemInstance* I = CreateItemInstance(ItemList[ID]);
	for(int j = 0; j < 40; ++j)
		I->Attributes->Value[j] = (int)(unsigned short)ReadShort(Stream) - 5000;
	I->ItemHealth = ReadByte(Stream);

	return I;
}

// Compares two item instances and returns true if they are the same
bool ItemInstancesIdentical(ItemInstance* A, ItemInstance* B)
{
	if(A == 0 || B == 0)
		return false;

	if(A->Item != B->Item)
		return false;

	if(A->ItemHealth != B->ItemHealth)
		return false;

	for(int i = 0; i < 40; ++i)
		if(A->Attributes->Value[i] != B->Attributes->Value[i])
			return false;

	return true;
}

// Creates a new item template
Item* CreateItem()
{
	Item* I;

	for(int ID = 0; ID < 65535; ++ID)
		if(ItemList[ID] == 0)
		{
			I = new Item();
			I->ID = ID;
			ItemList[I->ID] = I;
			I->Attributes = new Attributes();
			I->ItemType = 1;
			I->SlotType = 1;
			I->Value = 1;
			I->Mass = 1;
			I->ImageID = 65535;
			break;
		}

	return I;
}

// Finds an item by name
Item* FindItem(string Name)
{
	Name = stringToUpper(Name);

	List<Item*>::Iterator It = Item::ItemList.Begin();
	for(;;++It)
	{
		string IName = (*It)->Name;
		IName = stringToUpper(IName);
		if(IName == Name)
			return (*It);

		if(It == Item::ItemList.End())
			break;
	}

	return 0;
}

// Creates a new instance of an item
ItemInstance* CreateItemInstance(Item* Item)
{
	ItemInstance* I = new ItemInstance();
	I->Item = Item;
	I->ItemHealth = 100;
	I->Attributes = new Attributes();
	for(int j = 0; j < 40; ++j)
		I->Attributes->Value[j] = I->Item->Attributes->Value[j];

	return I;
}


// Copies an item instance exactly
ItemInstance* CopyItemInstance(ItemInstance* A)
{
	ItemInstance* I = new ItemInstance();
	I->Attributes = new Attributes();
	I->Item = A->Item;
	I->ItemHealth = A->ItemHealth;
	for(int j = 0; j < 40; ++j)
		I->Attributes->Value[j] = A->Attributes->Value[j];

	return I;
}



// Frees an item instance
void FreeItemInstance(ItemInstance* I)
{
	delete I->Attributes;
	I->Attributes = NULL;
	ItemInstance::Delete(I);
	ItemInstance::Clean();
}

// Loads attribute names from file
bool LoadDamageTypes(string Filename)
{
	FILE* F = ReadFile(Filename);
	if(F == 0)
		return false;

	for(int i = 0; i < 20; ++i)
		DamageTypes[i] = ReadString(F);

	CloseFile(F);
	return true;
}

// Looks up a damage type number from the name
int FindDamageType(string Name)
{
	for(int i = 0; i < 20; ++i)
		if(DamageTypes[i] == Name)
			return i;

	return -1;
}

// Gets the item type in text form
string GetItemType(Item* I)
{
	switch(I->ItemType)
	{
	case I_Weapon:
		return LanguageString[LS_Weapon];
		break;
	case I_Armour:
		return LanguageString[LS_Armour];
		break;
	case I_Ring:
		if(I->SlotType == Slot_Ring)
			return LanguageString[LS_Ring];
		else
			return LanguageString[LS_Amulet];
		break;
	case I_Potion:
		return LanguageString[LS_Potion];
		break;
	case I_Ingredient:
		return LanguageString[LS_Ingredient];
		break;
	case I_Image:
		return LanguageString[LS_Image];
		break;
	case I_Other:
		return LanguageString[LS_Miscellaneous];
		break;
	}
	return LanguageString[LS_Unknown];
}

// Gets the weapon type in text form
string GetWeaponType(Item* I)
{
	switch(I->WeaponType)
	{
	case W_OneHand:
		return LanguageString[LS_OneHanded];
		break;
	case W_TwoHand:
		return LanguageString[LS_TwoHanded];
		break;
	case W_Ranged:
		return LanguageString[LS_Ranged];
		break;
	}
	return LanguageString[LS_Unknown];
}





