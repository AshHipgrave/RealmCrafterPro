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

#include "ActorInstance.h"

#include "Inventories.h"
#include "Items.h"
#include "ClientCombat.h"
#include "Gubbins.h"

// AI modes
#define AI_Wait         0
#define AI_Patrol       1
#define AI_Run          2
#define AI_Chase        3
#define AI_PatrolPause  4
#define AI_Pet          5
#define AI_PetChase     6
#define AI_PetWait      7

// Speech sounds
#define Speech_Greet1        0
#define Speech_Greet2        1
#define Speech_Bye1          2
#define Speech_Bye2          3
#define Speech_Attack1       4
#define Speech_Attack2       5
#define Speech_Hit1          6
#define Speech_Hit2          7
#define Speech_RequestHelp   8
#define Speech_Death         9
#define Speech_FootstepDry   10
#define Speech_FootstepWet   11

// Environment types
#define Environment_Amphibious  0
#define Environment_Swim        1
#define Environment_Fly         2
#define Environment_Walk        3

#define TYPE_ActorInstance 15

class ActorInstance;
struct Attributes;
struct Inventory;
struct FloatingNumber;
class GubbinTemplate;

// Texture set
struct ActorTextureSet
{
	int Tex0;
	int Tex1;
	int Tex2;
	int Tex3;

	ActorTextureSet(int t0, int t1, int t2, int t3)
		: Tex0(t0), Tex1(t1), Tex2(t2), Tex3(t3)
	{}
};

// Actor template
struct Actor
{
	int ID;
	string Race, Class, Description, StartArea, StartPortal;
	float Radius;          // For server use, since the server is not aware of the details of the mesh itself
	float Scale;           // Actor scale, applied to the base mesh

	int MaleMesh, FemaleMesh;
	std::vector<std::vector<GubbinTemplate*> > Beards, MaleHairs, FemaleHairs;
	std::vector<GubbinTemplate*> Gubbins;

	std::vector<ActorTextureSet> MaleFaceIDs;   // Allowed face textures for male
	std::vector<ActorTextureSet> FemaleFaceIDs; // Allowed face textures for female
	std::vector<ActorTextureSet> MaleBodyIDs;   // Allowed body textures for the male
	std::vector<ActorTextureSet> FemaleBodyIDs; // Allowed body textures for the female
	int MSpeechIDs[16];   // Male sound IDs for speech
	int FSpeechIDs[16];   // Female sound IDs for speech
	int HairColours[16];  // Values for hair vertex colouring
	int BloodTexID;       // For blood particles
	char Genders;          // 0 for normal (male and female), 1 for male only, 2 for female only, 3 for no genders
	Attributes* Attributes;
	int Resistances[19];    // Damage type resistances
	int MAnimationSet;      // The ID of the male animation set to use
	int FAnimationSet;      // The ID of the female animation set to use
	bool Playable;           // Can a player be this actor?
	char Rideable;           // Can this actor be ridden by another?
	char Aggressiveness;     // Aggressiveness - 0 = passive, 1 = attack when provoked, 2 = attack on sight, 3 = no combat
	int AggressiveRange;    // From how nearby will the actor detect targets?
	char TradeMode;          // 0 = will not trade, 1 = trades for free (pack mules!), 2 = charges for trade (salesman)
	char Environment;        // Whether actor walks, swims, flies, etc.
	int InventorySlots;     // Short (up to 16 true/false flags) for the slots defined in Inventories.bb
	char DefaultDamageType;
	char DefaultFaction;     // Initial home faction for instances of this actor
	int XPMultiplier;       // How much experience another actor gets for killing an instance of this actor
	char PolyCollision;      // True for polygonal collision instead of ellipsoid

	ClassDecl(Actor, ActorList, ActorDelete);
};

extern Actor* ActorList[65535];



struct Party
{
	int Members;
	ActorInstance* Player[7];

	ClassDecl(Party, PartyList, PartyDelete);
};

// Actor attributes (strength, dexterity, health, armour, whatever the user decides)
extern int AttributeAssignment;

struct Attributes
{
	int Value[40];
	int Maximum[40];
};

// Actor effect (buff)
struct ActorEffect
{
	string Name;
	ActorInstance* Owner;
	Attributes* Attributes;
	int CreatedTime, Length; // Time created and time it lasts in milliseconds (Length = 0 for infinite)
	int IconTexID;

	ClassDecl(ActorEffect, ActorEffectList, ActorEffectDelete);
};

// Factions
extern string FactionNames[100];
extern int FactionDefaultRatings[100][100];

// Functions
int GetFlag(int TheInt, int Flag);
ActorInstance* FindPlayerFromName(string Name);
Actor* CreateActor();
ActorInstance* CreateActorInstance(Actor* Actor);
void FreeActorInstance(ActorInstance* A);
void FreeActorInstanceSlaves(ActorInstance* A);
bool ActorHasFace(Actor* A, char Gender = 0);
bool ActorHasHair(Actor* A, char Gender = 0);
bool ActorHasBeard(Actor* A);
bool ActorHasMultipleTextures(Actor* A, char Gender);
ActorInstance* ActorInstanceFromString(NGin::CString &Pa);
bool MultipleClasses(Actor* A);

