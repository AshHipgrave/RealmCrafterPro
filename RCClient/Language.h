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
#include <BlitzPlus.h>

// Language string IDS
#define MaxLanguageString 213
#define LS_ConnectingToServer      0
#define LS_FileProgress            1
#define LS_UpdateFile              2
#define LS_UpdateProgress          3
#define LS_InvalidHost             4
#define LS_NoResponse              5
#define LS_TooManyPlayers          6
#define LS_LostConnection          7
#define LS_ReceivingFiles          8
#define LS_CheckingFiles           9
#define LS_CouldNotDownload        10
#define LS_Username                11
#define LS_Password                12
#define LS_EmailAddr               13
#define LS_Connected               14
#define LS_Error                   15
#define LS_InvalidUsername         16
#define LS_InvalidPassword         17
#define LS_WaitingForReply         18
#define LS_DownloadingChars        19
#define LS_InvalidCharName         20
#define LS_AttributePoints         21
#define LS_GraphicsOptions         22
#define LS_SelectResolution        23
#define LS_EnableAA                24
#define LS_ColourDepth             25
#define LS_SelectColourDepth       26
#define LS_BestAvailable           27
#define LS_OtherOptions            28
#define LS_SndVolume               29
#define LS_SkipMusic               30
#define LS_ControlOptions          31
#define LS_CForward                32
#define LS_CStop                   33
#define LS_CTurnRight              34
#define LS_CTurnLeft               35
#define LS_CFlyUp                  36
#define LS_CFlyDown                37
#define LS_CRun                    38
#define LS_CJump                   39
#define LS_CViewMode               40
#define LS_CZoomIn                 41
#define LS_CZoomOut                42
#define LS_CCamRight               43
#define LS_CCamLeft                44
#define LS_CPressKey               45
#define LS_Unknown                 46
#define LS_AlreadyInGame           47
#define LS_Weapon                  48
#define LS_Armour                  49
#define LS_Ring                    50
#define LS_Amulet                  51
#define LS_Potion                  52
#define LS_Ingredient              53
#define LS_Image                   54
#define LS_Miscellaneous           55
#define LS_OneHanded               56
#define LS_TwoHanded               57
#define LS_Ranged                  58
#define LS_Money                   59
#define LS_Cost                    60
#define LS_XPReceived              61
#define LS_QuestLogUpdate          62
#define LS_YouKilled               63
#define LS_PickedUpItem            64
#define LS_PlayerLeftZone          65
#define LS_PlayerEnteredZone       66
#define LS_EnteredZone             67
#define LS_XHasJoinedParty         68
#define LS_YouHaveJoinedParty      69
#define LS_CouldNotJoinParty       70
#define LS_PartyInvite             71
#define LS_PartyInviteInstruction  72
#define LS_CouldNotInviteParty     73
#define LS_TradeInvite             74
#define LS_TradeInviteInstruction  75
#define LS_XIsOffline              76
#define LS_PlayerNotFound          77
#define LS_PlayersInGame           78
#define LS_PlayersInZone           79
#define LS_Season                  80
#define LS_AbilityNotRecharged     81
#define LS_YouHit                  82
#define LS_For                     83
#define LS_DamageWow               84
#define LS_HitsYou                 85
#define LS_AttacksYouMisses        86
#define LS_YouAttack               87
#define LS_AndMiss                 88
#define LS_NoInventorySpace        89
#define LS_MemoriseAbility         90
#define LS_MemorisingAbility       91
#define LS_MaximumMemorised        92
#define LS_AlreadyMemorised        93
#define LS_Reputation              94
#define LS_Level                   95
#define LS_Experience              96
#define LS_ToUse                   97
#define LS_Type                    98
#define LS_Damage                  99
#define LS_Indestructible          100
#define LS_Value                   101
#define LS_Mass                    102
#define LS_CanBeStacked            103
#define LS_CannotBeStacked         104
#define LS_DamageType              105
#define LS_WeaponType              106
#define LS_ArmourLevel             107
#define LS_EffectsLast             108
#define LS_Seconds                 109
#define LS_RaceOnly                110
#define LS_ClassOnly               111
#define LS_NoDescription           112
#define LS_MemorisedYouMust        113
#define LS_MoveItToActionBar       114
#define LS_Trading                 115
#define LS_TradingNoSpace          116
#define LS_TradingNoMoney          117
#define LS_EscapeAgainToQuit       118
#define LS_MemorisedAbilities      119
#define LS_Page                    120
#define LS_QuitProgress            121
#define LS_Map                     122
#define LS_ChooseAmount            123
#define LS_ChooseAmountDetail      124
#define LS_Party                   125
#define LS_LeaveParty              126
#define LS_Abilities               127
#define LS_Unmemorise              128
#define LS_UnmemoriseDetail        129
#define LS_Yes                     130
#define LS_No                      131
#define LS_Quests                  132
#define LS_ShowCompleted           133
#define LS_Up                      134
#define LS_Down                    135
#define LS_Character               136
#define LS_Attributes              137
#define LS_Inventory               138
#define LS_Drop                    139
#define LS_Use                     140
#define LS_IncreaseCost            141
#define LS_DecreaseCost            142
#define LS_Accept                  143
#define LS_Cancel                  144
#define LS_NoQuestsAvailable       145
#define LS_Completed               146
#define LS_CharacterTitle          147
#define LS_AttributesTitle         148
#define LS_Race                    149
#define LS_Gender                  150
#define LS_Class                   151
#define LS_Hair                    152
#define LS_Face                    153
#define LS_Beard                   154
#define LS_Clothes                 155
#define LS_CharacterName           156
#define LS_UnusedPoints            157
#define LS_Warning                 158
#define LS_Decline                 159
#define LS_InvertAxis1             160
#define LS_InvertAxis3             161
#define LS_CAttackTarget           162
#define LS_CannotCreateChar        163
#define LS_ReallyDeleteChar        164
#define LS_CAlwaysRun              165
#define LS_CCycleTarget            166
#define LS_WindowedMode            167
#define LS_YouAreBanned            168
#define LS_AccountDoesNotExist     169
#define LS_InvalidEmailAddress     170
#define LS_UsernameAlreadyExists   171
#define LS_Success                 172
#define LS_NewAccountCreated       173
#define LS_CriticalDamage          174
#define LS_WeaponDamaged           175
#define LS_Ignoring                176
#define LS_UnIgnoring              177
#define LS_CMoveTo                 178
#define LS_CTalkTo                 179
#define LS_Faction                 180
#define LS_Interact                181
#define LS_RetrievingList          182
#define LS_Connect                 183
#define LS_AvailableServers        184
#define LS_CSelect                 185
#define LS_AnisotropyLevel         186
#define LS_Disabled                187
#define LS_NewCharacter            188
#define LS_Help                    189
#define LS_ShadowDisabled			190
#define LS_ShadowPlayers			191
#define LS_ShadowAllActors			192
#define LS_ShadowScenery			193
#define LS_ShadowTrees				194
#define LS_ShadowDetail				195
#define LS_ShaderQuality			196
#define LS_QualityHigh				197
#define LS_QualityMedium			198
#define LS_QualityLow				199
#define LS_GrassDistance			200
#define LS_OptionFullscreen			201
#define LS_OK						202

#define LS_LoadDefaultShaders	203
#define LS_LoadShadowShaders	204
#define LS_LoadUserShaders		205
#define LS_LoadUserParameters	206
#define LS_LoadTerrainManager	207
#define LS_LoadTreeManager		208
#define LS_LoadSDK				209
#define LS_LoadOptions			210
#define LS_LoadGubbins			211
#define LS_LoadClient			212

extern string LanguageStringNames[];

// Array to store strings
extern string LanguageString[MaxLanguageString];

// Loads all language strings from file
bool LoadLanguage(string Filename);



