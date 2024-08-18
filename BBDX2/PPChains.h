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

#include <d3dx9.h>
#include "CPPEffect.h"
#include "cRT.h"
#include "irrString.h"
#include "irrArray.h"

using namespace irr;
using namespace irr::core;

void PPInit(IDirect3DDevice9* Device, int Width, int Height);
void PPSize(int Width, int Height);
void PPReset();
ID3DXEffect* LoadEffect(const char* Name);
IDirect3DTexture9* LoadTexture(const char* Name);
ID3DXEffect* EffectFromID(int ID);
IDirect3DTexture9* TextureFromID(int ID);
void LoadChain(const char* CFile);
bool PreRenderChain();
void RenderChain(cRT* BackBuffer);
void SetPPEffectDirectory(stringc &Directory);
void SetPPEffectActive(stringc &Tag, bool Active);
bool GetPPEffectActive(stringc &Tag);
void TestPPEffect(const char* File);
const char* GetName();
const char* GetDescription();
int GetTextures();

