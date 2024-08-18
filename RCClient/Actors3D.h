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
class GubbinTemplate;

// Gubbin joint names
extern string GubbinJoints[6];

// Other options
extern char HideNametags;
extern bool DisableCollisions;
extern int NametagFont;

void InvertScale(uint Child, uint Parent);
void LoadGubbinNames();
void SetActorWeapon(ActorInstance* AI, std::vector<GubbinTemplate*>* templates);
void SetActorShield(ActorInstance* AI, std::vector<GubbinTemplate*>* templates);
void SetActorChestArmour(ActorInstance* AI, std::vector<GubbinTemplate*>* templates);
void SetActorHat(ActorInstance* AI, std::vector<GubbinTemplate*>* templates);
bool LoadActorInstance3D(ActorInstance* A, float Scale = 1.0, bool SkipAttachments = false,  bool SkipCollisions = false);
void CreateActorNametag(ActorInstance* A);
void FreeActorInstance3D(ActorInstance* A);
void SafeFreeActorInstance(ActorInstance* A);
void ShowGubbins(ActorInstance* A);
void HideGubbins(ActorInstance* A);
void CreateEntityEmitters(int E);
void FreeEntityEmitters(int E);
void UpdateActorItems(ActorInstance* A);
void PlayActorSound(ActorInstance* A, int Speech);

