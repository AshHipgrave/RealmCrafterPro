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
#include "CBubbleOutput.h"
#include <IControlLayout.h>
/*#include "cRadar.h"*/

#include <list>
#include "Actors.h"

struct ItemInstance;

// Dialogs
struct Dialog
{
	IWindow* Win;
	ILabel* TextLines[14];
	string TextText[14];
	int TextR[14];
	int TextG[14];
	int TextB[14];
	int OptionNum[14];
	int TotalOptions;
	int LastLine;
	ActorInstance* ActorInstance;
	uint ScriptHandle;

	ClassDecl(Dialog, DialogList, DialogDelete);
};

// Scripted progress bars
struct ScriptProgressBar
{
	IProgressBar* BarHandle;
	uint ScriptHandle;

	ClassDecl(ScriptProgressBar, ScriptProgressBarList, ScriptProgressBarDelete);
};

struct TextInput
{
	IWindow* Win;
	ITextBox* TextBox;
	IButton* AcceptButton;
	uint ScriptHandle;

	ClassDecl(TextInput, TextInputList, TextInputDelete);
};


struct Bubble
{
	RealmCrafter::CBubbleOutput* EN;
	float Width, Height;
	int Timer;
	ActorInstance* ActorInstance;

	ClassDecl(Bubble, BubbleList, BubbleDelete);
};

// Actor effect icons
struct EffectIcon
{
	string Name;
	uint ID, TextureID;

	ClassDecl(EffectIcon, EffectIconList, EffectIconDelete);
};

struct EffectIconSlot
{
	IPictureBox* EN;
	EffectIcon* Effect;

	ClassDecl(EffectIconSlot, EffectIconSlotList, EffectIconSlotDelete);
};

// Interface component settings
struct InterfaceComponent
{
	float X, Y, Width, Height; // Position/size in fraction of screen size
	float Alpha;                  // Transparency level 0.0 - 1.0
	uint Component;               // Handle to an entity, Gooey gadget, whatever
	int Texture;                 // TextureID for entity
	int R, G, B;                 // Colours for a Gooey gadget

	ClassDecl(InterfaceComponent, InterfaceComponentList, InterfaceComponentDelete);
};

// Dreamora: 1.23
extern bool ItemWindowFromInventory;
extern IWindow* WItemWindow;

// JB: 2.50 controls
extern RealmCrafter::IControlLayout* ControlLayout;

// JB: 2.52 Fixed Controls
extern RealmCrafter::IPlayerAttributeBars* PlayerAttributeBars;
extern RealmCrafter::IPlayerMap* PlayerMap;
extern bool PlayerMapVisible;
extern RealmCrafter::IPlayerCompass* PlayerCompass;
extern bool QuestLogVisible;
extern RealmCrafter::IPlayerQuestLog* PlayerQuestLog;
extern bool CharStatsVisible;
extern RealmCrafter::IPlayerStats* PlayerStats;
extern bool HelpVisible;
extern RealmCrafter::IPlayerHelp* PlayerHelp;
extern bool PartyVisible;
extern RealmCrafter::IPlayerParty* PlayerParty;

// Chat bubbles
extern char UseBubbles;
extern int BubblesR, BubblesG, BubblesB;
extern uint ChatBubbleFont, ChatBubbleEN, ChatBubbleTex;


extern RealmCrafter::IPlayerActionBar* PlayerActionBar;
extern int ActionBarSlots[36]; // Values greater than 0 are item IDs, -1 to -10 are memorised spells, or if memorisation isn't used -1 to -1000 are known spells

// Radar and map
typedef std::pair<ActorInstance *, IPictureBox*> tpMark;

extern bool ShowRadar, ShowActorsOnRadar;
extern int idPlayer, idEnemy, idFriendly, idNPC, idOthers;
extern std::list<tpMark> lpRadarMarks;	
extern IPictureBox* pRadarMainPlayer;

extern bool InventoryVisible;
extern RealmCrafter::IPlayerInventory* PlayerInventory;

// Spells (abilities) window
extern RealmCrafter::IPlayerSpells* PlayerSpells;


extern bool SpellsVisible;
extern IWindow* WSpellRemove;
extern IButton* BSpellRemoveOK, *BSpellRemoveCancel;
extern int SpellRemoveNum;
extern IWindow* WSpellError;
extern IButton* BSpellError;
extern ILabel* LMemorising;
extern IProgressBar* SMemorising;
extern int MemoriseSpell, MemoriseSlot, MemoriseProgress, LastMemoriseUpdate;
extern uint LastSpellRecharge;

// Misc
extern int LastMouseMove, LastLeftClick, MouseControl, InWaitPeriod, MouseDownTime, MouseClicks;
extern bool MouseWasDown;
extern uint RightDownTime;
extern int SelectKeyClickTime;
extern bool InDialog, SelectKeyWasDown, RightWasDown;

extern ITextBox* ChatEntry;
extern IRadar* RadarBox;


// Check if a textbox is in focus
bool TextBoxIsFocused();

// Gets the script handle for a dialog
uint DialogScriptHandle(uint Han);

// Loads control bindings
// bool LoadControlBindings(string Filename);
//
// Saves control bindings
// bool SaveControlBindings(string Filename);

// Returns the position at which a string should be split to word wrap to a maximum number of characters
int WordWrap(string St, int MaxChars);

// Returns the number of times a control has been pressed since the last call
bool ControlHit(int Ctrl);

float ControlDownF(int Ctrl);

// Returns whether the specified control is being held down
bool ControlDown(int Ctrl);

// Returns the name of a control number
string ControlName(int ControlNumber);

// Returns true of no dialogs are open preventing the player from moving.
bool CanMovePlayer();

