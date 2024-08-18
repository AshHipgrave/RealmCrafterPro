//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include "SDKMain.h"
#include "SectorVector.h"
#include <string>
#include "BBDX.h"
#include <IControl.h>
#include "IControlHostManager.h"
#include "IServerSelector.h"
#include <ITimerManager.h>
#include <IChatBox.h>

namespace RealmCrafter
{
	namespace SDK
	{
		class IControlHostManager;
	}
	class IServerSelector;
	class IGameMenu;
	class IChatBox;
	class IMouseItem;
	class IMouseToolTip;

	// Attributes paired array, engine inclusive
	struct SAttributes
	{
		int Value[40];
		int Maximum[40];
	};

	// Attribute definitions, engine inclusive
	struct SAttributeDefinition
	{
		std::string Name[40];
		bool IsSkill[40]; // False for a stat (health, strength, armour), True for a skill (fishing, riding)
		bool Hidden[40];
	};

	// QuestLog array, engine inclusive
	struct SQuestLog
	{
		std::string EntryName[500];
		std::string EntryStatus[500];
	};

	// Globals structure, engine inclusive
	struct SGlobals
	{
		bool AlwaysRun;
		int SpeedStat;
		int StrengthStat;
		int HealthStat;
		int EnergyStat;
		int BreathStat;
		bool AttackTarget;
		int LastZoneLoad;
		std::string GameName;
		std::string ServerHost;
		int ServerPort;
		std::string AreaName;
		bool QuitActive;
		std::string Money1;
		std::string Money2;
		std::string Money3;
		std::string Money4;
		int Money2x;
		int Money3x;
		int Money4x;
		int LastAttack;
		int CombatDelay;
		unsigned char FogR;
		unsigned char FogG;
		unsigned char FogB;
		float FogNear;
		float FogFar;
		float DefaultVolume;
		RealmCrafter::SDK::IControlHostManager* ControlHostManager;
		RealmCrafter::IServerSelector* ServerSelector;
		RealmCrafter::IGameMenu* GameMenu;
		NGin::ITimerManager* TimerManager;
		RealmCrafter::IChatBox* ChatBox;
		bool RequireMemorise;
		RealmCrafter::IMouseItem* CurrentMouseItem;
		RealmCrafter::IMouseToolTip* CurrentMouseToolTip;
		float DeltaTime;
		SAttributeDefinition* Attributes;
	};
	extern SGlobals* Globals;

	namespace SDK
	{
		// Actor Instance Interface for RealmCrafter SDK
		class IActorInstance
		{
		public:

			IActorInstance() {}
			virtual ~IActorInstance() {}

			virtual uint GetEN() = 0;
			virtual uint GetCollisionEN() = 0;
			virtual float GetYaw() = 0;
			virtual void SetYaw(float yaw) = 0;
			virtual RealmCrafter::SectorVector GetDestination() = 0;
			virtual RealmCrafter::SectorVector GetPosition() = 0;
			virtual bool GetRunning() = 0;
			virtual void SetRunning(bool running) = 0;
			virtual IActorInstance* GetMount() = 0;
			virtual bool GetWalkingBackward() = 0;
			virtual void SetWalkingBackward(bool backward) = 0;
			virtual bool GetStrafingLeft() = 0;
			virtual void SetStrafingLeft(bool strafing) = 0;
			virtual bool GetStrafingRight() = 0;
			virtual void SetStrafingRight(bool strafing) = 0;
			virtual SAttributes* GetAttributes() = 0;


			virtual std::string GetArea() = 0;
			virtual std::string GetName() = 0;
			virtual std::string GetTag() = 0;
			virtual char GetGender() = 0;
			virtual int GetFaceTex() = 0;
			virtual int GetHair() = 0;
			virtual int GetBeard() = 0;
			virtual int GetBodyTex() = 0;
			virtual int GetLevel() = 0;
			virtual int GetXP() = 0;
			virtual int GetReputation() = 0;
			virtual int GetMoney() = 0;

			virtual void SetDestination(RealmCrafter::SectorVector &destination, bool turnMe = false) = 0;

			//virtual void UpdateGubbinSet(std::vector<GubbinPreviewInstance*>& previews) = 0;
		};

		// Engine definitions
		namespace __SDKDef
		{
			typedef bool (tCanMovePlayerfn)();
			typedef bool (tControlHitfn)(int ctrl);
			typedef bool (tControlDownfn)(int ctrl);
			typedef float (tControlDownFfn)(int ctrl);
			typedef std::string (tControlNamefn)(int controlNumber);

			typedef void (tRP_Update)(float Delta);
			typedef bool (tRP_ConfigLifespan)(uint ID, int Lifespan);
			typedef bool (tRP_ConfigSpawnRate)(uint ID, int SpawnRate);
			typedef bool (tRP_ConfigVelocityMode)(uint ID, int Level);
			typedef bool (tRP_ConfigVelocityX)(uint ID, float VelocityX);
			typedef bool (tRP_ConfigVelocityY)(uint ID, float VelocityY);
			typedef bool (tRP_ConfigVelocityZ)(uint ID, float VelocityZ);
			typedef bool (tRP_ConfigVelocityRndX)(uint ID, float VelocityX);
			typedef bool (tRP_ConfigVelocityRndY)(uint ID, float VelocityY);
			typedef bool (tRP_ConfigVelocityRndZ)(uint ID, float VelocityZ);
			typedef bool (tRP_ConfigForceX)(uint ID, float VelocityX);
			typedef bool (tRP_ConfigForceY)(uint ID, float VelocityY);
			typedef bool (tRP_ConfigForceZ)(uint ID, float VelocityZ);
			typedef bool (tRP_ConfigForceModMode)(uint ID, int Mode);
			typedef bool (tRP_ConfigForceModX)(uint ID, float ForceModX);
			typedef bool (tRP_ConfigForceModY)(uint ID, float ForceModY);
			typedef bool (tRP_ConfigForceModZ)(uint ID, float ForceModZ);
			typedef bool (tRP_ConfigInitialRed)(uint ID, int Cl);
			typedef bool (tRP_ConfigInitialGreen)(uint ID, int Cl);
			typedef bool (tRP_ConfigInitialBlue)(uint ID, int Cl);
			typedef bool (tRP_ConfigRedChange)(uint ID, float Cl);
			typedef bool (tRP_ConfigGreenChange)(uint ID, float Cl);
			typedef bool (tRP_ConfigBlueChange)(uint ID, float Cl);
			typedef bool (tRP_ConfigInitialAlpha)(uint ID, float Alpha);
			typedef bool (tRP_ConfigAlphaChange)(uint ID, float Alpha);
			typedef bool (tRP_ConfigFaceEntity)(uint ID, uint Entity);
			typedef bool (tRP_ConfigBlendMode)(uint ID, int Blend);
			typedef bool (tRP_ConfigMaxParticles)(uint ID, int MaxParticles);
			typedef bool (tRP_ConfigInitialScale)(uint ID, float Scale);
			typedef bool (tRP_ConfigScaleChange)(uint ID, float Scale);
			typedef bool (tRP_ConfigTexture)(uint ID, uint Texture, int TilesX, int TilesY, bool FreePreviousTexture);
			typedef bool (tRP_ConfigTextureAnimSpeed)(uint ID, int Speed);
			typedef bool (tRP_ConfigTextureRandomStartFrame)(uint ID, int Flag);
			typedef bool (tRP_ConfigShapeSphere)(uint ID, float MinRadius, float MaxRadius);
			typedef bool (tRP_ConfigShapeCylinder)(uint ID, float MinRadius, float MaxRadius, float Length, int Axis);
			typedef bool (tRP_ConfigShapeBox)(uint ID, float Width, float Height, float Depth);
			typedef uint (tRP_CreateEmitterConfig)(int MaxParticles, int SpawnRate, uint FaceEntity, uint Texture, int TilesX, int TilesY, const char* Name);
			typedef uint (tRP_CopyEmitterConfig)(uint ID);
			typedef bool (tRP_FreeEmitterConfig)(uint ID, uint FreeTex);
			typedef uint (tRP_LoadEmitterConfig)(const char* File, uint Texture, uint FaceEntity);
			typedef uint (tRP_CreateEmitter)(uint Configuration, float Scale);
			typedef int  (tRP_EmitterActiveParticles)(uint ID);
			typedef bool (tRP_EnableEmitter)(uint ID);
			typedef bool (tRP_DisableEmitter)(uint ID);
			typedef bool (tRP_HideEmitter)(uint ID);
			typedef bool (tRP_ShowEmitter)(uint ID);
			typedef bool (tRP_ScaleEmitter)(uint ID, float Scale);
			typedef bool (tRP_KillEmitter)(uint ID, bool FreeConfig, bool FreeTex);
			typedef bool (tRP_FreeEmitter)(uint ID, bool FreeConfig, bool FreeTex);
			typedef void (tRP_Clear)(bool Configs, bool Textures);
			typedef void (tRemoveUnderwaterParticlesfn)(int emitter);
			typedef void (tScreenFlashfn)(int r, int g, int b, int textureID, int length, float initialAlpha);
			typedef bool (tInventorySwap)(int SlotA, int SlotB, int Amount);
			typedef void (tInventoryDrop)(int SlotFrom, int Amount);
			typedef void (tUseItem)(int SlotIndex, int Amount);
			typedef std::string (tGetItemPathFromID)(int id);
			typedef std::string (tGetSpellPathFromID)(int id);
			typedef std::string (tGetSpellName)(int id);
			typedef std::string (tGetItemName)(int id);
			typedef std::string (tGetItemTypeString)(unsigned short itemID);
			typedef bool (tGetItemTakesDamage)(unsigned short itemID);
			typedef int (tGetItemHealth)(int slot);
			typedef int (tGetItemValue)(unsigned short itemID);
			typedef int (tGetItemMass)(unsigned short itemID);
			typedef bool (tGetItemStackable)(unsigned short itemID);
			typedef int (tGetItemType)(unsigned short itemID);
			typedef int (tGetItemWeaponDamage)(unsigned short itemID);
			typedef std::string (tGetItemWeaponDamageType)(unsigned short itemID);
			typedef std::string (tGetItemWeaponType)(unsigned short itemID);
			typedef int (tGetItemArmourLevel)(unsigned short itemID);
			typedef int (tGetItemEatEffects)(unsigned short itemID);
			typedef std::string (tGetItemExclusiveRace)(unsigned short itemID);
			typedef std::string (tGetItemExclusiveClass)(unsigned short itemID);
			typedef bool (tGetPlayerIsExclusiveRace)(unsigned short itemID);
			typedef bool (tGetPlayerIsExclusiveClass)(unsigned short itemID);
			typedef std::string (tGetSpellDescription)(unsigned short spellID);
			typedef std::string (tGetSpellNameFromID)(int id);
		}

		// Engine functions
		struct RCCommandData
		{
			__SDKDef::tCanMovePlayerfn* CanMovePlayer;
			__SDKDef::tControlHitfn* ControlHit;
			__SDKDef::tControlDownfn* ControlDown;
			__SDKDef::tControlDownFfn* ControlDownF;
			__SDKDef::tControlNamefn* ControlName;

			__SDKDef::tRP_Update* RP_Update;
			__SDKDef::tRP_ConfigLifespan* RP_ConfigLifespan;
			__SDKDef::tRP_ConfigSpawnRate* RP_ConfigSpawnRate;
			__SDKDef::tRP_ConfigVelocityMode* RP_ConfigVelocityMode;
			__SDKDef::tRP_ConfigVelocityX* RP_ConfigVelocityX;
			__SDKDef::tRP_ConfigVelocityY* RP_ConfigVelocityY;
			__SDKDef::tRP_ConfigVelocityZ* RP_ConfigVelocityZ;
			__SDKDef::tRP_ConfigVelocityRndX* RP_ConfigVelocityRndX;
			__SDKDef::tRP_ConfigVelocityRndY* RP_ConfigVelocityRndY;
			__SDKDef::tRP_ConfigVelocityRndZ* RP_ConfigVelocityRndZ;
			__SDKDef::tRP_ConfigForceX* RP_ConfigForceX;
			__SDKDef::tRP_ConfigForceY* RP_ConfigForceY;
			__SDKDef::tRP_ConfigForceZ* RP_ConfigForceZ;
			__SDKDef::tRP_ConfigForceModMode* RP_ConfigForceModMode;
			__SDKDef::tRP_ConfigForceModX* RP_ConfigForceModX;
			__SDKDef::tRP_ConfigForceModY* RP_ConfigForceModY;
			__SDKDef::tRP_ConfigForceModZ* RP_ConfigForceModZ;
			__SDKDef::tRP_ConfigInitialRed* RP_ConfigInitialRed;
			__SDKDef::tRP_ConfigInitialGreen* RP_ConfigInitialGreen;
			__SDKDef::tRP_ConfigInitialBlue* RP_ConfigInitialBlue;
			__SDKDef::tRP_ConfigRedChange* RP_ConfigRedChange;
			__SDKDef::tRP_ConfigGreenChange* RP_ConfigGreenChange;
			__SDKDef::tRP_ConfigBlueChange* RP_ConfigBlueChange;
			__SDKDef::tRP_ConfigInitialAlpha* RP_ConfigInitialAlpha;
			__SDKDef::tRP_ConfigAlphaChange* RP_ConfigAlphaChange;
			__SDKDef::tRP_ConfigFaceEntity* RP_ConfigFaceEntity;
			__SDKDef::tRP_ConfigBlendMode* RP_ConfigBlendMode;
			__SDKDef::tRP_ConfigMaxParticles* RP_ConfigMaxParticles;
			__SDKDef::tRP_ConfigInitialScale* RP_ConfigInitialScale;
			__SDKDef::tRP_ConfigScaleChange* RP_ConfigScaleChange;
			__SDKDef::tRP_ConfigTexture* RP_ConfigTexture;
			__SDKDef::tRP_ConfigTextureAnimSpeed* RP_ConfigTextureAnimSpeed;
			__SDKDef::tRP_ConfigTextureRandomStartFrame* RP_ConfigTextureRandomStartFrame;
			__SDKDef::tRP_ConfigShapeSphere* RP_ConfigShapeSphere;
			__SDKDef::tRP_ConfigShapeCylinder* RP_ConfigShapeCylinder;
			__SDKDef::tRP_ConfigShapeBox* RP_ConfigShapeBox;
			__SDKDef::tRP_CreateEmitterConfig* RP_CreateEmitterConfig;
			__SDKDef::tRP_CopyEmitterConfig* RP_CopyEmitterConfig;
			__SDKDef::tRP_FreeEmitterConfig* RP_FreeEmitterConfig;
			__SDKDef::tRP_LoadEmitterConfig* RP_LoadEmitterConfig;
			__SDKDef::tRP_CreateEmitter* RP_CreateEmitter;
			__SDKDef::tRP_EmitterActiveParticles* RP_EmitterActiveParticles;
			__SDKDef::tRP_EnableEmitter* RP_EnableEmitter;
			__SDKDef::tRP_DisableEmitter* RP_DisableEmitter;
			__SDKDef::tRP_HideEmitter* RP_HideEmitter;
			__SDKDef::tRP_ShowEmitter* RP_ShowEmitter;
			__SDKDef::tRP_ScaleEmitter* RP_ScaleEmitter;
			__SDKDef::tRP_KillEmitter* RP_KillEmitter;
			__SDKDef::tRP_FreeEmitter* RP_FreeEmitter;
			__SDKDef::tRP_Clear* RP_Clear;
			__SDKDef::tRemoveUnderwaterParticlesfn* RemoveUnderwaterParticles;
			__SDKDef::tScreenFlashfn* ScreenFlash;

			__SDKDef::tInventorySwap* InventorySwap;
			__SDKDef::tInventoryDrop* InventoryDrop;
			__SDKDef::tUseItem* UseItem;
			__SDKDef::tGetItemPathFromID* GetItemPathFromID;
			__SDKDef::tGetSpellPathFromID* GetSpellPathFromID;
			__SDKDef::tGetSpellName* GetSpellName;
			__SDKDef::tGetItemName* GetItemName;
			__SDKDef::tGetItemTypeString* GetItemTypeString;
			__SDKDef::tGetItemTakesDamage* GetItemTakesDamage;
			__SDKDef::tGetItemHealth* GetItemHealth;
			__SDKDef::tGetItemValue* GetItemValue;
			__SDKDef::tGetItemMass* GetItemMass;
			__SDKDef::tGetItemStackable* GetItemStackable;
			__SDKDef::tGetItemType* GetItemType;
			__SDKDef::tGetItemWeaponDamage* GetItemWeaponDamage;
			__SDKDef::tGetItemWeaponDamageType* GetItemWeaponDamageType;
			__SDKDef::tGetItemWeaponType* GetItemWeaponType;
			__SDKDef::tGetItemArmourLevel* GetItemArmourLevel;
			__SDKDef::tGetItemEatEffects* GetItemEatEffects;
			__SDKDef::tGetItemExclusiveRace* GetItemExclusiveRace;
			__SDKDef::tGetItemExclusiveClass* GetItemExclusiveClass;
			__SDKDef::tGetPlayerIsExclusiveRace* GetPlayerIsExclusiveRace;
			__SDKDef::tGetPlayerIsExclusiveClass* GetPlayerIsExclusiveClass;
			__SDKDef::tGetSpellDescription* GetSpellDescription;
			__SDKDef::tGetSpellNameFromID* GetSpellNameFromID;
			

			NGin::GUI::GUIUpdateParameters* NGUIUpdateParameters;
		};
		extern RCCommandData* RCCommands;

		// External globals
#ifdef RC_SDK
		extern NGin::GUI::GUIUpdateParameters* NGUIUpdateParameters;
#endif
	}

	// SDK Start
#ifdef RC_SDK
	DLLOUT int StartRCSDK(SDK::RCCommandData* rcCommandData, SGlobals* globals, int);
#else
	typedef int (tStartRCSDKfn)(SDK::RCCommandData* rcCommandData, SGlobals* globals, int);
#endif

#ifdef RC_SDK

	// This enumeration is SDK only. The client source uses the same but in a different scope.
	enum ItemSlots
	{
		SlotI_Weapon    = 0,
		SlotI_Shield    = 1,
		SlotI_Hat       = 2,
		SlotI_Chest     = 3,
		SlotI_Hand      = 4,
		SlotI_Belt      = 5,
		SlotI_Legs      = 6,
		SlotI_Feet      = 7,
		SlotI_Ring1     = 8,
		SlotI_Ring2     = 9,
		SlotI_Ring3     = 10,
		SlotI_Ring4     = 11,
		SlotI_Amulet1   = 12,
		SlotI_Amulet2   = 13,
		SlotI_Backpack  = 14
	};

	// This enumeration is SDK only. The client source uses the same but in a different scope.
	enum ItemTypes
	{
		I_Weapon      = 1,
		I_Armour      = 2,
		I_Ring        = 3,
		I_Potion      = 4,
		I_Ingredient  = 5,
		I_Image       = 6,
		I_Other       = 7
	};

	inline bool InventorySwap(int slotA, int slotB, int amount = 0)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->InventorySwap(slotA, slotB, amount);
	}

	inline void InventoryDrop(int slotFrom, int amount)
	{
		if(SDK::RCCommands == NULL)
			return ;
		return SDK::RCCommands->InventoryDrop(slotFrom, amount);
	}

	inline void UseItem(int slotIndex, int amount)
	{
		if(SDK::RCCommands == NULL)
			return ;
		return SDK::RCCommands->UseItem(slotIndex, amount);
	}


	inline std::string GetItemPathFromID(int id)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemPathFromID(id);
	}

	inline std::string GetSpellPathFromID(int id)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetSpellPathFromID(id);
	}

	inline std::string GetSpellName(int id)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetSpellName(id);
	}

	inline std::string GetItemName(int id)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemName(id);
	}

	inline std::string GetItemTypeString(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemTypeString(itemID);
	}

	inline bool GetItemTakesDamage(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->GetItemTakesDamage(itemID);
	}

	inline int GetItemHealth(int slot)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemHealth(slot);
	}

	inline int GetItemValue(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemValue(itemID);
	}

	inline int GetItemMass(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemMass(itemID);
	}

	inline bool GetItemStackable(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->GetItemStackable(itemID);
	}

	inline int GetItemType(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemType(itemID);
	}

	inline int GetItemWeaponDamage(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemWeaponDamage(itemID);
	}

	inline std::string GetItemWeaponDamageType(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemWeaponDamageType(itemID);
	}

	inline std::string GetItemWeaponType(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemWeaponType(itemID);
	}

	inline int GetItemArmourLevel(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemArmourLevel(itemID);
	}

	inline int GetItemEatEffects(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->GetItemEatEffects(itemID);
	}

	inline std::string GetItemExclusiveRace(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemExclusiveRace(itemID);
	}

	inline std::string GetItemExclusiveClass(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetItemExclusiveClass(itemID);
	}

	inline bool GetPlayerIsExclusiveRace(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->GetPlayerIsExclusiveRace(itemID);
	}

	inline bool GetPlayerIsExclusiveClass(unsigned short itemID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->GetPlayerIsExclusiveClass(itemID);
	}

	inline std::string GetSpellDescription(unsigned short spellID)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetSpellDescription(spellID);
	}

	inline std::string GetSpellNameFromID(int id)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->GetSpellNameFromID(id);
	}


	inline void RemoveUnderwaterParticles(int emitter)
	{
		if(SDK::RCCommands == NULL)
			return ;
		return SDK::RCCommands->RemoveUnderwaterParticles(emitter);
	}

	inline void ScreenFlash(int r, int g, int b, int textureID, int length, float initialAlpha)
	{
		if(SDK::RCCommands == NULL)
			return ;
		return SDK::RCCommands->ScreenFlash(r, g, b, textureID, length, initialAlpha);
	}

	inline bool CanMovePlayer()
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->CanMovePlayer();
	}

	inline bool ControlHit(int ctrl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->ControlHit(ctrl);
	}

	inline bool ControlDown(int ctrl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->ControlDown(ctrl);
	}

	inline float ControlDownF(int ctrl)
	{
		if(SDK::RCCommands == NULL)
			return 0.0f;
		return SDK::RCCommands->ControlDownF(ctrl);
	}

	inline std::string ControlName(int ctrl)
	{
		if(SDK::RCCommands == NULL)
			return "";
		return SDK::RCCommands->ControlName(ctrl);
	}


	inline void RP_Update(float Delta = 1.0f)
	{
		if(SDK::RCCommands == NULL)
			return ;
		return SDK::RCCommands->RP_Update(Delta);
	}

	inline bool RP_ConfigLifespan(uint ID, int Lifespan)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigLifespan(ID, Lifespan);
	}

	inline bool RP_ConfigSpawnRate(uint ID, int SpawnRate)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigSpawnRate(ID, SpawnRate);
	}

	inline bool RP_ConfigVelocityMode(uint ID, int Level)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityMode(ID, Level);
	}

	inline bool RP_ConfigVelocityX(uint ID, float VelocityX)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityX(ID, VelocityX);
	}

	inline bool RP_ConfigVelocityY(uint ID, float VelocityY)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityY(ID, VelocityY);
	}

	inline bool RP_ConfigVelocityZ(uint ID, float VelocityZ)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityZ(ID, VelocityZ);
	}

	inline bool RP_ConfigVelocityRndX(uint ID, float VelocityX)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityRndX(ID, VelocityX);
	}

	inline bool RP_ConfigVelocityRndY(uint ID, float VelocityY)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityRndY(ID, VelocityY);
	}

	inline bool RP_ConfigVelocityRndZ(uint ID, float VelocityZ)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigVelocityRndZ(ID, VelocityZ);
	}

	inline bool RP_ConfigForceX(uint ID, float VelocityX)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceX(ID, VelocityX);
	}

	inline bool RP_ConfigForceY(uint ID, float VelocityY)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceY(ID, VelocityY);
	}

	inline bool RP_ConfigForceZ(uint ID, float VelocityZ)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceZ(ID, VelocityZ);
	}

	inline bool RP_ConfigForceModMode(uint ID, int Mode)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceModMode(ID, Mode);
	}

	inline bool RP_ConfigForceModX(uint ID, float ForceModX)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceModX(ID, ForceModX);
	}

	inline bool RP_ConfigForceModY(uint ID, float ForceModY)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceModY(ID, ForceModY);
	}

	inline bool RP_ConfigForceModZ(uint ID, float ForceModZ)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigForceModZ(ID, ForceModZ);
	}

	inline bool RP_ConfigInitialRed(uint ID, int Cl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigInitialRed(ID, Cl);
	}

	inline bool RP_ConfigInitialGreen(uint ID, int Cl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigInitialGreen(ID, Cl);
	}

	inline bool RP_ConfigInitialBlue(uint ID, int Cl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigInitialBlue(ID, Cl);
	}

	inline bool RP_ConfigRedChange(uint ID, float Cl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigRedChange(ID, Cl);
	}

	inline bool RP_ConfigGreenChange(uint ID, float Cl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigGreenChange(ID, Cl);
	}

	inline bool RP_ConfigBlueChange(uint ID, float Cl)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigBlueChange(ID, Cl);
	}

	inline bool RP_ConfigInitialAlpha(uint ID, float Alpha)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigInitialAlpha(ID, Alpha);
	}

	inline bool RP_ConfigAlphaChange(uint ID, float Alpha)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigAlphaChange(ID, Alpha);
	}

	inline bool RP_ConfigFaceEntity(uint ID, uint Entity)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigFaceEntity(ID, Entity);
	}

	inline bool RP_ConfigBlendMode(uint ID, int Blend)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigBlendMode(ID, Blend);
	}

	inline bool RP_ConfigMaxParticles(uint ID, int MaxParticles)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigMaxParticles(ID, MaxParticles);
	}

	inline bool RP_ConfigInitialScale(uint ID, float Scale)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigInitialScale(ID, Scale);
	}

	inline bool RP_ConfigScaleChange(uint ID, float Scale)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigScaleChange(ID, Scale);
	}

	inline bool RP_ConfigTexture(uint ID, uint Texture, int TilesX, int TilesY, bool FreePreviousTexture = true)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigTexture(ID, Texture, TilesX, TilesY, FreePreviousTexture);
	}

	inline bool RP_ConfigTextureAnimSpeed(uint ID, int Speed)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigTextureAnimSpeed(ID, Speed);
	}

	inline bool RP_ConfigTextureRandomStartFrame(uint ID, int Flag)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigTextureRandomStartFrame(ID, Flag);
	}

	inline bool RP_ConfigShapeSphere(uint ID, float MinRadius = 0.0f, float MaxRadius = 0.0f)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigShapeSphere(ID, MinRadius, MaxRadius);
	}

	inline bool RP_ConfigShapeCylinder(uint ID, float MinRadius, float MaxRadius, float Length, int Axis)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigShapeCylinder(ID, MinRadius, MaxRadius, Length, Axis);
	}

	inline bool RP_ConfigShapeBox(uint ID, float Width, float Height, float Depth)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ConfigShapeBox(ID, Width, Height, Depth);
	}

	inline uint RP_CreateEmitterConfig(int MaxParticles, int SpawnRate, uint FaceEntity, uint Texture, int TilesX = 1, int TilesY = 1, std::string Name = "New Emitter")
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->RP_CreateEmitterConfig(MaxParticles, SpawnRate, FaceEntity, Texture, TilesX, TilesY, Name.c_str());
	}

	inline uint  RP_CopyEmitterConfig(uint ID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->RP_CopyEmitterConfig(ID);
	}

	inline bool RP_FreeEmitterConfig(uint ID, uint FreeTex)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_FreeEmitterConfig(ID, FreeTex);
	}

	inline uint RP_LoadEmitterConfig(std::string File, uint Texture, uint FaceEntity)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->RP_LoadEmitterConfig(File.c_str(), Texture, FaceEntity);
	}

	inline uint RP_CreateEmitter(uint Configuration, float Scale = 1.0f)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->RP_CreateEmitter(Configuration, Scale);
	}

	inline int RP_EmitterActiveParticles(uint ID)
	{
		if(SDK::RCCommands == NULL)
			return 0;
		return SDK::RCCommands->RP_EmitterActiveParticles(ID);
	}

	inline bool RP_EnableEmitter(uint ID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_EnableEmitter(ID);
	}

	inline bool RP_DisableEmitter(uint ID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_DisableEmitter(ID);
	}

	inline bool RP_HideEmitter(uint ID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_HideEmitter(ID);
	}

	inline bool RP_ShowEmitter(uint ID)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ShowEmitter(ID);
	}

	inline bool RP_ScaleEmitter(uint ID, float Scale)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_ScaleEmitter(ID, Scale); 
	}

	inline bool RP_KillEmitter(uint ID, bool FreeConfig = false, bool FreeTex = false)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_KillEmitter(ID, FreeConfig, FreeTex);
	}

	inline bool RP_FreeEmitter(uint ID, bool FreeConfig = false, bool FreeTex = false)
	{
		if(SDK::RCCommands == NULL)
			return false;
		return SDK::RCCommands->RP_FreeEmitter(ID, FreeConfig, FreeTex);
	}

	inline void RP_Clear(bool Configs = true, bool Textures = true)	
	{
		if(SDK::RCCommands == NULL)
			return ;
		return SDK::RCCommands->RP_Clear(Configs, Textures);
	}

#endif
}