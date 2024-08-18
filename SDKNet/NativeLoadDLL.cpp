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
#include "Stdafx.h"
#include <windows.h>
#include <string>
#include <RealmCrafter.h>

typedef void (tChangeEnvironmentfn)(std::string environmentName, std::string zoneName);
typedef void (tSetEnvTimefn)(int timeH, int timeM, float secondInterpolation);
typedef void (tSetEnvDatefn)(int* day, int* year);
typedef void (tUpdateEnvironmentfn)(float deltaTime);
typedef void (tSetEnvCameraUnderWaterfn)(bool under, unsigned char destR, unsigned char destG, unsigned char destB);
//typedef void (tProcessEnvNetPropertyfn)(std::string propertyName, std::string propertyString);
typedef bool (tEnvPlayWetFootstepfn)();
typedef int (tStartRCSDKfn)(RealmCrafter::SDK::RCCommandData* rcCommandData, RealmCrafter::SGlobals* globals, int);
typedef void (tEnvShowfn)();
typedef void (tEnvHidefn)();
tChangeEnvironmentfn* ChangeEnvironment;
tSetEnvTimefn* SetEnvTime;
tSetEnvDatefn* SetEnvDate;
tUpdateEnvironmentfn* UpdateEnvironment;
tSetEnvCameraUnderWaterfn* SetEnvCameraUnderWater;
//tProcessEnvNetPropertyfn* ProcessEnvNetProperty;
tEnvPlayWetFootstepfn* EnvPlayWetFootstep;
tStartRCSDKfn* StartRCSDK = NULL;
tSDKMainfn* SDKMain = NULL;
HMODULE SDKLib = NULL;
tEnvShowfn* EnvShow = NULL;
tEnvHidefn* EnvHide = NULL;

void RuntimeError(const char* message)
{
	MessageBoxA(NULL, message, "SDK Invoker Error", MB_OK | MB_ICONERROR);
	exit(0);
}

void Load(const char* path)
{
	SDKLib = LoadLibraryA(path);
	if(SDKLib == NULL)
		RuntimeError("Could not load DLL: SDK.dll");

	StartRCSDK = (RealmCrafter::tStartRCSDKfn*)GetProcAddress(SDKLib, "StartRCSDK");
	if(StartRCSDK == NULL)
		RuntimeError("Could not find SDK InitPoint: StartRCSDK(ptr)");

	SDKMain = (tSDKMainfn*)GetProcAddress(SDKLib, "SDKMain");
	if(SDKMain == NULL)
		RuntimeError("Could not find SDK EntryPoint: SDKMain(ptr, ptr, uint)");

	ChangeEnvironment = (tChangeEnvironmentfn*)GetProcAddress(SDKLib, "ChangeEnvironment");
	if(ChangeEnvironment == NULL)
		RuntimeError("Could not find SDK Export: ChangeEnvironment()");

	SetEnvTime = (tSetEnvTimefn*)GetProcAddress(SDKLib, "SetEnvTime");
	if(SetEnvTime == NULL)
		RuntimeError("Could not find SDK Export: SetEnvTime()");

	SetEnvDate = (tSetEnvDatefn*)GetProcAddress(SDKLib, "SetEnvDate");
	if(SetEnvDate == NULL)
		RuntimeError("Could not find SDK Export: SetEnvDate()");

	UpdateEnvironment = (tUpdateEnvironmentfn*)GetProcAddress(SDKLib, "UpdateEnvironment");
	if(UpdateEnvironment == NULL)
		RuntimeError("Could not find SDK Export: UpdateEnvironment()");

	SetEnvCameraUnderWater = (tSetEnvCameraUnderWaterfn*)GetProcAddress(SDKLib, "SetEnvCameraUnderWater");
	if(SetEnvCameraUnderWater == NULL)
		RuntimeError("Could not find SDK Export: SetEnvCameraUnderWater()");

// 	ProcessEnvNetProperty = (tProcessEnvNetPropertyfn*)GetProcAddress(SDKLib, "ProcessEnvNetProperty");
// 	if(ProcessEnvNetProperty == NULL)
// 		RuntimeError("Could not find SDK Export: ProcessEnvNetProperty()");

	EnvPlayWetFootstep = (tEnvPlayWetFootstepfn*)GetProcAddress(SDKLib, "EnvPlayWetFootstep");
	if(EnvPlayWetFootstep == NULL)
		RuntimeError("Could not find SDK Export: EnvPlayWetFootstep()");

	EnvShow = (tEnvShowfn*)GetProcAddress(SDKLib, "EnvShow");
	if(EnvShow == NULL)
		RuntimeError("Could not find SDK Export: EnvShow()");

	EnvHide = (tEnvHidefn*)GetProcAddress(SDKLib, "EnvHide");
	if(EnvHide == NULL)
		RuntimeError("Could not find SDK Export: EnvHide()");
}