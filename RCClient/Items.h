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

#include "Language.h"
#include "Inventories.h"
#include "Gubbins.h"

#define I_Weapon      1 // Item types
#define I_Armour      2
#define I_Ring        3
#define I_Potion      4
#define I_Ingredient  5
#define I_Image       6
#define I_Other       7

#define A_Hat       0 // Armour types
#define A_Shirt     1
#define A_Trousers  2
#define A_Gloves    3
#define A_Boots     4
#define A_Shield    5

#define W_OneHand  1 // Weapon types
#define W_TwoHand  2
#define W_Ranged   3

#define TYPE_DroppedItem 14

extern string DamageTypes[20];
extern int WeaponDamage, ArmourDamage;

struct Attributes;
class ActorInstance;
class GubbinTemplate;

// Decribes an item
struct Item
{
	int ID;
	string Name;
	string ExclusiveRace, ExclusiveClass;			// If this item can only be used by a certain race and/or class
	string Script, Method;							// Called when the item is right clicked
	char ItemType;									// Should be one of the constants above
	int Value;
	int Mass;								// Average monetary value, and item weight
	int ThumbnailTexID;								// The texture ID for the image seen in the inventory system
// 	int MMeshID, FMeshID;							// Weapon/hat/shield/chest/forearm/shin mesh IDs
// 	int Gubbins[6];									// Flags to activate gubbins when item is equipped
	std::vector<GubbinTemplate*> GubbinTemplates;
	Attributes* Attributes;							// An actor attributes object (for extra weapon damage effects, armour use, food eating, etc.)
	char TakesDamage;								// True if using this item reduces its health, False for it to be indestructable
	int SlotType;									// Should be set to one of the slot type constants in Inventories.h
	int WeaponDamage, WeaponDamageType, WeaponType;	// Weapon specific
	int RangedProjectile;							// 
	string RangedAnimation;							// Ranged weapon specific
	float Range;									// 
	int ArmourLevel;								// Armour specific
	int EatEffectsLength;							// Potion or ingredients specific
	int ImageID;									// Image item specific (Texture ID)
	string MiscData;								// General use for misc items
	bool Stackable;									// Item can be stacked up

	ClassDecl(Item, ItemList, ItemDelete);
};
extern Item* ItemList[65534];

// Is used when an actual instance of an item is created in the world (on the floor, in someone's inventory, etc.)
struct ItemInstance
{
	Item* Item;
	Attributes* Attributes;		// Replaces Item\Attributes which is merely the default item attributes
	char ItemHealth;				// The amount of damage (percentage) the item has left before breaking
	int Assignment;
	ActorInstance* AssignTo;	// Server use only - Assignment is > 0 if item instance is created but not assigned an inventory slot yet

	ClassDecl(ItemInstance, ItemInstanceList, ItemInstanceDelete);
};

// Item dropped on the floor
struct DroppedItem
{
	int TYPE;
	int EN;
	int ServerHandle;
	RealmCrafter::SectorVector Position;
	ItemInstance* Item;
	int Amount;

	ClassDecl(DroppedItem, DroppedItemList, DroppedItemDelete);
};

// Functions
NGin::CString StrFromFloat(float I);
NGin::CString StrFromInt(int I);
NGin::CString StrFromShort(short int I);
NGin::CString StrFromChar(char I);
short int ShortFromStr(NGin::CString& S);
char CharFromStr(NGin::CString& S);
float FloatFromStr(NGin::CString& S);
int IntFromStr(NGin::CString& S);
int ItemInstanceStringLength();
NGin::CString ItemInstanceToString(ItemInstance* I);
ItemInstance* ItemInstanceFromString(NGin::CString Pa);
bool WriteItemInstance(int Stream, ItemInstance* I);
ItemInstance* ReadItemInstance(int Stream);
bool ItemInstancesIdentical(ItemInstance* A, ItemInstance* B);
Item* CreateItem();
Item* FindItem(string Name);
ItemInstance* CreateItemInstance(Item* Item);
ItemInstance* CopyItemInstance(ItemInstance* A);
void FreeItemInstance(ItemInstance* I);
bool LoadDamageTypes(string Filename);
int FindDamageType(string Name);
string GetItemType(Item* I);
string GetWeaponType(Item* I);



