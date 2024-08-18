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

#include "Inventories.h"
#include "Items.h"
#include "ClientCombat.h"
#include "Gubbins.h"

// Definitions
struct Actor;
struct Inventory;
struct FloatingNumber;
class GubbinPreviewInstance;

enum ScriptShaderParameterType
{
	SSPT_Float = 0,
	SSPT_Float2 = 1,
	SSPT_Float3 = 2,
	SSPT_Float4 = 3,
	SSPT_Time = 4,
	SSPT_TimeReset = 5,
	SSPT_TimeLoop = 6
};

struct ScriptShaderParameter
{
	std::string Name;
	ScriptShaderParameterType ParameterType;
	NGin::Math::Vector4 Data;
	uint Time;
};

// Actor instance
class ActorInstance : public RealmCrafter::SDK::IActorInstance
{
public:
	int TYPE;
	Actor* Actor;
	ActorInstance* NextInZone; // Linked list containing all actors in zone

	//JB 2.50 removed, and SectorVector added
// 	float X, Y, Z;
// 	float OldX, OldZ;
// 	float DestX, DestZ;

	RealmCrafter::SectorVector Position;
	RealmCrafter::SectorVector PreviousPosition;
	RealmCrafter::SectorVector Destination;

	uint LastUpdateTime;
	float PrevYaw, Yaw, NewYaw;
	bool WalkingBackward;
	bool StrafingLeft, StrafingRight;
	string Area;
	int ServerArea, Account;
	string Name, Tag;
	int LastPortal, LastTrigger, LastPortalTime;
	int TeamID; // Used to allow scripting to put people together in teams
	int PartyID;
	char AcceptPending; // Holds the handle of a Party object (or 0 if the actor is not in a party)
	char Gender; // 0 for male, 1 for female
	unsigned int EN, CollisionEN; // HatEN will store the hair entity if a hat is not worn

	RealmCrafter::SInventory InventoryHandle;

	std::vector<GubbinPreviewInstance*> HatENs;
	std::vector<GubbinPreviewInstance*> BeardENs;
	std::vector<GubbinPreviewInstance*> ChestENs;
	std::vector<GubbinPreviewInstance*> WeaponENs;
	std::vector<GubbinPreviewInstance*> ShieldENs;

	std::vector<GubbinPreviewInstance*> GubbinENs;

	std::vector<ScriptShaderParameter*> ShaderParameters;

	ILabel* NametagEN, *TagEN;
	int FaceTex, Hair, Beard, BodyTex; // Fixed throughout a character's life unless altered by scripting
	int Level;
	int XP;
	char XPBarLevel;
	char HomeFaction;               // Faction this actor belongs to (0-100 with 0 meaning no faction)
	char FactionRatings[100];        // Individual ratings with each faction for this actor - start off as home faction defaults
	RealmCrafter::SAttributes* Attributes;     // Replaces Actor\Attributes which is merely the default actor attributes
	int Resistances[20];           // Resistances against damage types
	string Script;                   // Script which executes when character is selected (for traders mainly)
	string DeathScript;              // Script which executes when actor is killed (NPCs only)
	Inventory* Inventory;       // The actor's inventory slots!
	ActorInstance* Leader;      // For slaves, pets, etc.
	char NumberOfSlaves;            // Whether this actor owns any slaves (to speed up saving actor instances)
	int Reputation;
	int Gold;
	int RNID;                      // RottNet ID (-1 for AI actors, 0 for not-in-game)
	int RuntimeID;                 // Assigned by server
	int AnimSeqs[150];             // Animation sequences
	int SourceSP, CurrentWaypoint, AIMode; // AI stuff
	ActorInstance*Rider;
	ActorInstance* Mount;
	ActorInstance* AITarget; // Mount riding
	bool IsRunning;
	int LastAttack;
	char FootstepPlayedThisCycle ;  // To prevent too many footstep noises! See UpdateActorInstances() in Default Project.bb
	string ScriptGlobals[10];
	int KnownSpells[1000];
	int SpellLevels[1000];
	int MemorisedSpells[10];
	int SpellCharge[1000]; // How long until the spell is usable
	char IsTrading; // 0 for not trading, 1 for trading with NPC, 2 for trading with pet, 3 for trading with player, 4/5 for accepted trading with player
	ActorInstance* TradingActor;
	string TradeResult;
	uint Underwater;
	bool IgnoreUpdate;
	ArrayList<FloatingNumber*> FloatingNumbers;

	//Tmp: Added for debugging rotations
	bool ForceUpdate;

	ActorInstance();
	virtual ~ActorInstance();
	
	static NGin::List<ActorInstance*> ActorInstanceList;
	static NGin::List<ActorInstance*> ActorInstanceDelete;
	static void Delete(ActorInstance* Item);
	static void Clean();

	void UpdateShaderParameters();

	virtual void UpdateGubbins();
	virtual void UpdateGubbinSet(std::vector<GubbinPreviewInstance*>& previews);
	virtual void ShowGubbinSet(std::vector<GubbinTemplate*>* templates, std::vector<GubbinPreviewInstance*>& previews);
	virtual void HideGubbinSet(std::vector<GubbinPreviewInstance*>& previews);

	// SDK Methods
	virtual uint GetEN();
	virtual uint GetCollisionEN();
	virtual float GetYaw();
	virtual void SetYaw(float yaw);
	virtual RealmCrafter::SectorVector GetDestination();
	virtual RealmCrafter::SectorVector GetPosition();
	virtual bool GetRunning();
	virtual void SetRunning(bool running);
	virtual RealmCrafter::SDK::IActorInstance* GetMount();
	virtual bool GetWalkingBackward();
	virtual void SetWalkingBackward(bool backward);
	virtual bool GetStrafingLeft();
	virtual void SetStrafingLeft(bool strafing);
	virtual bool GetStrafingRight();
	virtual void SetStrafingRight(bool strafing);
	virtual RealmCrafter::SAttributes* GetAttributes();

	virtual std::string GetArea() { return Area; }
	virtual std::string GetName() { return Name; }
	virtual std::string GetTag() { return Tag; }
	virtual char GetGender() { return Gender; }
	virtual int GetFaceTex() { return FaceTex; }
	virtual int GetHair() { return Hair; }
	virtual int GetBeard() { return Beard; }
	virtual int GetBodyTex() { return BodyTex; }
	virtual int GetLevel() { return Level; }
	virtual int GetXP() { return XP; }
	virtual int GetReputation() { return Reputation; }
	virtual int GetMoney() { return Gold; }

	virtual void SetDestination(RealmCrafter::SectorVector &destination, bool turnMe = false);
};
extern ActorInstance* RuntimeIDList[65535];
extern int LastRuntimeID;
