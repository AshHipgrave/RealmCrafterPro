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

#include <IServerSelector.h>

namespace RealmCrafter
{
	// Server Selector dialog
	class CServerSelector : public IServerSelector
	{
	public:
		
		// CTor
		CServerSelector(NGin::GUI::IGUIManager* guiManager);

		// DTor
		~CServerSelector();

		// Setup window GUI and load server names
		virtual void Initialize();

		// Show GUI window
		virtual void Show();

		// Get 'closed' event to update current connection
		virtual NGin::GUI::EventHandler* Closed() const;

		// Get whether this game has multiple realms
		virtual bool IsMultiServer();

		// Get current server data
		virtual std::string GetServerName();
		virtual std::string GetServerAddress();
		virtual int GetServerPort();

	private:

		NGin::GUI::IGUIManager* GUIManager;

		void CancelButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);
		void ConnectButton_Click(NGin::GUI::IControl* sender, NGin::GUI::EventArgs* e);

		int LastSelectedIndex;
		NGin::GUI::IWindow* SelectorWindow;
		NGin::GUI::IListBox* SelectorList;
		NGin::GUI::IButton* SelectorConnect;

		NGin::GUI::EventHandler* ClosedEvent;

		struct Selection { std::string Name; std::string Host; int Port; };
		std::vector<Selection*> Servers;

	};
}