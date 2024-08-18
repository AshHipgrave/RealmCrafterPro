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
/*************************************************
* Frame Profiler for Debug Overlay
*
* Jared Belkus
*
**************************************************/

#ifndef __FRAMEPROFILER_H
#define __FRAMEPROFILER_H

#include "ASyncJobModule.h"
#include <windows.h>

struct FP_Task
{
	FP_Task* parentTask;

	char taskName[128];
	float timeGlobal;
	float percGlobal;
	float percGroup;
	LARGE_INTEGER timer;

	int numChildren;
	FP_Task* children[32];
};

void FP_Init();
int FP_IsRunning();
int FP_GetFramesCaptured();
void FP_SetCaptureFrames( int frames );
int FP_GetCaptureFrames();
FP_Task* FP_GetGraph();

void FP_BeginCapture();
void FP_FrameSync();

void FP_PushTask( const char* name );
void FP_PopTask( );

#endif // __FRAMEPROFILER_H