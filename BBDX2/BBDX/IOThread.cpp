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
#include "IOThread.h"
#include "..\LockableQueue.h"

// Thread Handles
DWORD IOThreadID = 0;
HANDLE IOThreadThread = NULL;

// File IO structure
IOThreadInfo::IOThreadInfo()
{
	Start = Length = 0;
	Buffer = 0;
	EventHandler = 0;
	Priority = 0;
	Tag = 0;
}

IOThreadInfo::~IOThreadInfo()
{
	if(Buffer != 0)
		delete[] Buffer;
	//if(EventHandler != 0)
	//	delete EventHandler;
}


LockableQueue<IOThreadInfo*> IOIn;
LockableQueue<IOThreadInfo*> IOOut;

// Start the IOThread
void IOThreadInit()
{
	IOThreadThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)IOThreadASyncInjectThread, NULL, 0, &IOThreadID);
}

// IOThread
void IOThreadASyncInjectThread()
{
	while(true)
	{
		if(IOIn.GetSize() > 0)
		{
			// Ask for any waiting 'orders'
			IOThreadInfo* Info = IOIn.Pop();
			if(Info == 0)
			{
				continue;
			}

			// Open file
			FILE* fp = fopen(Info->Path.c_str(), "rb");
			fseek(fp, 0, SEEK_END);
			long Length = ftell(fp) - Info->Start;
			if(Length > 0)
			{
				if(Length < Info->Length)
					Info->Length = Length;

				fseek(fp, Info->Start, SEEK_SET);
				Info->Buffer = new char[Length];
				fread(Info->Buffer, 1, Length, fp);
			}
			fclose(fp);

			// Order complete, send it back to the main thread
			IOOut.Push(Info);
		}
		else
		{
			Sleep(0);
		}
	}
}

bool IOThreadReadFile(const char* path, unsigned char priority, NGin::IEventHandler<void, IOThreadInfo>* eventHandler, void* tag)
{
	return IOThreadReadFileEx(path, priority, 0, 0xffffffff, eventHandler, tag);
}

bool IOThreadReadFileEx(const char* path, unsigned char priority, unsigned int start, unsigned int length, NGin::IEventHandler<void, IOThreadInfo>* eventHandler, void* tag)
{
	// Setup thread
	IOThreadInfo* Info = new IOThreadInfo();
	Info->Start = start;
	Info->Length = length;
	Info->EventHandler = eventHandler;
	Info->Path = path;
	Info->Priority = priority;
	Info->Tag = tag;

	// Add to list
	IOIn.Push(Info);
	return true;
}


// Called from UpdateWorld() to check for completed orders
void IOThreadASyncInjectCompleteUpdate()
{
	unsigned int TickStart = GetTickCount();

	// While there are orders left
	while(IOOut.GetSize() > 0)
	{
		IOThreadInfo* Info = IOOut.Pop();

		if(Info->EventHandler != 0)
		{
			Info->EventHandler->Execute(0, Info);
		}

		delete Info;
		
		// Get next order if there is time
		if(GetTickCount() - TickStart > 50)
			break;
	}

	char DBO[256];
	sprintf(DBO, "IO Thread: %i\n", IOIn.GetSize());
	StaticDebugStr(4, DBO);
}