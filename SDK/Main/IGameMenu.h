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

#include <SDKMain.h>

namespace RealmCrafter
{
	//! Ingame Menu Dialog Interface
	class IGameMenu
	{
	public:

		// Setup window GUI
		virtual void Initialize() = 0;

		// Show GUI window
		virtual void Show() = 0;

		// Hide GUI window
		virtual void Hide() = 0;

		// Get whether the menu is visible
		virtual bool IsVisible() = 0;

		// Get callback invoked when 'Settings' is clicked
		virtual NGin::GUI::EventHandler* OptionsClick() const = 0;

		// Get callback invoked when a logout is required
		virtual NGin::GUI::EventHandler* Logout() const = 0;

		// Get callback invoked when an exit is required
		virtual NGin::GUI::EventHandler* Quit() const = 0;

		// Get callback invoked when the quit time starts (to halt movement)
		virtual NGin::GUI::EventHandler* LogoutTimerStart() const = 0;
	};
}