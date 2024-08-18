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
#include "DebugFrameProfiler.h"
#include <windows.h>
#include <Strsafe.h>

namespace
{
	bool Capturing = false;
	
	int CaptureFrames = 60;
	int FramesCaptured = 0;
	LARGE_INTEGER CounterFrequency;


	FP_Task* Head = NULL;
	FP_Task* Current = NULL;

	void DestroyTree( FP_Task* head )
	{
		for( int i = 0; i < head->numChildren; ++i )
		{
			DestroyTree( head->children[i] );
		}

		delete head;
	}

	FP_Task* CreateTree( const char* name, FP_Task* parent )
	{
		FP_Task* head = new FP_Task();

		memset( head, 0, sizeof(FP_Task) );

		memcpy( head->taskName, name, strlen(name) );
		head->parentTask = parent;

		if( parent != NULL )
		{
			parent->children[ parent->numChildren++ ] = head;
		}

		return head;
	}

	FP_Task* GetOrCreate( const char* name, FP_Task* parent )
	{
		if( parent == NULL )
			return NULL;

		for( int i = 0; i < parent->numChildren; ++i )
		{
			if( strcmp( name, parent->children[i]->taskName ) == 0 )
			{
				return parent->children[i];
			}
		}

		return CreateTree( name, parent );
	}

	void FixParts( FP_Task* task, float globalLen )
	{
		if( globalLen < 0.001f )
			return;

		task->timeGlobal /= (float)FramesCaptured;

		if( task->parentTask != NULL )
		{
			task->percGlobal = task->timeGlobal / globalLen;
			task->percGroup = task->timeGlobal / task->parentTask->timeGlobal;
		}
		else
		{
			task->percGlobal = 1.0f;
			task->percGroup = 1.0f;
		}

		for(int i = 0; i < task->numChildren; ++i)
		{
			FixParts( task->children[i], globalLen );
		}
	}
}

void FP_Init()
{
	// Get counter frequency
	QueryPerformanceFrequency( &CounterFrequency );
	CounterFrequency.QuadPart /= 1000;
}

FP_Task* FP_GetGraph()
{
	return Head;
}

int FP_IsRunning()
{
	return Capturing ? 1 : 0;
}

void FP_SetCaptureFrames( int frames )
{
	CaptureFrames = frames;
}

int FP_GetCaptureFrames()
{
	return CaptureFrames;
}

int FP_GetFramesCaptured()
{
	return FramesCaptured;
}

void FP_BeginCapture()
{
	if( Head != NULL )
	{
		DestroyTree( Head );
	}

	Head = CreateTree( "Frame", NULL );
	Current = Head;

	FramesCaptured = 0;
	Capturing = true;
}

void FP_DumpTask( FP_Task* task, int nest )
{
	char DO[512];

	for(int i = 0; i < nest * 4; ++i)
		DO[i] = ' ';

	sprintf(DO + nest * 4, "%s\n", task->taskName);
	OutputDebugStringA(DO);

	for(int i = 0; i < task->numChildren; ++i)
		FP_DumpTask( task->children[i], nest + 1 );
}

void FP_FrameSync()
{
	if( !Capturing )
		return;

	++FramesCaptured;
	if( FramesCaptured == CaptureFrames )
	{
		Capturing = false;

		if( Head != Current )
		{
			OutputDebugStringA("\nFP Perf Stack\n");
			OutputDebugStringA("----------------------\n");
			FP_DumpTask(Head, 0);
			OutputDebugStringA("----------------------\n");
			OutputDebugStringA("Current: ");

			if( Current != NULL )
				OutputDebugStringA(Current->taskName);
			else
				OutputDebugStringA("NULL");
			OutputDebugStringA("\n");
			OutputDebugStringA("----------------------\n");
			

			MessageBoxA( NULL, "Error: FP_FrameSync() ended with an odd perf stack", "Fatal Error", MB_OK );
			exit(0);
		}

		for( int i = 0; i < Head->numChildren; ++i )
		{
			Head->timeGlobal += Head->children[i]->timeGlobal;
		}

		FixParts( Head, (float)Head->timeGlobal / (float)FramesCaptured );
	}
}

void FP_PushTask( const char* name )
{
	if( !Capturing )
		return;

	Current = GetOrCreate( name, Current );

	QueryPerformanceCounter( &Current->timer );
}

void FP_PopTask()
{
	if( !Capturing )
		return;

	if( Current != NULL )
	{
		LARGE_INTEGER endTimer;
		QueryPerformanceCounter( &endTimer );

		Current->timeGlobal += (float)(( endTimer.QuadPart - Current->timer.QuadPart ) / CounterFrequency.QuadPart );

		Current = Current->parentTask;
	}else
	{
		OutputDebugStringA("FP_PopTask() failed, perf stack corrupt!\n");
	}
}
