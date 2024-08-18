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
#include "SDKMain.h"

namespace RealmCrafter
{
	class IItemButton : public NGin::GUI::IControl
	{
	public:

		IItemButton(NGin::Math::Vector2& ScreenScale, NGin::GUI::IGUIManager* Manager) : IControl(ScreenScale, Manager) {}
		virtual ~IItemButton() { };
		virtual bool Initialize() = 0;

		virtual float GetTimerValue() = 0;
		virtual void SetTimerValue(float timer) = 0;

		virtual unsigned short GetHeldItem() = 0;
		virtual unsigned short GetHeldSpell() = 0;
		virtual unsigned short GetHeldAmount() = 0;
		virtual std::string GetBackgroundImage() = 0;
		virtual void SetBackgroundImage(std::string path) = 0;
		virtual void SetHeldItem(unsigned short id, unsigned short amount) = 0;
		virtual void SetHeldSpell(unsigned short id) = 0;

		virtual bool GetDisplayOnly() = 0;
		virtual void SetDisplayOnly(bool display) = 0;

		virtual NGin::GUI::EventHandler* Click() = 0;
		virtual NGin::GUI::EventHandler* RightClick() = 0;
		virtual NGin::GUI::EventHandler* MouseEnter() = 0;
	};

	IItemButton* CreateItemButton(NGin::GUI::IGUIManager* manager, std::string name, NGin::Math::Vector2 location, NGin::Math::Vector2 size);
}