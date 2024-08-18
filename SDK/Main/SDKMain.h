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

// Externalized commands are C-specification for simple import
#define DLLOUT extern "C" __declspec(dllexport)

// Includes
#include <string>
#include "CPacketReader.h"
#include "CPacketWriter.h"
#include "BBDX.h"
#include "BB.h"
#include <IGUIManager.h>
#include "RealmCrafter.h"
#include "IControlLayout.h"
#include "ICameraController.h"
#include "IControlEditor.h"
#include "IEnvironment.h"
#include "IControlHostManager.h"
#include "IControlHost.h"
#include "IGameMenu.h"
#include "IPlayerActionBar.h"
#include "IPlayerInventory.h"
#include "IPlayerSpells.h"
#include "IMouseItem.h"
#include "IMouseToolTip.h"
#include "IItemButton.h"
#include "IPlayerAttributeBars.h"
#include "IPlayerMap.h"
#include "IPlayerCompass.h"
#include "IPlayerQuestLog.h"
#include "IPlayerStats.h"
#include "IPlayerHelp.h"
#include "IPlayerParty.h"

namespace std
{
	// Trim values using a bit of work (grr SC++L)
	inline std::string trim(const std::string &str)
	{
		std::string OutStr = "";
		size_t TrimStart = str.find_first_not_of(" \t");
		size_t TrimEnd = str.find_last_not_of(" \t");
		if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
			OutStr = "";
		else
			OutStr = str.substr(TrimStart, TrimEnd - TrimStart + 1);

		return OutStr;
	}

	// Convert a string to a float
	inline float toFloat(const std::string &str)
	{
		float Out = (float)atof(str.c_str());
		//std::stringstream(str) >> Out;
		return Out;
	}

	// Convert a string to an integer
	inline int toInt(const std::string &str)
	{
		int Out = atoi(str.c_str());
		//std::stringstream(str) >> Out;
		return Out;
	}

	// Mini converter class
	class miniConverter
	{
		std::string Output;
	public:

		miniConverter(std::string &str) { Output = str; }
		miniConverter(int i) { char T[32]; sprintf_s(T, "%i", i); Output = T; }
		miniConverter(unsigned int i) { char T[32]; sprintf_s(T, "%u", i); Output = T; }
		miniConverter(float i) { char T[32]; sprintf_s(T, "%f", i); Output = T; }

		std::string str() { return Output; }
	};

	// Convert an integer to a string
	template <typename T>
	inline std::string toString(T in)
	{
		return miniConverter(in).str();
	}

	// Convert a string to lower case
	inline std::string stringToLower(const std::string &str)
	{
		std::string Out = str;
		std::transform(Out.begin(), Out.end(), Out.begin(), ::tolower);
		return Out;
	}

	// Convert a string to upper case
	inline std::string stringToUpper(const std::string &str)
	{
		std::string Out = str;
		std::transform(Out.begin(), Out.end(), Out.begin(), ::toupper);
		return Out;
	}
}

// Exported Functions
#ifdef RC_SDK
	DLLOUT void SDKMain(void* bbdxCommandData, void* bbCommandData, uint camera, NGin::GUI::IGUIManager* guiManager);
#else
	typedef void (tSDKMainfn)(void* bbdxCommandData, void* bbCommandData, uint camera, NGin::GUI::IGUIManager* guiManager);
#endif


