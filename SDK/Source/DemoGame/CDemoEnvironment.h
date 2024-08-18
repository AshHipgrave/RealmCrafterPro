//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include "SDKMain.h"
#include <vector3.h>

namespace RealmCrafter
{
	class CDemoEnvironment : public IEnvironment
	{

		uint SkyEN, CloudEN;

		int Year, Day, TimeH, TimeM, TimeFactor;

		float SecondInterp;

		uint SunEN, MoonEN;
		uint Light0, Light1;

		int TimeUpdate; 
		bool HourChanged, MinuteChanged;
		bool FogChange;

		float FogNearDest, FogFarDest, FogNearNow, FogFarNow;
		bool CameraUnderwater;

		uint Camera;

		// Linear Interpolate
		float Lerp(float A, float B, float T);
		float Clamp(float value, float min, float max);

		// Returns the delta (in minutes) between two times
		int TimeDelta(int StartH, int StartM, int EndH, int EndM);

		// Sets the view distance
		// Used on zone load (reset), underwater and fog weather.
		void SetViewDistance(float Near, float Far, bool ForceSkyChange);

	public:

		CDemoEnvironment(uint camera);
		virtual ~CDemoEnvironment();

		virtual void SetTime(int timeH, int timeM, float secondInterpolation);
		virtual void SetDate(int* day, int* year);

		virtual void LoadArea(std::string zoneName);
		virtual void UnloadArea();

		virtual void Update(float deltaTime);

		virtual void SetCameraUnderWater(bool under, unsigned char destR, unsigned char destG, unsigned char destB);
		virtual bool PlayWetFootstep();

		virtual std::string GetName();

		virtual void ProcessNetPacket(RealmCrafter::PacketReader &reader);

		virtual void Hide();
		virtual void Show();
	};
}