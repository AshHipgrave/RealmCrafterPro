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

class ActorInstance;

// Required animations
#define Anim_Walk          149
#define Anim_Run           148
#define Anim_SwimIdle      147
#define Anim_SwimSlow      146
#define Anim_SwimFast      145
#define Anim_RideIdle      144
#define Anim_RideWalk      143	
#define Anim_RideRun       142
#define Anim_DefaultAttack 141
#define Anim_RightAttack   140
#define Anim_TwoHandAttack 139
#define Anim_StaffAttack   138
#define Anim_DefaultParry  137
#define Anim_RightParry    136
#define Anim_TwoHandParry  135
#define Anim_StaffParry    134
#define Anim_ShieldParry   133
#define Anim_LastHit       132
#define Anim_FirstHit      130
#define Anim_LastDeath     129
#define Anim_FirstDeath    127
#define Anim_Jump          126
#define Anim_Idle          125
#define Anim_Yawn          124
#define Anim_LookRound     123
#define Anim_SitDown       122
#define Anim_SitIdle       121
#define Anim_StandUp       120
#define Anim_StrafeLeft    119
#define Anim_StrafeRight   118

#define Anim_RangedAttack 0

struct AnimSet
{
	string Name;
	int ID;
	string AnimName[150];
	int AnimStart[150], AnimEnd[150];
	float AnimSpeed[150];

	ClassDecl(AnimSet, AnimSetList, AnimSetDelete);
};
extern AnimSet* AnimList[1000];

void PlayAnimation(ActorInstance* AI, int Mode, float Speed, int Seq, bool FixedSpeed = true);
int CreateAnimSet();
int LoadAnimSets(string Filename);
bool SaveAnimSets(string Filename);
int FindAnimation(AnimSet* A, string AnimName);
int CurrentSeq(ActorInstance* AI);
