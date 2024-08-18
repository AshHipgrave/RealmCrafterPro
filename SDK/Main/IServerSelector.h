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
	//! Server Selector Dialog Interface
	class IServerSelector
	{
	public:

		// Setup internal GUI
		virtual void Initialize() = 0;

		// Show dialog
		virtual void Show() = 0;

		// Get 'Closed' EventHandler
		virtual NGin::GUI::EventHandler* Closed() const = 0;

		// Check to see if this game has multiple 'realms'
		virtual bool IsMultiServer() = 0;

		// Get the currently selected server name
		virtual std::string GetServerName() = 0;

		// Get the current server address
		virtual std::string GetServerAddress() = 0;

		// Get the current server port
		virtual int GetServerPort() = 0;
	};
}