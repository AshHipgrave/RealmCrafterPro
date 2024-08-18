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
#include "RealmCrafter.h"

namespace RealmCrafter
{
	// Resolve sectorvector
	const float SectorVector::SectorSize = 768.0f;

	SGlobals* Globals = NULL;

	namespace SDK
	{
		RCCommandData* RCCommands;
		NGin::GUI::GUIUpdateParameters* NGUIUpdateParameters = NULL;
	}

	int StartRCSDK(SDK::RCCommandData* rcCommandData, SGlobals* globals, int crtVersion)
	{
		if(crtVersion != _MSC_VER)
		{
			return -1;
		}

		SDK::RCCommands = rcCommandData;

		if(rcCommandData != NULL)
			SDK::NGUIUpdateParameters = rcCommandData->NGUIUpdateParameters;
		else
			SDK::NGUIUpdateParameters = NULL;

		Globals = globals;

		return 1;
	}
}