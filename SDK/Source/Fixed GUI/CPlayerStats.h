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

#include <IGUIManager.h>
#include <vector>
#include "IPlayerStats.h"

namespace RealmCrafter
{
	class CPlayerStats : public IPlayerStats
	{
		NGin::GUI::IGUIManager* GUIManager;

		NGin::GUI::IWindow* Window;

		NGin::GUI::ILabel* NameLabel;
		NGin::GUI::ILabel* LevelLabel;
		NGin::GUI::ILabel* XPLabel;
		NGin::GUI::ILabel* GoldLabel;
		NGin::GUI::ILabel* ReputationLabel;
		
		std::vector<NGin::GUI::ILabel*> AttributeNames;
		std::vector<NGin::GUI::ILabel*> AttributeStats;

	public:

		CPlayerStats(NGin::GUI::IGUIManager* guiManager);
		virtual ~CPlayerStats();

		virtual void Initialize();
		virtual void Update(SDK::IActorInstance* actorInstance);
		
		virtual void SetVisible(bool visible);
		virtual bool GetVisible();

		virtual bool IsActiveWindow();
		virtual NGin::GUI::EventHandler* Closed();
	};
}