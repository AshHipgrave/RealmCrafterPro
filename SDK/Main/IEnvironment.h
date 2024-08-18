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

#include <string>
#include <vector>

namespace RealmCrafter
{
	// Interface class for zone environments (skies and such)
	class IEnvironment
	{
	public:

		virtual ~IEnvironment() {}

		// Set the current time
		virtual void SetTime(int timeH, int timeM, float secondInterpolation) = 0;

		// Set the current date
		virtual void SetDate(int* day, int* year) = 0;

		// Loading a new zone
		virtual void LoadArea(std::string zoneName) = 0;

		// Unloading a zone
		virtual void UnloadArea() = 0;

		// Update environment frame
		virtual void Update(float deltaTime) = 0;

		// Sets whether the camera is underwater (process fogging here)
		virtual void SetCameraUnderWater(bool under, unsigned char destR, unsigned char destG, unsigned char destB) = 0;

		// Called to check whether a footstep sound effect is wet or dry.
		virtual bool PlayWetFootstep() = 0;

		// Get the name of this environment instance
		virtual std::string GetName() = 0;

		// Process a network property packet (for weather and such)
		virtual void ProcessNetPacket(RealmCrafter::PacketReader &reader) = 0;

		// Hide this environment (for GE tab switch)
		virtual void Hide() = 0;

		// Show this environment (for GE tab switch)
		virtual void Show() = 0;
	};
}