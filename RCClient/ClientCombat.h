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
#include "BlitzPlus.h"

#include "Actors.h"
#include "Default Project.h"
#include "Animations.h"
#include "Media.h"
#include "Gooey Text.h"

extern int DamageInfoStyle;

class ActorInstance;

struct BloodSpurt
{
	int EmitterEN;
	int Timer;

	ClassDecl(BloodSpurt, BloodSpurtList, BloodSpurtDelete);
};

struct FloatingNumber
{
	NGin::GUI::ILabel* EN;
	float Lifespan;
	ActorInstance* AI;

	FloatingNumber();
	//ClassDecl(FloatingNumber, FloatingNumberList, FloatingNumberDelete);
};


void UpdateCombat();
void LoadCombat();
void AnimateActorAttack(ActorInstance* A);
void AnimateActorParry(ActorInstance* A);
void CombatDamageOutput(ActorInstance* AI, int Amount, string DType);
void CreateFloatingNumber(ActorInstance* AI, int Amount, int R, int G, int B);
bool UpdateFloatingNumber(FloatingNumber* F);
