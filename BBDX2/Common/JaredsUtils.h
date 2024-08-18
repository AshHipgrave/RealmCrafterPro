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
#ifndef __JAREDS_UTILS
#define __JAREDS_UTILS

#include <fstream>

#define GMB(x) MessageBox(NULL, x, "Debugger", MB_OK);

#include <windows.h>
#include <string>
#include <imagehlp.h>
#include <Strsafe.h>

int GenerateDump(_EXCEPTION_POINTERS* pExceptionPointers);
DWORD HandleGlobalException(_EXCEPTION_POINTERS* ExInfo);

extern "C" __declspec(dllexport) int DebuggerConnected();
extern "C" __declspec(dllexport) int StartRemoteDebugging(const char* address, int port);
extern "C" __declspec(dllexport) void StaticDebugStr(int id, const char* data);
extern "C" __declspec(dllexport) void SetDebugPrefix(const char* prefix);
extern "C" __declspec(dllexport) void StartDebugTimer(const char* name);
extern "C" __declspec(dllexport) void StopDebugTimer();
extern "C" __declspec(dllexport) void BeginDebugFrame();
extern "C" __declspec(dllexport) void EndDebugFrame();
extern "C" __declspec(dllexport) void DebugActor(int id, unsigned short sx, unsigned short sz, float x, float z, char me);

#define __J_MODULEVERSION "BBDX(2.51)";

//#define SEHSTART __try {
//#define SEHEND } __except (HandleGlobalException(GetExceptionInformation())) {exit(0);}
#define SEHSTART
#define SEHEND


#endif