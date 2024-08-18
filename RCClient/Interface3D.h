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

#include "Actors.h"
#include "Gooey.h"
#include "Items.h"

class ActorInstance;

// Alphabetically sorted list of abilities
extern int KnownSpellSort[1000];

extern bool EnableClickToMove;

#ifdef CreateDialog
#undef CreateDialog
#endif

uint CreateTextInput(string Title, string Prompt, int Numeric, uint ScriptHandle);
void FreeTextInput(uint Han);
uint CreateDialog(string Title, ActorInstance* A, uint ScriptHandle, int BackgroundTexID = 65535);
void FreeDialog(uint Han);
void AddDialogOption(uint Han, string Opt);
void DialogOutput(uint Han, string T, int R = 255, int G = 255, int B = 255, int Opt = 0);
void Output(string Dat, int R = 255, int G = 255, int B = 255);
void BubbleOutput(string Label, int R, int G, int B, ActorInstance* AI, bool NoText = false);
string GetTarget(uint EN);
void UpdateCompass();
void UpdateRadar( ActorInstance *pMainPlayer );
void AddRadarMark( ActorInstance *pActor, int IdMark );
void RemoveRadarMark( ActorInstance *pActor );
void UpdateEffectIcons();
void CreateInterface();
void EnableInventoryBlanks(bool Disable = false);
void SetPickModes(int Scenery = 0, int Actors = 0, bool NonCombatants = true, int DroppedItems = 0);
IButton* CreateActionBarButton(string TexName, float X, bool Toggle = true, bool FreeTex = true);
void CreateInventoryButton(IWindow* W, int S, string Tex);
void UseItem(int SlotIndex, int Amount);
void UpdateInterface();
void PlayerMap_Closed(IControl* Sender, EventArgs* E);
void GameMenu_LogoutTimerStart(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
void GameMenu_OptionsClick(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
void GameMenu_Logout(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
void GameMenu_Quit(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);

void HideInterface();
void ShowDefaultInterface();



