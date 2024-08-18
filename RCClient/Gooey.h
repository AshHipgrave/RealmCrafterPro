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

#include "Default Project.h"

// Misc
#define GY_Path string("Data\\UI")  // Change this if you move the Gooey folder

enum MessageBoxIcon
{
	MBI_None = 0,
	MBI_Warning = 1,
	MBI_Error = 2,
	MBI_Information = 3
};

// Input
extern bool GY_EnterHit;
extern bool GY_LeftClick, GY_RightClick;
extern float GY_MouseX, GY_MouseY;
extern int GY_MouseXSpeed, GY_MouseYSpeed;

// Sounds
extern uint GY_SClick, GY_SBeep;

// Functions
void GY_RecursiveEntityAlpha(uint EN, float A);
void GY_Load();
void GY_Update();

// Dialogs
void MessageBox(string Title, string Message, MessageBoxIcon Icon = MBI_None, EventHandler::CBFN ClickCallback = 0);
void MessageBoxYesNo(string Title, string Message, MessageBoxIcon Icon = MBI_None, EventHandler::CBFN OkCallback = 0, EventHandler::CBFN CancelCallback = 0);
