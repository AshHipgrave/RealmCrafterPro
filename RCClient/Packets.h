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

// Packet types
#define P_CreateAccount       1
#define P_VerifyAccount       2
#define P_FetchCharacter      3
#define P_CreateCharacter     4
#define P_DeleteCharacter     5
#define P_ChangePassword      6
#define P_FetchActors         7
#define P_FetchItems          8
#define P_ChangeArea          9
#define P_FetchUpdateFiles    10
#define P_NewActor            11
#define P_StartGame           12
#define P_ActorGone           13
#define P_StandardUpdate      14
#define P_InventoryUpdate     15
#define P_ChatMessage         16
#define P_WeatherChange       17
#define P_AttackActor         18
#define P_ActorDead           19
#define P_RightClick          20
#define P_Dialog              21
#define P_StatUpdate          22
#define P_QuestLog            23
#define P_GoldChange          24
#define P_NameChange          25
#define P_KnownSpellUpdate    26
#define P_SpellUpdate         27
#define P_CreateEmitter       28
#define P_Sound               29
#define P_AnimateActor        30
#define P_ActionBarUpdate     31
#define P_XPUpdate            32
#define P_ScreenFlash         33
#define P_Music               34
#define P_SceneryInteract     35
#define P_ActorEffect         36
#define P_Projectile          37
#define P_PartyUpdate         38
#define P_AppearanceUpdate    39
#define P_ShaderConstant      40
//#define P_UpdateTrading       41
#define P_SelectScenery       42
#define P_ItemScript          43
#define P_EatItem             44
#define P_ItemHealth          45
#define P_Jump                46
#define P_Dismount            47
#define P_FloatingNumber      48
#define P_RepositionActor     49
#define P_Speech              50
#define P_ProgressBar         51
#define P_BubbleMessage       52
#define P_ScriptInput         53
#define P_KickedPlayer		  60

#define P_ConnectInit 62
#define P_ReAllocateIDs 66


// GUI
#define P_OpenForm 71
#define P_PropertiesUpdate 72
#define P_GUIEvent 73
#define P_CloseForm 74

// For chat tbas
#define P_ChatTab 78