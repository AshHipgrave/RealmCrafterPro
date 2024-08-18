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
#include "IItemButton.h"
#include "IPlayerActionBar.h"

namespace RealmCrafter
{
	class CPlayerActionBar : public IPlayerActionBar
	{
		SActionBar InternalHandle;

		int CurrentPage;
		bool Invalidated;

		NGin::GUI::IGUIManager* GUIManager;
		IItemButton* Slots[12];
		NGin::GUI::IButton* NextPage, *PreviousPage;
		NGin::GUI::IPictureBox* XPEN, *ActionEN; // Experience progress bar
		NGin::GUI::IButton* BChat, *BMap, *BInventory, *BSpells, *BCharStats, *BQuestLog, *BParty, *BHelp;

		IItemButton* MouseOver;
		unsigned int MouseOverTimer;

		ActionBarEvent* ClickEvent;
		ActionBarEvent* RightClickEvent;


		void Slots_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void Slots_RightClick(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void Slots_MouseEnter(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void NextPage_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);
		void PreviousPage_Click(NGin::GUI::IControl* control, NGin::GUI::EventArgs* e);

		int GetSlot(IItemButton* button);

	public:

		CPlayerActionBar(NGin::GUI::IGUIManager* guiManager);
		virtual ~CPlayerActionBar();

		virtual void Initialize();
		virtual void Update(SActionBar* playerActionBar, NGin::Math::Vector2 &mousePosition);

		virtual void UpdateXPBar(char xpBarLevel);
		virtual void SetVisible(bool visible);
		virtual int GetPage();

		// This return event handlers invoked when associated buttons are pressed.
		virtual NGin::GUI::EventHandler* BChat_Click();
		virtual NGin::GUI::EventHandler* BMap_Click();
		virtual NGin::GUI::EventHandler* BInventory_Click();
		virtual NGin::GUI::EventHandler* BSpells_Click();
		virtual NGin::GUI::EventHandler* BCharStats_Click();
		virtual NGin::GUI::EventHandler* BQuestLog_Click();
		virtual NGin::GUI::EventHandler* BParty_Click();
		virtual NGin::GUI::EventHandler* BHelp_Click();

		virtual ActionBarEvent* Click();
		virtual ActionBarEvent* RightClick();
	};
}