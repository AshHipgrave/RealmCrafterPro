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

#include "Actors.h"

ClassDef(Actor, ActorList, ActorDelete);
ClassDef(Party, PartyList, PartyDelete);
ClassDef(ActorEffect, ActorEffectList, ActorEffectDelete);

int AttributeAssignment;

string FactionNames[100];
int FactionDefaultRatings[100][100];
Actor* ActorList[65535];

int GetFlag(int TheInt, int Flag)
{
	return (TheInt >> Flag) & 1;
}

// Finds a human actor instance based on their name
ActorInstance* FindPlayerFromName(string Name)
{
	Name = stringToUpper(Name);
	foreachc(It, ActorInstance, ActorInstanceList)
	{
		if((*It)->RNID > -1)
			if(stringToUpper((*It)->Name).compare(Name) == 0)
				return (*It);

		nextc(It, ActorInstance, ActorInstanceList);
	}

	return 0;
}

// Creates a new actor template
Actor* CreateActor()
{
	for(int i = 0; i < 65535; ++i)
		if(ActorList[i] == 0)
		{
			Actor* A = new Actor();
			A->ID = i;
			ActorList[A->ID] = A;
			A->Attributes = new Attributes();
			for(int j = 0; j < 40; ++j)
				A->Attributes->Maximum[j] = 100;

			A->MaleMesh = 0;
			A->FemaleMesh = 0;

			for(int j = 0; j < 12; ++j)
			{
				A->MSpeechIDs[j] = 65535;
				A->FSpeechIDs[j] = 65535;
			}
			A->InventorySlots = 0xffffffff;
			A->Scale = 1.0f;
			A->AggressiveRange = 50;
			return A;
		}

	return 0;
}

// Creates a new instance of an actor
ActorInstance* CreateActorInstance(Actor* Actor)
{
	if(Actor == 0)
		RuntimeError("Could not create actor instance - actor does not exist!");

	ActorInstance* A = new ActorInstance();
	A->TYPE = TYPE_ActorInstance;

	A->Attributes = new RealmCrafter::SAttributes();
	A->Inventory = new Inventory();
	A->Actor = Actor;
	A->Name = A->Actor->Race;
	A->HomeFaction = A->Actor->DefaultFaction;
	for(int i = 0; i < 100; ++i)
		A->FactionRatings[i] = FactionDefaultRatings[A->HomeFaction][i];
	for(int i = 0; i < 40; ++i)
	{
		A->Attributes->Value[i] = A->Actor->Attributes->Value[i];
		A->Attributes->Maximum[i] = A->Actor->Attributes->Maximum[i];
	}
	for(int i = 0; i < 20; ++i)
		A->Resistances[i] = A->Actor->Resistances[i];
	for(int i = 0; i < 10; ++i)
		A->MemorisedSpells[i] = 5000; // No spell memorised
	for(int i = 0; i < 1000; ++i)
		A->KnownSpells[i] = 65535;
	if(A->Actor->Genders == 2)
		A->Gender = 1;
	A->Level = 1;
	A->RuntimeID = -1;
	A->LastAttack = MilliSecs();
	A->SourceSP = -1;
	A->LastTrigger = -1;
	A->LastPortal = -1;
	A->IgnoreUpdate = false;
	return A;
}

// Frees an actor instance
void FreeActorInstance(ActorInstance* A)
{
	if(A->RuntimeID > -1)
		if(RuntimeIDList[A->RuntimeID] == A)
			RuntimeIDList[A->RuntimeID] = 0;

	if(A->Leader != 0)
		--A->Leader->NumberOfSlaves;

	RemoveRadarMark(A);
	ActorInstance::Delete(A);
}

// Frees all the slaves of an actor instance (RECURSIVE)
void FreeActorInstanceSlaves(ActorInstance* A)
{
	foreachc(It, ActorInstance, ActorInstanceList)
	{
		ActorInstance* A2 = (*It);
		if(A->NumberOfSlaves == 0)
			break;
		
		if(A2->Leader == A)
		{
			FreeActorInstanceSlaves(A2);
			FreeActorInstance(A2);
		}

		nextc(It, ActorInstance, ActorInstanceList);
	}
}

// Returns whether a specified actor has any allowed face textures or not (gender should be 1 for male, 2 for female, or 0 for either)
bool ActorHasFace(Actor* A, char Gender)
{
	if(Gender != 2)
	{
		for(int i = 0; i < A->MaleFaceIDs.size(); ++i)
		{
			if(A->MaleFaceIDs[i].Tex0 >= 0 && A->MaleFaceIDs[i].Tex0 < 65535)
				return true;
		}
	}else if(Gender != 1)
	{
		for(int i = 0; i < A->FemaleFaceIDs.size(); ++i)
		{
			if(A->FemaleFaceIDs[i].Tex0 >= 0 && A->FemaleFaceIDs[i].Tex0 < 65535)
				return true;
		}
	}
	return false;
}

// Returns whether a specified actor has any allowed hair meshes or not (gender should be 1 for male, 2 for female, or 0 for either)
bool ActorHasHair(Actor* A, char Gender)
{
	if(Gender != 2 && A->MaleHairs.size() > 0)
		return true;
	if(Gender != 1 && A->FemaleHairs.size() > 0)
		return true;

	return false;
}

// Returns whether a specified actor has any allowed beard meshes or not
bool ActorHasBeard(Actor* A)
{
	if(A->Genders == 2)
		return false;

	return A->Beards.size() > 0;
}

// Returns whether a specified actor has multiple possible body or head textures
bool ActorHasMultipleTextures(Actor* A, char Gender)
{
	bool FoundBody = false;

	// Male
	if(Gender == 0)
	{
		int Length = A->MaleFaceIDs.size();
		if(Length > A->MaleBodyIDs.size())
			Length = A->MaleBodyIDs.size();

		for(int i = 0; i < Length; ++i)
		{
			if(A->MaleFaceIDs[i].Tex0 >= 0 && A->MaleFaceIDs[i].Tex0 < 65535)
				return true;
			if(A->MaleBodyIDs[i].Tex0 >= 0 && A->MaleBodyIDs[i].Tex0 < 65535)
				if(FoundBody)
					return true;
				else
					FoundBody = true;
		}
	}else // Female
	{
		int Length = A->FemaleFaceIDs.size();
		if(Length > A->FemaleBodyIDs.size())
			Length = A->FemaleBodyIDs.size();

		for(int i = 0; i < Length; ++i)
		{
			if(A->FemaleFaceIDs[i].Tex0 >= 0 && A->FemaleFaceIDs[i].Tex0 < 65535)
				return true;
			if(A->FemaleBodyIDs[i].Tex0 >= 0 && A->FemaleBodyIDs[i].Tex0 < 65535)
				if(FoundBody)
					return true;
				else
					FoundBody = true;
		}
	}
	return false;
}



// Converts a string back into an actor instance after network transmission
ActorInstance* ActorInstanceFromString(NGin::CString &Pa)
{
	int ServerArea = IntFromStr(Pa.Substr(0, 4));
	if(ServerArea != CurrentAreaID)
		return 0;

	int RuntimeID = ShortFromStr(Pa.Substr(4, 2));
	int ActorID = ShortFromStr(Pa.Substr(12, 2));
	ActorInstance* A = CreateActorInstance(ActorList[ActorID]);
	A->RuntimeID = RuntimeID;
	RuntimeIDList[RuntimeID] = A;
	A->Level = ShortFromStr(Pa.Substr(6, 2));
	A->XP = IntFromStr(Pa.Substr(8, 4));

	A->Position.SectorX = ShortFromStr(Pa.Substr(14, 2));
	A->Position.SectorZ = ShortFromStr(Pa.Substr(16, 2));
	A->Position.X = FloatFromStr(Pa.Substr(18, 4));
	A->Position.Y = FloatFromStr(Pa.Substr(22, 4));
	A->Position.Z = FloatFromStr(Pa.Substr(26, 4));

	A->Yaw = FloatFromStr(Pa.Substr(30, 4));
	A->Destination = A->Position;
	A->RNID = CharFromStr(Pa.Substr(34, 1)); // 1 if human, 0 if AI
	int NameLen = CharFromStr(Pa.Substr(35, 1));
	A->Name = Pa.Substr(36, NameLen).c_str();
	int Offset = 36 + NameLen;
	NameLen = CharFromStr(Pa.Substr(Offset, 1));
	A->Tag = Pa.Substr(Offset + 1, NameLen).c_str();
	Offset += 1 + NameLen;
	if(A->Actor->Genders == 0)
	{	
		A->Gender = CharFromStr(Pa.Substr(Offset, 1));
		++Offset;
	}
	A->Reputation = ShortFromStr(Pa.Substr(Offset, 2));
	A->FaceTex = ShortFromStr(Pa.Substr(Offset + 2, 2));
	A->Hair    = ShortFromStr(Pa.Substr(Offset + 4, 2));
	A->BodyTex = ShortFromStr(Pa.Substr(Offset + 6, 2));
	A->Beard   = ShortFromStr(Pa.Substr(Offset + 8, 2));

	printf("New Actor: %s; Body: %i; Face: %i;\n", A->Name.c_str(), A->BodyTex, A->FaceTex);

	A->Attributes->Value[RealmCrafter::Globals->SpeedStat]    = (int)(unsigned short)ShortFromStr(Pa.Substr(Offset + 10, 2));
	A->Attributes->Maximum[RealmCrafter::Globals->SpeedStat]  = (int)(unsigned short)ShortFromStr(Pa.Substr(Offset + 12, 2));
	A->Attributes->Value[RealmCrafter::Globals->HealthStat]   = (int)(unsigned short)ShortFromStr(Pa.Substr(Offset + 14, 2));
	A->Attributes->Maximum[RealmCrafter::Globals->HealthStat] = (int)(unsigned short)ShortFromStr(Pa.Substr(Offset + 16, 2));
	uint WeaponID   = ShortFromStr(Pa.Substr(Offset + 18, 2));
	uint ShieldID   = ShortFromStr(Pa.Substr(Offset + 20, 2));
	uint HatID      = ShortFromStr(Pa.Substr(Offset + 22, 2));
	uint ChestID    = ShortFromStr(Pa.Substr(Offset + 24, 2));
	if(WeaponID < 65535)
		A->Inventory->Items[SlotI_Weapon] = CreateItemInstance(ItemList[WeaponID]);
	if(ShieldID < 65535)
		A->Inventory->Items[SlotI_Shield] = CreateItemInstance(ItemList[ShieldID]);
	if(HatID < 65535)
		A->Inventory->Items[SlotI_Hat] = CreateItemInstance(ItemList[HatID]);
	if(ChestID < 65535)
		A->Inventory->Items[SlotI_Chest] = CreateItemInstance(ItemList[ChestID]);
	A->HomeFaction = CharFromStr(Pa.Substr(Offset + 26, 1));

	Offset += 27;
	for(int i = 0; i < 100; ++i)
		A->FactionRatings[i] = CharFromStr(Pa.Substr(Offset + i, 1));

	Offset += 100;
	int Count = Pa.GetRealShort(Offset); Offset += 2;
	for(int i = 0; i < Count; ++i)
	{
		ScriptShaderParameter* Param = new ScriptShaderParameter();
		int Len = Pa.GetRealChar(Offset);
		Param->Name = Pa.Substr(Offset + 1, Len).c_str();
		Offset += Len + 1;

		Param->ParameterType = (ScriptShaderParameterType)Pa.GetRealChar(Offset++);
		Param->Data.X = Pa.GetRealFloat(Offset); Offset += 4;
		Param->Data.Y = Pa.GetRealFloat(Offset); Offset += 4;
		Param->Data.Z = Pa.GetRealFloat(Offset); Offset += 4;
		Param->Data.W = Pa.GetRealFloat(Offset); Offset += 4;
		Param->Time = MilliSecs();

		A->ShaderParameters.push_back(Param);
	}

	return A;
}

// See if more than one class exists
bool MultipleClasses(Actor* A)
{
	string Race = stringToUpper(A->Race);

	foreachc(AIt, Actor, ActorList)
	{
		if(stringToUpper((*AIt)->Race).compare(Race) == 0)
			return true;

		nextc(AIt, Actor, ActorList);
	}

	return false;
}





