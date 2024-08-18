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
#include "stdafx.h"
#include "SDKNet.h"


#include <SDKMain.h>
#include <NGinString.h>
#include <vcclr.h>

//#include <BlitzPlus.h>

	//extern "C" __declspec(dllexport) void* GetBBCommandData();


using namespace System;

typedef void (tChangeEnvironmentfn)(std::string environmentName, std::string zoneName);
typedef void (tSetEnvTimefn)(int timeH, int timeM, float secondInterpolation);
typedef void (tSetEnvDatefn)(int* day, int* year);
typedef void (tUpdateEnvironmentfn)(float deltaTime);
typedef void (tSetEnvCameraUnderWaterfn)(bool under, unsigned char destR, unsigned char destG, unsigned char destB);
//typedef void (tProcessEnvNetPacketfn)(RealmCrafter::PacketReader &reader);
typedef bool (tEnvPlayWetFootstepfn)();
typedef int (tStartRCSDKfn)(RealmCrafter::SDK::RCCommandData* rcCommandData, RealmCrafter::SGlobals* globals, int crtVersion);
typedef void (tEnvShowfn)();
typedef void (tEnvHidefn)();
extern tChangeEnvironmentfn* ChangeEnvironment;
extern tSetEnvTimefn* SetEnvTime;
extern tSetEnvDatefn* SetEnvDate;
extern tUpdateEnvironmentfn* UpdateEnvironment;
extern tSetEnvCameraUnderWaterfn* SetEnvCameraUnderWater;
//extern tProcessEnvNetPacketfn* ProcessEnvNetPacket;
extern tEnvPlayWetFootstepfn* EnvPlayWetFootstep;
extern tStartRCSDKfn* StartRCSDK;
extern tSDKMainfn* SDKMain;
extern tEnvShowfn* EnvShow;
extern tEnvHidefn* EnvHide;

void Load(const char* path);

namespace RealmCrafter
{
	SGlobals* Globals = NULL;
}

extern "C" __declspec(dllexport) void* GetBBCommandData();
//void* GetBBDXCommandData();

namespace SDKNet
{

	void SDKInvoker::Start(System::String^ path, int bbdxData, uint camera, int guiManager)
	{
		pin_ptr<const wchar_t> pinnedPath = PtrToStringChars(path);
		NGin::WString cPath(pinnedPath);

		Load(cPath.AsCString().c_str());


		RealmCrafter::Globals = new RealmCrafter::SGlobals();
		RealmCrafter::Globals->AlwaysRun = false;
		RealmCrafter::Globals->SpeedStat = 0;
		RealmCrafter::Globals->StrengthStat = 0;
		RealmCrafter::Globals->HealthStat = 0;
		RealmCrafter::Globals->EnergyStat = 0;
		RealmCrafter::Globals->BreathStat = 0;
		RealmCrafter::Globals->AttackTarget = false;
		RealmCrafter::Globals->LastZoneLoad = 0;
		RealmCrafter::Globals->GameName = "GameName";
		RealmCrafter::Globals->ServerHost = "ServerHost";
		RealmCrafter::Globals->ServerPort = 0;
		RealmCrafter::Globals->AreaName = "AreaName";
		RealmCrafter::Globals->QuitActive = false;
		RealmCrafter::Globals->Money1 = "Money1";
		RealmCrafter::Globals->Money2 = "Money2";
		RealmCrafter::Globals->Money3 = "Money3";
		RealmCrafter::Globals->Money4 = "Money4";
		RealmCrafter::Globals->Money2x = 0;
		RealmCrafter::Globals->Money3x = 0;
		RealmCrafter::Globals->Money4x = 0;
		RealmCrafter::Globals->LastAttack = 0;
		RealmCrafter::Globals->CombatDelay = 0;
		RealmCrafter::Globals->FogR = 0;
		RealmCrafter::Globals->FogG = 0;
		RealmCrafter::Globals->FogB = 0;
		RealmCrafter::Globals->FogNear = 500.0f;
		RealmCrafter::Globals->FogFar = 1000.0f;
		RealmCrafter::Globals->DefaultVolume = 1.0f;
		RealmCrafter::Globals->TimerManager = NULL;

		if(StartRCSDK(NULL, RealmCrafter::Globals, _MSC_VER) == -1)
		{
			char DBO[2048];
			sprintf_s(DBO, "Could not load SDK.DLL: StartSDK failed _MSC_VER comparison, should have been '%i'", _MSC_VER);
			System::String^ ErrString = gcnew System::String((const char*)DBO);
			System::Windows::Forms::MessageBox::Show(ErrString, "Runtime Error");
			exit(0);
		}

		void* bbCommands = GetBBCommandData();
		void* bbdxCommands = reinterpret_cast<void*>(bbdxData);
		SDKMain(bbdxCommands, bbCommands, camera, reinterpret_cast<NGin::GUI::IGUIManager*>(guiManager));
	}

	void SDKInvoker::ChangeEnvironment(System::String^ environmentName, System::String^ zoneName)
	{
		pin_ptr<const wchar_t> pinnedEnvName = PtrToStringChars(environmentName);
		NGin::WString cEnvName(pinnedEnvName);

		pin_ptr<const wchar_t> pinnedZoneName = PtrToStringChars(zoneName);
		NGin::WString cZoneName(pinnedZoneName);

		::ChangeEnvironment(cEnvName.AsCString().c_str(), cZoneName.AsCString().c_str());
	}

	void SDKInvoker::SetEnvTime(int TimeH, int TimeM, float SecondInterp)
	{
		::SetEnvTime(TimeH, TimeM, SecondInterp);
	}

	void SDKInvoker::UpdateEnvironment(float deltaTime)
	{
		::UpdateEnvironment(deltaTime);
	}

	void SDKInvoker::FogChange(float fogNear, float fogFar)
	{
		RealmCrafter::Globals->FogNear = fogNear;
		RealmCrafter::Globals->FogFar = fogFar;
	}

	void SDKInvoker::EnvHide()
	{
		::EnvHide();
	}

	void SDKInvoker::EnvShow()
	{
		::EnvShow();
	}
}
